using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 Base class for any sort of inventory.
 */
public class Inventory : MonoBehaviour
{
	public List<PickupInteraction> items = new List<PickupInteraction>();


	public void AddItem(PickupInteraction item)
	{
		items.Add(item);
	}


	public void RemoveItem(PickupInteraction item)
	{
		items.Remove(item);
	}
}
