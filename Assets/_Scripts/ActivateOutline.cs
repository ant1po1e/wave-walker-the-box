using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOutline : MonoBehaviour
{   
    #region Singleton
    public static ActivateOutline instance { get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    Outline outline;
    private Coroutine fadeCoroutine;

    void Start() {
        outline = GetComponent<Outline>();
        outline.OutlineWidth = 0f;
    }

    public void Activate()
    {
        if (fadeCoroutine != null)
    {
        StopCoroutine(fadeCoroutine);
    }
    
    fadeCoroutine = StartCoroutine(Active());
    }

    IEnumerator Active()
    {
        outline.enabled = true;
        outline.OutlineWidth = 6f; 
        yield return new WaitForSeconds(3);

        float fadeDuration = 1f; 
        float startWidth = outline.OutlineWidth; 
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newWidth = Mathf.Lerp(startWidth, 0f, elapsedTime / fadeDuration);
            outline.OutlineWidth = newWidth; 
            yield return null; 
        }

        outline.OutlineWidth = 0f;
        fadeCoroutine = null;
    }
}
