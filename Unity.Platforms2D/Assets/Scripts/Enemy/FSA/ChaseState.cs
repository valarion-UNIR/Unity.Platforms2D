using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : TransformTargetState
{
    [SerializeField] private float sensorRadius;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float attackDistance;

    private void Awake()
    {
        var hitbox = this.EnsureSelfHitBox<CapsuleCollider2D>();
        var sensor = this.EnsureChildSensorBox<CircleCollider2D>(hitbox);
        if (sensor is CircleCollider2D)
            ((CircleCollider2D)sensor).radius = sensorRadius;
    }

    public override void OnUpdateState()
    {
        if (target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, chaseSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) <= attackDistance)
                ChangeState<AttackState>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSensor"))
        {
            StopAllCoroutines();
            ResetState(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSensor"))
        {
            ResetState();
            StartCoroutine(WaitForSensingAndReturnToPatrol());
        }
    }

    private IEnumerator WaitForSensingAndReturnToPatrol()
    {
        target = null;
        yield return new WaitForSeconds(2);
        StateMachine.ChangeState<PatrolState>();
    }

    private void OnDrawGizmosSelected()
    {
        var sensor = GetComponent<Collider2D>();
        Util.DrawGizmosCircle(transform.position + (Vector3)sensor.offset, Vector3.forward, sensorRadius);
        Util.DrawGizmosCircle(transform.position, Vector3.forward, attackDistance);
    }
}
