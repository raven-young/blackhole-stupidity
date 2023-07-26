using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameParams",
    menuName = "ScriptableObject/GameParams")]
public class GameParams : ScriptableObject
{
    [Header("Black Hole")]
    [SerializeField] private float _initalForce;
}
