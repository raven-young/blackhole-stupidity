using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using DamageNumbersPro;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameParams _gameParams;
    //[SerializeField] private GameObject _hitEffect;
    //[SerializeField] private float _effectDuration = 0.2f;
    [SerializeField] private float _piercing = 0;

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
        target.TakeDamage(SettingsManager.BulletDamage + (Ship.Instance.IsOverdriveActive ? _gameParams.OverdriveBulletDamageBonus : 0));

        BulletExit(collider);
    }

    private void BulletExit(Collider2D collider)
    {
        //GameObject effect = Instantiate(_hitEffect, transform.position, Quaternion.identity);
        //Destroy(effect, _effectDuration);

        if (_piercing <= 0)
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