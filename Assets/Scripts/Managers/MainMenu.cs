using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ManagerClasses
{
	public class MainMenu : MonoBehaviour
	{
		private void Start ()
		{
			GetComponent<Canvas> ().worldCamera = Camera.main;
			StartCoroutine (PlayerInput ());
		}

		IEnumerator PlayerInput ()
		{
			bool keyPressed = false;
			while (!keyPressed)
			{
				if (!SettingsMenu.instance._menuOpen)
				{
					if (InputManager.ActiveDevice.Action1)
					{
						keyPressed = true;
						Time.timeScale = 1;
						OnClickStart ();
					}
					if (InputManager.ActiveDevice.Action2)
					{
						keyPressed = true;
						Time.timeScale = 1;
						Settings ();
					}
					if (InputManager.ActiveDevice.Action3)
					{
						keyPressed = true;
						Time.timeScale = 1;
						OnClickExit ();
					}
				}
				yield return null;
			}
		}

		public void OnClickStart ()
		{
			ApplicationManager.instance.LoadLevel01 ();
		}

		public void OnClickExit ()
		{
			ApplicationManager.instance.LoadExit ();
		}

		public void Settings ()
		{
			SettingsMenu.instance.ToggleSettings (true);
		}
	}

}