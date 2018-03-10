using System.Collections;
using System.Collections.Generic;
using ManagerClasses;
using ObjectPooling;
using UnityEngine;

public class Bullet : PoolObject
{

	Rigidbody _rb;
	SphereCollider _col;
	TrailRenderer _tr;
	[SerializeField] float _bulletInactivetime = 0.04f;

	[SerializeField] float _initialSpeed, _speed = 4f;
	[SerializeField] int _initialDamage, _damage = 4;

	[SerializeField] List<AudioClip> _soundsLaunch;

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

	void Update ()
	{
		if (_rb.velocity.magnitude < _speed) _rb.velocity = (_rb.velocity.normalized * _speed);
	}

	public override void OnObjectReuse ()
	{
		_speed = _initialSpeed;
		_damage = _initialDamage;

		_tr.Clear ();
		_col.enabled = false;
		StartCoroutine (EnableBoxCollider ());
	}

	public void SetVelocity (Vector2 para)
	{
		AudioManager.instance.Play (_soundsLaunch.RandomFromList ().name);
		_rb.velocity = para.normalized * _speed;
	}

	IEnumerator EnableBoxCollider ()
	{
		yield return new WaitForSeconds (_bulletInactivetime);
		_col.enabled = true;
	}

	public void ChangeVelocityPercent (float per_)
	{
		_speed *= per_;
		SetVelocity (_rb.velocity * _speed);
	}

	private void OnCollisionEnter (Collision other)
	{
		PoolManager.instance.ReuseHitEffectPool (transform.position, Quaternion.identity);

		if (other.gameObject.CompareTag ("Player"))
		{
			Health h = other.gameObject.GetComponent<Health> ();
			h.Damage (_damage);
			_damage = 0;
			Destroy (5.0f);
		}
	}

	void PlaySound () { }
}