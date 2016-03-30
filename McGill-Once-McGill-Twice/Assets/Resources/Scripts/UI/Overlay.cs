using UnityEngine;
using System.Collections;

public class Overlay : MonoBehaviour {

    [SerializeField] private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup { get { return this.canvasGroup; } private set { this.canvasGroup = value; } }
    [SerializeField] private RectTransform RectTransform;

	void Awake()
    {
        this.RectTransform.offsetMin = this.RectTransform.offsetMax = Vector2.zero;
    }
}
