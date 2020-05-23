using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEditor;



[DefaultExecutionOrder(-1)]//CardDataは他から引用されるのでstartを先行処理させる。
public class CardData : MonoBehaviour {
    const int BLOCKTYPE_NUM = 4;                 //ブロックの色の種類数
    const int DECKCARD_NUM = 20;                 //デッキのカード枚数
    const int HAND_NUM = 3;                      //手札の枚数
    //呪文のスキル種別に関する定数
    const int SUMMON = 1;                        //第０種、第２種呪文においてスキル種別（召喚）を表す。
    const int COUNTER = 2;                       //第２種呪文においてスキル種別（２種呪文妨害）を表す。
    const int DECK_EAT = 3;                      //第２種呪文においてスキル種別（デッキ破壊）を表す。
    const int HAND_CHANGE = 4;                   //第２種呪文においてスキル種別（手札交換）を表す。
    const int OWN = 1;                           //第１種呪文においてスキル種別（対象：自身のシュジンコウ）を表す。
    const int YOURS = 2;                         //第１種呪文においてスキル種別（対象：自身のシュジンコウ）を表す。
    const int OTHER = 10000;                     //第２種呪文においてスキル種別（その他）を表す。

    public Card[,] deckCard = new Card[2, DECKCARD_NUM];                   //デッキに入れているカードを示す配列。（中の数字がカード番号）
    public int[] enemyGetManaPace = new int[BLOCKTYPE_NUM + 1];          //敵のマナ獲得ペース

    public List<Card> card=new List<Card>();
    PuzzleSceneManager p1;

    //持っているカードをセーブデータから取り出す
    public void LoadHaveCard()
    {
        int i;

        for (i = 0; i < card.Count; i++)
        {
            card[i].haveCard = 0;
        }
        if (PlayerPrefs.GetInt("haveCard1", 0) == 0)//初期状態なら
        {
            card[1].haveCard = 3;
            card[2].haveCard = 3;
            card[3].haveCard = 3;
            card[4].haveCard = 3;
            card[5].haveCard = 3;
            card[6].haveCard = 3;
            card[7].haveCard = 3;
            for (i = 1; i < card.Count; i++)
            {
                PlayerPrefs.SetInt("haveCard" + i.ToString(), card[i].haveCard);
            }
        }
        else
        {
            for (i = 1; i < card.Count; i++)
            {
                card[i].haveCard = PlayerPrefs.GetInt("haveCard" + i.ToString(), 0);
            }
        }
            for (i = 1; i < card.Count; i++)
            {
                card[i].cardRest = card[i].haveCard;//残りカードの数＝手持ちカードの数（この後LoadDeckList関数やDropスクリプトのOnDrop関数でデッキ使用分を増減する）
            }
    }

    //デッキリストをセーブデータから取り出す
    public void LoadDeckList()
    {
        int i;
        if (PlayerPrefs.GetInt("deckCard0", 0) == 0)//初期状態なら
        {
            deckCard[0, 0] = card[1].Clone();
            deckCard[0, 1] = card[1].Clone();
            deckCard[0, 2] = card[1].Clone();
            deckCard[0, 3] = card[2].Clone();
            deckCard[0, 4] = card[2].Clone();
            deckCard[0, 5] = card[2].Clone();
            deckCard[0, 6] = card[3].Clone();
            deckCard[0, 7] = card[3].Clone();
            deckCard[0, 8] = card[3].Clone();
            deckCard[0, 9] = card[4].Clone();
            deckCard[0, 10] = card[4].Clone();
            deckCard[0, 11] = card[4].Clone();
            deckCard[0, 12] = card[5].Clone();
            deckCard[0, 13] = card[5].Clone();
            deckCard[0, 14] = card[5].Clone();
            deckCard[0, 15] = card[6].Clone();
            deckCard[0, 16] = card[6].Clone();
            deckCard[0, 17] = card[6].Clone();
            deckCard[0, 18] = card[7].Clone();
            deckCard[0, 19] = card[7].Clone();
        }
        else
        {
            for (i = 0; i < DECKCARD_NUM; i++)
            {
                deckCard[0, i] = card[PlayerPrefs.GetInt("deckCard" + i.ToString(), 1)].Clone();
            }//デッキのロード
        }
            for (i = 0; i < DECKCARD_NUM; i++)
            {
                card[deckCard[0, i].cardNum].cardRest--;//デッキで使っている分、残りカードの枚数を減らす。
            }
    }

