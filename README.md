### Voltage Control GUI for TS_AIO-5 Card 🌟

Welcome to the Voltage Control GUI project developed for the Picanol Group! This project features a graphical user interface (GUI) designed to control an Arduino, which, in turn, manages the voltage on the TS_AIO-5 card. The GUI is created using the Windows Presentation Foundation (WPF) and utilizes serial communication to connect with the Arduino.

#### Project Overview 🚀

The primary functionalities of the Voltage Control GUI are as follows:
- **Connection Setup**: Establish a connection between the GUI and the Arduino.
- **Current and Voltage Measurement**: Measure current and voltage on the TS_AIO-5 card.
- **Channel Control**: Connect channels to ground or the bus as needed.
- **Voltage Control**: Set a specified voltage on the bus.

#### Arduino Source Code 📦

The source code for the Arduino, responsible for handling voltage control and serial communication, can be found in the following repository: [Arduino Source Code](https://github.com/WoutDeleu/PsiControl_AdjustablePowersupply). The Arduino relies on the cmdMessenger library to streamline the serial communication process.

#### Getting Started 📋

To run and execute the program successfully:
1. Connect the appropriate pins on the Arduino to the corresponding pins on the AIO-board.
2. Load the correct Arduino code onto the Arduino board.
3. After the code is uploaded, launch the GUI, and it will be ready for use in controlling the Arduino through serial communication.

#### Support and Contribution 🤝

We welcome contributions and feedback from the community. If you have ideas to enhance this project or encounter any issues, please open an issue or submit a pull request. We appreciate your support in making this project even more valuable!

#### Explore and Enjoy! 🎉

We hope this Voltage Control GUI makes it easier to manage and control the voltage on the TS_AIO-5 card effectively. Enjoy exploring and using this tool for your requirements!
