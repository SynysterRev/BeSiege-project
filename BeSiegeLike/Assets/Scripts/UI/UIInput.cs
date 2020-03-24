using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInput : MonoBehaviour
{
    [SerializeField] List<Button> buttons = null;

    List<Text> textButton;

    [SerializeField] Button buttonOk = null;

    [SerializeField] GameObject editionMode = null;

    [SerializeField] GameObject inputMode = null;

    [SerializeField] Text textInput;

    BuilderManager bm;

    bool isWaitingKey;
    InputManager im;
    int currentIndex;
    // Start is called before the first frame update
    void Start()
    {
        bm = BuilderManager.Instance;
        for (int i = 0; i < buttons.Count; ++i)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => Onclick(index));
        }
        im = InputManager.Instance;
        buttonOk.onClick.AddListener(OnQuitInput);
        textButton = new List<Text>();
        for (int i = 0; i < buttons.Count; i++)
        {
            textButton.Add(buttons[i].GetComponentInChildren<Text>());
        }
        for (int i = 0; i < im.dicKey.Count; i++)
        {
            textButton[i].text = im.dicKey[(NameInput)i].ToString();
        }
    }

    private void OnGUI()
    {
        var e = Event.current;
        if(isWaitingKey && e.isKey)
        {
            int otherIndex = im.ChangeInput(currentIndex, e.keyCode);

            textButton[currentIndex].text = im.dicKey[(NameInput)currentIndex].ToString();
            if(otherIndex != -1)
            {
                textButton[otherIndex].text = im.dicKey[(NameInput)otherIndex].ToString();
            }

            foreach (Button button in buttons)
            {
                button.interactable = true;
            }
            buttonOk.interactable = true;
            isWaitingKey = false;
            textInput.enabled = false;
        }
    }

    void Onclick(int _index)
    {
        isWaitingKey = true;
        currentIndex = _index;
        buttonOk.interactable = false;
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
        textInput.enabled = true;
    }

    void OnQuitInput()
    {
        inputMode.SetActive(false);
        editionMode.SetActive(true);
        GetComponent<UIBuilder>().enabled = true;
        enabled = false;
        bm.isLoading = false;
    }

}
