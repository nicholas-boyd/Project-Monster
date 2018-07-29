using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {
	public Tile tile { get; protected set; }
    public AbilityDeck deck;
    public AbilityDeck discards;
    public List<Card> hand;
    public int handSize = 4;
    public Directions dir;
    public bool acting;
    public bool beingHealed;
    public int DeckCount;
    public int flashCount;

    void Start()
    {
        acting = false;
        flashCount = 4;
    }

    void OnEnable()
    {
        this.AddObserver(OnHit, DamageAbilityEffect.DamageDealtNotification);
        this.AddObserver(OnHeal, HealAbilityEffect.UnitHealedNotification);
    }

    void OnDisable()
    {
        this.RemoveObserver(OnHit, DamageAbilityEffect.DamageDealtNotification);
        this.RemoveObserver(OnHeal, HealAbilityEffect.UnitHealedNotification);
    }

    void OnHit(object sender, object target)
    {
        Info<Unit, Unit, int> info = target as Info<Unit, Unit, int>;
        if (info.arg1.Equals(this))
            StartCoroutine(Flash(Color.red));
    }

    void OnHeal(object sender, object target)
    {
        Info<Unit, Unit, int> info = target as Info<Unit, Unit, int>;
        if (info.arg1.Equals(this))
            StartCoroutine(Flash(Color.green));
    }

    IEnumerator Flash(Color color)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        for (int i = 0; i < flashCount; i++)
        {
            sprite.color = color;
            yield return new WaitForSeconds(0.06f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.06f);
        }
    }

    void Update()
    {
        DeckCount = deck.Count;
    }

    public void Place (Tile target) {
		// Make sure old tile location is not still pointing to this unit
		if (tile != null && tile.content == gameObject)
			tile.content = null;

		// Link unit and tile references
		tile = target;

		if (target != null)
			target.content = gameObject;
	}

	public void Match () {
        transform.localPosition = new Vector3(tile.center.x, tile.center.y+GetComponent<SpriteRenderer>().bounds.size.y/2, tile.center.z);
		transform.localEulerAngles = dir.ToEuler();
	}
}