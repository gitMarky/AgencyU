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

		HumanoidInventoryController inventory = this.gameObject.GetComponent<HumanoidInventoryController>();
		InteractionController interactions = this.gameObject.GetComponent<InteractionController>();

		interactions.UpdateInteractions();


		if (Input.GetButtonDown("Place"))
		{
			inventory.ExecutePlace();
		}
		else if (Input.GetButtonDown("Holster"))
		{
			inventory.ExecuteHolster();
		}
	}
#endregion
}
