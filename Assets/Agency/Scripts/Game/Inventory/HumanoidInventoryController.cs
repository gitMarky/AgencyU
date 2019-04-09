﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidInventoryController : Inventory
{
	// This item is held in the right hand
	private PickupInteraction main_hand_item;

	// This item is held in the left hand
	private PickupInteraction off_hand_item;

	// This item is carried on the back
	private PickupInteraction back_item;

	// This item is selected for holstering/drawing.
	// This is also the item for the main hand,
	// which may be confusing (and maybe I find a better solution)
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
		if (main_hand_item != null)
		{
			DropItem(main_hand_item, true);
			main_hand_item = null;
			return;
		}
	}

#endregion

#region Holstering

	private enum HolsterResult { Success, Blocked, Invalid }

	private void ExecuteHolster()
	{
		// Holster types:
		// - holster right hand item to inventory
		// - holster right hand item to back
		// - draw back item
		// - draw current item
		// - (not holstering): Drop left hand item

		// Put away the main hand item?
		if (DoHolsterMainHand() != HolsterResult.Invalid)
		{
			return;
		}
		if (DoUnholsterInventoryItem() != HolsterResult.Invalid)
		{
			return;
		}

/* 
		// Structure is not good yet here...
		if (TryHolster(main_hand_item))
		{
			main_hand_item = null; // Right hand is empty :)
		}
		else if (TryDraw(inventory_item))
		{
			main_hand_item = inventory_item;
		}
*/
	}

	private HolsterResult DoHolsterMainHand()
	{
		if (main_hand_item == null || main_hand_item.IsHolstered())
		{
			return HolsterResult.Invalid;
		}
		switch (main_hand_item.GetHolsterType())
		{
			case HolsterType.Stashable:
				Debug.Log("Stash to inventory"); // + item.gameObject.Name);
				// Item is holstered
				main_hand_item.SetHolstered(true);
				main_hand_item.SetVisible(false);
				// Item is selected
				inventory_item = main_hand_item;
				// Hand is empty now, but do not detach
				main_hand_item = null;
				return HolsterResult.Success;
			case HolsterType.Sling:
				if (back_item == null)
				{
					Debug.Log("Holster to back not supported yet");
					return HolsterResult.Success;
				}
				else
				{
					// Do not holster, but also do not do anything else
					return HolsterResult.Blocked;
				}
			case HolsterType.CarryOnly:
				// Do not holster, but also do not do anything else
				return HolsterResult.Blocked;
			default:
				Debug.Log("Unsupported");
				break;
		}
		return HolsterResult.Invalid;
	}

	private HolsterResult DoUnholsterInventoryItem()
	{
		if (inventory_item == null || !inventory_item.IsHolstered())
		{
			return HolsterResult.Invalid;
		}
		if (main_hand_item != null)
		{
			Debug.Log("Unsupported: Stash holstered item first");
			return HolsterResult.Blocked;
		}
		// Put item in main hand
		main_hand_item = inventory_item;
		// Unholster
		main_hand_item.SetHolstered(false);
		main_hand_item.SetVisible(true);
		// We are good!
		return HolsterResult.Success;
	}


	private bool TryDraw(PickupInteraction item)
	{
		if (item != null && item.IsHolstered())
		{
			//HolsterRightHand();
			if (main_hand_item == null)
			{
				// Item status
				item.SetHolstered(false);
				item.gameObject.SetActive(true);
				// Inventory status
				main_hand_item = item;
				inventory_item = item;
				return true;
			}
		}
		return false;
	}

#endregion

#region Getters
	
	public PickupInteraction GetItemInRightHand()
	{
		return main_hand_item;
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
		//HolsterRightHand();
		// Hand is free, pick it up
		if (main_hand_item == null)
		{
			Debug.Log("Picked up to right hand"); // + item.gameObject.Name);
			AddItem(item);
			main_hand_item = item;
			inventory_item = item;
			AttachToRightHand(item);
			return true;
		}

		return false;
	}


	/* private void HolsterRightHand()
	{
		if (TryHolster(main_hand_item))
		{
			main_hand_item = null;
		}
	}*/

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