using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 Defines how an item can be holstered.
 */
public enum HolsterType
{
	/** Can be stashed to the inventory. */
	Stashable,
	/** Can be holstered to the back. */
	Sling,
	/** Can be carried only. */
	CarryOnly
}

