using UnityEngine;

[CreateAssetMenu (fileName = "PlayerDetails", menuName = "Gallant/PlayerDetails", order = 0)]
public class PlayerDetails : ScriptableObject
{
	public int _playerNumber;
	public Material _playerMaterial;
}