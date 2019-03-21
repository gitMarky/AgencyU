using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPickup : InteractOnButtonPress
{
	protected override void AssignCollider()
	{
		if (null == is_near_collider)
		{
			SphereCollider sphere = this.gameObject.AddComponent<SphereCollider>();
			sphere.radius = 1.5f;
			is_near_collider = sphere;
		}
	}
}

