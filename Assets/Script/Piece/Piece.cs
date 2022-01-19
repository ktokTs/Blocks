using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public List<int[]> Design{get; set;}
    public Vector3 WaitPoint{get; set;}
    public bool IsSet;
    void Start()
    {
        Design = PieceDesign.ReturnDesign(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ReturnWaitPoint()
    {
        this.transform.position = WaitPoint;
    }
}
