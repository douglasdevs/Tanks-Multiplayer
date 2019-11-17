using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerSetup : NetworkBehaviour
{
	public Color playerColor;
	public string baseName = "PLAYER";
	public int playerNum = 1;
	public Text playerNameText;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	//This function is called only if you are a client. In this case not a local player
	public override void OnStartClient ()
	{
		base.OnStartClient ();
		playerNameText.enabled = false;
	}

	//This function is called on local player only
	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();

		CmdSetupPlayer ();

		MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer> ();
		foreach (MeshRenderer m in meshes) {
			m.material.color = playerColor;
		}

		playerNameText.enabled = true;
		playerNameText.text = baseName + " " + playerNum;
	}

	[Command]
	void CmdSetupPlayer ()
	{
		GameManager.instance.AddPlayer (this);
	}
}
