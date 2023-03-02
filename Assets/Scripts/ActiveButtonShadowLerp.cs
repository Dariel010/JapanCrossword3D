using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActiveButtonShadowLerp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Image _imageShadow;
    private IEnumerator _coroutineAnimation = null, _coroutineAnimationUp = null;
    [SerializeField]
    float _timeDelay = 0.05f, _animationDuration = 1f, _shadowPressedPositionX = -5f, _shadowPressedScale = 0.85f, _buttonPressedScale = 0.91f;
    float _shadowIdlePositionX;

    private void Start()
    {
        _shadowIdlePositionX = _imageShadow.transform.localPosition.x;
    }

    private void OnDisable()
    {
        transform.localScale = Vector3.one;
        _imageShadow.transform.localScale = Vector3.one;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Debug.Log("transform.localPosition.x DOWN==" + transform.localPosition.x);
        Debug.Log(name + "Game Object Click in Progress");
        AudioPlayer.instance.PlayClick(2);
        if (_coroutineAnimation == null && pointerEventData.button == PointerEventData.InputButton.Left)
        {
            _coroutineAnimation = ButtonAnimateDownscale();
            StartCoroutine(_coroutineAnimation);
        }
        if (_coroutineAnimationUp != null)
        {
            StopCoroutine(_coroutineAnimationUp);
        }

    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        Debug.Log("transform.localPosition.x UP ==" + transform.localPosition.x);
        Debug.Log(name + "No longer being clicked");
        if (_coroutineAnimation != null)
        {
            StopCoroutine(_coroutineAnimation);
            _coroutineAnimation = null;
        }
        _coroutineAnimationUp = ButtonAnimateUpscale();
        StartCoroutine(_coroutineAnimationUp);
    }

    IEnumerator ButtonAnimateDownscale()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = new Vector3(_buttonPressedScale, _buttonPressedScale, transform.localScale.z);
        Vector3 startShadowScale = _imageShadow.transform.localScale;
        Vector3 endShadowScale = new Vector3(_shadowPressedScale, _shadowPressedScale, transform.localScale.z);
        Vector3 startPosition = _imageShadow.transform.localPosition;//////LOCALLLLLLLLLL
        Vector3 endPosition = new Vector3(_shadowPressedPositionX, _imageShadow.transform.localPosition.y, _imageShadow.transform.localPosition.z);//// LOCAALL
        float currentTime = 0;
        while (currentTime < _animationDuration)
        {
            float buttonScale = Mathf.Lerp(startScale.x, endScale.x, currentTime / _animationDuration);
            transform.localScale = new Vector3(buttonScale, buttonScale, transform.localScale.z);
            startScale = transform.localScale;

            float shadowScale = Mathf.Lerp(startShadowScale.x, endShadowScale.x, currentTime / _animationDuration);
            _imageShadow.transform.localScale = new Vector3(shadowScale, shadowScale, _imageShadow.transform.localScale.z);
            startShadowScale = _imageShadow.transform.localScale;

            float shadowPositionX = Mathf.Lerp(startPosition.x, endPosition.x, currentTime / _animationDuration);
            _imageShadow.transform.localPosition = new Vector3(shadowPositionX, _imageShadow.transform.localPosition.y,
                _imageShadow.transform.localPosition.z);

            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(_timeDelay);
        }
        Debug.Log("ButtonAnimateDownscale ----");
        _imageShadow.transform.localPosition = new Vector3(_shadowPressedPositionX, _imageShadow.transform.localPosition.y,
            _imageShadow.transform.localPosition.z);
        _coroutineAnimation = null;
        yield return null;
    }

    IEnumerator ButtonAnimateUpscale()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one;
        Vector3 startShadowScale = _imageShadow.transform.localScale;
        Vector3 endShadowScale = Vector3.one;
        Vector3 startPosition = _imageShadow.transform.localPosition;///////LOCAL
        Vector3 endPosition = new Vector3(_shadowIdlePositionX, _imageShadow.transform.localPosition.y, _imageShadow.transform.localPosition.z);////// LOOCALL
        float currentTime = 0;
        while (currentTime < _animationDuration | transform.localScale.x < 1f) //while (transform.localScale.x < 1f)
        {
            float buttonScale = Mathf.Lerp(startScale.x, endScale.x, currentTime / _animationDuration);
            transform.localScale = new Vector3(buttonScale, buttonScale, transform.localScale.z);
            startScale = transform.localScale;
            Debug.Log("startScaleUP = " + startScale.x + ", " + startScale.y + "; ");

            float shadowScale = Mathf.Lerp(startShadowScale.x, endShadowScale.x, currentTime / _animationDuration);
            _imageShadow.transform.localScale = new Vector3(shadowScale, shadowScale, _imageShadow.transform.localScale.z);
            startShadowScale = _imageShadow.transform.localScale;
            Debug.Log("startShadowScaleUP = " + startShadowScale.x + ", " + startShadowScale.y + "; ");

            float shadowPositionX = Mathf.Lerp(startPosition.x, endPosition.x, currentTime / _animationDuration);
            //float shadowPositionY = Mathf.Lerp(startPosition.y, endPosition.y, currentTime / duration);
            _imageShadow.transform.localPosition = new Vector3(shadowPositionX, _imageShadow.transform.localPosition.y,
                _imageShadow.transform.localPosition.z);

            if (Mathf.Approximately(transform.localScale.x, 1f))
            {
                transform.localScale = Vector3.one;
            }

            currentTime += Time.deltaTime;
            yield return new WaitForSeconds(_timeDelay);
        }
        Debug.Log("ButtonAnimateUpscale ----");
        transform.localScale = Vector3.one;
        _imageShadow.transform.localPosition = new Vector3(_shadowIdlePositionX, _imageShadow.transform.localPosition.y, _imageShadow.transform.localPosition.z);
        _imageShadow.transform.localScale = Vector3.one;
        StopCoroutine(_coroutineAnimationUp);
        _coroutineAnimationUp = null;
        yield return null;
    }
}
