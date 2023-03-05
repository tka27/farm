using UnityEngine;

namespace Game.Scripts.Common.SavableValues
{
    public class IntDataValueSavable : DataValueSavable<int>
    {
        private const string INTDataValue = "IntDataValue ";

        public IntDataValueSavable(string saveKey) : base(saveKey)
        {
        }

        protected override void Load()
        {
            base.Load();
            Value = PlayerPrefs.GetInt(INTDataValue + SaveKey, 0);
        }

        public override void Save()
        {
            base.Save();
            PlayerPrefs.SetInt(INTDataValue + SaveKey, Value);
        }

        public override bool HasSaving()
        {
            return PlayerPrefs.HasKey(INTDataValue + SaveKey);
        }
    }
}