using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    const int DECKCARD_NUM = 20;
    private Vector3 startR;
    private RectTransform r;
    public static int dragNum=0;
    public GameObject refObj;
    public GameObject objSelectCardText;                             //objSelectCardTextは手持ちカードのゲームオブジェクトを代入する配列。
    public GameObject objSelectCardImage;                       //同上（Image）
    public GameObject[] objDeckCard=new GameObject[DECKCARD_NUM];
    
    void Start()
    {
        int i;
        refObj = GameObject.Find("MakeBookSceneManager");
        r = GetComponent<RectTransform>();
        objSelectCardText = GameObject.Find("explaintext").gameObject as GameObject;
        objSelectCardImage = GameObject.Find("explainimage").gameObject as GameObject;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            objDeckCard[i] = GameObject.Find("deckcard" + i.ToString()).gameObject as GameObject;
        }
        startR = r.localPosition;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        int i;
        MakeBookSceneManager m1 = refObj.GetComponent<MakeBookSceneManager>();
        i = int.Parse(name.Substring(4))+m1.cardPage;//card~~のオブジェクト名から先頭４文字（card)を抜いて数値に型変更。それにcardPageを足す。
        dragNum = i;
        objSelectCardText.GetComponent<Text>().enabled = true;
        objSelectCardImage.GetComponent<Image>().enabled = true;
        CardData c1 = refObj.GetComponent<CardData>();
        c1.CardList();
        if (c1.haveCard[dragNum] > 0)
        {
            objSelectCardText.GetComponent<Text>().text = c1.cardExplain[dragNum];
        }
        else
        {
            objSelectCardText.GetComponent<Text>().text = "<size=48>未所持のため詳細不明</size>";
        }
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            objDeckCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        }
    }

    public void OnDrag(PointerEventData e)
    {
        r.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        int i;
        r.localPosition = startR;
        dragNum = 0;
        objSelectCardText.GetComponent<Text>().enabled = false;
        objSelectCardImage.GetComponent<Image>().enabled = false;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            objDeckCard[i].GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f,1.0f);
        }
    }
}