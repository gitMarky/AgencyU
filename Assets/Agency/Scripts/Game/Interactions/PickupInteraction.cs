using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : InteractionDescription
{
	public override void Reset()
	{
		this.description = "Pick up";
		this.button = "Pickup";
	}


	public override void DoInteraction(GameObject user)
	{
		base.DoInteraction(user);

		if (null != user)
		{
			gameObject.transform.SetParent(user.transform);
		}
		// TODO: Add to inventory
		//Destroy(gameObject);
	}
}

