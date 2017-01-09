using UnityEngine;
using System.Collections.Generic;

public class Bartok : MonoBehaviour {

    static public Bartok S;

    public TextAsset deckXML;
    public TextAsset layoutXML;
    public Vector3 layoutCenter = Vector3.zero;

    // The number of degrees to fan each card in a hand
    public float handFanDegrees = 10f;

    public bool ___________________;

    public Deck deck;
    public List<CardBartok> drawPile;
    public List<CardBartok> discardPile;

    public BartokLayout layout;
    public Transform layoutAnchor;
    public List<Player> players;
    public CardBartok targetCard;

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
        LayoutGame ();
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

    // Position all of the cards in the drawPile properly
    public void ArrangeDrawPile ()
    {
        CardBartok tCB;

        for (int i = 0; i < drawPile.Count; i++)
        {
            tCB = drawPile [i];
            tCB.transform.parent = layoutAnchor;
            tCB.transform.localPosition = layout.drawPile.pos;
            // Rotation should start at 0
            tCB.faceUp = false;
            tCB.SetSortingLayerName (layout.drawPile.layerName);
            // Order them front to back
            tCB.SetSortOrder (-i * 4);
            tCB.state = CBState.drawpile;
        }
    }

    // Perform the initial game layout
    private void LayoutGame ()
    {
        // Create an empty GameObject to serve as the anchor for the tableau
        if (layoutAnchor == null)
        {
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        // Position the drawPile cards
        ArrangeDrawPile ();

        // Set up the players
        Player p1;
        players = new List<Player> ();

        foreach (SlotDef tSD in layout.slotDefs)
        {
            p1 = new Player ();
            p1.handSlotDef = tSD;
            players.Add (p1);
            p1.playerNum = players.Count;
        }
        // Make the 0th player human
        players [0].type = PlayerType.human;
    }

    // The Draw function will pull a single card from the drawPile return it
    public CardBartok Draw ()
    {
        // Pull the top card from the drawPile
        CardBartok cd = drawPile [0];
        // Remove it from the List<> drawPile
        drawPile.RemoveAt (0);
        // Return the card
        return cd;
    }

    // This Update method is used to test adding cards to players' hands
    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Alpha1))
        {
            players [0].AddCard (Draw ());
        }
        if (Input.GetKeyDown (KeyCode.Alpha2))
        {
            players [1].AddCard (Draw ());
        }
        if (Input.GetKeyDown (KeyCode.Alpha3))
        {
            players [2].AddCard (Draw ());
        }
        if (Input.GetKeyDown (KeyCode.Alpha4))
        {
            players [3].AddCard (Draw ());
        }
    }

}
