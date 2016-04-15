using UnityEngine;
using System.Collections;

public class Launch : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GUIManager.FadeToClear( () => GUIManager.Instance.OpenMenu(GUIManager.Instance.StartupMenu) );
	}
}
