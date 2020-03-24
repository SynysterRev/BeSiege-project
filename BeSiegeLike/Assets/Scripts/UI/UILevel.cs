using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UILevel : MonoBehaviour
{
    [SerializeField] RectTransform startPos = null;
    [SerializeField] Font font = null;
    [SerializeField] Vector2 sizeImage = Vector2.zero;
    [SerializeField] Sprite sprite = null;

    [SerializeField] Color whiteTransparent = Color.white;

    [SerializeField] List<string> listScenes = null;

    List<string> nameList;

    AssetsManager am = null;
    // Start is called before the first frame update
    void Start()
    {
        am = AssetsManager.Instance;
        am.LoadAssetBundle("levelbundle");
        foreach (string scene in am.LoadingAllScene("levelbundle"))
        {
            listScenes.Add(scene);
        }
        GetSceneName();
        CreateInterface();
    }

    void GetSceneName()
    {
        nameList = new List<string>();
        foreach (string scene in listScenes)
        {
            string tmpString = "";
            foreach (char s in scene)
            {
                if (s == '.')
                    break;
                if (s == '/' || s == '\\')
                    tmpString = "";
                else
                    tmpString += s;
            }
            nameList.Add(tmpString);
        }
    }

    void CreateInterface()
    {
        int cpt = 0;
        int cptX = 0;
        for (int i = 0; i < listScenes.Count; i++)
        {
            if (i != 0 && i % 16 == 0)
            {
                cpt++;
                cptX = 0;
            }
            GameObject tmpImg = new GameObject();
            tmpImg.transform.SetParent(transform);
            tmpImg.name = nameList[i];
            Vector2 tmpPos = Vector3.zero;
            RectTransform tmpRect = tmpImg.AddComponent<RectTransform>();
            tmpPos.x = startPos.anchoredPosition.x + cptX * (sizeImage.x + 30.0f);
            cptX++;
            tmpPos.y = startPos.anchoredPosition.y - cpt * (sizeImage.y + 30.0f);
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
            text.text = nameList[i];
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
            button.colors = colors;
            int index = i;
            button.onClick.AddListener(() => SelectScene(listScenes[index]));
        }
    }

    public void SelectScene(string _name)
    {
        GameManager.Instance.SelectLevel(_name);
        SceneManager.LoadScene(_name);
    }
}
