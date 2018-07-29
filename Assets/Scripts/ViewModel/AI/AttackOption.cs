using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttackOption
{
    class Mark
    {
        public Tile tile;
        public bool isMatch;

        public Mark(Tile tile, bool isMatch)
        {
            this.tile = tile;
            this.isMatch = isMatch;
        }
    }
    
    public List<Tile> targets = new List<Tile>();
    public int bestScore { get; private set; }
    List<Mark> marks = new List<Mark>();
    public List<Tile> moveTargets = new List<Tile>();

    public void AddMoveTarget(Tile tile)
    {
        moveTargets.Add(tile);
    }

    public void AddMark(Tile tile, bool isMatch)
    {
        marks.Add(new Mark(tile, isMatch));
    }

    public int GetScore(Unit caster, Card ability)
    {
        int score = 0;
        for (int i = 0; i < marks.Count; ++i)
        {
            if (marks[i].isMatch)
                score=score+2;
            else
                score--;
        }
        return score;
    }
}