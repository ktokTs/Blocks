using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public List<int[]> Design{get; set;}
    public Vector3 WaitPoint{get; set;}
    public bool IsSet; //置かれた後のピースか判断 あまり必要ないかも
    void Start()
    {
        Design = PieceDesign.ReturnDesign(this.gameObject);
    }

    void Update()
    {
    }

    public void ReturnWaitPoint()
    {
        this.transform.position = WaitPoint;
    }
}
