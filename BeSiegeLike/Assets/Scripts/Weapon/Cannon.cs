using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    Rigidbody rb;
    FixedJoint joint;
    BuilderManager bm;
    InputManager im;
    [SerializeField] GameObject prefabBall = null;
    [SerializeField] Transform startBall = null;
    [SerializeField] float forceShot = 30.0f;
    [SerializeField] float coolDownShoot = 2.0f;

    bool isGameStarted = false;
    bool canShot = true;
    // Start is called before the first frame update
    void Start()
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

    // Update is called once per frame
    void Update()
    {
        if (isGameStarted && canShot && Input.GetKeyDown(im.dicKey[NameInput.shot]))
        {
            GameObject go = Instantiate(prefabBall, startBall.position, Quaternion.identity);
            if (go.GetComponent<Rigidbody>() != null)
            {
                go.GetComponent<Rigidbody>().AddForce(transform.forward * forceShot, ForceMode.Impulse);
            }
            StartCoroutine(CDRShot());
        }
    }

    void CreateJoint()
    {
        RaycastHit hitInfo;
        GetComponentInChildren<Collider>().enabled = false;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, 1.0f))
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
        GetComponentInChildren<Collider>().enabled = true;
    }

    IEnumerator CDRShot()
    {
        canShot = false;
        yield return new WaitForSeconds(coolDownShoot);
        canShot = true;
    }

    void StartGame()
    {
        rb.isKinematic = false;
        isGameStarted = true;
    }

    void OnDestroy()
    {
        if (bm != null)
            bm.OnStartGame -= StartGame;
    }
}
