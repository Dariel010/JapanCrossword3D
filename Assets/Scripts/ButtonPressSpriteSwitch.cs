using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPressSpriteSwitch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Sprite _buttonPressedSprite;
    [SerializeField]
    private Sprite _buttonReleasedSprite;

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = _buttonPressedSprite;
        AudioPlayer.instance.PlayClick(0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = _buttonReleasedSprite;
    }
}
