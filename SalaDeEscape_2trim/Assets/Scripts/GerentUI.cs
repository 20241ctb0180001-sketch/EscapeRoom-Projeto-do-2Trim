using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
public class GerentUI : MonoBehaviour
{
    public static GerentUI instance;
    public GameObject CursorPata;
    public GameObject saiinteract;
    public Image interactIMG;
    public GameObject InventoryIMG;
    public TextMeshProUGUI[] InventoryItens;
    public TextMeshProUGUI InfoTxt;

    public InputActionAsset inputAction;
    private InputAction Inventory;

    private void Awake()
    {
        instance = this;
        Inventory = InputSystem.actions.FindAction("Inventario");
    }

    void Update()
    {
        if (Inventory.WasPressedThisFrame())
        {
            InventoryIMG.SetActive(!InventoryIMG.activeInHierarchy);
        }
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

    public void setItens(Item item, int index)
    {
        InventoryItens[index].text = item.InvetoryTxt;
        InfoTxt.text = item.CollectMsg;
        StartCoroutine(FadingText());
    }

    IEnumerator FadingText()
    {
        Color newColor = InfoTxt.color;
        while(newColor.a < 1)
        {
            newColor.a += Time.deltaTime;
            InfoTxt.color = newColor;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        while(newColor.a > 0)
        {
            newColor.a -= Time.deltaTime;
            InfoTxt.color = newColor;
            yield return null;
        }
    }
}
