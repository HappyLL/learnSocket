using System;
using System.Timers;
using System.Threading;

namespace SimpleNet
{
	//测试线程
	public class MyThread
	{
		//临界资源
		private string m_str;

		private Thread m_th1;
		private Thread m_th2;

		public MyThread ()
		{
			
		}

		public void Init()
		{
			m_str = "";

			m_th1 = new Thread (AddA);

			m_th2 = new Thread (AddB);
		}

		public void UnInit()
		{
			m_str = null;
			m_th1 = null;
			m_th2 = null;
		}

		public void Start()
		{
			m_th1.Start ();
			m_th2.Start ();

			Thread.Sleep (10000);
			Console.WriteLine(m_str);
		}

		private void AddA()
		{
			//直到线程th1释放m_str时线程th2才会运行
			lock (m_str) 
			{
				for (int i = 0; i < 20; ++i) 
				{
					Thread.Sleep (100);
					m_str += "A";
				}
			}
		}

		private void AddB()
		{
			lock (m_str) 
			{
				for (int i = 0; i < 20; ++i) 
				{
					Thread.Sleep (10);
					m_str += "B";
				}
			}
		}
	}
}

