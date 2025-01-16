using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayerOnTouch : MonoBehaviour
{
    [SerializeField] private bool destroyOnTouch;
    [SerializeField] private float damage;

    private void Awake()
    {
        this.EnsureSelfHitBox<CircleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerHitbox") && collision.TryGetComponent<HealthSystem>(out var healthSystem))
        {
            healthSystem.ApplyDamage(damage);
            if(destroyOnTouch)
                Destroy(gameObject);
        }
    }
}
