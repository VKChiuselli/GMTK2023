using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroCutSceneManager : MonoBehaviour
{

    public List<GameObject> cutSceneObjects;
  [SerializeField]  public AudioSource audiosource;
    int currentIndex;

    private void Start()
    {
        currentIndex = 0;
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            currentIndex = currentIndex + 1;


            if (currentIndex == 0)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.GetChild(2).gameObject.SetActive(false);
                transform.GetChild(3).gameObject.SetActive(false);
                transform.GetChild(4).gameObject.SetActive(false);
            }
            else if (currentIndex == 1)
            {
               transform.GetChild(0).gameObject.SetActive(false);
               transform.GetChild(1).gameObject.SetActive(true);
               transform.GetChild(2).gameObject.SetActive(false);
               transform.GetChild(3).gameObject.SetActive(false);
               transform.GetChild(4).gameObject.SetActive(false);
            }
            else if (currentIndex == 2)
            {
              transform.GetChild(0).gameObject.SetActive(false);
              transform.GetChild(1).gameObject.SetActive(false);
              transform.GetChild(2).gameObject.SetActive(true);
              transform.GetChild(3).gameObject.SetActive(false);
              transform.GetChild(4).gameObject.SetActive(false);
            }
            else if (currentIndex == 3)
            {
               transform.GetChild(0).gameObject.SetActive(false);
               transform.GetChild(1).gameObject.SetActive(false);
               transform.GetChild(2).gameObject.SetActive(false);
               transform.GetChild(3).gameObject.SetActive(true);
               transform.GetChild(4).gameObject.SetActive(false);
            }
            else if (currentIndex == 4)
            {
               transform.GetChild(0).gameObject.SetActive(false);
               transform.GetChild(1).gameObject.SetActive(false);
               transform.GetChild(2).gameObject.SetActive(false);
               transform.GetChild(3).gameObject.SetActive(false);
               transform.GetChild(4).gameObject.SetActive(true);
            }
            else
            {
                if (audiosource != null)
                {
                    audiosource.volume = 0.3f;
                }
                gameObject.SetActive(false);
            }
             
        }


    }

}
