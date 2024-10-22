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
    void Start() {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Activate()
    {
        outline.enabled = true;
    }
}
