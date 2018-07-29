using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public abstract class AbilityRange : MonoBehaviour {
	public int horizontal = 1;
	protected Unit unit { get { return GetComponentInParent<Unit>(); }}
	public abstract List<Tile> GetTilesInRange (Board board);
}