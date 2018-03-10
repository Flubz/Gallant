using System.Collections;
using System.Collections.Generic;
using ManagerClasses;
using ObjectPooling;
using UnityEngine;

public class Torp : PoolObject
{

	Rigidbody _rb;
	SphereCollider _col;
	TrailRenderer _tr;
	[SerializeField] float _bulletInactivetime = 0.04f;
	[SerializeField] float _timeUntilBoom = 4f;

	[SerializeField] float _boomRadius = 10f;
	[SerializeField] float _boomForce = 1000f;
	[SerializeField] float _secondsToDisableFor = 4f;

	[SerializeField] float _speed = 4f;
	[SerializeField] int _damage = 12;

	void Awake ()
	{
		_rb = GetComponent<Rigidbody> ();
		_col = GetComponent<SphereCollider> ();
		_tr = GetComponent<TrailRenderer> ();
	}

	void Start ()
	{
		OnObjectReuse ();
	}

	public override void OnObjectReuse ()
	{
		_tr.Clear ();
		_col.enabled = false;
		StartCoroutine (EnableCollider ());
		StartCoroutine (Explode ());
	}

	public void SetVelocity (Vector2 para)
	{
		_rb.velocity = para.normalized * _speed;
	}

	IEnumerator EnableCollider ()
	{
		yield return new WaitForSeconds (_bulletInactivetime);
		_col.enabled = true;
	}

	IEnumerator Explode ()
	{
		yield return new WaitForSeconds (_timeUntilBoom);

		Collider[] colliders = Physics.OverlapSphere (transform.position, _boomRadius);
		foreach (Collider nearbyObject in colliders)
		{
			Rigidbody rb = nearbyObject.GetComponent<Rigidbody> ();
			if (rb) rb.AddExplosionForce (_boomForce, transform.position, _boomRadius);
			Player p = nearbyObject.GetComponent<Player> ();
			if (p) p.TorpPlayer (_damage, _secondsToDisableFor);
			PoolManager.instance.ReuseExplosionEffect (transform.position, Quaternion.identity);
			SetInactive ();
		}
	}

	private void OnCollisionEnter (Collision other)
	{
		PoolManager.instance.ReuseHitEffectPool (transform.position, Quaternion.identity);

		if (other.gameObject.CompareTag ("Player"))
		{
			Player p = other.gameObject.GetComponent<Player> ();
			if (p) p.TorpPlayer (0, _secondsToDisableFor);
			PoolManager.instance.ReuseExplosionEffect (transform.position, Quaternion.identity);
			SetInactive ();
		}
	}
	
	void PlaySound ()
	{
		// int i = Random.Range (1, 4);
		// string s = "ES" + i.ToString ();
		// AudioManager.instance.Play (s);
	}
}