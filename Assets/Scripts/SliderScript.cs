using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] GameObject sliderText; 
    [SerializeField] private string prefix;
    void Start()
    {
        if (sliderText.GetComponent<TMPro.TMP_InputField>())
        {
            slider.onValueChanged.AddListener((value) => {
                sliderText.GetComponent<TMPro.TMP_InputField>().text = value.ToString() + prefix;
            });
        }
        else if (sliderText.GetComponent<TMPro.TMP_Text>())
        {
            slider.onValueChanged.AddListener((value) => {
                sliderText.GetComponent<TMPro.TMP_Text>().text = value.ToString() + prefix;
            });
        }
    }

    public void UpdateText()
    {
        if (sliderText.GetComponent<TMPro.TMP_InputField>())
        {
            sliderText.GetComponent<TMPro.TMP_InputField>().text = slider.value.ToString() + prefix;
           
        }
        else if (sliderText.GetComponent<TMPro.TMP_Text>())
        {
            sliderText.GetComponent<TMPro.TMP_Text>().text = slider.value.ToString() + prefix;
        }
    }
}
