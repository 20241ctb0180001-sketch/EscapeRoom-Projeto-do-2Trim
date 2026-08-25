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
    public bool isPaused { get; private set; }

    // Referência direta para a keypad da sua amiga para validar estado do puzzle
    [SerializeField] private CdgSenha keypadScript;

    private void Awake()
    {
        instance = this;
        Inventory = InputSystem.actions.FindAction("Inventario");
        Pause = InputSystem.actions.FindAction("Pause");
        
        //if (BoxInteract != null) BoxInteract.SetActive(false);
        if (PauseMenu != null) PauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Pause != null && Pause.WasPressedThisFrame())
        {
            // Impede de abrir o menu de pausa se estiver dentro de um puzzle ou keypad
            if (!IsPuzzleOuKeypadAtivo())
            {
                TogglePause();
            }
        }

        if (isPaused)
        {
            return;
        }

        if (Inventory != null && Inventory.WasPressedThisFrame())
        {
            if (InventoryIMG != null)
                InventoryIMG.SetActive(!InventoryIMG.activeInHierarchy);
        }
    }

    private bool IsPuzzleOuKeypadAtivo()
    {
        bool puzzleAtivo = painelManager.instance != null && painelManager.instance.puzzleAtivo;
        bool keypadAtiva = keypadScript != null && keypadScript.painelCdg != null && keypadScript.painelCdg.activeInHierarchy;
        
        return puzzleAtivo || keypadAtiva;
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
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (PauseMenu != null)
            PauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // Se estiver fechando a pausa, verifica se a keypad/puzzle não está ativa antes de travar o cursor
        if (!IsPuzzleOuKeypadAtivo())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (PauseMenu != null)
            PauseMenu.SetActive(false);
    }

    public void SetActivePauseMenu(bool state)
    {
        if (PauseMenu != null)
            PauseMenu.SetActive(state);
    }

    public void SetPawCursor(bool state)
    {
        if (CursorPata != null) CursorPata.SetActive(state);
    }

    public void SetbackImg(bool state)
    {
        if (saiinteract != null) saiinteract.SetActive(state);
        if (!state && interactIMG != null)
        {
            interactIMG.enabled = false;
        }
    }

    public void SetIntIMG(Sprite img)
    {
        if (interactIMG != null)
        {
            interactIMG.sprite = img;
            interactIMG.enabled = true;
        }
    }

    /*public void SetBoxInteract(bool state)
    {
        if (BoxInteract != null) BoxInteract.SetActive(state);
    }*/

    public void setItens(Item item, int index)
    {
        if (InventoryItens != null && index < InventoryItens.Length)
        {
            InventoryItens[index].text = item.InvetoryTxt;
            ShowMessage(item.CollectMsg);
        }
    }

    public void ShowMessage(string msg)
    {
        if (InfoTxt != null)
        {
            InfoTxt.text = msg;
            StartCoroutine(FadingText());
        }
    }

    IEnumerator FadingText()
    {
        Color newColor = InfoTxt.color;
        while (newColor.a < 1)
        {
            newColor.a += Time.unscaledDeltaTime;
            InfoTxt.color = newColor;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(2f);
        while (newColor.a > 0)
        {
            newColor.a -= Time.unscaledDeltaTime;
            InfoTxt.color = newColor;
            yield return null;
        }
    }
}
