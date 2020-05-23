using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class MakeBookSceneManager : MonoBehaviour {
    const int BLOCKTYPE_NUM = 4;                 //ブロックの色の種類数
    const int SKILL_TYPE = 4;                    //カードのスキルタイプの数
    const int DECKCARD_NUM = 20;                //デッキのカード枚数
    const int CARD_NUM = 20;                    //入れ替え画面に表示されるカードの数

    public Slider objManaSort;
    public GameObject objSourceBookAnime;
    public GameObject objCards;
    public GameObject objBackImage;
    public GameObject objSort;
    public GameObject objSelectCardExplain;                                                 //objSelectCardはカード説明表示のゲームオブジェクトを代入する配列。
    private GameObject[] objDeckCard = new GameObject[DECKCARD_NUM];                      //objDeckCardはデッキのカードのゲームオブジェクトを代入する配列。
    private GameObject[] objSelectCard = new GameObject[CARD_NUM + 1];                    //objCardは手持ちカードのゲームオブジェクトを代入する配列。
    private GameObject[] objCardRestText = new GameObject[CARD_NUM + 1];                  //カードの残り枚数表示テキストのオブジェクトを代入する配列。
    public GameObject[] objNum = new GameObject[2];                                      //ページ番号を表示するオブジェクト
    public GameObject[] objManaSortButton = new GameObject[BLOCKTYPE_NUM+1];
    private List<Sprite> cardImage = new List<Sprite>();                                  //カードの画像（配列は全種類分だが、実際にロードするのは使用する分のみ）

    public int cardPage;                                                                  //カード一覧の現在のページ数
    public Sprite[] bookImage = new Sprite[10];
    public bool[] manaSort = new bool[BLOCKTYPE_NUM+1];
    private bool flickon;
    CardData c1;
    Utility u1;


    // Use this for initialization
    void Start () {
        int i;
        c1 = GetComponent<CardData>();
        u1 = GetComponent<Utility>();
        //BGM読み込みと再生
        u1.BGMPlay(Resources.Load<AudioClip>("ソウルトゥーレディオ"));

        //所持カードとデッキのロード

        for (i = 0; i < c1.card.Count; i++)//カード画像全種読み込み
        {
            cardImage.Add(Resources.Load<Sprite>("card" + i.ToString()));
        }

        //オブジェクト読み込み
        //デッキのカードについてオブジェクト読み込み。
        for (i = 0; i < DECKCARD_NUM; i++)
        {
                objDeckCard[i] = GameObject.Find("deckcard" + i.ToString()).gameObject as GameObject;          
        }
        //手持ちカードについて描画用オブジェクト読み込み。
        for (i = 1; i < CARD_NUM+1; i++)//手持ちカードオブジェクトは番号が１から始まる（カード番号が１から始まるのとリンクしているため）
        {
            objSelectCard[i] = GameObject.Find("card" + i.ToString()).gameObject as GameObject;
        }
        for (i = 1; i < CARD_NUM + 1; i++)
        {
            objCardRestText[i] = GameObject.Find("cardrest" + i.ToString()).gameObject as GameObject;//カード残り枚数表示のオブジェクト読み込み
        }
        cardPage = 0;//ページ番号の初期化

        DrawPage();

    }
	
	// Update is called once per frame
	void Update () {	

	}

    public void PushUpButton()
    {
        if (cardPage >= 20)
        {
            cardPage -= 20;
            StartCoroutine(PageTurn(true));

        }

    }

    public void PushDownButton()
    {
        if (cardPage < c1.card.Count - 21)//c1.card.Countはカード０枚の時でも要素０が存在する。なので-21になる。
        {
            cardPage += 20;
            StartCoroutine(PageTurn(false));
        }
    }

    private IEnumerator PageTurn(bool left)
    {
        int j;
        for (j = 0; j <= 10; j++)
        {
            for (int i = 1; i < CARD_NUM + 1; i++)
            {
                if (i + cardPage < c1.card.Count)//オブジェクトの数はCARD_NUM個
                {
                    if (c1.card[i + cardPage].cardRest > 0)
                    {
                        objSelectCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1 - j * 0.1f);
                    }
                    else
                    {
                        objSelectCard[i].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1 - j * 0.1f);
                    }
                }
            }
            yield return null;
        }
        objSourceBookAnime.SetActive(true);
        for (int i = 0; i < 10; i++)
        {
            if (left) { j = 9 - i; } else { j = i; }
            objSourceBookAnime.GetComponent<Image>().sprite = null;//unity不具合回避
            objSourceBookAnime.GetComponent<Image>().sprite = bookImage[j];
            yield return null;
        }
        objNum[0].GetComponent<Text>().text = cardPage.ToString();
        objNum[1].GetComponent<Text>().text = (cardPage + 20).ToString();
        DrawPage();
        objNum[0].GetComponentInChildren<Image>().enabled = true;
        objNum[1].GetComponentInChildren<Image>().enabled = true;
        if (cardPage < 20) { objNum[0].GetComponentInChildren<Image>().enabled = false; }
        if (cardPage >= c1.card.Count - 21) { objNum[1].GetComponentInChildren<Image>().enabled = false; }
        objSourceBookAnime.SetActive(false);
        for (j = 0; j <= 10; j++)
        {
            for (int i = 1; i < CARD_NUM + 1; i++)
            {
                if (i + cardPage < c1.card.Count)//オブジェクトの数はCARD_NUM個
                {
                    if (c1.card[i + cardPage].cardRest > 0)
                    {
                        objSelectCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, j * 0.1f);
                    }
                    else
                    {
                        objSelectCard[i].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, j * 0.1f);
                    }
                }
            }
            yield return null;
        }
    }


    public void PushSelectSceneButton()
    {
        c1.SaveDeckList();

        objSelectCardExplain.SetActive(true);
        objSelectCardExplain.GetComponentInChildren<Text>().text = "デッキを保存しました。";//説明テキストで表示
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
    }

    //手持ちカードのページ表示を更新する
    private void DrawPage()
    {
        int i;
        for (i = 1; i < CARD_NUM+1; i++)//手持ちカードオブジェクトの配列は１番から
        {
                if (i < c1.card.Count)
                {
                    objSelectCard[i].gameObject.SetActive(true);//欠番でないオブジェクトはアクティブに
                }
                else
                {
                    objSelectCard[i].gameObject.SetActive(false);//欠番のオブジェクトは非アクティブに
                }
            
        }
        //手持ちカード、デッキのカードについて描画。
        ScreenChange();
    }

    //オブジェクトの描画内容等を更新（デッキのカード変更、手持ちカードのページ送り等の際に呼び出す）
    public void ScreenChange()
    {
        int i;
        //デッキの画像
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            objDeckCard[i].GetComponent<Image>().sprite = null;//unity不具合回避
            objDeckCard[i].GetComponent<Image>().sprite =cardImage[c1.deckCard[0,i].cardNum];
        }
        //手持ちカードの画像
        for (i = 1; i < CARD_NUM + 1; i++)
        {
            if (i + cardPage < c1.card.Count)//オブジェクトの数はCARD_NUM個
            {
                objSelectCard[i].GetComponent<Image>().sprite = null;//unity不具合回避
                objSelectCard[i].GetComponent<Image>().sprite = cardImage[c1.card[i + cardPage].cardNum];
                objCardRestText[i].GetComponent<Text>().text=c1.card[i+cardPage].cardRest.ToString() + "/" + c1.card[i+cardPage].haveCard.ToString();
                if (c1.card[i + cardPage].cardRest > 0)
                {
                    objSelectCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                } else {
                    objSelectCard[i].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                }
            }
        }
    }

    public void PushManaSort(int x)
    {
        if (manaSort[x]) {
            manaSort[x] = false;
            objManaSortButton[x].GetComponent<Image>().color = new Color(1,1,1, 1.0f);
        }
        else {
            manaSort[x] = true;
            objManaSortButton[x].GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 1.0f);
        }
    }

    public void PushSort()
    {
        objSort.SetActive(true);
    }

    public void PushSortEnd()
    {
        c1.CardList();
        for (int i = 0; i < DECKCARD_NUM; i++)
        {
            c1.card[c1.deckCard[0, i].cardNum].cardRest--;//デッキで使っている分、残りカードの枚数を減らす。
        }
        c1.card.RemoveAll(CardSortMatch);
        cardPage = 0;
        StartCoroutine(PageTurn(false));
        objSort.SetActive(false);
    }

    public bool CardSortMatch(Card c)
    {
        int value = 1;
        bool result = false;
        for (int i = 0; i < objManaSort.value; i++) { value *= 3; }
        for (int i = 0; i < BLOCKTYPE_NUM; i++) {
            if (value < c.cardCost[i]) { result = true; }
            if (c.cardCost[i] > 0 && manaSort[i]) { result = true; }
                }
        return result;
    }

    public void SourcebookPush()
    {
        if (flickon) { return; }
        flickon = true;
        StartCoroutine(SourcebookSwipe(Input.mousePosition));
    }
    private IEnumerator SourcebookSwipe(Vector3 position)
    {
        while (u1.flick==0) {
            yield return null;
        }
        if (u1.flick==-1) { PushUpButton(); }
        if (u1.flick==1) { PushDownButton(); }
        flickon=false;
    }
}