using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSpin : MonoBehaviour
{
    private int propertyId;
    private void Start()
    {
        propertyId = GetComponent<TMPro.TMP_Text>().material.shader.FindPropertyIndex("Face Dilate");
        string paramS = GetComponent<TMPro.TMP_Text>().material.shader.name;
        GetComponent<TMPro.TMP_Text>().material.SetFloat("_Sharpness", -1f);
    }
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1));
    }
}
