using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float timerDestruction = 10.0f;
    [SerializeField] float forceExplosion = 1000.0f;
    [SerializeField] float areaExplosion = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, timerDestruction);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider[] collider = Physics.OverlapSphere(collision.contacts[0].point, areaExplosion);
        foreach (Collider collider1 in collider)
        {

            Rigidbody rb = collider1.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(forceExplosion, collision.contacts[0].point, areaExplosion, 1.0f, ForceMode.Impulse);
                if (collider1.CompareTag("Destructible"))
                    Destroy(collider1.gameObject, timerDestruction / 2.0f);
            }
        }
        Destroy(gameObject);
    }
}
