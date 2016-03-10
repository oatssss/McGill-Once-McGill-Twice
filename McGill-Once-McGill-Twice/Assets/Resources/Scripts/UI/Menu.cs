﻿using UnityEngine;

public class Menu : MonoBehaviour {

    [SerializeField] private Animator animator;
    private Animator Animator { get { return this.animator; } }
    [SerializeField] private CanvasGroup CanvasGroup;
    [SerializeField] private RectTransform RectTransform;

    public void Open()
    {
        this.Activate();
        this.Animator.SetTrigger("Open");
    }

    public void Close()
    {
        this.Deactivate();
        this.Animator.SetTrigger("Close");
    }

    public void Hide()
    {
        this.Deactivate();
        if (GUIManager.Instance.PreviousMenu() == this)
            { this.Animator.SetTrigger("Hide"); }
        else
            { this.Animator.SetTrigger("HidePermanent"); }
    }

    public void Reset()
    {
        this.Deactivate();
        this.Animator.SetTrigger("Reset");
    }

    public void ResetTriggers()
    {
        this.Animator.ResetTrigger("Open");
        this.Animator.ResetTrigger("Close");
        this.Animator.ResetTrigger("Hide");
        this.Animator.ResetTrigger("HidePermanent");
        this.Animator.ResetTrigger("Reset");
    }

    void Awake()
    {
        this.RectTransform.offsetMin = this.RectTransform.offsetMax = Vector2.zero;
    }

    protected virtual void Activate()
    {
        this.CanvasGroup.blocksRaycasts = this.CanvasGroup.interactable = true;
    }

    protected virtual void Deactivate()
    {
        this.CanvasGroup.blocksRaycasts = this.CanvasGroup.interactable = false;
    }
}
