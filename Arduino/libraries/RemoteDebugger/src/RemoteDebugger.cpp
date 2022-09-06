/*
 * Libraries Arduino
 * *****************
 * Library : RemoteDebugger: An Simple Software debuggger, addon for RemoteDebug library
 * Author  : Joao Lopes
 * Comments: Routines to the simple software debugger, based on SerialDebug library.
 *           This not is a C++ class, to keep compatibility with SerialDebug
 *
 * Versions:
 *  ------	----------	-----------------
 *  0.9.4	2019-03-16  Adjustments on example
 *  0.9.3	2019-03-13	Support to RemoteDebug with connection by web socket
 *                      debugBreak for RemoteDebug now is redirect to a debugSilence of RemoteDebug
 *  0.9.2	2019-03-04	Adjustments in example
 *  0.9.1	2019-03-01	Adjustment: the debugger still disable until dbg command, equal to SerialDebug
 *                      Changed to one debugHandleDebugger routine
 *                      Add debugSetDebuggerEnabled routine
 *                      Changed handle debugger logic
 *
 *	0.9.0 	2019-02-28	Beta 1
 *
 */

/*
 * Source for RemoteDebugger
 *
 * Copyright (C) 2018  Joao Lopes
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, version 3.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * This file contains the code for debugger addon to RemoteDebug library.
 *
 */

/*
 * TODO list:
 * - Do this code is same for RemoteDebug and SerialDebug
 * - see all warnings
 * - more optimizations to speed
 * - more optimizations to reduce memory and program for low memory boards (UNO, etc.)
 * - more types
 * - gpio commands
 */

/*
 * TODO known issues:
 * - Error on use debug* macros with F()
 */

/////// Includes

// Incude for this module

#include "RemoteDebugger.h"

// Disable debug - good for release (production)

#ifndef DEBUG_DISABLED

// Disable debugger ? No more commands and features as functions and globals

#ifndef DEBUG_DISABLE_DEBUGGER

#include <Arduino.h>

#ifndef _STDARG_H // To avoid error in Arduino IDE 1.8.5 and Linux - thanks to @wd5gnr
	#include <stdarg.h>
#endif

// Library

#ifdef DEBUGGER_FOR_SERIALDEBUG

#include "SerialDebug.h"

#else // RemoteDebug

#ifdef SERIAL_DEBUG_H // Cannot used with SerialDebug at same time
#error "RemoteDebugger cannot be used with SerialDebug"
#endif

#include "RemoteDebug.h"

// ESP8266 or ESP32 ?

#if defined(ESP8266)
#include <ESP8266WiFi.h>
#elif defined(ESP32)
#include <WiFi.h>
#else
#error Only for ESP8266 or ESP32
#endif

#endif

// Utilities

#include "utility/Util.h"
#include "utility/Fields.h"

#ifdef DEBUGGER_FOR_SERIALDEBUG
#include "utility/Boards.h"
#endif

#ifdef BOARD_LOW_MEMORY
// Warning to low memory MCUs
#warning "Debugger on low memory MCU is still not yet full optimized - use with caution"
#endif

// Vector (to reduce memory and no fixed limit)

// Arduino arch have C++ std vector ?

#if defined ESP8266 || defined ESP32 || defined _GLIBCXX_VECTOR

	#define VECTOR_STD true

	// C++ std vector

	#include <vector>

	using namespace std;

#else // This Arduino arch is not compatible with std::vector

	// Using a lightweight Arduino_Vector library: https://github.com/zacsketches/Arduino_Vector/
	// Adapted and otimized by JoaoLopesF

	#include <utility/Vector.h>

#endif

//////// Defines

// Version

#define DEBUGGER_VERSION "0.9.4"

// Low memory board ?

#if defined BOARD_LOW_MEMORY && !(defined DEBUG_NOT_USE_FLASH_F)
	#define DEBUG_USE_FLASH_F true
#endif

/////// Variables - public

#ifdef DEBUGGER_FOR_SERIALDEBUG
bool _debugSilence = false;						// Silent mode ?
#endif
//DEBUGGER_VAR_TYPE unsigned long _debugLastTime = millis(); 			// Last time show a debug

DEBUGGER_VAR_TYPE uint8_t _debugFunctionsAdded = 0;					// Number of functions added

DEBUGGER_VAR_TYPE uint8_t _debugGlobalsAdded = 0;						// Number of globals added

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

DEBUGGER_VAR_TYPE uint8_t _debugWatchesAdded = 0;						// Number of watches added
DEBUGGER_VAR_TYPE boolean _debugWatchesEnabled = false;				// Watches is enabled (only after add any)?

#endif

DEBUGGER_VAR_TYPE boolean _debugDebuggerEnabled = false;				// Simple Software Debugger enabled ?

/////// Variables - private

#ifdef DEBUGGER_FOR_REMOTEDEBUG

static RemoteDebug *_Debug;							// Instance of RemoteDebug
//static WiFiClient *_TelnetClient;					// Telnet client

#endif

//static String *_command; 							// Command received

// Type of global or function arg

typedef enum {

	DEBUG_TYPE_BOOLEAN,									// Basic types
	DEBUG_TYPE_CHAR,
	DEBUG_TYPE_INT,
	DEBUG_TYPE_U_LONG,

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

	DEBUG_TYPE_BYTE,
	DEBUG_TYPE_U_INT,
	DEBUG_TYPE_LONG,

	DEBUG_TYPE_FLOAT,
	DEBUG_TYPE_DOUBLE,

	DEBUG_TYPE_INT8_T,									// Integers size _t
	DEBUG_TYPE_INT16_T,
	DEBUG_TYPE_INT32_T,
//#ifdef ESP32
//		DEBUG_TYPE_INT64_T,
//#endif
	DEBUG_TYPE_UINT8_T,									// Unsigned integers size _t
	DEBUG_TYPE_UINT16_T,
	DEBUG_TYPE_UINT32_T,
//#ifdef ESP32
//		DEBUG_TYPE_UINT64_T
//#endif

#endif

	DEBUG_TYPE_CHAR_ARRAY,								// Strings
	DEBUG_TYPE_STRING,

	DEBUG_TYPE_FUNCTION_VOID,							// For function void

	DEBUG_TYPE_UNDEFINED								// Not defined

} debugEnumTypes_t;

// Debug functions

struct debugFunction_t {
	const char* name = 0;							// Name
#ifdef DEBUG_USE_FLASH_F
	const __FlashStringHelper *nameF = 0;			// Name (in flash)
#endif
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
	const char* description = 0;					// Description
#ifdef DEBUG_USE_FLASH_F
	const __FlashStringHelper *descriptionF = 0;	// Description (in flash)
#endif
#endif
	void (*callback)() = 0;							// Callbacks
	uint8_t argType = 0;							// Type of argument
};

#ifdef VECTOR_STD  // Arduino arch have C++ std vector

DEBUGGER_VAR_TYPE vector<debugFunction_t> _debugFunctions;		// Vector array of functions

#else // Using a Arduino_Vector library: https://github.com/zacsketches/Arduino_Vector/

DEBUGGER_VAR_TYPE Vector<debugFunction_t> _debugFunctions;		// Vector array of functions

#endif

// Debug global variables

struct debugGlobal_t {
	const char* name = 0;							// Name
#ifdef DEBUG_USE_FLASH_F
	const __FlashStringHelper *nameF = 0;			// Name (in flash)
#endif
	uint8_t type = 0;								// Type of variable (see enum below)
	void *pointer = 0;								// Generic pointer
	uint8_t showLength = 0;							// To show only a part (strings)
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
	const char* description = 0;					// Description
#ifdef DEBUG_USE_FLASH_F
	const __FlashStringHelper *descriptionF = 0;	// Description (in flash)
#endif
#endif
	uint8_t typeOld = 0;							// Type of old value variable (used to strings)
	void *pointerOld = 0;							// Generic pointer for old value
	boolean changed = false;						// Value change (between 2 debug handle call)
	boolean updateOldValue = false;					// Update old value ? (in debug handle call)
};

#ifdef VECTOR_STD  // Arduino arch have C++ std vector

DEBUGGER_VAR_TYPE vector<debugGlobal_t> _debugGlobals;		// Vector array of globals

#else // Using a Arduino_Vector library: https://github.com/zacsketches/Arduino_Vector/

DEBUGGER_VAR_TYPE Vector<debugGlobal_t> _debugGlobals;		// Vector array of globals

#endif

typedef enum {
		DEBUG_SHOW_GLOBAL, 							// For globals
		DEBUG_SHOW_GLOBAL_WATCH,					// For watches
		DEBUG_SHOW_GLOBAL_APP_CONN					// For SerialDebugApp connection
} debugEnumShowGlobais_t;

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

// Watches

struct debugWatch_t {
	uint8_t globalNum;								// Global id
	uint8_t operation;								// Operation
	uint8_t typeValue = 0;							// Type of old value variable (used to strings)
	void *pointerValue = 0;							// Generic pointer to value (to do operation)
	boolean watchCross = false;						// Is a cross watch ?
	uint8_t globalNumCross = 0;						// To watch cross - compare two globals
	boolean enabled = true;							// Enabled ?
	boolean triggered = false;						// Triggered ? (operation -> true)
	boolean alwaysStop = false;						// Always stop, even _debugWatchStop = false
};

#ifdef VECTOR_STD  // Arduino arch have C++ std vector

DEBUGGER_VAR_TYPE vector<debugWatch_t> _debugWatches;		// Vector array of watches

#else // Using a Arduino_Vector library: https://github.com/zacsketches/Arduino_Vector/

DEBUGGER_VAR_TYPE 	Vector<debugWatch_t> _debugWatches;		// Vector array of watches

#endif

static boolean _debugWatchStop = true;				// Causes a stop in debug for any positive watches ?

static int8_t addWatch(Fields& fields);

#endif

#ifndef DEBUG_MINIMUM

	// Connection with App ? e.g. SerialDebugApp

	static boolean _debugApp = false;

	// String helper, used to store name extracted from Flash and receive commands
	// Unified this 2 usages, to reduce memory

	static String _debugString = "";

	// Last command

	static String _debugLastCommand = "";

	// Repeat last command (in each debugHandler)

	static boolean _debugRepeatCommand = false;

	// To show help (uses PROGMEM)
	// Note: Using PROGMEM in large string (even for Espressif boards)

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

   static const char debugHelp[] PROGMEM = \
"\
*\r\n\
* Debugger Commands:\r\n\
*   r -> repeat last command (in each debugHandle)\r\n\
*      r ? -> to show more help \r\n\
*\r\n\
*   f -> call the function\r\n\
*      f ?  -> to show more help \r\n\
*\r\n\
*   dbg [on|off] -> enable/disable the simple software debugger\r\n\
*   Only if debugger is enabled: \r\n\
*      g -> see/change global variables\r\n\
*         g ?  -> to show more help \r\n\
*      wa -> see/change watches for global variables\r\n\
*         wa ?  -> to show more help \r\n\
*\r\n\
*   Not yet implemented:\r\n\
*      gpio -> see/control gpio\r\n\
*";

#else // Low memory board

   static const char debugHelp[] PROGMEM = \
"\
*\r\n\
* Debugger Commands:\r\n\
*   r -> repeat last command (in each debugHandle)\r\n\
*      r ? -> to show more help \r\n\
*\r\n\
*   f -> call the function\r\n\
*      f ?  -> to show more help \r\n\
*   dbg [on|off] -> enable/disable the simple software debugger\r\n\
*\r\n\
*   dbg [on|off] -> enable/disable the simple software debugger\r\n\
*   Only if debugger is enabled: \r\n\
*      g -> see/change global variables\r\n\
*         g ?  -> to show more help \r\n\
*\r\n\
*   Not yet implemented:\r\n\
*      gpio -> see/control gpio\r\n\
*";

#endif

   /////// Prototypes - private

	// Note: only public functions start with debug...

	static void processCommand(String& command, boolean repeating = false, boolean showError = true);
#ifdef DEBUGGER_FOR_SERIALDEBUG
	static void showHelp();
#endif
	// For functions

	static int8_t addFunction(const char* name, uint8_t argType);
#ifdef DEBUG_USE_FLASH_F
	static int8_t addFunction(const __FlashStringHelper *name, uint8_t argType);
#endif

	static void processFunctions(String& options);
	static int8_t showFunctions(String& options, boolean one, boolean debugSerialApp = false);
	static void callFunction(String& options);

	// For globais

	static int8_t addGlobal(const char* name,  void* pointer, uint8_t type, uint8_t showLength);
#ifdef DEBUG_USE_FLASH_F
	static int8_t addGlobal(const __FlashStringHelper* name, void* pointer, uint8_t type, uint8_t showLength);
#endif

	static void processGlobals(String& options);
	static int8_t showGlobals(String& options, boolean one, boolean debugSerialApp = false);
	static boolean showGlobal(uint8_t globalNum, debugEnumShowGlobais_t mode, boolean getLastNameF);
	static void changeGlobal(Fields& fields);

	static boolean findGlobal (const char* globalName, uint8_t* globalNum, boolean sumOne = true);
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
	static boolean verifyGlobalType(uint8_t globalNum, uint8_t type);
#endif

	static void removeQuotation(String& string, boolean single);

	// For void* pointerValue values

	static void getStrValue(uint8_t type, void* pointer, uint8_t showLength, boolean showSize,  String& response, String& responseType);
	static void updateValue(uint8_t typeFrom, void* pointerFrom, uint8_t typeTo, void** pointerTo);
	static boolean apllyOperation(uint8_t type1, void* pointer1, uint8_t operation, uint8_t type2, void* pointer2);

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

	// For watches

	static int8_t addWatch(uint8_t globalNum, uint8_t operation, boolean allwaysStop);

	static void processWatches(String& options);
	static void processWatchesAction(Fields& fields);
	static int8_t showWatches(String& options, boolean debugSerialApp = false);
	static boolean showWatch(uint8_t watchNum, boolean debugSerialApp = false);
	static int8_t addWatchCross(Fields& fields);
	static boolean changeWatch(Fields& fields);
	static boolean getWatchOperation(String str, uint8_t* operation);

#endif

#endif // DEBUG_MINIMUM

static void printLibrary();

/////// Prototypes - private

//#ifndef DEBUG_MINIMUM
//static void debugAppConnection();
//#endif // DEBUG_MINIMUM

/////// Defines (private)

#ifndef DEBUG_USE_FLASH_F // Not using flash to string and functions (good for board with memory  - more faster)
	#ifdef BOARD_ENOUGH_MEMORY
		#define DEBUG_NOT_USE_FLASH_F true
	#endif
#endif
#ifdef DEBUG_NOT_USE_FLASH_F // Not using Flash F, only string in RAM (more fast)
	#undef F
 	#define F(str) str
#endif

// Internal printf (no time, auto func, etc, just print it)

#ifndef DEBUG_MINIMUM

#ifdef DEBUG_USE_NATIVE_PRINTF // For Espressif boards, have PRINTFf

#ifdef DEBUGGER_FOR_SERIALDEBUG
	#define PRINTF(fmt, ...)   PRINTF(fmt, ##__VA_ARGS__)
	#define PRINTFLN(fmt, ...) PRINTF(fmt "\r\n", ##__VA_ARGS__)
    #define PRINTLN() 		   PRINTF("\r\n")
#else // RemoteDebug
	#define PRINTF(fmt, ...)   if (_Debug) {_Debug->showRaw(true);_Debug->printf(fmt, ##__VA_ARGS__);_Debug->showRaw(false);} // TODO: put ANY isactive
	#define PRINTFLN(fmt, ...) if (_Debug) {_Debug->showRaw(true);_Debug->printf(fmt, ##__VA_ARGS__);_Debug->println();_Debug->showRaw(false);}
	#define PRINTLN() 		   if (_Debug) {_Debug->showRaw(true);_Debug->println();_Debug->showRaw(false);}
#endif

#else // Use debugPrintf

	#define PRINTF(fmt, ...)   debugPrintf(false, ' ', " ", fmt, ##__VA_ARGS__)
	#define PRINTFLN(fmt, ...) debugPrintf(true, ' ', " ", fmt, ##__VA_ARGS__)

#endif

#endif // DEBUG_MINIMUM

// Debug internal, all must commented on release

