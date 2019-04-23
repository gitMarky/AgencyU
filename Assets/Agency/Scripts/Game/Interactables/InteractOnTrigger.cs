using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
	Interaction when entering or leaving.
 */
public class InteractOnTrigger : Interactable
{
	public UnityEvent trigger_on_enter;
	public UnityEvent trigger_on_exit;

	/* --- Custom interface --- */


	protected override void ExecuteOnEnter(GameObject user)
	{
		trigger_on_enter.Invoke();
	}


	protected override void ExecuteOnExit(GameObject user)
	{
		trigger_on_exit.Invoke();
	}
} 

