using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
	Interaction when pressing a button
 */
public class InteractOnButton : Interactable
{
	public string button_name = "Interact"; // TODO: Make configurable by menu
	public UnityEvent button_event;


	void Update()
	{
		if (is_near && Input.GetButtonDown(button_name))
		{
			button_event.Invoke();
			Debug.Log("Interaction");
		}
	}
}

