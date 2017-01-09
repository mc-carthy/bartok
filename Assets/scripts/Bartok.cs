﻿using UnityEngine;
using System.Collections.Generic;

public class Bartok : MonoBehaviour {

    static public Bartok S;

    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;

    public bool ___________________;

    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> discardPile;

    public BartokLayout layout;
    public Transform layoutAnchor;

    private void Awake ()
    {
        S = this;
    }

    private void Start ()
    {
        deck = GetComponent<Deck>();
        deck.InitDeck (deckXML.text);
        Deck.Shuffle (ref deck.cards);

        layout = GetComponent<BartokLayout> ();
        layout.ReadLayout (layoutXML.text);

        drawPile = UpgradeCardsList (deck.cards);
    }

    // UpgradeCardsList casts the Cards in lCD to be CardBartoks
    // Of course, they were all along, but this lets Unity know
    private List<CardBartok> UpgradeCardsList (List<Card> lCD)
    {
        List<CardBartok> lCB = new List<CardBartok> ();
        foreach (Card tCD in lCD)
        {
            lCB.Add (tCD as CardBartok);
        }
        return lCB;
    }

}
