using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AIPlayerController : MonoBehaviour
{
    BattleController owner;
    Unit actor { get { return GetComponent<Unit>(); } }
    Alliance alliance { get { return actor.GetComponent<Alliance>(); } }
    Unit nearestFoe;
    float AIThoughtLatency;

    void Start()
    {
        owner = GetComponentInParent<BattleController>();
        AIThoughtLatency = 0;
    }
    void Update()
    {
        if (!AssessHand())
        {
            if (!actor.acting && !actor.beingHealed)
            {
                AIThoughtLatency = 1 / actor.GetComponent<Stats>()[StatTypes.WIS];
                PlanOfAttack plan = Evaluate();
                if (plan.fireLocations.Count > 0 && plan.ability != null && plan.moveLocation == actor.tile.pos)
                {
                    StartCoroutine(AIAttackSequence(plan));
                }
                else
                {
                    StartCoroutine(AIMoveSequence(plan));
                }
            }
        }
    }
    public PlanOfAttack Evaluate()
    {
        // Create an empty plan of Attack to fill in
        PlanOfAttack poa = new PlanOfAttack();
        poa.fireLocations = new List<Tile>();
        poa.moveLocation = actor.tile.pos;
        // Step 1: Decide what ability to use with a pattern if available. Redraw hand if unusable.

        AttackPattern pattern = actor.GetComponentInChildren<AttackPattern>();
        if (pattern)
            pattern.Pick(poa);

        List<Tile> moveOptions = GetMoveOptions();
        moveOptions.Add(actor.tile);
        Tile startTile = actor.tile;
        Dictionary<Tile, AttackOption> map = new Dictionary<Tile, AttackOption>();

        for (int i = 0; i < moveOptions.Count; ++i)
        {
            Tile moveTile = moveOptions[i];
            actor.Place(moveTile);
            AttackOption ao = null;
            if (map.ContainsKey(moveTile))
            {
                ao = map[moveTile];
            }
            else
            {
                ao = new AttackOption();
                map[moveTile] = ao;
                RateFireLocation(poa, ao);
            }
            ao.AddMoveTarget(moveTile);
        }

        actor.Place(startTile);
        List<AttackOption> list = new List<AttackOption>(map.Values);
        PickBestOption(poa, list);

        // Step 2: Determine where to move and aim to best use the ability
        if (poa.ability == null)
            MoveTowardOpponent(poa);
        else if (poa.ability.GetComponent<BaseAbilityEffect>() is HealAbilityEffect && poa.moveLocation == actor.tile.pos)
        {
            foreach (Tile t in poa.fireLocations)
            {
                if (t.content != null)
                    t.content.GetComponent<Unit>().beingHealed = true;
            }
        }

        // Return the completed plan
        return poa;
    }

    List<Tile> GetMoveOptions()
    {
        return actor.GetComponent<Movement>().GetTilesInRange(owner.board);
    }

    bool AssessHand()
    {
        bool AllSameCard = true;
        bool HealInHand = false;
        bool DamagedAllies = false;

        Card comp = actor.hand.Count > 0 ? actor.hand[0] : null;
        foreach (Card c in actor.hand)
        {
            if (c.GetName() != comp.GetName())
                AllSameCard = false;
            if (c.GetComponent<BaseAbilityEffect>() is HealAbilityEffect)
                HealInHand = true;
        }

        foreach (Unit u in owner.enemyUnits)
            if (u.GetComponent<Alliance>().IsMatch(actor.GetComponent<Alliance>(), Targets.Ally) && !u.GetComponent<Health>().FullHP)
                DamagedAllies = true;

        if (AllSameCard && ((DamagedAllies && !HealInHand) || (HealInHand && !DamagedAllies)) && !actor.acting)
        {
            actor.acting = true;
            AIThoughtLatency = 1 / actor.GetComponent<Stats>()[StatTypes.WIS];
            StartCoroutine(AIReloadHandSequence());
            return true;
        }
        return false;
    }

    public IEnumerator AIMoveSequence(PlanOfAttack attack)
    {
        Tile location = owner.board.GetTile(attack.moveLocation);
        if (location != null)
        {
            actor.acting = true;
            yield return StartCoroutine(actor.GetComponent<Movement>().Traverse(location));
            actor.acting = false;
        }
    }

    public IEnumerator AIAttackSequence(PlanOfAttack attack)
    {
        actor.acting = true;
        if (attack.ability.GetComponent<AbilityManaCost>().amount <= actor.GetComponent<Mana>().MP)
        {
            List<Tile> targets = attack.fireLocations;
            attack.ability.Perform(targets);
            yield return StartCoroutine(AIReloadCardSequence(attack.ability));
        }
        yield return new WaitForSeconds(1 - (actor.GetComponent<Stats>()[StatTypes.DEX] * .02f));
        yield return new WaitForSeconds(AIThoughtLatency);
        actor.acting = false;
    }

    public IEnumerator AIReloadCardSequence(Card c)
    {
        if (actor.hand.Contains(c))
        {
            int index = actor.hand.IndexOf(c);
            actor.discards.Add(c);
            if (actor.deck.Count <= 0)
                yield return StartCoroutine(AIReloadDeckSequence());
            actor.hand[index] = actor.deck.Draw();
        }
        else if (c == null)
        {
            for (int i = 0; i < actor.hand.Count; i++)
            {
                if (actor.hand[i] == null)
                {
                    if (actor.deck.Count <= 0)
                        yield return StartCoroutine(AIReloadDeckSequence());
                    actor.hand[i] = actor.deck.Draw();
                    break;
                }
            }
        }
    }

    public IEnumerator AIReloadHandSequence()
    {
        actor.acting = true;
        yield return new WaitForSeconds(1 - (actor.GetComponent<Stats>()[StatTypes.DEX] * .02f));
        for (int i = 0; i < actor.hand.Count; i++)
            yield return AIReloadCardSequence(actor.hand[i]);
        yield return new WaitForSeconds(AIThoughtLatency);
        actor.acting = false;
    }

    public IEnumerator AIReloadDeckSequence()
    {
        actor.deck = actor.discards;
        actor.deck.Shuffle();
        actor.discards = new AbilityDeck(new List<Card>());
        yield return new WaitForSeconds(1 - (actor.GetComponent<Stats>()[StatTypes.DEX] * .02f));
    }

    void RateFireLocation(PlanOfAttack poa, AttackOption option)
    {
        if (poa.ability != null)
        {
            AbilityRange range = poa.ability.GetComponent<AbilityRange>();
            List<Tile> tiles = range.GetTilesInRange(owner.board);
            option.targets = tiles;
            for (int i = 0; i < tiles.Count; ++i)
            {
                Tile tile = tiles[i];
                if (!poa.ability.GetComponent<AbilityEffectTarget>().IsTarget(tile))
                    continue;

                bool isMatch = IsAbilityTargetMatch(poa, tile);
                if (isMatch && poa.ability.GetComponent<BaseAbilityEffect>() is HealAbilityEffect)
                {
                    if (tile.content.GetComponent<Health>().FullHP)
                    {
                        option.AddMark(tile, false);
                        option.AddMark(tile, false);
                    }
                    else
                        option.AddMark(tile, isMatch);
                }
                option.AddMark(tile, isMatch);
            }
        }
    }

    bool IsAbilityTargetMatch(PlanOfAttack poa, Tile tile)
    {
        bool isMatch = false;
        if (poa.target == Targets.Tile)
            isMatch = true;
        else if (poa.target != Targets.None)
        {
            Alliance other = tile.content.GetComponentInChildren<Alliance>();
            if (other != null && alliance.IsMatch(other, poa.target))
                isMatch = true;
        }
        return isMatch;
    }

    void PickBestOption(PlanOfAttack poa, List<AttackOption> list)
    {
        int bestScore = 1;
        List<AttackOption> bestOptions = new List<AttackOption>();
        for (int i = 0; i < list.Count; ++i)
        {
            AttackOption option = list[i];
            int score = option.GetScore(actor, poa.ability);
            if (score > bestScore)
            {
                bestScore = score;
                bestOptions.Clear();
                bestOptions.Add(option);
            }
            else if (score == bestScore)
            {
                bestOptions.Add(option);
            }
        }
        if (bestOptions.Count == 0)
        {
            poa.ability = null; // Clear ability as a sign not to perform it
            return;
        }
        List<AttackOption> finalPicks = new List<AttackOption>();
        bestScore = 0;
        for (int i = 0; i < bestOptions.Count; ++i)
        {
            AttackOption option = bestOptions[i];
            int score = option.bestScore;
            if (score > bestScore)
            {
                bestScore = score;
                finalPicks.Clear();
                finalPicks.Add(option);
            }
            else if (score == bestScore)
            {
                finalPicks.Add(option);
            }
        }

        AttackOption choice = finalPicks[UnityEngine.Random.Range(0, finalPicks.Count)];
        poa.fireLocations = choice.targets;
        poa.moveLocation = choice.moveTargets[choice.moveTargets.Count - 1].pos;
    }

    void FindNearestFoe()
    {
        nearestFoe = null;
        owner.board.Search(actor.tile, delegate (Tile arg1, Tile arg2)
        {
            if (nearestFoe == null && arg2.content != null)
            {
                Alliance other = arg2.content.GetComponentInChildren<Alliance>();
                if (other != null && alliance.IsMatch(other, Targets.Foe))
                {
                    Unit unit = other.GetComponent<Unit>();
                    Stats stats = unit.GetComponent<Stats>();
                    if (stats[StatTypes.HP] > 0)
                    {
                        nearestFoe = unit;
                        return true;
                    }
                }
            }
            return nearestFoe == null;
        });
    }

    void MoveTowardOpponent(PlanOfAttack poa)
    {
        List<Tile> moveOptions = GetMoveOptions();
        FindNearestFoe();
        if (nearestFoe != null)
        {
            Tile toCheck = nearestFoe.tile;
            while (toCheck != null)
            {
                if (moveOptions.Contains(toCheck))
                {
                    poa.moveLocation = toCheck.pos;
                    return;
                }
                toCheck = toCheck.prev;
            }
        }
        poa.moveLocation = actor.tile.pos;
    }
}