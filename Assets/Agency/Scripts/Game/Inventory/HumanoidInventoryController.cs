using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidInventoryController : Inventory
{
	// This item is held in the right hand
	private PickupInteraction right_hand_item;

	// This item is held in the left hand
	private PickupInteraction left_hand_item;

	// This item is carried on the back
	private PickupInteraction back_item;

	// This item is selected for holstering/drawing.
	private PickupInteraction inventory_item;

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
		// Holster types:
		// - holster right hand item to inventory
		// - holster right hand item to back
		// - draw back item
		// - draw current item
		// - (not holstering): Drop left hand item

		// Structure is not good yet here...
		if (TryHolster(right_hand_item))
		{
			right_hand_item = null; // Right hand is empty :)
		}
		else if (TryDraw(inventory_item))
		{
			right_hand_item = inventory_item;
		}
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


	private bool TryDraw(PickupInteraction item)
	{
		if (item != null && item.IsHolstered())
		{
			HolsterRightHand();
			if (right_hand_item == null)
			{
				// Item status
				item.SetHolstered(false);
				item.gameObject.SetActive(true);
				// Inventory status
				right_hand_item = item;
				inventory_item = item;
				return true;
			}
		}
		return false;
	}

	private bool StashToInventory(PickupInteraction item)
	{
		if (item != null)
		{
			Debug.Log("Stash to inventory"); // + item.gameObject.Name);
			// Item status
			item.SetHolstered(true);
			item.gameObject.SetActive(false);
			// Inventory status
			inventory_item = item;
			return true;
		}
		return false;
	}

#endregion

#region Getters
	
	public PickupInteraction GetItemInRightHand()
	{
		return right_hand_item;
	}

	public PickupInteraction GetItemInSelection()
	{
		return inventory_item;
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
		// Holster the right hand item, if possible
		HolsterRightHand();
		// Hand is free, pick it up
		if (right_hand_item == null)
		{
			Debug.Log("Picked up to right hand"); // + item.gameObject.Name);
			AddItem(item);
			right_hand_item = item;
			inventory_item = item;
			AttachToRightHand(item);
			return true;
		}

		return false;
	}


	private void HolsterRightHand()
	{
		if (TryHolster(right_hand_item))
		{
			right_hand_item = null;
		}
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