#define D(fmt, ...)
//#define D(fmt, ...) PRINTF("rdbg: " fmt "\r\n", ##__VA_ARGS__)
//#define D(fmt, ...) Serial.printf("rdbg: " fmt "\r\n", ##__VA_ARGS__)

/////// Methods

// Init the debugger

#ifdef DEBUGGER_FOR_SERIALDEBUG
void debugInitDebugger (HardwareSerial *serial, void (*callbackHelp)(), void (*callbackProcessCmd)(),
						boolean *debuggerActive) {
	// TODO: in future do both lib share this same code
}
#else // RemoteDebug
void debugInitDebugger (RemoteDebug *Debug) {

	// Init the debugger

	_Debug = Debug; // Instance of RemoteDebug

//	_TelnetClient = _Debug->getTelnetClient(); // Instance of Telnet client
}
#endif

////// Handles

#ifdef DEBUGGER_FOR_SERIALDEBUG
// Silence

void debugSilence(boolean activate, boolean showMessage, boolean fromBreak) {

	_debugSilence = activate;

	//D("silence %d", _debugSilence);

	if (_debugSilence && showMessage) {

		printLibrary();
		PRINTFLN(F("Debug now is in silent mode"));
		printLibrary();
		PRINTFLN(F("Press enter or another command to return show debugs"));

	} else if (!_debugSilence && showMessage) {

		printLibrary();
		PRINTFLN(F("Debug now exit from silent mode"));
	}

#ifndef DEBUG_MINIMUM
	if (_debugApp && !fromBreak) { // For DebugSerialApp connection ?

		// Send status

		PRINTFLN(F("$app:S:%c"), ((activate)? '1':'0'));
	}
#endif // DEBUG_MINIMUM

}
#endif // DEBUGGER_FOR_SERIALDEBUG

// Print name of library in begin for messages
// Note: It is done to reduce Program memory consupmition - for low memory boards support

void printLibrary() {

#ifdef DEBUGGER_FOR_SERIALDEBUG
	PRINTF(F("* SerialDebug: "));
#else // RemoteDebug
	PRINTF(F("* RemoteDebugger: "));
#endif
}

// For receipt of commands to processs

//// Process command received to debugger

void debugProcessCmdDebugger() {

	D("dggger");
#ifdef DEBUGGER_FOR_REMOTEDEBUG
	if (_Debug) {
		String command = _Debug->getLastCommand();

		D("cmd dbgger: ", command.c_str());

		processCommand(command, false, true);
	}
#endif
}

// Get help for debugger

String debugGetHelpDebugger() {

	String help = "\r\n";
#ifdef DEBUGGER_FOR_REMOTEDEBUG
	help.concat("* Debugger for RemoteDebug");
	help.concat("\r\n");
#endif
	help.concat(F("* Debugger version: "));
	help.concat(DEBUGGER_VERSION);
	help.concat("\r\n");
	help.concat(debugHelp); // Using PROGMEM in large strings (even for Espressif boards)
	help.concat("\r\n");

#ifdef DEBUGGER_FOR_REMOTEDEBUG
	help.replace("*", "    *"); // Indent this
//	Serial.println("getHelp");
#endif

	return help;
}

// Debugger is enabled now ?

boolean debugGetDebuggerEnabled() {

	return _debugDebuggerEnabled;
}

// Debugger set enabled (true/false)

void debugSetDebuggerEnabled(boolean enabled) {

	_debugDebuggerEnabled = enabled;
}

// Not for minimum mode - to save memory

#ifndef DEBUG_MINIMUM

// debugBreak - show a message and wait for response

#ifndef BOARD_LOW_MEMORY // Not for low memory boards
String debugBreak(String str, uint32_t timeout) {
	return debugBreak(str.c_str(), timeout);
}
#endif

String debugBreak() {

	return debugBreak("", DEBUG_BREAK_TIMEOUT, false);

}
String debugBreak(const __FlashStringHelper *ifsh, uint32_t timeout, boolean byWatch) {

	return debugBreak(String(ifsh).c_str(), timeout, byWatch);
}

String debugBreak(const char* str, uint32_t timeout, boolean byWatch) {

	D("debugBreak - timeout %u", timeout);

	// Show a message

	char appBreakType = '1';

	if (strlen(str) > 0) { // String is informed ?

		printLibrary();
		PRINTFLN(str);

	} else if (timeout == DEBUG_BREAK_TIMEOUT) { // Is called by debugBreak() and not for watches ?

		printLibrary();
#ifdef DEBUGGER_FOR_SERIALDEBUG
		PRINTFLN(F("Press any key or command, and enter to continue"));
#else
		if (_Debug && ((_Debug->isConnected()))) {
			PRINTFLN(F("Send any command or press the \"Silence\" button, to continue."));
		} else {
			PRINTFLN(F("Press any command, and enter to continue"));
		}
#endif
		appBreakType = 'C';
	}

	if (_debugApp) { // For DebugSerialApp connection ?

		PRINTFLN(F("$app:B:%c:%c"), appBreakType, ((byWatch)?'W':' '));
	}

	// Response buffer

	String response = "";

	// Wait for response // Note: the Arduino will wait for it, to continue runs

	uint32_t lastTime = millis(); 			// Time of last receipt
	char last = ' ';						// Last char received

#ifdef DEBUGGER_FOR_SERIALDEBUG

	// Enter in silence (if is not yet)

	boolean oldSilence;

	oldSilence= _debugSilence;		// In silence mode ?

	if (!oldSilence) {
		debugSilence(true, false, true);
	}

	// Ignore buffer

	while (Serial.available()) {
		Serial.read();
	}

	// Process serial data (until timeout, if informed)

	while (timeout == 0 ||
			((millis() - lastTime) <= timeout)){

		if (Serial.available()) {

		    // Get the new char:

		    char character = (char)Serial.read();

		    // Clear buffer if is a long time of last receipt

		    if (response.length() > 0 && (millis() - lastTime) > 2000) {
		    	response = "";
		    }
		    lastTime = millis(); // Save it

			// Newline (CR or LF) - once one time if (\r\n)

			if (isCRLF(character) == true) {

				if (isCRLF(last) == false) { // New line -> return the command

					//D("break");

					break;
				}

			} else if (isPrintable(character)) { // Only valid

				// Concat

				response.concat(character);
			}

			// Last char

			last = character;
		}

		delay(10); // Give a time
	}

	if (_debugApp) { // For DebugSerialApp connection ?

		PRINTFLN(F("$app:B:0"));

	}

	// Is in silence ? (restore it)

	if (_debugSilence && !oldSilence) {
		debugSilence(false, false, true);
	}

#else // RemoteDebug

	// Client connected ?

	if (_Debug) {

		if (!(_Debug->isConnected())) {
			return "";
		}

		// Break on RemoteDebug is only using silente mode and not returns nothing
		// TODO: improve this

		D("silence");
		_Debug->silence(true, false, true, timeout);
	}
#endif

	return response;
}


// Process command receipt by serial (Arduino monitor, etc.)

static void processCommand(String& command, boolean repeating, boolean showError) {

	// Reduce information on error to support low memory boards
	// Use a variable to reduce memory

#if DEBUG_USE_FLASH_F

	__FlashStringHelper* errorSintax = F("Invalid command. Use ? to show help");

#else

	const char* errorSintax = "Invalid command. Use ? to show help";

#endif

	D("Command: %s last: %s", command.c_str(), _debugLastCommand.c_str());

	// Disable repeating

	if (!repeating && _debugRepeatCommand) {
		_debugRepeatCommand = false;
	}

	// Can repeat ?

	boolean canRepeat = false;

	// Extract options

	String options = "";
	int16_t pos = command.indexOf(' ');
	if (pos > 0) {
		options = command.substring(pos + 1);
		command = command.substring(0, pos);
	}

	// Invalid

	if (command.length() > DEBUG_MAX_SIZE_COMMANDS) {
		printLibrary();
		PRINTFLN(errorSintax);
		return;
	}

	if (options.length() > DEBUG_MAX_SIZE_CMD_OPTIONS) {
		printLibrary();
		PRINTFLN(errorSintax);
		return;
	}

	// Show command

	if (command != "$app") {
		printLibrary();
		PRINTFLN(F("Command recv.: %s opt: %s"), command.c_str(), options.c_str());
	}

	// Verify if pass value between '"' - to avoid split the value in fields logic

	boolean inMark = false;
	char conv = 31;

	for (uint8_t i=0; i<options.length(); i++) {

		char c = options.charAt(i);

		if (c == '"') {

			inMark = !inMark;

		} else {

			if (c == ' ' && inMark) {
				options.setCharAt(i, conv);
			}
		}
	}

	// Process the command

	//D("opt -> %s", options.c_str());

	if (command == "dbg") {

#ifdef DEBUGGER_FOR_SERIALDEBUG
		// Help ?

		if (options == "h" || options == "help" || options == "?") {

			// Show help

			showHelp();

			// Do a break

			String response = debugBreak("", DEBUG_BREAK_TIMEOUT, true);

			if (response.length() > 0) { // Process command

				processCommand(response, false);
			}

			return;
		}
#endif

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

		// Enable/disable the Simple Software Debugger

		boolean saveWatchStop = _debugWatchStop;

		if (_debugApp) { // For DebugSerialApp connection ?

			_debugWatchStop = false; // disable it for now

		}
#endif

		// Globals or functions added ?

		if (_debugGlobalsAdded == 0 && _debugFunctionsAdded == 0) {
			printLibrary();
			PRINTFLN(F("No globals or functions added"));
			return;
		}

		// Process

		if (options == "on") {
			_debugDebuggerEnabled = true;
		} else if (options == "off") {
			_debugDebuggerEnabled = false;
		} else {
			_debugDebuggerEnabled = !_debugDebuggerEnabled; // invert it
		}

		// Process handle to update globals

		debugHandleDebugger(true);

		// Is from app ?

		if (_debugApp) { // For Debug App connection ?

			// Debugger enabled ?

#ifdef DEBUGGER_FOR_SERIALDEBUG
			if (_debugDebuggerEnabled) {

				// Send debugger elements

				printLibrary();
				PRINTFLN(F("Sending debugger objects ..."));

				// Send info

				PRINTFLN(F("$app:D:")); // To clean arrays

				String all="";

				if (_debugFunctionsAdded > 0) {
					showFunctions(all, false, true);
				}

				if (_debugGlobalsAdded > 0) {
					showGlobals(all, false, true);
				}

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

				if (_debugWatchesAdded > 0) {
					showWatches(all, true);
				}

#endif // BOARD_LOW_MEMORY

				printLibrary();
				PRINTFLN(F("End of sending."));

			}
#else // RemoteDebug

			// TODO: sending not is necessary, due app not have a screen for debugger
#endif

			// Send status

			PRINTFLN(F("$app:D:%u"), _debugDebuggerEnabled);

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

			// Restore stop

			_debugWatchStop = saveWatchStop;
#endif // Not low memory board


		}

		printLibrary();
		PRINTFLN(F("Simple software debugger: %s"), (_debugDebuggerEnabled) ? "On":"Off");

	} else if (command == "f") {

		// Process

		processFunctions(options);

		canRepeat = true;
#else
		printDebugger();
		PRINTFLN(F("Debug functions is not enabled in your project"));

#endif
	} else if (command == "g") {

#ifndef DEBUG_DISABLE_DEBUGGER // Only if debugger is enabled

		// Process

		if (_debugDebuggerEnabled) {

			processGlobals(options);

			canRepeat = true;

		} else {

			printLibrary();
			PRINTFLN(F("Debugger is not enabled, please command dbg to enable this"));
		}

#else
		printDebugger();
		PRINTFLN(F("Debug functions is not enabled in your project"));

#endif

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

	} else if (command == "wa") {

#ifndef DEBUG_DISABLE_DEBUGGER // Only if debugger is enabled

		// Process

		if (_debugDebuggerEnabled) {

			processWatches(options);

		} else {

			printLibrary();
			PRINTFLN(F("Debugger is not enabled, please command dbg to enable this"));
		}

#else
		printDebugger();
		PRINTFLN(F("Debug functions is not enabled in your project"));

#endif

#endif // Not low memory board

#ifdef DEBUGGER_FOR_SERIALDEBUG
	} else if (command == "s") {

		// Silence

		debugSilence (!_debugSilence, true, false);
#endif

	} else if (command == "r") {

		// Repeat last command

		if (options == "?") { // Help

			printLibrary();
			PRINTFLN(F("Valid last commands to repeat: f|g|m"));

		} else if (_debugLastCommand.length() == 0) {

			printLibrary();
			PRINTFLN(F("Last command not set or unsupported. use r ? to show help"));

		} else { // Start repeats

			printLibrary();
			// TODO: verify it
			PRINTFLN(F("Start repeating command: %s - press any command or enter to stop"), _debugLastCommand.c_str());
			_debugRepeatCommand = true;
		}

	} else if (command == "$app") {

		// TODO see it

//		// Connection with SerialDebugApp
//
//		D("$app -> %s", options.c_str());
//
//		debugAppConnection();

	} else {

		// Command invalid

		if (showError) {
			printLibrary();
			PRINTFLN(errorSintax);
		}
	}

	// Can repeat ?

	//D("canrepeat=%d", canRepeat);

	if (!repeating) { // Not if repeating commands

		if (canRepeat) {

			// Save it

			_debugLastCommand = command;
			_debugLastCommand.concat(' ');
			_debugLastCommand.concat(options);

		} else if (!_debugRepeatCommand) {

			// Clear it

			_debugLastCommand = "";
		}
	}
}

#ifdef DEBUGGER_FOR_SERIALDEBUG

// Show debug help

void showHelp() {

	PRINTFLN("*");
	printLibrary();
	PRINTLN();
	PRINTF(F("* Debugger version: "));
	PRINTF(DEBUGGER_VERSION);
	PRINTLN();

	// Using PROGMEM in large strings (even for Espressif boards)

	PRINTFLN(FPSTR(debugHelp));
	PRINTLN();

}
#endif

//////// Simple software debugger

// Handle for debugger

