using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour 
{
	public bool isOccupied = false;

	void OnTriggerEnter(Collider other)
	{
        if (other.CompareTag("Player"))
		{
			isOccupied = true;
		}
	}

	void OnTriggerStay(Collider other) 
	{
	    if (other.CompareTag("Player"))
		{
			isOccupied = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
	    if (other.CompareTag("Player"))
		{
			isOccupied = false;
		}
	}
}
