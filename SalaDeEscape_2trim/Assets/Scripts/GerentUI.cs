using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
public class GerentUI : MonoBehaviour
{
    public static GerentUI instance;
    public GameObject CursorPata;

    private void Awake()
    {
        instance = this;
    }

    public void SetPawCursor(bool state)
    {
        CursorPata.SetActive(state);
    }
}
