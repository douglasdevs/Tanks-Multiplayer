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

    [Header("-Score-")]
    public int maxScore = 3;

    [SyncVar]
    bool gameOver = false;
    PlayerControl winner;
    
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
        StartCoroutine(GameLoopRoutine());
	}

	IEnumerator EnterLobby ()
	{
		while (playerCountConnected < minPlayersToStartMatch) {
            UpdateMessage("Esperando jogadores...");
			DisablePlayers ();
			yield return null;
		}
	}

	IEnumerator PlayGame ()
	{
        UpdateMessage("");
		EnablePlayers ();
        UpdateScore();
        while (!gameOver)
        {
            yield return null;
        }
	}

	IEnumerator EndGame ()
	{
        DisablePlayers();
        UpdateMessage("GAME OVER \n "+winner.pSetup.playerNameText.text + " venceu!");
        Reset();
        yield return new WaitForSeconds(3.0f);
        UpdateMessage("Reiniciando...");
		yield return new WaitForSeconds(3.0f);
	}

    [ClientRpc]
    void RpcUpdateMessage(string msg)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = msg;
    }

    public void UpdateMessage(string msg)
    {
        if (isServer)
        {
            RpcUpdateMessage(msg);
        }
    }

    [ClientRpc]
	void RpcSetPlayerState (bool state)
	{
		PlayerControl[] allPlayers = FindObjectsOfType<PlayerControl> () as PlayerControl[];
		foreach (PlayerControl p in allPlayers) {
			p.enabled = state;
		}
	}

	void EnablePlayers ()
    {
        if (isServer)
        {
            RpcSetPlayerState(true);
        }
	}

	void DisablePlayers ()
	{
        if (isServer)
        {
            RpcSetPlayerState(false);
        }
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
            winner = GetWinner();
            if(winner!=null)
            {
                gameOver = true;
            }
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

    PlayerControl GetWinner()
    {
        if (isServer)
        {
            for (int i = 0; i < playerCountConnected; i++)
            {
                if(allPlayers[i].score >= maxScore)
                {
                    return allPlayers[i];
                }
            }
        }

        return null;
    }

    void Reset()
    {
        if (isServer)
        {
            RpcReset();
            UpdateScore();
            winner = null;
            gameOver = false;
        }
    }

    [ClientRpc]
    void RpcReset()
    {
        PlayerControl[] players = FindObjectsOfType<PlayerControl>();
        foreach(var player in players)
        {
            player.score = 0;
            player.pHealth.Reset();
        }
    }
}
