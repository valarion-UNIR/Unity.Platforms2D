using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifespan;

    void Start()
    {
        StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }
}
