using UnityEngine;
using System.Collections.Generic;

public class Chunk
{
	public List<GameObject> tiles;
	public List<Tile> tilesID;
	public GameObject parent;
	public int x;

	public Chunk(int x)
	{
		this.x = x;

		parent = new GameObject(this.ToString ());
		tiles = new List<GameObject>();
		tilesID = new List<Tile>();
		parent.transform.position = new Vector3(0, 0, 0);
	}

	public void AddTile(int ID, Vector3 location, bool isNew = true)
	{
		GameObject Go = null;
		UnityThreadHelper.Dispatcher.Dispatch(() => 
		{  
			Go = MonoBehaviour.Instantiate (WorldHandler.worldHandler.tiles[ID]) as GameObject;
			Go.transform.parent = parent.transform;
			Go.transform.position = location;
		});
		
		tiles.Add (Go);

		if(isNew)
			tilesID.Add (new Tile(ID, location));
	}

	public void Unload()
	{
		MonoBehaviour.print ("Unloading chunk: " + x);

		foreach(GameObject go in tiles)
		{
			MonoBehaviour.Destroy (go);
		}

		tiles.Clear ();

		MonoBehaviour.Destroy (parent);
		parent = null;
	}

	public void Load()
	{
		if(isLoaded()) return;
		MonoBehaviour.print ("Loading chunk: " + x);

		parent = new GameObject(this.ToString ());

		foreach(Tile t in tilesID)
		{
			AddTile (t.ID, t.location, false);
		}
	}

	public override string ToString ()
	{
		return string.Format ("[Chunk] -> {0}", x);
	}

	public bool isLoaded()
	{
		return parent != null;
	}
}
