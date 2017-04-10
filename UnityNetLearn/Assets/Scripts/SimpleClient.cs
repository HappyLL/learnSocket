using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class SimpleClient : MonoBehaviour {

	private Socket m_socket;

	// Use this for initialization
	void Start () {
		m_socket = new Socket (AddressFamily.InterNetwork , SocketType.Stream , ProtocolType.Tcp);
		IPAddress ipAdress = IPAddress.Parse("127.0.0.1");

		IPEndPoint ed = new IPEndPoint (ipAdress , 1234);
		m_socket.Connect (ed);

		string s = "The Client Is Connect!";

		byte [] sBy = System.Text.Encoding.Default.GetBytes(s);
		m_socket.Send(sBy);

		byte[] reBy = new byte[1024];
		int count  = m_socket.Receive(reBy);

		string recs = System.Text.Encoding.UTF8.GetString (reBy, 0 , count);
		print (recs);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
