using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraMovement : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public RectTransform rectTransform; 
        public float parallaxStrength = 5f; 
    }

    public ParallaxLayer[] layers; 

    private Vector2[] initialPositions; 

    void Start()
    {
        initialPositions = new Vector2[layers.Length];

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].rectTransform != null)
            {
                initialPositions[i] = layers[i].rectTransform.anchoredPosition;
            }
        }
    }

    void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].rectTransform == null)
                continue;
            Vector2 newPosition = initialPositions[i] + new Vector2(mouseX * layers[i].parallaxStrength, mouseY * layers[i].parallaxStrength);

            layers[i].rectTransform.anchoredPosition = Vector2.Lerp(layers[i].rectTransform.anchoredPosition, newPosition, Time.unscaledDeltaTime * 5f);
        }
    }
}
