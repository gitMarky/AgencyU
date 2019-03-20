using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
	Interaction when pressing a button
 */
public class InteractOnButtonPress : Interactable
{
	public string button_name = "Interact";


	void Start()
	{
		this.description = "[E] Use"; // TODO
		base.Start();
	}


	void Update()
	{
		base.Update();
		if (AllowInteraction() && Input.GetButtonDown(button_name))
		{
			ExecuteInteraction();
		}
	}
}

