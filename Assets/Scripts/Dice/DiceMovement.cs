using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMovement : MonoBehaviour
{

    public int moveDirection;
    public bool Move;
    
    
    public float edgeLength;
    public int numEdges = 3;

    public float lerpStrength;
    public Rigidbody rigid;

    public float rotationAngle = 90 + 45;
    public float epsilon = 0.05f;

    public Vector3[] directions;

    // Start is called before the first frame update
    void Start() {
        rigid = this.GetComponent<Rigidbody>();
        rigid.isKinematic = true;
        FindMovementDirections();
    }

    void FindMovementDirections()
    {
        directions = new Vector3[numEdges];
        for (int i = 0; i < numEdges; i++)
        {
            directions[i] = new Vector3(Mathf.Sin(i * 2 * Mathf.PI / numEdges), 0, Mathf.Cos(i * 2 * Mathf.PI / numEdges));
        }
    }

    // Update is called once per frame
    void Update() {
        if (Move)
        {
            Move = false;
            StartMove(moveDirection);
        }
    }

    void StartMove(int direction) {
        direction = Mathf.Abs(direction % numEdges); //So we dont get those nasty errors
        StartCoroutine(MoveAndRotate(direction));
    }

    public Vector3 goalPosition;
    public Vector3 angleOfRotation;
    IEnumerator MoveAndRotate(int direction)
    {
        float time = Time.time;
        goalPosition = rigid.position + directions[direction] * (edgeLength * 0.866f * this.transform.localScale.x);
        Vector3 speedAndDir = directions[direction] * (edgeLength * 0.866f * this.transform.localScale.x);

        angleOfRotation = Vector3.Cross(Vector3.left, directions[direction]);
        Quaternion goalRot = rigid.rotation * Quaternion.AngleAxis(rotationAngle, angleOfRotation);
        
        while (Vector3.Distance(rigid.position, goalPosition) > epsilon)
        {
            
            rigid.MoveRotation(Quaternion.Lerp(rigid.rotation, goalRot, lerpStrength * Time.deltaTime));
            rigid.MovePosition(Vector3.Lerp(rigid.position, goalPosition, lerpStrength * Time.deltaTime));
            yield return new WaitForEndOfFrame();
            Debug.DrawLine(this.transform.position, this.transform.position + angleOfRotation * 2, Color.green);
            Debug.DrawLine(this.transform.position, goalPosition, Color.red);
        }
        rigid.MoveRotation(goalRot);
        rigid.MovePosition(goalPosition);
    }
}
