using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidInventoryController : MonoBehaviour
{
	// This item is held in the right hand
	private PickupInteraction right_hand_item;

	// This item is held in the left hand
	private PickupInteraction left_hand_item;

	// This item is carried on the back
	private PickupInteraction back_item;

#region UnityCallbacks
	void Update()
	{
		if (Input.GetButton("Place"))
		{
			ExecutePlace();
		}
		else if (Input.GetButton("Holster"))
		{
			ExecuteHolster();
		}
	}
#endregion

#region Placing

	private void ExecutePlace()
	{
		if (right_hand_item != null)
		{
			DropItem(right_hand_item, true);
			right_hand_item = null;
			return;
		}
	}

#endregion

#region Holstering

	private void ExecuteHolster()
	{
		TryHolster(right_hand_item);
	}

	private bool TryHolster(PickupInteraction item)
	{
		if (item == null)
		{
			return false;
		}
		switch (item.GetHolsterType())
		{
			case HolsterType.Stashable:
				if (item.IsHolstered())
				{
					Debug.Log("Unsupported");
				}
				else
				{
					return StashToInventory(item);
				}
				break;
			default:
				Debug.Log("Unsupported");
				break;
		}
		return false;
	}

	private bool StashToInventory(PickupInteraction item)
	{
		if (item != null)
		{
			item.SetHolstered(true);
			item.gameObject.SetActive(false);
		}
		return false;
	}

#endregion

#region Getters
	
	public PickupInteraction GetItemInRightHand()
	{
		return right_hand_item;
	}

#endregion

#region PickupLogic

	public void Pickup(PickupInteraction item)
	{
		if (PickupRightHand(item))
		{
			return;
		}
	}

	public bool PickupRightHand(PickupInteraction item)
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

#endregion

#region  MeshLogic
	private void AttachToRightHand(PickupInteraction item)
	{
		HumanoidAttachment attach_manager = GetComponent<HumanoidAttachment>();

		if (attach_manager != null && item != null)
		{
			item.AttachTo(attach_manager.attach_right_hand);
		}
	}


	private void DropItem(PickupInteraction item, bool is_attached)
	{
		if (item != null)
		{
			item.Detach();
		}
	}
}

#endregion