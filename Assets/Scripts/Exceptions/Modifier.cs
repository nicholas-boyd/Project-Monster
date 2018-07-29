using UnityEngine;
using System.Collections;
using System;

public abstract class Modifier: IComparable<Modifier> {
	public readonly int sortOrder;
	public Modifier (int sortOrder) {
		this.sortOrder = sortOrder;
	}

    public int CompareTo(Modifier other)
    {
        if (sortOrder < other.sortOrder)
            return -1;
        if (sortOrder > other.sortOrder)
            return 1;
        return 0;
    }
}