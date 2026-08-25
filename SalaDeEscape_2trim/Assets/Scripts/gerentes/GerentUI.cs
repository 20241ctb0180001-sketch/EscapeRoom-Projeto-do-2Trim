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
    public GameObject BoxInteract;
    public GameObject PauseMenu;

    public InputActionAsset inputAction;
    private InputAction Inventory;
    private InputAction Pause;
    private bool isPaused;

    private void Awake()
    {
        instance = this;
        Inventory = InputSystem.actions.FindAction("Inventario");
        Pause = InputSystem.actions.FindAction("Pause");
        BoxInteract.SetActive(false);
        ResumeGame();
    }

    void Update()
    {
        if (Pause.WasPressedThisFrame())
        {
            TogglePause();
        }

        if (isPaused)
        {
            return;
        }

        if (Inventory.WasPressedThisFrame())
        {
            InventoryIMG.SetActive(!InventoryIMG.activeInHierarchy);
        }
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        //ime.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PauseMenu != null)
            PauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        //Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (PauseMenu != null)
            PauseMenu.SetActive(false);
    }
    public void SetActivePauseMenu(bool state)
    {
        if (PauseMenu != null)
            PauseMenu.SetActive(state);
    }

    /*private void OnDestroy()
    {
        Time.timeScale = 1f;
    }*/

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

    public void SetBoxInteract(bool state)
    {
        BoxInteract.SetActive(state);
    }

    public void setItens(Item item, int index)
    {
        InventoryItens[index].text = item.InvetoryTxt;
        ShowMessage(item.CollectMsg);
    }

    public void ShowMessage(string msg)
    {
        InfoTxt.text = msg;
        StartCoroutine(FadingText());
    }

    IEnumerator FadingText()
    {
        Color newColor = InfoTxt.color;
        while (newColor.a < 1)
        {
            newColor.a += Time.deltaTime;
            InfoTxt.color = newColor;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        while (newColor.a > 0)
        {
            newColor.a -= Time.deltaTime;
            InfoTxt.color = newColor;
            yield return null;
        }
    }
}
