﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBubble : Enemy
{
    [Header("Attack")]
    [Range(0f, 5f)]
    [SerializeField] private float explosionRadius = 2.8f;
    [SerializeField] private float explosionDamage = 10;
    [SerializeField] private float explosionDelay = 1.5f;
    [SerializeField] private float explosionKnockbackForce = 15f;

    private SpriteRenderer sr = null;

    protected override void Awake()
    {
        base.Awake();

        sr = GetComponent<SpriteRenderer>();
    }

    protected override void Update()
    {
        base.Update();

        Attack();
    }

    protected override void Attack()
    {
        if (enemyState == ENEMY_STATE.ATTACKING) {
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        sr.color = Color.yellow;

        yield return new WaitForSeconds(explosionDelay);

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, explosionRadius, whatIsPlayer);

        if (hitPlayer) {
            Player player = hitPlayer.GetComponent<Player>();
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            Vector2 dir = transform.position - player.transform.position;

            player.TakeDamage(explosionDamage);
            playerRb.MovePosition(playerRb.position + dir * -explosionKnockbackForce * Time.fixedDeltaTime);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
