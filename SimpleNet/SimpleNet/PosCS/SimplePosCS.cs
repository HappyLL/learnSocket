using System;
using System.Net;
using System.Net.Sockets;

namespace SimpleNet
{
	public class SimplePosCS
	{
		private const int CONN_NUM = 50;

		private Socket m_socket;
		private SimplePosConn[] m_posConns;

		public SimplePosCS ()
		{
			
		}

		public void SPCSInit()
		{
			m_posConns = new SimplePosConn[CONN_NUM];

			m_socket = new Socket (AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);

		}
			
		public void SPCSStart(string ip = "127.0.0.1" , int port = 1234)
		{
			IPAddress ipAdd = IPAddress.Parse (ip);
			IPEndPoint ipEnd = new IPEndPoint (ipAdd , port);

			m_socket.Bind (ipEnd);
			m_socket.Listen (CONN_NUM);
			m_socket.BeginAccept (AcceptConnCb , null);

			Console.WriteLine ("Server is Running");
		}

		private SimplePosConn NewSPConn()
		{
			for (int i = 0; i < m_posConns.Length; ++i) 
			{
				if (m_posConns [i] == null) 
				{
					m_posConns [i] = new SimplePosConn ();
					m_posConns [i].SPConnInit (HandleMsg);
					return m_posConns [i];
				} 
				else if(!m_posConns[i].PosConnUsed)
				{
					return m_posConns [i];
				}
			}

			return null;
		}

		private int GetIndex(SimplePosConn conn)
		{
			for (int i = 0; i < m_posConns.Length; ++i) 
			{
				if (m_posConns [i] == conn)
					return i;
			}

			return -1;
		}

		private void AcceptConnCb(IAsyncResult ret)
		{
			Socket sk = m_socket.EndAccept (ret);
			SimplePosConn conn = NewSPConn ();
			if (conn == null) 
			{
				Console.WriteLine ("The Connector Pool Is Full");
				sk.Close ();
				return;
			}

			conn.PosConnSocket = sk;
			conn.PosConnUsed = true;
			conn.SPConnStart ();

			Console.WriteLine (sk.RemoteEndPoint.ToString() + "Is Connected");

			m_socket.BeginAccept (AcceptConnCb, null);
		}

		private void HandleMsg(string content, SimplePosConn conn)
		{
			if (content.Length == 0 || conn == null) 
				return;
			
			PrintMsg (content , conn);
			string[] proto = content.Split ('|');

			if (proto.Length <= 1)
				return;
				
			if (proto[0].Equals("Leave"))
			{
				LeaveMsg (conn);
			} 
			else if (proto[0].Equals("Create")) 
			{
				//可能当本次连接创建后已经有其他连接 需要向该conn发送其他人的信息(数据小会将数据合并：先发了创建又发了其他人的信息所以会导致数据合并)
				proto [1] = GetIndex (conn) + ""; 
				content = string.Join ("|" , proto);

				content += this.GetMsgToOne (conn);

				byte[] sBy = System.Text.Encoding.Default.GetBytes (content);
				conn.PosConnSocket.Send (sBy);
			} 
			else if (proto[0].Equals("Post"))
			{
				SendMsgToAll (content, conn);
			}
				
		}
			
		private void PrintMsg(string content , SimplePosConn conn)
		{
			Socket sk = conn.PosConnSocket;

			string msgAddress =	sk.RemoteEndPoint.ToString();
			string msgSays = msgAddress + " content: " + content;

			Console.WriteLine (msgSays);
		}

		private string GetMsgToOne(SimplePosConn conn)
		{

			string ret = "";
			for (int i = 0; i < m_posConns.Length; ++i) 
			{
				if (m_posConns [i] != null && m_posConns [i].PosConnUsed && m_posConns [i] != conn) 
				{
					string str = System.Text.Encoding.UTF8.GetString (m_posConns[i].GetContent() , 0 , m_posConns[i].ByteCount);
					string[] proto = str.Split ('|');
					if (proto.Length <= 1 || proto [0] == "Leave")
						continue;
					proto[0] = "Post";
					str = string.Join ("|",proto);
					ret = ret + '}' + str;
					//byte[] sBy = System.Text.Encoding.Default.GetBytes (str);
					//conn.PosConnSocket.Send (sBy);
				}
			}

			return ret;
		}

		private void SendMsgToAll(string content , SimplePosConn conn)
		{
			byte[] sBy = System.Text.Encoding.Default.GetBytes (content);

			for (int i = 0; i < m_posConns.Length; ++i) 
			{
				if (m_posConns [i] != null && m_posConns [i].PosConnUsed && m_posConns[i] != conn) 
				{
					m_posConns [i].PosConnSocket.Send (sBy);
				}
			}
		}

		private void LeaveMsg(SimplePosConn conn)
		{
			Socket sk = conn.PosConnSocket;

			string msg = sk.RemoteEndPoint.ToString ();
			msg += "Leaved";

			SendMsgToAll (msg , conn);

			conn.SPShutDown ();
		}

		public void SPCSUInit()
		{
			for (int i = 0; i < m_posConns.Length; ++i) 
			{
				m_posConns [i] = null;
			}

			m_socket.Close ();
			m_socket = null;
		}

	}
}

