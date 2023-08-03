using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParams", menuName = "ScriptableObject/GameParams")]
public class GameParams : ScriptableObject
{

    [Header("Base Params")]
    [SerializeField, Range(10, 30),Tooltip("Escape horizon distance from origin (workaround)")] 
    public float WinRadius = 18f;
    [Range(0,20f), Tooltip("Critical distance to event horizon (change music etc.)")]
    public float DangerZoneDistance = 4f;
    [Range(0, Mathf.PI / 2), Tooltip("Cone spread in radians")]
    public float MaxTheta;

    [Header("Ship")]
    public int MaxHealth = 100;
    public float AngularVelocity = 1f;
    [Tooltip("Rate of fuel consumption. Higher means fuel is consumed more quickly")]
    public float BurnRate = 2f;
    [Tooltip("Multiply ship's radial velocity by this value.")]
    public float RadialVelocityScale = 0.06f;


    [Header("Shooting")]
    public float FireRate = 1f;
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
    [Range(0, 100), Tooltip("Maximum impulse applied to spawned objects")]
    public float SpawnImpulse;
    [Range(0, 360), Tooltip("Maximum angle applied to spawned objects")]
    public float MaxSpawnAngle;
    public float QuestionAsteroidSpeed = 1f;

    [Header("Asteroid")]
    public int PlayerDamage = 3;
    public int AsteroidHealth = 5;

    [Header("Items")]
    public int ScrapValue;
    public int FuelValue;

    [Header("Random Astroid Spawner")]
    public float SpawnPeriod = 3f;
    public float RandomAsteroidImpulse = 10f;

    [Header("Scoring")]
    public int CorrectAnswerScore = 1;
    public int ShotAsteroidScore = 1;
    public int CollectedItemScore = 1;

    [Header("Juice")]
    public float ScreenShakeDuration = 0.2f;

    [Header("Difficulty Settings")]
    [Tooltip("Multiply or divide certain parameters by this value to make it harder:\n" +
        "Small asteroid spawn period")]

    public float EasyMultiplier = 0.7f;

    public float HardMultiplier = 1.2f;
    public int HardPlayerDamageBonus = 2;
    public int FailAsteroidSpawnBonus = 2;

    [Header("Don't change in editor")]
    public int HighScore = 0;
    public Vector2 ScreenBounds;
}
