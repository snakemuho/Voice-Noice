using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]
public class PlayerData : ScriptableObject
{
    [Header("Audio stuff")]
    public float threshold = 0.001f;
    public float loudnessSensitivity = 4f, minVolume = 0.02f, maxVolume = 0.1f;
}
