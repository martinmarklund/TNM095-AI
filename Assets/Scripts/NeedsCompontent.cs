using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NeedsCompontent : MonoBehaviour
{

    public float energyLevel; 
    public float bladderLevel;
    public float hygieneLevel;
    public float socialLevel = 1.0f;

    private void Awake()
    {
        energyLevel = Random.Range(90.0f, 250.0f);
        bladderLevel = Random.Range(0.5f, 2.0f);
        hygieneLevel = Random.Range(9.0f, 10.0f);
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
        energyLevel -= Time.deltaTime * Random.Range(0.0f, 5.0f);
        bladderLevel -= Time.deltaTime * Random.Range(0.0f, 0.02f);
        hygieneLevel -= 0.005f;
    }
}
