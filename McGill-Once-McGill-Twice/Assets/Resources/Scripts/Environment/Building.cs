using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	[SerializeField] private SphereCollider _sphericalBounds;
	public SphereCollider SphericalBounds { get { return this._sphericalBounds; } }
}
