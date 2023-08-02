using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DamageNumbersPro;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameParams _gameParams;
    [SerializeField] private GameObject hitEffect;
    //[SerializeField] private float effectDuration = 0.2f;
    [SerializeField] private float piercing = 0;

    private IObjectPool<Bullet> _pool;
    [SerializeField] private DamageNumber critprefab;

    private void OnEnable()
    {
        StartCoroutine(DestroyBullet());
    }
    private void OnDisable()
    {
        StopCoroutine(DestroyBullet());
    }
    public void SetPool(IObjectPool<Bullet> pool)
    {
        _pool = pool;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Asteroid target = collider.GetComponent<Asteroid>();
        target.TakeDamage(_gameParams.BulletDamage);

        BulletExit(collider);
    }

    private void BulletExit(Collider2D collider)
    {
        //GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        //Destroy(effect, effectDuration);

        if (piercing <= 0)
        {
            _pool.Release(this);
        }
        else
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider);
        }
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(3f);
        _pool.Release(this);
    }

}