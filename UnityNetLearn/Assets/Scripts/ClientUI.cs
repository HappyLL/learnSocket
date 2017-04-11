using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientUI : MonoBehaviour {

	public Button m_SendButton;
	public Text m_RecContent;
	public InputField m_InputField;

	private string m_text;
	private MultiClient m_client;
	// Use this for initialization
	void Start () {
		m_SendButton.onClick.AddListener (OnClickCallBack);
		m_InputField.onEndEdit.AddListener (OnInputEndedEdit);

		m_client = new MultiClient ();
		m_client.InitClient (GetMsg);

		m_client.StartClient ();
	}

	private void OnInputEndedEdit(string text)
	{
		m_text = text;
	}

	private void OnClickCallBack()
	{
		if (m_text == null || m_text.Equals ("") || m_text.Length == 0)
			return;
		
		m_client.SendMsg (m_text);
	}

	private void GetMsg(string con)
	{
		m_RecContent.text += con + "\n"; 
	}


	// Update is called once per frame
	void Update () {
	
	}
}
