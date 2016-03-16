using UnityEngine;

public abstract class LiveMenuView : MonoBehaviour {

    public virtual void Activate()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Deactivate()
    {
        this.gameObject.SetActive(false);
    }

    protected abstract void Update();
}
