using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] private GameParams _gameParams;

    private float _initalForce;
    public float CurrentForce;

    public static BlackHole Instance;

    [SerializeField] private GameObject _eventHorizonGlow;
    private Material _eventHorizonGlowMaterial;
    [SerializeField] private float _initialGlow = 15f;

    [SerializeField] private GameObject _blackHoleHardLayer;

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

        _eventHorizonGlowMaterial = _eventHorizonGlow.GetComponent<Renderer>().material;
        _eventHorizonGlowMaterial.SetFloat("_Glow", _initialGlow);

        switch (SettingsManager.Instance.SelectedDifficulty)
        {
            case SettingsManager.DifficultySetting.Hard:
                _blackHoleHardLayer.SetActive(true);
                break;
            default:
                _blackHoleHardLayer.SetActive(false);
                break;
        }

    }

    public void GrowBlackHole(float scaleMultiplier)
    {
        transform.localScale *= scaleMultiplier;
        CurrentForce *= scaleMultiplier;

        float glow = _eventHorizonGlowMaterial.GetFloat("_Glow");
        _eventHorizonGlowMaterial.SetFloat("_Glow", 1.02f * glow * scaleMultiplier);
    }
}
