using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParams", menuName = "ScriptableObject/GameParams")]
public class GameParams : ScriptableObject
{
    [Header("Base Params")]
    [Range(10, 30),Tooltip("Escape horizon distance from origin (workaround)")] 
    public float WinRadius = 18f;
    [Range(0,20f), Tooltip("Critical distance to event horizon (change music etc.)")]
    public float DangerZoneDistance = 4f;
    [Range(0, Mathf.PI / 2), Tooltip("Cone spread in radians")]
    public float MaxTheta;

    [Header("Ship")]
    public int MaxHealth = 100;
    public float AngularVelocity = 1f;
    [Tooltip("Multiply ship's radial velocity by this value.")]
    public float RadialVelocityScale = 0.06f;
    [Tooltip("Rate of fuel consumption. Higher means fuel is consumed more quickly")]
    public float FuelBurnRate = 2f;
    [Tooltip("Determines the item magnet size")]
    public float MagnetScale = 15f;

    [Header("Shooting")]
    public float FirePeriod = 1f;
    public float BulletVelocity = 10f;
    public int BulletDamage = 1;
    public float LaserDuration = 0.5f;

    [Header("Black Hole")]
    [Range(0f,2f), Tooltip("Growth rate when asteroids fall in")]
    public float BlackHoleGrowthRate = 1f;
    //public float BlackHoleRotationSpeed;

    [Header("Question Asteroid")]
    //public float QuestionDuration; // time to answer the question
    public float QuestionDelta; // time until next question
    public int SpawnAmount; // how many things to spawn after answering question
    [Range(-100, 100), Tooltip("Maximum impulse applied to spawned objects; positive values indicate upward force")]
    public float SpawnImpulse;
    [Range(0, 360), Tooltip("Maximum angle applied to force vector of spawned objects")]
    public float MaxSpawnAngle;
    public float QuestionAsteroidSpeed = 1f;
    [Tooltip("Every cycle, multiply speed by this value"), Range(1f, 1.07f)]
    public float QuestionAsteroidAcceleration = 1f;
    [SerializeField, Tooltip("Get bonus score if solved before reaching this radius")]
    private float _quickSolveRadius;
    public float QuickSolveRadius { get => _quickSolveRadius; set => _quickSolveRadius = Mathf.Clamp(value, WinRadius, 30); }

    [Header("Asteroid")]
    [Range(0f,2f),Tooltip("Increase drag to slow down asteroid")]
    public float AsteroidLinearDrag = 0f;
    public int PlayerDamage = 3;
    public int AsteroidHealth = 5;

    [Header("Items")]
    public int ScrapValue;
    public int FuelValue;
    public int CrystalValue;
    [Range(0f,1f), Tooltip("Probability of spawning crystal asteroid instead of regular one")]
    public float CrystalProbability;

    [Header("Random Astroid Spawner")]
    public float SpawnPeriod = 3f;
    public float RandomAsteroidImpulse = 10f;
    [Tooltip("Every cycle, multiply period by this value"),Range(0.98f,1f)]
    public float SpawnAcceleration = 0.995f;

    [Header("Scoring")]
    public int CorrectAnswerScore = 1;
    public int ShotAsteroidScore = 1;
    public int CollectedItemScore = 1;
    public float EasyScoreMultiplier = 0.75f;
    public float NormalScoreMultiplier = 1f;
    public float HardScoreMultiplier = 1.5f;
    public float VictoryMultiplier = 1.5f;
    public float GameOverMultiplier = 0.75f;

    [Header("Juice")]
    public float ScreenShakeDuration = 0.2f;

    //[Header("Overdrive")]
    //public float OverdriveDuration = 10f;
    //public float OverdriveScoreMultiplier = 2f;
    //public int OverdriveBulletDamageBonus = 1;
    //public float OverdriveMagnetScale = 1.3f;

    [Header("Difficulty Settings")]
    [Tooltip("Multiply or divide certain parameters by this value to adjust difficulty:\n" +
        "Small asteroid spawn period")]

    public float EasyMultiplier = 0.7f;
    public float HardMultiplier = 1.2f;

    public int HardPlayerDamageBonus = 2;
    public int FailAsteroidSpawnBonus = 2;

    [Header("Don't change in editor")]
    //public int HighScore = 0;
    public Vector2 ScreenBounds;

    // Needed for property validation in inspector (e.g. clamping)
    private void OnValidate()
    {
        QuickSolveRadius = _quickSolveRadius;
    }
}
