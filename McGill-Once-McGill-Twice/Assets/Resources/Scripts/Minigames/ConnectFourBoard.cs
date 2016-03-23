using UnityEngine;
using System;
using System.Collections.Generic;
using ExtensionMethods;

public class ConnectFourBoard : Photon.PunBehaviour {

    [SerializeField] private Minigame ParentMinigame;
    [SerializeField] private bool singleSided;
    public bool SingleSided { get { return this.singleSided; } private set { this.singleSided = value; } }
    private bool LocalSideB = false;
    private PhotonPlayer RemotePlayer;
    private ConnectFourSlot.Colour LocalColour;
    private ConnectFourSlot.Colour RemoteColour;
    private static int Rows = 6;
    private static int Columns = 7;
    private ConnectFourSlot[,] Slots = new ConnectFourSlot[Columns,Rows];    // [col,row] following standard cartesian coordinates from side A (+x=right,+y=up)
    [SerializeField] ConnectFourSlot[] Col0 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col1 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col2 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col3 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col4 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col5 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] Col6 = new ConnectFourSlot[Rows];
    [SerializeField] ConnectFourSlot[] SelectorsA = new ConnectFourSlot[Columns];
    [SerializeField] ConnectFourSlot[] SelectorsB = new ConnectFourSlot[Columns];
    [ReadOnly] [SerializeField] ConnectFourSlot[] LocalSelectors;
    [ReadOnly] [SerializeField] ConnectFourSlot[] RemoteSelectors;

    private int LocalSelectorIndex = 0;

    [ReadOnly] public bool Playing = false;
    [ReadOnly] [SerializeField] private bool PlayerTurn = false;

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

    public void StartPlaying(PhotonPlayer A, PhotonPlayer B)
    {
        this.photonView.ClearRpcBufferAsMasterClient();    // We don't care about instructions in the buffer for previous games at this point
        this.photonView.RPC("ResetBoard", PhotonTargets.AllBufferedViaServer);
        this.PlayerTurn = false;

        if (A.Equals(PhotonNetwork.player))
        {
            this.LocalSideB = false;
            this.RemotePlayer = B;
            this.LocalColour = ConnectFourSlot.Colour.Black;
            this.RemoteColour = ConnectFourSlot.Colour.Red;
            this.LocalSelectors = SelectorsA;
            this.RemoteSelectors = SelectorsB;
            if (this.SingleSided)
                { Array.Reverse(this.RemoteSelectors); }
        }
        else if (B.Equals(PhotonNetwork.player))
        {
            this.LocalSideB = true;
            this.RemotePlayer = A;
            this.LocalColour = ConnectFourSlot.Colour.Red;
            this.RemoteColour = ConnectFourSlot.Colour.Black;
            this.LocalSelectors = SelectorsB;
            this.RemoteSelectors = SelectorsA;
            if (this.SingleSided)
                { Array.Reverse(this.LocalSelectors); }
        }
        else    // This game is starting, but the local player isn't one of the players
        {
            return;
        }
        this.photonView.RPC("ResetBoardForPlay", this.RemotePlayer);
        this.Playing = true;
        this.LocalSelectors[this.LocalSelectorIndex].Status = this.LocalColour;
    }

    public void StopPlaying()
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
            this.LocalSelectors[this.LocalSelectorIndex].Status = ConnectFourSlot.Colour.Empty;
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
            this.LocalSelectors[this.LocalSelectorIndex].Status = ConnectFourSlot.Colour.Empty;
            this.LocalSelectors[this.LocalSelectorIndex + 1].Status = this.LocalColour;

            // Tell opposing player that our selection has moved right
            this.photonView.RPC("RemoteMoveSelectionRight", this.RemotePlayer, this.LocalSelectorIndex++);
        }
    }

    [PunRPC]
    private void RemoteMoveSelectionLeft(int currentRemoteSelectorIndex, PhotonMessageInfo info)
    {
        Debug.Log("REMOTE INDEX PRIOR TO MOVE LEFT: " + currentRemoteSelectorIndex);
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
        Debug.Log("REMOTE INDEX PRIOR TO MOVE RIGHT: " + currentRemoteSelectorIndex);
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
            GUIManager.Instance.ShowTooltip("It's not your turn!");
            return;
        }

        int slotColumn = this.LocalSelectorIndex;

        // Board is flipped from B side
        if (this.LocalSideB && !this.SingleSided)
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
            GUIManager.Instance.ShowTooltip("There are no free slots in that column.");
            return;
        }

        this.PlayerTurn = false;
        this.photonView.RPC("ModifySlot", PhotonTargets.AllBufferedViaServer, slotColumn, slotRow, this.LocalColour);
    }

    [PunRPC]
    private void ModifySlot(int slotColumn, int slotRow, ConnectFourSlot.Colour colour, PhotonMessageInfo info)
    {
        this.Slots[slotColumn,slotRow].Status = colour;

        // Don't check for wins if the slot was set to empty
        if (colour == ConnectFourSlot.Colour.Empty)
            { return; }

        bool winner = false;
        winner |= this.CheckHorizontalWin(slotColumn, slotRow, colour);
        winner |= this.CheckVerticalWin(slotColumn, slotRow, colour);
        winner |= this.CheckTLBRDiagonalWin(slotColumn, slotRow, colour);
        winner |= this.CheckTRBLDiagonalWin(slotColumn, slotRow, colour);

        if (winner && this.Playing)
        {
            this.StopPlaying();
            this.ParentMinigame.RequireInteractForLobby();
        }
        else if (colour != this.LocalColour)
        {
            this.PlayerTurn = true;
        }
    }

    private bool CheckHorizontalWin(int slotColumn, int slotRow, ConnectFourSlot.Colour colour)
    {
        List<ConnectFourSlot> winningSlots = new List<ConnectFourSlot> { this.Slots[slotColumn,slotRow] };

        int col = slotColumn + 1;   // Check towards the right first
        ConnectFourSlot slot = col < Columns ? this.Slots[col,slotRow] : null;
        while (slot != null && slot.Status == colour)
        {
            winningSlots.Add(slot);
            col++;
        }

        col = slotColumn - 1;       // Check towards the left now
        slot = col >= 0 ? this.Slots[col,slotRow] : null;
        while (slot != null && slot.Status == colour)
        {
            winningSlots.Add(slot);
            col--;
        }

        if (winningSlots.Count >= 4)
        {
            this.HighlightWinningSlots(winningSlots);
            return true;
        }

        return false;
    }

    private bool CheckVerticalWin(int slotColumn, int slotRow, ConnectFourSlot.Colour colour)
    {
        List<ConnectFourSlot> winningSlots = new List<ConnectFourSlot> { this.Slots[slotColumn,slotRow] };

        int row = slotRow + 1;   // Check towards the top first
        ConnectFourSlot slot = row < Rows ? this.Slots[slotColumn,row] : null;
        while (slot != null && slot.Status == colour)
        {
            winningSlots.Add(slot);
            row++;
        }

        row = slotRow - 1;       // Check towards the bottom now
        slot = row >= 0 ? this.Slots[slotColumn,row] : null;
        while (slot != null && slot.Status == colour)
        {
            winningSlots.Add(slot);
            row--;
        }

        if (winningSlots.Count >= 4)
        {
            this.HighlightWinningSlots(winningSlots);
            return true;
        }

        return false;
    }

    // TLBR = Top Left Bottom Right
    private bool CheckTLBRDiagonalWin(int slotColumn, int slotRow, ConnectFourSlot.Colour colour)
    {
        List<ConnectFourSlot> winningSlots = new List<ConnectFourSlot> { this.Slots[slotColumn,slotRow] };

        int row = slotRow + 1;   // Check towards the top left first
        int col = slotColumn - 1;
        ConnectFourSlot slot = (row < Rows && col >= 0) ? this.Slots[col,row] : null;
        while (slot != null && slot.Status == colour)
        {
            winningSlots.Add(slot);
            row++;
            col--;
        }

        row = slotRow - 1;       // Check towards the bottom right now
        col = slotColumn + 1;
        slot = (row >= 0 && col < Columns) ? this.Slots[col,row] : null;
        while (slot != null && slot.Status == colour)
        {
            winningSlots.Add(slot);
            row--;
            col++;
        }

        if (winningSlots.Count >= 4)
        {
            this.HighlightWinningSlots(winningSlots);
            return true;
        }

        return false;
    }

    // TRBL = Top Right Bottom Left
    private bool CheckTRBLDiagonalWin(int slotColumn, int slotRow, ConnectFourSlot.Colour colour)
    {
        List<ConnectFourSlot> winningSlots = new List<ConnectFourSlot> { this.Slots[slotColumn,slotRow] };

        int row = slotRow + 1;   // Check towards the top right first
        int col = slotColumn + 1;
        ConnectFourSlot slot = (row < Rows && col < Columns) ? this.Slots[col,row] : null;
        while (slot != null && slot.Status == colour)
        {
            winningSlots.Add(slot);
            row++;
            col++;
        }

        row = slotRow - 1;       // Check towards the bottom left now
        col = slotColumn - 1;
        slot = (row >= 0 && col >= 0) ? this.Slots[col,row] : null;
        while (slot != null && slot.Status == colour)
        {
            winningSlots.Add(slot);
            row--;
            col--;
        }

        if (winningSlots.Count >= 4)
        {
            // Only highlight for players in the game
            if (this.Playing)
                { this.HighlightWinningSlots(winningSlots); }

            return true;
        }

        return false;
    }

    private void HighlightWinningSlots(List<ConnectFourSlot> slots)
    {
        foreach (ConnectFourSlot slot in slots)
        {
            slot.Highlight(true);
        }
    }

    [PunRPC]
    private void ResetBoard(PhotonMessageInfo info)
    {
        foreach (ConnectFourSlot slot in this.Slots)
        {
            slot.Status = ConnectFourSlot.Colour.Empty;
            slot.Highlight(false);
        }

        foreach (ConnectFourSlot selector in this.LocalSelectors)
        {
            selector.Status = ConnectFourSlot.Colour.Empty;
            selector.Highlight(false);
        }

        foreach (ConnectFourSlot selector in this.RemoteSelectors)
        {
            selector.Status = ConnectFourSlot.Colour.Empty;
            selector.Highlight(false);
        }

        this.LocalSelectorIndex = 0;
    }

    [PunRPC]
    private void ResetBoardForPlay(PhotonMessageInfo info)
    {
        this.LocalSelectorIndex = 0;

        foreach (ConnectFourSlot slot in this.Slots)
        {
            slot.Status = ConnectFourSlot.Colour.Empty;
            slot.Highlight(false);
        }

        foreach (ConnectFourSlot selector in this.LocalSelectors)
        {
            selector.Status = ConnectFourSlot.Colour.Empty;
            selector.Highlight(false);
        }
        this.LocalSelectors[0].Status = this.LocalColour;

        foreach (ConnectFourSlot selector in this.RemoteSelectors)
        {
            selector.Status = ConnectFourSlot.Colour.Empty;
            selector.Highlight(false);
        }
        this.RemoteSelectors[0].Status = this.RemoteColour;
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
