using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseArea {

    public Transform areaTransform;
    public bool inUse;        
    
    public UseArea(Transform t)
    {
        areaTransform = t;
        inUse = false;
    }

    public void Book()
    {
        this.inUse = true;        
    }

    public void Release()
    {
        inUse = false;
    }
}
public class QueueArea {

    public Transform areaTransform;
    public bool inUse;

    public QueueArea(Transform t)
    {
        areaTransform = t;
        inUse = false;
    }

    public void Book()
    {
        this.inUse = true;        
    }

    public void Release()
    {
        inUse = false;
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
    public bool[] queuePlaces;
    
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

    public void BookUseAreaState(int areaIdx)
    {               
        useAreas[areaIdx].Book();        
    }

    public void ReleaseUseAreaState(int areaIdx)
    {
        useAreas[areaIdx].Release();        
    }

    public void BookQueueAreaState(int areaIdx)
    {
        queueAreas[areaIdx].Book();
    }

    public void ReleaseQueueAreaState(int areaIdx)
    {
        queueAreas[areaIdx].Release();
    }



    /// <summary>
    /// Return index of first free use area, if no queue areas are free, return -1
    /// </summary>
    public int GetFirstFreeUseArea() {
        int index = -1;
        if(!isFull())
        {
            for(int i = 0; i < useAreas.Count; i++)
            {
                if(!useAreas[i].inUse)
                {
                    index = i;
                    break;
                }
            }
        }
        return index;
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
                {
                    index = i;
                    break;
                }
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

        queuePlaces = new bool[queueAreasTransforms.Length];
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Shows editor whether occupied and queueable or not
        occupied = isFull();
        queueable = !isQueueFull();

        /*** Use this if you want to debug queue area occupation ***
        for(int i = 0; i < queueAreas.Count; i++)
            Debug.Log("Spot: " + i + " is occupied = " + queueAreas[i].inUse);
        */

        for (int i = 0; i < queueAreasTransforms.Length; i++)
        {
            queuePlaces[i] = queueAreas[i].inUse;
        }
    }
}
