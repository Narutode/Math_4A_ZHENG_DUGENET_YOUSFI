using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public LineConstrutor ls;

    public float Time;

    public TextMeshPro text;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Time = LineConstrutor.Timer;
        text.text = Time.ToString() + " milliseconde";
    }
}