using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using UnityEngine;

public class SimplePosClient
{
	public delegate void SendMsg (string content);

	private const int BYTE_NUM = 1024;

	private Socket m_socket;

	private SendMsg m_fSendMsg;

	private byte[] m_bytes;

	public SimplePosClient()
	{
		
	}

	public void SPClientInit(SendMsg fMsg)
	{
		m_fSendMsg = fMsg;

		m_socket = new Socket(AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);

		m_bytes = new byte[BYTE_NUM];

	}

	public void SPClientStart(string ip = "127.0.0.1" , int port = 1234)
	{
		IPAddress ipadd = IPAddress.Parse (ip);
		IPEndPoint ipend = new IPEndPoint (ipadd , port);

		m_socket.Connect (ipend);

		this.BeginReceiveMsg ();
	}
		
	public void SPClientUnit()
	{
		m_bytes = null;
		m_fSendMsg = null;
		m_socket = null;
	}

	public void Send(string content)
	{
		Debug.Log (content);
		byte[] sby = System.Text.Encoding.Default.GetBytes (content);
		m_socket.Send (sby);
	}

	private void BeginReceiveMsg()
	{
		int offset = 0;
		m_socket.BeginReceive (m_bytes , offset , BYTE_NUM ,  SocketFlags.None , ReceiveMsgCB , null);
	}

	private void ReceiveMsgCB(IAsyncResult ret)
	{
		int	count = m_socket.EndReceive (ret);

		string content = System.Text.Encoding.UTF8.GetString (m_bytes, 0, count);

		m_fSendMsg (content);

		this.BeginReceiveMsg ();
	}
}
