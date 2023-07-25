using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private int _health = 3;

    [SerializeField] private GameObject _explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void takeDamage(int damage)
    {
        Debug.Log(_health);
        _health--;
        Debug.Log(_health);

        if (_health <= 0)
        {
            GameObject effect = Instantiate(_explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.3f);
            Destroy(gameObject);
        }
    }
}
