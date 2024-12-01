using System;
using System.IO.Ports;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
    
    private SerialPort _serial;

    // Common default serial device on a Windows machine
    [SerializeField] private string serialPort = "COM1";
    [SerializeField] private int baudrate = 115200;
    
    [SerializeField] private Component river;
    private Rigidbody2D _riverRigidbody2D;
    private SpriteRenderer _riverSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        _riverRigidbody2D = river.GetComponentInParent<Rigidbody2D>();
        _riverSprite = river.GetComponentInParent<SpriteRenderer>();
        
        _serial = new SerialPort(serialPort,baudrate);
        // Guarantee that the newline is common across environments.
        _serial.NewLine = "\n";
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();
    }

    // Update is called once per frame
    void Update()
    {
        // Return early if not open, prevent spamming errors for no reason.
        if (!_serial.IsOpen) return;
        // Prevent blocking if no message is available as we are not doing anything else
        // Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
        if (_serial.BytesToRead <= 0) return;
        
        // Trim leading and trailing whitespaces, makes it easier to handle different line endings.
        // Arduino uses \r\n by default with `.println()`.
        var message = _serial.ReadLine().Trim();

        switch (message)
        {
            case "dry":
                _riverRigidbody2D.simulated = false;
                _riverSprite.color = new Color32(146,108,77,255);
                break;
            case "wet":
                _riverRigidbody2D.simulated = true;
                _riverSprite.color = new Color32(16,107,255,255);
                break;
        }
    }

    public void SetLed(bool newState)
    {
        if (!_serial.IsOpen) return;
        _serial.WriteLine(newState ? "LED ON" : "LED OFF");
    }
    
    private void OnDestroy()
    {
        if (!_serial.IsOpen) return;
        _serial.Close();
    }
}
