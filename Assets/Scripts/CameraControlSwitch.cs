using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraControlSwitch : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private CameraMove _cameraMove;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite _isOnRotateSprite;
    [SerializeField] private Sprite _isOffRotateSprite;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_cameraMove.switchRotate)
        {
            _cameraMove.switchRotate = false;
            _image.sprite = _isOffRotateSprite;
        }
        else
        {
            _cameraMove.switchRotate = true;
            _image.sprite = _isOnRotateSprite;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        _image.color = new Color32(4, 255, 35, 142);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        _image.color = new Color32(255, 255, 255, 142);
    }

    public void SetOffRotate()
    {
        _cameraMove.switchRotate = false;
        _image.sprite = _isOffRotateSprite;
    }
}
