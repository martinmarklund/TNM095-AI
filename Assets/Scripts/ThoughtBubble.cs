using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubble : MonoBehaviour
{
    public RawImage thoughtBubble;
    public Texture[] thoughts = new Texture[3]; // Coffee, Work, Toilet
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        thoughtBubble.texture = thoughts[Random.Range(0,3)];
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {
        Vector3 bubblePosition = Camera.main.WorldToScreenPoint(this.transform.position);
        thoughtBubble.transform.position = bubblePosition;
    }
}
