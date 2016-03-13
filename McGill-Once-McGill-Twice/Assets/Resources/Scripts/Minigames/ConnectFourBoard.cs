﻿using UnityEngine;
using ExtensionMethods;

public class ConnectFourBoard : Photon.PunBehaviour {

    private class ConnectFourSlot : MonoBehaviour
    {
        public enum Colour { Red, Blue, Empty }
        private Colour status;
        public Colour Status
        {
            get { return this.status; }
            set
            {
                this.status = value;
                // TODO : Set chip colour
            }
        }
    }

    private bool LocalSideB = false;
    private PhotonPlayer RemotePlayer;
    private ConnectFourSlot.Colour LocalColour;
    private ConnectFourSlot.Colour RemoteColour;
    private static int Rows = 6;
    private static int Columns = 7;
    private ConnectFourSlot[,] Slots = new ConnectFourSlot[Columns,Rows];    // [col,row] following standard cartesian coordinates from side A (+x=right+x,+y=up)
    [SerializeField] ConnectFourSlot[] Col0 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col1 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col2 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col3 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col4 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col5 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col6 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] SelectorsA = new ConnectFourSlot[Columns];
    [SerializeField] ConnectFourSlot[] SelectorsB = new ConnectFourSlot[Columns];
    [SerializeField] ConnectFourSlot[] LocalSelectors;
    [SerializeField] ConnectFourSlot[] RemoteSelectors;

    private int LocalSelectorIndex = 0;

    public bool Playing = false;
    private bool PlayerTurn = false;

    void Awake()
    {
        for (int x = 0; x < this.Slots.GetLength(0); x++)
        {
            ConnectFourSlot[] column = this.Col0;
            switch (x)
            {
                case 0: column = this.Col0; break;
                case 1: column = this.Col1; break;
                case 2: column = this.Col2; break;
                case 3: column = this.Col3; break;
                case 4: column = this.Col4; break;
                case 5: column = this.Col5; break;
                case 6: column = this.Col6; break;
            }

            for (int y = 0; y < this.Slots.GetLength(1); y++)
            {
                this.Slots[x,y] = column[y];
            }
        }
    }

    public void StartGame(PhotonPlayer A, PhotonPlayer B)
    {
        this.photonView.ClearRpcBufferAsMasterClient();    // We don't care about instructions in the buffer for previous games at this point
        this.photonView.RPC("ResetBoard", PhotonTargets.AllBufferedViaServer);
        this.PlayerTurn = false;

        if (A.Equals(PhotonNetwork.player))
        {
            this.LocalSideB = false;
            this.RemotePlayer = B;
            this.LocalColour = ConnectFourSlot.Colour.Blue;
            this.RemoteColour = ConnectFourSlot.Colour.Red;
            this.LocalSelectors = SelectorsA;
            this.RemoteSelectors = SelectorsB;
        }
        else if (B.Equals(PhotonNetwork.player))
        {
            this.LocalSideB = true;
            this.RemotePlayer = A;
            this.LocalColour = ConnectFourSlot.Colour.Red;
            this.RemoteColour = ConnectFourSlot.Colour.Blue;
            this.LocalSelectors = SelectorsB;
            this.RemoteSelectors = SelectorsA;
        }
        else    // This game is starting, but the local player isn't one of the players
        {
            return;
        }

        this.Playing = true;
        this.LocalSelectors[this.LocalSelectorIndex].Status = this.LocalColour;
    }

    public void EndGame()
    {
        this.Playing = false;
    }

    private void MoveSelectionLeft()
    {
        if (this.LocalSelectorIndex <= 0)
        {
            Debug.Log("Selection is already at the leftmost edge");
        }
        else
        {
            // Disable current selector, enable selector to left
            this.LocalSelectors[LocalSelectorIndex].Status = ConnectFourSlot.Colour.Empty;
            this.LocalSelectors[this.LocalSelectorIndex - 1].Status = this.LocalColour;

            // Tell opposing player that our selection has moved left
            this.photonView.RPC("RemoteMoveSelectionLeft", this.RemotePlayer, this.LocalSelectorIndex--);
        }
    }

    private void MoveSelectionRight()
    {
        if (this.LocalSelectorIndex >= this.LocalSelectors.Length-1)
        {
            Debug.Log("Selection is already at the rightmost edge");
        }
        else
        {
            // Disable current selector, enable selector to right
            this.LocalSelectors[LocalSelectorIndex].Status = ConnectFourSlot.Colour.Empty;
            this.LocalSelectors[this.LocalSelectorIndex + 1].Status = this.LocalColour;

            // Tell opposing player that our selection has moved right
            this.photonView.RPC("RemoteMoveSelectionRight", this.RemotePlayer, this.LocalSelectorIndex++);
        }
    }

    [PunRPC]
    private void RemoteMoveSelectionLeft(int currentRemoteSelectorIndex, PhotonMessageInfo info)
    {
        if (currentRemoteSelectorIndex <= 0)
        {
            Debug.LogErrorFormat("{0} attempted to move the connect four move selector left from index {1} which would be out of bounds.", info.sender, currentRemoteSelectorIndex);
            return;
        }

        this.RemoteSelectors[currentRemoteSelectorIndex].Status = ConnectFourSlot.Colour.Empty;
        this.RemoteSelectors[currentRemoteSelectorIndex - 1].Status = this.RemoteColour;
    }

    [PunRPC]
    private void RemoteMoveSelectionRight(int currentRemoteSelectorIndex, PhotonMessageInfo info)
    {
        if (currentRemoteSelectorIndex >= this.RemoteSelectors.Length-1)
        {
            Debug.LogErrorFormat("{0} attempted to move the connect four move selector right from index {1} which would be out of bounds.", info.sender, currentRemoteSelectorIndex);
            return;
        }

        this.RemoteSelectors[currentRemoteSelectorIndex].Status = ConnectFourSlot.Colour.Empty;
        this.RemoteSelectors[currentRemoteSelectorIndex + 1].Status = this.RemoteColour;
    }

    private void AcceptMove()
    {
        if (!this.PlayerTurn)
        {
            // TODO : Alert not this player's turn
            return;
        }

        int slotColumn = this.LocalSelectorIndex;

        // Board is flipped from B side
        if (this.LocalSideB)
        { slotColumn = this.LocalSelectors.Length - this.LocalSelectorIndex - 1; }

        // Get the next empty slot in the column if it exists
        int slotRow = 0;
        while (slotRow < Rows)
        {
            ConnectFourSlot slot = this.Slots[slotColumn,slotRow];
            if (slot.Status == ConnectFourSlot.Colour.Empty)
            { break; }

            slotRow++;
        }

        if (slotRow == Rows)    // No empty slots in this column
        {
            Debug.Log("Selected column is full!");
            // TODO : Alert full column to player
            return;
        }

        this.PlayerTurn = false;
        this.photonView.RPC("ModifySlot", PhotonTargets.AllBufferedViaServer, slotColumn, slotRow, this.LocalColour);
        this.photonView.RPC("SetTurn", this.RemotePlayer);
    }

    [PunRPC]
    private void ModifySlot(int slotColumn, int slotRow, ConnectFourSlot.Colour colour, PhotonMessageInfo info)
    {
        this.Slots[slotColumn,slotRow].Status = colour;

        // TODO : Check if someone won with four in a row (use new slot as starting point)
    }

    [PunRPC]
    private void ResetBoard(PhotonMessageInfo info)
    {
        foreach (ConnectFourSlot slot in this.Slots)
        { slot.Status = ConnectFourSlot.Colour.Empty; }

        foreach (ConnectFourSlot selector in this.LocalSelectors)
        { selector.Status = ConnectFourSlot.Colour.Empty; }

        foreach (ConnectFourSlot selector in this.RemoteSelectors)
        { selector.Status = ConnectFourSlot.Colour.Empty; }

        this.LocalSelectorIndex = 0;
    }

    [PunRPC]
    private void SetTurn()
    {
        this.PlayerTurn = true;
    }

    void Update()
    {
        if (!this.Playing)
        { return; }

        if (CustomInputManager.GetButtonDown("Left"))
        { this.MoveSelectionLeft(); }

        else if (CustomInputManager.GetButtonDown("Right"))
        { this.MoveSelectionRight(); }

        else if (CustomInputManager.GetButtonDown("Submit"))
        { this.AcceptMove(); }
    }
}