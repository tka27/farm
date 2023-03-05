using System;
using UnityEngine;

namespace Game.Scripts.Common.Dev
{
	public class DevOption : MonoBehaviour
	{
		public event Action<DevOption> OnPropertyChanged;

		[SerializeField] private bool _needToHideDevPanel;
		public bool NeedToHideDevPanel => _needToHideDevPanel;

		private void Start()
		{
			Init();
			UpdateFields();
		}

		protected void ChangeProperty()
		{
			OnPropertyChanged?.Invoke(this);
		}

		protected virtual void Init()
		{
		}

		protected virtual void UpdateFields()
		{
		}
	}
}