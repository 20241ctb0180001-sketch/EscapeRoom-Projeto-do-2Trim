using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class GerentUI : MonoBehaviour
{
    public static GerentUI instance;
    public GameObject CursorPata;
    public GameObject saiinteract;
    public Image interactIMG;

    private void Awake()
    {
        instance = this;
    }

    public void SetPawCursor(bool state)
    {
        CursorPata.SetActive(state);
    }
    public void SetbackImg(bool state)
    {
        saiinteract.SetActive(state);
        if (!state)
        {
            interactIMG.enabled = false;
        }
    }
    public void SetIntIMG(Sprite img)
    {
        interactIMG.sprite = img;
        interactIMG.enabled = true;
    }
}
