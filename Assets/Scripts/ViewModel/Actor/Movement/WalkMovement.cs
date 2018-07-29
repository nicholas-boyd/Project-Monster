using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WalkMovement : Movement
{

    bool acting = false;

    void OnEnable()
    {
        this.AddObserver(AbilityActivated, AbilityHandController.AbilityCardSelectedNotification);
        this.AddObserver(AbilityCompleted, Card.AbilityCardCantPerformNotification);
        this.AddObserver(AbilityCompleted, Card.AbilityCardActivatedNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(AbilityActivated, AbilityHandController.AbilityCardSelectedNotification);
        this.RemoveObserver(AbilityCompleted, Card.AbilityCardCantPerformNotification);
        this.RemoveObserver(AbilityCompleted, Card.AbilityCardActivatedNotification);
    }

    void AbilityActivated(object sender, object args)
    {
        acting = true;
    }

    void AbilityCompleted(object sender, object args)
    {
        acting = false;
    }

    protected override bool ExpandSearch(Tile from, Tile to)
    {
        // Skip if the tile is occupied by an object
        if (to.content != null)
            return false;
        // Skip if the tile is on the enemy side
        if (to.pos.x > 3)
            return false;
        return base.ExpandSearch(from, to);
    }

    public override IEnumerator Traverse(Tile tile)
    {
        if (!acting)
        {
            this.PostNotification(MovementBegunNotification);

            float speed = gameObject.GetComponentInParent<Stats>()[StatTypes.SPD];
            Debug.Log(speed);
            if (speed > 0)
            {
                unit.Place(tile);
                // Build a list of way points from the unit's 
                // starting tile to the destination tile
                List<Tile> targets = new List<Tile>();
                while (tile != null)
                {
                    targets.Insert(0, tile);
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
                yield return StartCoroutine(Flip(Directions.South));
            }
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