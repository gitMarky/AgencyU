using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : InteractionDescription
{
	[Tooltip("The object attaches at this position when held, relative to the parent transform. Optional, for when you have no transform.")]
	public Vector3 carry_position;
	public Vector3 carry_rotation;

	[Tooltip("The object attaches at this position when held, overrides position and rotation")]
	public Transform carry_transform;


	[Tooltip("Defines the size of the item in the inventory")]
	public ItemSize item_size;

	private bool is_holstered = false;


	public override void Reset()
	{
		this.description = "Pick up";
		this.button = "Pickup";
	}


	public override void DoInteraction(GameObject user)
	{
		base.DoInteraction(user);

		if (user != null)
		{
			HumanoidInventoryController inventory = user.GetComponent<HumanoidInventoryController>();
			if (inventory != null)
			{
				inventory.Pickup(this);
			}
		}
	}

#region Holstering

	public void SetHolstered(bool holstered)
	{
		is_holstered = holstered;
	}

	public bool IsHolstered()
	{
		return is_holstered;
	}

#endregion

#region  Visibility

	public void SetVisible(bool visible)
	{
		// TODO: Should be the model
		gameObject.SetActive(visible);
	}
	public bool IsVisible()
	{
		// TODO: Should be the model
		return gameObject.activeSelf;
	}

#endregion

#region Getters

	public ItemSize GetItemSize()
	{
		return item_size;
	}

#endregion

#region Behavior
	public void AttachTo(Transform parent)
	{
		gameObject.transform.SetParent(parent);

		if (carry_transform == null)
		{
			gameObject.transform.localPosition = carry_position;
			gameObject.transform.localEulerAngles = carry_rotation;
		}
		else
		{
			gameObject.transform.localPosition = carry_transform.localPosition;
			gameObject.transform.localEulerAngles = carry_transform.localEulerAngles;
		}

		SetPhysicsActive(false);
	}


	public void Detach()
	{
		gameObject.transform.SetParent(null);

		SetPhysicsActive(true);
	}


	private void SetPhysicsActive(bool active)
	{
		Rigidbody physics = this.GetComponent<Rigidbody>();
		if (null != physics)
		{
			physics.isKinematic = !active;
		}
	}


	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position + carry_position, 0.05f);
	}

#endregion
}

