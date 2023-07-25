using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] private GameObject _asteroidPrefab;

    [SerializeField] private float _impulse; // impulse on newly spawned asteroids
    [SerializeField] private float _spawnPeriod; // spawn new asteroids at this rate

    [SerializeField] private Camera _cam;
    private float _screenBoundsX;
    private float _spawnTimer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _screenBoundsX = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z)).x;
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer > _spawnPeriod)
        {
            Vector2 spawnPos =  new Vector2(Random.Range(-_screenBoundsX, _screenBoundsX), transform.position.y);
            GameObject asteroid = Instantiate(_asteroidPrefab, spawnPos, Quaternion.identity);
            asteroid.GetComponent<Rigidbody2D>().AddForce(-_impulse*Vector2.up, ForceMode2D.Impulse);
            _spawnTimer = 0;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}
