using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionEndLevel : MonoBehaviour
{
    [SerializeField] int nbWallToDestroy = 4;

    [SerializeField] TextMesh textMesh;

    int currentWallDestroy = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IsBroken()
    {
        currentWallDestroy++;
        if (currentWallDestroy >= nbWallToDestroy)
        {
            textMesh.text = "Amazing";
            GameManager.Instance.LevelFinished();
        }
    }
}
