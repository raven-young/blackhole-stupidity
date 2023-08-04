using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;
    [SerializeField] private GameParams _gameParams;
    public DifficultySetting SelectedDifficulty;
    public ShipType SelectedShipType;

    public enum DifficultySetting
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
        Expert = 3
    }

    public enum ShipType
    {
        Basic = 0, // balanced
        Collector = 1, // bigger item magnet
        Destroyer = 2, // more firepower
        Tank = 3, // more HP NOT YET USED
        Scorer = 4 // more points NOT YET USED
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // GAME PARAMETERS
    public static int BulletDamage;
    public static float BurnRate;
    public static float MagnetScale;

    // Calculate game params based on difficulty and ship selected
    public void CalculateGameParams()
    {

        // BULLET DAMAGE
        BulletDamage = _gameParams.BulletDamage;
        switch (SelectedShipType)
        {
            case ShipType.Destroyer: BulletDamage += _gameParams.DestroyerBulletDamageBonus; break;
        }
        
        // BURN RATE
        BurnRate = _gameParams.BurnRate;
        if (SelectedDifficulty < DifficultySetting.Hard)
            BurnRate = 0;
        switch (SelectedShipType)
        {
            case ShipType.Basic: BurnRate /= _gameParams.BasicShipMultiplier; break;
        }

        // MAGNET SCALE
        MagnetScale = _gameParams.MagnetScale;
        switch (SelectedShipType)
        {
            case ShipType.Collector: MagnetScale *= _gameParams.CollectorMagnetScaleMultiplier; break;
            case ShipType.Basic: MagnetScale *= _gameParams.BasicShipMultiplier; break;
        }
    }
}
