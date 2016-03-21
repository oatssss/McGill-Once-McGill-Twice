using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class HostGameButton : MonoBehaviour, IPointerClickHandler {

    [System.Serializable] public class HostEvent : UnityEvent<SavedGameItem, Toggle> { }
    [SerializeField] private SavedGameItem savedGameItem;
    [SerializeField] private Toggle Toggle;
    [SerializeField] private HostEvent onClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.onClick != null)
            { this.onClick.Invoke(this.savedGameItem, this.Toggle); }
    }
}
