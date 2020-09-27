﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager instance;
    private void Awake() {
        if(instance == null)
            instance = this;
        else
            Debug.LogWarning("more than one instance of progressmanager");
    }


    [SerializeField] DoorStruct[] door;

    int currentKills;

    private void Start() {
        Reset();    
    }

    private void Reset() {
        currentKills = 0;
        UpdateUI();
    }

    public void AddKill(){
        currentKills++;
        foreach(DoorStruct d in door){
            if(d.killsToUnlock == currentKills)
            {
                d.doorToOpen.canOpen = true;
            }

        }
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach(DoorStruct d in door){
            int killsLeft = d.killsToUnlock-currentKills;

            if (killsLeft == 0)
                d.display.text = "Åben døren ven. :)";
            else
                d.display.text = currentKills.ToString() + " / " + killsLeft.ToString();
        }
        
    }

}

[System.Serializable]
public struct DoorStruct
{
    public int killsToUnlock;
    public Door doorToOpen;
    public TextMeshProUGUI display;

}