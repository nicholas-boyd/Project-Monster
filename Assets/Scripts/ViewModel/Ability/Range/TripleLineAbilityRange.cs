using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TripleLineAbilityRange : AbilityRange
{
    public override List<Tile> GetTilesInRange(Board board)
    {
        List<Point> startPositions = new List<Point>();
        startPositions.Add(unit.tile.pos);
        startPositions.Add(unit.tile.pos + new Point(0, 1));
        startPositions.Add(unit.tile.pos + new Point(0, -1));
        Point endPos;
        List<Tile> retValue = new List<Tile>();
        
        for (int i = 0; i < startPositions.Count; i++)
        {
            Point startPos = startPositions[i];
            if (horizontal > 0)
            {
                endPos = new Point(Mathf.RoundToInt(Mathf.Clamp(startPositions[i].x + horizontal, 0, board.max.x)), startPositions[i].y);
            }
            else
            {
                endPos = new Point(board.max.x, startPositions[i].y);
            }
            while (startPos != endPos)
            {
                if (startPos.x < endPos.x) startPos.x++;
                else if (startPos.x > endPos.x) startPos.x--;
                if (startPos.y < endPos.y) startPos.y++;
                else if (startPos.y > endPos.y) startPos.y--;
                Tile t = board.GetTile(startPos);
                if (t != null)
                    retValue.Add(t);
            }
        }

        return retValue;
    }
}