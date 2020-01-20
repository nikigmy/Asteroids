using System;
using Iterfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

using DirectionalArgs = Utils.ValueArgs<UnityEngine.Vector2>;
namespace Input
{
    /// <summary>
    /// Graphical directional joypad
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class Joypad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IDirectionalInput
    {
        public event EventHandler<DirectionalArgs> OnDirectinalInput;

        private void Awake()
        {
            mBgImage = GetComponent<RectTransform>();

            Debug.Assert(pad != null);
        }

        private void Start()
        {
            mRectSize = new Vector2(mBgImage.rect.width, mBgImage.rect.height);
        }
        
        private void OnDisable()
        {
            mInputVector = Vector2.zero;
            pad.anchoredPosition = Vector2.zero;
        }

        private void Update()
        {
            if (mInputVector != Vector2.zero)
                if (OnDirectinalInput != null)
                {
                    var returnValue = mInputVector;

                    this.Raise(OnDirectinalInput, new DirectionalArgs(returnValue));
                }
        }

        public void OnDrag(PointerEventData e)
        {
            UpdateInputVec(e);
        }

        public void OnPointerDown(PointerEventData e)
        {
            UpdateInputVec(e);
        }

        public void OnPointerUp(PointerEventData e)
        {
            mInputVector = Vector2.zero;
            pad.anchoredPosition = Vector2.zero;
        }
        
        private void UpdateImagePosition()
        {
            pad.anchoredPosition = new Vector2(mInputVector.x * (mRectSize.x / 2), mInputVector.y * (mRectSize.y / 2));
        }

        private void UpdateInputVec(PointerEventData e)
        {
            Vector2 pos;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mBgImage, e.position, e.pressEventCamera, out pos))
            {
                mInputVector =
                    Vector2.ClampMagnitude(new Vector2(pos.x / mRectSize.x * 2f, pos.y / mRectSize.y * 2f),
                        1f); // Input space to Normalized Input Coordinates
                UpdateImagePosition();
            }
        }

        private RectTransform mBgImage;
        
        private Vector2 mInputVector;

        private Vector2 mRectSize;
        
        [SerializeField] private RectTransform pad;
    }
}