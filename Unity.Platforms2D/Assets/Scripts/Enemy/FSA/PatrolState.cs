using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolState : SimpleState
{
    #region Editor fields

    [SerializeField]
    private Transform patrolRoute;
    [SerializeField]
    private float patrolSpeed;
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private bool pingpong;
    #endregion

    #region Private fields

    private List<Vector3> patrolPoints;
    private int currentPoint = -1;
    private int direction = 1;

    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        patrolPoints = patrolRoute.OfType<Transform>().Select(transform => transform.position).ToList();
        RecalculateNextPoint();
    }

    public override void OnEnterState()
    {
        StartCoroutine(PatrolCorroutine());
    }

    public override void OnExitState()
    {
        StopAllCoroutines();
    }

    IEnumerator PatrolCorroutine()
    {
        while (true)
        {
            // Move to nextr point
            while (transform.position != patrolPoints[currentPoint])
            {
                transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPoint], patrolSpeed * Time.deltaTime);
                yield return null;
            }

            // Wait
            yield return new WaitForSeconds(waitTime);

            RecalculateNextPoint();
        }
    }

    private void RecalculateNextPoint()
    {
        // Find next point
        currentPoint = (currentPoint + direction) % patrolPoints.Count;

        if (pingpong)
        {
            if (currentPoint <= 0)
                direction = 1;
            else if (currentPoint + 1 >= patrolPoints.Count)
                direction = -1;
        }

        // Look at next point
        transform.eulerAngles = new Vector3(0, transform.position.x < patrolPoints[currentPoint].x ? 0 : 180, 0);
    }
}
