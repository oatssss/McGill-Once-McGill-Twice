using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ExistingGameItem : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public GameManager.GameState Session;
    public Text SessionName;

    [SerializeField] private UnityEvent onSelect;
    [SerializeField] private UnityEvent onDeselect;

    public void OnDeselect(BaseEventData eventData)
    {
        if (this.onDeselect != null)
            { this.onDeselect.Invoke(); }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (this.onSelect != null)
            { this.onSelect.Invoke(); }
    }
}
