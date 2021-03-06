﻿using UnityEngine;

public class MinigameZone : MonoBehaviour {

    [SerializeField] private Minigame ParentMinigame;
    [SerializeField] private Collider Zone;

    public void EnableZone()
    {
        this.Zone.enabled = true;
    }

    public void DisableZone()
    {
        this.Zone.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        GUIManager.Instance.ShowTooltip("Press E for game details.", GUIManager.TOOL_TIP_DURATION.INSTANTANEOUS);

        if (CustomInputManager.GetButtonDown("Interact Main", CustomInputManager.InputMode.Gameplay) && other.tag == GameConstants.TAG_MAINPLAYER)
        {
            // Show game info menu
            this.ParentMinigame.DisplayGameInfo();
            // Disable collider until user exits menu
            this.DisableZone();
        }
    }
}
