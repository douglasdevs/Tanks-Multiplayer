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

    public Color[] playerColors;


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
            pS.playerColor = playerColors[playerCountConnected];
            pS.playerNum = playerCountConnected + 1;
			playerCountConnected++;
		}
	}

    [ClientRpc]
    void RpcUpdateScore(int[] playerScores, string[] playerNames)
    {
        for (int i = 0; i < playerCountConnected; i++)
        {
            playerScoreText[i].text = playerScores[i].ToString();
            nameText[i].text = playerNames[i];
        }
    }

    public void UpdateScore()
    {
        if (isServer)
        {
            int[] scores = new int[playerCountConnected];
            string[] names = new string[playerCountConnected];
            for (int i = 0; i < playerCountConnected; i++)
            {
                scores[i] = allPlayers[i].score;
                names[i] = allPlayers[i].GetComponent<PlayerSetup>().playerNameText.text;
            }

            RpcUpdateScore(scores, names);
        }
    }
}
