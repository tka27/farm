using UnityEngine;

namespace Game.Scripts
{
    public struct PlayerRunState : IState
    {
        private static readonly int Run = Animator.StringToHash("Run");

        public void Enter()
        {
            Player.Instance.Animator.SetBool(Run, true);
        }

        public void Update()
        {
            const float defaultMoveSpeed = 5;
            Vector3 moveDirection =
                defaultMoveSpeed * MainJoystick.Instance.Direction;
            moveDirection.y = Player.Instance.Rigidbody.velocity.y;
            Player.Instance.Rigidbody.velocity = moveDirection;

            Vector3 velocity = new Vector3(Player.Instance.Rigidbody.velocity.x, 0,
                Player.Instance.Rigidbody.velocity.z);
            if (velocity.magnitude > 0)
            {
                Player.Instance.transform.rotation = Quaternion.Lerp(Player.Instance.transform.rotation,
                    Quaternion.LookRotation(velocity.normalized), Time.deltaTime * 20);
            }

            if (!MainJoystick.Instance.IsActive()) Player.Instance.StateMachine.SetState(UnitState.Idle);
        }

        public void Exit()
        {
            Player.Instance.Animator.SetBool(Run, false);
        }
    }
}