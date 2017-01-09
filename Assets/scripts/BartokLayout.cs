using UnityEngine;
using System.Collections.Generic;

// The SlotDef class is not a subclass of MonoBehaviour,
// so it does not need a separate C# file

// This makes SlotDefs visible in the Unity Inspector pane
[System.SerializableAttribute]
public class SlotDef {
    public float x;
    public float y;
    public bool faceUp = false;
    public string layerName = "Default";
    public int layerID = 0;
    public int id;
    public float rot;
    public string type = "slot";
    public Vector2 stagger;
    public int player;
    public Vector3 pos;
}

public class BartokLayout : MonoBehaviour {

    // Just like Deck, this has a PT_XMLReader
	public PT_XMLReader xmlr;
    // This variable is for eaasier xml access
    public PT_XMLHashtable xml;
    // Sets the spacing of the tableau
    public Vector2 multiplier;

    // SlotDef references
    // The SlotDefs hands
    public List<SlotDef> slotDefs;
    public SlotDef drawPile;
    public SlotDef discardPile;
    public SlotDef target;


    // This function is called to read in the LayoutXML.xml file
    public void ReadLayout (string xmlText)
    {
        xmlr = new PT_XMLReader ();
        // The XML is parsed
        xmlr.Parse (xmlText);
        // And xml is set as a shortcut to the XML
        xml = xmlr.xml ["xml"][0];

        // Read in the multiplier, which sets card spacing
        multiplier.x = float.Parse (xml ["multiplier"][0].att ("x"));
        multiplier.y = float.Parse (xml ["multiplier"][0].att ("y"));

        // Read in the slots
        SlotDef tSD;
        // slotsX is used as a shortcut to al the <slot>s
        PT_XMLHashList slotsX = xml ["slot"];

        for (int i = 0; i < slotsX.Count; i++)
        {
            // Create a new SlotDef instance
            tSD = new SlotDef ();
            // If this <slot> has a type attribute, parse it
            if (slotsX [i].HasAtt ("type"))
            {
                tSD.type = slotsX [i].att ("type");
            }
            // If not, set its type to "slot"; it's a tableau card
            else
            {
                tSD.type = "slot";
            }

            // Various attributes are parsed into numerical values
            tSD.x = float.Parse (slotsX [i].att ("x"));
            tSD.y = float.Parse (slotsX [i].att ("y"));
            tSD.pos = new Vector3 (tSD.x * multiplier.x, tSD.y  *multiplier.y, 0);

            // Sorting layers
            tSD.layerID = int.Parse (slotsX [i].att ("layer"));
            // The Sorting Layers are named 1, 2, 3 through to 10
            // This converts the number of the layerID into a text layerName
            tSD.layerName = tSD.layerID.ToString ();
            // The layers are used to make sure that the correct cards are
            // on top of the othrs. In Unity2D, all of the assets are
            // effectively at the same Z depth, so the layer is used
            // to differentiate between them.

            switch (tSD.type)
            {
                // Pull additional attributes based on the type of this <slot>
                case ("slot"):
                    // Ignore slots that are just of the "slot" type
                    break;
                case ("drawpile"):
                    tSD.stagger.x = float.Parse (slotsX [i].att ("xstagger"));
                    drawPile = tSD;
                    break;
                case ("discardpile"):
                    discardPile = tSD;
                    break;
                case ("target"):
                    // The target card has a different layer from discardPile
                    target = tSD;
                    break;
                case ("hand"):
                    // Information for each players hand
                    tSD.player = int.Parse (slotsX [i].att ("player"));
                    tSD.rot = float.Parse (slotsX [i].att ("rot"));
                    break;
            }
        }
    }

}
