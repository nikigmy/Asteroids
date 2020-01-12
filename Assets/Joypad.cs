using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joypad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IDirectionalInput
{
    [SerializeField] private bool arcade = false;
    private Vector2 inputVector;
    private RectTransform bgImage;
    private RectTransform pad;
    
    private Vector2 rectSize;
    // Start is called before the first frame update
    void Start()
    {
        pad = transform.GetChild(0).GetComponent<RectTransform>();
        bgImage = GetComponent<RectTransform>();
        rectSize = new Vector2(bgImage.rect.width,bgImage.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputVector != Vector2.zero)
        {
            if (OnDirectinalInput != null)
            {
                var returnValue = inputVector;
                if (arcade)
                {
                    if (returnValue.x > -0.2f && returnValue.x < 0.2f)
                    {
                        returnValue.x = 0;
                    }
                    else
                    {
                        returnValue.x /= Mathf.Abs(returnValue.x);//normalisation
                    }
                    if (returnValue.y > -0.2f && returnValue.y < 0.2f)
                    {
                        returnValue.y = 0;
                    }
                    else
                    {
                        returnValue.y = 1;
                    }
                }
                OnDirectinalInput.Invoke(returnValue);
            }
        }
    }
    public void OnPointerDown(PointerEventData e)
    {
        UpdateInputVec(e);
    }
    
    public void OnDrag(PointerEventData e)
    {
        UpdateInputVec(e);
    }
    
    public void OnPointerUp(PointerEventData e)
    {
        inputVector = Vector2.zero;
        pad.anchoredPosition = Vector2.zero;
    }
    private void UpdateImagePosition()
    {
        pad.anchoredPosition = new Vector2(inputVector.x * (rectSize.x / 2),inputVector.y * (rectSize.y / 2));
    }
    
    private void UpdateInputVec(PointerEventData e)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgImage, e.position, e.pressEventCamera, out pos))
        {
            inputVector = Vector2.ClampMagnitude(new Vector2((pos.x / (float)rectSize.x) * 2f, (pos.y / (float)rectSize.y) * 2f), 1f); // Input space to Normalized Input Coordinates
            UpdateImagePosition();
        }
    }

    public event Defines.DirectionalInputDelegate OnDirectinalInput;
}
