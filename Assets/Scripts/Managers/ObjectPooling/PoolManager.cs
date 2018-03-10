using System.Collections;
using UnityEngine;

// In the tutorial he has PoolCreator(His PoolManager) as a singleton.
// I decided to make this the PoolManager instead so that I can have multiple pools in this singleton.
// Whereas with his method you could only have one pool in the PoolCreator Singleton.
// I've added functionality where it won't Enque and reuse an active gameobject. 
// This is quite useful because I don't want enemies to randomly disappear if we use all the objects in the pool.
namespace ObjectPooling
{
	public class PoolManager : MonoBehaviour
	{
		[Header ("Hit Effect:")]
		[SerializeField] PoolObject _hitEffect;
		[SerializeField] int _hitEffectsInPoolCount;
		[SerializeField] float _hitEffectTimer;
		public static PoolCreator _HitEffectPool { get; set; }

		[Header ("Explosion Effect:")]
		[SerializeField] PoolObject _explosionEffect;
		[SerializeField] int _explosionEffectsInPoolCount;
		[SerializeField] float _explosionEffectTimer;
		public static PoolCreator _ExplosionEffectPool { get; set; }

		[Header ("Dodge Effect:")]
		[SerializeField] PoolObject _dodgeEffect;
		[SerializeField] int _dodgeEffectsInPoolCount;
		[SerializeField] float _dodgeEffectTimer;
		public static PoolCreator _DodgeEffectPool { get; set; }

		[Header ("Distortion Effect:")]
		[SerializeField] PoolObject _distortionEffect;
		[SerializeField] int _distortionEffectsInPoolCount;
		[SerializeField] float _distortionEffectTimer;
		public static PoolCreator _DistortionEffectPool { get; set; }

		[Header ("Bullet:")]
		[SerializeField] PoolObject _bullet;
		[SerializeField] int _bulletsInPoolCount;
		[SerializeField] float _bulletTimer;
		public static PoolCreator _BulletPool { get; set; }

		[Header ("Torp:")]
		[SerializeField] PoolObject _torp;
		[SerializeField] int _torpsInPoolCount;
		[SerializeField] float _torpTimer;
		public static PoolCreator _TorpPool { get; set; }

		static PoolManager _instance;
		public static PoolManager instance
		{
			get
			{
				if (!_instance)
					_instance = FindObjectOfType<PoolManager> ();
				return _instance;
			}
		}

		void Awake ()
		{
			_HitEffectPool = new PoolCreator ();
			_HitEffectPool.CreatePool (_hitEffect.gameObject, _hitEffectsInPoolCount);

			_ExplosionEffectPool = new PoolCreator ();
			_ExplosionEffectPool.CreatePool (_explosionEffect.gameObject, _explosionEffectsInPoolCount);

			_DodgeEffectPool = new PoolCreator ();
			_DodgeEffectPool.CreatePool (_dodgeEffect.gameObject, _dodgeEffectsInPoolCount);

			_DistortionEffectPool = new PoolCreator ();
			_DistortionEffectPool.CreatePool (_distortionEffect.gameObject, _distortionEffectsInPoolCount);

			_BulletPool = new PoolCreator ();
			_BulletPool.CreatePool (_bullet.gameObject, _bulletsInPoolCount);

			_TorpPool = new PoolCreator ();
			_TorpPool.CreatePool (_torp.gameObject, _torpsInPoolCount);
		}

		public void ReuseHitEffectPool (Vector3 position_, Quaternion rotation_)
		{
			GameObject g = _HitEffectPool.ReuseObject (_hitEffect.gameObject, position_, rotation_);
			if (g != null) StartCoroutine (SetInactive (g, _hitEffectTimer));
		}

		public void ReuseExplosionEffect (Vector3 position_, Quaternion rotation_)
		{
			GameObject g = _ExplosionEffectPool.ReuseObject (_explosionEffect.gameObject, position_, rotation_);
			if (g != null) StartCoroutine (SetInactive (g, _explosionEffectTimer));
		}

		public void ReuseDodgeEffect (Vector3 position_, Quaternion rotation_)
		{
			GameObject g = _DodgeEffectPool.ReuseObject (_dodgeEffect.gameObject, position_, rotation_);
			if (g != null) StartCoroutine (SetInactive (g, _dodgeEffectTimer));
		}

		public void ReuseDistortionEffect (Vector3 position_, Quaternion rotation_)
		{
			GameObject g = _DistortionEffectPool.ReuseObject (_distortionEffect.gameObject, position_, rotation_);
			if (g != null) StartCoroutine (SetInactive (g, _distortionEffectTimer));
		}

		public void ReuseBullet (Vector3 position_, Quaternion rotation_, Vector3 velocity_)
		{
			GameObject g = _BulletPool.ReuseObject (_bullet.gameObject, position_, rotation_);
			if (g != null)
			{
				Bullet b = g.GetComponent<Bullet> ();
				b.OnObjectReuse ();
				b.SetVelocity (velocity_);
				StartCoroutine (SetInactive (g, _bulletTimer));
			}
		}

		public void ReuseTorp (Vector3 position_, Quaternion rotation_, Vector3 velocity_)
		{
			GameObject g = _TorpPool.ReuseObject (_torp.gameObject, position_, rotation_);
			if (g != null)
			{
				Torp b = g.GetComponent<Torp> ();
				b.OnObjectReuse ();
				b.SetVelocity (velocity_);
				StartCoroutine (SetInactive (g, _torpTimer));
			}
		}

		IEnumerator SetInactive (GameObject g, float timeBeforeInactive_ = 0.0f)
		{
			yield return new WaitForSeconds (timeBeforeInactive_);
			g.SetActive (false);
		}

	}
}