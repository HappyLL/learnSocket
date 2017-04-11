using System;
using System.Net;
using System.Net.Sockets;

namespace SimpleNet
{
	//面向一个客户端连接(处理客户端的数据的收发)
	public class Connector
	{

		public delegate void SendAllMsg(Connector obj , string msg);


		//持有客户端socket的引用
		private Socket m_socket;
		//发送和接收客户端的字节
		private byte[] m_bytes;

		//判断当前Conn有没有再用
		private bool m_isUsed;

		private SendAllMsg m_sendAllMsg;


		public Connector (SendAllMsg allMsgCallBack)
		{
			m_socket = null;
			m_bytes = new byte[1024];	
			m_isUsed = false;

			m_sendAllMsg = allMsgCallBack;
		}

		public void StartReceiveMessage()
		{
			if (m_socket == null) 
			{
				Console.WriteLine ("StartRecMsg The Conn Socket is NULL");
				return;
			}
			m_socket.BeginReceive (m_bytes , 0 , 1024, 0 ,  MessageCallBack, this);
		}

		public void MessageCallBack(IAsyncResult state)
		{
			Connector conn = (Connector)state.AsyncState;
			if (conn == null) 
			{
				Console.WriteLine ("MsgCallBack The State is NULL");
				return;
			}

			IPEndPoint ed = (IPEndPoint)conn.ConnSocket.RemoteEndPoint;
			int count = conn.ConnSocket.EndReceive (state);
			if (count <= 0) 
			{
				Console.WriteLine ("conn is " + ed.ToString());
				conn.ConnUsed = false;
				conn.ConnSocket.Close ();
				return;
			}

			string str = System.Text.Encoding.UTF8.GetString (conn.ConnBytes , 0 , count);
			Console.WriteLine ("conn is " + ed.ToString() + "say: "+ str);
			str = ed.ToString () + " Says: " + str;  
			if (m_sendAllMsg != null)
				m_sendAllMsg (this, str);

			this.StartReceiveMessage ();
		}

		public byte[] ConnBytes
		{
			get
			{ 
				return m_bytes;
			}
		}

		public bool ConnUsed
		{
			get
			{ 
				return m_isUsed;
			}
			set
			{ 
				m_isUsed = value;
			}
		}

		public Socket ConnSocket
		{
			get
			{ 
				return m_socket;
			}
			set
			{ 
				m_socket = value;
			}
		}

	}
}

