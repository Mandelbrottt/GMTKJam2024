using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TabsManager : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtons;
    public Sprite Tab_Default, Tab_OnClick;
    public Vector2 InactiveTabButtonSize, ActiveTabButtonSize;


    public void SwitchTab(int TabID)
    {
        foreach(GameObject go in Tabs)
        {
            go.SetActive(false);
        }
        Tabs[TabID].SetActive(true);

        foreach (Image im in TabButtons)
        {
            im.sprite = Tab_Default;
            im.rectTransform.sizeDelta = InactiveTabButtonSize;
        }
        TabButtons[TabID].sprite = Tab_Default;
        TabButtons[TabID].rectTransform.sizeDelta = ActiveTabButtonSize;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
