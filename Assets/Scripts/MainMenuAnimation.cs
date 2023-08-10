using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class MainMenuAnimation : MonoBehaviour
{
    public Sprite[] sprites;
    public int framePerSprite = 12;
    private Image image;
    int index = 0;
    int frame = 0;
    void Awake()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        frame++;
        if (frame < framePerSprite) return;
        image.sprite = sprites[index];
        frame = 0;
        index++;
        if (index == sprites.Length)
        {
            index = 0;
        }
    }
}
