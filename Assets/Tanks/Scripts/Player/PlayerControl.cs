using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(PlayerMotor))]
public class PlayerControl : NetworkBehaviour
{
	public GameObject spawnFX;

    public int score;

	PlayerMotor pMotor;
	PlayerShoot pShoot;
	PlayerHealth pHealth;

	public float respawnTime = 3.0f;

	void Start ()
	{
		pMotor = GetComponent<PlayerMotor> ();
		pShoot = GetComponent<PlayerShoot> ();
		pHealth = GetComponent<PlayerHealth> ();
	}

	void Update ()
	{
		if (!isLocalPlayer || pHealth.isDead)
			return;

		Vector3 inputDirection = GetInput ();
		if (inputDirection.sqrMagnitude > 0.25f) {
			pMotor.RotateChassis (inputDirection);
		}

		Vector3 turretDir = Utility.GetWorldPointFromScreenPoint (Input.mousePosition, pMotor.turret.position.y) - pMotor.turret.position;
		pMotor.RotateTurret (turretDir);

		if (Input.GetMouseButton (0)) {
			pShoot.CmdShoot ();
		}
	}

	void FixedUpdate ()
	{
		if (!isLocalPlayer || pHealth.isDead)
			return;

		pMotor.MovePayer (GetInput ());
	}

	Vector3 GetInput ()
	{
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		return new Vector3 (h, 0, v);
	}

	void Disable ()
	{
		StartCoroutine (Respawn ());
	}

	IEnumerator Respawn ()
	{
		transform.position = Vector3.zero;
		pMotor.GetRigidbody ().velocity = Vector3.zero;
		yield return new WaitForSeconds (respawnTime);
		pHealth.Reset ();
		GameObject newSpawnFX = Instantiate (spawnFX, transform.position, Quaternion.identity);
		Destroy (newSpawnFX, 3.0f);
	}
}
