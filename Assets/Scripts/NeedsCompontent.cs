using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NeedsCompontent : MonoBehaviour
{

    public float energyLevel = 0.0f; 
    public float bladderLevel = 0.0f;
    //public float socialLevel;


    private void Awake()
    {
        energyLevel = Random.Range(5.0f, 15.0f);
        bladderLevel = Random.Range(0.5f, 1.0f);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateNeeds();
    }

    void updateNeeds() {
        energyLevel -= Time.deltaTime * Random.Range(0.0f, 1.0f);
        bladderLevel -= Time.deltaTime * Random.Range(0.0f, 0.02f);
    }


}
