using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtBubble : MonoBehaviour
{
    private Transform pivotPosition;
    public Camera cam;
    private float angle;

    void Start() {
        pivotPosition = transform;
    }
    // Update is called once per frame
    private void Update() {
        // Check the pivot point's rotation relative to camera view direction and update it accordingly
        angle = Vector3.Angle(cam.transform.forward, pivotPosition.forward);
        Rotate(angle);
    }

    // Updates the rotation
    private void Rotate(float diff) {
        float xRot = pivotPosition.rotation.x;
        
        float zRot = pivotPosition.rotation.z;

        pivotPosition.eulerAngles = new Vector3(xRot,90+diff,zRot);
        
    }
}
