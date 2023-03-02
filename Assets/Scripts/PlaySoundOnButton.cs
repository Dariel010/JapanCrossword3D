using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaySoundOnButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioSource _audioSourceOnHover;
    [SerializeField] private AudioSource _audioSourceOnClick;
    [SerializeField] private AnimationClip animationPlay;

    public void PlaySoundOnRotate()
    {
        _audioSourceOnClick.PlayOneShot(_audioSourceOnClick.clip);
    }

    public void PlaySoundOnClick()
    {
        _audioSourceOnHover.PlayOneShot(_audioSourceOnHover.clip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Animator animator = GetComponent<Animator>();
        animator.Play(animationPlay.name, 0, 0);
        Debug.Log("PLAY animation==" + animationPlay.name);
    }
}
