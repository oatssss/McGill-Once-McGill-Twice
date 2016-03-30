using UnityEngine;
using UnityEngine.UI;

public class ConnectFourUI : Overlay {

    [SerializeField] private Text TurnLabel;

    public void SetTurn(bool yourTurn)
    {
        if (yourTurn)
            { this.TurnLabel.text = "Your turn"; }
        else
            { this.TurnLabel.text = "Opponent's turn"; }
    }
}
