using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : InteractionDescription
{
	[Tooltip("The object attaches at this position when held, relative to the parent transform.")]
	public Vector3 attachment_position;
	public Vector3 attachment_rotation;


	public override void Reset()
	{
		this.description = "Pick up";
		this.button = "Pickup";
	}


	public override void DoInteraction(GameObject user)
	{
		base.DoInteraction(user);

		if (null != user)
		{
			HumanoidAttachment attach_manager = user.GetComponent<HumanoidAttachment>();
			if (null != attach_manager)
			{
				gameObject.transform.SetParent(attach_manager.attach_right_hand);
				gameObject.transform.localPosition = attachment_position;
				gameObject.transform.localEulerAngles = attachment_rotation;

				Rigidbody physics = this.GetComponent<Rigidbody>();
				if (null != physics)
				{
					physics.isKinematic = true;
				}
			}
			//gameObject.transform.SetParent(user.transform);
		}
		// TODO: Add to inventory
		//Destroy(gameObject);
	}


	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position + attachment_position, 0.01f);
	}
}

