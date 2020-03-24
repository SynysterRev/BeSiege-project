using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NameInput
{
    forward,
    backward,
    left,
    right,
    reactor,
    shot,
    fly
}
public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    [SerializeField] public KeyCode forward;
    [SerializeField] public KeyCode backward;
    [SerializeField] public KeyCode left;
    [SerializeField] public KeyCode right;
    [SerializeField] public KeyCode reactor;
    [SerializeField] public KeyCode shot;
    [SerializeField] public KeyCode fly;

    public Dictionary<NameInput, KeyCode> dicKey;
    public static InputManager Instance
    {
        get
        {
            return instance;
        }

    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
        dicKey = new Dictionary<NameInput, KeyCode>();
        dicKey[NameInput.forward] = forward;
        dicKey[NameInput.backward] = backward;
        dicKey[NameInput.left] = left;
        dicKey[NameInput.right] = right;
        dicKey[NameInput.reactor] = reactor;
        dicKey[NameInput.shot] = shot;
        dicKey[NameInput.fly] = fly;
    }

    public int ChangeInput(int index, KeyCode keyCode)
    {
        int returnedIndex = -1;
        foreach (NameInput name in Enum.GetValues(typeof(NameInput)))
        {
            if (dicKey[name] == keyCode)
            {
                dicKey[name] = KeyCode.None;
                returnedIndex = (int)name;
            }
        }
        dicKey[(NameInput)index] = keyCode;
        Debug.Log(dicKey[(NameInput)index]);
        return returnedIndex;
    }
}
