﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform attackPoint = null;

    [SerializeField] private GameObject projectile = null;
    [SerializeField] private GameObject smokeEffect = null;

    [SerializeField] private ParticleSystem canUseShurikenEffect = null;

    [SerializeField] private LayerMask whatIsEnv = 0;

    [Header("Config")]
    [Tooltip("Defines how far the melee attack point will get from player")]
    [Range(0.1f, 6f)]
    public float attackPointRotRadius = 1f;

    [Header("Shuriken")]
    [SerializeField] private int shurikenDamage = 1;
    [SerializeField] private int shurikenHealth = 3;
    [SerializeField] private float duration = 5f;
    [SerializeField] private float cooldown = .5f;
    [SerializeField] private float shurikenSpeed = 10f;
    [SerializeField] private float shurikenReflectForce = 20f;

    [SerializeField] private int maxAmountOfShurikens = 3;

    private Player player = null;
    private Shuriken shuriken = null;

    private float angle = 0f;

    private int shurikensUsed = 0;

    private bool isAShuriken = false;
    private bool canShoot = true;
    private bool isOnShurikenAreas = true;

    private void Awake() => player = GetComponent<Player>();

    private void Update()
    {
        angle = StaticRes.LookDir(transform.position);

        if (isOnShurikenAreas && shurikensUsed < maxAmountOfShurikens) {
            if (!canUseShurikenEffect.isPlaying) {
                canUseShurikenEffect.Play();
            }
        }
        else {
            if (canUseShurikenEffect.isPlaying) {
                canUseShurikenEffect.Clear();
                canUseShurikenEffect.Pause();
            }
        }

        if (isAShuriken && shuriken.life <= 0) {
            StopCoroutine(EnableIsAShuriken(0f));
            StopCoroutine(shuriken.DieOnTimer(0f));

            Instantiate(smokeEffect).GetComponent<Transform>().position = transform.position;

            isAShuriken = false;
            shuriken.isBreaked = true; 
        }

        if (InputManager.I.btnThrowShuriken && !isAShuriken && shurikensUsed < maxAmountOfShurikens && canShoot && isOnShurikenAreas) {
            Shoot();

            shurikensUsed++;

            StartCoroutine(Cooldown(cooldown));
            player.TakeDamage(shurikenDamage);
        }
        
        Aim();

        player.isAShuriken = isAShuriken;

        if (isAShuriken) {
            transform.position = shuriken.transform.position;
            player.GetRigidbody().gravityScale = 0f;
        }
        else {
            player.GetRigidbody().gravityScale = 2.5f;
            player.shouldLerpMovement = true;
        }
    }

    private void Aim()
    {
        Vector3 v3Pos = Quaternion.AngleAxis(angle, Vector3.forward) * ( Vector3.right * attackPointRotRadius );

        attackPoint.position = transform.position + v3Pos;
    }

    private void Shoot()
    {
        shuriken = Instantiate(projectile, attackPoint.position, Quaternion.Euler(0f, 0f, angle)).GetComponent<Shuriken>();

        shuriken.whatIsEnv = whatIsEnv;
        shuriken.life = shurikenHealth;
        shuriken.speed = shurikenSpeed;
        shuriken.reflectForce = shurikenReflectForce;

        StartCoroutine(EnableIsAShuriken(duration));
        //StartCoroutine(shuriken.DieOnTimer(duration));

        Instantiate(smokeEffect).GetComponent<Transform>().position = transform.position;
    }

    private IEnumerator EnableIsAShuriken(float time)
    {
        isAShuriken = true;

        yield return new WaitForSeconds(time);

        isAShuriken = false;
        Instantiate(smokeEffect).GetComponent<Transform>().position = transform.position;
    }
    
    private IEnumerator Cooldown(float time)
    {
        canShoot = false;

        yield return new WaitForSeconds(time);

        canShoot = true;
    }

    public void KillShuriken()
    {
        StopCoroutine(EnableIsAShuriken(0f));
        StopCoroutine(shuriken.DieOnTimer(0f));

        Instantiate(smokeEffect).GetComponent<Transform>().position = transform.position;

        isAShuriken = false;
        shuriken.isBreaked = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11) {
            isOnShurikenAreas = false;
        }
        else if (collision.gameObject.layer == 12) {
            isOnShurikenAreas = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 v3Pos = Quaternion.AngleAxis(angle, Vector3.forward) * ( Vector3.right * attackPointRotRadius );

        Gizmos.DrawWireSphere(transform.position + v3Pos, 0.4f);
    }
}
