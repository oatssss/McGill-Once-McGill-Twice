﻿using UnityEngine;

public class PhotonSingleton<T> : Photon.MonoBehaviour
	where T : Component
{
	private static T instance;
	public static T Instance {
		get {
			if (instance == null) {
				instance = FindObjectOfType<T> ();
				if (instance == null) {
					GameObject obj = new GameObject ();
					obj.hideFlags = HideFlags.HideAndDontSave;
					instance = obj.AddComponent<T> ();
				}
			}
			return instance;
		}
	}
}