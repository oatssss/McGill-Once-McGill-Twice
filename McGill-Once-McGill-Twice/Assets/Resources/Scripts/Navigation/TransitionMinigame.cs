using UnityEngine;

public class TransitionMinigame : TransitionTeleportAdapter
{
    // private Minigame Minigame;

    [SerializeField] private bool TeleportsToInterior;

    void Awake()
    {
        if (!string.IsNullOrEmpty(this.TooltipText))
            { return; }

        if (this.TeleportsToInterior)
            { this.TooltipText = "Press E to enter."; }
        else
            { this.TooltipText = "Press E to exit."; }
    }

    protected override void FinishTransition(ThirdPersonCharacterCustom player)
    {
        //  Do whatever the minigame requires when you enter the space. ex. display minigame information
        Debug.LogWarningFormat("{0} has an incomplete implementation for FinishTransition", this);
    }
}