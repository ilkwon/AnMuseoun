using System.Collections.Generic;

public class StateMachine
{
    private IState currentState;
    private readonly Dictionary<System.Type, IState> states = new();
    public IState CurrentState => currentState;

    public void AddState(IState state)
    {
        states[state.GetType()] = state;
    }

    public void ChangeState<T>() where T : IState
    {
        var type = typeof(T);
        if (!states.ContainsKey(type)) return;

        currentState?.Exit();
        currentState = states[type];
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}