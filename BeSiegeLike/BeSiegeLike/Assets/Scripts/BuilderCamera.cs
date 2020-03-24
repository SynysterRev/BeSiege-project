using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderCamera : MonoBehaviour
{
    //[SerializeField] Transform target;

    [SerializeField]
    Vector3 offSet;

    [SerializeField]
    float speed = 10.0f;

    [SerializeField]
    float angleMax = 89.0f;

    Vector3 destination;

    float rotX = 0.0f;
    float rotY = 0.0f;

    [SerializeField]
    float XrotationSpeed = 70.0f;

    [SerializeField]
    float YrotationSpeed = 70.0f;

    int nbObject;

    Vector3 target;

    Vector3 targetPosition;

    BuilderManager bm;

    float minDist = 5.0f;

    float maxDist = 50.0f;

    bool isGameStarted;

    // Start is called before the first frame update
    void Start()
    {
        nbObject = 0;
        target = Vector3.zero;
        bm = BuilderManager.Instance;
        bm.OnStartGame += StartGame;
    }
    //arranger camera
    // Update is called once per frame
    void Update()
    {
        Zoom();
        CalculateNewPosition();
        if (Input.GetMouseButton(1))
            RotateAroundTarget();
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, destination, speed);
        transform.LookAt(target);
    }

    void RotateAroundTarget()
    {
        rotX += Input.GetAxis("Mouse X") * XrotationSpeed;
        rotY += Input.GetAxis("Mouse Y") * YrotationSpeed;
        rotY = Mathf.Clamp(rotY, -angleMax, angleMax);
    }

    void CalculateNewPosition()
    {
        if(bm.nbBlock == 0)
        {
            nbObject = 0;
            target = Vector3.zero;
        }
        //camera will not target the core but a average of all the block
        else if (nbObject != bm.nbBlock)
        {
            target = Vector3.zero;
            foreach (Transform transform in bm.vehicle.transform)
            {
                target += transform.position;
            }
            target /= bm.nbBlock;
            nbObject = bm.nbBlock;
        }
        //multiply current rotation by forward (get direction) and by offset to keep camera at the right distance
        destination = Quaternion.Euler(rotY, rotX, 0.0f) * Vector3.forward * offSet.z + target;
    }

    void Zoom()
    {
        offSet.z -= Input.GetAxis("Mouse ScrollWheel") * 5.0f;
        offSet.z = Mathf.Clamp(offSet.z, minDist, maxDist);
    }

    void StartGame()
    {
        GetComponent<ThirdPersonCamera>().enabled = true;
        this.enabled = false;
    }
}
