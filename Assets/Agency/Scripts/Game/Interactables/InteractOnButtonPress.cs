using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
	Interaction when pressing a button
 */
public class InteractOnButtonPress : Interactable
{
	protected override void Update()
	{
		base.Update();
		if (AllowInteraction() && Input.GetButtonDown(GetInteractionData().GetButtonName()))
		{
			ExecuteInteraction();
		}
	}
}

