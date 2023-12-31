﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryBubble : MonoBehaviour
{
    [SerializeField] private ParticleSystem movingEffect = null;
    [SerializeField] private GameObject explodeEffect = null;
    [SerializeField] private GameObject arrow = null;
    
    [SerializeField] private LayerMask whatIsWall = 8;

    [SerializeField] private float speed = 10f;
    [SerializeField] private float duration = 1f;
    [Range(0.1f, 1f)]
    [SerializeField] private float sphereRadius = 0.5f;

    private Player player = null;

    [SerializeField] private int xDir = 0;
    [SerializeField] private int yDir = 0;

    private bool hasChoosed = false;

    bool start = false;

    private void Awake() => player = GameObject.Find("[Player]").GetComponent<Player>();

    private void Update()
    {
        if (start) {
            player.transform.position = transform.position;

            if (Physics2D.OverlapCircle(transform.position, sphereRadius, whatIsWall)) {
                Explode();
            }

            if (hasChoosed) {
                if (InputManager.I.keyJump) {
                    Explode();
                }
            }

            if (hasChoosed)
                StartCoroutine(Explode(duration));

            if (InputManager.I.keyD || InputManager.I.keyA || InputManager.I.keyS || InputManager.I.keyJump) {
                if (!hasChoosed) {
                    hasChoosed = true;
                }
            }
        }
    }

    private void Move()
    {
        if (hasChoosed) {
            player.canMove = false;

            if (!InputManager.I.keyJump) {
                player.GetRigidbody().gravityScale = 0;
            }

            transform.Translate(transform.right * xDir * speed * Time.deltaTime);
            transform.Translate(transform.up * yDir * speed * Time.deltaTime);
        }
    }

    private IEnumerator Explode(float duration)
    {
        yield return new WaitForSeconds(duration);

        Explode();
    }
    
    private void Explode()
    {
        player.canMove = true;
        player.shouldLerpMovement = true;
        player.GetRigidbody().gravityScale = 2.5f;

        player.Jump(Vector2.up, 10f);

        Instantiate(explodeEffect).GetComponent<Transform>().position = transform.position;
        Destroy(arrow);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        start = true;

        if (!collision.gameObject.CompareTag("Player")) {
            Explode();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) {
            Move();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}