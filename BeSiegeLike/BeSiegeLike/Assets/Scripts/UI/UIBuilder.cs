using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypeMessage
{
    save,
    load,
    levelDone,
    death
}
public class UIBuilder : MonoBehaviour
{
    [SerializeField] RectTransform startPos = null;
    [SerializeField] Transform parent = null;
    [SerializeField] Button[] buttonCanvas = null;
    [SerializeField] Font font = null;

    [SerializeField] Button load = null;

    [SerializeField] Button save = null;

    [SerializeField] Button edition = null;

    [SerializeField] Button destruction = null;

    [SerializeField] Button menu = null;

    [SerializeField] Button menuPause = null;

    [SerializeField] Button inputButton = null;

    [SerializeField] Button reset = null;

    [SerializeField] Button destructionAll = null;

    [SerializeField] Button play = null;

    [SerializeField] GameObject editionMode = null;

    [SerializeField] GameObject loadingMode = null;

    [SerializeField] GameObject pauseMode = null;

    [SerializeField] GameObject inputMode = null;

    [SerializeField] Vector2 sizeImage = Vector2.zero;

    [SerializeField] Text textToFill = null;

    [SerializeField] Text textToDisable = null;
    [SerializeField] Text failedText = null;

    [SerializeField] Slider slider = null;
    [SerializeField] Text sliderText = null;

    BuilderManager bm;
    AssetsManager am;
    SaveManager sm;
    GameManager gm;
    Dictionary<BuilderManager.BlockCategory, Dictionary<string, Sprite>> dicoBlocks;

    Dictionary<BuilderManager.BlockCategory, List<Button>> buttons;
    BuilderManager.BlockCategory currentType;

    // Start is called before the first frame update
    void Start()
    {
        buttons = new Dictionary<BuilderManager.BlockCategory, List<Button>>();
        bm = BuilderManager.Instance;
        am = AssetsManager.Instance;
        sm = SaveManager.Instance;
        gm = GameManager.Instance;
        am.LoadAssetBundle("spriteblockbundle");
        am.LoadAssetBundle("spritemovementbundle");
        am.LoadAssetBundle("spriteweaponbundle");

        bm.OnStartGame += DisableHUDOnStartGame;
        bm.OnLoading += OnLoading;
        sm.OnFailedSaveGame += PrintFailed;
        sm.OnSuccessSaveGame += PrintSuccess;
        GameManager.Instance.OnPause += OnPause;
        gm.OnEndLevel += PrintSuccess;

        dicoBlocks = new Dictionary<BuilderManager.BlockCategory, Dictionary<string, Sprite>>();
        //load all sprite from every sprite bundle
        //it will be change in the future
        for (int i = 0; i < (int)BuilderManager.BlockCategory.total; i++)
            dicoBlocks[(BuilderManager.BlockCategory)i] = new Dictionary<string, Sprite>();

        foreach (Sprite go in am.LoadingAllAssets<Sprite>("spriteblockbundle"))
        {
            dicoBlocks[BuilderManager.BlockCategory.block].Add(go.name, go);
        }
        foreach (Sprite go in am.LoadingAllAssets<Sprite>("spritemovementbundle"))
        {
            dicoBlocks[BuilderManager.BlockCategory.movement].Add(go.name, go);
        }
        foreach (Sprite go in am.LoadingAllAssets<Sprite>("spriteweaponbundle"))
        {
            dicoBlocks[BuilderManager.BlockCategory.weapon].Add(go.name, go);
        }
        CreateInterface(BuilderManager.BlockCategory.block);
        CreateInterface(BuilderManager.BlockCategory.movement);
        CreateInterface(BuilderManager.BlockCategory.weapon);

        for (int i = 0; i < buttonCanvas.Length; i++)
        {
            BuilderManager.BlockCategory type = (BuilderManager.BlockCategory)i;
            buttonCanvas[i].onClick.AddListener(() => ChangeTypeBlock(type));
        }

        load.onClick.AddListener(() => bm.LoadVehicle());
        save.onClick.AddListener(() => bm.SaveVehicle());
        edition.onClick.AddListener(() => bm.GoToEditionMode());
        destruction.onClick.AddListener(() => bm.GoToDeleteMode());
        destructionAll.onClick.AddListener(() => bm.DeleteAll());
        play.onClick.AddListener(() => bm.StartGame());
        menu.onClick.AddListener(() => gm.GoBackMenu());
        menuPause.onClick.AddListener(() => gm.GoBackMenu());
        reset.onClick.AddListener(() => gm.ResetLevel());

        slider.onValueChanged.AddListener(ChangeForce);

        inputButton.onClick.AddListener(OnInput);
    }

