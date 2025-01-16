using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private SerializableType<SimpleState> componentType;

    public IReadOnlyDictionary<Type, BaseState> States { get; private set; }
    public BaseState CurrentState { get; private set; }

    /// <summary>
    /// Find states
    /// </summary>
    private void Start()
    {
        States = new ReadOnlyDictionary<Type, BaseState>(GetComponents<BaseState>().ToDictionary(c => c.GetType(), c => c));
        if (States.ContainsKey(componentType.type) && States[componentType.type] is SimpleState)
            ChangeState((SimpleState)States[componentType.type]);
    }

    /// <summary>
    /// Update current state
    /// </summary>
    private void Update()
    {
        if(CurrentState != null)
            CurrentState.OnUpdateState();
    }

    /// <summary>
    /// Exits from previous state, and assigns and enters to the new state of the type specified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="forceChange">If true, will assign null state if a state of the type doesn't exist</param>
    public void ChangeState<T>(bool forceChange = false) where T : SimpleState
    {
        if (States.TryGetValue(typeof(T), out var newState) || forceChange)
            ChangeState((T)newState);
    }

    /// <summary>
    /// Exits from previous state, and assigns and enters to the new state of the type specified.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="forceChange">If true, will assign null state if a state of the type doesn't exist</param>
    public void ChangeState<T, D>(D data, bool forceChange = false) where T : DataState<D>
    {
        if (States.TryGetValue(typeof(T), out var newState) || forceChange)
            ChangeState((T)newState, data);
    }

    /// <summary>
    /// Exits from previous state, and assigns and enters to the new state in the parameter.
    /// </summary>
    /// <param name="newState">New state</param>
    public void ChangeState(SimpleState newState)
    {
        // Exit previous state
        if (CurrentState != null)
            CurrentState.OnExitState();

        // Assign state
        CurrentState = newState;

        // Enter new state
        if (newState != null)
            newState.OnEnterState();
    }

    /// <summary>
    /// Exits from previous state, and assigns and enters to the new state in the parameter.
    /// </summary>
    /// <param name="newState">New state</param>
    public void ChangeState<D>(DataState<D> newState, D data)
    {
        // Exit previous state
        if (CurrentState != null)
            CurrentState.OnExitState();

        // Assign state
        CurrentState = newState;

        // Enter new state
        if (newState != null)
        {
            newState.OnEnterState(data);
            newState.OnEnterState(data);
        }
    }
}