using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : InteractionDescription
{
	public void Reset()
	{
		this.description = "Pick up";
		this.button = "Pickup";
	}


	public override void DoInteraction()
	{
		base.DoInteraction();
		// TODO: Add to inventory
		Destroy(gameObject);
	}
}

