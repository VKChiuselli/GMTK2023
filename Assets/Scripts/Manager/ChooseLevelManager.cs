using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseLevelManager : MonoBehaviour
{
  
    public void ChangeScene(int indexScene)
    {
        SceneManager.LoadScene(indexScene);
    }


}
