using System;
using UnityEngine;

[Serializable]
public class PlayerGameData
{
    public Vector3 playerPosition;

    public PlayerGameData()
    {
        playerPosition = Vector3.zero;
    }    
}
