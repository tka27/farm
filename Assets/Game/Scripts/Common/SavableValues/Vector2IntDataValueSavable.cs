using UnityEngine;

namespace Game.Scripts.Common.SavableValues
{
	public class Vector2IntDataValueSavable
	{
		private Vector2Int _value;
		private bool       _isLoaded;
		private const string Vector2IntXDataValue = "Vector2IntXDataValue ";
		private const string Vector2IntYDataValue = "Vector2IntYDataValue ";

		public Vector2Int Value
		{
			get
			{
				if (!_isLoaded) Load();

				return _value;
			}

			set => _value = value;
		}

		private readonly string _saveKey;

		public Vector2IntDataValueSavable(string saveKey)
		{
			_saveKey = saveKey;
		}

		private void Load()
		{
			_isLoaded = true;
			_value = new Vector2Int(PlayerPrefs.GetInt(Vector2IntXDataValue + _saveKey, 0),
															PlayerPrefs.GetInt(Vector2IntYDataValue + _saveKey, 0));
		}

		public void Save()
		{
			PlayerPrefs.SetInt(Vector2IntXDataValue + _saveKey, _value.x);
			PlayerPrefs.SetInt(Vector2IntYDataValue + _saveKey, _value.y);
		}

		public bool HasSaving()
		{
			return PlayerPrefs.HasKey(Vector2IntXDataValue + _saveKey) &&
						 PlayerPrefs.HasKey(Vector2IntYDataValue + _saveKey);
		}
	}
}