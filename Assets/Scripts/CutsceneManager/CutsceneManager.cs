﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    #region Singleton

    public static CutsceneManager instance;

     void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Debug.LogWarning("more than one instance of CutsceneManager");
    }

    #endregion






    [Header("Things to disable")]
    [SerializeField] GameObject player;
    
    [SerializeField] PlayerAttack playerAttack;

    [Header("Cutscene properties")]
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private Animator fader;
    
    [SerializeField] private GameObject overlayCanvas;
    public Queue<CutsceneSequence> sequences = new Queue<CutsceneSequence>();
    [HideInInspector] public bool running;
    private float time;

    public void AddSequence(CutsceneSequence cs){
        sequences.Enqueue(cs);

        if(!running ){
            EnableCutscene();
            running = true;
        }

    }




    public void EnableCutscene(){
               
        StartCoroutine(Cutscene());
    }

    IEnumerator Cutscene(){

        while(!playerAttack.CanAttack())
        {
            yield return new WaitForSeconds(0.1f);
        }

        CutsceneSequence sequence = sequences.Dequeue();

        float d = sequence.cameraDuration;
        time = 0;

        overlayCanvas.SetActive(false);
        cutsceneCamera.SetActive(true);

        if(sequence.usePlayerCam)
            sequence.camStart = player.GetComponentInChildren<PlayerCamera>().transform;
 
        player.SetActive(false);



        while(time < d)
        {
            if(sequence.useTurnPoint){
                if(time >= d/2){
                    cutsceneCamera.transform.position = Vector3.Lerp(sequence.camTurnpoint.position, sequence.camEnd.position, (time-d/2) / (d/2));
                    cutsceneCamera.transform.rotation = Quaternion.Lerp(sequence.camTurnpoint.rotation, sequence.camEnd.rotation, (time-d/2) / (d/2));
                }
                else
                {
                cutsceneCamera.transform.position = Vector3.Lerp(sequence.camStart.position, sequence.camTurnpoint.position, time / (d / 2));
                cutsceneCamera.transform.rotation = Quaternion.Lerp(sequence.camStart.rotation, sequence.camTurnpoint.rotation, time / (d / 2));
                }
            }
            else
            {
                cutsceneCamera.transform.position = Vector3.Lerp(sequence.camStart.position, sequence.camEnd.position, time / d);
                cutsceneCamera.transform.rotation = Quaternion.Lerp(sequence.camStart.rotation, sequence.camEnd.rotation, time / d);
            }
            
            
            time += Time.deltaTime;
            yield return null;
        }
        cutsceneCamera.transform.position = sequence.camEnd.position;

        yield return new WaitForSeconds(sequence.duration);
        


        if(sequences.Count > 0){
            StartCoroutine(Cutscene());        
        }
        else{
            //DISABLE
            fader.Play("ToBlack");
            yield return new WaitForSeconds(0.75f);
            cutsceneCamera.SetActive(false);
            player.SetActive(true);
            overlayCanvas.SetActive(true);
            fader.Play("FromBlack");
            running = false;

        }

    }
}

[System.Serializable]
public struct CutsceneSequence
{
    public float duration;
    public float cameraDuration;
    [Header("Start transform")]
    public Transform camStart;
    [Tooltip("Use player camera transform instead of above value")]
    public bool usePlayerCam;

    [Header("Turnpoint transform")]
    public Transform camTurnpoint;
    public bool useTurnPoint;

    [Header("End transform")]
    public Transform camEnd;
}
