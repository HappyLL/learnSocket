using System;
using System.Net;
using System.Net.Sockets;

namespace SimpleNet
{
	public class MultiChatServer
	{
		private const int CONN_NUM = 50;

		private Connector []m_conntors;
		private Socket m_socket;

		public MultiChatServer ()
		{
		}

		public void ServerInit()
		{
			m_socket = new Socket (AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);

			m_conntors = new Connector[CONN_NUM];
		}

		public void ServerStart(string ip = "127.0.0.1" , int port = 1234)
		{
			IPAddress ipAddress = IPAddress.Parse (ip);
			IPEndPoint ed = new IPEndPoint (ipAddress, port);

			m_socket.Bind (ed);
			m_socket.Listen (CONN_NUM);//表示最多能监听CONN_NUM

			Console.WriteLine ("开始接受客户端连接");
			StartAcceptConnector ();
		}

		private Connector NewIndex()
		{
			for (int i = 0; i < m_conntors.Length; ++i) 
			{
				if (m_conntors [i] == null) 
				{
					m_conntors [i] = new Connector (SendAllMsg);
					return m_conntors [i];
				} 
				else if(!m_conntors[i].ConnUsed)
				{
					return m_conntors [i];
				}
			}
			return null;
		}

		private void StartAcceptConnector()
		{
			m_socket.BeginAccept (EndAcceptConnector , this);
		}

		private void EndAcceptConnector(IAsyncResult state)
		{
			Socket clientSk = m_socket.EndAccept (state);
			if (clientSk == null) 
			{
				Console.WriteLine ("the client Sk is Null");
				StartAcceptConnector ();
				return;
			}

			Connector conn = NewIndex();
			if (conn == null) 
			{
				Console.WriteLine ("The Client Poor Is Full");
				return;
			}

			conn.ConnUsed = true;
			conn.ConnSocket = clientSk;//连接池的对象保持socket

			IPEndPoint ed = (IPEndPoint)conn.ConnSocket.RemoteEndPoint;
			Console.WriteLine ("conn" + ed.ToString()+" connected");

			conn.StartReceiveMessage ();

			StartAcceptConnector ();
		}

		public void ServerUInit()
		{
			m_conntors = null;
		}

		public void SendAllMsg(Connector obj , string msg)
		{
			if (obj == null || msg == null || msg.Length == 0)
				return;
			for (int i = 0; i < m_conntors.Length; ++i) 
			{
				if (m_conntors[i] == obj || m_conntors [i] == null || !m_conntors [i].ConnUsed)
					continue;
				byte[] sBy = System.Text.Encoding.Default.GetBytes (msg);
				m_conntors [i].ConnSocket.Send (sBy);
			}
		}

	}
}

