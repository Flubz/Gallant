using System.Collections.Generic;
using ManagerClasses;
using UnityEngine;

namespace ObjectPooling
{
	public class PoolParticle : PoolObject
	{
		ParticleSystem _particleSystem;

		[SerializeField] List<AudioClip> _soundsExplosion;

		void Awake ()
		{
			_particleSystem = GetComponent<ParticleSystem> ();
		}

		public override void OnObjectReuse ()
		{
			AudioManager.instance.Play (_soundsExplosion.RandomFromList ().name);
			_particleSystem.Play ();
		}

	}
}