void debugHandleDebugger (const boolean calledByHandleEvent) {

	// Time measure - give commented

	uint32_t time = micros();

	//D("debugH %d", _debugWatchesEnabled);

	// Inactive ?

#ifdef DEBUGGER_FOR_SERIALDEBUG

	if (!_debugActive || !_debugDebuggerEnabled) {
		return;
	}

#else // RemoteDebug

	// Verify connection

	if (_Debug) {
		
		if (!(_Debug->isConnected())) { // Not connected ?
			if (_debugDebuggerEnabled) { // If debugger is enabled -> disable it
				_debugDebuggerEnabled = false;
			}
		}
		_debugApp = _Debug->wsIsConnected();

	}

	if (!_debugDebuggerEnabled) {
		return;
	}
#endif

	// Process globals

	for (uint8_t g=0; g < _debugGlobalsAdded; g++) {

#ifdef BOARD_ENOUGH_MEMORY // Modern and faster board ?

//		boolean process = _debugApp; // Always process for app
		boolean process = calledByHandleEvent; // Only by event, app not showing vars on debugger screen yet

#else
		boolean process = _debugApp && calledByHandleEvent;
#endif
		debugGlobal_t *global = &_debugGlobals[g];

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

		if (!process) {

			// Process watches of type when changed

			if (_debugWatchesAdded > 0 && _debugWatchesEnabled) {

				// Verify if exist any watch for this global
				// Not for strings - due is low to process any time

				if (calledByHandleEvent ||
					(global->type != DEBUG_TYPE_CHAR_ARRAY &&
					global->type != DEBUG_TYPE_STRING)) {

					for (uint8_t w=0; w < _debugWatchesAdded; w++) {

						if (_debugWatches[w].globalNum == g &&
							_debugWatches[w].enabled) {

							process = true; // Process it

							break;
						}
					}
				}
			}
		}
#endif

		// Process ?

		//D("debugH w %d enab %d proc %d ", _debugWatchesAdded,  _debugWatchesEnabled, process);

		if (process) {

			// Value changed ?

			boolean changed = apllyOperation(global->type, global->pointer, \
												DEBUG_WATCH_DIFF, \
												global->typeOld, global->pointerOld);

			//D("global %u %s type %u type old %u chg %d %d", g, global->name, global->type, global->typeOld, changed, global->changed);

			if (changed) { // Changed ?

				//D("global changed");

				if (global->pointerOld) { // Only if it has content

					global->changed = true;

					if (_debugApp) { // SerialDebugApp connected ?

						// Get value

						String value = "";
						String type = "";

						getStrValue(global->type, global->pointer, global->showLength, false, value, type);

#ifdef DEBUGGER_SEND_INFO
						// Send it

						if (_debugApp) {
							PRINTFLN(F("$app:C:%u:%s"), (g + 1), value.c_str());
						}
#endif
					}
				}

				// Mark to update old value

				global->updateOldValue = true;

			} else if (!changed) { // && calledByHandleEvent) { // only if called by handle event

				if (global->changed) {

					global->changed = false;

					//D("global %u !chg", g);

				}

			}
		}
	}

	// Break ?

	boolean hasBreak = false;

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

	// Process watches

	if (_debugWatchesAdded > 0 && _debugWatchesEnabled) {

		uint8_t totTriggered = 0;
		boolean alwaysStop = false;

		for (uint8_t w=0; w < _debugWatchesAdded; w++) {

			debugWatch_t *watch = &_debugWatches[w];
			debugGlobal_t *global = &_debugGlobals[watch->globalNum];

			// Only verify watch, if have change in global or in globals for watch cross
			// Only if enabled

			if (watch->enabled && (global->updateOldValue || watch->watchCross || watch->operation == DEBUG_WATCH_CHANGED)) {

				boolean triggered = false;

				// When changed type

				if (watch->operation == DEBUG_WATCH_CHANGED) {

					// Otimized: global is checked before

					triggered = global->changed;

				} else if (watch->watchCross) { // Is cross type ?

					debugGlobal_t *globalCross = &_debugGlobals[watch->globalNumCross];

					if (global->updateOldValue || globalCross->updateOldValue) {

						triggered = apllyOperation(global->type, global->pointer, \
													watch->operation, \
													globalCross->type, globalCross->pointer);
					}

				} else { // Anothers

					triggered = apllyOperation(global->type, global->pointer, \
												watch->operation, \
												global->type, watch->pointerValue);
				}

				// Has triggered  ?

				if (!watch->triggered && triggered) {

					watch->triggered = true;

					totTriggered++;

					if (watch->alwaysStop && !alwaysStop) { // Always stop ?
						alwaysStop = true;
					}

					printLibrary();
					PRINTFLN(F("Watch %u is triggered:"), (w + 1));

					showWatch(w, false);

#ifdef DEBUGGER_SEND_INFO
					if (_debugApp) { // App ?
						PRINTFLN(F("$app:T:%u:1"), (w + 1));
					}
#endif
				} else if (watch->triggered && !triggered) { // Unmark it

					watch->triggered = false;

#ifdef DEBUGGER_SEND_INFO
					if (_debugApp) { // App ?
						PRINTFLN(F("$app:T:%u:0"), (w + 1));
					}
#endif
				}
			}
		}

		// Has triggered any watch ?

		if (totTriggered > 0 && (_debugWatchStop || alwaysStop)) { // Stop on watches ?

			printLibrary();
			PRINTFLN(F("%u watch(es) has triggered."), totTriggered);
			printLibrary();
#ifdef DEBUGGER_FOR_SERIALDEBUG
		PRINTFLN(F("Press enter to continue"));
#else
		if (_Debug && ((_Debug->isConnected()))) {
			PRINTFLN(F("Send any command or press the \"Silence\" button, to continue."));
		} else {
			PRINTFLN(F("Press any command, and enter to continue"));
		}
#endif
			printLibrary();
			PRINTFLN(F("Or another command as: reset(to reset) or ns(to not stop again)"));

			// Do a break

			String response = debugBreak("", DEBUG_BREAK_WATCH, true);

			if (response.length() > 0) { // Process command

				if (response == "ns") { // Non stop
					response = "wa ns";
				}
				processCommand(response, false, false);
			}

			hasBreak = true;
		}
	}

#endif

	// Update old value for globals (after watch, to show old value correct

	for (uint8_t g=0; g < _debugGlobalsAdded; g++) {

		debugGlobal_t *global = &_debugGlobals[g];

		if (global->updateOldValue) {

			// Copy value of globals variable to old

			updateValue(global->type, global->pointer, global->typeOld, &(global->pointerOld));

			// Unmark it

			global->updateOldValue = false;
		}
	}

	// Debug (give commented)

	if (!hasBreak) { // Not for watch triggered, have a delay for waiting response
		time = (micros() - time);
		if (time >= 250) {
			D("handle dbg call=%d elap=%u us", calledByHandleEvent, time);
		}
	}
}

// Add a function

int8_t debugAddFunctionVoid(const char* name, void (*callback)()) {

	int8_t pos = addFunction(name, DEBUG_TYPE_FUNCTION_VOID);

	if (pos != -1) {
		_debugFunctions[pos].callback = callback;
	}
	return pos;
}

int8_t debugAddFunctionStr(const char* name, void (*callback)(String)) {

	int8_t pos = addFunction(name, DEBUG_TYPE_STRING);

	if (pos != -1) {
		_debugFunctions[pos].callback = (void(*) (void)) callback;
	}
	return pos;
}

int8_t debugAddFunctionChar(const char* name, void (*callback)(char)) {

	int8_t pos = addFunction(name, DEBUG_TYPE_CHAR);

	if (pos != -1) {
		_debugFunctions[pos].callback = (void(*) (void)) callback;
	}
	return pos;
}

int8_t debugAddFunctionInt(const char* name, void (*callback)(int)) {

	int8_t pos = addFunction(name, DEBUG_TYPE_INT);

	if (pos != -1) {
		_debugFunctions[pos].callback = (void(*) (void))callback;
	}
	return pos;
}

#ifdef DEBUG_USE_FLASH_F
// For Flash F()

int8_t debugAddFunctionVoid(const __FlashStringHelper* name, void (*callback)()) {

	int8_t pos = addFunction(name, DEBUG_TYPE_FUNCTION_VOID);

	if (pos != -1) {
		_debugFunctions[pos].callback = callback;
	}
	return pos;
}

int8_t debugAddFunctionStr(const __FlashStringHelper* name, void (*callback)(String)) {

	int8_t pos = addFunction(name, DEBUG_TYPE_STRING);

	if (pos != -1) {
		_debugFunctions[pos].callback = (void(*) (void)) callback;
	}
	return pos;
}

int8_t debugAddFunctionChar(const __FlashStringHelper* name, void (*callback)(char)) {

	int8_t pos = addFunction(name, DEBUG_TYPE_CHAR);

	if (pos != -1) {
		_debugFunctions[pos].callback = (void(*) (void)) callback;
	}
	return pos;
}

int8_t debugAddFunctionInt(const __FlashStringHelper* name, void (*callback)(int)) {

	int8_t pos = addFunction(name, DEBUG_TYPE_INT);

	if (pos != -1) {
		_debugFunctions[pos].callback = (void(*) (void)) callback;
	}
	return pos;
}
#endif

#ifndef BOARD_LOW_MEMORY // Not for low memory boards
// Add a function description for last added

void debugSetLastFunctionDescription(const char *description) {

	if (_debugFunctionsAdded > 0) {
		_debugFunctions[_debugFunctionsAdded - 1].description = description;
	}
}

#ifdef DEBUG_USE_FLASH_F
void debugSetLastFunctionDescription(const __FlashStringHelper *description) {

	if (_debugFunctionsAdded > 0) {
		_debugFunctions[_debugFunctionsAdded - 1].descriptionF = description;
	}
}

#endif

// Add a global variable

// Basic types

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

int8_t debugAddGlobalBoolean (const char* name, boolean* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_BOOLEAN, 0);
}
int8_t debugAddGlobalChar (const char* name, char* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_CHAR, 0);
}
int8_t debugAddGlobalByte (const char* name, byte* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_BYTE, 0);
}
int8_t debugAddGlobalInt (const char* name, int* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_INT, 0);
}
int8_t debugAddGlobalUInt (const char* name, unsigned int* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_U_INT, 0);
}
int8_t debugAddGlobalLong (const char* name, long* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_LONG, 0);
}
int8_t debugAddGlobalULong (const char* name, unsigned long* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_U_LONG, 0);
}
int8_t debugAddGlobalFloat (const char* name, float* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_FLOAT, 0);
}
int8_t debugAddGlobalDouble (const char* name, double* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_DOUBLE, 0);
}

// Integer C size t

int8_t debugAddGlobalInt8_t (const char* name, int8_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT8_T, 0);
}
int8_t debugAddGlobalInt16_t (const char* name, int16_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT16_T, 0);
}
int8_t debugAddGlobalInt32_t (const char* name, int32_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT32_T, 0);
}
//#ifdef ESP32
//int8_t debugAddGlobalInt64_t (const char* name, int64_t* pointer) {
//
//	return debugAddGlobal(name, pointer, DEBUG_TYPE_UINT64_T);
//}
//#endif
int8_t debugAddGlobalUInt8_t (const char* name, uint8_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT8_T, 0);
}
int8_t debugAddGlobalUInt16_t (const char* name, uint16_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT16_T, 0);
}
int8_t debugAddGlobalUInt32_t (const char* name, uint32_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT32_T, 0);
}
//#ifdef ESP32
//int8_t debugAddGlobalUInt64_t (const char* name, uint64_t* pointer) {
//
//	debugAddGlobal(name, pointer, DEBUG_TYPE_UINT64_T);
//}
//#endif

// Strings

int8_t debugAddGlobalCharArray (const char* name, char* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_CHAR_ARRAY, 0);
}
int8_t debugAddGlobalCharArray (const char* name, char* pointer, uint8_t showLength) {

	return addGlobal(name, pointer, DEBUG_TYPE_CHAR_ARRAY, showLength);
}

int8_t debugAddGlobalString (const char* name, String* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_STRING, 0);
}
int8_t debugAddGlobalString (const char* name, String* pointer, uint8_t showLength) {

	return addGlobal(name, pointer, DEBUG_TYPE_STRING, showLength);
}
#endif// Not low memory board

// For Flash F()

#ifdef DEBUG_USE_FLASH_F

// Basic types

int8_t debugAddGlobalBoolean (const __FlashStringHelper* name, boolean* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_BOOLEAN, 0);
}
int8_t debugAddGlobalChar (const __FlashStringHelper* name, char* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_CHAR, 0);
}
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
int8_t debugAddGlobalByte (const __FlashStringHelper* name, byte* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_BYTE, 0);
}
#endif
int8_t debugAddGlobalInt (const __FlashStringHelper* name, int* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_INT, 0);
}
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
int8_t debugAddGlobalUInt (const __FlashStringHelper* name, unsigned int* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_U_INT, 0);
}
int8_t debugAddGlobalLong (const __FlashStringHelper* name, long* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_LONG, 0);
}
#endif
int8_t debugAddGlobalULong (const __FlashStringHelper* name, unsigned long* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_U_LONG, 0);
}
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
int8_t debugAddGlobalFloat (const __FlashStringHelper* name, float* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_FLOAT, 0);
}
int8_t debugAddGlobalDouble (const __FlashStringHelper* name, double* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_DOUBLE, 0);
}

// Integer C size t

int8_t debugAddGlobalInt8_t (const __FlashStringHelper* name, int8_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT8_T, 0);
}
int8_t debugAddGlobalInt16_t (const __FlashStringHelper* name, int16_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT16_T, 0);
}
int8_t debugAddGlobalInt32_t (const __FlashStringHelper* name, int32_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT32_T, 0);
}
//#ifdef ESP32
//int8_t debugAddGlobalInt64_t (const char* name, int64_t* pointer) {
//
//	return debugAddGlobal(name, pointer, DEBUG_TYPE_UINT64_T);
//}
//#endif
int8_t debugAddGlobalUInt8_t (const __FlashStringHelper* name, uint8_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT8_T, 0);
}
int8_t debugAddGlobalUInt16_t (const __FlashStringHelper* name, uint16_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT16_T, 0);
}
int8_t debugAddGlobalUInt32_t (const __FlashStringHelper* name, uint32_t* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_UINT32_T, 0);
}
//#ifdef ESP32
//int8_t debugAddGlobalUInt64_t (const char* name, uint64_t* pointer) {
//
//	debugAddGlobal(name, pointer, DEBUG_TYPE_UINT64_T);
//}
//#endif

// Strings

int8_t debugAddGlobalCharArray (const __FlashStringHelper* name, char* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_CHAR_ARRAY, 0);
}
int8_t debugAddGlobalCharArray (const __FlashStringHelper* name, char* pointer, uint8_t showLength) {

	return addGlobal(name, pointer, DEBUG_TYPE_CHAR_ARRAY, showLength);
}
#endif // Not low memory board

int8_t debugAddGlobalString (const __FlashStringHelper* name, String* pointer) {

	return addGlobal(name, pointer, DEBUG_TYPE_STRING, 0);
}
int8_t debugAddGlobalString (const __FlashStringHelper* name, String* pointer, uint8_t showLength) {

	return addGlobal(name, pointer, DEBUG_TYPE_STRING, showLength);
}
#endif // DEBUG_USE_FLASH_F

#ifndef BOARD_LOW_MEMORY // Not for low memory boards
// Add a description for global in last added

void debugSetLastGlobalDescription(const char *description) {

	_debugGlobals[_debugGlobalsAdded - 1].description = description;
}

#ifdef DEBUG_USE_FLASH_F
void debugSetLastGlobalDescription(const __FlashStringHelper *description) {

	_debugGlobals[_debugGlobalsAdded - 1].descriptionF = description;
}
#endif
#endif // Not low memory board

//// Not allowed
//
//int8_t debugAddGlobalCharArray (const char* name, const char* pointer) {
//}
//int8_t debugAddGlobalCharArray (const char* name, const char* pointer, uint8_t showLength) {
//}

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

// Add a watch

