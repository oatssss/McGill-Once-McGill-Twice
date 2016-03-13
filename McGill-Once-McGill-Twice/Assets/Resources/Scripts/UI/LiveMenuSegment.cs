using UnityEngine;

public abstract class LiveMenuSegment : MonoBehaviour {

    public void Activate()
    {
        this.enabled = true;
    }

    public void Deactivate()
    {
        this.enabled = false;
    }

    void Update()
    {
        UpdateSegment();
    }
    public abstract void UpdateSegment();
}
