using UnityEngine;
using System.Collections;
using System;

public abstract class ValueModifier : Modifier, IComparable<ValueModifier> {
	public ValueModifier (int sortOrder) : base (sortOrder) {}
	public abstract float Modify (float fromValue, float toValue);
    public int CompareTo(ValueModifier other)
    {
        if (sortOrder < other.sortOrder)
            return -1;
        if (sortOrder > other.sortOrder)
            return 1;
        return 0;
    }
}