using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Panda;

public class Boss : MonoBehaviour
{

    // Position related variables
    public Transform office;
    private Vector3[] patrolPoints;  

    //Agent Related variables
    public NavMeshAgent agent;
    
    private float keyStrokeTimer;
    private float speed;
    private Task move;
    public int nextPP;
    public bool arrived;        
    private bool playerWasInControll;

    [Task]
    private bool isNextPP; //If we have a next patrolpoint

    [Task]
    //If the player controls the boss
    public bool playerInControl() 
    {
        if (keyStrokeTimer > 0)
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

        //Init variables
        keyStrokeTimer = 0;
        isNextPP = false;
        nextPP = -1;
        speed = 3.5f;
        agent.destination = office.position;
        playerWasInControll = false;
    }

    // Update is called once per frame
    void Update()
    {        
        IsAtGoal();
        

        keyStrokeTimer -= Time.deltaTime;
        if (Input.anyKey)
        {
            PlayerCtrlMove();            
        }
    }          

    // True if agent is close enough to goal, otherwise false
    public void IsAtGoal()
    {        
        if (!arrived && (Vector3.Distance(agent.transform.position, agent.destination) <= 2.0f))
        {            
            move.Complete(true);            
            arrived = true;
            nextPP++;
        }
    }

    //Set variables that player controll the boss, read and move character
    public void PlayerCtrlMove()
    {
        keyStrokeTimer = 5.0f;
        agent.ResetPath();
        playerWasInControll = true;
        isNextPP = true;

        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        //To have an even speed and not move faster diagonal
        if (moveDirection.magnitude > 1)
            moveDirection /= moveDirection.magnitude;

        gameObject.transform.position += moveDirection * speed * Time.deltaTime;
        if(moveDirection.magnitude != 0)
            gameObject.transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    [Task]
    //Function to set Office as destination (called from tree)
    public void GoToOffice()
    {
        arrived = false;
        agent.destination = office.position;
        move = Task.current;
        isNextPP = true;
    }
    
    //Fucntion to find the closest patrolpoint to the boss
    public void FindClosestPP()
    {
        playerWasInControll = false;        
        NavMeshPath path = new NavMeshPath();
        float minPathLength = 99999.0f;

        //Loop trough all pp
        for (int i = 0; i < patrolPoints.Length+1; i++)
        {
            path.ClearCorners();
            float pathL = 0.0f;

            //Calculate the path
            if(i < patrolPoints.Length)
                agent.CalculatePath(patrolPoints[i], path);            
            else
                agent.CalculatePath(office.position, path);

            //If path is valid
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                //Calc the leght of the path            
                for (int j = 1; j < path.corners.Length; ++j)
                {
                    pathL += Vector3.Distance(path.corners[j - 1], path.corners[j]);
                }
                //If this path is shorter than the current min set it as the current destiantion
                if (pathL < minPathLength)
                {
                    minPathLength = pathL;
                    nextPP = i;
                }                
            }
        }        
    }

    [Task]
    //Function to set a patrolpoint as destination (called from tree)
    public void GoToPP()
    {        
        //If we have been in controll we need to find closest patrol point
        if (playerWasInControll)
        {
            FindClosestPP();
        }

        //If we are at the last patrolpoint we set an end to the patroling and let the tree know by isNextPP
        //Else we set next pp as our destiantion
        if (nextPP > patrolPoints.Length-1)
        {
            isNextPP = false;
            Task.current.Fail();
            nextPP = -1;
        }
        else
        {
            move = Task.current;
            agent.destination = patrolPoints[nextPP];                        
            arrived = false;
        }        
    }
}
