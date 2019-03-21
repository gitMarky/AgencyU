using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPickup : InteractOnButtonPress
{
	public string button_name = "Pickup";
	public string description = "[F] Pick up";


	protected virtual void AssignCollider()
	{
		if (null == collider)
		{
			SphereCollider sphere = this.gameObject.AddComponent<SphereCollider>();
			sphere.radius = 1.5f;
			collider = sphere;
		}
	}
}

