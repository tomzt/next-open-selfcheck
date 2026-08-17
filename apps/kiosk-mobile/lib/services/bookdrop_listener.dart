// SPDX-License-Identifier: GPL-3.0-or-later
// Bookdrop Listener — Backend-Centric Event Polling
//
// Architecture: Tablet polls central backend for RFID tag events
// Backend daemon reads Feig RD5200 on server side (no direct WiFi to tablet)
//
// Authentication: Tablet authenticates to backend (JWT token)
// No RD5200 WiFi auth needed — fully handles by central server
//
// Benefits:
// - Works across any org WiFi (Eduroam, WPA2-Enterprise, captive portal)
// - No RD5200 WiFi config per site
// - Scalable: multiple tablets polling same backend
// - Offline resilience: backend caches events

import 'package:http/http.dart' as http;
import 'dart:convert';

/// Checkin result from backend /api/bookdrop/checkin
class BookdropCheckinResult {
  final bool success;
  final String itemBarcode;
  final String? title;
  final String message; // Thai localized
  final String messageEn; // English localized
  final bool alert;

  BookdropCheckinResult({
    required this.success,
    required this.itemBarcode,
    this.title,
    required this.message,
    required this.messageEn,
    this.alert = false,
  });

  factory BookdropCheckinResult.fromJson(Map<String, dynamic> json) {
    return BookdropCheckinResult(
      success: json['success'] ?? false,
      itemBarcode: json['itemBarcode'] ?? '',
      title: json['title'],
      message: json['message'] ?? '',
      messageEn: json['message_en'] ?? json['message'] ?? '',
      alert: json['alert'] ?? false,
    );
  }

  factory BookdropCheckinResult.error(String errorMsg) {
    return BookdropCheckinResult(
      success: false,
      itemBarcode: '',
      message: 'เกิดข้อผิดพลาด: $errorMsg',
      messageEn: 'Error: $errorMsg',
    );
  }
}

typedef TagDetectedCallback = void Function(String barcode);
typedef ErrorCallback = void Function(String error);

/// Backend-Centric Event Listener
/// Polls central server for RFID tag detections from RD5200 daemon
class BookdropListener {
  // Configuration: Read from environment or config
  final String backendUrl; // e.g., http://192.168.1.100:3000
  final String deviceId; // e.g., 'bookdrop-01'
  final String? jwtToken; // Optional: JWT bearer token for auth

  late TagDetectedCallback onTagDetected;
  late ErrorCallback onError;

  bool _isListening = false;
  late Future<void> _pollingLoop;

  BookdropListener({
    required this.backendUrl,
    required this.deviceId,
    required this.onTagDetected,
    required this.onError,
    this.jwtToken,
  });

  /// Start polling backend for RFID tag events
  /// Polls /api/bookdrop/events?deviceId=bookdrop-01 every 100ms
  ///
  /// Flow:
  /// 1. Tablet polls: GET /api/bookdrop/events?deviceId=bookdrop-01
  /// 2. Backend daemon caches tag events from RD5200
  /// 3. Backend returns: { events: [{ barcode, timestamp }] }
  /// 4. Tablet processes tag → calls /api/bookdrop/checkin
  /// 5. Resume polling for next tag
  Future<void> startListening() async {
    if (_isListening) return;
    _isListening = true;

    // Start polling loop (runs in background)
    _pollingLoop = _pollLoop();
  }

  /// Polling loop: check every 100ms for new tags
  /// Uses HTTP polling for simplicity; can upgrade to WebSocket later
  Future<void> _pollLoop() async {
    while (_isListening) {
      try {
        await _pollBackendForTags();
      } catch (e) {
        print('[BookdropListener] Polling error: $e');
        onError('Connection error: $e');
      }

      // Poll interval: 100ms (10 Hz)
      // Latency: ~100-200ms from tag detection to tablet UI
      await Future.delayed(const Duration(milliseconds: 100));
    }
  }

  /// Poll backend /api/bookdrop/events for pending tag events
  /// Returns: { deviceId, events: [ { barcode, timestamp } ], ... }
  Future<void> _pollBackendForTags() async {
    final url = Uri.parse('$backendUrl/api/bookdrop/events').replace(
      queryParameters: {'deviceId': deviceId},
    );

    final headers = {
      'Accept': 'application/json',
      'User-Agent': 'bookdrop-tablet/1.0',
    };

    // Add JWT token if provided (for protected backend)
    if (jwtToken != null) {
      headers['Authorization'] = 'Bearer $jwtToken';
    }

    try {
      final response = await http.get(url, headers: headers).timeout(
            const Duration(seconds: 5),
          );

      if (response.statusCode == 200) {
        final data = jsonDecode(response.body);
        final events = data['events'] as List? ?? [];

        // Process each tag event
        for (final event in events) {
          final barcode = event['barcode'] as String?;
          if (barcode != null && barcode.isNotEmpty) {
            // Notify UI of new tag
            onTagDetected(barcode);
          }
        }
      } else if (response.statusCode == 401) {
        // Authentication failed
        onError('Authentication failed. Please login again.');
        stopListening();
      } else if (response.statusCode >= 500) {
        // Server error — log but continue polling
        print('[BookdropListener] Server error ${response.statusCode}');
      }
    } on Exception catch (e) {
      // Network error — continue polling, will retry
      print('[BookdropListener] Network error: $e');
    }
  }

  /// Submit book return to backend
  /// Backend will call SIP2 checkin and return item details
  Future<BookdropCheckinResult> submitBookdropCheckin(String itemBarcode) async {
    final url = Uri.parse('$backendUrl/api/bookdrop/checkin');

    final headers = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      'User-Agent': 'bookdrop-tablet/1.0',
    };

    if (jwtToken != null) {
      headers['Authorization'] = 'Bearer $jwtToken';
    }

    try {
      final response = await http
          .post(
            url,
            headers: headers,
            body: jsonEncode({
              'itemBarcode': itemBarcode,
              'deviceId': deviceId,
            }),
          )
          .timeout(const Duration(seconds: 10));

      if (response.statusCode == 200 || response.statusCode == 503) {
        final data = jsonDecode(response.body);
        return BookdropCheckinResult.fromJson(data);
      } else if (response.statusCode == 401) {
        return BookdropCheckinResult.error('Authentication failed');
      } else {
        return BookdropCheckinResult.error('HTTP ${response.statusCode}');
      }
    } catch (e) {
      return BookdropCheckinResult.error(e.toString());
    }
  }

  /// Stop polling for RFID tags
  void stopListening() {
    _isListening = false;
  }

  /// Dispose resources
  void dispose() {
    stopListening();
  }
}
