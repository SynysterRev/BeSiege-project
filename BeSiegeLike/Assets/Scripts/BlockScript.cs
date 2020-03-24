using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockScript : MonoBehaviour
{
    Rigidbody rb;
    List<FixedJoint> joints;
    BuilderManager bm;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        joints = new List<FixedJoint>();
        bm = BuilderManager.Instance;

        FixedJoint[] jointsArray = GetComponents<FixedJoint>();
        CreateAllJoints();
        if (bm != null)
            bm.OnStartGame += StartGame;
        else
            StartGame();
    }

    void Update()
    {

    }

    void CreateAllJoints()
    {
        //raycast in all direction to link all block together foreveeer
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward.normalized, out hitInfo, 1.0f))
        {
            if (hitInfo.collider.CompareTag("Block") || hitInfo.collider.CompareTag("Core"))
            {
                CreateJoint(hitInfo);
            }
        }

        if (Physics.Raycast(transform.position, -transform.forward.normalized, out hitInfo, 1.0f))
        {
            if (hitInfo.collider.CompareTag("Block") || hitInfo.collider.CompareTag("Core"))
            {
                CreateJoint(hitInfo);
            }
        }

        if (Physics.Raycast(transform.position, transform.right.normalized, out hitInfo, 1.0f))
        {
            if (hitInfo.collider.CompareTag("Block") || hitInfo.collider.CompareTag("Core"))
            {
                CreateJoint(hitInfo);
            }
        }

        if (Physics.Raycast(transform.position, -transform.right.normalized, out hitInfo, 1.0f))
        {
            if (hitInfo.collider.CompareTag("Block") || hitInfo.collider.CompareTag("Core"))
            {
                CreateJoint(hitInfo);
            }
        }

        if (Physics.Raycast(transform.position, transform.up.normalized, out hitInfo, 1.0f))
        {
            if (hitInfo.collider.CompareTag("Block") || hitInfo.collider.CompareTag("Core"))
            {
                CreateJoint(hitInfo);
            }
        }

        if (Physics.Raycast(transform.position, -transform.up.normalized, out hitInfo, 1.0f))
        {
            if (hitInfo.collider.CompareTag("Block") || hitInfo.collider.CompareTag("Core"))
            {
                CreateJoint(hitInfo);
            }
        }
    }

    void CreateJoint(RaycastHit hitInfo)
    {
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = hitInfo.rigidbody;
        joint.connectedAnchor = hitInfo.transform.position;
        joint.anchor = transform.position;
        joint.breakForce = 900.0f;
        AddJointToList(joint);
        FixedJoint jointHit = hitInfo.collider.gameObject.AddComponent<FixedJoint>();
        jointHit.connectedBody = rb;
        jointHit.anchor = hitInfo.transform.position;
        jointHit.connectedAnchor = transform.position;
        jointHit.breakForce = 900.0f;
        BlockScript script = hitInfo.collider.gameObject.GetComponent<BlockScript>();
        if (script != null)
            script.AddJointToList(jointHit);
    }

    public void AddJointToList(FixedJoint _joint)
    {
        if (joints.Any(item => item.connectedBody == _joint.connectedBody))
        {
            Destroy(_joint);
        }
        else joints.Add(_joint);
    }

    void StartGame()
    {
        if (rb == null)
            Destroy(gameObject);
        rb.isKinematic = false;

        for (int i = 0; i < joints.Count; i++)
        {
            if (joints[i].connectedBody == null)
            {
                FixedJoint f = joints[i];
                joints.Remove(joints[i]);
                Destroy(f);
            }
        }
    }

    private void OnDestroy()
    {
        if (bm != null)
            bm.OnStartGame -= StartGame;
    }
}
