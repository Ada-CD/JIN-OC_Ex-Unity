using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPHandler : MonoBehaviour
{
    private TcpListener _tcpListener;
    
    private Thread _listenerThread;

    private TcpClient _Esp;

    [SerializeField] public int ListenPort = 55555;

    // Start is called before the first frame update
    void Start()
    {
        // Start the TCP server in the background
        _listenerThread = new Thread(new ThreadStart(TcpListen));
        _listenerThread.IsBackground = true;
        _listenerThread.Start();
    }

    private void OnDestroy() {
        _Esp.Close();
        _tcpListener.Stop();
        _listenerThread.Abort();
    }

    void Update() {
        if (Input.GetKeyDown("space"))
        {
            SendTcpMessage("Pressed down space !\n");
        }
    }

    // Background thread handling communications.
    private void TcpListen() {
        try {
            // Create listener on port ListenPort. 			
            _tcpListener = new TcpListener(IPAddress.Parse("0.0.0.0"), ListenPort);
            _tcpListener.Start();
            Debug.Log("Server is listening");
        }
        catch (SocketException exception) {
            Debug.LogError("Error while opening server " + exception);
            return;
        }

        var bytes = new byte[1024];
        while (true) {
            // Should allow reconnects.
            using (_Esp = _tcpListener.AcceptTcpClient()) {
                // Get a stream for reading
                try
                {
                    using NetworkStream stream = _Esp.GetStream();
                    int length;
                    // Read incoming stream into byte array.
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {
                        var incomingData = new byte[length];
                        Array.Copy(bytes, 0, incomingData, 0, length);
                        // Convert byte array to string message.
                        var clientMessage = Encoding.ASCII.GetString(incomingData);
                        Debug.Log("Received : \n" + clientMessage);
                    }
                }
                catch (SocketException exception)
                {
                    Debug.LogError("Exception during communication :\n" + exception);
                }
            }
        }
    }

    // Send a message to the connected client.
    private void SendTcpMessage(String message) {
        if (_Esp == null) {
            return;
        }

        try {           
            // Get a stream for writing.             
            NetworkStream stream = _Esp.GetStream();          
            if (stream.CanWrite) {
                // Convert message to a byte array.
                byte[] messageToTransmit = Encoding.ASCII.GetBytes(message);
                // Write byte array to socketConnection stream.
                stream.Write(messageToTransmit, 0, messageToTransmit.Length);               
                Debug.Log($"Sent '{message}' to TCP client.");
            }
        }
        catch (SocketException exception) {
            Debug.Log("Error during writing : " + exception);
        }
    }
}
