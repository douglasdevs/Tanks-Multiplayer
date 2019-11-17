using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
	public static GameManager instance;

	public Text messageText;

	// Minimum players to start match
	public int minPlayersToStartMatch = 2;
	// Maximum players for each room
	public int maxPlayersForMatch = 4;

	[SyncVar]//Current amount players connected until now
	public int playerCountConnected = 0;


	void Awake ()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		StartCoroutine (GameLoopRoutine ());
	}

	IEnumerator GameLoopRoutine ()
	{
		yield return EnterLobby ();
		yield return PlayGame ();
		yield return EndGame ();
	}

	IEnumerator EnterLobby ()
	{
		messageText.gameObject.SetActive (true);
		messageText.text = "Esperando jogadores...";

		while (playerCountConnected < minPlayersToStartMatch) {
			DisablePlayers ();
			yield return null;
		}
	}

	IEnumerator PlayGame ()
	{
		EnablePlayers ();
		messageText.gameObject.SetActive (false);
		yield return null;
	}

	IEnumerator EndGame ()
	{
		yield return null;
	}

	void SetPlayerState (bool state)
	{
		PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl> () as PlayerControl[];
		foreach (PlayerControl p in allPlayers) {
			p.enabled = state;
		}
	}

	void EnablePlayers ()
	{
		SetPlayerState (true);
	}

	void DisablePlayers ()
	{
		SetPlayerState (false);
	}

	public void AddPlayer ()
	{
		if (playerCountConnected < maxPlayersForMatch) {
			playerCountConnected++;
		}
	}
}
