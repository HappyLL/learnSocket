using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplePosMgr : MonoBehaviour {

	public GameObject m_template;
	public GameObject m_parentUI;

	private SimplePosClient m_spCt;

	private Queue<string> m_msgQ;

	//private Dictionary<int , List<string>> m_msgDc;

	private Dictionary<int , GameObject> m_gObjDic;

	private int m_curIndex;

	// Use this for initialization
	void Start () {
		
		m_curIndex = -1;

		m_msgQ = new Queue<string> ();
		//m_msgDc = new Dictionary<int, List<string>>();
		m_gObjDic = new Dictionary<int, GameObject> ();

		m_spCt = new SimplePosClient ();
		m_spCt.SPClientInit (PushMsg);
		m_spCt.SPClientStart ();

		SendCreateMsg ();
	}
	
	// Update is called once per frame
	void Update () {
		HandleMsg ();
		HandleInput ();
	}

	private void SendMsg(string content)
	{
		m_spCt.Send (content);
	}

	//发送创建gObj信息(create 信息表示当前只发给自己，且只发一次)
	private void SendCreateMsg()
	{
		string type = "Create";
		float x = Random.Range (0 , 5);
		float y = Random.Range (0 , 5);
		float z = 0;
		string content = type + '|' + "-1" +'|'+ x + '|' + y + '|' + z ;
		SendMsg (content);
	}

	private void SendPosMsg()
	{
		GameObject obj = m_gObjDic [m_curIndex];
		if (obj == null)
			return;
		
		Transform trans = obj.GetComponent<Transform> ();
		string type = "Post";
		float x = trans.position.x;
		float y = trans.position.y;
		float z = trans.position.z;
		string content = type + '|' + m_curIndex + '|' + x + '|' + y + '|' + z;
		SendMsg (content);
	}

	private void SendLeaveMsg()
	{
		if (m_curIndex != -1)
			return;
		
		string type = "Leave";
		string content = type + '|' + m_curIndex + '|' + "-1" + '|' + "-1" + '|' + "-1";
		SendMsg (content);
	}

	//处理消息更新
	private void HandleMsg()
	{
		if (m_msgQ == null)
			return;
		
		if (m_msgQ.Count == 0)
			return;

		string content = m_msgQ.Dequeue ();

		string[] proto = content.Split ('|');
		if (proto.Length <= 1)
			return;
		
		string type = proto [0];

		int index = int.Parse(proto [1]);

		if (index == -1)
			return;

		Debug.Log (content);

		if (type == "Create")
			HandleCreateMsg (proto , index);
		else if (type == "Post")
			HandlePosMsg (proto , index);
		else if (type == "Leave")
			HandleLeaveMsg (proto , index);
		
		return;
	}
		
	private void CreateObj(int ind , Vector3 pos)
	{
		GameObject obj = GameObject.Instantiate (m_template , pos , Quaternion.identity) as GameObject;

		m_gObjDic.Add( ind , obj );

		if (m_parentUI != null)
			obj.transform.SetParent (m_parentUI.transform);
	}

	//id为服务端连接池的id
	private void HandleCreateMsg(string[] proto , int ind)
	{
		m_curIndex = ind;

		int x = int.Parse(proto [2]);
		int y = int.Parse(proto [3]);
		int z = int.Parse(proto [4]);

		this.CreateObj (ind , new Vector3(x , y , z));

		SendPosMsg ();
	}

	private void HandleLeaveMsg(string[] proto , int ind)
	{
		GameObject obj = m_gObjDic [ind];
		if (obj == null)
			return;
		
		m_gObjDic.Remove (ind);
		GameObject.Destroy (obj);
	}

	private void HandlePosMsg(string[] proto , int ind)
	{
		Debug.Log (ind);
		GameObject obj = null;
		if (m_gObjDic.ContainsKey (ind)) {
			obj = m_gObjDic [ind];
		}
		int x = int.Parse(proto [2]);
		int y = int.Parse(proto [3]);
		int z = int.Parse(proto [4]);
		if (obj == null) 
		{
			this.CreateObj (ind , new Vector3(x , y , z));	
			obj = m_gObjDic [ind];
		}

		obj.transform.position = new Vector3 (x , y , z);
	}

	//处理案件信息
	private void HandleInput()
	{
		if (m_curIndex == -1)
			return;
		
		GameObject obj = m_gObjDic [m_curIndex];
		if (obj == null)
			return;
		
		float xx = obj.transform.position.x;
		float yy = obj.transform.position.y;
		float zz = obj.transform.position.z;

		bool flag = false;

		if (Input.GetKeyDown (KeyCode.W)) 
		{
			flag = true;
			yy += 1;
		}
		else if (Input.GetKeyDown (KeyCode.S)) 
		{
			flag = true;
			yy -= 1;
		}
		else if (Input.GetKeyDown (KeyCode.A)) 
		{
			flag = true;
			xx -= 1;
		}
		else if (Input.GetKeyDown (KeyCode.D)) 
		{
			flag = true;
			xx += 1;
		}
			
		if (!flag)
			return;

		obj.transform.position = new Vector3 (xx , yy , zz);
		 
		SendPosMsg ();
	}

	void OnDestory()
	{
		Debug.Log ("OnDestory");
		SendLeaveMsg ();
		m_spCt.SPClientUnit ();
	}

	//消息压栈
	private void PushMsg(string content)
	{
		m_msgQ.Enqueue (content);
	}
}
