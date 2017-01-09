using UnityEngine;
using System.Collections.Generic;

// CBState includes both states for the game and to___ states for movement
public enum CBState {
    drawpile,
    toHand,
    hand,
    toTarget,
    target,
    discard,
    to,
    idle
}

// CardBartok extends Card
public class CardBartok : Card {

    // These static fields are used to set values that 
    // will be the same for all instances of CardBartok
    static public float MOVE_DURATION = 0.5f;
    static public string MOVE_EASING = Easing.InOut;
    static public float CARD_HEIGHT = 3.5f;
    static public float CARD_WIDTH = 2f;

    public CBState state = CBState.drawpile;

    // Fields to store info the card will use to move and rotate
    public List<Vector3> bezierPts;
    public List<Quaternion> bezierRots;
    public float timeStart;
    public float timeDuration;

    public int eventualSortOrder;
    public string eventualSortLayer;

    // When the card is done moving, it will call reportFinishTo.SendMessage ()
    public GameObject reportFinishTo = null;
    public Player callbackPlayer = null;

    private void Awake ()
    {
        callbackPlayer = null;
        Debug.Log (transform.position);
    }

    // MoveTo tells the card to interpolate to a new position and rotation
    public void MoveTo (Vector3 ePos, Quaternion eRot)
    {
        // Make new interpolation lists for the card
        // Position and rotation will each have only two points
        bezierPts = new List<Vector3> ();
        // Current position
        bezierPts.Add (transform.localPosition);
        // New position
        bezierPts.Add (ePos);

        bezierRots = new List<Quaternion> ();
        // Current rotation
        bezierRots.Add (transform.localRotation);
        // New rotation
        bezierRots.Add (eRot);

        // If timeStart is 0, then it's set to start immediately,
        // otherwise, it stats at timeStart. This wat, if timeStart is
        // already set, it won't be overwritten
        if (timeStart == 0)
        {
            timeStart = Time.time;
        }
        // timeDuration always starts the same but can be altered later
        timeDuration = MOVE_DURATION;

        // Setting state to either toHand or toTarget 
        // will be handled by the calling method
        state = CBState.to;
    }

    // This overload of MoveTo doesn't require a rotation argument
    public void MoveTo (Vector3 ePos)
    {
        MoveTo (ePos, Quaternion.identity);
    }

    private void Update ()
    {
        switch (state)
        {
            // All the to___ states are ones where the card is interpolating
            case (CBState.toHand):
            case (CBState.toTarget):
            case (CBState.to):
                // Get u from current time and duration
                // u ranges from 0 to 1
                float u = (Time.time - timeStart) / timeDuration;

                // Use Easing class from Utils to curve the u value
                float uC = Easing.Ease (u, MOVE_EASING);

                // If u < 0, we shouldn't move yet
                if (uC < 0)
                {
                    // Stay at the initial position
                    transform.localPosition = bezierPts [0];
                    transform.rotation = bezierRots [0];
                    return;
                }
                // If u >= 1, we're finished moving
                else if (u >= 1)
                {
                    uC = 1;
                    // Move from the to__ state to the following state
                    if (state == CBState.toHand)
                    {
                        state = CBState.hand;
                    }
                    if (state == CBState.toTarget)
                    {
                        state = CBState.target;
                    }
                    if (state == CBState.to)
                    {
                        state = CBState.idle;
                    }
                    // Move to the final position
                    transform.localPosition = bezierPts [bezierPts.Count - 1];
                    transform.rotation = bezierRots [bezierPts.Count - 1];

                    // Reset timeStart to 0 so it gets overwritten next time
                    timeStart = 0;

                    // If there's a callback GameObject
                    if (reportFinishTo != null)
                    {
                        // Then use SendMessage to call the CBCallback 
                        // method with this as the parameter
                        reportFinishTo.SendMessage ("CBCallback", this);
                        // After calling SendMessage (), reportFinishTo must be set
                        // to null so that the card doesn't continue to report
                        // to the same GameObject every subsequent time it moves
                        reportFinishTo = null;
                    }
                    // If there is nothing to callback
                    else
                    {
                        // Do nothing
                    }
                }
                // 0 <= u < 1, means that this is interpolating now
                else
                {
                    // Use Bézier curve to move this to the right point
                    Vector3 pos = Utils.Bezier (uC, bezierPts);
                    transform.localPosition = pos;
                    Quaternion rotQ = Utils.Bezier (uC, bezierRots);
                    transform.rotation = rotQ;

                    if (u > 0.5f && spriteRenderers [0].sortingOrder != eventualSortOrder)
                    {
                        // Jump to the proper sort order
                        SetSortOrder (eventualSortOrder);
                    }
                    if (u > 0.75f && spriteRenderers [0].sortingLayerName != eventualSortLayer)
                    {
                        // Jump to the proper sort order
                        SetSortingLayerName (eventualSortLayer);
                    }


                }
                break;
        }
    }

}
