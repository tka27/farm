using UnityEngine;

namespace Game.Scripts.Common.SavableValues
{
    public class StringDataValueSavable : DataValueSavable<string>
    {
        private const string StringDataValue = "StringDataValue ";

        public StringDataValueSavable(string saveKey) : base(saveKey)
        {
        }

        protected override void Load()
        {
            base.Load();
            Value = PlayerPrefs.GetString(StringDataValue + SaveKey, "");
        }

        public override void Save()
        {
            base.Save();
            PlayerPrefs.SetString(StringDataValue + SaveKey, Value);
        }

        public override bool HasSaving()
        {
            return PlayerPrefs.HasKey(StringDataValue + SaveKey);
        }
    }
}