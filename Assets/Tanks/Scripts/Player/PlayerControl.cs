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
	public PlayerHealth pHealth;
    public PlayerSetup pSetup;
	NetworkStartPosition[] spawnPoints;

	public float respawnTime = 3.0f;

	Vector3 originalPosition;

	void Start ()
	{
		pMotor = GetComponent<PlayerMotor> ();
		pShoot = GetComponent<PlayerShoot> ();
		pHealth = GetComponent<PlayerHealth> ();
        pSetup = GetComponent<PlayerSetup>();
	}

	public override void OnStartLocalPlayer ()
	{
		spawnPoints = FindObjectsOfType (typeof(NetworkStartPosition)) as NetworkStartPosition[];
		originalPosition = transform.position;
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
		SpawnPoint oldSpawn = GetNearestSpawnPoint ();
		if (oldSpawn != null) {
			oldSpawn.isOccupied = false;
		}

		transform.position = GetRandomSpawnPosition ();
		pMotor.GetRigidbody ().velocity = Vector3.zero;
		yield return new WaitForSeconds (respawnTime);
		pHealth.Reset ();
		GameObject newSpawnFX = Instantiate (spawnFX, transform.position, Quaternion.identity);
		Destroy (newSpawnFX, 3.0f);
	}

	SpawnPoint GetNearestSpawnPoint ()
	{
		Collider[] triggerColliders = Physics.OverlapSphere (transform.position, 3.0f, Physics.AllLayers, QueryTriggerInteraction.Collide);
		foreach (var c in triggerColliders) {
			SpawnPoint spawnPoint = c.GetComponent<SpawnPoint> ();
			if (spawnPoint != null) {
				return spawnPoint;
			}
		}

		return null;
	}

	Vector3 GetRandomSpawnPosition ()
	{
		if (spawnPoints != null) {
			bool foundSpawner = false;
			Vector3 newStartPosition = new Vector3 ();
			float timeOut = Time.time + 2.0f;

			while (!foundSpawner) {
				NetworkStartPosition startPoint = spawnPoints [Random.Range (0, spawnPoints.Length)];
				SpawnPoint spawnPoint = startPoint.GetComponent<SpawnPoint> ();

				if (!spawnPoint.isOccupied) {
					newStartPosition = startPoint.transform.position;
					foundSpawner = true;
				}

				if (Time.time > timeOut) {
					newStartPosition = originalPosition;
					foundSpawner = true;
				}
			}

			return newStartPosition;
		}

		return originalPosition;
	}
}
