using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalManager : MonoBehaviour
{
    public static PortalManager Instance;
    OutlineManager outlineManager;
    UI ui;

    [SerializeField] GameObject portalsSettingPanel;
    [SerializeField] GameObject prefabPoint, prefabArrow;
    [SerializeField] Color incoming, outgoing;

    List<Building> allPortals = new List<Building>();
    List<RectTransform> points = new List<RectTransform>();
    List<Arrow> arrows = new List<Arrow>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        outlineManager = OutlineManager.Instance;
        ui = UI.Instance;
    }

    RectTransform arrow;
    IncomingPortal curIcoming;

    private void Update()
    {
        if (portalsSettingPanel.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ui.EnableMenuButtons();
                portalsSettingPanel.SetActive(false);
            }

            if (allPortals.Count > 0)
            {
                for (int i = 0; i < allPortals.Count; i++)
                {
                    Vector3 pos = Camera.main.WorldToScreenPoint(allPortals[i].center.position);
                    points[i].anchoredPosition = new Vector3(pos.x, pos.y, 0);
                }
                foreach (Arrow arrow in arrows)
                {
                    arrow.SetPosition();
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")))
                {
                    if(hit.collider.GetComponent<IncomingPortal>())
                    {
                        IncomingPortal incoming = hit.collider.GetComponent<IncomingPortal>();
                        if (!PortalDontHaveArrow(incoming))
                        {
                            Destroy(GetArrow(incoming).arrow.gameObject);
                            arrows.Remove(GetArrow(incoming));
                        }

                        arrow = Instantiate(prefabArrow, portalsSettingPanel.transform).GetComponent<RectTransform>();
                        curIcoming = hit.collider.GetComponent<IncomingPortal>();
                        arrow.anchoredPosition = Camera.main.WorldToScreenPoint(curIcoming.center.position);
                    }
                }
            }
            if(Input.GetMouseButton(0))
            {
                if (arrow != null)
                {
                    Vector2 incomingPos = Camera.main.WorldToScreenPoint(curIcoming.center.position);
                    arrow.sizeDelta = new Vector2(arrow.sizeDelta.x, Vector2.Distance(Input.mousePosition, incomingPos));
                    Vector2 pos = new Vector2(Input.mousePosition.x - incomingPos.x, Input.mousePosition.y - incomingPos.y).normalized;
                    arrow.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y, pos.x) * (180 / Mathf.PI) - 90);
                }
            }
            if(Input.GetMouseButtonUp(0))
            {
                if (arrow != null)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Building")))
                    {
                        if (hit.collider.GetComponent<OutgoingPortal>())
                        {
                            arrows.Add(new Arrow(curIcoming, hit.collider.GetComponent<OutgoingPortal>(), arrow));
                            curIcoming.SetRelation(hit.collider.GetComponent<OutgoingPortal>());
                        }
                        else
                        {
                            Destroy(arrow.gameObject);
                            curIcoming.BreakRelation();
                        }
                    }
                    else
                    {
                        Destroy(arrow.gameObject);
                        curIcoming.BreakRelation();
                    }
                    arrow = null;
                    curIcoming = null;
                }
            }
        }
    }

    public void AddPortal(Building building)
    {
        allPortals.Add(building);
        points.Add(Instantiate(prefabPoint, portalsSettingPanel.transform).GetComponent<RectTransform>());
        if (building is IncomingPortal)
            points[points.Count - 1].GetComponent<Image>().color = incoming;
        else
            points[points.Count - 1].GetComponent<Image>().color = outgoing;
    }

    public void RemovePortal(Building building)
    {
        for (int i = 0; i < allPortals.Count; i++)
        {
            if (allPortals[i] == building)
            {
                allPortals.Remove(building);
                Destroy(points[i].gameObject);
                points.RemoveAt(i);
                if (building is IncomingPortal)
                {
                    IncomingPortal incoming = building.GetComponent<IncomingPortal>();
                    if (incoming.HasRelation())
                    {
                        Destroy(GetArrow(incoming).arrow.gameObject);
                        arrows.Remove(GetArrow(incoming));
                    }
                }
                else
                {
                    List<Arrow> arrowsForRemove = new List<Arrow>();
                    foreach (Arrow arrow in arrows)
                    {
                        if (arrow.outgoing == building.GetComponent<OutgoingPortal>())
                        {
                            Destroy(arrow.arrow.gameObject);
                            arrow.incoming.BreakRelation();
                            arrowsForRemove.Add(arrow);
                        }
                    }
                    foreach (Arrow arrow1 in arrowsForRemove)
                    {
                        arrows.Remove(arrow1);
                    }
                }
            }
        }
    }

    public bool PortalDontHaveArrow(IncomingPortal portal)
    {
        foreach (Arrow arrow in arrows)
        {
            if (arrow.incoming == portal)
                return false;
        }
        return true;
    }

    public Arrow GetArrow(IncomingPortal portal)
    {
        foreach (Arrow arrow in arrows)
        {
            if (arrow.incoming == portal)
                return arrow;
        }
        return null;
    }

    public void StartSetting()
    {
        ui.DisableAllPanelsStrong();
        ui.DisableMenuButtons();
        portalsSettingPanel.SetActive(true);
        outlineManager.DisableOutlineStrong();
    }
}

public class Arrow 
{
    public IncomingPortal incoming { get; private set; }
    public OutgoingPortal outgoing;
    public RectTransform arrow { get; private set; }

    public Arrow(IncomingPortal incoming, OutgoingPortal outgoing, RectTransform arrow)
    {
        this.incoming = incoming;
        this.outgoing = outgoing;
        this.arrow = arrow;
    }

    public void SetPosition()
    {
        Vector3 incomingPos = Camera.main.WorldToScreenPoint(incoming.center.position);
        Vector3 outgoingPos = Camera.main.WorldToScreenPoint(outgoing.center.position);
        arrow.anchoredPosition = incomingPos;
        arrow.sizeDelta = new Vector2(arrow.sizeDelta.x, Vector2.Distance(incomingPos, outgoingPos));
        Vector2 pos = new Vector2(outgoingPos.x - incomingPos.x, outgoingPos.y - incomingPos.y).normalized;
        arrow.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(pos.y, pos.x) * (180 / Mathf.PI) - 90);
    }
}
