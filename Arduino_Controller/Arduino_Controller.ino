// The flag signals to the rest of the program an interrupt occured
static bool button_flag = false;
// Remember the state the river in the Unity program is in
static bool river_state = true;

// Interrupt handler, sets the flag for later processing
void buttonPress() {
  button_flag = true;
}

void setup() {
  int buttonPin = 2;
  
  pinMode(LED_BUILTIN, OUTPUT);
  // Internal pullup, no external resistor necessary
  pinMode(buttonPin,INPUT_PULLUP);
  // 115200 is a common baudrate : fast without being overwhelming
  Serial.begin(115200);

  // As the button is in pullup, detect a connection to ground
  attachInterrupt(digitalPinToInterrupt(buttonPin),buttonPress,FALLING);

  // Wait for a serial connection
  while (!Serial.availableForWrite());
  // Make sure the buffer is clean !
  Serial.flush();
  // In case the Unity project isn't synced with the boolean.
  Serial.println("river 1");
}

// Processes button input
void loop() {
  // Slows reaction down a bit but prevents _most_ button press misdetections.
  delay(200);
  
  if (button_flag) {
    // Use print to add another part before the new line ; note the space !
    Serial.print("river ");
    int unity_river_arg;
    if (river_state) {
      unity_river_arg = 0;
    } else {
      unity_river_arg = 1;
    }
    // Complete the message with the value and a new line.
    Serial.println(unity_river_arg);

    river_state = !river_state;
    // Clear the interrupt flag so we don't re-do the change.
    button_flag = false;
  }
}

// Handles incoming messages
// Called by Arduino if any serial data has been received
void serialEvent()
{
  String message = Serial.readStringUntil('\n');
  if (message == "LED ON") {
    digitalWrite(13,HIGH);
  } else if (message == "LED OFF") {
    digitalWrite(13,LOW);
  }
}
