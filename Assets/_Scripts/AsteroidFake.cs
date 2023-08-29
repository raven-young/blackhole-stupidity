using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class AsteroidFake : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Sprite _vachette, _raccoon; // easter egg

    private void Start()
    {
        // random flip
        gameObject.GetComponent<SpriteRenderer>().flipY = UnityEngine.Random.Range(0f, 1f) < 0.5f;

        var spin = UnityEngine.Random.Range(0f, 1f) < 0.5f ? 1 : -1;
        float torque = UnityEngine.Random.Range(3f, 18f);

        if (UnityEngine.Random.Range(0f, 1f) < 0.02f)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = UnityEngine.Random.Range(0f, 1f) < 0.5f ? _vachette : _raccoon;
            if (gameObject.GetComponent<SpriteRenderer>().sprite == _raccoon) transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            torque *= 0.3f;
        }
        // random torque
        _rb.AddTorque(torque * spin, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (transform.position.y < -60 || transform.position.y > 60 || transform.position.x > 60 || transform.position.x < -60)
            Destroy(gameObject);
    }
}