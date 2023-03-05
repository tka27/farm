using UnityEngine;

namespace Game.Scripts.Common
{
	public class DontDestroyOnLoadGameObject : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}