using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts
{
    public class Player : Unit
    {
        public static Player Instance { get; private set; }
        [field: SerializeField] public Inventory.Inventory Inventory { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }

        protected override void Awake()
        {
            Instance = this;
            base.Awake();
        }

        protected override void Update()
        {
        }

        private void FixedUpdate()
        {
            StateMachine.Update();
        }

        protected override void InitStates()
        {
            Dictionary<UnitState, IState> states = new()
            {
                [UnitState.Idle] = new PlayerIdleState(),
                [UnitState.Run] = new PlayerRunState()
            };

            StateMachine = new(states, UnitState.Idle);
        }
    }
}