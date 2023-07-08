using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSprite : MonoBehaviour
{
    public List<Image> listOfImages = new List<Image>();
    public int imageCounter;

    void Start()
    {
        LoadImages();
        DefaultImage();
    }

    private void LoadImages()
    {
        throw new NotImplementedException();
    }

    private void DefaultImage()
    {
        imageCounter = 0;
        GetComponent<Image>().sprite = listOfImages[imageCounter].sprite;
    }

    public void NextImage()
    {
        imageCounter = imageCounter + 1;
        if (imageCounter < listOfImages.Count)
        {
            GetComponent<Image>().sprite = listOfImages[imageCounter].sprite;
        }
        else
        {
            GetComponent<Image>().sprite = listOfImages[listOfImages.Count - 1].sprite;
        }
    }

}
