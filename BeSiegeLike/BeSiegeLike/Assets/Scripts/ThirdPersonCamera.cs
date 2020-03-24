using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform target;

    [SerializeField]
    Vector3 offSet = Vector3.zero;

    Vector3 destination = Vector3.zero;
    Vector3 targetPos = Vector3.zero;

    float rotX = 0.0f;
    float rotY = 0.0f;

    [SerializeField]
    float damping = 10.0f;

    [SerializeField]
    float XrotationSpeed = 70.0f;

    [SerializeField]
    float YrotationSpeed = 70.0f;

    [SerializeField]
    float speed = 10.0f;

    [SerializeField]
    bool ReverseXAxis = false;

    [SerializeField]
    bool ReverseYAxis = false;

    float correctedOffsetZ = 0.0f;

    int mask = 0;

    void Start()
    {

        if (target == null)
        {
            if (GameObject.FindGameObjectWithTag("Core") == null)
                Debug.LogError("can't find core");
            else
                target = GameObject.FindGameObjectWithTag("Core").transform;
        }
        mask = 1 << 8;
        mask = ~mask;
    }

    void Update()
    {
        RotateAroundPlayer();
        CalculateNewPosition();
    }

    private void LateUpdate()
    {
        FollowPlayer();
        LookTarget();
    }

    //keep focus on the player
    void LookTarget()
    {
        Quaternion rotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, damping);
    }

    void CalculateNewPosition()
    {
        targetPos = target.position + Vector3.up * offSet.y;
        TestColliding(ref correctedOffsetZ, ref speed);

        //multiply current rotation by forward (get direction) and by offset to keep camera at the right distance
        destination = Quaternion.Euler(rotY, rotX, 0.0f) * Vector3.forward * correctedOffsetZ + targetPos;
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, destination, speed);
    }

    void RotateAroundPlayer()
    {
        rotX += Input.GetAxis("HorizontalCamera") * XrotationSpeed * Time.deltaTime * (ReverseXAxis ? -1 : 1);

        rotY += Input.GetAxis("VerticalCamera") * YrotationSpeed * Time.deltaTime * (ReverseYAxis ? -1 : 1);
        rotY = Mathf.Clamp(rotY, -55.0f, 70.0f);
    }

    //sphereraycast, if collision adjust distance to stay close to the wall and not inside
    void TestColliding(ref float correctedZ, ref float speed)
    {
        RaycastHit raycastHit;
        Vector3 dirToCam = (transform.position - targetPos).normalized;
        Debug.DrawRay(targetPos, dirToCam * -offSet.z, Color.yellow);


        if (Physics.SphereCast(targetPos, 0.4f, dirToCam, out raycastHit, -offSet.z, mask))
        {
            //teleport camera instead of lerping inside wall
            if (raycastHit.distance < -offSet.z)
            {
                transform.position = raycastHit.point + raycastHit.normal * 0.4f;
            }
            else
            {
                //correct distance between player and camera
                correctedZ = -raycastHit.distance;
            }
        }
        else
        {
            correctedZ = offSet.z;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.4f);
    }
}
