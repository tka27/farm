using Game.Scripts;

public struct PlayerIdleState : IState
{
    public void Enter()
    {
    }

    public void Update()
    {
        if (MainJoystick.Instance.IsActive()) Player.Instance.StateMachine.SetState(UnitState.Run);
    }

    public void Exit()
    {
    }
}
