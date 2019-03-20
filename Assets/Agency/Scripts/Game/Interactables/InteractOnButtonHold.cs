using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
	Interaction after holding a button
 */
public class InteractOnButtonHold : Interactable
{
	public string button_name = "Manipulate";

	[Tooltip("The button must be held this long, in seconds, before the event is triggered")]
	protected float time_to_hold = 0.8f; // Seconds

	[Tooltip("Graphics for filling the hold progress")]
	protected Image progress_image;

	private float time_elapsed; // Counter for the progress


	void Start()
	{
		this.description = "[G] Manipulate";
		base.Start();
	}


	void Update()
	{
		base.Update();
		if (AllowInteraction() && Input.GetButton(button_name))
		{
			if (FinishProgress())
			{
				ExecuteInteraction();
			}
		}
		else
		{
			ResetProgress();
		}
	}


	private bool FinishProgress()
	{
		// Increase the progress
		time_elapsed += Time.deltaTime;

		// Update the image
		//progress_image.gameObject.SetActive(true);
		//progress_image.fillAmount = time_elapsed / time_to_hold;

		
		Debug.Log("Progress " + (time_elapsed / time_to_hold));

		// Evaluate
		if (time_elapsed > time_to_hold)
		{
			ResetProgress();
			return true;
		}
		return false;
	}


	private void ResetProgress()
	{
		time_elapsed = 0f;
		//progress_image.gameObject.SetActive(false);
	}
}
