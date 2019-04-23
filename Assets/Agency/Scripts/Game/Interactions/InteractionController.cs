using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
		Trace.Assert(valid);
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
		Trace.Assert(valid);
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
		sorted.TryGetValue(type, out list);
		// Return an empty list?
		if (list == null)
		{
			Trace.Assert(false, "List is null");
			return new List<Interactable>();
		}
		else
		{
			return list;
		}
	}
}

