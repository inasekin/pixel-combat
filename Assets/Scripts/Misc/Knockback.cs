using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public bool gettingKnockedback { get; private set; }
    
    [SerializeField] private float knockBackTime = .2f;
    
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    public void GetKnockBack(Transform damageSource, float knockBackThrust)
    {
        gettingKnockedback = true;
        Vector2 difference = (transform.position - damageSource.position).normalized * knockBackThrust * rb2d.mass;
        rb2d.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);
        rb2d.velocity = Vector2.zero;
        gettingKnockedback = false;
    }
}
