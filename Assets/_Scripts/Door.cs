using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    #region Singleton
    public static Door instance { get; private set; }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public Animator needKeyText;
    public Animator nextLevel;
    public GameObject keyIcon;

    Outline outline;


    public bool hasKey;

    void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        outline.OutlineWidth = 0f;   
        keyIcon.SetActive(false);
    }

    void Update()
    {
        if (hasKey == true)
        {
            outline.OutlineWidth = 10f;
            keyIcon.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (hasKey == false)
            {
                needKeyText.SetBool("Activate", true);
            } else 
            {
                UnlockNewLevel();
                Time.timeScale = 0f;
                AudioManager audioManager = AudioManager.instance;
                audioManager.PlaySFX("door");
                nextLevel.SetTrigger("Next");
                GameManager.instance.isFinished = true;  
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {    
        if (other.CompareTag("Player"))
        {
            if (hasKey == false)
            {
                needKeyText.SetBool("Activate", false);
            }
        }
    }

    void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
