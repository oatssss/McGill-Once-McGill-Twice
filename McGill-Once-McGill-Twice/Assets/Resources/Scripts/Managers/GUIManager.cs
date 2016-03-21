using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : UnitySingletonPersistent<GUIManager> {

    public static readonly float FadeDuration = 1f;

    private bool GamePaused = false;
    private Menu CurrentMenu;
    private Stack<Menu> History = new Stack<Menu>();
    private enum TRANSITION { STACK, NOSTACK, CLOSE }

    private Coroutine Fading = null;

    [Header("UI Elements")]
    [SerializeField] public Canvas Canvas;
    [SerializeField] private CanvasRenderer MinorFadeRenderer;
    [SerializeField] private CanvasRenderer MajorFadeRenderer;
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
    [ReadOnly] public List<Tooltip> Tooltips = new List<Tooltip>();

    void Start()
    {
        MajorFadeToClear(() => this.OpenMenu(this.StartupMenu));
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
    public static void MinorFadeToBlack(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        Instance.Fading = Instance.StartCoroutine(FadeUtility.UIAlphaFade(Instance.MinorFadeRenderer, Instance.MinorFadeRenderer.GetAlpha(), 1f, FadeDuration, FadeUtility.EaseType.InOut, () => {
                Instance.Fading = null;
                if (callback != null)
                    { callback(); }
            }));
    }

    /// <summary>Cause the screen to fade to clear but don't cover UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void MinorFadeToClear(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        Instance.Fading = Instance.StartCoroutine(FadeUtility.UIAlphaFade(Instance.MinorFadeRenderer, Instance.MinorFadeRenderer.GetAlpha(), 0f, FadeDuration, FadeUtility.EaseType.InOut, () => {
                Instance.Fading = null;
                if (callback != null)
                    { callback(); }
            }));
    }

    /// <summary>Cause the screen to fade to black, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void MajorFadeToBlack(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        //  Clear the minor fade overlay in case it's opaque
        Instance.MinorFadeRenderer.SetAlpha(0f);

        Instance.Fading = Instance.StartCoroutine(FadeUtility.UIAlphaFade(Instance.MajorFadeRenderer, Instance.MajorFadeRenderer.GetAlpha(), 1f, FadeDuration, FadeUtility.EaseType.InOut, () => {
                Instance.Fading = null;
                if (callback != null)
                    { callback(); }
            }));
    }

    /// <summary>Cause the screen to fade to clear, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void MajorFadeToClear(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        //  Clear the minor fade overlay in case it's opaque
        Instance.MinorFadeRenderer.SetAlpha(0f);

        Instance.Fading = Instance.StartCoroutine(FadeUtility.UIAlphaFade(Instance.MajorFadeRenderer, Instance.MajorFadeRenderer.GetAlpha(), 0f, FadeDuration, FadeUtility.EaseType.InOut, () => {
                Instance.Fading = null;
                if (callback != null)
                    { callback(); }
            }));
    }

    public static void ShowMinigameJoinedUI(Minigame minigame)
    {
        // Make use of a currentUI

        throw new NotImplementedException();
    }

    public static void ShowMinigameUI(Minigame minigame)
    {
        // Make use of a currentUI

        throw new NotImplementedException();
    }

    public static void ShowFreeRoamUI()
    {
        // Make use of a currentUI

        throw new NotImplementedException();
    }

    public static void HideCurrentUI()
    {
        throw new NotImplementedException();
    }

    private void OpenMenu(Menu menu, TRANSITION transition)
    {
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
        if (Instance.CurrentMenu.NonEscapable)
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
            { duplicate.Show(tooltip, duration); }
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

    public void PauseGame()
    {
        Instance.GamePaused = true;
        Instance.SetMenuFocus();
        Instance.OpenMenu(GUIManager.Instance.PauseMenu);
    }

    public void ResumeGame()
    {
        Instance.SetGameFocus();
        Instance.CurrentMenu.Close();
        Instance.CurrentMenu = null;
        foreach (Menu menu in Instance.History)
            { menu.Reset(); }
        Instance.History.Clear();
        Instance.GamePaused = false;
    }

    public Menu PreviousMenu()
    {
        return Instance.History.Peek();
    }

    private void SetMenuFocus()
    {
        // TODO
    }

    private void SetGameFocus()
    {
        // TODO
    }

    void Update()
    {
        if (CustomInputManager.GetButtonDown("Cancel"))
        {
            if (Instance.CurrentMenu != null)
                { Instance.BackFromCurrentMenu(); }
            else
                { Instance.PauseGame(); }
        }
    }

    /*
     * UI Prefab References
     */
     [Space(10)]
     [Header("Prefabs")]
     public PlayerListItem PlayerListItemPrefab;
     public RoomListItem RoomListItemPrefab;
     public Tooltip TooltipPrefab;
}
