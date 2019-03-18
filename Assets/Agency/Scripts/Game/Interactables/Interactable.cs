using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
	Allows setting up relevant layers that can
	interact with this object.
 */
[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
	public LayerMask relevant_layers;
	new Collider collider;
	protected bool is_near;

	/* --- Unity Callbacks (?) --- */

	void Reset()
	{
		relevant_layers = LayerMask.NameToLayer("Everything");
		collider = GetComponent<Collider>();
		collider.isTrigger = true;
	}


	void OnTriggerEnter(Collider user)
	{
		if (IsInAppropriateLayer(user))
		{
			is_near = true;
			ExecuteOnEnter(user);
		}
	}


	void OnTriggerExit(Collider user)
	{
		if (IsInAppropriateLayer(user))
		{
			is_near = false;
			ExecuteOnExit(user);
		}
	}


	/* --- Custom interface --- */


	protected virtual void ExecuteOnEnter(Collider user)
	{
		// Does nothing by default
		Debug.Log("On enter");
	}


	protected virtual void ExecuteOnExit(Collider user)
	{
		// Does nothing by default
		Debug.Log("On exit");
	}


	/* --- Gizmos --- */


	void OnDrawGizmos()
	{
		Gizmos.DrawIcon(transform.position, "InteractionTrigger", false);
	}


	void OnDrawGizmosSelected()
	{
		// Need to inspect events and draw arrows to relevant gameObjects.
	}


	/* --- Internals --- */


	private bool IsInAppropriateLayer(Collider user)
	{
		return 0 != (relevant_layers.value & 1 << user.gameObject.layer);
	}
} 

