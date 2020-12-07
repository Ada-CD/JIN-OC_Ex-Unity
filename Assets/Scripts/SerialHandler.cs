using System.IO.Ports;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
    
    private SerialPort _serial;

    // Common default serial device on a Linux machine
    [SerializeField] private string serialPort = "/dev/ttyACM0";
    [SerializeField] private int baudrate = 115200;
    
    [SerializeField] private Component river;
    [SerializeField] private Rigidbody2D _riverRigidbody2D;
    [SerializeField] private SpriteRenderer _riverSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        _serial = new SerialPort(serialPort,baudrate);
        // Once configured, the serial communication must be opened just like a file : the OS handles the communication.
        _serial.Open();
        
        _riverRigidbody2D = river.GetComponentInParent<Rigidbody2D>();
        _riverSprite = river.GetComponentInParent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Prevent blocking if no message is available as we are not doing anything else
        // Alternative solutions : set a timeout, read messages in another thread, coroutines, futures...
        if (_serial.BytesToRead <= 0) return;
        
        var message = _serial.ReadLine();
        // Arduino sends "\r\n" with println, '\n' is removed by ReadLine()
        message = message.Trim('\r');
        
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
        _serial.WriteLine(newState ? "LED ON" : "LED OFF");
    }
    
    private void OnDestroy()
    {
        _serial.Close();
    }
}
