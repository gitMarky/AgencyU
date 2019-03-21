using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/**
	Allows setting up relevant layers that can
	interact with this object.
 */
[RequireComponent(typeof(InteractionDescription))]
public class Interactable : MonoBehaviour
{
	public LayerMask relevant_layers;

	public InteractionDescription interaction_data;

	public Collider is_near_collider;
	protected bool is_near;


	protected Text interaction_description; // TODO: This may go to a different behavior?

	/* --- Unity Callbacks (?) --- */

	protected void Reset()
	{
		relevant_layers = LayerMask.NameToLayer("Everything");
	}

	protected void Start()
	{
		InitCollider();
		InitInteractionData();
		interaction_description = CreateText(GameObject.Find("InteractionCanvas"));
	}


	protected virtual void Update()
	{
		UpdateTextPosition();
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

	public InteractionDescription GetInteractionData()
	{
		return this.interaction_data;
	}

	/* --- Custom callbacks --- */


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


	protected virtual void ExecuteInteraction()
	{
		Debug.Log("Interaction");
		GetInteractionData().DoInteraction();
	}


	protected virtual void AssignCollider()
	{
		if (null == is_near_collider)
		{
			is_near_collider = GetComponent<Collider>();
		}
	}


	protected void InitCollider()
	{
		AssignCollider();
		is_near_collider.isTrigger = true;
	}


	protected void InitInteractionData()
	{
		if (null == interaction_data)
		{
			interaction_data = this.gameObject.GetComponent<InteractionDescription>();
		}
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


	protected bool AllowInteraction()
	{
		return is_near;
	}


	private Text CreateText(GameObject canvas)
	{
		GameObject new_text = new GameObject("InteractionDescription");
		new_text.transform.SetParent(canvas.transform);

		Text text = new_text.AddComponent<Text>();
		if (null == GetInteractionData())
		{
			text.text = "ERROR: UNKNOWN TEXT";
		}
		else
		{
			text.text = GetInteractionData().GetDescription();
		}
		text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		text.fontSize = 20;
		//text.enabled = true;
		//text.color = text_color;

		return text;
	}


	protected void UpdateTextPosition()
	{
		if (null != interaction_description)
		{
			// TODO: WorldToScreenPoint should be cached, because it eats performance for every object that does that
			interaction_description.transform.position = Camera.main.WorldToScreenPoint(this.transform.position);
		}
	}
} 

