using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareDice : MonoBehaviour
{

    public float diceWidth;
    public Rigidbody rigid;
    public float epsilon = 0.05f;
    public float lerpStrength;

    public bool move = false;
    public Direction dir;

    public enum Direction
    {
        forward,
        back,
        left,
        right
    }

    private static readonly Vector3[] directions = new[]
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };

    private static readonly Vector3[] axes = new[]
    {
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
    };

    // Start is called before the first frame update
    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
        rigid.isKinematic = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            move = false;
            Move(dir);
        }
    }
    

    void Move(Direction e)
    {
        StartCoroutine(MoveAndRotate(e));
    }





    IEnumerator MoveAndRotate(Direction e)
    {
        Vector3 goalPosition = rigid.position + directions[(int)e] * (diceWidth * this.transform.localScale.x);
        Quaternion goalRot = rigid.rotation * Quaternion.Euler(axes[(int)e] * 90);
        
        while (Vector3.Distance(rigid.position, goalPosition) > epsilon)
        {
            
            rigid.MoveRotation(Quaternion.Lerp(rigid.rotation, goalRot, lerpStrength * Time.deltaTime));
            rigid.MovePosition(Vector3.Lerp(rigid.position, goalPosition, lerpStrength * Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
        rigid.MoveRotation(goalRot);
        rigid.MovePosition(goalPosition);
    }
}
