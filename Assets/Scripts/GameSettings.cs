using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty currentDifficulty = Difficulty.Medium; // Default difficulty

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

