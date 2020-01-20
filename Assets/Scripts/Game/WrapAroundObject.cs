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
            if (other.CompareTag(Constants.Tags.cBoundary)) CheckForTeleport();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(Constants.Tags.cBoundary)) CheckForTeleport();
        }

        private void CheckForTeleport()
        {
            var fieldSize = GameManager.Instance.LevelManager.FieldSize;
            var fieldPos = GameManager.Instance.LevelManager.FieldPosition;
            var pos = transform.position;

            if (Math.Abs(fieldPos.x - pos.x) >
                fieldSize.x / 2)
                pos.x -= fieldSize.x * (pos.x / Math.Abs(pos.x));

            if (Math.Abs(fieldPos.y - pos.y) >
                fieldSize.y / 2)
                pos.y -= fieldSize.y * (pos.y / Math.Abs(pos.y));

            if (pos != transform.position)
            {
                this.Raise(OnBeginTeleport, EventArgs.Empty);
                transform.position = pos;
            }
        }
    }
}