using UnityEngine;

public class TargetTransform
{
    public Transform Target { get; set; }
}
public class TargetPoint
{
    public Vector3 Target { get; set; }
}

public abstract class TransformTargetState : DataState<TargetTransform>
{
    protected Transform target;

    public override void OnEnterState(TargetTransform data)
        => target = data.Target;

    public void ChangeState<T>(Transform target = null) where T : DataState<TargetTransform>
    {
        StateMachine.ChangeState<T, TargetTransform>(new TargetTransform { Target = target != null ? target : this.target });
    }

    public void ResetState(Transform target = null)
    {
        StateMachine.ChangeState(this, new TargetTransform { Target = target != null ? target : this.target });
    }

    public void ChangeState<T>(Vector3? target = null) where T : DataState<TargetPoint>
    {
        StateMachine.ChangeState<T, TargetPoint>(new TargetPoint { Target = target != null ? target.Value : this.target.position });
    }
}

public abstract class PointTargetState : DataState<TargetPoint>
{
    protected Vector3 target;

    public override void OnEnterState(TargetPoint data)
        => target = data.Target;

    public void ChangeState<T>(Vector3? target = null) where T : DataState<TargetPoint>
    {
        StateMachine.ChangeState<T, TargetPoint>(new TargetPoint { Target = target != null ? target.Value : this.target });
    }

    public void ChangeState(Vector3? target = null)
    {
        StateMachine.ChangeState(this, new TargetPoint { Target = target != null ? target.Value : this.target });
    }
}