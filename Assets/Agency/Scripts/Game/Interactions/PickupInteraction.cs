using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : InteractionDescription
{
	[Tooltip("The object attaches at this position when held, relative to the parent transform. Optional, for when you have no transform.")]
	public Vector3 attachment_position;
	public Vector3 attachment_rotation;

	[Tooltip("The object attaches at this position when held, overrides position and rotation")]
	public Transform attachment_transform;


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
				inventory.Pickup(gameObject);
			}
			//gameObject.transform.SetParent(user.transform);
		}
		// TODO: Add to inventory
		//Destroy(gameObject);
	}


	public void AttachTo(Transform parent)
	{
		gameObject.transform.SetParent(parent);

		if (attachment_transform == null)
		{
			gameObject.transform.localPosition = attachment_position;
			gameObject.transform.localEulerAngles = attachment_rotation;
		}
		else
		{
			gameObject.transform.localPosition = attachment_transform.localPosition;
			gameObject.transform.localEulerAngles = attachment_transform.localEulerAngles;
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
		Gizmos.DrawSphere(transform.position + attachment_position, 0.01f);
	}
}

