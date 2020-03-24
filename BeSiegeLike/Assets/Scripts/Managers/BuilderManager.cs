using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuilderManager : MonoBehaviour
{
    public delegate void DelegateStartGame();
    public event DelegateStartGame OnStartGame;

    public delegate void DelegateLoadingVehicle();
    public event DelegateLoadingVehicle OnLoading;
    public event DelegateLoadingVehicle OnEndLoadingVehicle;
    public event DelegateLoadingVehicle OnEndLoading;

    [Serializable]
    public enum BlockCategory
    {
        core = -1,
        block,
        movement,
        weapon,
        total
    }

    private static BuilderManager instance;
    public static BuilderManager Instance
    {
        get
        {
            return instance;
        }

    }

    [SerializeField] GameObject prefabCore = null;
    public Dictionary<string, GameObject> dicoBlock;
    public Dictionary<BlockCategory, Dictionary<string, GameObject>> dicoBlocks;
    Camera cam;

    public GameObject selectedObject;
    GameObject objectToDestroy;
    public GameObject vehicle;

    Vector3 gravity;

    Color[] color;
    Renderer[] renderer;
    Color[] destroyColor;
    Color redColor;

    BlockCategory category;
    int currentBlock;
    string currentBlocks = "";
    string nameSave;

    public int nbBlock;

    bool isGameStarted;

    bool isInDeleteMode;

    public bool isLoading;

    public bool isReset;

    Quaternion rot;

    public float currentForce = 1.0f;

    public bool IsGameStarted { get => isGameStarted; }

    public string NameSave { set => nameSave = value; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }

        instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        InitDictionary();
        Initiliaze();
        OnStartGame += () => { };
        GameManager.Instance.OnReturnMenu += Destroy;
    }



    void InitDictionary()
    {
        AssetsManager am = AssetsManager.Instance;
        am.LoadAssetBundle("blockbundle");
        am.LoadAssetBundle("movementbundle");
        am.LoadAssetBundle("weaponbundle");
        //dicoBlocks = new Dictionary<BlockCategory, List<GameObject>>();
        dicoBlocks = new Dictionary<BlockCategory, Dictionary<string, GameObject>>();
        for (int i = 0; i < (int)BlockCategory.total; i++)
        {
            dicoBlocks[(BlockCategory)i] = new Dictionary<string, GameObject>();
        }
        foreach (GameObject go in am.LoadingAllAssets<GameObject>("blockbundle"))
        {
            dicoBlocks[BlockCategory.block].Add(go.name, go);
            if (currentBlocks == "")
                currentBlocks = go.name;
        }
        foreach (GameObject go in am.LoadingAllAssets<GameObject>("movementbundle"))
        {
            dicoBlocks[BlockCategory.movement].Add(go.name, go);
        }
        foreach (GameObject go in am.LoadingAllAssets<GameObject>("weaponbundle"))
        {
            dicoBlocks[BlockCategory.weapon].Add(go.name, go);
        }
    }

    void Update()
    {
        if (!isGameStarted && !isLoading)
        {
            if (isInDeleteMode)
            {
                DeletionMode();
            }
            else
            {
                EditionMode();
            }
        }
    }

    void EditionMode()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (!Physics.Raycast(hitInfo.transform.position, hitInfo.normal, 1.0f))
            {
                if (hitInfo.collider.gameObject.layer != 8 && hitInfo.collider.GetComponent<DataObject>().blockCategory != BlockCategory.movement
                && hitInfo.collider.GetComponent<DataObject>().blockCategory != BlockCategory.weapon)
                {
                    if (category == BlockCategory.movement)
                    {
                        if (selectedObject.CompareTag("Wheel"))
                        {
                            selectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.right, hitInfo.normal);
                        }
                        else
                            selectedObject.transform.rotation = Quaternion.FromToRotation(-Vector3.up, hitInfo.normal);
                    }
                    if (category == BlockCategory.weapon)
                    {
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            rot *= Quaternion.AngleAxis(-45.0f, transform.up);
                        }
                        else if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            rot *= Quaternion.AngleAxis(45.0f, transform.up);
                        }
                        selectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * rot;
                    }
                    //get size of the object and shift its position according to its size and the normal of hit
                    Vector3 shiftPos = Vector3.Scale(hitInfo.normal, Vector3.Scale(selectedObject.GetComponentInChildren<MeshRenderer>().bounds.size, selectedObject.transform.localScale) / 2.0f)
                        + Vector3.Scale(hitInfo.normal, Vector3.Scale(hitInfo.transform.GetComponent<MeshRenderer>().bounds.size, hitInfo.transform.localScale) / 2.0f);
                    if (selectedObject.CompareTag("Wheel"))
                    {
                        shiftPos += hitInfo.normal * 0.1f;
                    }
                    Vector3 pos = Vector3Int.FloorToInt(hitInfo.collider.transform.position) + shiftPos;
                    if (!selectedObject.activeSelf)
                        selectedObject.SetActive(true);
                    selectedObject.transform.position = pos;
                    if (Input.GetMouseButtonDown(0))
                    {
                        SetUpBlock(pos, hitInfo.collider.transform.parent);
                        CreateBlock();
                    }
                }
            }
        }
        else
        {
            if (selectedObject != null && selectedObject.activeSelf)
            {
                selectedObject.SetActive(false);
            }
        }
    }

    public void Initiliaze()
    {
        nameSave = "";
        isReset = false;
        isLoading = false;
        isGameStarted = false;
        gravity = Physics.gravity;
        Physics.gravity = Vector3.zero;
        cam = Camera.main;
        redColor = Color.red;
        redColor.a = 0.4f;
        vehicle = new GameObject();
        vehicle.name = "Vehicle";
        vehicle.transform.position = Vector3.zero;
        vehicle.tag = "Player";
        GameObject go = Instantiate(prefabCore, Vector3.zero, Quaternion.identity, vehicle.transform);
        category = BlockCategory.block;
        currentBlock = 0;
        nbBlock = 0;
        CreateBlock();
    }

    void DeletionMode()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            if (!hitInfo.collider.CompareTag("Core"))
            {
                SelectObjectToDestroy(hitInfo.collider.gameObject);
                if (Input.GetMouseButtonDown(0))
                {
                    if (objectToDestroy.CompareTag("Wheel") || objectToDestroy.CompareTag("Cannon"))
                    {
                        Destroy(objectToDestroy.transform.parent.gameObject);
                    }
                    else
                    {
                        Destroy(objectToDestroy);
                    }
                    nbBlock--;
                }
            }
        }
        else
        {
            DeselectObjectToDestroy();
        }
    }

    void SelectObjectToDestroy(GameObject go)
    {
        if (objectToDestroy == null || objectToDestroy != go)
        {
            DeselectObjectToDestroy();
            objectToDestroy = go;
            if (!objectToDestroy.CompareTag("Core"))
            {
                if (objectToDestroy.GetComponentsInChildren<Renderer>() != null)
                {
                    if (!objectToDestroy.transform.parent.CompareTag("Player"))
                    {
                        renderer = objectToDestroy.transform.parent.GetComponentsInChildren<Renderer>();
                    }
                    else
                    {
                        renderer = objectToDestroy.GetComponentsInChildren<Renderer>();
                    }
                    destroyColor = new Color[renderer.Length];

                    for (int i = 0; i < renderer.Length; i++)
                    {
                        destroyColor[i] = renderer[i].material.color;

                        renderer[i].material.color = redColor;
                    }
                }
            }
            else
            {
                objectToDestroy = null;
            }
        }
    }

    void DeselectObjectToDestroy()
    {
        if (objectToDestroy != null)
        {
            for (int i = 0; i < destroyColor.Length; i++)
            {
                renderer[i].material.color = destroyColor[i];
            }
        }
        objectToDestroy = null;
    }

    void SetUpBlock(Vector3 position, Transform parent)
    {
        selectedObject.transform.position = position;
        selectedObject.transform.SetParent(parent);
        //set layer so the block can be hit by raycast
        //when they are just preview they can't to avoid problem
        if (category == BlockCategory.movement || category == BlockCategory.weapon)
        {
            Transform tr;
            selectedObject.layer = 8;
            if (selectedObject.transform.parent != null && !selectedObject.transform.parent.CompareTag("Player"))
            {
                tr = selectedObject.transform.parent.transform;
            }
            else
            {
                tr = selectedObject.transform;
            }
            foreach (Transform transform in tr)
            {
                transform.gameObject.layer = 8;
            }
        }
        else
            selectedObject.layer = 0;

        for (int i = 0; i < color.Length; i++)
        {
            color[i].a = 1.0f;
            renderer[i].material.color = color[i];
        }
        selectedObject.name = category.ToString() + currentBlock;
        nbBlock++;
        //add the correct script according to the type of block
        EnableCorrectScript(ref selectedObject, category, currentBlocks, true);
    }

    void CreateBlock()
    {
        if (dicoBlocks.ContainsKey(category))
        {

            selectedObject = Instantiate(dicoBlocks[category][currentBlocks]);
            //the block will be transparent and be use as a preview
            if (selectedObject.GetComponentsInChildren<Renderer>() != null)
            {
                renderer = selectedObject.GetComponentsInChildren<Renderer>();
                color = new Color[renderer.Length];
                for (int i = 0; i < renderer.Length; i++)
                {
                    color[i] = renderer[i].material.color;
                    color[i].a = 0.4f;
                    renderer[i].material.color = color[i];
                }
            }
            EnableCorrectScript(ref selectedObject, category, currentBlocks, false);
            selectedObject.SetActive(false);
            rot = Quaternion.AngleAxis(0.0f, transform.up);
        }
    }

    void EnableCorrectScript(ref GameObject _go, BlockCategory _category, string _index, bool _enable)
    {
        DataObject dob = _go.GetComponent<DataObject>();
        if (dob != null)
        {
            dob.nameObject = _index;
            dob.scaleForce = currentForce;
        }
        switch (_category)
        {
            case BlockCategory.block:
                BlockScript bs = _go.GetComponentInChildren<BlockScript>();
                if (bs != null)
                {
                    bs.enabled = _enable;
                }
                if (dob != null)
                {
                    dob.blockCategory = BlockCategory.block;
                }
                break;
            case BlockCategory.movement:
                if (dob != null)
                {
                    dob.blockCategory = BlockCategory.movement;
                }
                BalloonScript bas = _go.GetComponentInChildren<BalloonScript>();
                if (bas != null)
                {
                    if (bas != null)
                        bas.enabled = _enable;
                }

                WheelScript sw = _go.GetComponentInChildren<WheelScript>();
                if (sw != null)
                {
                    sw.enabled = _enable;
                }

                ReactorScript rs = _go.GetComponentInChildren<ReactorScript>();
                if (rs != null)
                {
                    rs.enabled = _enable;
                }
                break;
            case BlockCategory.weapon:
                Cannon c = _go.GetComponentInChildren<Cannon>();
                if (c != null)
                {
                    c.enabled = _enable;
                }
                break;
            case BlockCategory.total:
                break;
            default:
                break;
        }
    }
    public void GoToEditionMode()
    {
        if (isInDeleteMode)
        {
            isInDeleteMode = false;
            CreateBlock();
        }
    }

    public void GoToDeleteMode()
    {
        if (!isInDeleteMode)
        {
            isInDeleteMode = true;
            Destroy(selectedObject);
        }
    }

    public void DeleteAll()
    {
        foreach (Transform transform in vehicle.transform)
        {
            if (!transform.CompareTag("Core"))
                Destroy(transform.gameObject);
        }
        nbBlock = 0;
    }

    public void StartGame()
    {
        if (!isLoading)
        {
            Physics.gravity = gravity;
            isGameStarted = true;
            Destroy(selectedObject);
            OnStartGame();
        }
    }

    public void ChangingBlock(BlockCategory typeBlock, string index)
    {
        if (!isInDeleteMode)
        {
            category = typeBlock;
            currentBlocks = index;
            Destroy(selectedObject);
            CreateBlock();
        }
    }

    public void SaveVehicle()
    {
        isLoading = true;
        SaveManager.Instance.SaveVehicle(vehicle, nameSave);
        isLoading = false;
    }

    public void LoadVehicle()
    {
        isLoading = true;
        OnLoading();
    }

    public void InstantiateVehicle(SavingObject _save)
    {
        if (vehicle == null)
        {
            vehicle = new GameObject();
            vehicle.transform.position = Vector3.zero;
            GameObject go = Instantiate(prefabCore, Vector3.zero, Quaternion.identity, vehicle.transform);
        }
        DeleteAll();
        StartCoroutine(Test(_save));
    }

    public void ValidateVehicleLoad()
    {
        OnEndLoading();
        isLoading = false;
    }

    IEnumerator Test(SavingObject _save)
    {
        if (_save != null)
        {
            for (int i = 0; i < _save.position.Count; i++)
            {
                GameObject go = Instantiate(dicoBlocks[_save.categoryBlock[i]][_save.nameObject[i]]);
                if (_save.categoryBlock[i] == BlockCategory.movement || _save.categoryBlock[i] == BlockCategory.weapon)
                {
                    Transform tr;
                    go.layer = 8;
                    if (go.transform.parent != null && !go.transform.parent.CompareTag("Player"))
                    {
                        tr = go.transform.parent.transform;
                    }
                    else
                    {
                        tr = go.transform;
                    }
                    foreach (Transform transform in tr)
                    {
                        transform.gameObject.layer = 8;
                    }
                }
                else
                    go.layer = 0;
                go.name = _save.categoryBlock[i].ToString() + i;
                go.transform.position = _save.position[i].CastIntoVector3();
                go.transform.localScale = _save.scale[i].CastIntoVector3();
                go.transform.rotation = Quaternion.Euler(_save.rotation[i].CastIntoVector3());
                go.transform.SetParent(vehicle.transform);
                DataObject dob = go.GetComponent<DataObject>();
                if (dob != null)
                {
                    dob.nameObject = _save.nameObject[i];
                    dob.scaleForce = _save.scaleForce[i];
                }
                yield return new WaitForSeconds(0.05f);
                nbBlock++;
            }
        }
        OnEndLoadingVehicle();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Physics.gravity = gravity;
        GameManager.Instance.OnReturnMenu -= Destroy;
    }
}
