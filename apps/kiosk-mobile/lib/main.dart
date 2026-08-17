// SPDX-License-Identifier: GPL-3.0-or-later
// Next-open-selfcheck Kiosk Mobile App (Flutter)
// Supports: Kiosk, Workstation, Bookdrop modes

import 'package:flutter/material.dart';
import 'screens/bookdrop_screen.dart';

// App mode selector (from environment or command-line args)
enum AppMode {
  kiosk,      // Patron self-check terminal
  workstation, // Staff manual operations
  bookdrop,   // Automated return station
}

void main() {
  // TODO: Read APP_MODE from environment:
  // - flutter run --dart-define=APP_MODE=bookdrop
  const appMode = AppMode.bookdrop; // Default: Bookdrop for this implementation

  runApp(MyApp(mode: appMode));
}

class MyApp extends StatelessWidget {
  final AppMode mode;

  const MyApp({
    Key? key,
    required this.mode,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Library Kiosk',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(
          seedColor: Colors.blue,
          brightness: Brightness.light,
        ),
        useMaterial3: true,
        fontFamily: 'Sarabun',
      ),
      home: switch (mode) {
        AppMode.kiosk => const Placeholder(fallbackHeight: double.infinity), // TODO: Implement KioskScreen
        AppMode.workstation => const Placeholder(fallbackHeight: double.infinity), // TODO: Implement WorkstationScreen
        AppMode.bookdrop => const BookdropScreen(),
      },
    );
  }
}
