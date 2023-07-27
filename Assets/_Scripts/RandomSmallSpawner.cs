using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSmallSpawner : MonoBehaviour
{

    [SerializeField] private GameParams _gameParams;
    [SerializeField] private GameObject _asteroidPrefab;

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
        if (_spawnTimer > _gameParams.SpawnPeriod)
        {
            Vector2 spawnPos =  new Vector2(0.6f*Random.Range(-_screenBoundsX, _screenBoundsX), transform.position.y);
            GameObject asteroid = Instantiate(_asteroidPrefab, spawnPos, Quaternion.identity);
            float randomAngle = Random.Range(0, 15f);
            Vector2 direction = Vector2.up.Rotate(spawnPos.x > 0 ? -randomAngle : randomAngle);
            float randomImpulse = Random.Range(0.5f, 1f);
            asteroid.GetComponent<Rigidbody2D>().AddForce(-randomImpulse*_gameParams.RandomAsteroidImpulse * direction, ForceMode2D.Impulse);
            _spawnTimer = 0;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}
