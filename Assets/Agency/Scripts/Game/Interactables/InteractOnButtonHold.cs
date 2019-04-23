using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
	Interaction after holding a button
 */
public class InteractOnButtonHold : Interactable
{
	/* [Range(0.0f, 1.0f)]
	[Tooltip("The button must be held this long, in seconds, before the event is triggered")]
	public float time_to_hold = 0.0f; // Seconds

	[Tooltip("Graphics for filling the hold progress")]
	protected Image progress_image;

	private float time_elapsed; // Counter for the progress


	protected override void Update()
	{
		base.Update();
		if (AllowInteraction(player) && Input.GetButton(GetInteractionData().GetButtonName()))
		{
			if (FinishProgress())
			{
				ExecuteInteraction(player.gameObject);
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
	}*/
}
