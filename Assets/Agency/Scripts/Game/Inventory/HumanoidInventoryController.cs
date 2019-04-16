using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidInventoryController : Inventory
{
	private float button_cooldown = 0.0f;

	// This item is held in the right hand
	private PickupInteraction in_hand;

	// This item is held in the left hand
	private PickupInteraction suitcase;

	// This item is carried on the back
	private PickupInteraction on_back;

	// This item is selected for holstering/drawing.
	// This is also the item for the main hand,
	// which may be confusing (and maybe I find a better solution)
	private PickupInteraction active_item;

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
		if (in_hand != null)
		{
			DropItem(in_hand, true);
			in_hand = null;
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
		if (in_hand == null || in_hand.IsHolstered())
		{
			return HolsterResult.Invalid;
		}
		switch (in_hand.GetItemSize())
		{
			case ItemSize.Small:
				Debug.Log("Stash to inventory"); // + item.gameObject.Name);
				// Item is holstered
				in_hand.SetHolstered(true);
				in_hand.SetVisible(false);
				// Item is selected
				active_item = in_hand;
				// Hand is empty now, but do not detach
				in_hand = null;
				return HolsterResult.Success;
			case ItemSize.Large:
				if (on_back == null)
				{
					Debug.Log("Holster to back");
					// Item is holstered
					in_hand.SetHolstered(true);
					in_hand.SetVisible(true);
					// Item is selected
					on_back = in_hand;
					// Hand is empty now, but do not detach
					in_hand = null;
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
		if (active_item == null || !active_item.IsHolstered())
		{
			return HolsterResult.Invalid;
		}
		if (in_hand != null)
		{
			Debug.Log("Unsupported: Stash holstered item first");
			return HolsterResult.Blocked;
		}
		// Put item in main hand
		in_hand = active_item;
		// Unholster
		in_hand.SetHolstered(false);
		in_hand.SetVisible(true);
		// We are good!
		return HolsterResult.Success;
	}


	private bool TryDraw(PickupInteraction item)
	{
		if (item != null && item.IsHolstered())
		{
			//HolsterRightHand();
			if (in_hand == null)
			{
				// Item status
				item.SetHolstered(false);
				item.gameObject.SetActive(true);
				// Inventory status
				in_hand = item;
				active_item = item;
				return true;
			}
		}
		return false;
	}

#endregion

#region Getters
	
	public PickupInteraction GetItemInRightHand()
	{
		return in_hand;
	}

	public PickupInteraction GetItemInSelection()
	{
		return active_item;
	}

#endregion

#region PickupLogic

	public void Pickup(PickupInteraction item)
	{
		/*
		if (item == in_hand
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
		if (in_hand == null)
		{
			// Pick it up
			Debug.Log("Unsupported: Pick up main hand item");
		}
		else
		{
			PickupSwap(item, in_hand);
		}
	}

	private void PickupHolsterable(PickupInteraction item)
	{
		DoButtonCooldown();
		
		// Hand is free, pick it up
		if (in_hand == null)
		{
			Debug.Log("Picked up to right hand"); // + item.gameObject.Name);
			AddItem(item);
			in_hand = item;
			active_item = item;
			AttachToRightHand(item);
			return;	
		}
		// Hand is occuppied? Pick it up to the inventory
		else if (in_hand.GetItemSize() == ItemSize.Large)
		{
			Debug.Log("Unsupported: Pick up to inventory"); // + item.gameObject.Name);
			AddItem(item);
			// Item is holstered
			item.SetHolstered(true);
			item.SetVisible(false);
			// Item is not selected and not in hand
			return;
		}
		// Holster the main hand item, pick up new item
		else
		{
			Debug.Log("Unsupported: Holster, pick up new item");
			if (DoHolsterMainHand() == HolsterResult.Success)
			{
				// Same as initial result. TODO Change code structure?
				AddItem(item);
				in_hand = item;
				active_item = item;
				AttachToRightHand(item);
			}
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
