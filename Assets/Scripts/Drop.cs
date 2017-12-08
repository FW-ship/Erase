using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    GameObject refObj;
    // Use this for initialization
    void Start()
    {
        refObj = GameObject.Find("MakeBookSceneManager");
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnDrop(PointerEventData e)//ドロップ受ける側が上のレイヤーにいないと受け取れないので注意。
    {
        CardData c1 = refObj.GetComponent<CardData>();
        MakeBookSceneManager m1 = refObj.GetComponent<MakeBookSceneManager>();
        if (m1.cardRest[Drag.dragNum] > 0)//入れるカードの残り枚数が０でないならデッキに入れてよい。
        {
        m1.cardRest[c1.deckCard[0,int.Parse(name.Substring(8))]]++;//デッキから外れるカードの残り枚数を１増やす。
        c1.deckCard[0,int.Parse(name.Substring(8))] = Drag.dragNum;  //先頭から8文字（deckcard)を抜いて数値に型変換したものがデッキのカード番号。そこに新たにドラッグしてきたカードの種類を入れる。
        m1.cardRest[c1.deckCard[0,int.Parse(name.Substring(8))]]--;//デッキに入れたカードの残り枚数を１減らす。
        }
        m1.ScreenChange();
    }
}
