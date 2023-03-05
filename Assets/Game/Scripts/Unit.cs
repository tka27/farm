using UnityEngine;

namespace Game.Scripts
{
    public abstract class Unit : MonoBehaviour
    {
        public StateMachine StateMachine;
        [field: SerializeField] public Animator Animator { get; private set; }


        protected virtual void Awake()
        {
            InitStates();
        }

        protected virtual void Update()
        {
            StateMachine.Update();
        }

        protected abstract void InitStates();
    }
}