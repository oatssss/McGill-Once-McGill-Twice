﻿using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]

public abstract class TransitionTeleport : MonoBehaviour {
    
    private Collider TriggerCollider;
    [SerializeField] private Transform EntranceWalkTarget;
    [SerializeField] private Transform EntranceCameraTarget;
    [SerializeField] private Transform ExitSpawn;
    [SerializeField] private Transform ExitWalkTarget;
    [SerializeField] private Transform ExitCameraPosition;
    [SerializeField] private string CollisionTag;
	
    void Awake()
    {
        TriggerCollider = GetComponent<Collider>();
        TriggerCollider.isTrigger = true;
    }

	void OnTriggerEnter(Collider other)
    {
        if (other.tag == CollisionTag)
	       { DisplayTutorialMessage(); }
	}
    
    void OnTriggerStay(Collider other)
    {
        if ((CustomInputManager.GetButton("Interact Main") && (other.tag == CollisionTag) && (other.GetComponent<ThirdPersonCharacterCustom>() != null)))
        {
            Transition(other.GetComponent<ThirdPersonCharacterCustom>());
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.tag == CollisionTag)
	       { RemoveTutorialMessage(); }
	}
    
    private void DisplayTutorialMessage()
    {
        Debug.LogWarningFormat("{0} has an incomplete implementation for DisplayTutorialMessage", this);
    }
    
    private void RemoveTutorialMessage()
    {
        Debug.LogWarningFormat("{0} has an incomplete implementation for RemoveTutorialMessage", this);
    }
    
    public void Transition(ThirdPersonCharacterCustom player)
    {
        //  Prevent the player from spamming the interact key
        TriggerCollider.enabled = false;
        this.OnTriggerExit(player.GetComponent<Collider>());
        
        //  Do any preparation the specific teleport transition needs to do
        PrepareTransition(player);
        
        //  Perform the transition with FinishTransition as the callback
        DoTransition(player, FinishTransition);
    }
    
    protected void DoTransition(ThirdPersonCharacterCustom player, Action<ThirdPersonCharacterCustom> finishTransition)
    {
        //  Prevent the camera from following the player into the teleport space and constrain the view
        CameraManager.Instance.LockViewPosition();
        CameraManager.Instance.SetViewLookAngleMax(this.EntranceCameraTarget.forward, 45f);
        
        //  The AI will walk the player
        player.DisableUserControls();
        player.EnableAIControls();
        
        Action walkFromSpawn = () => {
            player.AIController.SetTarget(this.ExitWalkTarget, () => {
                
                CameraManager.Instance.SetViewToPlayer();
                
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
            CameraManager.Instance.SetViewPosition(this.ExitCameraPosition.position, 10000f);
            CameraManager.Instance.SetViewForward(this.ExitCameraPosition.forward);
            CameraManager.Instance.ForceViewDirectionTowardsTarget(this.ExitWalkTarget.position);
            
            //  Player has teleported so no need to prevent key spamming anymore
            TriggerCollider.enabled = true;
            FadeToClear(walkFromSpawn);
        };
        
        //  Get the AI to move the player to the designated teleport space, fade when finished, then teleport
        player.AIController.SetTarget(this.EntranceWalkTarget, () => { FadeToBlack(teleport); });
    }
    
    private void FadeToBlack(Action callback)
    {
        CameraManager.Instance.FadeToBlack(callback);
    }
    
    private void FadeToClear(Action callback)
    {
        CameraManager.Instance.FadeToClear(callback);
    }
    
    protected abstract void PrepareTransition(ThirdPersonCharacterCustom player);
    protected abstract void FinishTransition(ThirdPersonCharacterCustom player);
}