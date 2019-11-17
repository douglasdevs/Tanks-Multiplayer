using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
	public float maxHealth = 100.0f;
	public GameObject deathPrefab;

	[SyncVar]
	public bool isDead = false;
	public Image healthBar;

    public PlayerControl lastAttacker;

	[SyncVar (hook = "UpdateHealthBar")]
	float currentHealth;

	//	void Awake ()
	//	{
	//		healthBar.fillAmount = maxHealth / 100.0f;
	//	}

	// Use this for initialization
	void Start ()
	{
		currentHealth = maxHealth;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void TakeDamage (float damage, PlayerControl pc = null)
	{
		if (!isServer)
			return;

        if (pc != null && pc != this.GetComponent<PlayerControl>())
        {
            lastAttacker = pc;
        }

		currentHealth -= damage;
//		UpdateHealthBar (currentHealth);

		if (currentHealth <= 0.0f && !isDead) {
            if (lastAttacker != null) {
                lastAttacker.score++;
                lastAttacker = null;
             }
            GameManager.instance.UpdateScore();
			isDead = true;
			RpcDie ();
		}
	}

	void UpdateHealthBar (float value)
	{
		healthBar.fillAmount = value / 100.0f;
	}

	[ClientRpc] // This function is called in the server and executed in the all clients
	void RpcDie ()
	{
		GameObject deathFX = Instantiate (deathPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
		Destroy (deathFX, 3.0f);
		SetActiveState (false);

		gameObject.SendMessage ("Disable");
	}

	void SetActiveState (bool state)
	{
		Collider[] cols = GetComponentsInChildren<Collider> ();

		foreach (var c in cols) {
			c.enabled = state;
		}

		Canvas[] canvas = GetComponentsInChildren<Canvas> ();

		foreach (var cv in canvas) {
			cv.enabled = state;
		}

		Renderer[] renderers = GetComponentsInChildren<Renderer> ();

		foreach (var r in renderers) {
			r.enabled = state;
		}

	}

	public void Reset ()
	{
		currentHealth = maxHealth;
		SetActiveState (true);
		isDead = false;
	}
}
