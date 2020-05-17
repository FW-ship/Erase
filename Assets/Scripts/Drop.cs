using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    GameObject refObj;
    CardData c1;
    MakeBookSceneManager m1;

    // Use this for initialization
    void Start()
    {
        refObj = GameObject.Find("MakeBookSceneManager");
        c1 = refObj.GetComponent<CardData>();
        m1 = refObj.GetComponent<MakeBookSceneManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnDrop(PointerEventData e)//ドロップ受ける側が上のレイヤーにいないと受け取れないので注意。
    {
        if (c1.card[Drag.dragNum].cardRest > 0)//入れるカードの残り枚数が０でないならデッキに入れてよい。
        {
        foreach (Card c in c1.card) { if (c.cardNum == c1.deckCard[0, int.Parse(name.Substring(8))].cardNum) { c.cardRest++; } }//デッキから外れるカードの残り枚数を１増やす。
        c1.deckCard[0,int.Parse(name.Substring(8))] = c1.card[Drag.dragNum];  //先頭から8文字（deckcard)を抜いて数値に型変換したものがデッキのカード番号。そこに新たにドラッグしてきたカードの種類を入れる。
        c1.deckCard[0, int.Parse(name.Substring(8))].cardRest--;//デッキに入れたカードの残り枚数を１減らす。
        }
        m1.ScreenChange();
    }
}