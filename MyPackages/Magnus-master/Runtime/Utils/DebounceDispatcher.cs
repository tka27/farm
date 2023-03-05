using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MagnusSdk.Core.Utils
{
    public class DebounceDispatcher
    {
        private readonly int _debounceMs;
        private Action _prevDebouncedAction;
        private AsyncOperation _debounce;
        
        public DebounceDispatcher(int debounceMs)
        {
            _debounceMs = debounceMs;
        }

        public async void Dispatch(Action debouncedAction)
        {
            _prevDebouncedAction = debouncedAction;
            await Task.Delay(_debounceMs);

            if (_prevDebouncedAction != debouncedAction) return;
            _prevDebouncedAction?.Invoke();
        }
        
    }
}