using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionField : MonoBehaviour
{
	[SerializeField] float _reprojectionRadius = 10f;
	[SerializeField] float _reprojectionForce = 1000f;
	[SerializeField] float _repojectTimer = 2.0f;

	void Start ()
	{
		StartCoroutine (Reproject ());
	}

	void OnEnable ()
	{
		StartCoroutine (Reproject ());
	}

	private void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("Bullet"))
		{
			Bullet b = other.GetComponent<Bullet> ();
			if (b) b.ChangeVelocityPercent (0.00001f);
		}
	}

	IEnumerator Reproject ()
	{
		while (true)
		{
			yield return new WaitForSeconds (_repojectTimer);
			Collider[] colliders = Physics.OverlapSphere (transform.position, _reprojectionRadius);
			foreach (Collider nearbyObject in colliders)
			{
				Rigidbody rb = nearbyObject.GetComponent<Rigidbody> ();
				if (rb) rb.AddExplosionForce (_reprojectionForce, transform.position, _reprojectionRadius);
			}
		}

	}
}