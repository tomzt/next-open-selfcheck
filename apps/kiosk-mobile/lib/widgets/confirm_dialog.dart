// SPDX-License-Identifier: GPL-3.0-or-later
// Bookdrop Confirm Dialog — Shows result of book return
// Displays: success (green) or error (red) with localized messages

import 'package:flutter/material.dart';
import '../services/bookdrop_listener.dart';

class BookdropConfirmDialog extends StatefulWidget {
  final BookdropCheckinResult result;

  const BookdropConfirmDialog({
    Key? key,
    required this.result,
  }) : super(key: key);

  @override
  State<BookdropConfirmDialog> createState() => _BookdropConfirmDialogState();
}

class _BookdropConfirmDialogState extends State<BookdropConfirmDialog> with SingleTickerProviderStateMixin {
  late AnimationController _animationController;

  @override
  void initState() {
    super.initState();
    _animationController = AnimationController(
      duration: const Duration(milliseconds: 600),
      vsync: this,
    );
    _animationController.forward();

    // Play sound feedback (success beep or error beep)
    _playAudio();
  }

  @override
  void dispose() {
    _animationController.dispose();
    super.dispose();
  }

  void _playAudio() {
    // TODO: Implement audio playback
    // - Success: beep (short tone, C major)
    // - Error: warning sound (2x beep, lower tone)
    // Use audioplayers package: https://pub.dev/packages/audioplayers
    // final audioPlayer = AudioPlayer();
    // if (widget.result.success) {
    //   audioPlayer.play(AssetSource('sounds/success.mp3'));
    // } else {
    //   audioPlayer.play(AssetSource('sounds/error.mp3'));
    // }
  }

  @override
  Widget build(BuildContext context) {
    final isSuccess = widget.result.success;
    final bgColor = isSuccess ? Colors.green[50] : Colors.red[50];
    final borderColor = isSuccess ? Colors.green[700] : Colors.red[700];
    final iconColor = isSuccess ? Colors.green[700] : Colors.red[700];

    return Container(
      color: bgColor,
      child: Center(
        child: ScaleTransition(
          scale: Tween<double>(begin: 0.3, end: 1.0).animate(
            CurvedAnimation(parent: _animationController, curve: Curves.elasticOut),
          ),
          child: Card(
            elevation: 8,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(24),
              side: BorderSide(color: borderColor!, width: 4),
            ),
            margin: const EdgeInsets.all(32),
            child: Padding(
              padding: const EdgeInsets.all(48),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  // Large icon (success: checkmark, error: X)
                  Icon(
                    isSuccess ? Icons.check_circle : Icons.cancel,
                    size: 120,
                    color: iconColor,
                  ),
                  const SizedBox(height: 32),

                  // Title (Thai)
                  Text(
                    isSuccess ? '✓ สำเร็จแล้ว' : '✗ เกิดข้อผิดพลาด',
                    style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                          fontWeight: FontWeight.bold,
                          color: iconColor,
                        ),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 12),

                  // Message (Thai)
                  Text(
                    widget.result.message,
                    style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                          color: Colors.grey[700],
                        ),
                    textAlign: TextAlign.center,
                  ),

                  if (widget.result.title != null) ...[
                    const SizedBox(height: 24),
                    Container(
                      padding: const EdgeInsets.all(16),
                      decoration: BoxDecoration(
                        color: Colors.grey[100],
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Column(
                        children: [
                          Text(
                            'Book Title',
                            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                  color: Colors.grey[600],
                                ),
                          ),
                          const SizedBox(height: 8),
                          Text(
                            widget.result.title!,
                            style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                                  fontWeight: FontWeight.bold,
                                ),
                            textAlign: TextAlign.center,
                          ),
                        ],
                      ),
                    ),
                  ],

                  if (widget.result.alert) ...[
                    const SizedBox(height: 16),
                    Container(
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: Colors.orange[100],
                        borderRadius: BorderRadius.circular(12),
                        border: Border.all(color: Colors.orange[700]!),
                      ),
                      child: Row(
                        children: [
                          Icon(Icons.warning, color: Colors.orange[700]),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Text(
                              'Alert: Check item details with staff',
                              style: Theme.of(context).textTheme.bodySmall?.copyWith(
                                    color: Colors.orange[900],
                              ),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],

                  const SizedBox(height: 32),

                  // English subtitle
                  Text(
                    widget.result.messageEn,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: Colors.grey[500],
                        ),
                    textAlign: TextAlign.center,
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
