using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FunBarManager : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI sliderText;
  public  int funBarCounter;
    void Start()
    {
        funBarCounter = 10;
        slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("10.00");
        });
    }


    public void ChangeFunBarCounter(int amount)
    {
        funBarCounter = funBarCounter + amount;
    }
     
    void Update()
    {
        sliderText.text = funBarCounter.ToString();
        slider.value = funBarCounter;
    }
}
