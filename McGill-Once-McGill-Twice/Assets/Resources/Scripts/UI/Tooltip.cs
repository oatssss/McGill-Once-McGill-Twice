using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    private static float FadeDuration = 0.5f;
    public Text Text;
    [SerializeField] private CanvasRenderer Renderer;
    [ReadOnly] [SerializeField] private float Duration;
    private Coroutine Fading;

    public void Show(string tooltip, float duration)
    {
        if (!GUIManager.Instance.Tooltips.Contains(this))
            { GUIManager.Instance.Tooltips.Add(this); }

        this.Duration = duration;
        this.Text.text = tooltip;

        if (this.Fading != null)
        {
            StopCoroutine(this.Fading);
            this.Fading = null;
            this.Renderer.SetAlpha(1f);
        }
    }

    private void Remove()
    {
        GUIManager.Instance.Tooltips.Remove(this);
        Destroy(gameObject);
    }

    void Update()
    {
        this.Duration -= Time.deltaTime;

        if (this.Duration <= 0 && this.Fading == null)
        {
            this.Fading = StartCoroutine(FadeUtility.UIAlphaFade(this.Renderer, 1f, 0f, FadeDuration, FadeUtility.EaseType.InOut, this.Remove));
        }
    }
}
