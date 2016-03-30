using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MinigameInfoMenu : Menu {

    [SerializeField] protected Minigame GenericMinigame;
    public Button StartButton;
    private bool TimedOut;
    private static float TimeoutDuration = 0.5f;
    private Coroutine TimeoutRoutine;

    void Update()
    {
        this.StartButton.interactable = !this.GenericMinigame.Started && !this.TimedOut && this.GenericMinigame.ValidToStart();
    }

    public override void Close()
    {
        base.Close();
        if (!this.GenericMinigame.LocalPlayerJoined)
        {
            this.GenericMinigame.EnableZone();
        }
    }

    public void TimeoutStartButton()
    {
        if (this.TimeoutRoutine != null)
            { StopCoroutine(this.TimeoutRoutine); }

        this.TimeoutRoutine = StartCoroutine(DoStartButtonTimeout());
    }

    IEnumerator DoStartButtonTimeout()
    {
        this.TimedOut = true;
        float elapsedTime = 0f;
        while (elapsedTime <= TimeoutDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        this.TimedOut = false;
    }
}
