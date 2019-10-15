﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Singleton pattern
    public static GameManager instance = null;

    public static GameObject[] workers;
    private bool thoughtsActive = true;
    private GameObject[] thoughtBubbles;
    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Awake()
    {
        // Make sure there can only be one instance of GameManager
        if(instance == null) {
            instance = this;
        }
        else if(instance != null) {
            Destroy(gameObject);
        }
        if(thoughtBubbles == null)
            thoughtBubbles = GameObject.FindGameObjectsWithTag("Thought");
            
        if(workers == null)
            workers = GameObject.FindGameObjectsWithTag("Worker");

        AssignTags();
    }
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            ToggleThoughts();
        }
    }

    /// <summary>
    /// Toggles view of UI thoughts of workers
    /// </summary>
    void ToggleThoughts() {

        // Loop through thoughtBubbles and disable all thoughts
        if(thoughtsActive) {
            for(int i = 0; i < thoughtBubbles.Length; i++) {
                thoughtBubbles[i].SetActive(false);
            }
            thoughtsActive = false;
        }
        // Else activate all thoughts
        else {
            for(int i = 0; i < thoughtBubbles.Length; i++) {
                thoughtBubbles[i].SetActive(true);
            }
            thoughtsActive = true;
        }
    }

    /// <summary>
    /// Assigns either Boomer or Millenial tag to each worker
    /// </summary>
    void AssignTags() {
        for(int i = 0; i < workers.Length; i++)
        {
            int rand = Random.Range(0,2);
            switch (rand)
            {
                case 0:
                    workers[i].tag = "Millenial";
                    break;
                case 1:
                    workers[i].tag = "Boomer";
                    break;
            }
        }
    }
}
