using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonNetworkTab : MonoBehaviour
{
    public delegate void OnClickNetworkTab(GameObject sender, string filename);
    public static event OnClickNetworkTab CallEvent;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Click);
    }

    private void Click()
    {
        CallEvent(this.gameObject, transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text);
    }
}
