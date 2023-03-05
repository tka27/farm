using UnityEngine;

namespace Game.Scripts.Common.SavableValues
{
    public class BoolDataValueSavable : DataValueSavable<bool>
    {
        private const string BoolDataValue = "BoolDataValue ";

        public BoolDataValueSavable(string saveKey) : base(saveKey)
        {
        }

        protected override void Load()
        {
            base.Load();
            Value = PlayerPrefs.GetInt(BoolDataValue + SaveKey, 0) == 1;
        }

        public override void Save()
        {
            base.Save();
            PlayerPrefs.SetInt(BoolDataValue + SaveKey, Value ? 1 : 0);
        }

        public override bool HasSaving()
        {
            return PlayerPrefs.HasKey(BoolDataValue + SaveKey);
        }
    }
}