using UnityEngine;

public class ConnectFourSlot : MonoBehaviour
{
    public enum Colour { Red, Black, Empty }
    [SerializeField] private GameObject RedChip;
    [SerializeField] private GameObject BlackChip;
    private Colour status;
    public Colour Status
    {
        get { return this.status; }
        set
        {
            this.status = value;
            if (value == Colour.Empty)
            {
                RedChip.GetComponent<Renderer>().enabled = false;
                BlackChip.GetComponent<Renderer>().enabled = false;
            }
            else if (value == Colour.Red)
            {
                RedChip.GetComponent<Renderer>().enabled = true;
                BlackChip.GetComponent<Renderer>().enabled = false;
            }
            else if (value == Colour.Black)
            {
                RedChip.GetComponent<Renderer>().enabled = false;
                BlackChip.GetComponent<Renderer>().enabled = true;
            }
        }
    }

    public void Highlight(bool highlight)
    {
        HighlightManager.Instance.Highlight(this.gameObject, highlight);
    }
}