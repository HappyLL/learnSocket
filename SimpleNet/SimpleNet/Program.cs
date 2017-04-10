using System;


namespace SimpleNet
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			SimpleServer svr = new SimpleServer ();
			svr.ServerStart ();
		}
	}
}
