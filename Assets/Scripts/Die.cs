using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Die : MonoBehaviour
{
    [SerializeField] private float force = 1.0f;
    [SerializeField] private float forceHeight = 3.0f;
    [SerializeField] private int n = 3;
    [SerializeField] private float startAngle = 0.0f;
    private float angle;
    private float currentAngle;
    private float r = 2.0f;
    private Rigidbody rb;
    private Color color;
    private Transform tf;
    private AudioSource source;

    private void Awake()
    {
        tf = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
        color = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);
    }

    private void Start()
    {
        Audio.GetInstance().Register(source, "Dice");
    }

    private void Update()
    {
        Vector3 center = transform.position + rb.centerOfMass;
        center.y = forceHeight;
        foreach (Vector3 v in Directions())
        {
            Debug.DrawRay(center + (v * r), v, color);
        }
    }

    private List<Vector3> Directions()
    {
        currentAngle = tf.eulerAngles.y + startAngle;
        List<Vector3> directions = new List<Vector3>(n);
        angle = 360.0f / n;
        for (float f = 0.0f; f < 360.0f; f += angle)
        {
            Vector3 v = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (currentAngle + f)), 0.0f, Mathf.Sin(Mathf.Deg2Rad * (currentAngle + f)));
            v = Vector3.Normalize(v);
            directions.Add(v);
        }
        return directions;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Audio.GetInstance().RollDice(source);
            Vector2 temp = context.action.ReadValue<Vector2>();
            Vector3 v = new Vector3(temp.x, 0.0f, temp.y);
            v = Vector3.Normalize(v);

            float min = float.MaxValue;
            Vector3 direction = v;
            foreach (Vector3 u in Directions())
            {
                float distance = Vector3.Distance(v, u);
                if (distance < min)
                {
                    direction = u;
                    min = distance;
                }
            }

            Vector3 center = transform.position + rb.centerOfMass;
            center.y += forceHeight;
            rb.AddForceAtPosition(direction * force, center);
            Debug.DrawRay(center + (direction * r), direction, Color.white, 3.0f);
            Debug.DrawRay(center + (v * r), v, Color.black, 3.0f);
        }
    }
}
