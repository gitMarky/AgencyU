using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/**
	Defines how the player inventory works.
 */
[RequireComponent(typeof(HumanoidInventoryController))]
[RequireComponent(typeof(InteractionController))]
public class PlayerController : MonoBehaviour
{
	/** Force for throwing an object as a distraction (underhand swing), in N. */
	private const float THROW_DISTRACTION_FORCE = 5.0f;

	/** Force for throwing an object as an attack (like a ball), in N. */
	private const float THROW_ATTACK_FORCE = 20.0f;

	/** Reference throwing mass, in kg. */
	private const float THROW_REFERENCE_MASS = 1.0f;

	public LineRenderer trajectory_renderer;

#region UnityCallbacks

	void Update()
	{
		// Add some delay
		/*if (button_cooldown > 0.0f)
		{
			button_cooldown -= Time.deltaTime;
			return;
		}*/


		// Handle possible interactions first
		InteractionController interactions = this.gameObject.GetComponent<InteractionController>();
		interactions.UpdateInteractions();
		foreach (InteractionType type in Enum.GetValues(typeof(InteractionType)))
		{
			if (GetButtonDown(type))
			{
				Debug.Log("Checking interaction: " + type);
				foreach (Interactable obj in interactions.GetInteractions(type))
				{
					Debug.Log(">   " + obj.gameObject.name);
					if (obj.RequestInteraction(this.gameObject))
					{
						Debug.Log("Interacting with: " + obj);
						return;
					}
				}
			}
		}

		// Handle inventory stuff
		HumanoidInventoryController inventory = this.gameObject.GetComponent<HumanoidInventoryController>();
		if (GetButtonDown(InteractionType.Place))
		{
			inventory.ExecutePlace();
			return;
		}
		if (GetButtonDown(InteractionType.Holster))
		{
			inventory.ExecuteHolster();
			return;
		}
		if (GetButtonDown(InteractionType.Aim))
		{
			// TODO
		}
		if (CanThrow(inventory.GetItemInHands()))
		{
			if (GetButtonUp(InteractionType.Throw))
			{
				DoThrowExecute(inventory.GetItemInHands(), inventory);
			}
			else if (GetButtonPressed(InteractionType.Throw))
			{
				trajectory_renderer.enabled = DoThrowAiming(inventory.GetItemInHands());
				if (trajectory_renderer.enabled)
				{
					return;
				}
			}
		}
	}
#endregion

#region Internals

	private bool GetButtonDown(InteractionType type)
	{
		return Input.GetButtonDown(ButtonFor(type)); // Lazy method
	}

	private bool GetButtonUp(InteractionType type)
	{
		return Input.GetButtonUp(ButtonFor(type)); // Lazy method
	}

	private bool GetButtonPressed(InteractionType type)
	{
		return Input.GetButton(ButtonFor(type));
	}
	private string ButtonFor(InteractionType type)
	 {
		 switch (type)
		 {
    		case InteractionType.Use:			return "Interaction";
			case InteractionType.Pickup:		return "Pickup";
    		case InteractionType.Distraction:	return "Distraction";
			case InteractionType.Place:			return "Place";
			case InteractionType.Holster:		return "Holster";
			case InteractionType.Aim:		return "Aim";
			case InteractionType.Throw:			return "Throw";
			default:
				//Debug.Log("Unhandled");
				return "Cancel";
		 }
	 }

#endregion
#region Throwing Stuff

	private bool CanThrow(PickupInteraction item)
	{
		// Cannot throw?
		if (item == null
		||  item.IsHolstered()
		|| !item.IsVisible()
		||  item.GetComponent<Rigidbody>() == null) // Throwing makes sense for physical objects only
		{
			Debug.Log("Throw aiming not possible");
			return false;
		}

		// TODO: Make a throwable property and check that, too
		return true;
	}

	private Vector3 DoThrowGetAngle()
	{
		return gameObject.transform.forward;
	}

	private void DoThrowExecute(PickupInteraction item, HumanoidInventoryController inventory)
	{
		if (CanThrow(item))
		{
			inventory.DropItem(item, true); // TODO: The item should know by itself whether it is attached or not

			// Rigidbody is ensured by CanThrow
			Rigidbody body = item.GetComponent<Rigidbody>();
			body.AddForce(DoThrowGetAngle() * THROW_DISTRACTION_FORCE, ForceMode.Impulse);
		}
	}

	private bool DoThrowAiming(PickupInteraction item)
	{
		if (CanThrow(item))
		{
			// Rigidbody is ensured by CanThrow
			Rigidbody body = item.GetComponent<Rigidbody>();
			float mass_item = Mathf.Max(THROW_REFERENCE_MASS, body.mass);
			Vector3 start = body.position;

			// Do the trajectory!
			float v0 = THROW_DISTRACTION_FORCE / mass_item;
			float time_step = 0.1f; // Simulate every 100ms of flight
			DrawAimingTrajectory(start, DoThrowGetAngle() * v0, time_step);
			return true;
		}
		else
		{
			return false;
		}
	}


	/**
		Draws a trajectory for an item.
		@param start the starting position
		@param v0 the launch velocity
		@param time_step the time between two simulation steps,
		                 affects the accuracy of the hit detection.
						 Measured in seconds.
		@return the stopping position;
	 */
	private Vector3 DrawAimingTrajectory(Vector3 start, Vector3 v0, float time_step)
	{
		Debug.Log("Draw trajectory: " + start + ", V = " + v0);
		// Anything that flies longer than 20 seconds makes no sense, really
		float max_time = 20.0f;
		// Initialize positions
    	List<Vector3> positions = new List<Vector3>();
		Vector3 pos_cur = start;
		Vector3 pos_end = start;
		for (float time = 0.0f; time < max_time; time += time_step)
		{
			pos_end = start + time * (v0 + 0.5f * Physics.gravity * time);
			positions.Add(pos_end);
			if (Physics.Linecast(pos_cur, pos_end))
			{
				break;
			}
			Debug.DrawLine(pos_cur, pos_end, Color.red, 0.1f);
			pos_cur = pos_end;
		}
		//Gizmos.color = Color.red;
		//Gizmos.DrawWireSphere(pos_end, 0.05f);
		Debug.Log("Draw trajectory: " + start + ", V = " + v0 + ", End = " + pos_end + ", Player = " + gameObject.transform.position);
		trajectory_renderer.positionCount = positions.Count;
		for (int i = 0; i < positions.Count; ++i)
		{
			trajectory_renderer.SetPosition(i, positions[i]);
		}
		return pos_end;
	}
#endregion
}
