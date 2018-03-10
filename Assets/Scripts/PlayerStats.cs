using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
	[SerializeField] Image _healthBar;
	[SerializeField] Image _abilityX;
	[SerializeField] Image _abilityO;
	[SerializeField] Image _abilityS;
	[SerializeField] Image _abilityT;
	[SerializeField] Image _apmBar;

	public void UpdateStats (float h, float x, float o, float s, float apm)
	{
		_healthBar.fillAmount = h;
		_abilityX.fillAmount = x;
		_abilityO.fillAmount = o;
		_abilityS.fillAmount = s;
		_abilityT.fillAmount = 0.0f;
		_apmBar.fillAmount = apm;
	}

}