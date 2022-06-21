using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    Outline outline;
    UI ui;

    public static OutlineManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ui = UI.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            DisableOutline();
    }

    public void EnableOutline(Outline outline)
    {
        if (ui.EnabledStartUI())
        {
            DisableOutline();
            this.outline = outline;
            this.outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        if(outline != null && !ui.cursorOnButton)
            outline.enabled = false;
    }
}
