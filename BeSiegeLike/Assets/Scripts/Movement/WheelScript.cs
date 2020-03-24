using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelScript : Movement
{
    new HingeJoint hingeJoint;
    JointMotor jointMotorForward;
    JointMotor jointMotorBackward;
    JointMotor jointMotorRight;
    JointMotor jointMotorLeft;
    JointMotor jointMotorStop;

    bool inputForward;
    bool inputBackward;
    bool inputLeft;
    bool inputRight;

    [SerializeField] float force = 350.0f;

    [SerializeField] float targetVelocity = 350.0f;

    [SerializeField] float forceTurn = 450.0f;

    [SerializeField] float targetVelocityTurn = 450.0f;

    [SerializeField] bool isDriving = false;

    [SerializeField] SpriteRenderer arrow;

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

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        force *= GetComponent<DataObject>().scaleForce;

        jointMotorForward = new JointMotor();
        jointMotorForward.force = force;
        jointMotorForward.targetVelocity = force;

        jointMotorBackward = new JointMotor();
        jointMotorBackward.force = force;
        jointMotorBackward.targetVelocity = -force;

        jointMotorRight = new JointMotor();
        jointMotorRight.force = force + 100.0f;
        jointMotorRight.targetVelocity = -force - 100.0f;

        jointMotorLeft = new JointMotor();
        jointMotorLeft.force = force + 100.0f;
        jointMotorLeft.targetVelocity = force + 100.0f;

        jointMotorStop = new JointMotor();
        jointMotorStop.force = 0.0f;
        jointMotorStop.targetVelocity = 0.0f;

        arrow.enabled = true;
        //CreateHingeJoint();

    }

    protected override void StartGame()
    {
        base.StartGame();
        Destroy(arrow);
    }

    protected override void CreateJoint() 
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.right, out hitInfo, 1.0f))
        {
            if (hitInfo.collider.CompareTag("Block") || hitInfo.collider.CompareTag("Core"))
            {
                HingeJoint joint = gameObject.AddComponent<HingeJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedBody = hitInfo.rigidbody;
                //use to set anchor in the same direction, so motor will rotate all the wheels in the same direction
                joint.axis = Vector3.right * GetNormalSign(hitInfo.normal);
                joint.connectedAnchor = hitInfo.transform.InverseTransformPoint(hitInfo.point) + transform.right * 0.3f;
                joint.anchor = Vector3.zero;
                joint.breakForce = 1500.0f;
                arrow.transform.Rotate(Vector3.up * GetSign(hitInfo.normal), 180.0f);
                joint.motor = jointMotorForward;
                joint.useMotor = true;
                hingeJoint = joint;
            }
        }
    }

    int GetNormalSign(Vector3 normal)
    {
        if (normal.x < 0.0f || normal.y < 0.0f || normal.z < 0.0f)
            return -1;
        else
            return 1;
    }

    int GetSign(Vector3 normal)
    {
        if (normal.x < 0.0f || normal.y < 0.0f || normal.z < 0.0f)
            return -1;
        else
            return 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted)
        {
            if (hingeJoint != null)
            {
                GetInput();
                if (inputForward)
                {
                    hingeJoint.motor = jointMotorForward;
                    //hingeJoint.useMotor = true;
                }
                if (inputBackward)
                {
                    hingeJoint.motor = jointMotorBackward;
                    //hingeJoint.useMotor = true;
                }

                if (isDriving)
                {
                    if (inputRight)
                    {
                        //hingeJoint.useMotor = true;
                        if (hingeJoint.axis.x > 0.0f)
                        {
                            hingeJoint.motor = jointMotorRight;
                        }
                        else
                        {
                            hingeJoint.motor = jointMotorLeft;
                        }
                    }
                    if (inputLeft)
                    {
                        //hingeJoint.useMotor = true;
                        if (hingeJoint.axis.x < 0.0f)
                        {
                            hingeJoint.motor = jointMotorRight;
                        }
                        else
                        {
                            hingeJoint.motor = jointMotorLeft;
                        }
                    }
                }
                if (!inputForward && !inputBackward
                    && !inputLeft && !inputRight)
                {
                    hingeJoint.motor = jointMotorStop;
                    //hingeJoint.useMotor = false;
                }
            }
        }
    }

    void GetInput()
    {
        inputForward = Input.GetKey(im.dicKey[NameInput.forward]);
        inputBackward = Input.GetKey(im.dicKey[NameInput.backward]);
        inputLeft = Input.GetKey(im.dicKey[NameInput.left]);
        inputRight = Input.GetKey(im.dicKey[NameInput.right]);
    }
}
