using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public Point pos;
	public Vector3 center { get { return new Vector3(pos.x * transform.localScale.x, transform.localScale.y/2+0.03f, pos.y * transform.localScale.z); }}
	public GameObject content;

	//Used for pathfinding
	[HideInInspector] public Tile prev;
	[HideInInspector] public int distance;
    

    void SetPosition () {
		transform.localPosition = new Vector3( pos.x*transform.localScale.x, 0f, pos.y*transform.localScale.z );
	}

	public void Load (Point p) {
		pos = p;
		SetPosition ();
	}

}
