using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] GameObject proyectilePrefab;
    [SerializeField] Vector2 offset;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float throwForce;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while(true)
        {
            anim.SetTrigger("atacar");

            yield return new WaitForSeconds(timeBetweenAttacks);
        }
    }

    public void ThrowProyectile()
    {
        
        var proyectile = Instantiate(proyectilePrefab, transform.position + (Vector3)(offset * (transform.rotation.y == 0 ? Vector2.one : (Vector2.left + Vector2.up))), transform.rotation);

        if (proyectile.TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.AddForce(transform.right * throwForce, ForceMode2D.Impulse);
        }
    }
}
