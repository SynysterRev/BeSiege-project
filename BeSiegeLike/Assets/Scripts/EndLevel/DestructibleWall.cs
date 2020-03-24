using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    List<GameObject> child;
    int nbChild;
    bool isOver;
    // Start is called before the first frame update
    void Start()
    {
        child = new List<GameObject>();
        foreach (Transform tr in transform)
        {
            child.Add(tr.gameObject);
            tr.GetComponent<Test>().OnDestru += ClearList;
        }
        nbChild = child.Count;
        isOver = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ClearList(GameObject _go)
    {
        if (child.Contains(_go))
        {
            _go.GetComponent<Test>().OnDestru -= ClearList;
            child.Remove(_go);
        }
        if (!isOver && child.Count <= nbChild / 2.0f)
        {
            isOver = true;
            if (GetComponentInParent<DestructionEndLevel>() != null)
                GetComponentInParent<DestructionEndLevel>().IsBroken();
            Destroy(gameObject);
        }
    }
}
