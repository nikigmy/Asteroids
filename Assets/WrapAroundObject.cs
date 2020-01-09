using System;
using UnityEngine;

public class WrapAroundObject : MonoBehaviour
{

	public delegate void TeleportDelegate();

	public event TeleportDelegate OnBeginTeleport;
	public event TeleportDelegate OnTeleport;
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.CompareTag("Boundary"))
		{
			CheckForTeleport();
		}
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Boundary"))
		{
			CheckForTeleport();
		}
	}

	private void CheckForTeleport()
	{
		var pos = transform.position;
            
		if (Math.Abs(GameManager.instance.LevelManager.FieldPosition.x - pos.x) > GameManager.instance.LevelManager.FieldSize.x / 2)
		{
			pos.x -= GameManager.instance.LevelManager.FieldSize.x * (pos.x / Math.Abs(pos.x));
		}
            
		if (Math.Abs(GameManager.instance.LevelManager.FieldPosition.y - pos.y) > GameManager.instance.LevelManager.FieldSize.y / 2)
		{
			pos.y -= GameManager.instance.LevelManager.FieldSize.y * (pos.y / Math.Abs(pos.y));
		}

		if (pos != transform.position)
		{
			if (OnBeginTeleport != null)
			{
				OnBeginTeleport();
			}
			transform.position = pos;
			
			if (OnTeleport != null)
			{
				OnTeleport();
			}
		}
	}
}
