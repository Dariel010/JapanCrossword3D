using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DzinExitDerg : MonoBehaviour, IPointerClickHandler
{
    public bool exitStartAnimation = false;

    public void DzinExit() 
    {
        Manager.instance.OpenMainMenuFrom3DMenu();
    }

    public void PlaySoundOnRotate()
    {
        AudioPlayer.instance.PlayClick(5);
    }

    public void PlaySoundOnClick()
    {
        AudioPlayer.instance.PlayClick(4);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Exit3DTouch");
        animator.Play("Base Layer.Exit3DTouch", 0, 0);
        exitStartAnimation = true;
        Debug.Log("PLAY ==Base Layer.3DTouch");
    }

    private void OnDisable()
    {
        exitStartAnimation = false;
    }
}
