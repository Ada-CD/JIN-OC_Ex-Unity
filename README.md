# Arduino-Unity communication example

This is a simple Unity project that communicates to an Arduino card, controlling the state of a river blocking the way and turing on an LED when the player is inside a specific area.  
It uses the `System.IO.Ports` serial objects provided by the .NET framework. Check that the project is using .NET 4 in the player settings in Unity !

The Arduino script is contained in the [Arduino_Controller](Arduino_Controller) folder. It cannot be moved out, otherwise the Arduino IDE will complain and force you to create the folder.


This was tested using the **2019 LTS** version of Unity (2019.4.16f1 from my Unity Hub).