int8_t debugAddWatchBoolean (const char* globalName, uint8_t operation, boolean value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchBoolean(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}

int8_t debugAddWatchBoolean (uint8_t globalNum, uint8_t operation, boolean value, boolean allwaysStop) {

	// Check Operation

	if (operation == DEBUG_WATCH_LESS_EQ || operation == DEBUG_WATCH_GREAT_EQ) {
		printLibrary();
		PRINTFLN(F("Operation not allowed for boolean"));
		return -1;
	}

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_BOOLEAN)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {

		// Alloc memory for pointerValue and copy value
		size_t size = sizeof(boolean);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchChar (const char* globalName, uint8_t operation, char value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchChar(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchChar (uint8_t globalNum, uint8_t operation, char value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_CHAR)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(char);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchByte (const char* globalName, uint8_t operation, byte value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchByte(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}

int8_t debugAddWatchByte (uint8_t globalNum, uint8_t operation, byte value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_BYTE)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(byte);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchInt (const char* globalName, uint8_t operation, int value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchInt(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchInt (uint8_t globalNum, uint8_t operation, int value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_INT)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(int);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchUInt (const char* globalName, uint8_t operation, unsigned int value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchUInt(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchUInt (uint8_t globalNum, uint8_t operation, unsigned int value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_U_INT)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(unsigned int);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchLong (const char* globalName, uint8_t operation, long value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchLong(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchLong (uint8_t globalNum, uint8_t operation, long value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_LONG)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(long);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchULong (const char* globalName, uint8_t operation, unsigned long value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchULong(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchULong (uint8_t globalNum, uint8_t operation, unsigned long value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_U_LONG)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(unsigned long);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchFloat (const char* globalName, uint8_t operation, float value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchFloat(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchFloat (uint8_t globalNum, uint8_t operation, float value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_FLOAT)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(float);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchDouble (const char* globalName, uint8_t operation, double value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchDouble(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchDouble (uint8_t globalNum, uint8_t operation, double value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_DOUBLE)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(double);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchInt8_t (const char* globalName, uint8_t operation, int8_t value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchInt8_t(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchInt8_t (uint8_t globalNum, uint8_t operation, int8_t value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_INT8_T)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(int8_t);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchInt16_t (const char* globalName, uint8_t operation, int16_t value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchInt16_t(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchInt16_t (uint8_t globalNum, uint8_t operation, int16_t value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_INT16_T)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(int16_t);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchInt32_t (const char* globalName, uint8_t operation, int32_t value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchInt32_t(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchInt32_t (uint8_t globalNum, uint8_t operation, int32_t value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_INT32_T)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(int32_t);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

//#ifdef ESP32
//int8_t debugAddWatchInt64_t (uint8_t globalNum, uint8_t operation, int64_t value);
//#endif

int8_t debugAddWatchUInt8_t (const char* globalName, uint8_t operation, uint8_t value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		//D("debugAddWatchUInt8_t: global=%u oper=%u value=%u", globalNum, operation, value);
		return debugAddWatchUInt8_t(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchUInt8_t (uint8_t globalNum, uint8_t operation, uint8_t value, boolean allwaysStop) {

	// Verify global type

    //D("debugAddWatch: globalNum=%u oper=%u value=%u", globalNum. operation, value);

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_UINT8_T)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	//D("debugAddWatchUInt8_t: ret =%u ", ret);

	if (ret != -1) {
		size_t size = sizeof(uint8_t);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);

		// Test
//		String value="";
//		String type="";
//		getStrValue(DEBUG_TYPE_UINT8_T, _debugWatches[ret].pointer, 0, value, type);
//		D("debugAddWatchUInt8_t: size=%u value=%s", size, value.c_str());

	}

	return ret;
}

int8_t debugAddWatchUInt16_t (const char* globalName, uint8_t operation, uint16_t value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchUInt16_t(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchUInt16_t (uint8_t globalNum, uint8_t operation, uint16_t value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_UINT16_T)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(uint16_t);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchUInt32_t (const char* globalName, uint8_t operation, uint32_t value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchUInt32_t(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchUInt32_t (uint8_t globalNum, uint8_t operation, uint32_t value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_UINT32_T)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = sizeof(uint32_t);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

//#ifdef ESP32
//int8_t debugAddWatchUInt64_t (uint8_t globalNum, uint8_t operation, uint64_t value);
//#endif

int8_t debugAddWatchCharArray (const char* globalName, uint8_t operation, const char* value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchCharArray(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}

int8_t debugAddWatchCharArray (uint8_t globalNum, uint8_t operation, const char* value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_CHAR_ARRAY)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		size_t size = strlen(value);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy( _debugWatches[ret].pointerValue, &value, size);
	}

	return ret;
}

int8_t debugAddWatchString (const char* globalName, uint8_t operation, String value, boolean allwaysStop) {

	uint8_t globalNum;

	if (findGlobal(globalName, &globalNum)) {
		return debugAddWatchString(globalNum, operation, value, allwaysStop);
	} else {
		return -1;
	}
}
int8_t debugAddWatchString (uint8_t globalNum, uint8_t operation, String value, boolean allwaysStop) {

	// Verify global type

	if (!verifyGlobalType(globalNum, DEBUG_TYPE_STRING)) {
		return -1;
	}

	// Add watch

	int8_t ret = addWatch(globalNum, operation, allwaysStop);

	if (ret != -1) {
		_debugWatches[ret].typeValue = DEBUG_TYPE_CHAR_ARRAY; // Store value as char array
		const char* content = value.c_str(); // by char array
		size_t size = strlen(content);
		_debugWatches[ret].pointerValue = malloc (size);
		memcpy(&content, _debugWatches[ret].pointerValue, size);
	}

	return ret;
}

// For watches cross - between 2 globals

int8_t debugAddWatchCross(const char* globalName, uint8_t operation, const char* anotherGlobalName, boolean allwaysStop) {

	uint8_t globalNum;
	uint8_t anotherGlobalNum;

	if (!findGlobal(globalName, &globalNum)) {
		return -1;
	}
	if (!findGlobal(anotherGlobalName, &anotherGlobalNum)) {
		return -1;
	}

	// Add

	return debugAddWatchCross(globalNum, operation, anotherGlobalNum, allwaysStop);
}
int8_t debugAddWatchCross(uint8_t globalNum, uint8_t operation, uint8_t anotherGlobalNum, boolean allwaysStop) {

	int8_t ret = -1;

	// Validate

	if (globalNum > _debugGlobalsAdded) {

		printLibrary();
		PRINTFLN(F("First global number must between 1 and %u"), _debugGlobalsAdded);
		return -1;
	}

	if (anotherGlobalNum > _debugGlobalsAdded) {

		printLibrary();
		PRINTFLN(F("Second global number must between 1 and %u"), _debugGlobalsAdded);
		return -1;
	}

	if (globalNum == anotherGlobalNum) {

		printLibrary();
		PRINTFLN(F("Globals numbers (first and second) can not be equals"));
		return -1;
	}

	if (operation == DEBUG_WATCH_CHANGED) {

		printLibrary();
		PRINTFLN(F("Changed type opretation is not allowed for cross watch"));
		return -1;
	}

	// Adjust the numbers

	globalNum--;
	anotherGlobalNum--;

	// Add this

	debugWatch_t watch;

	watch.globalNum = globalNum;
	watch.operation = operation;
	watch.watchCross = true;
	watch.globalNumCross = anotherGlobalNum;
	watch.alwaysStop = allwaysStop;
	watch.typeValue = DEBUG_TYPE_UNDEFINED;

	_debugWatches.push_back(watch);

	ret = _debugWatchesAdded;

	// Count it

	if (ret != -1) {

		_debugWatchesAdded++;
	}

	// Return index of this

	return ret;

}

// For Flash F

#ifdef DEBUG_USE_FLASH_F

int8_t debugAddWatchBoolean (const __FlashStringHelper* globalName, uint8_t operation, boolean value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchBoolean(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchChar (const __FlashStringHelper* globalName, uint8_t operation, char value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchChar(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchByte (const __FlashStringHelper* globalName, uint8_t operation, byte value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchByte(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchInt (const __FlashStringHelper* globalName, uint8_t operation, int value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchInt(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchLong (const __FlashStringHelper* globalName, uint8_t operation, long value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchLong(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchULong (const __FlashStringHelper* globalName, uint8_t operation, unsigned long value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchULong(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchFloat (const __FlashStringHelper* globalName, uint8_t operation, float value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchFloat(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchDouble (const __FlashStringHelper* globalName, uint8_t operation, double value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchDouble(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchInt8_t (const __FlashStringHelper* globalName, uint8_t operation, int8_t value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchInt8_t(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchInt16_t (const __FlashStringHelper* globalName, uint8_t operation, int16_t value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchInt16_t(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchInt32_t (const __FlashStringHelper* globalName, uint8_t operation, int32_t value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchInt32_t(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchUInt8_t (const __FlashStringHelper* globalName, uint8_t operation, uint8_t value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchUInt8_t(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchUInt16_t (const __FlashStringHelper* globalName, uint8_t operation, uint16_t value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchUInt16_t(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchUInt32_t (const __FlashStringHelper* globalName, uint8_t operation, uint32_t value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchUInt32_t(name.c_str(), operation, value, allwaysStop);
}

int8_t debugAddWatchCharArray (const __FlashStringHelper* globalName, uint8_t operation, const char* value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchCharArray(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchUInt (const __FlashStringHelper* globalName, uint8_t operation, unsigned int value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchUInt(name.c_str(), operation, value, allwaysStop);
}
int8_t debugAddWatchString (const __FlashStringHelper* globalName, uint8_t operation, String value, boolean allwaysStop) {

	String name = String(globalName);
	return debugAddWatchString(name.c_str(), operation, value, allwaysStop);
}

int8_t debugAddWatchCross(const __FlashStringHelper* globalName, uint8_t operation, const __FlashStringHelper* anotherGlobalName, boolean allwaysStop) {

	String name = String(globalName);
	String anotherName = String(anotherGlobalName);
	return debugAddWatchCross(name.c_str(), operation, anotherName.c_str(), allwaysStop);
}

#endif // DEBUG_USE_FLASH_F

#endif // Not low memory board

/////// Private code (Note: this not starts with debug)

// Private function used for all types of functions

// Add function

static int8_t addFunction(const char* name, uint8_t argType) {

	int8_t ret = -1;

	// Add this

	debugFunction_t function;

	function.name = name;
	function.argType = argType;

	_debugFunctions.push_back(function);

	ret = _debugFunctionsAdded;

	// Count it

	if (ret != -1) {

		_debugFunctionsAdded++;
	}

	// Return index of this

	return ret;
}

#ifdef DEBUG_USE_FLASH_F
static int8_t addFunction(const __FlashStringHelper* name, uint8_t argType) {

	int8_t ret = -1;

	debugFunction_t function;

	// Add this

	function.nameF = name;
	function.argType = argType;

	_debugFunctions.push_back(function);

	ret = _debugFunctionsAdded;

	// Count it

	if (ret != -1) {

		_debugFunctionsAdded++;
	}

	// Return index of this

	return ret;
}
#endif

// Add Global variable

static int8_t addGlobal(const char* name, void* pointer, uint8_t type, uint8_t showLength) {

	int8_t ret = -1;

	debugGlobal_t global;

	// Add this

	global.name = name;
	global.pointer = pointer;
	global.type = type;
	global.showLength = showLength;

	if (type == DEBUG_TYPE_STRING) {
		global.typeOld = DEBUG_TYPE_CHAR_ARRAY; // Store old value as char array
	} else {
		global.typeOld = type; // Same type
	}

	_debugGlobals.push_back(global);

	ret = _debugGlobalsAdded;

	// Count it

	if (ret != -1) {

		_debugGlobalsAdded++;
	}

	// Return index of this

	return ret;
}

#ifdef DEBUG_USE_FLASH_F
static int8_t addGlobal(const __FlashStringHelper* name, void* pointer, uint8_t type, uint8_t showLength) {

	int8_t ret = -1;

	debugGlobal_t global;

	// Add this

	global.nameF = name;
	global.pointer = pointer;
	global.type = type;
	global.showLength = showLength;

	if (type == DEBUG_TYPE_STRING) {
		global.typeOld = DEBUG_TYPE_CHAR_ARRAY; // Store old value as char array
	} else {
		global.typeOld = type; // Same type
	}

	_debugGlobals.push_back(global);

	ret = _debugGlobalsAdded;

	// Count it

	if (ret != -1) {

		_debugGlobalsAdded++;
	}

	// Return index of this

	return ret;
}
#endif

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

// For watches

static int8_t addWatch(uint8_t globalNum, uint8_t operation, boolean allwaysStop) {

	int8_t ret = -1;

	// Validate

	if (globalNum == 0 || globalNum > _debugGlobalsAdded) {

		printLibrary();
		PRINTFLN(F("Global number must between 1 and %u"), _debugGlobalsAdded);
		return -1;
	}

	// Adjust the number

	globalNum--;

	// Add this

	debugWatch_t watch;

	watch.globalNum = globalNum;
	watch.operation = operation;
	watch.alwaysStop = allwaysStop;
	watch.typeValue = _debugGlobals[globalNum].type;

	_debugWatches.push_back(watch);

	//D("debugAddWatch n=%u glob=%u oper=%u", _debugWatchesAdded, globalNum, operation);

	ret = _debugWatchesAdded;

	// Count it

	_debugWatchesAdded++;

	// Enabled

	_debugWatchesEnabled = true;

	// Return index of this

	return ret;

}

// Process watches commands

static void processWatches(String& options) {

#if DEBUG_USE_FLASH_F

	__FlashStringHelper* errorSintax = F("Invalid sintax for watches. use w ? to show help");

#else

	const char* errorSintax = "Invalid sintax for watches. use w ? to show help";

#endif

	// Get fields of command options

	Fields fields(options, ' ', true);

    D("options = %s fields.size() = %u ", options.c_str(), fields.size());

	String firstOption = (fields.size() >= 1)? fields.getString(1) : "";
	firstOption.toLowerCase();

	//D("first = %s", firstOption.c_str());

	if (fields.size() > 6) {

		printLibrary();
		PRINTFLN(errorSintax);

	} else if (_debugWatchesAdded > 0 || (firstOption == "a" || firstOption == "")) {

		if (firstOption.length() == 0 || firstOption == "?") {

			// Just show globals and help

			firstOption = "";
			showWatches(firstOption);

		} else if (fields.size() == 1 && fields.isNum(1)) {

			// Search by number

			uint8_t watchNum = fields.getInt(1);

			showWatch(watchNum);

		} else {

			// Process commands

			if (firstOption == "a") {

				// Add watch

				if (fields.size() == 3 || fields.size() == 4) {

					addWatch(fields);

				} else {

					printLibrary();
					PRINTFLN(errorSintax);
				}

			} else if (firstOption == "ac") {

				// Add cross watch

				if (fields.size() == 4) {

					addWatchCross(fields);

				} else {

					printLibrary();
					PRINTFLN(errorSintax);
				}

			} else if (firstOption == "u") {

				// Update watch

				if (fields.size() >= 4) {

					changeWatch(fields);

				} else {

					printLibrary();
					PRINTFLN(errorSintax);
				}
			} else if (firstOption == "d" || firstOption == "e" || firstOption == "r") {

				// Process watches action

				if (fields.size() == 2) {

					processWatchesAction(fields);

				} else {

					printLibrary();
					PRINTFLN(errorSintax);
				}

			} else if (firstOption == "ns") {

				// Set to nonstop for watches

				_debugWatchStop = false;

				printLibrary();
				PRINTFLN("Watches set to non stop");

				if (_debugApp) { // App ?
					PRINTFLN(F("$app:W:s:%c"), ((_debugWatchStop)?'1':'0'));
				}

			} else if (firstOption == "s") {

				// Set to stop for watches

				_debugWatchStop = true;

				printLibrary();
				PRINTFLN("Watches set to stop");

				if (_debugApp) { // ? App
					PRINTFLN(F("$app:W:s:%c"), ((_debugWatchStop)?'1':'0'));
				}

			} else {

				printLibrary();
				PRINTFLN(errorSintax);
			}
		}

	} else {

		printLibrary();
		PRINTFLN(F("Watches not added yet"));
	}

	// Clear the fields

	fields.clear();

}

// Process watches action commands

static void processWatchesAction(Fields& fields) {

	String firstOption = fields.getString(1);
	String secondOption = fields.getString(2);

	uint8_t watchNum = 0;
	boolean all = false;

	// Process second option

	if (secondOption == "a" || secondOption == "all") {

		all = true;

	} else if (fields.isNum(2)) {

		watchNum = secondOption.toInt();

		if (watchNum == 0 || watchNum > _debugWatchesAdded) {

			printLibrary();
			PRINTFLN(F("Invalid watch num\r\n*"));
			return;
		}

		watchNum--; // Index starts in 0

	} else {

		printLibrary();
		PRINTFLN(F("Watch num must be numeric\r\n*"));
		return;

	}

	// Remove ?

	if (firstOption == "r") {

		if (all) {

			// Remove all

			_debugWatches.clear();
			_debugWatchesAdded = 0;

			printLibrary();
			PRINTFLN(F("All watches has removed"));

		} else {

			// Remove item

#ifdef VECTOR_STD
			_debugWatches.erase(_debugWatches.begin() + watchNum);
#else
			_debugWatches.erase(watchNum);
#endif

			_debugWatchesAdded--;

			printLibrary();
			PRINTFLN(F("Watch has removed"));

		}

		return;
	}

	// Process watches

	_debugWatchesEnabled = false; // Have any enabled ?

	for (uint8_t i=0; i < _debugWatchesAdded; i++) {

		// Process action

		if (firstOption == "d") {

			// Disable watch

			if (all || i == watchNum) {

				if (_debugWatches[i].enabled) {

					_debugWatches[i].enabled = false;

					printLibrary();
					PRINTFLN(F("Watch %u has disabled"), (i + 1));
				}
			}

		} else if (firstOption == "e") {

			// Enable watch

			if (all || i == watchNum) {

				if (!_debugWatches[i].enabled) {

					_debugWatches[i].enabled = true;

					printLibrary();
					PRINTFLN(F("Watch %u has enabled"), (i + 1));

				}
			}
		}

		// Have any enabled ?

		if (_debugWatches[i].enabled) {
			_debugWatchesEnabled = true;
		}
	}
}

// Add a watch

static int8_t addWatch(Fields& fields) {

	int8_t ret = -1; // Return
	uint8_t pos = 2; // Field possition

	// Global number

	uint8_t globalNum;

	if (fields.isNum(pos)) { // By num

		globalNum = fields.getInt(pos);

		if (globalNum == 0 || globalNum > _debugGlobalsAdded) {

			printLibrary();
			PRINTFLN(F("Invalid index for global in watch\r\n*"));
			return false;
		}

		globalNum--; // Globals index start in 0

	} else { // By name

		 if (!(findGlobal(fields.getString(pos).c_str(), &globalNum, false))) {
			 return false;
		 }
	}

	//D("globalnum=%u", globalNum);

	// Verify operation

	pos++;

	uint8_t operation = 0;

	if (!(getWatchOperation(fields.getString(pos), &operation))) {

		printLibrary();
		PRINTFLN(F("Invalid operation\r\n*"));
		return false;

	}

	// Verify value

	String value = "";

	if (operation != DEBUG_WATCH_CHANGED) { // Not for changed type

		pos++;

		switch (_debugGlobals[globalNum].type) {
			case DEBUG_TYPE_INT:
			case DEBUG_TYPE_U_INT:
			case DEBUG_TYPE_LONG:
			case DEBUG_TYPE_U_LONG:
			case DEBUG_TYPE_FLOAT:
			case DEBUG_TYPE_DOUBLE:
			case DEBUG_TYPE_INT8_T:
			case DEBUG_TYPE_INT16_T:
			case DEBUG_TYPE_INT32_T:
			case DEBUG_TYPE_UINT8_T:
			case DEBUG_TYPE_UINT16_T:
			case DEBUG_TYPE_UINT32_T:

				// Is number ?

				if (!(fields.isNum(pos))) {

					printLibrary();
					PRINTFLN(F("The value must be numeric\r\n*"));
					return false;

				}
				break;
		}

		// Get value

		value = fields.getString(pos);

		// Return the spaces

		char conv = 31;
		value.replace(conv, ' ');

	}

	// Verify allways stop

	pos++;

	boolean allwaysStop = false;

	if (fields.size() == pos) {

		allwaysStop = (fields.getString(pos) == "as");

	}

	// Add watch (these funcions work with index of global start by 1)

	if (operation == DEBUG_WATCH_CHANGED) { // Changed type

		ret = addWatch((globalNum + 1), DEBUG_WATCH_CHANGED, allwaysStop);

	} else {

		// From type of global

		switch (_debugGlobals[globalNum].type) {
			case DEBUG_TYPE_BOOLEAN:
				{
					boolean conv = (value == "1" || value == "t" || value == "true");
					ret = debugAddWatchBoolean((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_INT:
				{
					int conv = value.toInt();
					ret = debugAddWatchInt((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_U_INT:
				{
					unsigned int conv = value.toInt();
					ret = debugAddWatchUInt((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_LONG:
				{
					long conv = value.toInt(); // TODO see if works with large values ?????
					ret = debugAddWatchLong((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_U_LONG:
				{
					unsigned long conv = value.toInt(); // TODO see if works with large values ?????
					ret = debugAddWatchULong((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_FLOAT:
				{
					float conv = value.toFloat();
					ret = debugAddWatchFloat((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_DOUBLE:
				{
					double conv = value.toFloat(); // TODO see if works with large values ?????
					ret = debugAddWatchULong((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_INT8_T:
				{
					int8_t conv = value.toInt();
					ret = debugAddWatchInt8_t((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_INT16_T:
				{
					int16_t conv = value.toInt();
					ret = debugAddWatchInt16_t((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_INT32_T:
				{
					int32_t conv = value.toInt();
					ret = debugAddWatchInt32_t((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_UINT8_T:
				{
					uint8_t conv = value.toInt();
					ret = debugAddWatchUInt8_t((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_UINT16_T:
				{
					uint16_t conv = value.toInt();
					ret = debugAddWatchUInt16_t((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_UINT32_T:
				{
					uint32_t conv = value.toInt(); // TODO see if works with large values ?????
					ret = debugAddWatchUInt32_t((globalNum + 1), operation, conv, allwaysStop);
				}
				break;
			case DEBUG_TYPE_CHAR_ARRAY:
				{
					ret = debugAddWatchCharArray(globalNum, operation, value.c_str(), allwaysStop);
				}
				break;
			case DEBUG_TYPE_STRING:
				{
					ret = debugAddWatchString(globalNum, operation, value, allwaysStop);
				}
				break;
		}
	}

	// Return

	if (ret != -1) {
		printLibrary();
		PRINTFLN(F("Watch added with sucess:"));
		showWatch(ret);
		PRINTFLN("*");
	}

	return ret;
}

// Add a cross watch

static int8_t addWatchCross(Fields& fields) {

	int8_t ret = -1; // Return
	uint8_t pos = 2; // First field

	// Global number 1

	uint8_t globalNum1;

	if (fields.isNum(pos)) { // By num

		globalNum1 = fields.getInt(pos);

		if (globalNum1 == 0 || globalNum1 > _debugGlobalsAdded) {

			printLibrary();
			PRINTFLN(F("Invalid index for global1\r\n*"));
			return false;
		}

		globalNum1--; // Globals index start in 0

	} else { // By name

		 if (!(findGlobal(fields.getString(pos).c_str(), &globalNum1, false))) {
			 return false;
		 }
	}

	//D("globalnum1=%u", globalNum1);

	// Verify operation

	pos++;

	uint8_t operation = 0;

	if (!(getWatchOperation(fields.getString(pos), &operation))) {

		printLibrary();
		PRINTFLN(F("Invalid operation\r\n*"));
		return false;

	}

	if (operation == DEBUG_WATCH_CHANGED) { // Not for changed type

		printLibrary();
		PRINTFLN(F("Invalid operation - not can be of change type\r\n*"));
		return false;

	}

	// Global number 2

	pos++;

	uint8_t globalNum2;

	if (fields.isNum(pos)) { // By num

		globalNum2 = fields.getInt(pos);

		if (globalNum2 == 0 || globalNum2 > _debugGlobalsAdded) {

			printLibrary();
			PRINTFLN(F("Invalid index for global2\r\n*"));
			return false;
		}

		globalNum2--; // Globals index start in 0

	} else { // By name

		 if (!(findGlobal(fields.getString(pos).c_str(), &globalNum2, false))) {
			 return -1;
		 }
	}

	//D("globalnum2=%u", globalNum1);

	if (globalNum2 == globalNum1) {

		printLibrary();
		PRINTFLN(F("Global2 must be different than global1\r\n*"));
		return false;
	}

	// Verify allways stop

	pos++;

	boolean allwaysStop = false;

	if (fields.size() == pos) {

		allwaysStop = (fields.getString(pos) == "as");

	}

	// Add watch cross (these funcions work with index of global start by 1)

	ret = debugAddWatchCross((globalNum1 + 1), operation, (globalNum1 + 2), allwaysStop);

	// Return

	if (ret != -1) {
		printLibrary();
		PRINTFLN(F("Watch added with sucess:"));
		showWatch(ret);
	}

	return ret;
}

// Show list of watches for globals available
// Used to search and show all (return total showed)
// Or to search and show one (return number showed)

static int8_t showWatches(String& options, boolean debugSerialApp) {

	// Show all ?

	boolean showAll = false;
	int8_t byNumber = -1;

	// Searching ?

	if (options.length() == 0) {

		showAll = true;

	} else {

		// Is integer ?

		if (strIsNum(options)) {

			uint8_t num = options.toInt();

			//D("byNumber %s = %d", options.c_str(), num);

			if (num > 0) { // Find by number, else by exact name

				byNumber = num;
				showAll = false;

			} else {

				printLibrary();
				PRINTFLN(F("Option not is a number valid (>0)"));
				return -1;

			}

		} else {

			printLibrary();
			PRINTFLN(F("Option not is a number"));
			return -1;
		}

	}

	// Show global(s)

	if (!debugSerialApp) { // Not for SerialDebugApp connection

		printLibrary();
		if (showAll) {
			PRINTFLN(F("Showing all watches for global variables (%u):"), _debugWatchesAdded);
		} else {
			PRINTFLN(F("Searching and showing watch for global variable:"));
		}
	}

	int8_t showed = 0;

	// Process

	for (uint8_t i=0; i < _debugWatchesAdded; i++) {

		String name = "";
		boolean show = showAll;

		// Show ?

		if (byNumber != -1) {
			show = ((i + 1) == byNumber);
		}

		if (show) {

			// Get global num

			uint8_t globalNum = _debugWatches[i].globalNum;

			if (globalNum > _debugGlobalsAdded) {

				printLibrary();
				PRINTFLN(F("Invalid index for global variable in watch\r\n*"));
				return -1;
			}

			// Show

			if (showWatch(i, debugSerialApp)) {

				showed++;

			}
		}
	}

	if (showAll) {
		PRINTFLN("*");
	}

	// Help

	if (!debugSerialApp) { // Not for SerialDebugApp connection

		boolean doBreak = false;

		if (showed > 0) {

			PRINTFLN("*");
			printLibrary();
			PRINTLN();
			PRINTFLN(F("* To show: wa [num]"));
			PRINTFLN(F("* To add: wa a {global [name|number]} [==|!=|<|>|<=|>=|change] [value] [as]"));
			PRINTFLN(F("* note: as -> set watch to stop always"));
			PRINTFLN(F("* To add cross (2 globals): wa ac {global [name|number]} [=|!=|<|>|<=|>=] {global [name|number]} [as]"));
			PRINTFLN(F("* To change: wa u {global [name|number]} [==|!=|<|>|<=|>=|change] [value] [as]"));
			PRINTFLN(F("* To change cross (not yet implemented)"));
			PRINTFLN(F("* To disable: wa d [num|all]"));
			PRINTFLN(F("* To enable: wa e [num|all]"));
			PRINTFLN(F("* To nonstop on watches: wa ns"));
			PRINTFLN(F("* To stop on watches: wa s"));

			doBreak = true;

		} else {

			printLibrary();
			PRINTFLN(F("Watch not found."));

			doBreak = true;
		}

		// Do break ?

		if (doBreak) {
			String response = debugBreak();
			if (response.length() > 0) {
				processCommand(response, false, false);
			}
		}
	}

	// Return

	return showed;

}

// Show watch

static boolean showWatch(uint8_t watchNum, boolean debugSerialApp) {

	if (watchNum >= _debugWatchesAdded) {

		printLibrary();
		PRINTFLN(F("Invalid index for watch \r\n*"));
		return false;
	}

	debugWatch_t* watch = &_debugWatches[watchNum];

	// Show

	if (debugSerialApp) { // For DebugSerialApp connection ?

		// Operation

		String oper = "";

		switch (watch->operation) {
			case DEBUG_WATCH_CHANGED:
				oper = "chg";
				break;
			case DEBUG_WATCH_EQUAL:
				oper = "==";
				break;
			case DEBUG_WATCH_DIFF:
				oper = "!=";
				break;
			case DEBUG_WATCH_LESS:
				oper = "<";
				break;
			case DEBUG_WATCH_GREAT:
				oper = ">";
				break;
			case DEBUG_WATCH_LESS_EQ:
				oper = "<=";
				break;
			case DEBUG_WATCH_GREAT_EQ:
				oper = ">=";
				break;
		}

		// Is cross (between 2 globals)?

		if (watch->watchCross) {

			PRINTFLN(F("$app:W:a:c:%u:%u:%s:%u:%c:%c:%c"),
					(watchNum + 1),
					(watch->globalNum + 1),
					oper.c_str(),
					(watch->globalNumCross + 1),
					(watch->enabled)?'1':'0',
					(watch->alwaysStop)?'1':'0',
					(watch->triggered)?'1':'0');

		} else { // Normal

			// Global

			debugGlobal_t* global = &_debugGlobals[watch->globalNum];

			// Value

			String value = "";
			String type = "";

			if (watch->operation != DEBUG_WATCH_CHANGED) {

				getStrValue(global->type, watch->pointerValue, global->showLength, true, value, type);
			}

			PRINTFLN(F("$app:W:a:n:%u:%u:%s:%s:%c:%c:%c"),
					(watchNum + 1),
					(watch->globalNum + 1),
					oper.c_str(),
					value.c_str(),
					(watch->enabled)?'1':'0',
					(watch->alwaysStop)?'1':'0',
					(watch->triggered)?'1':'0');

		}

	} else { // For monitor serial

		PRINTF(F("* %02u {global "), (watchNum + 1));

		if (showGlobal((watch->globalNum + 1), DEBUG_SHOW_GLOBAL_WATCH, false)) {

			// Operation

			String oper = "";

			switch (watch->operation) {
				case DEBUG_WATCH_CHANGED:
					oper = "change";
					break;
				case DEBUG_WATCH_EQUAL:
					oper = "==";
					break;
				case DEBUG_WATCH_DIFF:
					oper = "!=";
					break;
				case DEBUG_WATCH_LESS:
					oper = "<";
					break;
				case DEBUG_WATCH_GREAT:
					oper = ">";
					break;
				case DEBUG_WATCH_LESS_EQ:
					oper = "<=";
					break;
				case DEBUG_WATCH_GREAT_EQ:
					oper = ">=";
					break;
			}

			// Is cross (between 2 globals)?

			if (watch->watchCross) {

				PRINTF(F("} %s {global "), oper.c_str());

				if (showGlobal((watch->globalNumCross + 1),  DEBUG_SHOW_GLOBAL_WATCH, false)) {

					PRINTFLN(F("} (%s) %s"), \
							((watch->enabled)?"enabled":"disabled"), \
							((watch->alwaysStop)?"(allwaysStop)":""));

				}

			} else { // Simple watch

				// Global

				if (watch->globalNum > _debugGlobalsAdded) {

					printLibrary();
					PRINTFLN(F("Invalid index for global in watch\r\n*"));
					return false;
				}

				debugGlobal_t* global = &_debugGlobals[watch->globalNum];

				// Value

				String value = "";
				String type = "";

				if (watch->operation != DEBUG_WATCH_CHANGED) {

					getStrValue(global->type, watch->pointerValue, global->showLength, true, value, type);

					PRINTFLN(F("} %s %s (%s) %s"), oper.c_str(), value.c_str(), \
							((watch->enabled)?"enabled":"disabled"), \
							((watch->alwaysStop)?"(allwaysStop)":""));

				} else { // When changed

					String oldValue = "";
					String oldType = "";

					getStrValue(global->type, global->pointerOld, global->showLength, true, oldValue, oldType);

					PRINTFLN(F("} %s (old value: %s) (%s) %s"), oper.c_str(), oldValue.c_str(), \
							((watch->enabled)?"enabled":"disabled"), \
							((watch->alwaysStop)?"(allwaysStop)":""));
				}
			}

			return true;
		}
	}

	return false;

}

// Change watches of global variables

static boolean changeWatch(Fields& fields) {

	uint8_t pos = 2; // Field possition

	// Watch number

	uint8_t watchNum;

	if (fields.isNum(pos)) { // By num

		watchNum = fields.getInt(pos);

		if (watchNum == 0 || watchNum > _debugWatchesAdded) {

			printLibrary();
			PRINTFLN(F("Invalid index for watch\r\n*"));
			return false;
		}

		watchNum--; // Watch index start in 0

	} else {

		printLibrary();
		PRINTFLN(F("Invalid watch number\r\n*"));

		return false;
	}

	// Global number

	pos++;

	uint8_t globalNum;

	if (fields.isNum(pos)) { // By num

		globalNum = fields.getInt(pos);

		if (globalNum == 0 || globalNum > _debugGlobalsAdded) {

			printLibrary();
			PRINTFLN(F("Invalid index for global in watch\r\n*"));
			return false;
		}

		globalNum--; // Globals index start in 0

	} else { // By name

		 if (!(findGlobal(fields.getString(pos).c_str(), &globalNum, false))) {
			 return false;
		 }
	}

	//D("globalnum=%u", globalNum);

	// Verify operation

	pos++;

	uint8_t operation = 0;

	if (!(getWatchOperation(fields.getString(pos), &operation))) {

		printLibrary();
		PRINTFLN(F("Invalid operation\r\n*"));
		return false;

	}

	// Verify value

	String value = "";

	if (operation != DEBUG_WATCH_CHANGED) { // Not for changed type

		pos++;

		switch (_debugGlobals[globalNum].type) {
			case DEBUG_TYPE_INT:
			case DEBUG_TYPE_U_INT:
			case DEBUG_TYPE_LONG:
			case DEBUG_TYPE_U_LONG:
			case DEBUG_TYPE_FLOAT:
			case DEBUG_TYPE_DOUBLE:
			case DEBUG_TYPE_INT8_T:
			case DEBUG_TYPE_INT16_T:
			case DEBUG_TYPE_INT32_T:
			case DEBUG_TYPE_UINT8_T:
			case DEBUG_TYPE_UINT16_T:
			case DEBUG_TYPE_UINT32_T:

				// Is number ?

				if (!(fields.isNum(pos))) {

					printLibrary();
					PRINTFLN(F("The value must be numeric\r\n*"));
					return false;

				}
				break;
		}

		// Get value

		value = fields.getString(pos);

		// Return the spaces

		char conv = 31;
		value.replace(conv, ' ');

	}

	// Verify allways stop

	pos++;

	boolean allwaysStop = false;

	if (fields.size() == pos) {

		allwaysStop = (fields.getString(pos) == "as");

	}

	// Get watch and this global

	debugWatch_t *watch = &_debugWatches[watchNum];
	debugGlobal_t *global = &_debugGlobals[globalNum];

	// Update watch

	watch->globalNum = globalNum;
	watch->operation = operation;
	watch->alwaysStop = allwaysStop;
	watch->typeValue = _debugGlobals[globalNum].type;

	if (operation == DEBUG_WATCH_CHANGED) { // Changed type

		if (watch->pointerValue) { // Free it
			free (watch->pointerValue);
		}

	} else { // Anothers

		// From type of global

		switch (_debugGlobals[globalNum].type) {
			case DEBUG_TYPE_BOOLEAN:
				{
					boolean conv = (value == "1" || value == "t" || value == "true");
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_INT:
				{
					int conv = value.toInt();
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_U_INT:
				{
					unsigned int conv = value.toInt();
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_LONG:
				{
					long conv = value.toInt(); // TODO see if works with large values ?????
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_U_LONG:
				{
					unsigned long conv = value.toInt(); // TODO see if works with large values ?????
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_FLOAT:
				{
					float conv = value.toFloat();
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_DOUBLE:
				{
					double conv = value.toFloat(); // TODO see if works with large values ?????
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_INT8_T:
				{
					int8_t conv = value.toInt();
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_INT16_T:
				{
					int16_t conv = value.toInt();
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_INT32_T:
				{
					int32_t conv = value.toInt();
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_UINT8_T:
				{
					uint8_t conv = value.toInt();
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_UINT16_T:
				{
					uint16_t conv = value.toInt();
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_UINT32_T:
				{
					uint32_t conv = value.toInt(); // TODO see if works with large values ?????
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));
				}
				break;
			case DEBUG_TYPE_CHAR_ARRAY:
				{
					const char* conv = value.c_str();
					if (watch->pointerValue) { // Free it first
						free (watch->pointerValue);
					}
					updateValue(global->type, &conv, global->type, &(watch->pointerValue));

				}
				break;
			case DEBUG_TYPE_STRING:
				{
					const char* conv = value.c_str();
					if (watch->pointerValue) { // Free it first
						free (watch->pointerValue);
					}
					updateValue(global->type, &conv, DEBUG_TYPE_CHAR_ARRAY, &(watch->pointerValue));
				}
				break;
		}
	}

	// Return

	printLibrary();
	PRINTFLN(F("Watch updated with sucess:"));
	showWatch(watchNum);
	PRINTFLN("*");

	return true;
}

// Verify global type

static boolean verifyGlobalType(uint8_t globalNum, uint8_t type) {

	if (globalNum == 0 || globalNum > _debugGlobalsAdded) {

		printLibrary();
		PRINTFLN(F("Invalid index for global in watch\r\n*"));
		return false;
	}

	boolean ret = (_debugGlobals[globalNum - 1].type == type);

	if (!ret) {

		printLibrary();
		PRINTFLN(F("Invalid type for global in watch\r\n*"));
	}

	return ret;
}

// Get operation type for watch from string

boolean getWatchOperation(String str, uint8_t* operation) {

	int8_t oper = -1;

	if (str.length() == 0) {
		oper = DEBUG_WATCH_CHANGED;
	} else if (str == "==") {
		oper = DEBUG_WATCH_EQUAL;
	} else if (str == "!=") {
		oper = DEBUG_WATCH_DIFF;
	} else if (str == "<") {
		oper = DEBUG_WATCH_LESS;
	} else if (str == ">") {
		oper = DEBUG_WATCH_GREAT;
	} else if (str == "<=") {
		oper = DEBUG_WATCH_LESS_EQ;
	} else if (str == ">=") {
		oper = DEBUG_WATCH_GREAT_EQ;
	}

	if (oper != -1) {
		*operation = oper;
		return true;
	} else {
		return false;
	}
}

#endif // Not low memory board

// Aplly the operation between two *void pointers values

static boolean apllyOperation(uint8_t type1, void* pointer1, uint8_t operation, uint8_t type2, void* pointer2) {

	// Process types and values)

	//D("apllyOperation(type1=%u pointer1=%p, operation=%u, type2=%u, pointer2=%p",
	//		type1,pointer1, operation, type2, pointer2);

	if (pointer1 && pointer2) { // Only if 2 pointer has data

		switch (type1) {

			case DEBUG_TYPE_BOOLEAN:
				{
					boolean value1 = *(boolean*) pointer1;
					boolean value2 = *(boolean*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_CHAR:
				{
					char value1 = *(char*) pointer1;
					char value2 = *(char*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_INT:
				{
					int value1 = *(int*) pointer1;
					int value2 = *(int*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_U_LONG:
				{
					unsigned long value1 = *(unsigned long*) pointer1;
					unsigned long value2 = *(unsigned long*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

			case DEBUG_TYPE_BYTE:
				{
					byte value1 = *(byte*) pointer1;
					byte value2 = *(byte*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_U_INT:
				{
					unsigned int value1 = *(unsigned int*) pointer1;
					unsigned int value2 = *(unsigned int*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_LONG:
				{
					long value1 = *(long*) pointer1;
					long value2 = *(long*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_FLOAT:
				{
					float value1 = *(float*) pointer1;
					float value2 = *(float*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_DOUBLE:
				{
					double value1 = *(double*) pointer1;
					double value2 = *(double*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_INT8_T:
				{
					int8_t value1 = *(int8_t*) pointer1;
					int8_t value2 = *(int8_t*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_INT16_T:
				{
					int16_t value1 = *(int16_t*) pointer1;
					int16_t value2 = *(int16_t*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;
			case DEBUG_TYPE_INT32_T:
				{
					int32_t value1 = *(int32_t*) pointer1;
					int32_t value2 = *(int32_t*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_UINT8_T:
				{
					uint8_t value1 = *(uint8_t*) pointer1;
					uint8_t value2 = *(uint8_t*) pointer2;

					//D("aplly v1=%u v2=%u", value1, value2);

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_UINT16_T:
				{
					uint16_t value1 = *(uint16_t*) pointer1;
					uint16_t value2 = *(uint16_t*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_UINT32_T:
				{
					uint32_t value1 = *(uint32_t*) pointer1;
					uint32_t value2 = *(uint32_t*) pointer2;

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (value1 != value2);
							break;

						case DEBUG_WATCH_EQUAL:
							return (value1 == value2);
							break;

						case DEBUG_WATCH_LESS:
							return (value1 < value2);
							break;

						case DEBUG_WATCH_GREAT:
							return (value1 > value2);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (value1 <= value2);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (value1 >= value2);
							break;

						default:
							return false;
							break;
					}
				}
				break;

			case DEBUG_TYPE_CHAR_ARRAY:
				{
					const char* value1 = (const char*) pointer1;
					const char* value2 = (const char*) pointer2;

					int ret = strncmp(value1, value2, DEBUG_MAX_CMP_STRING); // Compare max n characters to avoid problems - TODO see another ways

					//D("aplly v1=%s v2=%s ret=%u", value1, value2, ret);

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (ret != 0);
							break;

						case DEBUG_WATCH_EQUAL:
							return (ret == 0);
							break;

						case DEBUG_WATCH_LESS:
							return (ret < 0);
							break;

						case DEBUG_WATCH_GREAT:
							return (ret > 0);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (ret <= 0);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (ret >= 0);
							break;

						default:
							return false;
							break;
					}
				}
				break;


#endif // LOW_MEMORY

			case DEBUG_TYPE_STRING:
				{
					// Allways compare char array, due pointer2 can be of this type

					const char* value1;
					const char* value2;

					String temp = *(String*) pointer1;
					value1 = temp.c_str();

					switch (type2) {
						case DEBUG_TYPE_STRING:
							temp = *(String*) pointer2;
							value2 = temp.c_str();
							break;
						case DEBUG_TYPE_CHAR_ARRAY: // Old values of global string is a char array
							value2 = (const char*) pointer2;
							break;
						default:
							return false;
							break;
					}

					// Compare

					int ret = strncmp(value1, value2, DEBUG_MAX_CMP_STRING); // Compare max n characters to avoid problems - TODO see another ways

					//D("apply v1=%s v2=%s p2=%p ret=%d", value1, value2, pointer2, ret);

					// Aplly operation

					switch (operation) {

						case DEBUG_WATCH_CHANGED:
						case DEBUG_WATCH_DIFF:
							return (ret != 0);
							break;

						case DEBUG_WATCH_EQUAL:
							return (ret == 0);
							break;

						case DEBUG_WATCH_LESS:
							return (ret < 0);
							break;

						case DEBUG_WATCH_GREAT:
							return (ret > 0);
							break;

						case DEBUG_WATCH_LESS_EQ:
							return (ret <= 0);
							break;

						case DEBUG_WATCH_GREAT_EQ:
							return (ret >= 0);
							break;

						default:
							return false;
							break;
					}

				}
				break;
		}

		return false;

	} else {

		if (!pointer2) { // No value -> it is changed
			return true;
		} else {
			return false;
		}
	}
}

// Find a global variable added by name

static boolean findGlobal (const char* globalName, uint8_t* globalNum, boolean sumOne) {

	String find = globalName;
	String name = "";

	for (uint8_t i=0; i < _debugGlobalsAdded; i++) {

		// Get name

		if (_debugGlobals[i].name) { // Memory

			name = _debugGlobals[i].name;
#ifdef DEBUG_USE_FLASH_F

		} else if (_debugGlobals[i].nameF) { // For Flash F

			name = String(_debugGlobals[i].nameF);
#endif
		}

		if (name == find) {
			*globalNum = (sumOne)? (i + 1) : i;
			return true;
		}
	}

	// Not find

	printLibrary();
	PRINTFLN(F("Global mame not found: %s"), globalName);

	return false;
}

// Get a string value from a void pointerValue

static void getStrValue(uint8_t type, void* pointer, uint8_t showLength, boolean showSize, String& response, String& responseType) {

	response = "";

	if (responseType) responseType = "";

	if (!pointer) { // Not has value
		response = "?";
		return;
	}

	switch (type) {

		// Basic types

		case DEBUG_TYPE_BOOLEAN:
			response = ((*(boolean*)pointer) ? F("true") : F("false"));
			if (responseType) responseType = F("boolean");
			break;
		case DEBUG_TYPE_CHAR:
			response = '\'';
			response.concat(String(*(char*)pointer));
			response.concat('\'');
			if (responseType) responseType = F("char");
			break;
		case DEBUG_TYPE_INT:
			response = String(*(int*)pointer);
			if (responseType) responseType = F("int");
			break;
		case DEBUG_TYPE_U_LONG:
			response = String(*(unsigned long*)pointer);
			if (responseType) responseType = F("unsigned long");
			break;
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
		case DEBUG_TYPE_BYTE:
			response = String(*(byte*)pointer);
			if (responseType) responseType = F("byte");
			break;
		case DEBUG_TYPE_U_INT:
			response = String(*(unsigned int*)pointer);
			if (responseType) responseType = F("unsigned int");
			break;
		case DEBUG_TYPE_LONG:
			response = String(*(long*)pointer);
			if (responseType) responseType = F("long");
			break;
		case DEBUG_TYPE_FLOAT:
			response = String(*(float*)pointer);
			if (responseType) responseType = F("float");
			break;
		case DEBUG_TYPE_DOUBLE:
			response = String(*(double*)pointer);
			if (responseType) responseType = F("double");
			break;

		// Integer C size _t

		case DEBUG_TYPE_INT8_T:
			response = String(*(int8_t*)pointer);
			if (responseType) responseType = F("int8_t");
			break;
		case DEBUG_TYPE_INT16_T:
			response = String(*(int16_t*)pointer);
			if (responseType) responseType = F("int16_t");
			break;
		case DEBUG_TYPE_INT32_T:
			response = String(*(int32_t*)pointer);
			if (responseType) responseType = F("int32_t");
			break;
//#ifdef ESP32
//		case DEBUG_TYPE_INT64_T:
//			response = String(*(int64_t*)pointer);
//			if (responseType) responseType = F("int64_t");
//			break;
//#endif
		// Unsigned integer C size _t

		case DEBUG_TYPE_UINT8_T:
			response = String(*(uint8_t*)pointer);
			if (responseType) responseType = F("uint8_t");
			break;
		case DEBUG_TYPE_UINT16_T:
			response = String(*(uint16_t*)pointer);
			if (responseType) responseType = F("uint16_t");
			break;
		case DEBUG_TYPE_UINT32_T:
			response = String(*(uint32_t*)pointer);
			if (responseType) responseType = F("uint32_t");
			break;
//#ifdef ESP32
//			case DEBUG_TYPE_UINT64_T:
//					response = String(*(uint64_t*)pointer);
//					if (responseType) responseType = "uint64_t";
//					break;
//#endif
		// Strings

		case DEBUG_TYPE_CHAR_ARRAY:
			{
				String show = String((char*)pointer);
				size_t size = show.length();

				if (showLength > 0 &&
					size > showLength) {
					show = show.substring(0, showLength);
					show.concat(F("..."));
				}
				response = '\"';
				response.concat(show);
				response.concat('\"');
				if (showSize) {
					response.concat(F(" (size:"));
					response.concat(size);
					response.concat(")");
				}
				if (responseType) responseType = F("char array");
			}
			break;
#endif// Not low memory board

		case DEBUG_TYPE_STRING:
			{
				String show = *(String*)pointer;
				size_t size = show.length();
				if (showLength > 0 &&
						size > showLength) {
					show = show.substring(0, showLength);
					show.concat(F("..."));
				}
				response = '\"';
				response.concat(show);
				response.concat('\"');
				if (showSize) {
					response.concat(F(" (size:"));
					response.concat(size);
					response.concat(")");
				}
				if (responseType) responseType = F("String");
			}
			break;
	}
}

// Update to value in pointer from another pointer

static void updateValue(uint8_t typeFrom, void* pointerFrom, uint8_t typeTo, void** pointerTo) {

	D("updateValue from type=%u pointer=%p to type=%u pointer=%p",
			typeFrom, pointerFrom, typeTo, *pointerTo);

	if (!pointerFrom) {
		printLibrary();
		PRINTFLN(F("source value empty"));
		return;
	}

	// Update

	switch (typeTo) {

		// Basic types

		case DEBUG_TYPE_BOOLEAN:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(boolean);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_CHAR:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(char);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_INT:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(int);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_U_LONG:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(unsigned long);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

#ifndef BOARD_LOW_MEMORY // Not for low memory boards

		case DEBUG_TYPE_BYTE:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(byte);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_U_INT:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(unsigned int);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_LONG:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(long);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_FLOAT:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(float);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_DOUBLE:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(double);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		// Integer C size _t

		case DEBUG_TYPE_INT8_T:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(int8_t);
				//D("size=%u", size);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_INT16_T:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(int16_t);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

		case DEBUG_TYPE_INT32_T:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(int32_t);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;

//#ifdef ESP32
//		case DEBUG_TYPE_INT64_T:
//			break;
//#endif

		// Unsigned integer C size _t

		case DEBUG_TYPE_UINT8_T:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(uint8_t);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;
		case DEBUG_TYPE_UINT16_T:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(uint16_t);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;
		case DEBUG_TYPE_UINT32_T:
			{
				// Alloc memory for pointer (if need) and copy value
				size_t size = sizeof(uint32_t);
				if (!*pointerTo) {
					*pointerTo = malloc (size);
				}
				memcpy(*pointerTo, pointerFrom, size);
			}
			break;
//#ifdef ESP32
//		case DEBUG_TYPE_UINT64_T:
//			break;
//#endif

#endif // Not low memory board

		// Strings

		case DEBUG_TYPE_CHAR_ARRAY:
			{
				// Always free it before, due size can be changed // TODO optimize it
				if (*pointerTo) {
					free (*pointerTo);
				}
				const char* content;
				size_t size;
				// Value is string ?
				if (typeFrom == DEBUG_TYPE_STRING) {
					String temp = *(String*)pointerFrom;
					size = temp.length();
					//D("upd str temp %s size %d", temp.c_str(), size);
					// Alloc memory for pointerValue and copy value
					*pointerTo = malloc (size+1);
					memset(*pointerTo, '\0', (size+1));
					memcpy(*pointerTo, temp.c_str(), size);
				} else {
					content = (const char*)pointerFrom;
					size = strlen(content);
					// Alloc memory for pointerValue and copy value
					*pointerTo = malloc (size+1);
					memset(*pointerTo, '\0', (size+1));
					memcpy(*pointerTo, content, size);
				}
				//D("upd char* %s size %d", *pointerTo, size);
			}
			break;

		case DEBUG_TYPE_STRING:
			{
				// String not allowed to update
				printLibrary
			();
				PRINTFLN(F("String not allow updated"));
			}
			break;
	}

	//D("updateValue after from type=%u pointer=%p to type=%u pointer=%p",
	//		typeFrom, pointerFrom, typeTo, *pointerTo);

}


#ifndef DEBUG_DISABLE_DEBUGGER

// Process functions commands

static void processFunctions(String& options) {

	// Get fields of command options

	Fields fields(options, ' ', true);

	if (fields.size() > 2) {
		printLibrary();
		PRINTFLN(F("Invalid sintax for functions. use f ? to show help"));
		return;
	}

	String option = (fields.size() >= 1)? fields.getString(1) : "";

	//D("procf opts=%s flds=%d opt=%s", options.c_str(), fields.size(), option.c_str());

	if (_debugFunctionsAdded > 0) {

		if (option.length() == 0 || option == "?") {

			// Just show functions and help

			option = "";
			showFunctions(option,false);

		} else if (option.indexOf('*') >= 0) {

			// Search by name (case insensitive)

			showFunctions(options, false);

		} else {

			// Call a function

			callFunction(options);
		}

	} else {

		printLibrary();
		PRINTFLN(F("Functions not added to SerialDebug in your project"));
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
		PRINTFLN(F("* See how do it in advanced example"));
#endif
	}
}

// Show list of functions available
// Used to search and show all (return total showed)
// Or to search and show one (return number of item showed)

static int8_t showFunctions(String& options, boolean one, boolean debugSerialApp) {

	// Show all ?

	boolean showAll = false;
	boolean byStartName = false;
	int8_t byNumber = -1;

	_debugString = "";

	// Searching ?

	if (options.length() == 0) {

		if (one) {
    		printLibrary();
			PRINTFLN(F("Option not informed"));
			return -1;
		}

		showAll = true;

	} else {

		// Search by start name (case insensitive)

		int8_t pos = options.indexOf('*');

		//D("pos %d", pos);

		if (pos > 0) {

			if (one) {

				printLibrary();
				PRINTFLN(F("* not allowed, use name or number"));
				return -1;
			}

			options = options.substring(0, pos);
			options.toLowerCase(); // Case insensitive

			byStartName = true;
			//D("byName %s", options.c_str());

		} else {

			// Is integer ?

			if (strIsNum(options)) {

				uint8_t num = options.toInt();

				//D("byNumber %s = %d", options.c_str(), num);

				if (num > 0) { // Find by number, else by exact name

					if (num > _debugFunctionsAdded) {

						printLibrary();
						PRINTFLN(F("Function number must between 1 and %u"), _debugFunctionsAdded);
						return -1;
					}

					byNumber = num;
				}
			}
		}

		showAll = false;
	}

	// Show function(s)

	if (!debugSerialApp) { // Not for SerialDebugApp connection

		if (showAll) {
			printLibrary();
			PRINTFLN(F("Showing all functions (%u):\r\n*"), _debugFunctionsAdded);
		} else if (!one) {
			printLibrary();
			PRINTFLN(F("Searching and showing functions:"));
		}
	}

	int8_t showed = (!one)? 0 : -1;

	for (uint8_t i=0; i < _debugFunctionsAdded; i++) {

		String type = "";
		String name = "";
		boolean show = showAll;

		// Get name

		if (_debugFunctions[i].name) { // Memory

			name = _debugFunctions[i].name;

#ifdef DEBUG_USE_FLASH_F
		} else if (_debugFunctions[i].nameF) { // For Flash F

			name = String(_debugFunctions[i].nameF);
#endif
		}

		// Show ?

		if (!showAll) {

			if (byStartName) {
				String tolower = name;
				tolower.toLowerCase(); // Case insensitive
				show = (tolower.startsWith(options));
			} else if (byNumber > 0) {
				show = ((i + 1) == byNumber);
			} else {
				show = (name == options);
			}
		}

		//D("showFunction options = %s name: %s showAll=%d show=%d", options.c_str(), name.c_str(), showAll, show);

		if (show) {

			switch (_debugFunctions[i].argType) {
				case DEBUG_TYPE_FUNCTION_VOID:
					type = "";
					break;
				case DEBUG_TYPE_STRING:
					type = "String";
					break;
				case DEBUG_TYPE_CHAR:
					type = "char";
					break;
				case DEBUG_TYPE_INT:
					type = "int";
					break;
				default:
					break;
			}

			if (one) { // One search ?

				printLibrary();
				PRINTFLN(F("Function found: %02u %s(%s)"), (i + 1), name.c_str(), type.c_str());

#ifdef DEBUG_USE_FLASH_F
				if (_debugFunctions[i].nameF) { // For Flash F - save the content in variable in memory, to not extract Flash again
					_debugString = name;
				}
#endif
				showed = i; // Return the index

				break;

			} else { // Description, not for one search

				if (debugSerialApp) { // For DebugSerialApp connection ?

					PRINTF(F("$app:F:%u:%s:%s:"), (i + 1), name.c_str(), type.c_str());

#ifndef BOARD_LOW_MEMORY // Not for low memory boards
					if (_debugFunctions[i].description) { // Memory
						PRINTFLN(F(":%s"), _debugFunctions[i].description);
#ifdef DEBUG_USE_FLASH_F
					} else if (_debugFunctions[i].descriptionF) { // For Flash F, multiples print
						PRINTF(':');
						PRINTFLN(_debugFunctions[i].descriptionF);
#endif
					} else {
						PRINTLN();
					}
#else
					PRINTLN();
#endif

				} else { // For monitor serial

					PRINTF(F("* %02u %s(%s)"), (i + 1), name.c_str(), type.c_str());

#ifndef BOARD_LOW_MEMORY // Not for low memory boards
					if (_debugFunctions[i].description) { // Memory
						PRINTFLN(F(" // %s"), _debugFunctions[i].description);
#ifdef DEBUG_USE_FLASH_F
					} else if (_debugFunctions[i].descriptionF) { // For Flash F, multiples print
						PRINTF(F(" // "));
						PRINTFLN(_debugFunctions[i].descriptionF);
#endif
						} else {
						PRINTLN();
					}
#else
					PRINTLN();
#endif
				}

				showed++;

			}
		}
	}

	if (showAll) {
		PRINTFLN("*");
	}

	// Help

	if (!debugSerialApp) {

		boolean doBreak = false;

		if (!one && showed > 0) {

			if (!_debugRepeatCommand && !debugSerialApp) { // Not repeating and not for SerialDebugApp connection

				printLibrary();
				PRINTLN();
				PRINTFLN(F("* To show: f [name|num]"));
				PRINTFLN(F("* To search with start of name (case insensitive): f name*"));
				PRINTFLN(F("* To call it, just command: f [name|number] [arg]\r\n*"));

				doBreak = true;
			}

		} else if (!one || showed == -1) {

			printLibrary();
			PRINTFLN(F("Function not found."));

			doBreak = true;
		}

		// Do break

		if (doBreak) {
			String response = debugBreak();
			if (response.length() > 0) {
				processCommand(response, false, false);
			}
		}
	}

	// Return

	return showed;

}

// Call a function

static void callFunction(String& options) {

	// Extract options

	String funcId = "";
	String funcArg = "";

	int8_t pos = options.indexOf(' ');
	if (pos > 0) {
		funcId = options.substring(0, pos);
		funcArg = options.substring(pos + 1);
	} else {
		funcId = options;
	}

	//D("callFunction: id %s arg %s", funcId.c_str(), funcArg.c_str());

	// Return the spaces

	char conv = 31;
	funcArg.replace(conv, ' ');

	// Find and show a function (one)

	int8_t num = showFunctions(funcId, true);

	if (num == -1) { // Not found or error

		return;

	}

	//D("callFunction: num %u", num);

	// Call the function

	unsigned long timeBegin = 0;

	printLibrary();
	if (_debugFunctions[num].name) { // Memory
		PRINTF(F("Calling function %u -> %s("), (num + 1), _debugFunctions[num].name);
#ifdef DEBUG_USE_FLASH_F
	} else if (_debugFunctions[num].nameF) { // Use a temporary var to not get flash again
		PRINTF(F("Calling function %u -> %s("), (num + 1), _debugString.c_str());
#endif
	}

	if (!_debugFunctions[num].callback) { // Callback not set ?

		PRINTFLN(F(") - no callback set for function"));
		return;

	}

	if (funcArg.length() == 0 &&
		!(_debugFunctions[num].argType == DEBUG_TYPE_FUNCTION_VOID ||
			_debugFunctions[num].argType == DEBUG_TYPE_STRING ||
			_debugFunctions[num].argType == DEBUG_TYPE_CHAR_ARRAY)) {  // For others can not empty

		PRINTFLN(F(") - argument not informed"));
		return;
	}

	// Exit from silence mode

#ifdef DEBUGGER_FOR_SERIALDEBUG
	if (_debugSilence) {
		debugSilence(false, false);
	}
#else // RemoteDebug

	if (_Debug && _Debug->isSilence()) {
		_Debug->silence(false, false);
	}
#endif

	// Process

	bool called = false;

	switch (_debugFunctions[num].argType) {

		case DEBUG_TYPE_FUNCTION_VOID:
			{
				// Void arg

				PRINTFLN(F(") ..."));
				delay(500);
				timeBegin = micros();

				_debugFunctions[num].callback();
				called = true;
			}
			break;

		case DEBUG_TYPE_STRING:
			{
				// String arq

				if (funcArg.indexOf('"') != -1) {
					PRINTFLN(F("%s) ..."), funcArg.c_str());
				} else {
					PRINTFLN(F("\"%s\") ..."), funcArg.c_str());
				}
				delay(500);

				removeQuotation(funcArg, false);

				timeBegin = micros();

				void (*callback)(String) = (void(*) (String))_debugFunctions[num].callback;

				callback(funcArg);
				called = true;
			}
			break;

		case DEBUG_TYPE_CHAR:
			{
				// Arg char

				PRINTFLN(F("%s) ..."), funcArg.c_str());
				delay(500);

				removeQuotation(funcArg, true);

				timeBegin = micros();

				void (*callback)(char) = (void(*) (char))_debugFunctions[num].callback;

				callback(funcArg[0]);

				called = true;
			}
			break;

		case DEBUG_TYPE_INT:
			{
				// Arg Int

				if (strIsNum(funcArg)) { // Is numeric ?

					int val = funcArg.toInt();

					PRINTFLN(F("%d) ..."), val);
					delay(500);
					timeBegin = micros();

					void (*callback)(int) = (void(*) (int))_debugFunctions[num].callback;

					callback(val);

					called = true;

				} else {

					PRINTFLN(F("%s) - invalid number in argument"), funcArg.c_str());
				}
			}
			break;
	}

	// Called ?

	if (called) {

		unsigned long elapsed = (micros() - timeBegin);

		printLibrary();
		PRINTFLN(F("End of execution. Elapsed: %lu ms (%lu us)"), (elapsed / 1000), elapsed);

	}

	// Clear buffer

	_debugString = "";

	// Do a break

#ifdef DEBUGGER_FOR_SERIALDEBUG
	String response = debugBreak(F("Press enter to continue"), DEBUG_BREAK_TIMEOUT, true);

	if (response.length() > 0) { // Process command

		processCommand(response, false, false);
	}

#else
	if (_Debug && ((_Debug->wsIsConnected()))) {

		debugBreak(F("Send any command to continue, or press the \"Silence\" button, to continue."), DEBUG_BREAK_TIMEOUT, true);

	} else {

		debugBreak(F("Press enter to continue"), DEBUG_BREAK_TIMEOUT, true);

	}
#endif

}

// Process global variables commands

static void processGlobals(String& options) {

	// Reduce information on error to support low memory boards
	// Use a variable to reduce memory

#if DEBUG_USE_FLASH_F

	__FlashStringHelper* errorSintax = F("Invalid sintax. use g ? to show help");

#else

	const char* errorSintax = "Invalid sintax. use g ? to show help";

#endif

	// Have functions added ?

	if (_debugGlobalsAdded > 0) {

		// Get fields of command options

		Fields fields(options, ' ', true);

		if (fields.size() > 4) {
			printLibrary();
			PRINTFLN(errorSintax);
			return;
		}

		String firstOption = (fields.size() >= 1)? fields.getString(1) : "";

		//D("first=%s options=%s", firstOption.c_str(), options.c_str());

		if (firstOption.length() == 0 || firstOption == "?") {

			// Just show globals and help

			showGlobals(firstOption, false);

		} else if (fields.size() >= 2) { // Process change command

			if (fields.getChar(2) == '=') {

				if (fields.size() == 2) {
					printLibrary();
					PRINTFLN(errorSintax);
					return;

				} else {

					// Change the variable

					changeGlobal (fields);

				}

			} else {

				printLibrary();
				PRINTFLN(errorSintax);
				return;

			}

		} else {

			// Search by name/number

			showGlobals(options, false);

		}

	} else {

		printLibrary();
		PRINTLN();
		PRINTFLN(F("* Global variables not added to SerialDebug"));
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
		PRINTFLN(F("* See how do it in advanced example"));
#endif
	}
}

// Show list of globals available
// Used to search and show all (return total showed)
// Or to search and show one (return number showed)

static int8_t showGlobals(String& options, boolean one, boolean debugSerialApp) {

	// Show all ?

	boolean showAll = false;
	boolean byStartName = false;
	int8_t byNumber = -1;

	// Searching ?

	if (options.length() == 0 || options == "?") {

		if (one) {
			printLibrary();
			PRINTFLN(F("Option not informed"));
			return -1;
		}

		showAll = true;

	} else {

		// Search by start name (case insensitive)

		int8_t pos = options.indexOf('*');

		//D("pos %d", pos);

		if (pos > 0) {

			if (one) {

				printLibrary();
				PRINTFLN(F("* not allowed, use name or number instead"));
				return -1;
			}

			options = options.substring(0, pos);
			options.toLowerCase(); // Case insensitive

			byStartName = true;

		} else {

			// Is integer ?

			if (strIsNum(options)) {

				uint8_t num = options.toInt();

				//D("byNumber %s = %d", options.c_str(), num);

				if (num > 0) { // Find by number, else by exact name

					byNumber = num;
				}
			}
		}

		showAll = false;
	}

	// Show global(s)

	if (!debugSerialApp) { // Not for SerialDebugApp connection
		printLibrary();
		if (showAll) {
			PRINTFLN(F("Showing all global variables (%u) and actual values:"), _debugGlobalsAdded);
		} else {
			PRINTFLN(F("Searching and showing global variables and actual values:"));
		}
	}

	int8_t showed = (!one)? 0 : -1;

	// Process

	for (uint8_t i=0; i < _debugGlobalsAdded; i++) {

		String name = "";
		boolean show = showAll;

		// Get name

		if (_debugGlobals[i].name) { // Memory

			name = _debugGlobals[i].name;

#ifdef DEBUG_USE_FLASH_F
		} else if (_debugGlobals[i].nameF) { // For Flash F

			name = String(_debugGlobals[i].nameF);
			_debugString = name; // For Flash F - save the content in variable in memory, to not extract Flash again
#endif
		}

		// Show ?

		if (!showAll) {

			if (byStartName) {
				String tolower = name;
				tolower.toLowerCase(); // Case insensitive
				show = (tolower.startsWith(options));
			} else if (byNumber > 0) {
				show = ((i + 1) == byNumber);
			} else {
				show = (name == options);
			}
		}

//		D("showGlobais options = %s name: %s showAll=%d show=%d byStartName=%d byNumber=%d",
//				options.c_str(), name.c_str(), showAll, show,
//				byStartName, byNumber);

		if (show) {

			// Show

			if (debugSerialApp) { // For DebugSerialApp connection ?

				if (showGlobal((i + 1), DEBUG_SHOW_GLOBAL_APP_CONN, true)) {

#ifndef BOARD_LOW_MEMORY // Not for low memory boards
					if (_debugGlobals[i].description) { // Memory
						PRINTFLN(F(":%s"), _debugGlobals[i].description);
#ifdef DEBUG_USE_FLASH_F
					} else if (_debugGlobals[i].descriptionF) { // For Flash F, multiples print
						PRINTF(':');
						PRINTFLN(_debugGlobals[i].descriptionF);
#endif
					} else {
						PRINTLN();
					}
#else
					PRINTLN();
#endif
				}

			} else if (showGlobal((i + 1), DEBUG_SHOW_GLOBAL, true)) {

				if (one) { // One search ?

					showed = i; // Return the index
					break;

				} else { // Description, not for one search

#ifndef BOARD_LOW_MEMORY // Not for low memory boards
					if (_debugGlobals[i].description) { // Memory
						PRINTFLN(F(" // %s"), _debugGlobals[i].description);
#ifdef DEBUG_USE_FLASH_F
					} else if (_debugGlobals[i].descriptionF) { // For Flash F, multiples print
						PRINTF(F(" // "));
						PRINTFLN(_debugGlobals[i].descriptionF);
#endif
					} else {
						PRINTLN();
					}
#else
					PRINTLN();
#endif

					showed++;

				}
			}
		}
	}

	if (showAll) {
		PRINTFLN("*");
	}

	// Help

	if (!debugSerialApp) { // Not for SerialDebugApp connection

		boolean doBreak = false;

		if (!one && showed > 0) {

			if (!_debugRepeatCommand) { // Not repeating

				printLibrary();
				PRINTLN();
				PRINTFLN(F("* To show: g [name|num]"));
				PRINTFLN(F("* To search by start of name: g name*"));
				PRINTFLN(F("* To change global variable, g [name|number] = value [y]\r\n*"));

				doBreak = true;
			}

		} else if (!one || showed == -1) {

			printLibrary();
			PRINTFLN(F("Global variable not found."));

			doBreak = true;
		}

		// Do break ?

		if (doBreak) {
			String response = debugBreak();
			if (response.length() > 0) {
				processCommand(response, false, false);
			}
		}

	}

	// Clear buffer

	if (!one) {
		_debugString = "";
	}

	// Return

	return showed;

}

// Show one global variable

static boolean showGlobal(uint8_t globalNum, debugEnumShowGlobais_t mode, boolean getLastNameF) {

	// Validate

	if (globalNum == 0 || globalNum > _debugGlobalsAdded) {

		printLibrary();
		PRINTFLN(F("Invalid index for global variable\r\n*"));
		return false;
	}

	// Global

	debugGlobal_t* global = &_debugGlobals[globalNum - 1];

	// Get name

	String name = "";

	if (global->name) { // Memory

		name = global->name;

#ifdef DEBUG_USE_FLASH_F
	} else if (global->nameF) { // For Flash F

		if (getLastNameF) {

			name = _debugString; // Yet get from flash

		} else {

			name = String(global->nameF);
			_debugString = name; // For Flash F - save the content in variable in memory, to not extract Flash again
		}
#endif
	}

	// Get value and type

	String value = "";
	String type = "";

	// Get value


	getStrValue(global->type, global->pointer, \
			((mode != DEBUG_SHOW_GLOBAL_APP_CONN)?global->showLength:0), (mode != DEBUG_SHOW_GLOBAL_APP_CONN), \
			value, type);
/*
 * TODO: see it
//	getStrValue(global->type, global->pointer, \
//			((!_debugSerialApp)?global->showLength:0), (!_debugSerialApp), \
//			value, type);
*/
	if (value.length() > 0) {

		// Show

		switch (mode) {
			case DEBUG_SHOW_GLOBAL:
				PRINTF(F("* %02u %s(%s) = %s"), globalNum, name.c_str(), type.c_str(), value.c_str());
				break;
			case DEBUG_SHOW_GLOBAL_WATCH:
				PRINTF(F("%02u: %s (%s) %s"), globalNum, name.c_str(), type.c_str(), value.c_str());
				break;
			case DEBUG_SHOW_GLOBAL_APP_CONN:
				PRINTF(F("$app:G:%u:%s:%s:%s"), globalNum, name.c_str(), type.c_str(), value.c_str());
				break;
		}

	} else {

		printLibrary();
		PRINTFLN(F("Not possible show global variable\r\n*"));
		return false;
	}
	return true;
}

// Change content of global variable

static void changeGlobal(Fields& fields) {

	// Use a variable to reduce memory

#if DEBUG_USE_FLASH_F

	__FlashStringHelper* errorNoNumeric = F("Not numeric value is informed");

#else

	const char* errorNoNumeric = "Not numeric value is informed";

#endif

	// Extract options

	String globalId = fields.getString(1);
	String value = fields.getString(3);
#ifdef DEBUGGER_FOR_SERIALDEBUG
	// Check if is to not confirm
	boolean noConfirm = (fields.size() == 4 && fields.getChar(4) == 'y');
#else
	// Check if is to not confirm (or is app (not is possible confirmation on app))
	boolean noConfirm = (_Debug && (!(_Debug->isConnected())))?true:(fields.size() == 4 && fields.getChar(4) == 'y');
#endif

	String type = "";
	String tolower = "";

	// Clean the value

	value.trim(); // TODO ver isto

	// Return the spaces

	char conv = 31;
	value.replace(conv, ' ');

	// Save value to show

	String show = value;

	if (value.length() == 0) {

		printLibrary();
		PRINTFLN(F("No value informed (right of '=' in command)"));
		return;
	}

	tolower = value;
	tolower.toLowerCase();

	// Show the global and get the index

	int8_t num = showGlobals(globalId, true);

	if (num == -1) {

		// Invalid or not found

		return;

	}

	PRINTFLN(F(" // <- This is a old value"));

//	D("changeGlobal: id->%s num=%d value = %s (%d)", globalId.c_str(), num, value.c_str(),value.length());

	// Verify data

	switch (_debugGlobals[num].type) {

		// Basic types

		case DEBUG_TYPE_BOOLEAN:
			{
				if (value == "0" ||
						tolower == "f" ||
						tolower == F("false") ||
						value == "1" ||
						tolower == "t" ||
						tolower == F("true")) {

					// Value ok

					type = F("boolean");
				} else {
					printLibrary();
					PRINTFLN(F("no boolean in value (0|1|false|true|f|t")) ;
					return;
				}
			}
			break;
		case DEBUG_TYPE_CHAR:

			removeQuotation(value, true);

			if (value.length() != 1) {
				printLibrary();
				PRINTFLN(F("Note: string too large, truncated to size of 1"));
				value = value.substring(0,1);
			}
			type = F("char");
			break;
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
		case DEBUG_TYPE_BYTE:
			type = F("byte");
			break;
#endif
		case DEBUG_TYPE_INT:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("int");
			break;
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
		case DEBUG_TYPE_U_INT:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("unsigned int");
			break;
		case DEBUG_TYPE_LONG:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("long");
			break;
#endif
		case DEBUG_TYPE_U_LONG:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("unsigned long");
			break;
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
		case DEBUG_TYPE_FLOAT:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("float");
			break;
		case DEBUG_TYPE_DOUBLE:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("double");
			break;

		// Integer C size _t

		case DEBUG_TYPE_INT8_T:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("int8_t");
			break;
		case DEBUG_TYPE_INT16_T:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("int16_t");
			break;
		case DEBUG_TYPE_INT32_T:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("int32_t");
			break;
//#ifdef ESP32
//			case DEBUG_TYPE_INT64_T:
//					value = String(*(int64_t*)_debugGlobals[i].pointer);
//					type = "int64_t";
//					break;
//#endif
		// Unsigned integer C size _t

		case DEBUG_TYPE_UINT8_T:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("uint8_t");
			break;
		case DEBUG_TYPE_UINT16_T:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("uint16_t");
			break;
		case DEBUG_TYPE_UINT32_T:
			if (!strIsNum(value)) {
				printLibrary();
				PRINTFLN(errorNoNumeric);
				return;
			}
			type = F("uint32_t");
			break;
//#ifdef ESP32
//			case DEBUG_TYPE_UINT64_T:
//					value = String(*(uint64_t*)_debugGlobals[i].pointer);
//					type = "uint64_t";
//					break;
//#endif
		// Strings

		case DEBUG_TYPE_CHAR_ARRAY:
			{

//				size_t size = sizeof((char*)_debugGlobals[num].pointer);
//				if (value.length() >= size) {
//					PRINTF(F("* SerialDebug: Note: string too large, truncated to size of array (%u)"), size);
//					value = value.substring(0,size-1);
//				}
//				type = "char array";
				printLibrary();
				PRINTFLN(F("Not allowed change char arrays (due memory issues)"));
				return;
			}
			break;
#endif // Not low memory board

		case DEBUG_TYPE_STRING:

			removeQuotation(value, false);

			type = F("String");
			break;

	}

	// Show again with new value to confirm

	if (_debugGlobals[num].name) { // RAM Memory
		PRINTF(F("* %02u %s(%s) = %s"), (num + 1), _debugGlobals[num].name, type.c_str(), show.c_str());
#ifdef DEBUG_USE_FLASH_F
	} else if (_debugGlobals[num].nameF) { // Flash memory
		PRINTF(F("* %02u %s(%s) = %s"), (num + 1), _debugString.c_str(), type.c_str(), show.c_str());
#endif
	}
	PRINTFLN(F(" // <- This is a new value"));

	// Show a confirm message and wait from response (if not "y" passed to last field)

	String response;

	if (noConfirm) {
		response = "y";
	} else {

		response = debugBreak(F("*\r\n* Confirm do change value? (y-yes/n-no)"), DEBUG_BREAK_TIMEOUT, false);
	}

	if (response == "y" || response == "yes") {

		// Do change

		switch (_debugGlobals[num].type) {

				// Basic types

				case DEBUG_TYPE_BOOLEAN:
					{
						boolean change = (value == "1" || value[0] == 't');
						*(boolean*)_debugGlobals[num].pointer = change;
						value = (change) ? F("true") : F("false");
					}
					break;
				case DEBUG_TYPE_CHAR:
					{
						char change = value[0];
						*(char*)_debugGlobals[num].pointer = change;
					}
					break;
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
				case DEBUG_TYPE_BYTE:
					{
						char change = value[0];
						*(byte*)_debugGlobals[num].pointer = change;
					}
					break;
#endif
				case DEBUG_TYPE_INT:
					{
						int change = value.toInt();
						*(int*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
				case DEBUG_TYPE_U_INT:
					{
						unsigned int change = value.toInt();
						*(unsigned int*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
				case DEBUG_TYPE_LONG:
					{
						long change = value.toInt();
						*(long*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
#endif
				case DEBUG_TYPE_U_LONG:
					{
						unsigned long change = value.toInt();
						*(unsigned long*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
#ifndef BOARD_LOW_MEMORY // Not for low memory boards
				case DEBUG_TYPE_FLOAT:
					{
						float change = value.toFloat();
						*(float*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
				case DEBUG_TYPE_DOUBLE: // TODO no have toDouble in some archs - see
					{
						double change = value.toFloat();
						*(double*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;

				// Integer C size _t

				case DEBUG_TYPE_INT8_T:
					{
						int8_t change = value.toInt();
						*(int8_t*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
				case DEBUG_TYPE_INT16_T:
					{
						int16_t change = value.toInt();
						*(int16_t*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
				case DEBUG_TYPE_INT32_T:
					{
						int32_t change = value.toInt();
						*(int32_t*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
		//#ifdef ESP32
		//			case DEBUG_TYPE_INT64_T:
		//					value = String(*(int64_t*)_debugGlobals[i].pointer);
		//					type = "int64_t";
		//					break;
		//#endif

				// Unsigned integer C size _t

				case DEBUG_TYPE_UINT8_T:
					{
						uint8_t change = value.toInt();
						*(uint8_t*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
				case DEBUG_TYPE_UINT16_T:
					{
						uint16_t change = value.toInt();
						*(uint16_t*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
				case DEBUG_TYPE_UINT32_T:
					{
						uint32_t change = value.toInt();
						*(uint32_t*)_debugGlobals[num].pointer = change;
						value = String(change);
					}
					break;
		//#ifdef ESP32
		//			case DEBUG_TYPE_UINT64_T:
		//					value = String(*(uint64_t*)_debugGlobals[i].pointer);
		//					type = "uint64_t";
		//					break;
		//#endif
				// Strings

				case DEBUG_TYPE_CHAR_ARRAY:
					{
						strcpy((char*)(_debugGlobals[num].pointer), (char*)value.c_str());
					}
					break;
#endif // Not low memory board

				case DEBUG_TYPE_STRING:
					{
						*(String*)_debugGlobals[num].pointer = value;
					}
					break;
			}

			// Show again with new value

			if (_debugGlobals[num].name) { // RAM Memory
				PRINTF(F("* %02u %s(%s) = %s"), (num + 1), _debugGlobals[num].name, type.c_str(), show.c_str());
#ifdef DEBUG_USE_FLASH_F
			} else if (_debugGlobals[num].nameF) { // Flash memory
				PRINTF(F("* %02u %s(%s) = %s"), (num + 1), _debugString.c_str(), type.c_str(), show.c_str());
#endif
			}
			PRINTFLN(F(" // <- This has changed w/ success"));


	} else {

		printLibrary();
		PRINTFLN(F("Variable global not changed"));

	}

	// Clear buffer

	_debugString = "";

	// Do a break, if not confirm

	if (!noConfirm) {

		String response = debugBreak(F("Press enter to continue"), DEBUG_BREAK_TIMEOUT, true);

		if (response.length() > 0) { // Process command

			processCommand(response, false, false);
		}
	}
}

#endif // DEBUG_DISABLE_DEBUGGER

// Remove quotation marks from string

static void removeQuotation(String& string, boolean single) {

	if (single) {

		if (string.length() == 3) {
			string = string.charAt(1);
		}

	} else {

		if (string == "\"\"") {
			string = "";
		} else if (string.length() >= 3) {
			if (string.charAt(0) == '"') { // Retira as aspas
				string = string.substring(1, string.length() - 1);
			}
			if (string.charAt(string.length() - 1) == '"') { // Retira as aspas
				string = string.substring(0, string.length() - 1);
			}
		}
	}
}

#endif // DEBUG_DISABLE_DEBUGGER

// Free memory

#if defined ARDUINO_ARCH_AVR || defined __arm__

	// Based in https://forum.pjrc.com/threads/23256-Get-Free-Memory-for-Teensy-3-0

#ifdef __arm__

    // should use uinstd.h to define sbrk but Due causes a conflict
    extern "C" char* sbrk(int incr);
#else

    extern char *__brkval;
    extern char __bss_end;

#endif

#endif

int freeMemory() {

#if defined ESP8266 || defined ESP32

	 return ESP.getFreeHeap();

#elif defined ARDUINO_ARCH_AVR || defined __arm__

	// function from the sdFat library (SdFatUtil.cpp)
	// licensed under GPL v3
	// Full credit goes to William Greiman.
    char top;
    #ifdef __arm__
        return &top - reinterpret_cast<char*>(sbrk(0));
    #else
        return __brkval ? &top - __brkval : &top - &__bss_end;
    #endif

#else // Not known

	 return -1;

#endif

}

#endif // DEBUG_DISABLE_DEBUGGER

#endif // DEBUG_DISABLED

/////// End


