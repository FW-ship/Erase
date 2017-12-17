﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MakeBookSceneManager : MonoBehaviour {
    const int CARD_ALL = 77;                     //カードの全種類数。
    const int BLOCKTYPE_NUM = 5;                 //ブロックの色の種類数
    const int SKILL_TYPE = 4;                    //カードのスキルタイプの数
    const int DECKCARD_NUM = 20;                //デッキのカード枚数
    const int CARD_NUM = 20;                    //入れ替え画面に表示されるカードの数

    private GameObject objSelectCardText;                                                 //objSelectCardTextはカード説明表示のゲームオブジェクトを代入する配列。
    private GameObject objSelectCardImage;                                                //同上（Image）
    private GameObject[] objDeckCard = new GameObject[DECKCARD_NUM];                      //objDeckCardはデッキのカードのゲームオブジェクトを代入する配列。
    private GameObject[] objSelectCard = new GameObject[CARD_NUM + 1];                    //objCardは手持ちカードのゲームオブジェクトを代入する配列。
    private GameObject[] objCardRestText = new GameObject[CARD_NUM + 1];                  //カードの残り枚数表示テキストのオブジェクトを代入する配列。
    private GameObject[] objNum = new GameObject[2];                                      //ページ番号を表示するオブジェクト
    private List<Sprite> cardImage = new List<Sprite>();                                  //カードの画像（配列は全種類分だが、実際にロードするのは使用する分のみ）

    public int cardPage;                                                                  //カード一覧の現在のページ数
    public List<int> cardRest = new List<int>();                                          //カードの残り枚数

    // Use this for initialization
    void Start () {
        int i;

        //BGM読み込みと再生
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("ソウルトゥーレディオ"));

        //所持カードとデッキのロード
        CardData　c1 = GetComponent<CardData>();
        c1.LoadHaveCard(1);
        c1.LoadDeckList(1);

        for (i = 0; i < CARD_ALL+1; i++)//カード画像全種読み込み
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
        for (i = 0; i < 2; i++)
        {
            objNum[i]= GameObject.Find("Num" + i.ToString()).gameObject as GameObject;//ページ表示のオブジェクト読み込み
        }
        objSelectCardText = GameObject.Find("explaintext").gameObject as GameObject;//説明テキストのオブジェクト読み込み
        objSelectCardImage = GameObject.Find("explainimage").gameObject as GameObject;//説明テキストのオブジェクト読み込み
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
            objNum[0].GetComponent<Text>().text = cardPage.ToString();
            objNum[1].GetComponent<Text>().text = (cardPage+20).ToString();
            DrawPage();
        }

    }

    public void PushDownButton()
    {
        if (cardPage < CARD_ALL - 20)//ここはCARD_ALLに+1がいらない。CARD_ALL=20のときを考えると分かりやすい。+1があるとcardPage==0で条件を満たしてcardPage=20となり、21枚目からという存在しないカードのページを見ることになる。
        {
            cardPage += 20;
            objNum[0].GetComponent<Text>().text = cardPage.ToString();
            objNum[1].GetComponent<Text>().text = (cardPage + 20).ToString();
            DrawPage();
        }
    }




    public void PushSelectSceneButton()
    {
        CardData c1 = GetComponent<CardData>();
        c1.SaveDeckList();
        objSelectCardText.GetComponent<Text>().enabled = true;
        objSelectCardImage.GetComponent<Image>().enabled = true;
        objSelectCardText.GetComponent<Text>().text = "デッキを保存しました。";//説明テキストで表示
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
    }

    //手持ちカードのページ表示を更新する
    private void DrawPage()
    {
        int i;
        for (i = cardPage+1; i < CARD_NUM+cardPage+1; i++)//手持ちカードオブジェクトの配列は１番から
        {
            if (i - cardPage <= CARD_NUM)//オブジェクトの数はCARD_NUM個
            {
                if (i < CARD_ALL+1)
                {
                    objSelectCard[i - cardPage].gameObject.SetActive(true);//欠番でないオブジェクトはアクティブに
                }
                else
                {
                    objSelectCard[i - cardPage].gameObject.SetActive(false);//欠番のオブジェクトは非アクティブに
                }
            }
        }
        //手持ちカード、デッキのカードについて描画。
        ScreenChange();
    }

    //オブジェクトの描画内容等を更新（デッキのカード変更、手持ちカードのページ送り等の際に呼び出す）
    public void ScreenChange()
    {
        int i;
        CardData c1 = GetComponent<CardData>();
        //デッキの画像
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            objDeckCard[i].GetComponent<Image>().sprite =cardImage[c1.deckCard[0,i]];
        }
        //手持ちカードの画像
        for (i = 1; i < CARD_NUM + 1; i++)
        {
            if (i + cardPage < CARD_ALL+1)//オブジェクトの数はCARD_NUM個
            {
                objSelectCard[i].GetComponent<Image>().sprite = cardImage[cardPage + i];
                objCardRestText[i].GetComponent<Text>().text=cardRest[i + cardPage].ToString() + "/" + c1.haveCard[i+cardPage].ToString();
                if (cardRest[i + cardPage] > 0)
                {
                    objSelectCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                } else {
                    objSelectCard[i].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
                }
            }
        }
    }
}