using System;
// Will be used when adding TryParse with invariant culture
using System.Globalization;
using System.IO.Ports;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
	private SerialPort _serial;

	// Common default serial device on a Windows machine
	[SerializeField] private string serialPort;
	[SerializeField] [Min(0)] private int baudrate;
	private const float OpenAttemptDelay = 5;
	private float _lastOpenAttemptTime = -OpenAttemptDelay;

	[SerializeField] private Component river;
	private Rigidbody2D _riverRigidbody2D;
	private SpriteRenderer _riverSprite;

	[SerializeField] private LightController lightController;

	// Start is called before the first frame update
	void Start()
	{
		_riverRigidbody2D = river.GetComponentInParent<Rigidbody2D>();
		_riverSprite = river.GetComponentInParent<SpriteRenderer>();

		_serial = new SerialPort(serialPort, baudrate);
		// Guarantee that the newline is common across environments.
		_serial.NewLine = "\n";
		// Once configured, the serial communication must be opened.
		// Just like a file or a socket : the OS handles the hard work.
		OpenSerial();
	}

	// Try to open the configured serial port, rate limited by OpenAttemptDelay
	// Return if we successfully opened.
	private bool OpenSerial()
	{
		// Don't try to re-connect too often
		if (Time.time < _lastOpenAttemptTime + OpenAttemptDelay) return false;
		_lastOpenAttemptTime = Time.time;
		
		try {
			// Allow updating the port name in the inspector while running the game.
			_serial.PortName =  serialPort;
			_serial.Open();
		} catch (Exception e) {
			Debug.LogWarning($"Could not open serial {serialPort} : {e.Message}");
			return false;
		}

		Debug.Log($"Serial opened on {serialPort}");
		return true;
	}

	private void CloseSerial()
	{
		_serial.Close();
	}

	// Update is called once per frame
	void Update()
	{
		// If the port is not open and could not be opened, we have nothing to do : return early.
		if (!_serial.IsOpen && !OpenSerial()) return;
		
		string message;
		// We might not know that the serial port has been disconnected :
		// catch any errors and properly close it so we can recover.
		try {
			// Prevent blocking if no message is available as we are not doing anything else
			// Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
			if (_serial.BytesToRead <= 0) return;
			
			message = _serial.ReadLine();
		} catch (Exception e) {
			Debug.LogError($"Error reading serial : {e.Message}\nResetting connection.");
			CloseSerial();
			return;
		}
		
		// Trim leading and trailing whitespaces makes it easier to handle different line endings.
		// Arduino uses \r\n by default with `.println()`.
		message = message.Trim();
		// Split the message on spaces, so we can read the header and possible arguments.
		var messageParts = message.Split(' ');
		var header = messageParts[0];
		var arguments = messageParts[1..];

		switch (header) {
		case "river":
			if (arguments.Length != 1) {
				Debug.LogError($"Expected 1 argument for \"river\", got {arguments.Length}");
				return;
			}

			// Note that this is a switch on the *characters* 0 and 1, not the numbers !
			switch (arguments[0]) {
			case "0":
				_riverRigidbody2D.simulated = false;
				_riverSprite.color = RiverColors.Dry;
				break;
			case "1":
				_riverRigidbody2D.simulated = true;
				_riverSprite.color = RiverColors.Wet;
				break;
			default:
				Debug.LogError($"Unknown argument for \"river\" : {arguments[0]}");
				return;
			}

			break;
		/*
		 * FIXME: Partie 1 Question 2.c - ajouter un `case` pour le nouvel en-tête
		 * correspondant au message pour changer la vitesse des compte à rebours.
		 */
		default:
			Debug.Log($"Unknown message: {message}");
			break;
		}
	}

	public void SetLed(bool newState)
	{
		if (!_serial.IsOpen) return;
		try {
			_serial.WriteLine(newState ? "LED ON" : "LED OFF");
		} catch (Exception e) {
			Debug.LogError($"Error writing to serial : {e.Message}\nResetting connection.");
			CloseSerial();
		}
	}

	private void OnDestroy()
	{
		if (!_serial.IsOpen) return;
		_serial.Close();
	}
}
