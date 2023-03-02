using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialText : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMPro.TextMeshProUGUI textPro;
    [SerializeField] private Button button;
    float textSpeed = 0.8f;

    public void SetText(string text)
    {
        button.gameObject.SetActive(false);
        textPro.text = "";
        StartCoroutine(AnimateTextWrite(text));
    }

    private IEnumerator AnimateTextWrite(string text)
    {
        int count = 0;
        while (count < text.Length)
        {
            textPro.text += text.Substring(count, 1);
            AudioPlayer.instance.PlayLongSound(3, true);
            count += 1;
            yield return new WaitForSeconds(textSpeed);
        }
        AudioPlayer.instance.PlayLongSound(3, false);
        button.gameObject.SetActive(true);
        //Manager.instance.TutorialManager.OnEvent(TutorialEvent.FinishedTyping);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        textSpeed = 0.01f;
    }
}
