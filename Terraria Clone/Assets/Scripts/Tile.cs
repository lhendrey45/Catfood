using UnityEngine;
using System.Collections.Generic;

public class Tile
{
	public Vector2 location;
	public int ID;

	public Tile(int ID, int X, int Y)
	{
		_Tile (ID, X, Y);
	}
	public Tile(int ID, Vector2 location)
	{
		_Tile(ID, (int)location.x, (int)location.y);
	}
	public Tile(int ID, Vector3 location)
	{
		_Tile(ID, (int)location.x, (int)location.y);
	}

	public void _Tile(int ID, int X, int Y)
	{
		this.ID = ID;
		this.location = new Vector2(X, Y);
	}
}
