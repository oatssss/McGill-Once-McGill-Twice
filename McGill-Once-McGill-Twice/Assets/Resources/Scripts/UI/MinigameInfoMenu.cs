using UnityEngine;

public abstract class MinigameInfoMenu : Menu {

    private bool Live = false;
    [SerializeField] protected Minigame Minigame;

    void Update()
    {
        if (this.Live)
        {
            this.UpdateMenu();
        }
    }

    protected abstract void UpdateMenu();

    protected override void Activate()
    {
        base.Activate();
        this.Live = true;
    }

    protected override void Deactivate()
    {
        base.Activate();
        this.Live = false;
    }
}
