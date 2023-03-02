using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll3D : MonoBehaviour
{
    // 9 smeshenie tablici. 18- shag povorota. 11- povorotnoe propodanie
    private float speed;
    private Rigidbody rig;
    private bool dragging = false;
    private float xRot;
    [SerializeField] private AudioSource _audioSourceDrugOn;
    [SerializeField] private AudioSource _audioSourceDrugOff;
    [SerializeField] private AudioSource _audioSourceExitDerg;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject _exitButton;
    public float cliptime;

    void Start()
    {
        speed = 450;
        rig =  GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            if (_audioSourceDrugOn.isPlaying & !_exitButton.GetComponent<DzinExitDerg>().exitStartAnimation)
            {
                _audioSourceDrugOn.Stop();
                _audioSourceDrugOff.PlayOneShot(_audioSourceDrugOff.clip);
            }
            
            else if (_exitButton.GetComponent<DzinExitDerg>().exitStartAnimation)
            {
                _audioSourceDrugOn.Stop();
                _audioSourceDrugOff.Stop();
            }
            
        } 
    }
    private void OnMouseDrag()
    {
        dragging = true;
        if (_exitButton.GetComponent<DzinExitDerg>().exitStartAnimation)
        {
            _audioSourceDrugOn.Stop();
            //Debug.Log("_exitButton.GetComponent<DzinExitDerg>().exitStartAnimation"+_exitButton.GetComponent<DzinExitDerg>().exitStartAnimation);
        }
        else if(!_audioSourceDrugOn.isPlaying & !_exitButton.GetComponent<DzinExitDerg>().exitStartAnimation)
        {
            _audioSourceDrugOn.Play();
        }
    }

    private void FixedUpdate()
    {
        if (dragging)
        {
            //Debug.Log("dragging=======FIX");
            xRot = Input.GetAxis("Mouse X") * speed * Time.fixedDeltaTime;
            rig.AddTorque(new Vector3(0, 1, 0) * xRot);
        }
    }
}
