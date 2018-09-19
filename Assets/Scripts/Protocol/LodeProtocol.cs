using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;

public class Protocol
{
	public string command;
	public int parameter;

	public Protocol(string c, int p)
	{
		command = c;
		parameter = p;
	}
}

public class LodeProtocol : MonoBehaviour
{
	// Constants
	public const string GET_POWER = "PM"; //read resistance
	public const string GET_SPEED_INT = "RM";
	public const string GET_SPEED_FLOAT = "RN";
	public const string GET_HEART_RATE = "HR";
	public const string GET_STATUS = "RS";
	public const string GET_VERSION = "VR";
	public const string GET_SERIAL_NUMBER = "SN";
	public const string SET_POWER = "SP"; //set resistance
	public const string SET_TORQUE = "ST";

	// Settings
	//public string PortName = string.Empty;
	public int PortBaudRate = 9600;
	public Parity PortParity = Parity.None;
	public int PortDataBits = 8;
	public StopBits PortStopBits = StopBits.One;
	public Handshake PortHandshake = Handshake.None;
	public int PortRequestTimeout = 100;
	
	// Protocol
	private List<Protocol> runningProtocols = new List<Protocol>();
	private int protocolRunIdx = 0;

	public delegate void LodeResponse(Protocol command, string data);
	public event LodeResponse ResponseReceived;

	// Variables
	private SerialPort serialPort = null;
	private int machineID = -1;

	public void Connect()
	{
		if (serialPort != null && serialPort.IsOpen) return;

		// Ping available
		string[] ports = SerialPort.GetPortNames();
		for (int p = 0; p < ports.Length; p++)
		{
			PingPort(ports[p]);
		}

		Invoke("Connect", 5f);
	}

	private void PingPort(string portname)
	{
		// Try to create the connection
		SerialPort serial;
		try
		{
			serial = new SerialPort(@"\\.\" + portname, PortBaudRate, PortParity, PortDataBits, PortStopBits);
		}
		catch (System.IO.IOException ex)
		{
			Debug.LogWarning("(Port: " + portname + ") " + ex);
			return;
		}

		// Set up the settings
		serial.Handshake = PortHandshake;
		serial.ReadTimeout = PortRequestTimeout;
		serial.WriteTimeout = PortRequestTimeout;

		serial.RtsEnable = true;
		serial.DtrEnable = true;

		// Try to open the port
		try
		{
			serial.Open();
		}
		catch (System.Exception ex)
		{
			Debug.LogWarning("(Port: " + portname + ") " + ex);
			return;
		}

		StartCoroutine(CheckForMachine(serial, portname));
	}

	private IEnumerator CheckForMachine(SerialPort serial, string portname)
	{
		if (serial == null || !serial.IsOpen || machineID >= 0) yield break;
		
		serial.Write("0,SN\r");

		yield return new WaitForSeconds(PortRequestTimeout / 1000f);

		int device;
		string sn = GetDataFromPort(serial, out device);

		if (device >= 0)
		{
			Debug.Log("Machine detected and ready. (Port: " + portname + ", ID: " + device + ", Serial: " + sn + ")");
			machineID = device;
			serialPort = serial;

			CancelInvoke("Connect");
			StopAllCoroutines();
			StartCoroutine("CheckForData");
			yield break;
		}
	}

	public void Disconnect()
	{
		StopAllCoroutines();
		machineID = -1;

		if (serialPort != null && serialPort.IsOpen)
		{
			serialPort.Close();
		}

		Debug.Log("Machine disconnected. Searching for machine...");

		Connect();
	}

	// Send data
	public Protocol AddCall(string command, int parameter = -1, bool priorty = false)
	{
		Protocol prot = new Protocol(command, parameter);
		runningProtocols.Add(prot);
		
		if (priorty)
		{
			protocolRunIdx = runningProtocols.Count - 1;
		}
		return prot;
	}

	// Kill data flow
	public void RemoveCall(Protocol prot)
	{
		runningProtocols.Remove(prot);
	}

	// Kill data flow with string command find
	public void RemoveCall(string command)
	{
		for (int i = 0; i < runningProtocols.Count; i++)
		{
			if (runningProtocols[i].command != command) continue;

			runningProtocols.Remove(runningProtocols[i]);
			i--;
		}
	}

	// Checking
	private IEnumerator CheckForData()
	{
		while(true)
		{
			if (runningProtocols.Count == 0)
			{
				yield return new WaitForSeconds(0.5f);
				continue;
			}

			// Prepare request
			if (protocolRunIdx >= runningProtocols.Count || protocolRunIdx < 0)
			{
				protocolRunIdx = 0;
			}

			if (serialPort == null || !serialPort.IsOpen) break;

			Protocol prot = runningProtocols[protocolRunIdx];
			string request = (machineID + "," + prot.command);

			if (prot.parameter != -1)
			{
				request += prot.parameter.ToString();
			}
			request += "\r";

			// Write request
			try
			{
				serialPort.Write(request);
			}
			catch (System.Exception)
			{
				Disconnect();
				yield break;
			}


			yield return new WaitForSeconds(PortRequestTimeout / 1000f);

			// Receive message
			int device;
			string result = GetDataFromPort(serialPort, out device);

			if (!string.IsNullOrEmpty(result) && device == machineID)
			{
				ResponseReceived(prot, result);
			}

			protocolRunIdx++;
		}
	}
	
	// Receive data
	private string GetDataFromPort(SerialPort serial, out int deviceID)
	{
		List<char> message = new List<char>();
		deviceID = -1;

		try
		{
			int charbyte;
			do
			{
				charbyte = serial.ReadByte();
				message.Add((char) charbyte);
			}
			while (charbyte != '\r');
		}
		catch (System.TimeoutException)
		{
			//Debug.LogWarning("No data available:\r\n" + ex);
			serial.DiscardInBuffer();
			return string.Empty;
		}
		catch (System.IO.IOException)
		{
			Disconnect();
			return string.Empty;
		}

		serial.DiscardInBuffer();

		string[] final = new string(message.ToArray()).Split(new char[] { ',' }, 2);

		deviceID = int.Parse(final[0]);
		return final[1];
	}
}
