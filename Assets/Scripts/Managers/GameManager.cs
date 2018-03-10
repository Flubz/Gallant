using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ManagerClasses
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] PlayerStats[] _playerStats;
		[SerializeField] Transform[] _spawnPoints;
		[SerializeField] List<Player> _player;
		[SerializeField] List<Color> _colors;

		List<Player> _playerList = new List<Player> ();
		int _playerCount;

		static GameManager _instance;
		public static GameManager instance
		{
			get
			{
				if (!_instance)
					_instance = FindObjectOfType<GameManager> ();
				return _instance;
			}
		}

		void Start ()
		{
			_playerList.Clear ();
			_playerCount = InputManager.Devices.Count;

			for (int i = 0; i < _playerCount; i++)
			{
				Player p = Instantiate (_player.RandomFromList (), _spawnPoints[i].position, _spawnPoints[i].rotation);
				p.Setup (i, _colors[i]);
				_playerList.Add (p);
				_playerStats[i].gameObject.SetActive (true);
			}
		}

		public void PlayerIsDead (int playerNum_)
		{
			int counter = 0;
			foreach (Player p in _playerList)
				if (!p.IsDead ()) counter++;
			if (counter == 1)
				foreach (Player p in _playerList)
					if (!p.IsDead ()) ScoreManager.instance.GameOver (p, p._apm);
		}

		public void UpdateUI (int playerNum_, float h, float x, float o, float s, float apm)
		{
			_playerStats[playerNum_].UpdateStats (h, x, o, s, apm);
		}

		public void Settings ()
		{
			SettingsMenu.instance.ToggleSettings (true);
		}
	}
}