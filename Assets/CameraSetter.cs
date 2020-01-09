using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
	[SerializeField] private float cameraZOffset = -10;
	[SerializeField] private Camera[] cameras;

	public void PositionCameras(Vector2 fieldPos, Vector2 fieldSize)
	{
		//left
		cameras[0].transform.position = new Vector3(fieldPos.x - fieldSize.x, fieldPos.y, cameraZOffset);
		//leftUp
		cameras[1].transform.position = new Vector3(fieldPos.x - fieldSize.x, fieldPos.y + fieldSize.y, cameraZOffset);
		//up
		cameras[2].transform.position = new Vector3(fieldPos.x, fieldPos.y + fieldSize.y, cameraZOffset);
		//rightUp
		cameras[3].transform.position = new Vector3(fieldPos.x + fieldSize.x, fieldPos.y + fieldSize.y, cameraZOffset);
		//right
		cameras[4].transform.position = new Vector3(fieldPos.x + fieldSize.x, fieldPos.y, cameraZOffset);
		//rightBot
		cameras[5].transform.position = new Vector3(fieldPos.x + fieldSize.x, fieldPos.y - fieldSize.y, cameraZOffset);
		//bot
		cameras[6].transform.position = new Vector3(fieldPos.x, fieldPos.y - fieldSize.y, cameraZOffset);
		//leftBot
		cameras[7].transform.position = new Vector3(fieldPos.x - fieldSize.x, fieldPos.y - fieldSize.y, cameraZOffset);
	}
}
