using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Panda;

public class Boss : MonoBehaviour
{

    public Transform office;
    public Vector3[] patrolPoints;
    public NavMeshAgent agent;
    public bool monitorWorkers;

    private bool atOffice;
    private float keyStrokeTimer;
    private Task move;
    public int nextPP;
    public bool arrived;
    public bool goCrazy;

    [Task]
    public bool isNextPP;

    [Task]
    public bool playerInControl()    
    {
        if (keyStrokeTimer < 0)
        {
            return true;            
        }
        else
            return false;
    }    

    // Start is called before the first frame update
    void Start()
    {
        //Find and setting up the patrolpoints
        GameObject[] Gm = GameObject.FindGameObjectsWithTag("PatrolPoint").OrderBy(go => go.name).ToArray();
        patrolPoints = new Vector3[Gm.Length];
        for (int i = 0; i < Gm.Length; i++ )
        {            
            patrolPoints[i] = Gm[i].transform.position;
        }

        keyStrokeTimer = 0;
        isNextPP = false;
        nextPP = -1;
    }

    // Update is called once per frame
    void Update()
    {        
        IsAtGoal();                            
    }   

    // True if agent is close enough to goal, otherwise false
    public void IsAtGoal()
    {
        Debug.Log(agent.radius);
        if (!arrived && Vector3.Distance(gameObject.transform.position, agent.destination) < 2.0f)
        {                     
            move.Complete(true);
            arrived = true;
            nextPP++;
        }
    }

    [Task]
    void GoToOffice()
    {
        arrived = false;
        agent.destination = office.position;
        move = Task.current;
        isNextPP = true;
    }

    [Task]
    public void goToPP()
    {
        //Debug.Log(nextPP + " : " + patrolPoints.Length);
        if(nextPP > patrolPoints.Length-1)
        {
            isNextPP = false;
            Task.current.Fail();
            nextPP = -1;
        }
        else
        {            
            //Debug.Log("hit " + nextPP);
            //agent.destination = new Vector3(0,0,0);
            agent.destination = patrolPoints[nextPP];            
            move = Task.current;            
            arrived = false;
        }        
    }
}
