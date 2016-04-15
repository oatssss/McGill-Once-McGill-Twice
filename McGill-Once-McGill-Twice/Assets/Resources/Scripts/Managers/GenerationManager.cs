using UnityEngine;
using System.Collections.Generic;

public class GenerationManager : UnitySingleton<GenerationManager> {

	[SerializeField] private int MapSize = 200;
	[SerializeField] private Building[] BuildingPrefabs;
	[SerializeField] private int RandomBuildingPopulation = 20;
	[SerializeField] private LayerMask BoundLayers;

	private void Start()
	{
		this.GenerateMap(GameManager.Instance.SessionState.LevelSeed);
	}

	private void GenerateMap(long seed)
	{
		Random.InitState((int)seed);

		List<Building> existing = new List<Building>();
		for (int i = 0; i < this.RandomBuildingPopulation; i++)
		{
			// int randomIndex = Random.Range(0,this.BuildingPrefabs.Length);
			// Building randomPrefab = this.BuildingPrefabs[randomIndex];
			Building randomPrefab = this.BuildingPrefabs[i % this.BuildingPrefabs.Length];
			randomPrefab = Instantiate<Building>(randomPrefab);
			int iterations = 0;
			do {
				randomPrefab.transform.position = this.RandomLocation();
				randomPrefab.transform.rotation = this.RandomRotation();
				iterations++;
			} while (iterations < 30 && this.BuildingIntersectsExisting(randomPrefab, existing));

			if (iterations >= 30)
				{ Destroy(randomPrefab.gameObject); }
			else
				{ existing.Add(randomPrefab); }
		}
	}

	private Vector3 RandomLocation()
	{
		float xLoc = Random.Range(-this.MapSize, this.MapSize);
		float yLoc = 0;
		float zLoc = Random.Range(-this.MapSize, this.MapSize);
		return new Vector3(xLoc, yLoc, zLoc);
	}

	private Quaternion RandomRotation()
	{
		return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
	}

	private bool BuildingIntersectsExisting(Building spawned, List<Building> existing)
	{
		Collider[] intersecting = Physics.OverlapSphere(spawned.SphericalBounds.transform.position, spawned.SphericalBounds.transform.localScale.x*spawned.SphericalBounds.radius, this.BoundLayers, QueryTriggerInteraction.Collide);

		if (intersecting.Length > 1)
			{ return true; }
		else
			{ return false; }
	}
}
