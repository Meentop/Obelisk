using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDescriptionButton : MonoBehaviour, IPointerClickHandler
{
    public EnemyType type;

    UI ui;

    
    private void Start()
    {
        ui = UI.Instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ui.EnableEnemyInfoPanel(type);
    }
}
