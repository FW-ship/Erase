using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DefaultExecutionOrder(1)]//dragはManagerに依存するのでstartを後処理させる。
public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    const int DECKCARD_NUM = 20;
    private Vector3 startR;
    private RectTransform r;
    public static int dragNum=0;
    public GameObject refObj;
    public GameObject objSelectCardExplain;
    public GameObject objBackImage;
    public GameObject[] objDeckCard=new GameObject[DECKCARD_NUM];
    private bool nothave = false;
    MakeBookSceneManager m1;
    CardData c1;

    void Start()
    {
        int i;
        refObj = GameObject.Find("MakeBookSceneManager");
        objSelectCardExplain= refObj.GetComponent<MakeBookSceneManager>().objSelectCardExplain;
        objBackImage= refObj.GetComponent<MakeBookSceneManager>().objBackImage;
        r = GetComponent<RectTransform>();
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            objDeckCard[i] = GameObject.Find("deckcard" + i.ToString()).gameObject as GameObject;
        }
        startR = r.localPosition;
        m1 = refObj.GetComponent<MakeBookSceneManager>();
        c1 = refObj.GetComponent<CardData>();
        objSelectCardExplain.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData e)
    {
        int i;
        
        i = int.Parse(name.Substring(4))+m1.cardPage;//card~~のオブジェクト名から先頭４文字（card)を抜いて数値に型変更。それにcardPageを足す。
        dragNum = i;
        if (c1.card[dragNum].cardRest >0)
        {
            nothave = false;
            objSelectCardExplain.SetActive(true);
            objSelectCardExplain.GetComponentInChildren<Text>().text = c1.card[dragNum].cardExplain;
        }
        else
        {
            nothave = true;
            return;
        }
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            objDeckCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        }
        transform.SetParent(objSelectCardExplain.transform);
    }

    public void OnDrag(PointerEventData e)
    {
        if (nothave) { return; }
        if (r.position.y >= 430) { transform.SetParent(objBackImage.transform); } else { transform.SetParent(objSelectCardExplain.transform); }
        //ドラッグしてるモノとかぶったカードはα値０。
        for (int i = 0; i < DECKCARD_NUM; i++)
        {
            if (e.position.y > objDeckCard[i].GetComponent<RectTransform>().localPosition.y + 390 - 60 &&
                e.position.y < objDeckCard[i].GetComponent<RectTransform>().localPosition.y +390 + 60 &&
                e.position.x > objDeckCard[i].GetComponent<RectTransform>().localPosition.x + 640 - 45 &&
                e.position.x < objDeckCard[i].GetComponent<RectTransform>().localPosition.x +640 + 45 &&
                r.position.y >= 430)
            { objDeckCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0); } else { objDeckCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f); }
        }


        r.position = e.position;
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (nothave) { return; }
        int i;
        transform.SetParent(m1.objCards.transform);
        r.localPosition = startR;
        dragNum = 0;
        objSelectCardExplain.SetActive(false);
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            objDeckCard[i].GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f,1.0f);
        }
    }
}