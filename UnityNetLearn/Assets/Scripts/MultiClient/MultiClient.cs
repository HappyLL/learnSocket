using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class MultiClient{

	public delegate void PushMsg(string content);

	private Socket m_socket;
	private byte[] m_byte;

	private PushMsg m_msgFunc;

	public MultiClient()
	{
		
	}

	public void InitClient(PushMsg msgFunc)
	{
		m_socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		m_byte = new byte[1024];

		m_msgFunc = msgFunc;
	}

	public void StartClient(string ip = "127.0.0.1" , int port = 1234)
	{
		IPAddress ipAddress = IPAddress.Parse(ip);
		IPEndPoint ipEd = new IPEndPoint (ipAddress , port);
		m_socket.Connect (ipEd);

		BeginReceiveMsg ();
	}

	private void BeginReceiveMsg()
	{
		m_socket.BeginReceive (m_byte , 0 , 1024, SocketFlags.None , RecevieMsgFromSvr , this);
	}

	private void RecevieMsgFromSvr(System.IAsyncResult state)
	{
		int count = m_socket.EndReceive (state);
		if (count <= 0)
			return;
		string str = System.Text.Encoding.UTF8.GetString (m_byte , 0 , count);

		if(str.Length > 0 && m_msgFunc != null)
			m_msgFunc (str);

		BeginReceiveMsg ();
	}

	public void SendMsg(string content)
	{
		if (content.Length == 0) 
		{
			return;
		}
		byte[] sBy = System.Text.Encoding.UTF8.GetBytes (content);

		m_socket.Send (sBy);
	}

	public void UnInitClient()
	{
		m_socket.Close ();
		m_socket = null;

		m_byte = null;
	}

}