    //foreach kind of block it will create a button to change the current block by the one's click
    void CreateInterface(BuilderManager.BlockCategory typeBlock)
    {
        buttons[typeBlock] = new List<Button>();
        int i = 0;
        foreach (KeyValuePair<string, Sprite> item in dicoBlocks[typeBlock])
        {

            GameObject tmpImg = new GameObject();
            tmpImg.name = typeBlock.ToString() + i;
            tmpImg.AddComponent<RectTransform>();
            tmpImg.transform.SetParent(parent);
            Vector2 tmpPos = Vector2.zero;
            RectTransform tmpRect = tmpImg.GetComponent<RectTransform>();
            tmpPos.x = startPos.anchoredPosition.x + i * (sizeImage.x + 80.0f);
            tmpPos.y = startPos.anchoredPosition.y;
            tmpRect.pivot = Vector2.zero;
            tmpRect.anchorMin = Vector2.zero;
            tmpRect.anchorMax = Vector2.zero;
            tmpRect.sizeDelta = sizeImage;
            tmpRect.localScale = Vector3.one;
            tmpRect.anchoredPosition = tmpPos;
            tmpImg.AddComponent<Image>().sprite = item.Value;
            Button button = tmpImg.AddComponent<Button>();
            button.targetGraphic = tmpImg.GetComponent<Image>();
            string index = item.Key;
            button.onClick.AddListener(() => SelectItem(typeBlock, index));
            if (typeBlock != BuilderManager.BlockCategory.block)
                button.gameObject.SetActive(false);
            buttons[typeBlock].Add(button);


            GameObject tmptxt = new GameObject();
            RectTransform tmpRect2 = tmptxt.AddComponent<RectTransform>();
            tmptxt.transform.SetParent(tmpImg.transform);
            tmpPos = Vector2.zero;
            tmpPos.x = -sizeImage.x / 2.0f;
            tmpPos.y = sizeImage.y / 2.0f;
            tmpRect2.pivot = new Vector2(0.0f, 1.0f);
            tmpRect2.anchorMin = new Vector2(0.0f, 1.0f);
            tmpRect2.anchorMax = new Vector2(0.0f, 1.0f);
            tmpRect2.sizeDelta = new Vector2(100.0f, 20.0f);
            tmpRect2.localScale = Vector3.one;
            tmpRect2.anchoredPosition = tmpPos;

            Text text = tmptxt.AddComponent<Text>();
            text.text = item.Key;
            text.font = font;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 20;
            i++;
        }
    }

    void ChangeTypeBlock(BuilderManager.BlockCategory typeBlock)
    {
        foreach (Button button in buttons[currentType])
        {
            button.gameObject.SetActive(false);
        }
        currentType = typeBlock;
        foreach (Button button in buttons[currentType])
        {
            button.gameObject.SetActive(true);
        }
    }

    public void ChangeForce(float _value)
    {

        sliderText.text = _value.ToString("F2");
        bm.currentForce = _value;
    }

    void SelectItem(BuilderManager.BlockCategory typeBlock, string index)
    {
        if (typeBlock == BuilderManager.BlockCategory.movement)
        {
            slider.interactable = true;
            slider.value = slider.maxValue;
        }
        else
        {
            slider.interactable = false;
        }
        bm.ChangingBlock(typeBlock, index);
    }

    void DisableHUDOnStartGame()
    {
        editionMode.SetActive(false);
    }

    public void DisableText()
    {
        textToDisable.enabled = false;
    }

    void OnLoading()
    {
        loadingMode.SetActive(true);
        editionMode.SetActive(false);
        GetComponent<UILoading>().enabled = true;
        enabled = false;
    }

    void OnInput()
    {
        inputMode.SetActive(true);
        editionMode.SetActive(false);
        GetComponent<UIInput>().enabled = true;
        bm.isLoading = true;
        enabled = false;
    }

    void OnPause(bool _pause)
    {
        if (!_pause)
        {
            pauseMode.SetActive(false);
        }
        else
        {
            pauseMode.SetActive(true);
        }
    }

    public void CheckTextFill()
    {
        if (textToFill.text.Length == 0)
        {
            textToDisable.enabled = true;
        }
        bm.NameSave = textToFill.text;
    }

    void PrintSuccess(TypeMessage _type)
    {
        switch (_type)
        {
            case TypeMessage.save:
                failedText.text = "Save done";
                break;
            case TypeMessage.load:
                failedText.text = "Load done";
                bm.NameSave = textToFill.text;
                break;
            case TypeMessage.levelDone:
                failedText.text = "Well done !";
                break;
            case TypeMessage.death:
                break;
            default:
                break;
        }
        StartCoroutine(TimerText());
    }

    void PrintFailed(TypeMessage _type)
    {
        switch (_type)
        {
            case TypeMessage.save:
                failedText.text = "Enter a name please";
                break;
            case TypeMessage.load:
                failedText.text = "No save found";
                break;
            case TypeMessage.death:
                break;
            default:
                break;
        }
        StartCoroutine(TimerText());
    }

    IEnumerator TimerText()
    {
        failedText.enabled = true;
        yield return new WaitForSeconds(1.5f);
        failedText.enabled = false;
    }

    private void OnDestroy()
    {
        bm.OnStartGame -= DisableHUDOnStartGame;
        bm.OnLoading -= OnLoading;
        sm.OnFailedSaveGame -= PrintFailed;
        sm.OnSuccessSaveGame -= PrintSuccess;
        gm.OnEndLevel -= PrintSuccess;
        gm.OnPause -= OnPause;
    }
}
