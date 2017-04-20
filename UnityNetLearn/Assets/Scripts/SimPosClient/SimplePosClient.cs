using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

public class SimplePosClient
{
	public delegate void SendMsg ();

	private Socket m_socket;

	private SendMsg m_fSendMsg;

	public SimplePosClient()
	{
		
	}

	public void SPClientInit(SendMsg fMsg)
	{
		m_fSendMsg = fMsg;
	}

	public void SPClientStart(string ip = "127.0.0.1" , int port = 1234)
	{
		
	}
		
	public void SPClientUnit()
	{
		
	}

	public void Send(string Content)
	{
	}

	private void ReceiveMsgCB(IAsyncResult ret)
	{
		
	}
}
