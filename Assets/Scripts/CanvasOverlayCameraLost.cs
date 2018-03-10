using ManagerClasses;
using UnityEngine;
using UnityEngine.UI;

public class CanvasOverlayCameraLost : MonoBehaviour
{
	void Start ()
	{
		Canvas c = GetComponent<Canvas> ();
		c.worldCamera = ApplicationManager.instance.GetCamera ();
	}
}