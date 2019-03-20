using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/**
	Description for an interaction that is shown
	on the interactable object.
 */
public class InteractionDescription : MonoBehaviour
{
	public Text _interaction_text;


	// Start is called before the first frame update
	void Start()
	{
		//_interaction_text = Instantiate(_interaction_text, GameObject.Find("InteractionCanvas").transform);
		//_interaction_text = new Text();
		_interaction_text.text = "My cool custom boogy-woogy text";
	}

	// Update is called once per frame
	void Update()
	{

	}
}
