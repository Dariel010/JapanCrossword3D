using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NewCrosswordInit : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private TMPro.TMP_InputField wight;
    [SerializeField]
    private TMPro.TMP_InputField height;
    [SerializeField]
    private Manager switchCursor;

    public void OnPointerClick(PointerEventData eventData)
    {
          switchCursor.ClickNewEmptyCrosswordInit(int.Parse(wight.text), int.Parse(height.text));
    }
}
