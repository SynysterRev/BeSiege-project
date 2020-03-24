using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonScript : Movement
{
    [SerializeField] float force = 40.0f;

    public float Force
    {
        get
        {
            return force;
        }
        set
        {
            force = value;
        }
    }

    new void Start()
    {
        base.Start();
        force *= GetComponent<DataObject>().scaleForce;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted)
        {
            if (Input.GetKey(im.dicKey[NameInput.fly]))
            {
                rb.AddForce(-transform.up * force, ForceMode.Force);
                velocity = rb.velocity;
                velocity = Vector3.ClampMagnitude(rb.velocity, 30.0f);
                rb.velocity = velocity;
            }
        }
    }
}
