using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
	Defines how the player inventory works.
 */
public class PlayerInventoryController : HumanoidInventoryController
{
#region UnityCallbacks
	void Update()
	{
		// Add some delay
		if (button_cooldown > 0.0f)
		{
			button_cooldown -= Time.deltaTime;
			return;
		}

		if (Input.GetButtonDown("Place"))
		{
			ExecutePlace();
		}
		else if (Input.GetButtonDown("Holster"))
		{
			ExecuteHolster();
		}
	}
#endregion
}
