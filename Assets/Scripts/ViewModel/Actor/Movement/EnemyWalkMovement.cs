﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyWalkMovement : Movement
{
    public const string ReserveTileNotification = "ENEMY_ON_TILE";
    public const string UnreserveTilesNotification = "ENEMY_ON_TILE";
    protected List<Tile> unmoveableTiles;

    void OnEnable()
    {
        this.AddObserver(AddUnmoveableTile, EnemyWalkMovement.ReserveTileNotification);
        this.AddObserver(RemoveUnmoveableTiles, EnemyWalkMovement.UnreserveTilesNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(AddUnmoveableTile, EnemyWalkMovement.ReserveTileNotification);
        this.RemoveObserver(RemoveUnmoveableTiles, EnemyWalkMovement.UnreserveTilesNotification);
    }

    void AddUnmoveableTile(object sender, object args)
    {
        unmoveableTiles.Add(args as Tile);
    }

    void RemoveUnmoveableTiles(object sender, object args)
    {
        List<Tile> tiles = args as List<Tile>;
        if (tiles != null)
            foreach (Tile t in tiles)
                if (unmoveableTiles.Contains(t))
                    unmoveableTiles.Remove(t);
    }

    void Start()
    {
        unmoveableTiles = new List<Tile>();
    }

    protected override bool ExpandSearch(Tile from, Tile to)
    {
        // Skip if the tile is occupied by an object
        if (to.content != null)
            return false;
        // Skip if the tile is on the hero side
        if (to.pos.x < 3)
            return false;
        // Skip if the tile is already reserved by another enemy
        if (unmoveableTiles.Contains(to))
            return false;
        return base.ExpandSearch(from, to);
    }

    public override IEnumerator Traverse(Tile tile)
    {
        this.PostNotification(MovementBegunNotification);
        float speed = gameObject.GetComponentInParent<Stats>()[StatTypes.SPD];
        if (speed > 0)
        {
            unit.Place(tile);
            // Build a list of way points from the unit's 
            // starting tile to the destination tile
            List<Tile> targets = new List<Tile>();
            while (tile != null)
            {
                targets.Insert(0, tile);
                this.PostNotification(ReserveTileNotification, tile);
                tile = tile.prev;
            }
            // Move to each way point in succession

            for (int i = 1; i < targets.Count; ++i)
            {
                Tile from = targets[i - 1];
                Tile to = targets[i];
                Directions dir = from.GetDirection(to);
                if (dir == Directions.West)
                {
                    dir = Directions.North;
                }
                else if (dir == Directions.East)
                {
                    dir = Directions.South;
                }
                else if (dir == Directions.North)
                {
                    dir = Directions.South;
                }
                if (unit.dir != dir)
                {
                    yield return StartCoroutine(Flip(dir));
                }
                yield return StartCoroutine(Walk(to, speed));
            }
            this.PostNotification(UnreserveTilesNotification, targets);
            yield return StartCoroutine(Flip(Directions.South));
        }
        this.PostNotification(MovementCompleteNotification);
        yield return null;
    }

    IEnumerator Walk(Tile target, float speed)
    {
        Vector3 center = new Vector3(target.center.x, target.center.y + GetComponent<SpriteRenderer>().bounds.size.y / 2, target.center.z);
        Tweener tweener = transform.MoveTo(center, speed, EasingEquations.Linear);
        while (tweener != null)
            yield return null;
    }
}