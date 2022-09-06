# PsiControl_GUIAdjustableVoltageSource
GUI to control the Arduino designed to control the voltage on the TS_AIO-5 card. GUI is written as a WPF application and uses serial communication to communicate with the Arduino.
The main functionallities are the connection setup, measuring current and voltage, connecting channels to ground/the bus and putting a specified voltage on the bus.

The source code for the Arduino itself can be foudn in the repository https://github.com/WoutDeleu/PsiControl_AdjustablePowersupply. It is based on the serial communication between the Arduino and the PC. The Arduino uses to cmdMessenger library to simplify the serial communication.

To run and execute the program, connect the correct pins from the arduino to the correct pins on the AIO-board. Load the correct code on the arduino. After completing the upload, the GUI can be started and used to control the Arduino, using Serial Communication.
