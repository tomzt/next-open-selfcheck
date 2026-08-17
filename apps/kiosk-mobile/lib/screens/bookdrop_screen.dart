// SPDX-License-Identifier: GPL-3.0-or-later
// Bookdrop Return Station Screen
// Displays UI feedback as patron drops book into return chute with RFID reader

import 'package:flutter/material.dart';
import '../services/bookdrop_listener.dart';
import '../widgets/confirm_dialog.dart';

enum BookdropState {
  welcome,     // Ready for patron to drop book
  listening,   // Waiting for RFID tag detection
  processing,  // Sending checkin request to backend
  confirm,     // Show result (success or error)
}

class BookdropScreen extends StatefulWidget {
  const BookdropScreen({Key? key}) : super(key: key);

  @override
  State<BookdropScreen> createState() => _BookdropScreenState();
}

class _BookdropScreenState extends State<BookdropScreen> {
  late BookdropListener _listener;
  BookdropState _state = BookdropState.welcome;
  late BookdropCheckinResult _lastResult;

  @override
  void initState() {
    super.initState();
    // Backend-centric configuration
    // Backend URL: read from environment or app config
    // TODO: Move to app config/env
    const backendUrl = 'http://192.168.1.100:3000'; // Central server
    const deviceId = 'bookdrop-01'; // Physical location ID
    const jwtToken = null; // TODO: Get from auth provider if needed

    _listener = BookdropListener(
      backendUrl: backendUrl,
      deviceId: deviceId,
      jwtToken: jwtToken,
      onTagDetected: _onTagDetected,
      onError: _onError,
    );
  }

  @override
  void dispose() {
    _listener.dispose();
    super.dispose();
  }

  void _onTagDetected(String tagId) async {
    setState(() => _state = BookdropState.processing);

    try {
      final result = await _listener.submitBookdropCheckin(tagId);
      setState(() {
        _lastResult = result;
        _state = BookdropState.confirm;
      });

      // Auto-return to welcome screen after 3 seconds
      Future.delayed(const Duration(seconds: 3), () {
        if (mounted) {
          setState(() {
            _state = BookdropState.welcome;
            _listener.startListening(); // Resume listening
          });
        }
      });
    } catch (e) {
      setState(() {
        _lastResult = BookdropCheckinResult.error(e.toString());
        _state = BookdropState.confirm;
      });
    }
  }

  void _onError(String errorMsg) {
    setState(() {
      _lastResult = BookdropCheckinResult.error(errorMsg);
      _state = BookdropState.confirm;
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey[50],
      body: switch (_state) {
        BookdropState.welcome => _buildWelcomeScreen(),
        BookdropState.listening => _buildListeningScreen(),
        BookdropState.processing => _buildProcessingScreen(),
        BookdropState.confirm => _buildConfirmScreen(),
      },
    );
  }

  Widget _buildWelcomeScreen() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.local_library, size: 120, color: Colors.blue[700]),
          const SizedBox(height: 24),
          Text(
            'ยินรับสินค้าคืน',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                  color: Colors.blue[700],
                ),
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 16),
          Text(
            'Please drop your book into the return slot',
            style: Theme.of(context).textTheme.bodyLarge,
            textAlign: TextAlign.center,
          ),
          const SizedBox(height: 48),
          ElevatedButton.icon(
            onPressed: () {
              setState(() {
                _state = BookdropState.listening;
                _listener.startListening();
              });
            },
            icon: const Icon(Icons.arrow_forward),
            label: const Text('Start'),
            style: ElevatedButton.styleFrom(
              padding: const EdgeInsets.symmetric(horizontal: 32, vertical: 16),
              textStyle: const TextStyle(fontSize: 18),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildListeningScreen() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          const SizedBox(
            width: 100,
            height: 100,
            child: CircularProgressIndicator(strokeWidth: 8),
          ),
          const SizedBox(height: 32),
          Text(
            'กำลังรอการตรวจสอบ...',
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  color: Colors.blue[700],
                ),
          ),
          const SizedBox(height: 12),
          Text(
            'Waiting for book...',
            style: Theme.of(context).textTheme.bodyLarge,
          ),
        ],
      ),
    );
  }

  Widget _buildProcessingScreen() {
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          const SizedBox(
            width: 80,
            height: 80,
            child: CircularProgressIndicator(strokeWidth: 6),
          ),
          const SizedBox(height: 24),
          Text(
            'กำลังประมวลผล...',
            style: Theme.of(context).textTheme.headlineSmall,
          ),
        ],
      ),
    );
  }

  Widget _buildConfirmScreen() {
    return BookdropConfirmDialog(result: _lastResult);
  }
}
