using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 Defines how you can interact with an item.
 Sorted by priority.
 TODO: Define priority.
 */
public enum InteractionType
{
    /** [E] Use: Use, dump body, etc. */
    Use, 
	/** [F] Pick up, swap, etc. */
	Pickup,
    /** [G] Manipulate */
    Distraction,
    /** [B] Drag, drop, etc. */
    Body,
    /** [Z] Retrieve item */
    Case,
    /** [T] Disguise */
    Disguise,
    /** [P] Place */
    Place,
    /** [H] Holster */
    Holster, 
    /** [Mouse 2] Aim */
    Aim,
    /** [Mouse 2] Throw */
    Throw,
}

