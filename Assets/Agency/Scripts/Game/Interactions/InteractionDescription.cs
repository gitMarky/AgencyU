﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


/**
	Description for an interaction that is shown
	on the interactable object.
 */
public class InteractionDescription : MonoBehaviour
{
	/** Description for the interaction. */
	public string description;

	/** This event is called, by default, on interaction. */
	public UnityEvent interaction_event;

	/** This button needs to be pressed for the interaction to take place. */
	public string button;


	/* --- Unity Callbacks --- */

	public virtual void Reset()
	{
		button = "Interact";
		description = "";
	}


	/* --- Custom interface --- */

	/**
	 Some of this does not make much sense, 
	 given that the field is public - but,
	 I prefer function access when I do not
	 want to change the values. 
	 */
	

	/** Calls the interaction event. */
	public virtual void DoInteraction(GameObject user)
	{
		interaction_event.Invoke();
	}


	/** Gets the button name (the name of the control). */
	public string GetButtonName()
	{
		return button;
	}


	/** Gets a formatted description of what the button does. */
	public string GetDescription()
	{
		return "[" + button + "] " + description;
	}
}