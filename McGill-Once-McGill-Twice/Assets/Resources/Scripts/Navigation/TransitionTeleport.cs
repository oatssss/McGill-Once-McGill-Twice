using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]

public abstract class TransitionTeleport : MonoBehaviour {

    [SerializeField] private Collider TriggerCollider;
    [SerializeField] private Transform EntranceWalkTarget;
    [SerializeField] private Transform EntranceCameraPosition;
    [SerializeField] private Transform ExitSpawn;
    [SerializeField] private Transform ExitWalkTarget;
    [SerializeField] private Transform ExitCameraPosition;

    /*
	void OnTriggerEnter(Collider other)
    {
        if (other.tag == CollisionTag)
	       { DisplayTutorialMessage(); }
	}
    */

    void OnTriggerStay(Collider other)
    {
        GUIManager.Instance.ShowTooltip("Press E to enter.", 0);

        if (CustomInputManager.GetButton("Interact Main"))
        {
            Transition(other.GetComponent<ThirdPersonCharacterCustom>());
        }
    }

    /*
    void OnTriggerExit(Collider other)
    {
        if (other.tag == CollisionTag)
	       { RemoveTutorialMessage(); }
	}
    */

    public void Transition(ThirdPersonCharacterCustom player)
    {
        //  Prevent the player from spamming the interact key
        TriggerCollider.enabled = false;
        // this.OnTriggerExit(player.GetComponent<Collider>());

        //  Do any preparation the specific teleport transition needs to do
        PrepareTransition(player);

        //  Perform the transition with FinishTransition as the callback
        DoTransition(player, FinishTransition);
    }

    protected void DoTransition(ThirdPersonCharacterCustom player, Action<ThirdPersonCharacterCustom> finishTransition)
    {
        //  Prevent the camera from following the player into the teleport space and constrain the view
        //  CameraManager.LockViewPosition();
        CameraManager.SetViewPosition(this.EntranceCameraPosition.position, 3f);
        CameraManager.SetViewLookAngleMax(this.EntranceCameraPosition.forward, 45f);

        //  The AI will walk the player
        player.DisableUserControls();
        player.EnableAIControls();

        Action walkFromSpawn = () => {
            player.AIController.SetTarget(this.ExitWalkTarget, () => {

                CameraManager.SetViewToPlayer();

                player.DisableAIControls(false);
                finishTransition(player);
                player.EnableUserControls();
            });
        };

        Action teleport = () => {
            player.AIController.SetTarget(null); // This probably isn't necessary

            //  Warp player, if unsuccessful, keep trying, potential infinite loop? LOL
            while (!player.AIController.agent.Warp(this.ExitSpawn.position))
                { Debug.LogErrorFormat("{0} couldn't warp the player.", this); }

            //  Move the camera to the exit spawn
            //  CameraManager.SetViewPosition(this.ExitCameraPosition.position, CameraManager.MoveSpeedImmediate);
            CameraManager.SetViewPositionImmediate(this.ExitCameraPosition.position);
            CameraManager.SetViewForwardImmediate(this.ExitCameraPosition.forward);
            CameraManager.SetViewDirectionTowardsTarget(this.ExitCameraPosition.position + this.ExitCameraPosition.forward);

            //  Player has teleported so no need to prevent key spamming anymore
            TriggerCollider.enabled = true;

            //  Fade to clear when the camera has successfully moved to the exit
            Action<Vector3> fadeToClear = finalPosition => { FadeToClear(walkFromSpawn); };
            CameraManager.CallbackOnPositionReached(fadeToClear);
        };

        //  Get the AI to move the player to the designated teleport space, fade when finished, then teleport
        player.AIController.SetTarget(this.EntranceWalkTarget, () => { FadeToBlack(teleport); });
    }

    private void FadeToBlack(Action callback)
    {
        GUIManager.MinorFadeToBlack(callback);
    }

    private void FadeToClear(Action callback)
    {
        GUIManager.MinorFadeToClear(callback);
    }

    protected abstract void PrepareTransition(ThirdPersonCharacterCustom player);
    protected abstract void FinishTransition(ThirdPersonCharacterCustom player);
}
