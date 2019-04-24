using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
	Defines how the player inventory works.
 */
[RequireComponent(typeof(HumanoidInventoryController))]
[RequireComponent(typeof(InteractionController))]
public class PlayerController : MonoBehaviour
{
#region UnityCallbacks
	void Update()
	{
		// Add some delay
		/*if (button_cooldown > 0.0f)
		{
			button_cooldown -= Time.deltaTime;
			return;
		}*/


		// Handle possible interactions first
		InteractionController interactions = this.gameObject.GetComponent<InteractionController>();
		interactions.UpdateInteractions();
		foreach (InteractionType type in Enum.GetValues(typeof(InteractionType)))
		{
			if (GetButtonDown(type))
			{
				Debug.Log("Checking interaction: " + type);
				foreach (Interactable obj in interactions.GetInteractions(type))
				{
					Debug.Log(">   " + obj.gameObject.name);
					if (obj.RequestInteraction(this.gameObject))
					{
						Debug.Log("Interacting with: " + obj);
						return;
					}
				}
			}
		}

		// Handle inventory stuff
		HumanoidInventoryController inventory = this.gameObject.GetComponent<HumanoidInventoryController>();
		if (GetButtonDown(InteractionType.Place))
		{
			inventory.ExecutePlace();
		}
		else if (GetButtonDown(InteractionType.Holster))
		{
			inventory.ExecuteHolster();
		}
	}
#endregion

#region Internals

	private bool GetButtonDown(InteractionType type)
	{
		return Input.GetButtonDown(ButtonFor(type)); // Lazy method
	}
	private string ButtonFor(InteractionType type)
	 {
		 switch (type)
		 {
    		case InteractionType.Use:			return "Interaction";
			case InteractionType.Pickup:		return "Pickup";
    		case InteractionType.Distraction:	return "Distraction";
			case InteractionType.Place:			return "Place";
			case InteractionType.Holster:		return "Holster";
			default:
				//Debug.Log("Unhandled");
				return "Cancel";
		 }
	 }

#endregion
}
