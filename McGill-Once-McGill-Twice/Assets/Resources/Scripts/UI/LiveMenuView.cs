using UnityEngine;

public abstract class LiveMenuView : MonoBehaviour {

    public void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateSegment();
    }
    public abstract void UpdateSegment();
}
