using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Rigidbody rb;
    BuilderManager bm;
    protected InputManager im;

    public FixedJoint joint;
    public Vector3 velocity;
    public bool isGameStarted;
    // Start is called before the first frame update
    public void Start()
    {
        isGameStarted = false;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;

        CreateJoint();
        bm = BuilderManager.Instance;
        im = InputManager.Instance;
        if (bm != null)
            bm.OnStartGame += StartGame;
        else
            StartGame();
    }

    protected virtual void CreateJoint()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.up, out hitInfo, 1.0f))
        {
            if (hitInfo.collider.CompareTag("Block") || hitInfo.collider.CompareTag("Core"))
            {
                FixedJoint joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = hitInfo.rigidbody;
                joint.connectedAnchor = hitInfo.transform.position;
                joint.anchor = transform.position;
                joint.breakForce = 900.0f;
                this.joint = joint;
                FixedJoint jointHit = hitInfo.collider.gameObject.AddComponent<FixedJoint>();
                jointHit.connectedBody = rb;
                jointHit.anchor = hitInfo.transform.position;
                jointHit.connectedAnchor = transform.position;
                jointHit.breakForce = 900.0f;
                BlockScript script = hitInfo.collider.gameObject.GetComponent<BlockScript>();
                if (script != null)
                    script.AddJointToList(jointHit);
            }
        }
    }

    protected virtual void StartGame()
    {
        rb.isKinematic = false;
        isGameStarted = true;
    }

    protected void OnDestroy()
    {
        if (bm != null)
            bm.OnStartGame -= StartGame;
    }
}
