﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct UseArea {
    
    public Transform transform;
    public bool inUse {
        get
        {
            // If no workers are standing at the use area, return false
            foreach(GameObject worker in GameManager.workers) {
                if(Vector3.Distance(transform.position, worker.transform.position) > 1)
                    return false;
            }
            return true;
        }
    }
    
    public UseArea(Transform t)
    {
        transform = t;
    }
    
}
public struct QueueArea {

    public Transform transform;
    public bool inUse {
        get
        {
            // If no workers are standing at the queue area, return false
            foreach(GameObject worker in GameManager.workers) {
                if(Vector3.Distance(transform.position, worker.transform.position) > 0.2)
                    return false;
            }
            return true;
        }
    }

    public QueueArea(Transform t)
    {
        transform = t;
    }

}

public class DestinationProperties : MonoBehaviour
{

    // Public variables
    public int limit;    // Number of workers that can use the object at the same time
    public int qLimit;   // Number of workers that can wait to use the object at the same time
    public bool occupied;
    public Transform[] useAreasTransforms;      // Holds a reference(s) to the object's use area(s) transform(s)
    public Transform[] queueAreasTransforms;    // Holds a reference(s) to the object's queue area(s) transform(s)

    public List<UseArea> useAreas = new List<UseArea> ();
    public List<QueueArea> queueAreas = new List<QueueArea> ();

    public bool queueable;  // Is the object queueable?
    
    /// <summary>
    /// Loop through all use areas of the object and check if the number of users >= limit
    /// </summary>
    public bool isFull() {
        int users = 0;
        for(int i = 0; i < useAreas.Count; i++)
        {
            if(useAreas[i].inUse)
            {
                users++;
            }
        }
        return users >= limit; // Return true if all spots are being used, false if not
    }

    /// <summary>
    /// Loop through all queue areas and check if number of users >= qLimit
    /// </summary>
    public bool isQueueFull() {
        int users = 0;
        for(int i = 0; i < queueAreas.Count; i++)
        {
            if(queueAreas[i].inUse)
            {
                users++;
            }
        }
        return users >= qLimit; // Return true if all spots are being used, false if not
    }

    /// <summary>
    /// Return index of first free queue area, if no queue areas are free, return -1
    /// </summary>
    public int GetFirstFreeQueueArea() {
        int index = -1;
        if(!isQueueFull())
        {
            for(int i = 0; i < queueAreas.Count; i++)
            {
                if(!queueAreas[i].inUse)
                    index = i;
            }
        }
          
        return index;
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Initialize use areas and queue areas
        for(int i = 0; i < useAreasTransforms.Length; i++)
        {
            UseArea temp = new UseArea(useAreasTransforms[i]);
            useAreas.Add(temp);
        }
        for(int i = 0; i < queueAreasTransforms.Length; i++)
        {
            queueAreas.Add(new QueueArea(queueAreasTransforms[i]));
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        occupied = isFull();
        queueable = !isQueueFull();
        for(int i = 0; i < queueAreas.Count; i++)
            Debug.Log("Spot: " + i + " is occupied = " + queueAreas[i].inUse);
    }
}