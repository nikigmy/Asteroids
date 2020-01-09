using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSize : MonoBehaviour {

	
	private const float k_desiredFrustrumWidth = 1.0f;
	[ContextMenu("SetSize")]
	// Use this for initialization
	public void Apply ()
	{
		var cam = GetComponent<Camera>();
		cam.orthographicSize = (k_desiredFrustrumWidth / ((float)Screen.width / (float)Screen.height) / 2.0f);
	}
}
