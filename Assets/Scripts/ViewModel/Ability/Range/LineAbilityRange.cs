using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class LineAbilityRange : AbilityRange {
	public override List<Tile> GetTilesInRange (Board board) {
		Point startPos = unit.tile.pos;
		Point endPos;
		List<Tile> retValue = new List<Tile>();
        if (horizontal > 0)
        {
            endPos = unit.GetComponent<Alliance>().type == Alliances.Hero ? new Point(Mathf.RoundToInt(Mathf.Clamp(startPos.x + horizontal, board.min.x, board.max.x)), startPos.y) : new Point(Mathf.RoundToInt(Mathf.Clamp(startPos.x - horizontal, board.min.x, board.max.x)), startPos.y);
        }
        else
        {
            endPos = unit.GetComponent<Alliance>().type == Alliances.Hero ? new Point(board.max.x, startPos.y) : new Point(board.min.x, startPos.y);
        }

        while (startPos != endPos)
        {
            if (startPos.x < endPos.x) startPos.x++;
            else if (startPos.x > endPos.x) startPos.x--;
            Tile t = board.GetTile(startPos);
            if (t != null)
                retValue.Add(t);
        }

        return retValue;
	}
}