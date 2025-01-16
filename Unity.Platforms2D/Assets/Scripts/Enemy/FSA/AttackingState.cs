using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackingState : TransformTargetState
{
    [SerializeField] private float attackSpeed;

    private Vector3 sourcePoint;
    private Vector3 targetPoint;

    public override void OnEnterState(TargetTransform data)
    {
        base.OnEnterState(data);
        sourcePoint = transform.position;
        targetPoint = target.position;
    }

    public override void OnUpdateState()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, attackSpeed * Time.deltaTime);
        if(transform.position == targetPoint)
        {
            if (targetPoint != sourcePoint)
                targetPoint = sourcePoint; // Go back to source
            else
                ChangeState<ChaseState>(); // Go back to chasing
        }
    }
}
