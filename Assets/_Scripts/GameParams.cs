using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParams", menuName = "ScriptableObject/GameParams")]
public class GameParams : ScriptableObject
{
    [Header("Ship")]
    public float AngularVelocity = 1f;
    public float BurnRate = 2f; // multiplier that determines rate of fuel consumption
    public float VelocityScale = 2f; // modify ship velocity

    [Header("Black Hole")]
    public float BlackHoleRotationSpeed;
}
