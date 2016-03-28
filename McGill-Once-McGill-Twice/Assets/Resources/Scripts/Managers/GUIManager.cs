using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : UnitySingletonPersistent<GUIManager> {

    public bool GamePaused;
    public Menu CurrentMenu { get; private set; }
    private Stack<Menu> History = new Stack<Menu>();
    private enum TRANSITION { STACK, NOSTACK, CLOSE }

    [Header("UI Elements")]
    [SerializeField] private Canvas Canvas;
    [SerializeField] private Overlay CurrentUI;
    [Space(10)]

    [Header("Fading")]
    public static readonly float FadeDuration = 1f;
    private List<KeyValuePair<CanvasGroup,Coroutine>> FadingRenderers = new List<KeyValuePair<CanvasGroup,Coroutine>>();
    [SerializeField] private List<CanvasGroup> CurrentOverlays = new List<CanvasGroup>();
    [SerializeField] private CanvasGroup MinorFadeOverlay;
    [SerializeField] private CanvasGroup MajorFadeOverlay;
    [Space(10)]

    [Header("Menus")]
    [ReadOnly] [SerializeField] private bool InGame;
    [SerializeField] private Menu PauseMenu;
    public Menu StartupMenu;
    [Space(10)]

    [Header("Stats")]
    [SerializeField] private GameObject Stats;
    [SerializeField] private Text SleepPoints;
    [SerializeField] private Text AcademicPoints;
    [SerializeField] private Text SocialPoints;
    [Space(10)]

    [Header("Tooltips")]
    [SerializeField] private float TooltipDuration = 5f;
    public enum TOOL_TIP_DURATION { DEFAULT, INSTANTANEOUS }
    [ReadOnly] public List<Tooltip> Tooltips = new List<Tooltip>();

    void Start()
    {
        if (!GameManager.Instance.DebugMode)
            { FadeToClear(() => this.OpenMenu(this.StartupMenu)); }
        else
            { FadeToClear(null); }
    }

    void OnGUI()
    {
        if (PlayerManager.GetMainPlayer(false) != null)
            { UpdateStatusPoints(); }
    }

    private static void UpdateStatusPoints()
    {
        string sleepPoints = float.NaN.ToString();
        string academicPoints = float.NaN.ToString();
        string socialPoints = float.NaN.ToString();

        if (PlayerManager.GetMainPlayer(false) != null)
        {
            sleepPoints = PlayerManager.GetMainPlayer().SleepStatus.ToString();
            academicPoints = PlayerManager.GetMainPlayer().AcademicStatus.ToString();
            socialPoints = PlayerManager.GetMainPlayer().SocialStatus.ToString();
        }

        Instance.SleepPoints.text = sleepPoints;
        Instance.AcademicPoints.text = academicPoints;
        Instance.SocialPoints.text = socialPoints;
    }

    /// <summary>Cause the screen to fade to black but don't cover UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void FadeMinorToBlack(Action callback = null)
    {
        // No need to fade to black if it's already black
        if (Instance.CurrentOverlays.Contains(Instance.MinorFadeOverlay))
        {
            // But if there's a callback, make sure we do the callback
            if (callback != null)
                { callback(); }
            else
                { return; }
        }

        FadeCanvasGroupIn(Instance.MinorFadeOverlay, callback);
    }

    /// <summary>Cause the screen to fade to black, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void FadeToBlack(Action callback = null)
    {
        // No need to fade to black if it's already black
        if (Instance.CurrentOverlays.Contains(Instance.MajorFadeOverlay))
        {
            // But if there's a callback, make sure we do the callback
            if (callback != null)
                { callback(); }
            else
                { return; }
        }

        FadeCanvasGroupIn(Instance.MajorFadeOverlay, callback);
    }

    /// <summary>Cause the screen to fade to clear, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void FadeToClear(Action callback = null)
    {
        foreach (CanvasGroup overlay in Instance.CurrentOverlays)
            { FadeCanvasGroupOut(overlay, callback); }
    }

    public static void FadeMinorToClear(Action callback = null)
    {
        CanvasGroup[] currentUI = new CanvasGroup[] { null };
        if (Instance.CurrentUI)
            { currentUI[0] = Instance.CurrentUI.CanvasGroup; }

        FadeToClearExclusive(currentUI, callback);
    }

    public static void FadeToClearExclusive(CanvasGroup[] excludedGroups, Action callback = null)
    {
        IEnumerable<CanvasGroup> removing =
            from active in Instance.CurrentOverlays
            from keep in excludedGroups
            where active != keep
            select active;

        IEnumerator<CanvasGroup> enumerator = removing.GetEnumerator();
        CanvasGroup last = removing.Last();
        while (enumerator.MoveNext() && enumerator.Current != last)
            { FadeCanvasGroupOut(enumerator.Current, null); }

        // The last one will do the callback
        FadeCanvasGroupOut(enumerator.Current, callback);
    }

    public static void FadeCanvasGroupIn(CanvasGroup canvasGroup, float duration, Action callback = null)
    {
        // Find out if the renderer we want to fade is already fading
        KeyValuePair<CanvasGroup,Coroutine> existingFade = Instance.FadingRenderers.Find(renderCoroutinePair => renderCoroutinePair.Key == canvasGroup);

        // If it's already fading, stop the fade
        if (existingFade.Equals(default(KeyValuePair<CanvasRenderer,Coroutine>)) && existingFade.Value != null)
            { Instance.StopCoroutine(existingFade.Value); }

        // Setup a new fade routine for the renderer
        Coroutine fade = null;
        KeyValuePair<CanvasGroup,Coroutine> replacementFade = new KeyValuePair<CanvasGroup,Coroutine>(canvasGroup, fade);

        Action completionAction = () => {
            Instance.FadingRenderers.Remove(replacementFade);
            if (callback != null)
                { callback(); }
        };

        fade = Instance.StartCoroutine(FadeUtility.UIAlphaFade(canvasGroup, canvasGroup.alpha, 1f, duration, FadeUtility.EaseType.InOut, completionAction));

        Instance.FadingRenderers.Remove(existingFade);  // Remove the previous fade
        Instance.FadingRenderers.Add(replacementFade);  // Add the new fade
        Instance.CurrentOverlays.Add(canvasGroup);      // Store the renderer in a list of renderers that are the current overlays
    }

    public static void FadeCanvasGroupIn(CanvasGroup canvasGroup, Action callback = null)
    {
        FadeCanvasGroupIn(canvasGroup, FadeDuration, callback);
    }

    public static void FadeCanvasGroupOut(CanvasGroup canvasGroup, float duration, Action callback = null)
    {
        // Find out if the renderer we want to fade is already fading
        KeyValuePair<CanvasGroup,Coroutine> existingFade = Instance.FadingRenderers.Find(renderCoroutinePair => renderCoroutinePair.Key == canvasGroup);

        // If it's already fading, stop the fade
        if (existingFade.Equals(default(KeyValuePair<CanvasRenderer,Coroutine>)) && existingFade.Value != null)
            { Instance.StopCoroutine(existingFade.Value); }

        // Setup a new fade routine for the renderer
        Coroutine fade = null;
        KeyValuePair<CanvasGroup,Coroutine> replacementFade = new KeyValuePair<CanvasGroup,Coroutine>(canvasGroup, fade);

        Action completionAction = () => {
            Instance.FadingRenderers.Remove(replacementFade);
            Instance.CurrentOverlays.Remove(canvasGroup);
            if (callback != null)
                { callback(); }
        };

        fade = Instance.StartCoroutine(FadeUtility.UIAlphaFade(canvasGroup, canvasGroup.alpha, 0f, duration, FadeUtility.EaseType.InOut, completionAction));

        Instance.FadingRenderers.Remove(existingFade);  // Remove the previous fade
        Instance.FadingRenderers.Add(replacementFade);  // Add the new fade
    }

    public static void FadeCanvasGroupOut(CanvasGroup canvasGroup, Action callback = null)
    {
        FadeCanvasGroupOut(canvasGroup, FadeDuration, callback);
    }

    public static void ShowMinigameJoinedUI(Minigame minigame)
    {
        // Make use of a currentUI

        Debug.Log("SHOW MINIGAME JOINED UI NOT IMPLEMENTED");
        // throw new NotImplementedException();
    }

    public static void ShowMinigameUI(Minigame minigame)
    {
        // Make use of a currentUI
        Debug.Log("SHOW MINIGAME UI NOT IMPLEMENTED");
        // throw new NotImplementedException();
    }

    public static void ShowFreeRoamUI()
    {
        // Make use of a currentUI
        Debug.Log("SHOW FREE ROAM UI NOT IMPLEMENTED");
        // throw new NotImplementedException();
    }

    public static void HideCurrentUI()
    {
        Debug.Log("HIDE CURRENT UI NOT IMPLEMENTED");
        // throw new NotImplementedException();
    }

    private void OpenMenu(Menu menu, TRANSITION transition)
    {
        if (this.CurrentMenu == menu)
            { return; }

        Instance.SetMenuFocus();
        menu.ResetTriggers();

        if (Instance.CurrentMenu != null)
        {
            if (transition == TRANSITION.CLOSE)
                { Instance.CurrentMenu.Close(); }
            else
            {
                if (transition == TRANSITION.STACK)
                    { Instance.History.Push(Instance.CurrentMenu); }

                Instance.CurrentMenu.Hide();
            }
        }

        Instance.CurrentMenu = menu;
        Instance.CurrentMenu.Open();
    }

    public void OpenMenu(Menu menu, bool useHistory)
    {
        if (useHistory)
            { Instance.OpenMenu(menu, TRANSITION.STACK); }
        else
            { Instance.OpenMenu(menu, TRANSITION.NOSTACK); }
    }

    public void OpenMenu(Menu menu)
    {
        Instance.OpenMenu(menu, true);
    }

    public void BackFromCurrentMenu()
    {
        if (!Instance.CurrentMenu || Instance.CurrentMenu.NonEscapable)
            { return; }

        Menu previous = (Instance.History.Count > 0) ? Instance.History.Pop() : null;

        if (previous != null)
        {
            Instance.OpenMenu(previous, TRANSITION.CLOSE);
        }
        else
        {
            Instance.ResumeGame();
        }
    }

    public void ShowTooltip(string tooltip, float duration)
    {
        Tooltip duplicate = this.Tooltips.Find(tip => tip.Text.text.Equals(tooltip));

        if (duplicate)
            { duplicate.Show(tooltip, duration);}
        else
        {
            Tooltip newTip = Instantiate<Tooltip>(this.TooltipPrefab);
            newTip.transform.SetParent(this.Canvas.transform, false);
            newTip.Show(tooltip, duration);
        }
    }

    public void ShowTooltip(string tooltip)
    {
        this.ShowTooltip(tooltip, this.TooltipDuration);
    }

    public void ShowTooltip(string tooltip, TOOL_TIP_DURATION duration)
    {
        switch (duration)
        {
            case TOOL_TIP_DURATION.DEFAULT:         this.ShowTooltip(tooltip, this.TooltipDuration); break;
            case TOOL_TIP_DURATION.INSTANTANEOUS:   this.ShowTooltip(tooltip, 0.25f); break;
        }
    }

    public void PauseGame()
    {
        Instance.GamePaused = true;
        Instance.OpenMenu(GUIManager.Instance.PauseMenu);
    }

    public void ResumeGame()
    {
        Instance.GamePaused = false; // This has to occur before SetGameFocus()
        Instance.SetGameFocus();
        Instance.CurrentMenu.Close();
        Instance.CurrentMenu = null;
        foreach (Menu menu in Instance.History)
            { menu.Reset(); }
        Instance.History.Clear();
    }

    public void CloseMenuIfOpen(Menu menu)
    {
        if (menu == this.CurrentMenu)
            { ResumeGame(); }
    }

    public Menu PreviousMenu()
    {
        return Instance.History.Peek();
    }

    private void SetMenuFocus()
    {
        PlayerManager.DisableUserMovement();
        CustomInputManager.Mode = CustomInputManager.InputMode.Menu;
    }

    private void SetGameFocus()
    {
        CustomInputManager.Mode = CustomInputManager.InputMode.Gameplay;
        // Only enable user movement if the AI isn't in control
        Player player = PlayerManager.GetMainPlayer();
        if (player && !player.ThirdPersonCharacter.AIController.enabled)
            { PlayerManager.EnableUserMovement(); }
    }

    void Update()
    {
        if (CustomInputManager.GetButtonDown("Cancel", CustomInputManager.InputMode.Gameplay))
        {
            Instance.PauseGame();
        }

        if (CustomInputManager.GetButtonDown("Cancel", CustomInputManager.InputMode.Menu))
        {
            if (Instance.CurrentMenu != null)
                { Instance.BackFromCurrentMenu(); }
        }
    }

    /*
     * UI Prefab References
     */
     [Space(10)]
     [Header("Prefabs")]
     public Tooltip TooltipPrefab;
}
