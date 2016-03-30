using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : UnitySingletonPersistent<GUIManager> {

    [ReadOnly] public bool GamePaused;
    [ReadOnly] [SerializeField] private Menu currentMenu;
    public Menu CurrentMenu { get { return this.currentMenu; } private set { this.currentMenu = value; } }
    private Stack<Menu> History = new Stack<Menu>();
    private enum TRANSITION { STACK, NOSTACK, CLOSE }

    [Header("UI Elements")]
    [SerializeField] private Canvas Canvas;
    private List<KeyValuePair<Overlay,Coroutine>> FadingUIs = new List<KeyValuePair<Overlay,Coroutine>>();
    [SerializeField] private List<Overlay> CurrentUIs;
    [SerializeField] private Overlay MinigameJoinedUI;
    [SerializeField] private Overlay FreeRoamUI;
    [Space(10)]

    [Header("Fading")]
    public static readonly float FadeDuration = 1f;
    private List<KeyValuePair<Overlay,Coroutine>> FadingOverlays = new List<KeyValuePair<Overlay,Coroutine>>();
    [SerializeField] private List<Overlay> CurrentOverlays = new List<Overlay>();
    [SerializeField] private Overlay MinorFadeOverlay;
    [SerializeField] private Overlay MajorFadeOverlay;
    private List<KeyValuePair<CanvasGroup,Coroutine>> FadingCanvasGroups = new List<KeyValuePair<CanvasGroup,Coroutine>>();
    [Space(10)]

    [Header("Menus")]
    [ReadOnly] [SerializeField] private bool InGame;
    [SerializeField] private Menu PauseMenu;
    [SerializeField] private Menu StartupMenu;
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

    public static void FadeCanvasGroupIn(CanvasGroup cg, float duration, Action callback = null)
    {
        // Find out if the renderer we want to fade is already fading
        KeyValuePair<CanvasGroup,Coroutine> existingFade = Instance.FadingCanvasGroups.Find(cgCoroutinePair => cgCoroutinePair.Key == cg);

        // If it's already fading, stop the fade
        if (existingFade.Equals(default(KeyValuePair<CanvasGroup,Coroutine>)) && existingFade.Value != null)
            { Instance.StopCoroutine(existingFade.Value); }

        // Setup a new fade routine for the renderer
        Coroutine fade = null;
        KeyValuePair<CanvasGroup,Coroutine> replacementFade = new KeyValuePair<CanvasGroup,Coroutine>(cg, fade);

        Action completionAction = () => {
            Instance.FadingCanvasGroups.Remove(replacementFade);
            if (callback != null)
                { callback(); }
        };

        fade = Instance.StartCoroutine(FadeUtility.UIAlphaFade(cg, cg.alpha, 1f, duration, FadeUtility.EaseType.InOut, completionAction));

        Instance.FadingCanvasGroups.Remove(existingFade);     // Remove the previous fade
        Instance.FadingCanvasGroups.Add(replacementFade);     // Add the new fade
    }

    public static void FadeCanvasGroupIn(CanvasGroup cg, Action callback = null)
    {
        FadeCanvasGroupIn(cg, FadeDuration, callback);
    }

    public static void FadeCanvasGroupOut(CanvasGroup cg, float duration, Action callback = null)
    {
        // Find out if the renderer we want to fade is already fading
        KeyValuePair<CanvasGroup,Coroutine> existingFade = Instance.FadingCanvasGroups.Find(cgCoroutinePair => cgCoroutinePair.Key == cg);

        // If it's already fading, stop the fade
        if (existingFade.Equals(default(KeyValuePair<CanvasGroup,Coroutine>)) && existingFade.Value != null)
            { Instance.StopCoroutine(existingFade.Value); }

        // Setup a new fade routine for the renderer
        Coroutine fade = null;
        KeyValuePair<CanvasGroup,Coroutine> replacementFade = new KeyValuePair<CanvasGroup,Coroutine>(cg, fade);

        Action completionAction = () => {
            Instance.FadingCanvasGroups.Remove(replacementFade);
            if (callback != null)
                { callback(); }
        };

        fade = Instance.StartCoroutine(FadeUtility.UIAlphaFade(cg, cg.alpha, 0f, duration, FadeUtility.EaseType.InOut, completionAction));

        Instance.FadingCanvasGroups.Remove(existingFade);  // Remove the previous fade
        Instance.FadingCanvasGroups.Add(replacementFade);  // Add the new fade
    }

    public static void FadeCanvasGroupOut(CanvasGroup cg, Action callback = null)
    {
        FadeCanvasGroupOut(cg, FadeDuration, callback);
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

        FadeOverlayIn(Instance.MinorFadeOverlay, callback);
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

        FadeOverlayIn(Instance.MajorFadeOverlay, callback);
    }

    /// <summary>Cause the screen to fade to clear, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void FadeToClear(Action callback = null)
    {
        foreach (Overlay overlay in Instance.CurrentOverlays)
            { FadeOverlayOut(overlay, callback); }
    }

    public static void FadeMinorToClear(Action callback = null)
    {
        Overlay[] currentUI = Instance.CurrentUIs.ToArray();
        FadeToClearExclusive(currentUI, callback);
    }

    public static void FadeToClearExclusive(Overlay[] excludedOverlays, Action callback = null)
    {
        IEnumerable<Overlay> removing =
            from active in Instance.CurrentOverlays
            from keep in excludedOverlays
            where active != keep
            select active;

        IEnumerator<Overlay> enumerator = removing.GetEnumerator();
        Overlay last = removing.Last();
        while (enumerator.MoveNext() && enumerator.Current != last)
            { FadeOverlayOut(enumerator.Current, null); }

        // The last one will do the callback
        FadeOverlayOut(enumerator.Current, callback);
    }

    private static void FadeOverlayIn(Overlay overlay, float duration, List<KeyValuePair<Overlay,Coroutine>> existingFades, List<Overlay> currentOverlays, Action callback = null)
    {
        // Find out if the renderer we want to fade is already fading
        KeyValuePair<Overlay,Coroutine> existingFade = existingFades.Find(renderCoroutinePair => renderCoroutinePair.Key == overlay);

        // If it's already fading, stop the fade
        if (existingFade.Equals(default(KeyValuePair<Overlay,Coroutine>)) && existingFade.Value != null)
            { Instance.StopCoroutine(existingFade.Value); }

        // Setup a new fade routine for the renderer
        Coroutine fade = null;
        KeyValuePair<Overlay,Coroutine> replacementFade = new KeyValuePair<Overlay,Coroutine>(overlay, fade);

        Action completionAction = () => {
            existingFades.Remove(replacementFade);
            if (callback != null)
                { callback(); }
        };

        fade = Instance.StartCoroutine(FadeUtility.UIAlphaFade(overlay.CanvasGroup, overlay.CanvasGroup.alpha, 1f, duration, FadeUtility.EaseType.InOut, completionAction));

        existingFades.Remove(existingFade);     // Remove the previous fade
        existingFades.Add(replacementFade);     // Add the new fade
        currentOverlays.Add(overlay);           // Store the renderer in a list of renderers that are the current overlays
    }

    public static void FadeOverlayIn(Overlay overlay, float duration, Action callback = null)
    {
        FadeOverlayIn(overlay, duration, Instance.FadingOverlays, Instance.CurrentOverlays, callback);
    }

    public static void FadeOverlayIn(Overlay overlay, Action callback = null)
    {
        FadeOverlayIn(overlay, FadeDuration, callback);
    }

    private static void FadeOverlayOut(Overlay overlay, float duration, List<KeyValuePair<Overlay,Coroutine>> existingFades, List<Overlay> currentOverlays, Action callback = null)
    {
        // Find out if the renderer we want to fade is already fading
        KeyValuePair<Overlay,Coroutine> existingFade = existingFades.Find(renderCoroutinePair => renderCoroutinePair.Key == overlay);

        // If it's already fading, stop the fade
        if (existingFade.Equals(default(KeyValuePair<Overlay,Coroutine>)) && existingFade.Value != null)
            { Instance.StopCoroutine(existingFade.Value); }

        // Setup a new fade routine for the renderer
        Coroutine fade = null;
        KeyValuePair<Overlay,Coroutine> replacementFade = new KeyValuePair<Overlay,Coroutine>(overlay, fade);

        Action completionAction = () => {
            existingFades.Remove(replacementFade);
            currentOverlays.Remove(overlay);
            if (callback != null)
                { callback(); }
        };

        fade = Instance.StartCoroutine(FadeUtility.UIAlphaFade(overlay.CanvasGroup, overlay.CanvasGroup.alpha, 0f, duration, FadeUtility.EaseType.InOut, completionAction));

        existingFades.Remove(existingFade);  // Remove the previous fade
        existingFades.Add(replacementFade);  // Add the new fade
    }

    public static void FadeOverlayOut(Overlay overlay, float duration, Action callback = null)
    {
        FadeOverlayOut(overlay, duration, Instance.FadingOverlays, Instance.CurrentOverlays, callback);
    }

    public static void FadeOverlayOut(Overlay overlay, Action callback = null)
    {
        FadeOverlayOut(overlay, FadeDuration, callback);
    }

    public static void ShowMinigameJoinedUI(Minigame minigame)
    {
        HideUI();
        ShowUI(Instance.MinigameJoinedUI);
    }

    public static void ShowMinigameUI(Minigame minigame)
    {
        ShowUI(minigame.PlayingUI);
    }

    public static void ShowFreeRoamUI()
    {
        HideUI();
        ShowUI(Instance.FreeRoamUI);
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

    public static void ShowUI(Overlay ui, float duration, Action callback = null)
    {
        FadeOverlayIn(ui, duration, Instance.FadingUIs, Instance.CurrentUIs, callback);
    }

    public static void ShowUI(Overlay ui, Action callback = null)
    {
        ShowUI(ui, FadeDuration, callback);
    }

    public static void HideUI(Overlay ui, float duration, Action callback = null)
    {
        FadeOverlayOut(ui, duration, Instance.FadingUIs, Instance.CurrentUIs, callback);
    }

    public static void HideUI(Overlay ui, Action callback = null)
    {
        HideUI(ui, FadeDuration, callback);
    }

    public static void HideUI(float duration, Action callback = null)
    {
        foreach (Overlay ui in Instance.CurrentUIs)
            { HideUI(ui, duration, callback); }
    }

    public static void HideUI(Action callback = null)
    {
        HideUI(FadeDuration, callback);
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

        else if (CustomInputManager.GetButtonDown("Cancel", CustomInputManager.InputMode.Menu))
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
