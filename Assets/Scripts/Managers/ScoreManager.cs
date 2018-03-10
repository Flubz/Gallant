using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.UI;

namespace ManagerClasses
{
	[RequireComponent (typeof (CanvasGroup))]
	public class ScoreManager : MonoBehaviour
	{
		CanvasGroup _cg;
		bool _menuOpen;

		[HideInInspector] public int _timeSurvived;
		[HideInInspector] public int _enemyKillCount;
		[HideInInspector] public int _score;

		[Header ("Game Over UI: ")]

		[SerializeField] Text _timeSurvivedText;
		[SerializeField] Text _killCountText;
		[SerializeField] Text _scoreText;

		static ScoreManager _instance;
		public static ScoreManager instance
		{
			get
			{
				if (!_instance)
					_instance = FindObjectOfType<ScoreManager> ();
				return _instance;
			}
		}

		private void Awake ()
		{
			_cg = GetComponent<CanvasGroup> ();
		}

		private void Start ()
		{
			_cg.blocksRaycasts = false;
			_cg.alpha = 0;
		}

		public void GameOver (Player winner_, float apm_)
		{
			StartCoroutine (PlayerInput ());
			_killCountText.text = "PLAYER " + (winner_._playerNum + 1) + " HAS WON!";
			_scoreText.text = "APM " + Mathf.RoundToInt (apm_ * 1000.0f).ToString ();
			GameOverMenu (true);
		}

		public void GameOverMenu (bool openMenu_)
		{
			if (!_menuOpen && openMenu_)
			{
				_cg.blocksRaycasts = true;
				Time.timeScale = 0;
				StartCoroutine (_cg.FadeInCG (1.2f, true));
				_menuOpen = true;
			}
			else if (!openMenu_ && _menuOpen)
			{
				_cg.blocksRaycasts = false;
				Time.timeScale = 1;
				StartCoroutine (_cg.FadeOutCG (1.2f, true));
				_menuOpen = false;
			}
		}

		IEnumerator PlayerInput ()
		{
			bool keyPressed = false;
			while (!keyPressed)
			{
				if (InputManager.ActiveDevice.Action1)
				{
					keyPressed = true;
					Time.timeScale = 1;
					OnClickRestart ();
				}
				if (InputManager.ActiveDevice.Action2)
				{
					keyPressed = true;
					Time.timeScale = 1;
					OnClickMainMenu ();
				}
				if (InputManager.ActiveDevice.Action3)
				{
					keyPressed = true;
					Time.timeScale = 1;
					OnClickExit ();
				}
				yield return null;
			}
		}

		public void OnClickRestart ()
		{
			ApplicationManager.instance.ReLoadScene ();
		}

		public void OnClickMainMenu ()
		{
			ApplicationManager.instance.LoadMainMenu ();
		}

		public void OnClickExit ()
		{
			ApplicationManager.instance.LoadExit ();
		}

	}
}