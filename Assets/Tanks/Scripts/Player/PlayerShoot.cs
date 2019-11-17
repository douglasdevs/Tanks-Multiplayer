using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(Rigidbody))]
public class PlayerShoot : NetworkBehaviour
{
	public Rigidbody bulletPrefab;
	public float bulletSpeed = 20.0f;
	public float fireRate = 0.5f;
	public Transform bulletSpawn;

	float nextFire;

	[Command] //This function is called in the client and executed in the server to all clients
	public void CmdShoot ()
	{
		if (Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			Rigidbody tempBullet = Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation) as Rigidbody;
			tempBullet.velocity = bulletSpeed * bulletSpawn.transform.forward;

			NetworkServer.Spawn (tempBullet.gameObject);
		}
	}

}
