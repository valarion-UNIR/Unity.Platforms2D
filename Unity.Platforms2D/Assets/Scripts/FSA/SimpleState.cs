using UnityEngine;

public interface IBaseState
{
    void OnUpdateState();
    void OnExitState();
}

public interface ISimpleState : IBaseState
{
    void OnEnterState();
}

public interface IDataState<in InputData> : IBaseState
{
    void OnEnterState(InputData data);
}

[RequireComponent(typeof(StateMachine))]
public abstract class BaseState : MonoBehaviour, IBaseState
{
    public StateMachine StateMachine { get; private set; }

    private void Start()
    {
        StateMachine = GetComponent<StateMachine>();
    }

    public virtual void OnUpdateState()
    {

    }

    public virtual void OnExitState()
    {

    }
}

public abstract class SimpleState : BaseState, ISimpleState
{

    public virtual void OnEnterState()
    {

    }
}

public abstract class DataState<InputData> : BaseState, IDataState<InputData>
{
    public virtual void OnEnterState(InputData data)
    {

    }
}