using UnityEngine;

public class TransitionMinigame : TransitionTeleportAdapter
{
    protected override void FinishTransition(ThirdPersonCharacterCustom player)
    {
        //  Do whatever the minigame requires when you enter the space. ex. display minigame information
        Debug.LogWarningFormat("{0} has an incomplete implementation for FinishTransition", this);
    }
}