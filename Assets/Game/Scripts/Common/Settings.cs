using UnityEngine;

namespace Game.Scripts.Common
{
	public static class Settings
	{
		private const string Vibration = "Vibration";

		public static bool VibrationEnabled
		{
			get => PlayerPrefs.GetInt(Vibration, 1) == 1;
			set => PlayerPrefs.SetInt(Vibration, value ? 1 : 0);
		}
	}
}