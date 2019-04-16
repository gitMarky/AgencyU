using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 Defines how an item can be holstered.
 */
public enum ItemSize
{
	/** Can be stashed to the inventory. */
	Small,
	/** Can be holstered to the back. */
	Large,
	/** Can be carried only. */
	Bulky,
	/** Can be carried as a suitcase. */
	Case
}
