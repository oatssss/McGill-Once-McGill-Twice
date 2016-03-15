using UnityEngine;

public class ConnectFourSlot : MonoBehaviour
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

    public void Highlight(bool highlight)
    {
        HighlightManager.Instance.Highlight(this.gameObject, highlight);
    }
}