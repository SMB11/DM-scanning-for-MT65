import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:csv/csv.dart';
import 'package:mailer/mailer.dart';
import 'package:mailer/smtp_server.dart';
import 'package:path_provider/path_provider.dart';
import 'dart:io';

void main() {
  runApp(MyApp());
}

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'MT65 Scanner App',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: ScannerScreen(),
    );
  }
}

class ScannerScreen extends StatefulWidget {
  @override
  _ScannerScreenState createState() => _ScannerScreenState();
}

class _ScannerScreenState extends State<ScannerScreen> {
  final List<String> _scannedCodes = List.filled(12, '');
  int _currentCodeIndex = 0;
  bool _scanComplete = false;

  @override
  void initState() {
    super.initState();
    _captureScannerInput();
  }

  @override
  void dispose() {
    RawKeyboard.instance.removeListener(_handleKeyEvent);
    super.dispose();
  }

  void _captureScannerInput() {
    RawKeyboard.instance.addListener(_handleKeyEvent);
  }

  void _handleKeyEvent(RawKeyEvent event) {
    if (event is RawKeyDownEvent && !_scanComplete) {
      setState(() {
        // Handle case sensitivity
        if (event.data is RawKeyEventDataAndroid) {
          RawKeyEventDataAndroid data = event.data as RawKeyEventDataAndroid;
          String keyLabel = event.data.keyLabel;
          if (data.metaState & RawKeyEventDataAndroid.modifierShift != 0) {
            keyLabel = keyLabel.toUpperCase();
          } else {
            keyLabel = keyLabel.toLowerCase();
          }

          _scannedCodes[_currentCodeIndex] += keyLabel;
        }

        // Assuming Enter key indicates the end of a scan for a single code
        if (event.data.logicalKey == LogicalKeyboardKey.enter) {
          String scannedCode = _scannedCodes[_currentCodeIndex];

          // Check for duplicates
          if (_scannedCodes.contains(scannedCode) && _scannedCodes.indexOf(scannedCode) != _currentCodeIndex) {
            _showDuplicateSnackbar();
            _scannedCodes[_currentCodeIndex] = ''; // Clear the current code
          } else {
            _currentCodeIndex++;
            if (_currentCodeIndex >= 12) {
              _scanComplete = true;
            }
          }
        }
      });
    }
  }

  void _showDuplicateSnackbar() {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(
        content: Text('Duplicate code detected. Ignored.'),
        duration: Duration(seconds: 2),
      ),
    );
  }

  void _clearScannedCodes() {
    setState(() {
      for (int i = 0; i < _scannedCodes.length; i++) {
        _scannedCodes[i] = '';
      }
      _currentCodeIndex = 0;
      _scanComplete = false;
    });
  }

  Future<void> _generateCsvAndSendEmail() async {
    List<List<dynamic>> rows = [];
    for (var code in _scannedCodes) {
      rows.add([code]);
    }

    String csvData = const ListToCsvConverter(fieldDelimiter: ',', textDelimiter: '').convert(rows);

    final directory = await getApplicationDocumentsDirectory();
    final path = "${directory.path}/scanned_codes.csv";
    final file = File(path);
    await file.writeAsString(csvData);

    final smtpServer = gmail('waterkok8@gmail.com', 'jnxw psrt ckjz bacx');

    final message = Message()
      ..from = Address('waterkok8@gmail.com', 'Waterlok')
      ..recipients.add('waterkok8@gmail.com')
      ..subject = 'Scanned DataMatrix Codes'
      ..text = 'Please find the attached CSV file containing the scanned DataMatrix codes.'
      ..attachments.add(FileAttachment(file));

    try {
      final sendReport = await send(message, smtpServer);
      print('Message sent: ' + sendReport.toString());

      // Clear scanned codes
      _clearScannedCodes();

      // Show success notification
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('CSV file sent successfully.'),
          duration: Duration(seconds: 2),
        ),
      );
    } catch (e) {
      print('Message not sent.');
      print(e.toString());

      // Show error notification
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Failed to send CSV file.'),
          duration: Duration(seconds: 2),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text('MT65 Scanner'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            Expanded(
              child: ListView.builder(
                itemCount: _scannedCodes.length,
                itemBuilder: (context, index) {
                  return Padding(
                    padding: const EdgeInsets.symmetric(vertical: 4.0),
                    child: TextField(
                      controller: TextEditingController(text: _scannedCodes[index]),
                      readOnly: true,
                      decoration: InputDecoration(
                        border: OutlineInputBorder(),
                        labelText: 'Scanned Code ${index + 1}',
                      ),
                    ),
                  );
                },
              ),
            ),
            SizedBox(height: 20),
            Row(
              children: [
                ElevatedButton(
                  onPressed: _clearScannedCodes,
                  child: Text('Clear'),
                ),
                SizedBox(width: 20),
                if (_scanComplete)
                  ElevatedButton(
                    onPressed: _generateCsvAndSendEmail,
                    child: Text('Send CSV by Email'),
                  ),
              ],
            ),
          ],
        ),
      ),
    );
  }
}
