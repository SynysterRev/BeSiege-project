using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : MonoBehaviour
{
    [SerializeField] RectTransform startPos = null;
    [SerializeField] Vector2 sizeImage = Vector2.zero;
    [SerializeField] Font font = null;
    [SerializeField] Sprite sprite = null;

    [SerializeField] Button validate;

    [SerializeField] GameObject editionMode = null;
    [SerializeField] GameObject loadingMode = null;

    [SerializeField] Button ok = null;
    SaveManager sm = null;
    BuilderManager bm = null;
    List<SavingObject> vehicles = null;
    [SerializeField] Color whiteTransparent = Color.white;

    List<GameObject> buttons;

    bool isLoading;
    // Start is called before the first frame update
    void Start()
    {
        sm = SaveManager.Instance;
        bm = BuilderManager.Instance;
        bm.OnEndLoading += OnEndLoading;
        bm.OnEndLoadingVehicle += EndLoading;

        isLoading = false;
        validate.onClick.AddListener(() => bm.ValidateVehicleLoad());
        enabled = false;
        //whiteTransparent = new Color(255.0f, 255.0f, 255.0f, 130.0f);

    }

    private void OnEnable()
    {
        buttons = new List<GameObject>();
        if (sm != null)
        {
            vehicles = new List<SavingObject>();
            vehicles = sm.LoadAllVehicles();
            if (vehicles != null)
                CreateInterface();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateInterface()
    {
        for (int i = 0; i < vehicles.Count; i++)
        {
            GameObject tmpImg = new GameObject();
            tmpImg.name = vehicles[i].name;
            tmpImg.transform.SetParent(loadingMode.transform);
            Vector2 tmpPos = Vector3.zero;
            RectTransform tmpRect = tmpImg.AddComponent<RectTransform>();
            tmpPos.x = startPos.anchoredPosition.x;
            tmpPos.y = startPos.anchoredPosition.y - i * (sizeImage.y + 10.0f);
            tmpRect.pivot = new Vector2(0.0f, 1.0f);
            tmpRect.anchorMin = new Vector2(0.0f, 1.0f);
            tmpRect.anchorMax = new Vector2(0.0f, 1.0f);
            tmpRect.sizeDelta = sizeImage;
            tmpRect.localScale = Vector3.one;
            tmpRect.anchoredPosition = tmpPos;

            GameObject tmptxt = new GameObject();
            RectTransform tmpRect2 = tmptxt.AddComponent<RectTransform>();
            tmptxt.transform.SetParent(tmpImg.transform);
            tmpRect2.pivot = new Vector2(0.0f, 1.0f);
            tmpRect2.anchorMin = new Vector2(0.0f, 1.0f);
            tmpRect2.anchorMax = new Vector2(0.0f, 1.0f);
            tmpRect2.sizeDelta = sizeImage;
            tmpRect2.localScale = Vector3.one;
            tmpRect2.anchoredPosition = Vector2.zero;

            Text text = tmptxt.AddComponent<Text>();
            text.text = vehicles[i].name;
            text.font = font;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 25;

            Image img = tmpImg.AddComponent<Image>();
            img.sprite = sprite;
            img.type = Image.Type.Sliced;
            img.color = whiteTransparent;

            Button button = tmpImg.AddComponent<Button>();
            button.targetGraphic = tmpImg.GetComponent<Image>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.black;
            colors.highlightedColor = whiteTransparent;
            button.colors = colors;
            int index = i;
            button.onClick.AddListener(() => DisplayVehicle(index));
            buttons.Add(tmpImg);
        }
    }

    void DisplayVehicle(int _index)
    {
        bm.InstantiateVehicle(vehicles[_index]);
        ok.interactable = false;
    }

    void OnEndLoading()
    {
        loadingMode.SetActive(false);
        editionMode.SetActive(true);
        GetComponent<UIBuilder>().enabled = true;
        enabled = false;
    }

    void EndLoading()
    {
        ok.interactable = true;
    }

    private void OnDisable()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            Destroy(buttons[i]);
        }
        buttons.Clear();
    }

    private void OnDestroy()
    {
        bm.OnEndLoading -= OnEndLoading;
    }
}
