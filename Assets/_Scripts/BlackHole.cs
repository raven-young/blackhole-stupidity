using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private GameParams _gameParams;

    private float _initalForce;
    public float CurrentForce;

    public static BlackHole Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;

        
    }
    // Start is called before the first frame update
    void Start()
    {
        // At the start of the game, BH force and ship fuel are balanced
        _initalForce = Ship.Instance.CurrentHealth;
        CurrentForce = _initalForce;
    }

    public void ChangeSize(float scaleMultiplier)
    {
        transform.localScale *= scaleMultiplier;
        CurrentForce *= scaleMultiplier;
    }
}
