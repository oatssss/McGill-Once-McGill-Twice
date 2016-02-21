using UnityEngine;

public class MenuScreen : MonoBehaviour {
    
    // Animator parameter names
    const string HideTrigger = "Hide";
    const string CloseTrigger = "Close";
    const string OpenTrigger = "Open";
    
    [SerializeField] private Animator Animator;
    private MenuScreen PreviousMenu;
    private bool StateSaved = false;
    private AnimatorStateInfo SavedState;

	private void Activate()
    {
        this.gameObject.SetActive(true);
    }
    
    private void OnEnable()
    {
        // Restore the state of the animator
        if (this.StateSaved)
            { this.Animator.Play(this.SavedState.shortNameHash, 0, 1.1f); /* 1.1f normalized time skips to the end of the state's animation */ }
    }
    
    private void Deactivate()
    {
        this.SavedState = this.Animator.GetCurrentAnimatorStateInfo(0);
        this.StateSaved = true;
        this.gameObject.SetActive(false);
    }
    
    public void Display()
    {
        this.Activate();
        this.Populate();
        // Transition the menu in
        this.Animator.SetTrigger(OpenTrigger);
    }
    
    public void Open(MenuScreen callingMenu, bool returnsToCaller)
    {
        this.PreviousMenu = returnsToCaller ? callingMenu : null;
        
        this.Display();
        
        // Transition the calling menu out if this menu was triggered by a calling menu
        if (callingMenu != null)
            { callingMenu.Animator.SetTrigger(HideTrigger); }
    }
    
    public void OpenOtherMenu(MenuScreen otherMenu, bool returnsToCaller)
    {
        otherMenu.PreviousMenu = this;
        otherMenu.Display();
    }
    
    public void Close()
    {
        // Open the menu prior to this one if set
        if (this.PreviousMenu != null)
        {
            this.PreviousMenu.Activate();
            this.PreviousMenu.Animator.SetTrigger(OpenTrigger);
        }
        
        this.Animator.SetTrigger(CloseTrigger);
    }
    
    protected virtual void Populate()
    {
        // Do nothing
        // For a static menu, population using data acquired at runtime isn't necessary
    }
}
