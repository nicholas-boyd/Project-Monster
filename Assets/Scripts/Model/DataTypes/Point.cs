using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Point {

	public int x, y;

	public Point(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public static Point operator +(Point i, Point j) {
		return new Point(i.x + j.x, i.y + j.y);
	}

	public static Point operator -(Point i, Point j) {
		return new Point(i.x - j.x, i.y - j.y);
	}

	public static bool operator ==(Point i, Point j) {
		return i.x == j.x && i.y == j.y;
	}

	public static bool operator !=(Point i, Point j) {
		return !(i == j);
	}

	public override bool Equals (object obj) {
		
		if (obj is Point) 
		{
			Point p = (Point)obj;
			return x == p.x && y == p.y;
		}
		return false;

	}

	public bool Equals (Point p) {
		return x == p.x && y == p.y;
	}

	public override int GetHashCode () {
		return x ^ y;
	}

	public override string ToString () {
		return string.Format ("({0}, {1})", this.x, this.y);
	}
}
