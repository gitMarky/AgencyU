using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidInventoryController : MonoBehaviour
{
	// This item is held in the right hand
	private GameObject right_hand_item;

	// This item is held in the left hand
	private GameObject left_hand_item;

	// This item is carried on the back
	private GameObject back_item;

	void Update()
	{
		if (Input.GetButton("Place"))
		{
			if (right_hand_item != null)
			{
				DropItem(right_hand_item, true);
				right_hand_item = null;
				return;
			}
		}
	}

	public GameObject GetItemInRightHand()
	{
		return right_hand_item;
	}

	public void Pickup(GameObject item)
	{
		if (PickupRightHand(item))
		{
			return;
		}
	}

	public bool PickupRightHand(GameObject item)
	{
		// Hand is free, pick it up
		if (GetItemInRightHand() == null)
		{
			right_hand_item = item;
			AttachToRightHand(item);
		}
		else // If the item can be stashed, do that
		{
			// TODO!
		}

		return false;
	}

	private void AttachToRightHand(GameObject item)
	{
		HumanoidAttachment attach_manager = GetComponent<HumanoidAttachment>();
		PickupInteraction pickup = item.GetComponent<PickupInteraction>();

		if (attach_manager != null && pickup != null)
		{
			pickup.AttachTo(attach_manager.attach_right_hand);
		}
	}


	private void DropItem(GameObject item, bool is_attached)
	{
		PickupInteraction pickup = item.GetComponent<PickupInteraction>();

		if (pickup != null)
		{
			pickup.Detach();
		}
	}
}
