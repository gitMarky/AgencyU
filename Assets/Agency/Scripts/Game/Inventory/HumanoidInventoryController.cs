using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
	Defines how the inventory of humanoids works.
 */
public class HumanoidInventoryController : Inventory
{
	protected float button_cooldown = 0.0f;

	// This item is held in the right hand
	private PickupInteraction in_hands;

	// This item is held in the left hand
	private PickupInteraction suitcase;

	// This item is carried on the back
	private PickupInteraction on_back;

	// This item is selected for holstering/drawing.
	// This is also the item for the main hand,
	// which may be confusing (and maybe I find a better solution)
	private PickupInteraction active_item;

#region Placing

	protected void ExecutePlace()
	{
		if (in_hands != null)
		{
			DropItem(in_hands, true);
			in_hands = null;
			return;
		}
	}

#endregion

#region Holstering

	private enum HolsterResult { Success, Blocked, Invalid }

	protected void ExecuteHolster()
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
		if (in_hands == null || in_hands.IsHolstered())
		{
			return HolsterResult.Invalid;
		}
		switch (in_hands.GetItemSize())
		{
			case ItemSize.Small:
				Debug.Log("Stash to inventory: " + in_hands.gameObject.name);
				// Item is holstered and hidden
				in_hands.SetHolstered(true);
				in_hands.SetVisible(false);
				// Item is selected
				active_item = in_hands;
				AttachToRightHand(in_hands);
				// Hand is empty now, but do not detach
				in_hands = null;
				return HolsterResult.Success;
			case ItemSize.Large:
				if (on_back == null)
				{
					Debug.Log("Holster to back: " + in_hands.gameObject.name);
					// Item is holstered, but visible
					in_hands.SetHolstered(true);
					in_hands.SetVisible(true);
					// Item is on back
					on_back = in_hands;
					AttachToBack(in_hands);
					// Hand is empty now, but do not detach
					in_hands = null;
					return HolsterResult.Success;
				}
				else
				{
					// Do not holster, but also do not do anything else
					return HolsterResult.Blocked;
				}
			case ItemSize.Bulky:
			case ItemSize.Case:
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
		// No item, or already unholstered? Cancel right now.
		if (active_item == null || !active_item.IsHolstered())
		{
			return HolsterResult.Invalid;
		}
		// Hand is already occupied?
		if (in_hands != null)
		{
			Debug.Log("Unsupported: Stash holstered item first");
			return HolsterResult.Blocked;
		}
		// Put item in main hand
		in_hands = active_item;
		// Unholster
		in_hands.SetHolstered(false);
		in_hands.SetVisible(true);
		AttachToRightHand(in_hands);
		// We are good!
		return HolsterResult.Success;
	}

#endregion

#region Getters
	
	public PickupInteraction GetItemInHands()
	{
		return in_hands;
	}

	public PickupInteraction GetItemInSelection()
	{
		return active_item;
	}

	public PickupInteraction GetItemOnBack()
	{
		return on_back;
	}

#endregion

#region PickupLogic

	public void Pickup(PickupInteraction item)
	{
		/*
		if (item == in_hands
		||  item == suitcase
		||  item == on_back
		||  item == active_item
		||  HasI)*/
		if (item == null)
		{
			return;
		}
		if (HasItem(item))
		{
			return;
		}

		switch (item.GetItemSize())
		{
			case ItemSize.Bulky:
				PickupCarryOnly(item);
				return;
			case ItemSize.Small:
			case ItemSize.Large:
				PickupHolsterable(item);
				return;
			default:
				Debug.Log("This should be impossible");
				break;
		}
	}

	private void PickupCarryOnly(PickupInteraction item)
	{
		if (in_hands == null)
		{
			// Pick it up
			AddToHand(item);
		}
		else
		{
			PickupSwap(item, in_hands);
		}
	}

	private void PickupHolsterable(PickupInteraction item)
	{
		DoButtonCooldown();

		// Picking up a large item, while carrying a suitcase?
		if (suitcase != null && item.GetItemSize() == ItemSize.Large)
		{
			// Store to back if the slot is free?
			if (on_back == null)
			{
				AddToBack(item);
				return;
			}
			// TODO: No idea whether suit and bulky item will be dropped...
			// Cancel for now
			return;
		}

		// Already holding something?
		if (in_hands != null)
		{
			if (in_hands.GetItemSize() == ItemSize.Large || in_hands.GetItemSize() == ItemSize.Bulky)
			{
				switch (item.GetItemSize())
				{
					case ItemSize.Large:
						if (on_back == null)
						{
							AddToBack(item);
						}
						else
						{
							PickupSwap(item, on_back);
						}
						break;
					case ItemSize.Bulky:
						PickupSwap(item, in_hands);
						break;
					case ItemSize.Small:
						AddToInventory(item);
						break;
					default:
						Debug.Log("This code should not be reached");
						break;
				}
				return;
			}
			else if (in_hands.GetItemSize() == ItemSize.Small)
			{
				DoHolsterMainHand();
			}
		}
		
		// Hand is free, pick it up
		if (in_hands == null)
		{
			AddToHand(item);
			return;	
		}
	}

	private void AddToHand(PickupInteraction item)
	{
		// Save to inventory
		Debug.Log("Picked up to hand:" + item.gameObject.name);
		AddItem(item);
		// Item is selected and in the hand now
		in_hands = item;
		active_item = item;
		AttachToRightHand(item);
		// Make the item visible
		item.SetHolstered(false);
		item.SetVisible(true);
	}

	private void AddToInventory(PickupInteraction item)
	{
		// Save to inventory
		Debug.Log("Picked up to inventory:" + item.gameObject.name);
		AddItem(item);
		// Item is neither selected nor in the hand, 
		// but the attachment needs to be done anyway
		AttachToRightHand(item);
		// Make the item invisible
		item.SetHolstered(true);
		item.SetVisible(false);
	}

	private void AddToBack(PickupInteraction item)
	{
		// Save to inventory
		Debug.Log("Picked up to back:" + item.gameObject.name);
		AddItem(item);
		// Item is neither selected nor in the hand, 
		// but the attachment needs to be done anyway
		on_back = item;
		AttachToBack(item);
		// Make the item holstered, but visible
		item.SetHolstered(true);
		item.SetVisible(true);
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

	private void AttachToBack(PickupInteraction item)
	{
		HumanoidAttachment attach_manager = GetComponent<HumanoidAttachment>();

		if (attach_manager != null && item != null)
		{
			item.AttachTo(attach_manager.attach_back);
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

	private void PickupSwap(PickupInteraction item_new, PickupInteraction item_old)
	{
		Debug.Log("Swap not supported yet");
	}
#endregion

#region Misc

	private void DoButtonCooldown()
	{
		button_cooldown = 0.1f;
	}

#endregion
}
