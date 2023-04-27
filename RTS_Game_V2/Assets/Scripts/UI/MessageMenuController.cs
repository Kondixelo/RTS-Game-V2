using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
[RequireComponent(typeof(Canvas))]
public class MessageMenuController : MonoBehaviour
{
    private Camera mainCamera;

    private GameObject objectReq;
    private Vector3 objectReqPosition;

    [Header("Main Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform panelRect;
    private Vector2 panelSize;

    [Header("Button Panel")]
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private RectTransform buttonPanelRect;
    private Vector2 buttonPanelSize;
    [SerializeField] private GameObject buttonYES;
    [SerializeField] private GameObject buttonNO;
    [SerializeField] private GameObject buttonOK;
    private Button btYes;

    [Header("Text Holders")]
    [SerializeField] private TMP_Text nameHolder;
    [SerializeField] private TMP_Text descripotionHolder;
    [SerializeField] private TMP_Text messageHolder;
    
    [SerializeField] private RectTransform nameHolderRect;
    [SerializeField] private RectTransform descripotionHoldeRrect;
    [SerializeField] private RectTransform messageHolderRect;

    private Vector2 nameHolderSize;
    private Vector2 descrHolderSize;
    private Vector2 messHolderSize;

    [Header("Others")]
    [Tooltip("Screen MessageMenu position when is static and do not follow object. By default - (0,0,0) - middle of screen")]
    [SerializeField] private Vector3 staticMenuPosition;
    private Vector3 relativeOffset;
    private bool relativePosition;
    [SerializeField] private bool followMouse;

    private MessagePanelBehaviour messPanelBeh;
    private bool doFadeOutAndUnactive;
    private bool firstRun = true;

    private Vector3 lastMousePositionOnObject;

    private void Start()
    {
        SetAllElements();
    }

    void Update()
    {
        if (relativePosition)
        {
            CheckPointing();
        }
        else
        {
            Positioning();
        }
    }

    public void PrepareMessageMenu(IInteractionObjects intObject, MessageType messageType)
    {
        if (firstRun)
        {
            SetAllElements();
            ResetAllElements();
            firstRun = false;
        }
        switch (messageType)
        {
            case MessageType.POPUP:
                relativePosition = true;
                break;
            case MessageType.OPEN:
                relativePosition = false;
                btYes.onClick.AddListener(intObject.DoInteraction);
                buttonYES.SetActive(true);
                buttonNO.SetActive(true);
                messageHolder.text += "Open?";
                break;
            case MessageType.TAKE:
                relativePosition = false;
                btYes.onClick.AddListener(intObject.DoInteraction);
                buttonYES.SetActive(true);
                buttonNO.SetActive(true);
                messageHolder.text += "Take it?";
                break;
            case MessageType.INFORMATION:
                relativePosition = false;
                buttonOK.SetActive(true);
                break;
            default:
                relativePosition = false;
                buttonOK.SetActive(true);
                break;
        }

        objectReq = intObject.GetGameObject();
        objectReqPosition = objectReq.transform.position;
        SetTextHolders(intObject);
        CheckTextHolders();
        CheckButtons();
    }

    private void SetAllElements()
    {
        btYes = buttonYES.GetComponent<Button>();
        buttonPanelSize = buttonPanelRect.sizeDelta;
        nameHolderSize = nameHolderRect.sizeDelta;
        descrHolderSize = descripotionHoldeRrect.sizeDelta;
        messHolderSize = messageHolderRect.sizeDelta;

        doFadeOutAndUnactive = true;
        mainCamera = Camera.main;
        messPanelBeh = panel.GetComponent<MessagePanelBehaviour>();
        panelRect = panel.GetComponent<RectTransform>();
        panelSize = panelRect.sizeDelta;
    }

    private void ResetAllElements()
    {
        nameHolder.text = "";
        descripotionHolder.text = "";
        messageHolder.text = "";

        nameHolder.gameObject.SetActive(false);
        descripotionHolder.gameObject.SetActive(false);
        messageHolder.gameObject.SetActive(false);

        buttonYES.SetActive(false);
        buttonNO.SetActive(false);
        buttonOK.SetActive(false);

        panelRect.sizeDelta = new Vector2(panelSize.x, 0);
        nameHolderRect.localPosition = Vector3.zero;
        descripotionHoldeRrect.localPosition = Vector3.zero;
        messageHolderRect.localPosition = Vector3.zero;
        buttonPanelRect.localPosition = Vector3.zero;

        relativeOffset = Vector3.zero;
        btYes.onClick.RemoveAllListeners();
    }

    private void SetTextHolders(IInteractionObjects intObject)
    {
        Dictionary<string, string> lista = intObject.GetContentToDisplay();
        if (lista.Count > 0)
        {
            foreach (KeyValuePair<string, string> li in lista)
            {
                switch (li.Key)
                {
                    case "Name":
                        nameHolder.text += li.Value;
                        break;
                    case "Description":
                        descripotionHolder.text += li.Value;
                        break;
                    case "Message":
                        messageHolder.text += li.Value;
                        break;
                    default:
                        messageHolder.text = "Error";
                        break;
                }
            }
        }
    }

    private void CheckTextHolders()
    {
        if(nameHolder.text != "")
        {
            nameHolder.gameObject.SetActive(true);
            panelRect.sizeDelta += new Vector2(0f, nameHolderSize.y);

            nameHolderRect.localPosition -= new Vector3(0f, nameHolderSize.y, 0f);
            descripotionHoldeRrect.localPosition -= new Vector3(0f, nameHolderSize.y, 0f);
            messageHolderRect.localPosition -= new Vector3(0f, nameHolderSize.y, 0f);
            buttonPanelRect.localPosition -= new Vector3(0f, nameHolderSize.y, 0f);

            relativeOffset += new Vector3(0f, nameHolderSize.y, 0f);
        }
        if(descripotionHolder.text != "")
        {
            descripotionHolder.gameObject.SetActive(true);
            panelRect.sizeDelta += new Vector2(0f, descrHolderSize.y);

            descripotionHoldeRrect.localPosition -= new Vector3(0f, descrHolderSize.y, 0f);
            messageHolderRect.localPosition -= new Vector3(0f, descrHolderSize.y, 0f);
            buttonPanelRect.localPosition -= new Vector3(0f, descrHolderSize.y, 0f);

            relativeOffset += new Vector3(0f, descrHolderSize.y, 0f);
        }
        if(messageHolder.text != "")
        {
            messageHolder.gameObject.SetActive(true);
            panelRect.sizeDelta += new Vector2(0f, messHolderSize.y);

            messageHolderRect.localPosition -= new Vector3(0f, messHolderSize.y, 0f);
            buttonPanelRect.localPosition -= new Vector3(0f, messHolderSize.y, 0f);

            relativeOffset += new Vector3(0f, messHolderSize.y, 0f);
        }
    }

    private void CheckButtons()
    {
        if(buttonYES.activeSelf || buttonNO.activeSelf || buttonOK.activeSelf)
        {
            panelRect.sizeDelta += new Vector2(0f, buttonPanelSize.y);
            buttonPanelRect.localPosition -= new Vector3(0f, buttonPanelSize.y, 0f);

            relativeOffset += new Vector3(0f, buttonPanelSize.y, 0f);
        }
    }

    private void CheckPointing()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Positioning();
            if ((hit.transform.gameObject == objectReq) && (objectReq != null))
            {
                doFadeOutAndUnactive = true;
                lastMousePositionOnObject = Input.mousePosition;
            }
            else
            {
                if (doFadeOutAndUnactive)
                {
                    StartCoroutine(MenuDeactivate());
                    doFadeOutAndUnactive = false;
                }
            }
        }
    }

    private void Positioning()
    {
        if (relativePosition) //menu ma sledzic obiekt
        {
            if (followMouse) //...oraz myszke na obiektcie
            {
                Vector3 pos = lastMousePositionOnObject + relativeOffset;
                if (panel.transform.position != pos)
                {
                    panel.transform.position = pos;
                }
            }
            else
            {
                Vector3 pos = (mainCamera.WorldToScreenPoint(objectReqPosition) + relativeOffset);
                if (panel.transform.position != pos)
                {
                    panel.transform.position = pos;
                }
            }
        }
        else
        {
            panel.transform.localPosition = staticMenuPosition;
        }
    }

    private IEnumerator MenuDeactivate()
    {
        if (messPanelBeh != null)
        {
            yield return StartCoroutine(messPanelBeh.Deactivate());
        }
        gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        ResetAllElements();
    }
}