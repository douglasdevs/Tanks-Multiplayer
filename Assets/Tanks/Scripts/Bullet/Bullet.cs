using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public ParticleSystem explosionFX;
	public int bounces = 2;

    public PlayerControl owner;

	public float damage = 1.0f;

	Rigidbody rb;
	Collider col;

	// Use this for initialization
	void Start ()
	{
		rb = GetComponent<Rigidbody> ();
		col = GetComponent<Collider> ();
	}

	void Explode ()
	{
		rb.velocity = Vector3.zero;
		rb.Sleep ();
		col.enabled = false;

		MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer> ();

		foreach (MeshRenderer m in meshes) {
			m.enabled = false;
		}

		ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem> ();

		foreach (ParticleSystem ps in particles) {
			ps.Stop ();
		}

		explosionFX.transform.SetParent (null);
		explosionFX.Play ();
		Destroy (explosionFX, 1.0f);
		Destroy (gameObject);
	}

	void OnCollisionExit (Collision collision)
	{
		if (rb.velocity != Vector3.zero) {
			transform.rotation = Quaternion.LookRotation (rb.velocity);
		}
	}

	void OnCollisionEnter (Collision collision)
	{

		PlayerHealth pHealth = collision.gameObject.GetComponent<PlayerHealth> ();

		if (pHealth != null) {
			Explode ();
			pHealth.TakeDamage (damage, owner);
		}

		if (bounces <= 0) {
			Explode ();
		}

		bounces--;
	}
}
