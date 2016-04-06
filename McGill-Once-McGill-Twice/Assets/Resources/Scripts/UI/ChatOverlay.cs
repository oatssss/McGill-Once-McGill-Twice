using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class ChatOverlay : Overlay {

    [SerializeField] private ChatMessageInput MessageInput;
    [SerializeField] private ChatMessageText MessageTextTemplate;
    [SerializeField] private RectTransform Content;

    private List<ChatMessageText> Messages = new List<ChatMessageText>();

    public void OpenChatInput()
    {
        GUIManager.Instance.SetMenuFocus();
        this.MessageInput.Input.text = "";
        this.MessageInput.CanvasGroup.interactable = true;
        this.MessageInput.CanvasGroup.blocksRaycasts = true;
        this.MessageInput.CanvasGroup.alpha = 1f;
        this.MessageInput.Input.ActivateInputField();
    }

    public void CloseChatInput()
    {
        if (CustomInputManager.GetButtonDown("Submit"))
            { PhotonManager.SendChatMessage(this.MessageInput.Input.text); }

        GUIManager.Instance.SetGameFocus();
        this.MessageInput.CanvasGroup.alpha = 0f;
        this.MessageInput.CanvasGroup.interactable = false;
        this.MessageInput.CanvasGroup.blocksRaycasts = false;
        this.MessageInput.Input.text = "";
    }

    public void AddChatMessageText(string playerName, string messageText)
    {
        ChatMessageText adding = Instantiate<ChatMessageText>(this.MessageTextTemplate);
        adding.Text.text = "[" + playerName + "]: " + messageText;
        adding.transform.SetParent(this.Content, false);
        adding.gameObject.SetActive(true);
        adding.ParentOverlay = this;
        this.Messages.Insert(0, adding);

        if (this.Messages.Count > GUIManager.Instance.MaxChatMessages)
        {
            ChatMessageText removing = this.Messages.Last();
            removing.Remove();
        }
    }

    public void RemoveChatMessage(ChatMessageText removing)
    {
        this.Messages.Remove(removing);
        Destroy(removing.gameObject);
    }
}
