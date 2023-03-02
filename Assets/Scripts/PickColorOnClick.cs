using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickColorOnClick : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image finImage;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            int sourceMipLevel = 0;
            Color[] pixels = ScreenCapture.CaptureScreenshotAsTexture().GetPixels((int)pointerEventData.position.x, (int)pointerEventData.position.y, 1, 1, sourceMipLevel);
            Debug.Log("pointerEventData.position.x=" + pointerEventData.position.x + "|| pointerEventData.position.y" + pointerEventData.position.y +
                "|| pixels=" + pixels.ToString());
            finImage.color = pixels[0];
        }
    }
}
