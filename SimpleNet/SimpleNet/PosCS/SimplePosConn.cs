using System;
using System.Net;
using System.Net.Sockets;

namespace SimpleNet
{
	public class SimplePosConn
	{

		public delegate void SendMsg (string content , SimplePosConn conn);

		private const int BYTE_NUM = 1024;

		private Socket m_socket;
		private bool m_isUsed;
		private byte[] m_bytes;
		private int m_byteCnt;

		private SendMsg m_fSMsg;

		public SimplePosConn ()
		{
		}

		public void SPConnInit(SendMsg fSAMsg)
		{
			m_socket = null;
			m_isUsed = false;

			m_bytes = new byte[BYTE_NUM];
			m_byteCnt = 0;

			m_fSMsg = fSAMsg;
		}

		public void SPConnStart()
		{
			if (m_socket == null) 
			{
				Console.WriteLine ("The Socket Is Null");
				return;		
			}

			int offset = 0;
			m_socket.BeginReceive (m_bytes , offset , BYTE_NUM , SocketFlags.None , RecMsgCb , this);
		}

		private void RecMsgCb(IAsyncResult ret)
		{
			SimplePosConn conn = (SimplePosConn)ret.AsyncState;
			if (conn == null) 
			{
				Console.WriteLine ("The Conn is Null");
				return;
			}
			try
			{
				int ind = 0;
				int count  = conn.PosConnSocket.EndReceive(ret);
				conn.ByteCount = count;

				String s = System.Text.Encoding.Default.GetString(conn.GetContent() , ind , count);
				//回调发送
				if(m_fSMsg != null)
					m_fSMsg(s , conn);
				else
					Console.WriteLine("The CB Is Null");
			}
			catch(Exception e) 
			{
				Console.WriteLine (e.ToString());
			}

			conn.SPConnStart ();
		}

		public byte[] GetContent()
		{
			return m_bytes;
		}

		public int ByteCount
		{
			get
			{
				return m_byteCnt;
			}
			set
			{ 
				m_byteCnt = value;
			}
		}

		public void SPConnUnit()
		{
			m_socket.Close ();
			m_socket = null;
			m_isUsed = false;
			m_bytes = null;
		}

		public void SPShutDown()
		{
			m_socket.Close ();
			m_isUsed = false;
		}
			

		public Socket PosConnSocket
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

		public bool PosConnUsed
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
	}
}

