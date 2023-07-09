using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] TextMeshProUGUI tutorialText;
    [SerializeField] GameObject TutorialContainerImage;

    List<String> sentecesTutorial;

    public bool isTutorialProgress;
    public int tutorialIndexSenteces;

    private void Start()
    {
        sentecesTutorial = new List<string>();
        tutorialIndexSenteces = 0;
        AddSentences();
        isTutorialProgress = true;
    }

    private void AddSentences()
    {
        sentecesTutorial.Add("Hurry up! This is the first day of dungeoning for my son, I want to help him without getting notice from him");
        sentecesTutorial.Add( "So you have to do this");
        sentecesTutorial.Add( "And than this");
        sentecesTutorial.Add( "At the end that one");
    }

    void Update()
    {
        if (isTutorialProgress)
        {
            tutorialText.text = sentecesTutorial[tutorialIndexSenteces];
            gameManager.gameObject.SetActive(false);

            if (Input.GetKeyDown(KeyCode.N))
            {
                tutorialIndexSenteces = tutorialIndexSenteces + 1;

                if (tutorialIndexSenteces >= sentecesTutorial.Count)
                {
                    Debug.Log("ended tutorial");
                    isTutorialProgress = false;
                    TutorialContainerImage.SetActive(false);
                }
                else
                {
                tutorialText.text = sentecesTutorial[tutorialIndexSenteces];

                }

            }
           


        }
        else
        {
            gameManager.gameObject.SetActive(true);

        }
    }

    private void StartTutorial()
    {
        Debug.Log("TODO");
    }
}