    //デッキリストをセーブデータに保存する
    public void SaveDeckList()
    {
        int i;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            PlayerPrefs.SetInt("deckCard" + i.ToString(), deckCard[0, i].cardNum);//デッキのセーブ
        }
    }

    //敵のデッキリストやマナ獲得能力をロードする。（シーンをまたぐ時などに一度セーブデータに逃がしている）
    public void EnemyDeckList()
    {
        int i;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            deckCard[1, i] = card[PlayerPrefs.GetInt("enemyDeckCard" + i.ToString(), 1)];
        }
        for (i = 0; i < BLOCKTYPE_NUM + 1; i++)
        {
            enemyGetManaPace[i] = PlayerPrefs.GetInt("enemyGetManaPace" + i.ToString(), 100);
        }
    }

    //カードリスト
    public void CardList()
    {

        card.Clear();
        //①召喚について。AT…攻撃力。１ターンに１度この点数の攻撃を行う。DF…防御力。１ターンにこの点数以上のダメージを受けると破壊される。（ターンが終わればリセットされる）
        //②その他呪文について。スキルの処理についてはデリゲートによって配列管理した関数を使っている。
        card.Add(new Card());//採番に合わせ、０番にはカードを入れない。
        card.Add(
                new Card {
                    cardNum = 1,
                    cardName = "火花",
                    cardExplain = "<color=red>火花</color>\nコスト：<color=red><size=48>・★☽☀۞۝</size>：赤3</color>　　　　<b><color=black>Ｃ</color></b>\n対戦相手に1点のダメージを与える。\n\n<i>すべては小さな火花から始まる。</i>",
                    cardCost = new int[] { 0, 3, 0, 0, 0 },
                    damage = 1
                });
        card.Add(new Card {
            cardNum = 2,
            cardName = "火炎",
            cardExplain = "<color=red>火炎</color>\nコスト：赤9　　　　<b><color=black>Ｃ</color></b>\n対戦相手に2点のダメージを与える。\n\n<i>すべては炎を内包する。</i>",
            cardCost = new int[] { 9, 3, 0, 0, 0 },
            damage = 2
        });
        card.Add(new Card {
            cardNum = 3,
            cardName = "稲妻",
            cardExplain = "<color=red>稲妻</color>\nコスト：赤27　　　　<b><color=black>Ｃ</color></b>\n対戦相手に4点のダメージを与える。\n\n<i>すべては空がもたらす。</i>",
            cardCost = new int[] { 27, 3, 0, 0, 0 },
            damage = 4
        });
        card.Add(new Card {
            cardNum = 4,
            cardName = "業火",
            cardExplain = "<color=red>業火</color>\nコスト：赤81　　　　<b><color=black>Ｃ</color></b>\n対戦相手に6点のダメージを与える。\n\n<i>すべてを灰にする力。</i>",
            cardCost = new int[] { 81, 3, 0, 0, 0 },
            damage = 6
        });
        card.Add(new Card {
            cardNum = 5,
            cardName = "青光",
            cardExplain = "<color=red>青光</color>\nコスト：赤243　　　　<b><color=#a06000ff>ＵＣ</color></b>\n対戦相手に8点のダメージを与える。\n\n<i>すべてに沁みとおる青白い光。1934</i>",
            cardCost = new int[] { 243, 3, 0, 0, 0 },
            damage = 8
        });
        card.Add(new Card {
            cardNum = 6,
            cardName = "グイ",
            cardExplain = "<color=green>グイ</color>\nコスト：青3　　　　<b><color=black>Ｃ</color></b>\n対戦相手のライフを１点減少させる。\n\n<i>私たちはどこから来てどこへ行くのだろう。</i>",
            cardCost = new int[] { 0, 0, 3, 0, 0 },
            cardSkillDelegate = (int player) => { if (player == 0) { p1.lifePoint[1]--; } else { p1.lifePoint[1]--; } },
            cardSpeed = OTHER
        });
        card.Add(new Card {
            cardNum = 7,
            cardName = "テスタ",
            cardExplain = "<color=green>テスタ</color>\nコスト：青9　　　　<b><color=black>Ｃ</color></b>\n対戦相手に<b>ターン終了時：１点のライフを失う。\n効果終了時：３点のライフを失う。</b>の状態変化を与える。\n\n<i>私たちはどこから来てどこへ行くのだろう。</i>",
            cardCost = new int[] { 0, 0, 9, 0, 0 },
            cardSkillDelegate = (int player) => {
                if (player == 0) { p1.statusEffect[1].Add(new StatusEffect { cardNum = 7, restTurn = 2,effectExplain="ターン終了時：１点のダメージを受ける。\n効果終了時：３点のダメージを受ける。",statusEffectDelegate=(p)=> { p1.lifePoint[p]-- ; }, statusEndEffectDelegate = (p) => { p1.lifePoint[p]-=3; } }); } else {
                    p1.statusEffect[0].Add(new StatusEffect { cardNum = 7, restTurn = 2, effectExplain = "ターン終了時：１点のダメージを受ける。\n効果終了時：３点のダメージを受ける。", statusEffectDelegate = (p) => { p1.lifePoint[p]--; }, statusEndEffectDelegate = (p) => { p1.lifePoint[p] -= 3; } });
                } },
            cardSpeed = OTHER
        });
        
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                card.Add(new Card{ });
                

        LoadHaveCard();
    }


    /*
    //★第二種呪文効果関数に使用する汎用関数★
    //マナを減少させる効果
    public void Slip(int player, int lostMana)
    {
        int i, j;
        for (i = 0; i < HAND_NUM; i++)
        {
            for (j = 0; j < BLOCKTYPE_NUM + 1; j++)
            {
                if (p1.cardMana[player, i, j] >= p1.cardCost[player, i, j])//マナが足りて居たら
                {
                    p1.cardMana[player, i, j] = p1.cardCost[player, i, j] - lostMana;//コストからlostMana分だけ足りないように
                }
                else
                {
                    p1.cardMana[player, i, j] -= lostMana;//足りていなければその値からlostMana分減らす
                }
                if (p1.cardMana[player, i, j] < 0) { p1.cardMana[player, i, j] = 0; }//0未満になったら0に直す
            }
        }
    }

    //カードを捨てる効果
    public void Discard(int player)
    {
        for (int i = 0; i < HAND_NUM; i++)
        {
            if (p1.useCard[player, i] == false)
            {
                p1.DrawCard(player, i);
                for (int j = 1; j < BLOCKTYPE_NUM + 1; j++) { p1.cardMana[player, i, j] = 0; }//カードに貯まったマナもリセット
                p1.useCard[player, i] = false;//新たに引いたので、使っていない状態になおす。
                //カードを使用していないので非表示を表示にする処理はいらない。
                StartCoroutine(p1.ShakeCard(player, i));
            }
        }
    }

    //ライブラリを破壊する効果（残り０枚なら効果なし）
    public void LibraryBreak(int player, int count)
    {
        for (int l = 0; l < count; l++)
        {
            for (int i = 0; i < DECKCARD_NUM; i++)
            {
                if (p1.library[player, i, 0, 0] != 0)
                {//ライブラリの上から順にカードがある（０でない）ところまで探していく。
                    p1.library[player, i, 0, 0] = 0;//カードを引いたのでライブラリから消す。
                    p1.libraryNum[player]--;//ライブラリの残り枚数を1減らす。
                    StartCoroutine(p1.ShakeDeck(player));
                    break;
                }
            }
        }
    }

    //カードをライブラリに戻す
    public void HandEscape(int player)
    {
        for (int i = 0; i < HAND_NUM; i++)
        {
            if (p1.useCard[player, i] == false)
            {
                LibraryPut(player, p1.handCard[player, i]);
                p1.useCard[player, i] = true;
                p1.cardSkill[player, i] = 0; //非表示のカードは効果を発揮しない。
                p1.objCard[player, i].GetComponent<Image>().enabled = false;//戻したので非表示
            }
        }
    }

    //ライブラリの一番上にカードを置く
    public void LibraryPut(int player, int cardnum)
    {
        int i;
        if (p1.library[player, 0, 0, 0] == 0)//ライブラリが一杯でない時のみ
        {
            p1.libraryNum[player]++;
            p1.library[player, 0, 0, 0] = cardnum;
            //今あるカードの上に持って行く
            for (i = DECKCARD_NUM - 1; i > 0; i--)
            {
                if (p1.library[player, i, 0, 0] != 0)
                {
                    continue;
                }//カードがあるなら次を探査
                else
                {
                    p1.library[player, i, 0, 0] = p1.library[player, 0, 0, 0];
                    p1.library[player, 0, 0, 0] = 0;
                    break;
                }
            }
            for (int j = 0; j < BLOCKTYPE_NUM + 1; j++)
            {
                p1.library[player, i, 1, j] = card[p1.library[player, i, 0, 0]].cardCost[j];
            }//カードのコスト
        }
    }
    
*/


    // Use this for initialization
    void Start() {
        p1 = GetComponent<PuzzleSceneManager>();
        CardList();
        LoadDeckList();
    }

    // Update is called once per frame
    void Update() {

    }
}



public class Card
{
    const int BLOCKTYPE_NUM = 4;                 //ブロックの色の種類数
    public string cardName;
    public string cardExplain;
    public int[] cardCost = new int[BLOCKTYPE_NUM+1];                                                                  //必要なコスト。配列番号はマナ種類
    public int cardSpeed;                                                                 //呪文スピード
    public delegate void CardSkillDelegate(int usePlayer);                                  //特殊効果の関数アドレスを入れるデリゲート
    public CardSkillDelegate cardSkillDelegate;
    public int[,] buff = new int[2,2];                                                           //敵味方への攻守のバフ値
    public int damage;
    public int[] cardMana = new int[BLOCKTYPE_NUM+1];
    public int cardNum;
    public bool useCard;
    public int cardRest;
    public int haveCard;
    // コピーを作成するメソッド
    public Card Clone()
    {
        return (Card)MemberwiseClone();
    }

}


public class StatusEffect
{
    public int restTurn;//残りターン
    public int cardNum;//カード画像の番号
    public string effectExplain;//説明文
    public delegate void StatusEffectDelegate(int havePlayer);
    public StatusEffectDelegate statusEffectDelegate;//継続効果
    public StatusEffectDelegate statusEndEffectDelegate;//終了時効果
}

