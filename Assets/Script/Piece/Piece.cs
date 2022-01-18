using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField]
    public List<int[]> Design{get; set;}
    string PrefabName = "";
    void Start()
    {
        Design = PieceDesign.ReturnDesign(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
