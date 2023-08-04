using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Tooltip : MonoBehaviour
{
    //public Item item;

    //GUI, 에디터에서 사용
    [SerializeField]
    public Image tooltipBackground;
    [SerializeField]
    public Text tooltipNameText;
    [SerializeField]
    public Text tooltipDescText;

    //Tooltip Settings
    [SerializeField]
    public bool showTooltip;
    [SerializeField]
    private bool tooltipCreated;
    [SerializeField]
    private int tooltipWidth;
    [SerializeField]
    public int tooltipHeight;

    //icon settengs
    [SerializeField]
    private bool showTooltipIcon;
    [SerializeField]
    private int tooltipIconPosX;
    [SerializeField]
    private int tooltipIconPosY;
    [SerializeField]
    private int tooltipIconSize;

    //Name Settings
    [SerializeField]
    private bool showTooltipName;
    [SerializeField]
    private int tooltipNamePosX;
    [SerializeField]
    private int tooltipNamePosY;

    //desc Settings
    [SerializeField]
    private bool showTooltipDesc;
    [SerializeField]
    private int tooltipDescPosX;
    [SerializeField]
    private int tooltipDescPosY;
    [SerializeField]
    private int tooltipDescSizeX;
    [SerializeField]
    private int tooltipDescSizeY;

    //Tooltip Objects
    [SerializeField]
    private GameObject tooltip;
    [SerializeField]
    private RectTransform tooltipRectTransform;
    [SerializeField]
    private GameObject tooltipTextName;
    [SerializeField]
    private GameObject tooltipTextDesc;
    [SerializeField]
    private GameObject tooltipImageIcon;

    private Image tooltipImageIconImage;

#if UNITY_EDITOR //메뉴 에디터
    [MenuItem("Master System/Create/Tooltip")]        //creating the menu item
    public static void menuItemCreateInventory()       //create the inventory at start
    {
        if (GameObject.FindGameObjectWithTag("Tooltip") == null) //툴팁 없으면 생성
        {
            GameObject toolTip = (GameObject)Instantiate(Resources.Load("Prefabs/Tooltip - Inventory") as GameObject); //프리팸 인스턴스화

            toolTip.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0); //최초 설정
            toolTip.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform); //캔버스를 부모로
            toolTip.AddComponent<Tooltip>().setImportantVariables(); //생성 초기 설정
        }
    }
#endif
    public void setImportantVariables() //툴팁 생성시 기본 설정
    {
        tooltipRectTransform = this.GetComponent<RectTransform>();

        tooltipImageIcon = this.transform.GetChild(1).gameObject;
        tooltipImageIcon.SetActive(false);
        tooltipTextName = this.transform.GetChild(2).gameObject;
        tooltipTextName.SetActive(false);
        tooltipTextDesc = this.transform.GetChild(3).gameObject;
        tooltipTextDesc.SetActive(false);

        tooltipIconSize = 50;
        tooltipWidth = 150;
        tooltipHeight = 250;
        tooltipDescSizeX = 100;
        tooltipDescSizeY = 100;
    }
    public void setVariables() //에디터에서 사용, 컴포넌트 레퍼런스 저장
    {
        tooltipBackground = transform.GetChild(0).GetComponent<Image>();
        tooltipNameText = transform.GetChild(2).GetComponent<Text>();
        tooltipDescText = transform.GetChild(3).GetComponent<Text>();
    }
    public void updateTooltip() //에디터에서 사용, 에디터 수정치를 실제로 다른 컴포넌트에 적용
    {
        if (!Application.isPlaying)
        {
            tooltipRectTransform.sizeDelta = new Vector2(tooltipWidth, tooltipHeight);

            //이름
            if (showTooltipName)
            {
                tooltipTextName.gameObject.SetActive(true);
                this.transform.GetChild(2).GetComponent<RectTransform>().localPosition = new Vector3(tooltipNamePosX, tooltipNamePosY, 0);
            }
            else
            {
                this.transform.GetChild(2).gameObject.SetActive(false);
            }

            //아이콘
            if (showTooltipIcon)
            {
                this.transform.GetChild(1).gameObject.SetActive(true);
                this.transform.GetChild(1).GetComponent<RectTransform>().localPosition = new Vector3(tooltipIconPosX, tooltipIconPosY, 0);
                this.transform.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipIconSize, tooltipIconSize);
            }
            else
            {
                this.transform.GetChild(1).gameObject.SetActive(false);
            }

            //설명
            if (showTooltipDesc)
            {
                this.transform.GetChild(3).gameObject.SetActive(true);
                this.transform.GetChild(3).GetComponent<RectTransform>().localPosition = new Vector3(tooltipDescPosX, tooltipDescPosY, 0);
                this.transform.GetChild(3).GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipDescSizeX, tooltipDescSizeY);
            }
            else
            {
                this.transform.GetChild(3).gameObject.SetActive(false);
            }
        }
    }


    void Start()
    {
        setVariables();
        deactivateTooltip();
        tooltipImageIconImage = transform.GetChild(1).GetComponent<Image>();
    }

    //활성화, 아이템 내용(아이콘,이름,설명) 적용
    public void activateTooltip(Item _item, Vector3 _pos) //if you activate the tooltip through hovering over an item
    {
        tooltipTextName.SetActive(true);
        tooltipImageIcon.SetActive(true);
        tooltipTextDesc.SetActive(true);
        tooltipBackground.gameObject.SetActive(true);          //Tooltip getting activated

        tooltipImageIconImage.sprite = _item.itemIcon;         //and the itemIcon...
        tooltipNameText.text = _item.itemName;            //,itemName...
        tooltipDescText.text = _item.itemDesc;            //and itemDesc is getting set

        
    _pos += 
        new Vector3(GetComponent<RectTransform>().rect.width * 0.1f,
        -GetComponent<RectTransform>().rect.height * 0.1f,
        0);
        transform.position = _pos;
    }

    //비활성화
    public void deactivateTooltip()             //deactivating the tooltip after you went out of a slot
    {
        tooltipTextName.SetActive(false);
        tooltipImageIcon.SetActive(false);
        tooltipTextDesc.SetActive(false);
        tooltipBackground.gameObject.SetActive(false);
    }
}
