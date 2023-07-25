using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private int _health = 3;
    [SerializeField] private GameObject _explosionEffect;
    [SerializeField] private int _playerDamage = 1;
    [SerializeField] protected FlashColor flashEffect;
    [SerializeField] private float _blakHoleGrowthRate = 1.03f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        // force in direction of black hole
        float force = BlackHole.Instance.CurrentForce;
        transform.GetComponent<Rigidbody2D>().AddForce(-transform.position*force*Time.fixedDeltaTime, ForceMode2D.Force);
    }

    public void takeDamage(int damage)
    {

        flashEffect.Flash();
        _health--;

        if (_health <= 0)
        {
            Ship.Instance.Radius *= 1.1f;
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.layer)
        {
            case 6: // ship
                collision.gameObject.GetComponent<Ship>().takeDamage(_playerDamage);
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
