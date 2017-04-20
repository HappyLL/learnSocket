using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplePosMgr : MonoBehaviour {

	private SimplePosClient m_spCt;

	private Dictionary<int , List<string>> m_msgDc;

	private Dictionary<int , GameObject> m_gObjDic;

	private int m_curIndex;

	// Use this for initialization
	void Start () {
		
		m_curIndex = -1;

		m_msgDc = new Dictionary<int, List<string>>();
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
		
	}

	//发送创建gObj信息
	private void SendCreateMsg()
	{
		string type = "Create";
		float x = Random.Range (10 , 50);
		float y = Random.Range (10 , 60);
		float z = 0;
		string content = type + '|' + "-1" +'|'+ x + '|' + y + '|' + z ;
		SendMsg (content);
	}

	private void SendPosMsg()
	{
		
	}

	private void SendLeaveMsg()
	{
		
	}

	//处理消息更新
	private void HandleMsg()
	{

	}

	private void HandleCreateMsg()
	{
		
	}

	private void HandleLeaveMsg()
	{
		
	}

	private void HandlePosMsg()
	{
		
	}

	//处理案件信息
	private void HandleInput()
	{

	}

	void OnDestory()
	{
		m_spCt.Send();
		m_spCt.SPClientUnit ();
	}

	//消息压栈
	private void PushMsg(string content)
	{
		
	}
}
