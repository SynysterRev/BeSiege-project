using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    public delegate void DelegateSaveGame(TypeMessage _typeFail);
    public event DelegateSaveGame OnFailedSaveGame;
    public event DelegateSaveGame OnSuccessSaveGame;
    private static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            return instance;
        }
    }

    string pathVehicle = Path.Combine(Application.streamingAssetsPath, "Vehicles/");

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        GameManager.Instance.OnReturnMenu += Destroy;
    }

    public void SaveVehicle(GameObject _objectToSave, string _name)
    {
        if (_name.Length == 0)
        {
            OnFailedSaveGame(TypeMessage.save);
            return;
        }
        if (!Directory.Exists(pathVehicle))
        {
            Directory.CreateDirectory(pathVehicle);
        }
        SavingObject savingObject = new SavingObject();
        savingObject.SaveData(_objectToSave, _name);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(pathVehicle + _name + ".save", FileMode.OpenOrCreate);
        bf.Serialize(file, savingObject);
        file.Close();
        OnSuccessSaveGame(TypeMessage.save);
        Debug.Log("VehicleSave");
    }

    public SavingObject LoadVehicle(string _name)
    {
        if (!Directory.Exists(pathVehicle))
        {
            Debug.Log("No vehicle saved");
            OnFailedSaveGame(TypeMessage.load);
            return null;
        }
        if (!File.Exists(pathVehicle + _name + ".save"))
        {
            Debug.Log("No vehicle saved");
            OnFailedSaveGame(TypeMessage.load);
            return null;
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(pathVehicle + _name + ".save", FileMode.Open);
        SavingObject so = (SavingObject)bf.Deserialize(file);
        OnSuccessSaveGame(TypeMessage.load);
        file.Close();
        return so;
    }

    public List<SavingObject> LoadAllVehicles()
    {
        if (!Directory.Exists(pathVehicle))
        {
            Debug.Log("No vehicle saved");
            return null;
        }
        List<SavingObject> vehicles = new List<SavingObject>();
        string[] filesName = Directory.GetFiles(pathVehicle, "*.save");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileToRead;
        foreach (string file in filesName)
        {
            fileToRead = File.Open(file, FileMode.Open);
            SavingObject so = (SavingObject)bf.Deserialize(fileToRead);
            vehicles.Add(so);
            fileToRead.Close();
        }
        return vehicles;
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnReturnMenu -= Destroy;
    }
}

[Serializable]
public class SavingObject
{
    public List<MyVector3> position;
    public List<MyVector3> scale;
    public List<MyVector3> rotation;

    public List<BuilderManager.BlockCategory> categoryBlock;
    public List<string> nameObject;
    public List<float> scaleForce;

    public string name;

    public SavingObject()
    {
        position = new List<MyVector3>();
        scale = new List<MyVector3>();
        rotation = new List<MyVector3>();

        categoryBlock = new List<BuilderManager.BlockCategory>();
        nameObject = new List<string>();
        scaleForce = new List<float>();
        name = "";
    }

    public void SaveData(GameObject _go, string _name)
    {
        foreach (Transform transform in _go.transform)
        {
            if (!transform.CompareTag("Core"))
            {
                position.Add(new MyVector3(transform.position));
                scale.Add(new MyVector3(transform.localScale));
                rotation.Add(new MyVector3(transform.rotation.eulerAngles));
                DataObject dob = transform.GetComponent<DataObject>();
                categoryBlock.Add(dob.blockCategory);
                nameObject.Add(dob.nameObject);
                scaleForce.Add(dob.scaleForce);
                name = _name;
            }
        }
    }
}

[Serializable]
public class MyVector3
{
    float x;
    float y;
    float z;

    public MyVector3(Vector3 vec)
    {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }

    public Vector3 CastIntoVector3()
    {
        Vector3 vec = new Vector3(x, y, z);
        return vec;
    }
}
