using System.Collections.Generic;
using System.Linq;

public enum PlayerType {
    human,
    ai
}

// Make the player visible in the Inspector pane
[System.SerializableAttribute]
public class Player {

	public PlayerType type = PlayerType.ai;
    public int playerNum;

    // The cards in the player's hand
    public List<CardBartok> hand;

    public SlotDef handSlotDef;

    // Add a card to the hand
    public CardBartok AddCard (CardBartok eCB)
    {
        if (hand == null)
        {
            hand = new List<CardBartok> ();
        }

        // Add the card to the hand
        hand.Add (eCB);
        return eCB;
    }

    // Remove a card from the hand
    public CardBartok RemoveCard (CardBartok cB)
    {
        hand.Remove (cB);
        return cB;
    }

}
