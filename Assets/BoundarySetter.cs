using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundarySetter : MonoBehaviour
{
	[SerializeField]
	private float boundaryThichness = 1;
	[SerializeField] private BoxCollider2D[] boundaryColliders;

	public void SetBoundaries(Vector2 fieldPos, Vector2 fieldSize)
	{
		var leftBoundaryPos = new Vector3(fieldPos.x - (fieldSize.x / 2) - (boundaryThichness / 2), fieldPos.y);
		var rightBoundaryPos = new Vector3(fieldPos.x + (fieldSize.x / 2) + (boundaryThichness / 2), fieldPos.y);
		var topBoundaryPos = new Vector3(fieldPos.x, fieldPos.y + (fieldSize.y / 2) + (boundaryThichness / 2));
		var botBoundaryPos = new Vector3(fieldPos.x, fieldPos.y - (fieldSize.y / 2) - (boundaryThichness / 2));
		
		var verticalBoundarySize = new Vector2(boundaryThichness, fieldSize.y + (boundaryThichness * 2));
		var horizontalBoundarySize = new Vector2(fieldSize.x, boundaryThichness);
		
		
		boundaryColliders[0].transform.position = leftBoundaryPos;
		boundaryColliders[0].size = verticalBoundarySize;
		
		boundaryColliders[1].transform.position = rightBoundaryPos;
		boundaryColliders[1].size = verticalBoundarySize;

		boundaryColliders[2].transform.position = topBoundaryPos;
		boundaryColliders[2].size = horizontalBoundarySize;

		boundaryColliders[3].transform.position = botBoundaryPos;
		boundaryColliders[3].size = horizontalBoundarySize;

	}
}
