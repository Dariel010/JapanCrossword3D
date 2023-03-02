using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraMove : MonoBehaviour
{


    private float _dragSpeed = 1.4f, _zoomSpeed = 1.8f, _zoomMax = 95f, _zoomMin = 6f, _touchSensitive = 1.4f, _mouseSensitive = 30f, 
        hiz = 0.8f, _angleYdown = 3f, _angleYup = 89.9f, _touchSpeed = 0.02f;//angleYdown = 30f, angleYup = 120f, 
    //dragSpeed = 1.4f, _zoomSpeed = 2f, _zoomMax = 95f, _zoomMin = 6f, _touchSensitive = 1.6f
    private float _cameraRect = 60f;
    private Vector3 _dragOrigin, _initialPosition;
    [SerializeField] private Camera _camera;
    public bool switchRotate = false;
    public float zoomMulti = 0.2f, rotateMulti = 0.2f;
    //public static event Action UpdateMultipliers;
    

    private void Start()
    {
        _initialPosition = transform.position;
        SettingsScript.onZoomRotateMultiChange += SettingsScript_OnZoomRotateMultiChange;
        PlayerPrefs.GetFloat("zoomMulti", 0.2f);
        PlayerPrefs.GetFloat("rotateMulti", 0.2f);
    }

    private void SettingsScript_OnZoomRotateMultiChange(float zoom, float rotate)
    {
        if (zoom != 0)
        {
            zoomMulti = zoom;
        }
        if (rotate != 0)
        {
            rotateMulti = rotate;
        }
    }

    void LateUpdate()
    {
        
        if (transform.position.x <= _initialPosition.x + _cameraRect)
        {
            if (transform.position.x >= _initialPosition.x - _cameraRect)
            {
                if (transform.position.z <= _initialPosition.z + _cameraRect)
                {
                    if (transform.position.z >= _initialPosition.z - _cameraRect)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            _dragOrigin = Input.mousePosition;
                            return;
                        }

                        if (Input.touchCount == 2)
                        {
                            Touch touchOne = Input.GetTouch(0);
                            Touch touchTwo = Input.GetTouch(1);
                            Vector2 touchOneLastPosition = touchOne.position - touchOne.deltaPosition;
                            Vector2 touchTwoLastPosition = touchTwo.position - touchTwo.deltaPosition;
                            float lastMagnitude = (touchOneLastPosition - touchTwoLastPosition).magnitude;
                            float currentMagnitude = (touchOne.position - touchTwo.position).magnitude;
                            float finDifference = currentMagnitude - lastMagnitude;
                            if (switchRotate)
                            {
                                transform.Rotate(-touchOne.deltaPosition.y * (_touchSensitive + rotateMulti) * Time.deltaTime, touchOne.deltaPosition.x * (_mouseSensitive + rotateMulti) * Time.deltaTime, 0);
                                //transform.Rotate(-Input.GetAxis("Mouse Y") * hassasiyet * Time.deltaTime, Input.GetAxis("Mouse X") * hassasiyet * Time.deltaTime, 0);
                                transform.eulerAngles = new Vector3(Mathf.Clamp(transform.eulerAngles.x, _angleYdown, _angleYup), transform.eulerAngles.y, 0);
                                if (Manager.instance.TutorialManager != null)
                                {
                                    StartCoroutine(TimeDelayBeforeMethod(() => Manager.instance.TutorialManager.OnEvent(TutorialEvent.ShowRotate)));
                                }
                            }
                            else
                            {
                                ZoomCamera(finDifference * _touchSpeed);
                            }
                            return;
                        }

                        if (Input.mouseScrollDelta.y != 0)
                        {
                            ZoomCamera(Input.GetAxis("Mouse ScrollWheel"));
                            return;
                        }
                        
                        if (Input.GetMouseButton(1) & Input.touchCount == 0)
                        {
                            transform.Rotate(-Input.GetAxis("Mouse Y") * (_mouseSensitive + rotateMulti) * Time.deltaTime, Input.GetAxis("Mouse X") * (_mouseSensitive + rotateMulti) * Time.deltaTime, 0);
                            transform.eulerAngles = new Vector3(Mathf.Clamp(transform.eulerAngles.x, _angleYdown, _angleYup), transform.eulerAngles.y, 0);
                            transform.Translate(Input.GetAxis("Horizontal") * -hiz, 0, Input.GetAxis("Vertical") * -hiz);
                            if (Manager.instance.TutorialManager != null)
                            {
                                StartCoroutine(TimeDelayBeforeMethod(() => Manager.instance.TutorialManager.OnEvent(TutorialEvent.ShowRotate)));
                            }
                        }
                        
                        if (Input.GetMouseButton(0) & Input.touchCount < 2)
                        {
                            Vector3 pos = _camera.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
                            //if (pos.x * dragSpeed < 60 && pos.x * dragSpeed > -60 && pos.y * dragSpeed < 60 && pos.y * dragSpeed > -60)
                            //{
                            Vector3 move = new Vector3(pos.x * _dragSpeed, 0, pos.y * _dragSpeed);
                            //Vector3 move = new Vector3(Mathf.Clamp(pos.x * dragSpeed, -60f, 60f), 0, Mathf.Clamp(pos.y * dragSpeed, -60f, 60f));
                            transform.Translate(move, Space.World);
                            if (Manager.instance.TutorialManager != null)
                            {
                                StartCoroutine(TimeDelayBeforeMethod(() => Manager.instance.TutorialManager.OnEvent(TutorialEvent.ShowMove)));
                            }
                        }
                        return;
                    }
                    transform.position = new Vector3(transform.position.x, transform.position.y, _initialPosition.z - (_cameraRect - 0.00001f));
                    return;
                }
                transform.position = new Vector3(transform.position.x, transform.position.y, _initialPosition.z + (_cameraRect - 0.00001f));
                return;
            }
            transform.position = new Vector3(_initialPosition.x - (_cameraRect - 0.00001f), transform.position.y, transform.position.z);
            return;
        }
        transform.position = new Vector3(_initialPosition.x + (_cameraRect - 0.00001f), transform.position.y, transform.position.z);

        
    }
    private void ZoomCamera(float difference)
    {
        float currentY = transform.position.y;
        float newY = Mathf.Clamp(currentY - difference * (_zoomSpeed + zoomMulti), _zoomMin, _zoomMax);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        if (Manager.instance.TutorialManager != null)
        {
            StartCoroutine(TimeDelayBeforeMethod(() => Manager.instance.TutorialManager.OnEvent(TutorialEvent.ShowZoom)));
        }
    }

    private IEnumerator TimeDelayBeforeMethod(System.Action methodWithParameters)
    {
        yield return new WaitForSeconds(3);
        methodWithParameters();
    }
}
