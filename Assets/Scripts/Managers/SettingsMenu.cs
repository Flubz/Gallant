using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ManagerClasses
{
	// Wanted to add Post Process options, but that'd take too long.
	// This class handles all the UI within the Settings Canvas 
	// and how they relate with other gameobjects within the game.
	// Other classes can then easily check this singleton to 
	// use the proper settings.
	public class SettingsMenu : MonoBehaviour
	{
		CanvasGroup _cg;
		public bool _menuOpen;

		[SerializeField] Button _restartButton;
		[SerializeField] Button _mainMenuButton;
		[SerializeField] Button _recalibrateButton;
		[SerializeField] Slider _slider;

		static SettingsMenu _instance;
		public static SettingsMenu instance
		{
			get
			{
				if (!_instance)
					_instance = FindObjectOfType<SettingsMenu> ();
				return _instance;
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
					ToggleSettings (false);
				}
				if (InputManager.ActiveDevice.Action2)
				{
					keyPressed = true;
					Time.timeScale = 1;
					ApplicationManager.instance.ReLoadScene ();
				}
				if (InputManager.ActiveDevice.Action3)
				{
					keyPressed = true;
					Time.timeScale = 1;
					ApplicationManager.instance.LoadMainMenu ();
				}
				if (InputManager.ActiveDevice.Action4)
				{
					keyPressed = true;
					Time.timeScale = 1;
					ApplicationManager.instance.LoadExit ();
				}
				yield return null;
			}
		}

		private void Awake ()
		{
			_cg = GetComponent<CanvasGroup> ();
			SceneManager.sceneLoaded += ButtonCheck;
		}

		private void Start ()
		{
			_cg.alpha = 0;
			_cg.blocksRaycasts = false;
		}

		void ButtonCheck (Scene scene_, LoadSceneMode mode_)
		{
			if (_restartButton != null) _restartButton.interactable = scene_.name == "Level01" ? true : false;
			if (_mainMenuButton != null) _mainMenuButton.interactable = scene_.name == "MainMenu" ? false : true;
			if (_recalibrateButton != null) _recalibrateButton.interactable = scene_.name == "MainMenu" ? false : true;
		}

		public void OnVolumeSliderChanged (float sliderValue_)
		{
			AudioListener.volume = sliderValue_;
		}

		public void ToggleSettings (bool openMenu_)
		{
			if (openMenu_ && !_menuOpen)
			{
				_cg.blocksRaycasts = true;
				Time.timeScale = 0.0f;
				_menuOpen = true;
				StartCoroutine (_cg.FadeInCG (0.2f, true));
				StartCoroutine (PlayerInput ());
			}
			else if (!openMenu_ && _menuOpen)
			{
				StartCoroutine (_cg.FadeOutCG (0.2f, true));
				Time.timeScale = 1;
				_cg.blocksRaycasts = false;
				_menuOpen = false;
				StopCoroutine (PlayerInput ());
			}
		}
	}
}