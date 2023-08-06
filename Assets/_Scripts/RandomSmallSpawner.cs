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
    private float _spawnPeriod;
    
    // Start is called before the first frame update
    void Start()
    {
        _screenBoundsX = _cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _cam.transform.position.z)).x;

        _spawnPeriod = _gameParams.SpawnPeriod;

        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Easy:
                _spawnPeriod /= _gameParams.EasyMultiplier;
                break;

            case SettingsManager.DifficultySetting.Hard:
                _spawnPeriod /= _gameParams.HardMultiplier;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer > _spawnPeriod)
        {
            Vector2 spawnPos =  new Vector2(0.6f*Random.Range(-_screenBoundsX, _screenBoundsX), transform.position.y);
            GameObject asteroid = Instantiate(_asteroidPrefab, spawnPos, Quaternion.identity);
            float randomAngle = Random.Range(0, 15f);
            Vector2 direction = Vector2.up.Rotate(spawnPos.x > 0 ? -randomAngle : randomAngle);
            float randomImpulse = Random.Range(0.5f, 1f);
            asteroid.GetComponent<Rigidbody2D>().AddForce(-randomImpulse*_gameParams.RandomAsteroidImpulse * direction, ForceMode2D.Impulse);
            _spawnTimer = 0;

            _spawnPeriod *= _gameParams.SpawnAcceleration;
            _spawnPeriod = Mathf.Max(_spawnPeriod, 0.05f);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position, new Vector3(1, 1, 1));
    }
}
