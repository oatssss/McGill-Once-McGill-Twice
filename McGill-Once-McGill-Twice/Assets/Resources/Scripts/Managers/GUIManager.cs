using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GUIManager : UnitySingleton<GUIManager> {
    
    public static readonly float FadeDuration = 0.5f;
    
    private Coroutine Fading = null;
    [SerializeField] public Canvas Canvas;
    [SerializeField] private CanvasRenderer MinorFadeRenderer;
    [SerializeField] private CanvasRenderer MajorFadeRenderer;
    [SerializeField] private Text SleepPoints;
    [SerializeField] private Text AcademicPoints;
    [SerializeField] private Text SocialPoints;

	// Use this for initialization
	void OnEnable ()
    {
	   MajorFadeToClear(null);
	}
	
	// Update is called once per frame
	void Update ()
    {
	   	
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

        Instance.Fading = Instance.StartCoroutine(DoFadeToBlack(Instance.MinorFadeRenderer, callback));
    }

    /// <summary>Cause the screen to fade to clear but don't cover UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void MinorFadeToClear(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        Instance.Fading = Instance.StartCoroutine(DoFadeToClear(Instance.MinorFadeRenderer, callback));
    }
    
    /// <summary>Cause the screen to fade to black, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void MajorFadeToBlack(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }
        
        //  Clear the minor fade overlay in case it's opaque
        Instance.MinorFadeRenderer.SetAlpha(0f);

        Instance.Fading = Instance.StartCoroutine(DoFadeToBlack(Instance.MajorFadeRenderer, callback));
    }

    /// <summary>Cause the screen to fade to clear, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public static void MajorFadeToClear(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }
            
        //  Clear the minor fade overlay in case it's opaque
        Instance.MinorFadeRenderer.SetAlpha(0f);

        Instance.Fading = Instance.StartCoroutine(DoFadeToClear(Instance.MajorFadeRenderer, callback));
    }

    private static IEnumerator DoFadeToBlack(CanvasRenderer renderer, Action callback)
    {
        yield return Instance.StartCoroutine(FadeUtility.UIAlphaFade(renderer, 0f, 1f, FadeDuration, FadeUtility.EaseType.InOut));

        if (callback != null)
            { callback(); }
    }

    private static IEnumerator DoFadeToClear(CanvasRenderer renderer, Action callback)
    {
        yield return Instance.StartCoroutine(FadeUtility.UIAlphaFade(renderer, 1f, 0f, FadeDuration, FadeUtility.EaseType.InOut));

        if (callback != null)
            { callback(); }
    }
}
