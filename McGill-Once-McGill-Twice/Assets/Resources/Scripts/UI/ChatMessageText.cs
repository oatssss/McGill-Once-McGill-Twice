using UnityEngine;
using UnityEngine.UI;

public class ChatMessageText : MonoBehaviour {

    [HideInInspector] public ChatOverlay ParentOverlay;
    [SerializeField] private CanvasGroup CanvasGroup;
	public Text Text;
    private float Lifetime = 0f;

    public void Remove()
    {
        this.ParentOverlay.RemoveChatMessage(this);
    }

    void Update()
    {
        this.Lifetime += Time.unscaledDeltaTime;
        if (this.Lifetime > 15f)
            { StartCoroutine(FadeUtility.UIAlphaFade(this.CanvasGroup, 1f, 0f, 1f, FadeUtility.EaseType.InOut, () => this.Remove())); }
    }
}
