using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidInventoryController : Inventory
{
	private float button_cooldown = 0.0f;

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
		switch (DoHolsterMainHand())
		{
			case HolsterResult.Success:
				DoButtonCooldown();
				return;
			case HolsterResult.Blocked:
				// Stop, but allow dropping the item, etc.
				return;
			case HolsterResult.Invalid:
			default:
				// Try something else!
				break;
		}
		switch (DoUnholsterInventoryItem())
		{
			case HolsterResult.Success:
				DoButtonCooldown();
				return;
			case HolsterResult.Blocked:
				// Stop, but allow dropping the item, etc.
				return;
			case HolsterResult.Invalid:
			default:
				// Try something else!
				break;
		}
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
		switch (item.GetCarryType())
		{
			case CarryType.Main_Hand:
				// Hand is free, pick it up
				if (main_hand_item == null)
				{
					Debug.Log("Picked up to right hand"); // + item.gameObject.Name);
					AddItem(item);
					main_hand_item = item;
					inventory_item = item;
					AttachToRightHand(item);
					return;
				}
				break;
			default:
				break;
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
			// Not in the inventory anymore
			RemoveItem(item);
			// Make the item visible
			item.SetHolstered(false);
			item.SetVisible(true);
			// Drop it!
			item.Detach();
		}
	}
#endregion

#region Misc

	private void DoButtonCooldown()
	{
		button_cooldown = 0.1f;
	}

#endregion
}
