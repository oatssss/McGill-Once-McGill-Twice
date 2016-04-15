/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingLocationGenerator
{
	private int seed;
	private int numberOfBuildings;
	private Random rng;
	private List<Vector3> coordinateList;
	private int terrainDim; // assumes terrain is a square

	public BuildingLocationGenerator(int seed, int numberOfBuildings, int terrainDim)
	{
		this.seed = seed;
		this.numberOfBuildings = numberOfBuildings;
		this.terrainDim = terrainDim;
		this.rng = new Random(this.seed);
		this.coordinateList = new List<Vector3>(this.numberOfBuildings);

		generatePoints();
	}

	public void generatePoints()
	{
		for(int i = 0; i < numberOfBuildings; i++)
		{
			Vector3 newPoint = new Vector3(rng.next(terrainDim), 0, rng.next(terrainDim));
			this.coordinateList.add(newPoint);
		}
	}
	public List<Vector3> getPointList()
	{
		return this.coordinateList.Clone();
	}

	public int getSeed()
	{
		return this.seed;
	}
	public bool setSeed(int seed)
	{
		if(seed < 0)
			return false;

		this.seed = seed;
		return true;
	}

	public int getNumBuildings()
	{
		return this.numberOfBuildings;
	}
	public bool setNumBuildings(int numberOfBuildings)
	{
		if(numberOfBuildings < 0)
			return false;

		this.numberOfBuildings = numberOfBuildings;
		return true;
	}

	public int getTerrainDim()
	{
		return this.terrainDim;
	}
	public bool setTerrainDim(int terrainDim)
	{
		if(terrainDim < 0)
			return false;

		this.terrainDim = terrainDim;
		return true;
	}
}
*/