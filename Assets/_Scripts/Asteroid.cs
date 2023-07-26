using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private int _health = 3;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private int _playerDamage = 3; 
    [SerializeField] protected FlashColor flashEffect;
    [SerializeField] private float _blakHoleGrowthRate = 1.03f;
    [SerializeField] private float _scrapBonus = 1.03f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        // Despawn if below x axis
        if (transform.position.y < -1)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        // force in direction of black hole
        float force = BlackHole.Instance.CurrentForce;
        _rb.AddForce(-transform.position*force*Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public void TakeDamage(int damage)
    {

        flashEffect.Flash();
        _health -= damage;

        if (_health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 6: // ship
                collision.gameObject.GetComponent<Ship>().TakeDamage(_playerDamage);
                Die();
                break;
            case 10: // black hole
                // increase BH
                BlackHole.Instance.ChangeSize(_blakHoleGrowthRate);
                Die();
                break;
            default:
                break;
        }
        
    }

    private void Die()
    {
        GameObject effect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
        Destroy(effect, 0.3f);
        Destroy(gameObject);
    }
}
