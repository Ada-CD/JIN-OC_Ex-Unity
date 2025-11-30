using System;
using System.IO;
using System.IO.Ports;

using UnityEngine;

/**
 * Wrapper class around SerialPort to make basic checks
 * and handling of disconnection much easier.
 * As SerialPort is not polymorphic, re-implement all functions we might use.
 *
 * Catch IO exceptions and disconnect the serial port in order to recover.
 * Otherwise, the base class does not update the state of the device on errors
 * and will error infinitely, assuming the device is still properly connected.
 *
 * Try not to catch all exceptions : we still want usage errors to bubble up
 * to the students.
 */
public class SafeSerial : SerialPort
{
	public SafeSerial(string portName, int baudRate) : base(portName, baudRate) {}

	private const string ReadError = "Error reading from serial : {0}";
	private const string WriteError = "Error writing to serial : {0}";
	private const string NotOpen = "Serial port is not open !";

	/**
	 * If we caught an IO error, we probably have a connection issue with
	 * the serial device.
	 * Log the error to Unity and disconnect the serial port.
	 * This way, the game does not hog the port name, and we can
	 * try to re-connect which might even be on the same port.
	 */
	private void IoErrorAbort(string message, Exception e)
	{
		Debug.LogError($"{string.Format(message, e.Message)}\nClosing connection.");
		Close();
	}

	/**
	 * Catch IO errors on Open so we can easily check and try again later.
	 */
	public new bool Open()
	{
		try {
			base.Open();
			return true;
		} catch (Exception e) when (e is IOException) {
			Debug.LogWarning($"Could not open serial {PortName} : {e.Message}");
			return false;
		}
	}

	/**
	 * BytesToRead tries to access the underlying device,
	 * so it might error out if disconnected.
	 */
	public new int BytesToRead
	{
		get
		{
			try {
				return base.BytesToRead;
			} catch (Exception e) when (e is IOException) {
				IoErrorAbort(ReadError, e);
				return 0;
			}
		}
	}

#region Writes

	public new void Write(byte[] buffer, int offset, int count)
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return;
		}
		
		try {
			base.Write(buffer, offset, count);
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(WriteError, e);
		}
	}

	public new void Write(char[] buffer, int offset, int count)
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return;
		}
		
		try {
			base.Write(buffer, offset, count);
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(WriteError, e);
		}
	}

	public new void Write(string text)
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return;
		}

		try {
			base.Write(text);
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(WriteError, e);
		}
	}

	public new void WriteLine(string text) => Write(text + NewLine);

#endregion

#region Reads

	/**
	 * Don't return -1 when aborting buffer reads, as it should be a count.
	 * Instead, callers should make sure to check for the connection being open
	 * an abort accordingly : this at least makes it safe.
	 */
	public new int Read(byte[] buffer, int offset, int count)
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return 0;
		}

		try {
			return base.Read(buffer, offset, count);
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(ReadError, e);
			return 0;
		}
	}

	public new int Read(char[] buffer, int offset, int count)
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return 0;
		}

		try {
			return base.Read(buffer, offset, count);
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(ReadError, e);
			return 0;
		}
	}

	/**
	 * Return an empty string on error, so message processing can check for
	 * a length of zero.
	 */
	public new string ReadTo(string value)
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return "";
		}

		try {
			return base.ReadTo(value);
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(ReadError, e);
			return "";
		}
	}

	/**
	 * This is the default Mono implementation, but override it anyway
	 * in case other implementations differ.
	 */
	public new string ReadLine() => ReadTo(NewLine);
	
	public new string ReadExisting()
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return "";
		}

		try {
			return base.ReadExisting();
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(ReadError, e);
			return "";
		}
	}

	/**
	 * -1 Is used internally for invalid/no data so re-use it here.
	 */
	public new int ReadChar()
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return -1;
		}

		try {
			return base.ReadChar();
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(ReadError, e);
			return -1;
		}
	}

	public new int ReadByte()
	{
		if (!IsOpen) {
			Debug.LogWarning(NotOpen);
			return -1;
		}

		try {
			return base.ReadByte();
		} catch (Exception e) when (e is InvalidOperationException or IOException) {
			IoErrorAbort(ReadError, e);
			return -1;
		}
	}

#endregion
}
