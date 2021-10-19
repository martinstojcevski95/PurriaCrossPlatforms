using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Info : MonoBehaviour
{
    [SerializeField]
    Text InfoText;
    [SerializeField]
    Button ConfirmButton;



    public  void InfoDataSet(string info,GameObject infoObject,float destroyTime)
    {
        InfoText.text = info;
        ConfirmButton.onClick.AddListener(delegate { CloseUI(infoObject); });
        Destroy(infoObject, destroyTime);
    }


    void CloseUI(GameObject infObject)
    {
        DestroyImmediate(infObject);
    }

}
