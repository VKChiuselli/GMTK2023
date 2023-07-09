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
    
        sentecesTutorial.Add("While helping Dunkus on his adventure, you cannot let him see you, so make sure you end your turn in a place he can't reach you or see you.");
        sentecesTutorial.Add("Now as a powerful mage, you don't need to run to the next room, it's definitely not because of your old aching bones but because you're powerful!");
        sentecesTutorial.Add("Simply teleport to the next room.");
        sentecesTutorial.Add("Watch out my mana!"); 
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
