﻿using UnityEngine;
using System;

public class PlayerManager : UnitySingleton<PlayerManager> {
    
    [SerializeField] private Transform Malcolm;
    [SerializeField] private Transform Andromeda;
    // public static bool PlayerIsAlive { get; private set; }
    [SerializeField] private static Player MainPlayer;
    
    /// <summary>Retrieves the Player script for the client's player.</summary>
    /// <param name="logErrors">Should an error be logged if retrieving the player is unsuccessful?</param>
    /// <returns>The Player script of the main player.</returns>
    public static Player GetMainPlayer(bool logErrors)
    {
        if (MainPlayer == null)
        {
            GameObject player = GameObject.FindWithTag(GameConstants.TAG_MAINPLAYER);
            if (player != null)
            {
                Player playerScript = player.GetComponent<Player>();
                if (playerScript != null)
                {
                    MainPlayer = playerScript;
                    return MainPlayer;
                }
            }
            
            if (logErrors)
                { Debug.LogErrorFormat("{0} could not obtain the player's script.", Instance); }
            
            // PlayerIsAlive = false;
            return null;
        }
        else
        {
            return MainPlayer;
        }
    }
    
    /// <summary>Retrieves the Player script for the client's player. Log's an error if unsuccessful.</summary>
    /// <returns>The Player script of the main player.</returns>
    public static Player GetMainPlayer()
    {
        return GetMainPlayer(true);
    }
    
    void OnEnable()
    {
        // PlayerIsAlive = true;
    }
    
    /// <summary>Respawns the player.</summary>
    public static void Respawn()
    {
        if (MainPlayer != null)
            { return; }
        
        Instantiate<Transform>(Instance.Malcolm);
        CameraManager.SetViewToPlayer();
        // PlayerIsAlive = true;
    }
    
    /// <summary>Kills the player's current character.</summary>
    public static void Die()
    {
        if (MainPlayer != null)
        {
            MainPlayer.Die();
            MainPlayer = null;
        }
    }
    
    /// <summary>Adds <paramref name="points"/> to the player's character's sleep points.</summary>
    /// <remarks>The total sleep points after adding is clamped to 100.</remarks>
    /// <param name="points">The number of points to add to the player's sleep points.</param>
    public static void AddSleepPoints(float points)
    {
        MainPlayer.SleepStatus += points;
    }
    
    /// <summary>Adds <paramref name="points"/> to the player's character's grades points.</summary>
    /// <remarks>The total grade points after adding is clamped to 100.</remarks>
    /// <param name="points">The number of points to add to the player's grades points.</param>
    public static void AddGradePoints(float points)
    {
        MainPlayer.AcademicStatus += points;
    }
    
    /// <summary>Adds <paramref name="points"/> to the player's character's social points.</summary>
    /// <remarks>The total social points after adding is clamped to 100.</remarks>
    /// <param name="points">The number of points to add to the player's social points.</param>
    public static void AddSocialPoints(float points)
    {
        MainPlayer.AcademicStatus += points;
    }
    
    /// <summary>Removes <paramref name="points"/> from the player's character's sleep points.</summary>
    /// <param name="points">The number of points to remove from the player's sleep points.</param>
    public static void RemoveSleepPoints(float points)
    {
        MainPlayer.SleepStatus -= points;
    }
    
    /// <summary>Removes <paramref name="points"/> from the player's character's grades points.</summary>
    /// <param name="points">The number of points to remove from the player's grades points.</param>
    public static void RemoveGradePoints(float points)
    {
        MainPlayer.AcademicStatus -= points;
    }
    
    /// <summary>Removes <paramref name="points"/> from the player's character's social points.</summary>
    /// <param name="points">The number of points to remove from the player's social points.</param>
    public static void RemoveSocialPoints(float points)
    {
        MainPlayer.AcademicStatus -= points;
    }
}
