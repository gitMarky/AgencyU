using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionController : MonoBehaviour
{
	[Tooltip("Lists the interactions")]
	public /* readonly*/ List<Interactable> interactions = new List<Interactable>();
	public /* readonly*/ Dictionary<InteractionType, List<Interactable>> sorted = new Dictionary<InteractionType, List<Interactable>>();


	/**
		Registers an interaction opportunity.
	 */
	public void RegisterInteractable(Interactable item)
	{
		bool valid = item != null;
		Debug.Assert(valid);
		if (valid && !interactions.Contains(item))
		{
			interactions.Add(item);
		}
	}

	/**
		Removes an interaction opportunity.
	 */
	public void RemoveInteractable(Interactable item)
	{
		bool valid = item != null;
		Debug.Assert(valid);
		if (valid && interactions.Contains(item))
		{
			interactions.Remove(item);
		}
	}


	public void UpdateInteractions()
	{
		sorted.Clear();

		foreach (InteractionType type in Enum.GetValues(typeof(InteractionType)))
		{
			sorted.Add(type, new List<Interactable>());
		}

		foreach (Interactable item in interactions)
		{
			InteractionDescription data = item.GetInteractionData();
			if (data != null)
			{
				// TODO: This is risky, because it can add data to an unused empty list
				GetInteractions(data.GetInteractionType()).Add(item);
				item.UpdateTextPosition();
			}
		}
	}


	public List<Interactable> GetInteractions(InteractionType type)
	{
		List<Interactable> list;
		return sorted.TryGetValue(type, out list) ? list : new List<Interactable>();
	}
}

