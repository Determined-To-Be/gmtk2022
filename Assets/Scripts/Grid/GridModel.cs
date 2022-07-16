using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GridModel : MonoBehaviour
{

    public GridData gridData;
    public UnityEvent UpdateBoard;
    public UnityEvent ResetBoard;

    protected void Awake()
    {
        UpdateBoard = new UnityEvent();
        ResetBoard = new UnityEvent();
        //base.Awake();
        gridData.Generate();
    }

    protected void Start() {
        ResetBoard.Invoke();
    }

}
