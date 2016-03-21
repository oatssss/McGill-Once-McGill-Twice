using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;

public class ExistingGamesView : LiveMenuView {

    [SerializeField] private RectTransform Content;
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button DeleteGameButton;
    [SerializeField] private ExistingGameItem ExistingGameItemPrefab;
    [ReadOnly] [SerializeField] private Dictionary<ExistingGameItem,FileInfo> Games = new Dictionary<ExistingGameItem,FileInfo>();
    [ReadOnly] [SerializeField] private ExistingGameItem SelectedGame;

    public override void Activate()
    {
        SortedList<DateTime,FileInfo> saved = IOManager.GetSavedSessionFiles();
        foreach (FileInfo file in saved.Values)
        {
            GameManager.GameState session = IOManager.ReadFromFile<GameManager.GameState>(file.Directory.Name + "/" + file.Name);
            if (session != null)
            {
                ExistingGameItem newListItem = Instantiate<ExistingGameItem>(this.ExistingGameItemPrefab);
                newListItem.gameObject.SetActive(true);
                newListItem.SessionName.text = session.RoomName;
                newListItem.Session = session;
                newListItem.gameObject.transform.SetParent(this.Content, false);
                this.Games.Add(newListItem, file);
            }
        }
        base.Activate();
        this.DeleteGameButton.interactable = false;
        this.StartGameButton.interactable = false;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        this.Games.Clear();
        foreach (Transform child in this.Content)
        {
            Destroy(child.gameObject);
        }
        this.DeleteGameButton.interactable = false;
        this.StartGameButton.interactable = false;
        this.Deselect(this.SelectedGame);
    }

    protected override void Update()
    {
        // Update does nothing for this view
    }

    public void Select(ExistingGameItem game)
    {
        this.SelectedGame = game;
    }

    public void Deselect(ExistingGameItem caller)
    {
        if (this.SelectedGame == caller)
            { this.SelectedGame = null; }
    }

    public void DeleteGame()
    {
        ExistingGameItem deleting = this.SelectedGame;
        FileInfo file = this.Games[deleting];
        this.Games.Remove(deleting);
        this.Deselect(deleting);
        Destroy(deleting.gameObject);
        string relativePath = file.Directory.Name + "/" + file.Name;
        IOManager.DeleteFile(relativePath);
    }

    public void StartGame()
    {
        GameManager.Instance.SessionState = SelectedGame.Session;
    }
}
