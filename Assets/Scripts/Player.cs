using System.Collections;
using System.Collections.Generic;
using InControl;
using ManagerClasses;
using ObjectPooling;
using UnityEngine;

public class Player : MonoBehaviour
{
	public int _playerNum;

	[SerializeField] float _speed = 3.0f;
	[SerializeField] float _rotSpeed = 24.0f;

	[Space (1.0f)]
	[Header ("Weapon")]
	[SerializeField] Transform _firepoint;
	[SerializeField] float _recoilMultiplier = 45f;
	[SerializeField] float _fireRate = 12.0f;
	[SerializeField] float _bulletSpread = 8.0f;
	float _timeToFire;

	[Space (1.0f)]
	[Header ("Dodge")]
	[SerializeField] float _dodgeDistance = 2f;
	[SerializeField] float _dodgeRate = 0.1f;
	float _timeToDodge;

	[Space (1.0f)]
	[Header ("Distortion Field")]
	[SerializeField] float _distortionRate = 0.08f;
	float _timeUntilNextDistort;

	[Space (1.0f)]
	[Header ("TorpAbility")]
	[SerializeField] float _torpFireRate = 0.08f;
	float _timeUntilNextTorp;

	Rigidbody _rb;
	Health _health;
	float _abilityX;
	float _abilityO;
	float _abilityS;

	bool _disableInput;
	[SerializeField] GameObject _engines;

	[HideInInspector] public float _apm = 0;
	float _apmCounter = 0;
	float _apmTimer;

	private void Awake ()
	{
		_rb = GetComponent<Rigidbody> ();
		_health = GetComponent<Health> ();
	}

	private void Start ()
	{
		_health.DeathEvent += OnDeath;
	}

	public void Setup (int index_, Color col_)
	{
		_playerNum = index_;
		GetComponent<MeshRenderer> ().material.color = col_;
	}

	void FixedUpdate ()
	{
		InputDevice inputDevice = (InputManager.Devices.Count > _playerNum) ? InputManager.Devices[_playerNum] : null;

		if (inputDevice == null)
		{
			// If no controller exists for this cube, just make it translucent.
			// cubeRenderer.material.color = new Color (1.0f, 1.0f, 1.0f, 0.2f);
			Debug.Log ("PANICCCCC!");
		}
		else UpdateInputDevice (inputDevice);

	}

	void UpdateInputDevice (InputDevice inputDevice_)
	{
		if (_disableInput) return;

		Vector3 newPos = new Vector2 (inputDevice_.Direction.X, inputDevice_.Direction.Y);
		_rb.velocity = newPos * (_speed + (_apm * 10.0f));
		Vector2 shootDir = new Vector2 (inputDevice_.RightStickX, inputDevice_.RightStickY).normalized;

		if (Time.time > _timeToDodge && _dodgeRate != 0)
		{
			_abilityX = 1.0f;
			if (inputDevice_.Action1.HasChanged)
			{
				_apmCounter++;
				Dodge ();
				_timeToDodge = Time.time + 1 / _dodgeRate;
				_abilityX = 0.0f;
			}
		}

		if (Time.time > _timeUntilNextDistort && _distortionRate != 0)
		{
			_abilityO = 1.0f;
			if (inputDevice_.Action2.HasChanged)
			{
				_apmCounter++;
				DistortAbility ();
				_timeUntilNextDistort = Time.time + 1 / _distortionRate;
				_abilityO = 0.0f;
			}
		}

		if (Time.time > _timeUntilNextTorp && _torpFireRate != 0)
		{
			_abilityS = 1.0f;
			if (inputDevice_.Action3.HasChanged)
			{
				_apmCounter++;
				TorpAbility (shootDir);
				_timeUntilNextTorp = Time.time + 1 / _torpFireRate;
				_abilityS = 0.0f;
			}
		}

		if (Time.time > _timeToFire && _fireRate != 0)
		{
			if (shootDir.magnitude >= 0.5f)
			{
				Shoot (shootDir);
				_timeToFire = Time.time + 1 / _fireRate;
			}
		}

		if (inputDevice_.RightBumper.HasChanged || inputDevice_.LeftBumper.HasChanged)
		{
			_apmCounter++;
			if (Time.time > _apmTimer)
			{
				_apm = (_apmCounter / 1.0f) / 100.0f;
				_apmCounter = 0;
				_apmTimer = Time.time + 1.0f;
			}
		}

		if (shootDir.magnitude <= 0.5f && newPos.normalized.magnitude >= 0.5f)
		{
			transform.RotateTowardsVector (newPos, _rotSpeed);
		}

		UpdateUI ();
	}

	void UpdateUI ()
	{
		GameManager.instance.UpdateUI (_playerNum, _health._GetHealthPercent, _abilityX, _abilityO, _abilityS, _apm);
	}

	public bool IsDead ()
	{
		return _health.IsDead ();
	}

	void OnDeath ()
	{
		PoolManager.instance.ReuseExplosionEffect (transform.position, Quaternion.identity);
		gameObject.SetActive (false);
		GameManager.instance.PlayerIsDead (_playerNum);
	}

	public void StopTryingToEscape ()
	{
		PoolManager.instance.ReuseDodgeEffect (transform.position, Quaternion.identity);
		transform.position = Vector3.zero;
	}

	void Shoot (Vector2 axis_)
	{
		PoolManager.instance.ReuseBullet (_firepoint.position, Quaternion.identity, Quaternion.Euler (0, 0, Random.Range (-_bulletSpread, _bulletSpread)) * _firepoint.right);
		if (axis_.magnitude >= 0.5f) transform.RotateTowardsVector (axis_, _rotSpeed);
		_rb.velocity += (_firepoint.right * -1) * (Random.Range (0.1f, 1.0f) * _recoilMultiplier);
	}

	void TorpAbility (Vector2 axis_)
	{
		PoolManager.instance.ReuseTorp (_firepoint.position, Quaternion.identity, _firepoint.right);
	}

	void Dodge ()
	{
		Vector3 dodgeDestination = (_rb.velocity.normalized * _dodgeDistance);
		PoolManager.instance.ReuseDodgeEffect (transform.position, Quaternion.identity);
		transform.position += dodgeDestination;
	}

	void DistortAbility ()
	{
		PoolManager.instance.ReuseDistortionEffect (transform.position, Quaternion.identity);
	}

	public void TorpPlayer (int damage_, float secondsOfInactivity_)
	{
		StartCoroutine (DisablePlayer (secondsOfInactivity_));
		_health.Damage (damage_);
	}

	IEnumerator DisablePlayer (float secondsOfInactivity_)
	{
		_engines.SetActive (false);
		_disableInput = true;
		_rb.velocity = Vector3.zero;
		yield return new WaitForSeconds (secondsOfInactivity_);
		_disableInput = false;
		_engines.SetActive (true);

	}

}