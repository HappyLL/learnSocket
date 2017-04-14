using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ClientUI : MonoBehaviour {

	public Button m_SendButton;
	public Text m_RecContent;
	public InputField m_InputField;

	private string m_text;
	private MultiClient m_client;
	private string m_cbText;
	// Use this for initialization
	void Start () {
		m_SendButton.onClick.AddListener (OnClickCallBack);
		m_InputField.onEndEdit.AddListener (OnInputEndedEdit);

		m_cbText = "";
		m_text = "";

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

		GetMsg (m_text);
		m_client.SendMsg (m_text);
		m_text = "";
	}

	private void GetMsg(string con)
	{
		m_cbText = con + "\n"; 
	}


	// Update is called once per frame
	void Update () {
		
		if(m_cbText.Length > 0 )
		{
			m_RecContent.text += m_cbText;
			m_cbText = "";
		}

	}

	void OnDestory()
	{
		m_client.UnInitClient ();
	}
}
