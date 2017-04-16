using System;


namespace SimpleNet
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//简单服务器
			//SimpleServer svr = new SimpleServer ();
			//svr.ServerStart ();

			//MultiChatServer svr = new MultiChatServer ();
			//svr.ServerInit ();
			//svr.ServerStart ();

			/*while (true) 
			{
				if (Console.ReadLine () == "quit")
					break;	
			}
			svr.ServerUInit ();*/
			MyThread th = new MyThread ();
			th.Init ();
			th.Start ();
		}
	}
}
