using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorScript : Movement
{
    ParticleSystem ps;
    [SerializeField] float force = 50.0f;

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
        ps = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (isGameStarted)
        {
            if (Input.GetKey(im.dicKey[NameInput.reactor]))
            {
                rb.AddForce(transform.up * force, ForceMode.Force);
                velocity = rb.velocity;
                velocity = Vector3.ClampMagnitude(rb.velocity, force * 2.0f);
                rb.velocity = velocity;
                if (!ps.isPlaying)
                    ps.Play();
            }
            else if (ps.isPlaying)
            {
                ps.Stop();
            }
        }
    }
}
