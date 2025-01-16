using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackState : TransformTargetState
{
    [SerializeField] private float timeBetweenAttacks;

    private float timer;

    public override void OnEnterState(TargetTransform data)
    {
        base.OnEnterState(data);

        timer = 0;
    }

    public override void OnUpdateState()
    {
        timer += Time.deltaTime;
        if (timer > timeBetweenAttacks)
        {
            timer = 0;
            ChangeState<AttackingState>();
        }

    }
}
