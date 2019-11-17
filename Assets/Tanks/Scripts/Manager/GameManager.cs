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

    public List<PlayerControl> allPlayers;
    public List<Text> nameText;
    public List<Text> playerScoreText;


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
        UpdateScore();
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

	public void AddPlayer (PlayerSetup pS)
	{
		if (playerCountConnected < maxPlayersForMatch) {
            allPlayers.Add(pS.GetComponent<PlayerControl>());
			playerCountConnected++;
		}
	}

    [ClientRpc]
    void RpcUpdateScore(int[] playerScores)
    {
        for (int i = 0; i < playerCountConnected; i++)
        {
            playerScoreText[i].text = playerScores[i].ToString();
        }
    }

    public void UpdateScore()
    {
        if (isServer)
        {
            int[] scores = new int[playerCountConnected];
            for (int i = 0; i < playerCountConnected; i++)
            {
                scores[i] = allPlayers[i].score;
            }

            RpcUpdateScore(scores);
        }
    }
}
