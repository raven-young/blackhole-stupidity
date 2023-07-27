using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParams", menuName = "ScriptableObject/GameParams")]
public class GameParams : ScriptableObject
{

    [Header("Base Params")]
    [SerializeField, Range(10, 30),Tooltip("Escape horizon distance from origin (workaround)")] 
    public float WinRadius = 18f;

    [Header("Ship")]
    public float AngularVelocity = 1f;
    public float BurnRate = 2f; // multiplier that determines rate of fuel consumption
    public float VelocityScale = 2f; // modify ship velocity
    [Range(0, Mathf.PI/2), Tooltip("Cone spread in radians")]
    public float MaxTheta;

    [Header("Shooting")]
    public float FireRate = 1f;
    public float BulletForce = 10f;
    public int BulletDamage = 1;
    public float LaserDuration = 0.5f;

    [Header("Black Hole")]
    [Range(0f,2f), Tooltip("Growth rate when asteroids fall in")]
    public float BlackHoleGrowthRate = 1f;
    //public float BlackHoleRotationSpeed;

    [Header("Question Asteroid")]
    public float QuestionDuration; // time to answer the question
    public float QuestionDelta; // time until next question
    public int SpawnAmount; // how many things to spawn after answering question
    [Range(0, 10), Tooltip("Maximum impulse applied to spawned objects")]
    public float SpawnImpulse;
    [Range(0, 360), Tooltip("Maximum angle applied to spawned objects")]
    public float MaxSpawnAngle;

    [Header("Asteroid")]
    public int PlayerDamage = 3;
    public int AsteroidHealth = 5;
}
