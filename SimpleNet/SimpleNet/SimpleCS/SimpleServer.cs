using System;
using System.Net;
using System.Net.Sockets;

namespace SimpleNet
{
	public class SimpleServer
	{
		private Socket m_socket;

		public SimpleServer ()
		{
		}

		//服务开始
		public void ServerStart(string ip = "127.0.0.1" , int port = 1234)
		{
			//创建socket
			m_socket = new Socket(AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);

			//创建监听地址
			IPAddress ipAddress = IPAddress.Parse (ip);
			IPEndPoint ipEd = new IPEndPoint (ipAddress, port);
			m_socket.Bind (ipEd);

			//开始监听对应端口
			m_socket.Listen (0);

			Console.WriteLine ("Svr Start IP is " + ip + "Port is " + port);

			while (true) 
			{
				Socket connSocket = m_socket.Accept ();//阻塞式接受来自客户端的连接(返回客户端的套接字)
				Console.WriteLine("Accept The Client Connect");

				//读取来自客户端的数据
				byte[] sByte = new byte[1024];//这个是字节吧
				int count = connSocket.Receive (sByte);//阻塞式
				string content = System.Text.Encoding.UTF8.GetString(sByte , 0 , count);//将字节转化成字符串

				Console.WriteLine ("The Content is "+ content);

				//发回对应的字节向客户端
				byte[] callByte = System.Text.Encoding.Default.GetBytes ("SerCallBack: " + content);
				connSocket.Send (callByte);
			}
		}
	}
}

