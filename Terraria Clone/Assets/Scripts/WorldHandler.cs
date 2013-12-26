using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityThreading;

public class WorldHandler : MonoBehaviour 
{
	public static WorldHandler worldHandler;

	public Transform player;
	public int groundLevel = 900;
	public int width = 64;
	public int height = 64;
	public int chunkViewWidth = 1;
	public int chunkViewHeight = 1;
	public bool shouldFill = true;
	public bool shouldSmooth = true;
	public bool regenOnUpdate = false;

	public Vector2 offset = Vector2.zero;

	public GameObject[] tiles;

	public float yScale = 10;
	public float noiseScale = 10;

	public List<Chunk> Chunks = new List<Chunk>();
	public int playerChunkLocation;

	void Start()
	{
		worldHandler = this;

		InvokeRepeating ("worldLoader", 0, 0.3f);
		GenWorld ();
	}

	void Update()
	{
		if(regenOnUpdate) 
		{
			foreach(Chunk c in Chunks)
				DestroyImmediate (c.parent);

			Chunks = new List<Chunk>();
			GenWorld ();
		}
	}

	void GenWorld()
	{
		for(int c = 0; c<chunkViewWidth; c++)
		{
			genChunk(c);
		}
	}
	void genChunk(int c)
	{
		if(!ifChunkExists (c))
		{
			//Make new chunk
			Chunk chunk = new Chunk(c);
			Chunks.Add (chunk);
			print ("Making chunk: " + c);

			UnityThreadHelper.CreateThread(() => { genNewChunk(chunk); } );
		}
		else
		{
			//Load chunk
			GetChunk(c).Load ();
		}
	}
 	void genNewChunk(Chunk chunk)
	{
		for(int x = 0; x<width; x++)
		{
			float h = groundLevel + (yScale * Mathf.PerlinNoise ((x + offset.x + (chunk.x * width)) / noiseScale, offset.y));
			Vector3 location = new Vector3(x + (chunk.x * width), (int)h, 0);
			chunk.AddTile (0, location);
			
			if(shouldFill)
			{
				for(int y = 0; y<(int)h; y++)
				{
					location.y = y;
					chunk.AddTile (0, location);
				}
			}
		}
	}

	void worldLoader()
	{
		playerChunkLocation = (int)((int)player.transform.position.x / width);

		// Load
		for(int x = playerChunkLocation - chunkViewWidth; x<playerChunkLocation + chunkViewWidth; x++)
		{
			genChunk(x);
		}

		// Unload
		foreach(Chunk ch in Chunks)
		{
			print (string.Format ("Distance: {0}, ViewCheckDistance: {1}, ChunkLocation: {2}",
			                      Mathf.Abs (ch.x - playerChunkLocation), chunkViewWidth, ch.x));

			if((Mathf.Abs (ch.x - playerChunkLocation)) > chunkViewWidth)
				if(ch.isLoaded ())
					ch.Unload ();
		}
	}
	public bool ifChunkExists(int c)
	{
		foreach(Chunk ch in Chunks)
		{
			if(ch.x == c)
			{
				return true;
			}
		}

		return false;
	}
	public Chunk GetChunk(int c)
	{
		foreach(Chunk ch in Chunks)
		{
			if(ch.x == c)
			{
				return ch;
			}
		}
		
		return null;
	}
	GameObject getTile(int x, int y)
	{
		Collider2D go = Physics2D.OverlapPoint (new Vector2(x, y));
		if(go)
			return go.gameObject;
		else
			return null;
	}
	GameObject getTileLeft(int x, int y)
	{
		return getTile (x - 1, y);
	}
	GameObject getTileRight(int x, int y)
	{
		return getTile (x + 1, y);
	}
	GameObject getTileAbove(int x, int y)
	{
		return getTile (x, y + 1);
	}
	GameObject getTileBelow(int x, int y)
	{
		return getTile (x, y - 1);
	}
}