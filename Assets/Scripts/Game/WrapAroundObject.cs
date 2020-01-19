using System;
using Managers;
using UnityEngine;
using Utils;

namespace Game
{
    /// <summary>
    /// Handles the teleportation of objects when they exit the game view
    /// </summary>
    public class WrapAroundObject : MonoBehaviour
    {
        public event EventHandler<EventArgs> OnBeginTeleport;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag(Constants.Tags.boundary)) CheckForTeleport();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Constants.Tags.boundary)) CheckForTeleport();
        }

        private void CheckForTeleport()
        {
            var pos = transform.position;

            if (Math.Abs(GameManager.instance.LevelManager.FieldPosition.x - pos.x) >
                GameManager.instance.LevelManager.FieldSize.x / 2)
                pos.x -= GameManager.instance.LevelManager.FieldSize.x * (pos.x / Math.Abs(pos.x));

            if (Math.Abs(GameManager.instance.LevelManager.FieldPosition.y - pos.y) >
                GameManager.instance.LevelManager.FieldSize.y / 2)
                pos.y -= GameManager.instance.LevelManager.FieldSize.y * (pos.y / Math.Abs(pos.y));

            if (pos != transform.position)
            {
                this.Raise(OnBeginTeleport, EventArgs.Empty);
                transform.position = pos;
            }
        }
    }
}