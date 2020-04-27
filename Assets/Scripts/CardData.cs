using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;

[DefaultExecutionOrder(-1)]//CardDataは他から引用されるのでstartを先行処理させる。
public class CardData : MonoBehaviour {

    const int CARD_ALL = 100;                     //カードの全種類数
    const int BLOCKTYPE_NUM = 4;                 //ブロックの色の種類数
    const int SKILL_TYPE = 4;                    //カードのスキルタイプの数
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

    public int[,] deckCard = new int[2, DECKCARD_NUM];                   //デッキに入れているカードを示す配列。（中の数字がカード番号）
    public int[] haveCard = new int[CARD_ALL + 1];                       //各カードの持っている枚数
    public int[] enemyGetManaPace = new int[BLOCKTYPE_NUM + 1];          //敵のマナ獲得ペース
    public string[] cardName = new string[CARD_ALL + 1];                 //カードの名前
    public string[] cardExplain = new string[CARD_ALL + 1];              //カードの説明文
    public int[,] cardCost = new int[CARD_ALL + 1, BLOCKTYPE_NUM + 1];   //カードのコスト（１次元目がカードの番号、２次元目がマナの種類）
    public int[,] cardSkill = new int[CARD_ALL + 1, SKILL_TYPE];         //一次元目がカードの種類、二次元目がカードの効果の種別。
    public int[,] followerStatus = new int[CARD_ALL + 1, 3];             //シュジンコウのステータス（一次元目がカードの種類、二次元目が0==AT、1==DF、2==特殊能力）
    public delegate void CardSkill2Delegate(int usePlayer);                                  //第二種呪文の関数アドレスを入れるデリゲート
    public CardSkill2Delegate[] cardSkill2Use = new CardSkill2Delegate[CARD_ALL + 1];        //デリゲートを配列で扱う


    //持っているカードをセーブデータから取り出す
    public void LoadHaveCard(int x)
    {
        int i;

        for (i = 0; i < CARD_ALL + 1; i++)
        {
            haveCard[i] = 0;
        }
        if (PlayerPrefs.GetInt("haveCard1", 0) == 0)//初期状態なら
        {
            haveCard[1] = 2;
            haveCard[2] = 1;
            haveCard[3] = 1;
            haveCard[4] = 1;
            haveCard[5] = 2;
            haveCard[7] = 1;
            haveCard[8] = 1;
            haveCard[9] = 2;
            haveCard[10] = 2;
            haveCard[16] = 1;
            haveCard[17] = 1;
            haveCard[18] = 1;
            haveCard[19] = 1;
            haveCard[20] = 1;
            haveCard[21] = 2;
            for (i = 1; i < CARD_ALL + 1; i++)
            {
                PlayerPrefs.SetInt("haveCard" + i.ToString(), haveCard[i]);
            }
        }
        else
        {
            for (i = 1; i < CARD_ALL + 1; i++)
            {
                haveCard[i] = PlayerPrefs.GetInt("haveCard" + i.ToString(), 0);
            }
        }

        if (x == 1)
        {
            MakeBookSceneManager m1 = GetComponent<MakeBookSceneManager>();
            for (i = 0; i < CARD_ALL + 1; i++)
            {
                m1.cardRest.Add(0);//cardRestリストの初期化と要素数の確保。
            }
            for (i = 1; i < CARD_ALL + 1; i++)
            {
                m1.cardRest[i] = haveCard[i];//残りカードの数＝手持ちカードの数（この後LoadDeckList関数やDropスクリプトのOnDrop関数でデッキ使用分を増減する）
            }
        }
    }

    //デッキリストをセーブデータから取り出す
    public void LoadDeckList(int x)//0が通常時（パズルゲーム時）、1がデッキ作成時
    {
        int i;
        if (PlayerPrefs.GetInt("deckCard0", 0) == 0)//初期状態なら
        {
            deckCard[0, 0] = 1;
            deckCard[0, 1] = 1;
            deckCard[0, 2] = 2;
            deckCard[0, 3] = 3;
            deckCard[0, 4] = 4;
            deckCard[0, 5] = 5;
            deckCard[0, 6] = 5;
            deckCard[0, 7] = 7;
            deckCard[0, 8] = 8;
            deckCard[0, 9] = 9;
            deckCard[0, 10] = 9;
            deckCard[0, 11] = 10;
            deckCard[0, 12] = 10;
            deckCard[0, 13] = 16;
            deckCard[0, 14] = 17;
            deckCard[0, 15] = 18;
            deckCard[0, 16] = 19;
            deckCard[0, 17] = 20;
            deckCard[0, 18] = 21;
            deckCard[0, 19] = 21;
        }
        else
        {
            for (i = 0; i < DECKCARD_NUM; i++)
            {
                deckCard[0, i] = PlayerPrefs.GetInt("deckCard" + i.ToString(), 1);
            }//デッキのロード
        }
        if (x == 1)//デッキ作成時に呼ばれた場合（そうでない場合にここを呼び出すと、objectがないのでエラー）
        {
            MakeBookSceneManager m1 = GetComponent<MakeBookSceneManager>();
            for (i = 0; i < DECKCARD_NUM; i++)
            {
                m1.cardRest[deckCard[0, i]]--;//デッキで使っている分、残りカードの枚数を減らす。
            }
        }
    }

    //デッキリストをセーブデータに保存する
    public void SaveDeckList()
    {
        int i;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            PlayerPrefs.SetInt("deckCard" + i.ToString(), deckCard[0, i]);//デッキのセーブ
        }
    }

    //敵のデッキリストやマナ獲得能力をロードする。（シーンをまたぐ時などに一度セーブデータに逃がしている）
    public void EnemyDeckList()
    {
        int i;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            deckCard[1, i] = PlayerPrefs.GetInt("enemyDeckCard" + i.ToString(), 1);
        }
        for (i = 1; i < BLOCKTYPE_NUM + 1; i++)
        {
            enemyGetManaPace[i] = PlayerPrefs.GetInt("enemyGetManaPace" + i.ToString(), 100);
        }
    }


    //カードリスト(カードの内容を参照する際はコイツを呼び出してから参照しないと空のカードデータが返される)
    public void CardList()
    {

        int i;

        //①召喚について。AT…攻撃力。１ターンに１度この点数の攻撃を行う。DF…防御力。１ターンにこの点数以上のダメージを受けると破壊される。（ターンが終わればリセットされる）
        //②その他呪文について。スキルの処理についてはデリゲートによって配列管理した関数を使っている。

        //初期カード群
        i = 1;
        cardName[i] = "ショック";
        cardExplain[i] = "<color=red>ショック</color>\nコスト：赤10　　　　<b><color=black>Ｃ</color></b>\n対戦相手に2点のダメージを与える。\n\n<i>衝撃と痛みと。</i>";
        cardCost[i, 1] = 10;
        cardSkill[i, 3] = 2;

        i = 2;
        cardName[i] = "ブラスト";
        cardExplain[i] = "<color=red>ブラスト</color>\nコスト：赤30　　　　<b><color=black>Ｃ</color></b>\n対戦相手に4点のダメージを与える。\n\n<i>響く轟音、揺らめく世界。</i>";
        cardCost[i, 1] = 30;
        cardSkill[i, 3] = 4;

        i = 3;
        cardName[i] = "フレイム";
        cardExplain[i] = "<color=red>フレイム</color>\nコスト：赤60　　　　<b><color=#a06000ff>ＵＣ</color></b>\n対戦相手に6点のダメージを与える。\n\n<i>燃え上がる炎を覚えている。</i>";
        cardCost[i, 1] = 60;
        cardSkill[i, 3] = 6;

        i = 4;
        cardName[i] = "インフェルノ";
        cardExplain[i] = "<color=red>インフェルノ</color>\nコスト：赤100　　　　<b><color=#ff5000ff>Ｒ</color></b>\n対戦相手に8点のダメージを与える。\n\n<i>炎は全てを灰にする。</i>";
        cardCost[i, 1] = 100;
        cardSkill[i, 3] = 8;

        i = 5;
        cardName[i] = "グイ";
        cardExplain[i] = "<color=green>グイ</color>\nコスト：緑5　　　　<b><color=black>Ｃ</color></b>\nAT1/DF1\n\n<i>私たちはどこから来てどこへ行くのだろう。</i>";
        cardCost[i, 3] = 5;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 1;

        i = 6;
        cardName[i] = "キャシー";
        cardExplain[i] = "<color=green>キャシー</color>\nコスト：緑30青10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT2/DF3\n＜常在能力＞自身は各色１点のマナを獲得する。\n\n<i>他の物語から紛れ込んだ１ページ。</i>";
        cardCost[i, 3] = 30;
        cardCost[i, 2] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 3;
        followerStatus[i, 2] = i;

        i = 7;
        cardName[i] = "水晶の剣";
        cardExplain[i] = "<color=olive>水晶の剣</color>\nコスト：黄4　　　　<b><color=black>Ｃ</color></b>\n自身のシュジンコウのATを+1する。\n\n<i>見せかけだけの武器。</i>";
        cardCost[i, 4] = 5;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 0] = 1;

        i = 8;
        cardName[i] = "水晶の盾";
        cardExplain[i] = "<color=olive>水晶の盾</color>\nコスト黄8　　　　<b><color=black>Ｃ</color></b>\n自身のシュジンコウのDFを+1する。\n\n<i>おとぎ話にリアリティはいらない。</i>";
        cardCost[i, 4] = 10;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 1] = 1;

        i = 9;
        cardName[i] = "火花";
        cardExplain[i] = "<color=red>火花</color>\nコスト：赤5　　　　<b><color=#a06000ff>ＵＣ</color></b>\n対戦相手に1点のダメージを与える。\n\n<i>どんな大火も、最初は小さな火花だった。</i>";
        cardCost[i, 1] = 5;
        cardSkill[i, 3] = 1;

        i = 10;
        cardName[i] = "マッドハッター";
        cardExplain[i] = "<color=green>マッドハッター</color>\nコスト：赤5緑10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT2/DF1\n\n<i>今日もどこかの誰かが誕生日。『誕生日じゃない日』はいつだろう？</i>";
        cardCost[i, 1] = 5;
        cardCost[i, 3] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 1;

        i = 11;
        cardName[i] = "黒曜石の剣";
        cardExplain[i] = "<color=olive>黒曜石の剣</color>\nコスト：黄10　　　　<b><color=#a06000ff>ＵＣ</color></b>\n自身のシュジンコウのATを+2する。\n\n<i>切れ味鋭いファンタジー世界の剣。</i>";
        cardCost[i, 4] = 10;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 0] = 2;

        i = 12;
        cardName[i] = "黒曜石の盾";
        cardExplain[i] = "<color=olive>黒曜石の盾</color>\nコスト黄20　　　　<b><color=#a06000ff>ＵＣ</color></b>\n自身のシュジンコウのDFを+2する。\n\n<i>おとぎ話の中での戦いは華やかでわくわくする。</i>";
        cardCost[i, 4] = 20;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 1] = 2;

        i = 13;
        cardName[i] = "コフィン";
        cardExplain[i] = "<color=green>コフィン</color>\nコスト：緑4青4　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT1/DF2\n＜常在能力＞自身は各色5点のマナを失う。\n\n<i>他の物語から紛れ込んだ１ページ。</i>";
        cardCost[i, 3] = 4;
        cardCost[i, 2] = 4;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 2;
        followerStatus[i, 2] = i;

        i = 14;
        cardName[i] = "二首";
        cardExplain[i] = "<color=green>二首</color>\nコスト：緑10青10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT2/DF2\n＜常在能力＞自身のライブラリを上から１枚捨てる。\n\n<i>他の物語から紛れ込んだ１ページ。</i>";
        cardCost[i, 2] = 10;
        cardCost[i, 3] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 2;
        followerStatus[i, 2] = i;

        i = 15;
        cardName[i] = "眠気";
        cardExplain[i] = "<color=blue>眠気</color>\nコスト：青15　　　　<b><color=black>Ｃ</color></b>\n対戦相手は各色1点のマナを失う。この呪文は特殊呪文として扱う。\n\n<i>まどろみの中、本を読む声が聞こえる。</i>";
        cardCost[i, 2] = 15;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i]= new CardSkill2Delegate(Card15Skill2);

        i = 16;
        cardName[i] = "睡魔";
        cardExplain[i] = "<color=blue>睡魔</color>\nコスト：青30　　　　<b><color=#a06000ff>ＵＣ</color></b>\n対戦相手は各色8点のマナを失う。この呪文は特殊呪文として扱う。\n\n<i>足元注意。</i>";
        cardCost[i, 2] = 30;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card16Skill2);

        i = 17;
        cardName[i] = "長靴を履いた猫";
        cardExplain[i] = "<color=green>長靴を履いた猫</color>\nコスト：緑30赤20青10　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT4/DF3\n\n<i>お父さんがのこした宝物。</i>";
        cardCost[i, 3] = 30;
        cardCost[i, 1] = 20;
        cardCost[i, 2] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 4;
        followerStatus[i, 1] = 3;

        i = 18;
        cardName[i] = "過ち";
        cardExplain[i] = "<color=#666666ff>過ち</color>\nコスト：黒30黄10　　　　<b><color=#ff5000ff>Ｒ</color></b>\n対戦相手のシュジンコウのDFを-5する。\n\n<i>知らないうちに手元にあった１ページ。</i>";
        cardCost[i, 4] = 30;
        cardCost[i, 4] = 10;
        cardSkill[i, 1] = YOURS;
        followerStatus[i, 1] = -5;

        i = 19;
        cardName[i] = "速読";
        cardExplain[i] = "<color=blue>速読</color>\nコスト：青4　　　　<b><color=#ff5000ff>Ｒ</color></b>\n何もおこらない。この呪文は特殊呪文として扱う。\n\n<i>急がないで。読み飛ばさないで。この大切な時間を。</i>";
        cardCost[i, 2] = 4;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card19Skill2);

        i = 20;
        cardName[i] = "鉄砲玉のマサ";
        cardExplain[i] = "<color=green>鉄砲玉のマサ</color>\nコスト：緑20赤10　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT3/DF1\n\n<i>任侠映画の主人公。</i>";
        cardCost[i, 3] = 20;
        cardCost[i, 1] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 3;
        followerStatus[i, 1] = 1;

        i = 21;
        cardName[i] = "猛火";
        cardExplain[i] = "<color=red>猛火</color>\nコスト：赤25　　　　<b><color=black>Ｃ</color></b>\n対戦相手に3点のダメージを与える。\n\n<i>消せない炎。</i>";
        cardCost[i, 1] = 25;
        cardSkill[i, 3] = 3;

        i = 22;
        cardName[i] = "犬";
        cardExplain[i] = "<color=green>犬</color>\nコスト：緑20赤10　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT3/DF2\nこのシュジンコウが登場する際、特殊呪文として自身に5点のダメージを与える。\n\n<i>出来損ないの学芸会。</i>";
        cardCost[i, 1] = 10;
        cardCost[i, 3] = 20;
        cardSkill[i, 2] = OTHER;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 3;
        followerStatus[i, 1] = 2;
        cardSkill2Use[i]= new CardSkill2Delegate(Card22Skill2);

        i = 23;
        cardName[i] = "猿";
        cardExplain[i] = "<color=green>猿</color>\nコスト：緑30　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT0/DF4\n\n<i>気の毒な代役。</i>";
        cardCost[i, 3] = 30;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 4;

        i = 24;
        cardName[i] = "桃太郎";
        cardExplain[i] = "<color=green>桃太郎</color>\nコスト：緑30赤10　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT3/DF2\n\n<i>きっと、つよい、はず。戦ってるのは見たことないけど。</i>";
        cardCost[i, 3] = 30;
        cardCost[i, 1] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 3;
        followerStatus[i, 1] = 2;

        i = 25;
        cardName[i] = "魂の蹂躙";
        cardExplain[i] = "<color=#666666ff>魂の蹂躙</color>\nコスト：青20黒30　　　　<b><color=#ff5000ff>Ｒ</color></b>\n対戦相手は各色100点のマナを失う。この呪文は特殊呪文として扱う。\n\n<i>愛は壊れ、魂は穢される。</i>";
        cardCost[i, 2] = 20;
        cardCost[i, 4] = 30;
        cardSkill[i, 1] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card15Skill2);

        i = 26;
        cardName[i] = "赤鬼";
        cardExplain[i] = "<color=green>赤鬼</color>\nコスト：緑20赤20　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT2/DF1\nこのシュジンコウは登場の際に攻撃呪文として対戦相手に1点のダメージを与える。\n\n<i>鬼の目にも涙。</i>";
        cardCost[i, 3] = 20;
        cardCost[i, 1] = 20;
        cardSkill[i, 0] = SUMMON;
        cardSkill[i, 3] = 1;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 1;

        i = 27;
        cardName[i] = "青鬼";
        cardExplain[i] = "<color=green>青鬼</color>\nコスト：緑30青10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT1/DF3\n\n<i>昔話の汎用エネミー。一目見ただけで泣き出しそうな怖さ。しかし、とある物語では彼を見て別の意味で泣き出す読者が後をたたないとか。</i>";
        cardCost[i, 3] = 30;
        cardCost[i, 2] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 3;

        i = 28;
        cardName[i] = "雉";
        cardExplain[i] = "<color=blue>雉</color>\nコスト：緑20青10　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT1/DF2\nこのシュジンコウは特殊呪文フェイズに召喚される。この呪文は特殊呪文として扱う。\n\n<i>雉も鳴かずば撃たれまいに。</i>";
        cardCost[i, 3] = 20;
        cardCost[i, 2] = 10;
        cardSkill[i, 2] = SUMMON;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 2;
        cardSkill2Use[i] = new CardSkill2Delegate(Card28Skill2);

        i = 29;
        cardName[i] = "終わらない物語";
        cardExplain[i] = "<color=blue>終わらない物語</color>\nコスト青100　　　　<b><color=#ff5000ff>Ｒ</color></b>\n自身のライブラリをリセットする。（手札や場の状況には影響しない）。この呪文は特殊呪文として扱う。\n\n<i>バッドエンドのない世界。</i>";
        cardCost[i, 2] = 100;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card29Skill2);

        i = 30;
        cardName[i] = "十四年式拳銃";
        cardExplain[i] = "<color=olive>十四年式拳銃</color>\nコスト：黄80　　　　<b><color=#a06000ff>ＵＣ</color></b>\n自身のシュジンコウのATを+5する。\n\n<i>人の命を奪う武器。</i>";
        cardCost[i, 4] = 80;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 0] = 5;

        i = 31;
        cardName[i] = "ボディアーマー";
        cardExplain[i] = "<color=olive>ボディアーマー</color>\nコスト黄100　　　　<b><color=#a06000ff>ＵＣ</color></b>\n自身のシュジンコウのDFを+5する。\n\n<i>現実の世界は、ただただ悲しい。</i>";
        cardCost[i, 4] = 100;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 1] = 5;

        i = 32;
        cardName[i] = "炎の嵐";
        cardExplain[i] = "<color=red>炎の嵐</color>\nコスト：赤50　　　　<b><color=black>Ｃ</color></b>\n対戦相手に5点のダメージを与える。\n\n<i>燃え広がった炎は、多くの人を巻き込んでいく。</i>";
        cardCost[i, 1] = 50;
        cardSkill[i, 3] = 5;

        i = 33;
        cardName[i] = "火の粉";
        cardExplain[i] = "<color=blue>火の粉</color>\nコスト：赤10青10　　　　<b><color=#a06000ff>ＵＣ</color></b>\n特殊呪文フェイズと攻撃呪文フェイズにそれぞれ1回、対戦相手に1点のダメージを与える。この呪文は特殊呪文として扱う。\n\n<i>私が何をしたというんだろう。</i>";
        cardCost[i, 1] = 10;
        cardCost[i, 2] = 10;
        cardSkill[i, 3] = 1;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card33Skill2);

        i = 34;
        cardName[i] = "転進";
        cardExplain[i] = "<color=#666666ff>転進</color>\nコスト：黒10黄10　　　　<b><color=black>Ｃ</color></b>\n対戦相手のシュジンコウのDFを-1する。\n\n<i>英語で言うラウト。</i>";
        cardCost[i, 4] = 10;
        cardCost[i, 4] = 10;
        cardSkill[i, 1] = YOURS;
        followerStatus[i, 1] = -1;

        i = 35;
        cardName[i] = "謀略";
        cardExplain[i] = "<color=#666666ff>謀略</color>\nコスト：黒20黄10　　　　<b><color=#a06000ff>ＵＣ</color></b>\n対戦相手のシュジンコウのDFを-3する。\n\n<i>恰好良い戦いなんてない。</i>";
        cardCost[i, 4] = 20;
        cardCost[i, 4] = 10;
        cardSkill[i, 1] = YOURS;
        followerStatus[i, 1] = -3;

        i = 36;
        cardName[i] = "先制射撃";
        cardExplain[i] = "<color=blue>先制射撃</color>\nコスト：赤5青5　　　　<b><color=black>Ｃ</color></b>\n対戦相手に1点のダメージを与える。この呪文は特殊呪文として扱う。\n\n<i>逃げ延びるための非常手段。</i>";
        cardCost[i, 1] = 5;
        cardCost[i, 2] = 5;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card36Skill2);

        i = 37;
        cardName[i] = "チンピラ";
        cardExplain[i] = "<color=green>チンピラ</color>\nコスト：緑30　　　　<b><color=black>Ｃ</color></b>\nAT2/DF2\n\n<i>量産型チンピラ。数がいるので侮れない。</i>";
        cardCost[i, 3] = 30;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 2;

        i = 38;
        cardName[i] = "コスモス";
        cardExplain[i] = "<color=blue>コスモス</color>\nコスト：緑5青20　　　　<b><color=black>Ｃ</color></b>\n自身のＬＰを5点回復させる。この呪文は特殊呪文として扱う。\n\n<i>感傷とおもいで、そしてひとつまみの寂しさ。</i>";
        cardCost[i, 2] = 20;
        cardCost[i, 3] = 5;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card38Skill2);

        i = 39;
        cardName[i] = "特攻";
        cardExplain[i] = "<color=#666666ff>特攻</color>\nコスト：黒15　　　　<b><color=black>Ｃ</color></b>\n互いのシュジンコウのDFを-5する。（片方でもシュジンコウがいなければ失敗する）。この呪文は特殊呪文として扱う。\n\n<i>自滅作戦。</i>";
        cardCost[i, 4] = 15;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card39Skill2);

        i = 40;
        cardName[i] = "隠滅";
        cardExplain[i] = "<color=#666666ff>隠滅</color>\nコスト：黒5　　　　<b><color=black>Ｃ</color></b>\n自身のシュジンコウのDFを-10する。\n\n<i>不都合な証拠は残さない。</i>";
        cardCost[i, 4] = 5;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 1] = -10;

        i = 41;
        cardName[i] = "赤ずきん";
        cardExplain[i] = "<color=green>赤ずきん</color>\nコスト：緑30赤30　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT4/DF2\n\n<i>小さな悪戯心が生み出したモンスター。</i>";
        cardCost[i, 3] = 30;
        cardCost[i, 1] = 30;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 4;
        followerStatus[i, 1] = 2;

        i = 42;
        cardName[i] = "オオカミ";
        cardExplain[i] = "<color=green>オオカミ</color>\nコスト：緑30青10赤10　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT3/DF1\nこのシュジンコウは登場の際に攻撃呪文として対戦相手に1点のダメージを与える。\n\n<i>赤ずきんの中で『狼（オオカミ）』の役割（ロール）を得たマサ。\n――悲しんでくれる人がいるのは幸せなこと。</i>";
        cardCost[i, 3] = 30;
        cardCost[i, 1] = 10;
        cardCost[i, 2] = 10;
        cardSkill[i, 0] = SUMMON;
        cardSkill[i, 3] = 1;
        followerStatus[i, 0] = 3;
        followerStatus[i, 1] = 1;

        i = 43;
        cardName[i] = "組員";
        cardExplain[i] = "<color=green>組員</color>\nコスト：緑20黄10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT2/DF1\n\n<i>なぜ赤ずきんの物語にヤクザが出て来るのか。</i>";
        cardCost[i, 3] = 20;
        cardCost[i, 4] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 1;

        i = 44;
        cardName[i] = "無力化";
        cardExplain[i] = "<color=#666666ff>無力化</color>\nコスト：赤10黒20　　　　<b><color=#ff5000ff>Ｒ</color></b>\n対戦相手のシュジンコウのATを-3する。\n\n<i>迅速な鎮圧。</i>";
        cardCost[i, 1] = 10;
        cardCost[i, 4] = 20;
        cardSkill[i, 1] = YOURS;
        followerStatus[i, 0] = -3;

        i = 45;
        cardName[i] = "忘却";
        cardExplain[i] = "<color=#666666ff>忘却</color>\nコスト：青10黒10　　　　<b><color=#a06000ff>ＵＣ</color></b>\n自身のシュジンコウのDFを-10し、自身のＬＰを10点回復する。（自身にシュジンコウがいなければ呪文自体が失敗する）。この呪文は特殊呪文として扱う。\n\n<i>一番恐ろしいこと。</i>";
        cardCost[i, 2] = 10;
        cardCost[i, 4] = 10;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card45Skill2);

        //パック：幸福な王子
        i = 46;
        cardName[i] = "召喚阻害";
        cardExplain[i] = "<color=blue>召喚阻害</color>\nコスト：青10緑10黒10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nこのターンの召喚フェイズを飛ばす（使用されるはずだった呪文はそのまま捨てられる）。この呪文は特殊呪文として扱う。\n\n<i>助けを呼ぶ声は届かない。</i>";
        cardCost[i, 2] = 10;
        cardCost[i, 3] = 10;
        cardCost[i, 4] = 10;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card46Skill2);

        i = 47;
        cardName[i] = "攻呪阻害";
        cardExplain[i] = "<color=blue>攻呪阻害</color>\nコスト：青10赤10黒10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nこのターンの攻撃呪文フェイズを飛ばす（使用されるはずだった呪文はそのまま捨てられる）。この呪文は特殊呪文として扱う。\n\n<i>銃のない世界。</i>";
        cardCost[i, 1] = 10;
        cardCost[i, 2] = 10;
        cardCost[i, 4] = 10;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card47Skill2);

        i = 48;
        cardName[i] = "強化阻害";
        cardExplain[i] = "<color=blue>強化阻害</color>\nコスト：青10黒10黄10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nこのターンの強化呪文フェイズを飛ばす（使用されるはずだった呪文はそのまま捨てられる）。この呪文は特殊呪文として扱う。\n\n<i>武器を捨てた先に見えるもの。</i>";
        cardCost[i, 2] = 10;
        cardCost[i, 4] = 10;
        cardCost[i, 4] = 10;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card48Skill2);

        i = 49;
        cardName[i] = "戦闘阻害";
        cardExplain[i] = "<color=blue>戦闘阻害</color>\nコスト：緑4黒4黄4　　　　<b><color=#a06000ff>ＵＣ</color></b>\nこのターンの常在能力フェイズと戦闘フェイズを飛ばす。この呪文は特殊呪文として扱う。\n\n<i>動けなくても時間は進む。</i>";
        cardCost[i, 3] = 4;
        cardCost[i, 4] = 4;
        cardCost[i, 4] = 4;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card49Skill2);

        i = 50;
        cardName[i] = "特殊阻害";
        cardExplain[i] = "<color=blue>特殊阻害</color>\nコスト：青10赤10緑10　　　　<b><color=#a06000ff>ＵＣ</color></b>\nこのターンの特殊呪文フェイズを飛ばす。この呪文は特殊呪文として扱う。\n\n<i>逃げ足は誰より早い。</i>";
        cardCost[i, 1] = 10;
        cardCost[i, 2] = 10;
        cardCost[i, 3] = 10;
        cardSkill[i, 2] = COUNTER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card50Skill2);

        i = 51;
        cardName[i] = "排撃";
        cardExplain[i] = "<color=blue>排撃</color>\nコスト：黒40　　　　<b><color=#ff5000ff>Ｒ</color></b>\n対戦相手は使用状態にない呪文を全て捨てる。この呪文は特殊呪文として扱う。\n\n<i>準備がおろそかなら、武器はがらくたと変わらない。</i>";
        cardCost[i, 4] = 40;
        cardSkill[i, 2] = HAND_CHANGE;
        cardSkill2Use[i] = new CardSkill2Delegate(Card51Skill2);

        i = 52;
        cardName[i] = "焚書";
        cardExplain[i] = "<color=blue>焚書</color>\nコスト：黒30　　　　<b><color=#ff5000ff>Ｒ</color></b>\n対戦相手のライブラリを上から３枚捨てる。この呪文は特殊呪文として扱う。\n\n<i>物語が燃えてゆく。</i>";
        cardCost[i, 4] = 30;
        cardSkill[i, 2] = DECK_EAT;
        cardSkill2Use[i] = new CardSkill2Delegate(Card52Skill2);

        i = 53;
        cardName[i] = "幸福な王子";
        cardExplain[i] = "<color=green>幸福な王子</color>\nコスト：赤20青20緑20黒20黄20　　　　<b><color=#ff5000ff>Ｒ</color></b>\nAT0/DF1\nこのシュジンコウが登場する際、特殊呪文として対戦相手に(10-自身のライブラリ残り枚数)点のダメージを与える。\n\n<i>燕が可哀想だとは思わなかったのだろうか。</i>";
        cardCost[i, 2] = 20;
        cardCost[i, 2] = 20;
        cardCost[i, 3] = 20;
        cardCost[i, 4] = 20;
        cardCost[i, 4] = 20;
        cardSkill[i, 0] = SUMMON;
        cardSkill[i, 2] = OTHER;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 1;
        cardSkill2Use[i] = new CardSkill2Delegate(Card53Skill2);

        i = 54;
        cardName[i] = "破り捨て";
        cardExplain[i] = "<color=blue>破り捨て</color>\nコスト：青10黒20黄10　　　　<b><color=black>Ｃ</color></b>\nあなたと対戦相手は使用状態にない呪文を全て捨てる。この呪文は特殊呪文として扱う。\n\n<i>痛み分けで十分。</i>";
        cardCost[i, 2] = 10;
        cardCost[i, 4] = 20;
        cardCost[i, 4] = 10;
        cardSkill[i, 2] = HAND_CHANGE;
        cardSkill2Use[i] = new CardSkill2Delegate(Card54Skill2);

        i = 55;
        cardName[i] = "議員";
        cardExplain[i] = "<color=green>議員</color>\nコスト：緑20黒20　　　　<b><color=black>Ｃ</color></b>\nAT2/DF2\n\n<i>調子のいい市長の取り巻き。</i>";
        cardCost[i, 3] = 20;
        cardCost[i, 4] = 20;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 2;

        i = 56;
        cardName[i] = "市長";
        cardExplain[i] = "<color=green>市長</color>\nコスト：緑40黒30黄20　　　　<b><color=black>Ｃ</color></b>\nAT3/DF3\n\n<i>偉そうにするのが仕事。</i>";
        cardCost[i, 3] = 40;
        cardCost[i, 4] = 30;
        cardCost[i, 4] = 20;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 3;
        followerStatus[i, 1] = 3;

        i = 57;
        cardName[i] = "貧困";
        cardExplain[i] = "<color=#666666ff>貧困</color>\nコスト：赤5黒10　　　　<b><color=black>Ｃ</color></b>\n対戦相手のシュジンコウのATを-1する。\n\n<i>武器を買うお金がない。</i>";
        cardCost[i, 1] = 5;
        cardCost[i, 4] = 10;
        cardSkill[i, 1] = YOURS;
        followerStatus[i, 0] = -1;

        i = 58;
        cardName[i] = "ジャンク化";
        cardExplain[i] = "<color=#666666ff>ジャンク化</color>\nコスト：赤30黒60　　　　<b><color=black>Ｃ</color></b>\n対戦相手のシュジンコウのATを-5する。\n\n<i>うち捨てられたガラクタ。</i>";
        cardCost[i, 1] = 30;
        cardCost[i, 4] = 60;
        cardSkill[i, 1] = YOURS;
        followerStatus[i, 0] = -5;

        i = 59;
        cardName[i] = "墨塗り";
        cardExplain[i] = "<color=blue>墨塗り</color>\nコスト：黒10　　　　<b><color=black>Ｃ</color></b>\n対戦相手のライブラリを上から１枚捨てる。この呪文は特殊呪文として扱う。\n\n<i>黒塗りされたページ。</i>";
        cardCost[i, 4] = 10;
        cardSkill[i, 2] = DECK_EAT;
        cardSkill2Use[i] = new CardSkill2Delegate(Card59Skill2);

        i = 60;
        cardName[i] = "燕";
        cardExplain[i] = "<color=green>燕</color>\nコスト：緑4　　　　<b><color=black>Ｃ</color></b>\nAT1/DF2\nこのシュジンコウが登場する際、特殊呪文として自身のライブラリを3枚捨てる。\n\n<i>彼は何のために生きたんだろう。</i>";
        cardCost[i, 3] = 4;
        cardSkill[i, 0] = SUMMON;
        cardSkill[i, 2] = DECK_EAT;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 2;
        cardSkill2Use[i] = new CardSkill2Delegate(Card60Skill2);

        i = 61;
        cardName[i] = "里子";
        cardExplain[i] = "<color=blue>里子</color>\nコスト：青10黒10　　　　<b><color=#ff5000ff>Ｒ</color></b>\n互いのシュジンコウを交換する。（片方でもシュジンコウがいなければ失敗する）。この呪文は特殊呪文として扱う。\n\n<i>育てられなかった子供。</i>";
        cardCost[i, 2] = 10;
        cardCost[i, 4] = 10;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card61Skill2);

        i = 62;
        cardName[i] = "利己的な愛";
        cardExplain[i] = "<color=blue>利己的な愛</color>\nコスト：赤50黒50　　　　<b><color=#ff5000ff>Ｒ</color></b>\nあなたと対戦相手に、それぞれ10点のダメージを与える。この呪文は特殊呪文として扱う。\n\n<i>望んで迎えたバッドエンド。</i>";
        cardCost[i, 1] = 50;
        cardCost[i, 4] = 50;
        cardSkill[i, 2] = OTHER;
        cardSkill2Use[i] = new CardSkill2Delegate(Card62Skill2);

        //パック：アリス
        i = 63;
        cardName[i] = "チェシャ猫";
        cardExplain[i] = "<color=green>チェシャ猫</color>\nコスト：緑20青20黒10　　　　<b><color=#a06000ff>Ｒ</color></b>\nAT2/DF2\n＜常在能力＞このターン使用されないカードをライブラリに戻す。\n\n<i>神出鬼没な道化。そして、もう一人の案内人。</i>";
        cardCost[i, 2] = 20;
        cardCost[i, 3] = 20;
        cardCost[i, 4] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 2;
        followerStatus[i, 2] = i;

        i = 64;
        cardName[i] = "アリス";
        cardExplain[i] = "<color=green>アリス</color>\nコスト：緑10青10　　　　<b><color=#a06000ff>Ｒ</color></b>\nAT0/DF1\n＜常在能力＞お互いのライブラリをリセットする。（手札や場の状況には影響しない）\n\n<i>有り得ぬ世界を眺める空想の主。</i>";
        cardCost[i, 2] = 10;
        cardCost[i, 3] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 65;
        cardName[i] = "ハートの女王";
        cardExplain[i] = "<color=green>ハートの女王</color>\nコスト：緑20赤20黄40　　　　<b><color=#a06000ff>Ｒ</color></b>\nAT2/DF3\n＜常在能力＞このシュジンコウのATを+1する。\n\n<i>癇癪持ちの女王様。</i>";
        cardCost[i, 1] = 20;
        cardCost[i, 3] = 20;
        cardCost[i, 4] = 40;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 2;
        followerStatus[i, 1] = 3;
        followerStatus[i, 2] = i;

        i = 66;
        cardName[i] = "三月ウサギ";
        cardExplain[i] = "<color=green>三月ウサギ</color>\nコスト：緑10黄30　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT0/DF1\n＜常在能力＞このシュジンコウのDFを+1する。\n\n<i>狂った世界と狂った感性、本当に狂っているのは誰だろう。</i>";
        cardCost[i, 3] = 10;
        cardCost[i, 4] = 30;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 67;
        cardName[i] = "白ウサギ";
        cardExplain[i] = "<color=green>白ウサギ</color>\nコスト：緑4黄4　　　　<b><color=#a06000ff>Ｒ</color></b>\nAT1/DF6\n＜常在能力＞このシュジンコウのDFを-1する。\n\n<i>夢の世界に飛び込むきっかけ。</i>";
        cardCost[i, 3] = 4;
        cardCost[i, 4] = 4;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 6;
        followerStatus[i, 2] = i;

        i = 68;
        cardName[i] = "公爵夫人";
        cardExplain[i] = "<color=green>公爵夫人</color>\nコスト：緑10黒30　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT0/DF1\n＜常在能力＞対戦相手のシュジンコウのDFを-1する。\n\n<i>ざわ・・・ざわ・・・。</i>";
        cardCost[i, 3] = 10;
        cardCost[i, 4] = 30;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 69;
        cardName[i] = "眠りネズミ";
        cardExplain[i] = "<color=green>眠りネズミ</color>\nコスト：緑10黄20　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT0/DF1\n＜常在能力＞対戦相手のシュジンコウのATを-1する。\n\n<i>ティーポットの中で冬眠中。</i>";
        cardCost[i, 3] = 10;
        cardCost[i, 4] = 30;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 70;
        cardName[i] = "代用ウミガメ";
        cardExplain[i] = "<color=green>代用ウミガメ</color>\nコスト：緑5赤5　　　　<b><color=#a06000ff>ＵＣ</color></b>\nAT0/DF1\n＜常在能力＞対戦相手に１点のダメージを与える。\n\n<i>偽物の人生。</i>";
        cardCost[i, 1] = 5;
        cardCost[i, 3] = 5;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 71;
        cardName[i] = "クラブのトランプ";
        cardExplain[i] = "<color=green>クラブのトランプ</color>\nコスト：緑20赤20　　　　<b><color=black>Ｃ</color></b>\nAT1/DF1\n＜常在能力＞対戦相手に１点のダメージを与える。\n\n<i>頼りないトランプの兵士。</i>";
        cardCost[i, 1] = 20;
        cardCost[i, 3] = 20;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 72;
        cardName[i] = "ハートのトランプ";
        cardExplain[i] = "<color=green>ハートのトランプ</color>\nコスト：緑10黄10　　　　<b><color=black>Ｃ</color></b>\nAT1/DF1\n＜常在能力＞自身は１点のライフを回復する。\n\n<i>間の抜けたトランプの貴族。</i>";
        cardCost[i, 3] = 10;
        cardCost[i, 4] = 10;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 73;
        cardName[i] = "スペードのトランプ";
        cardExplain[i] = "<color=green>スペードのトランプ</color>\nコスト：緑20黒20　　　　<b><color=black>Ｃ</color></b>\nAT0/DF1\n＜常在能力＞自身のライブラリの枚数が20枚未満ならば、その一番上に「速読」のカードを置く。\n\n<i>手際の悪いトランプの庭師。</i>";
        cardCost[i, 3] = 20;
        cardCost[i, 4] = 20;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 74;
        cardName[i] = "ダイヤのトランプ";
        cardExplain[i] = "<color=green>ダイヤのトランプ</color>\nコスト：緑30青30　　　　<b><color=black>Ｃ</color></b>\nAT0/DF1\n＜常在能力＞対戦相手のライブラリを上から１枚捨てる。\n\n<i>あくどいトランプの官僚。</i>";
        cardCost[i, 2] = 30;
        cardCost[i, 3] = 30;
        cardSkill[i, 0] = SUMMON;
        followerStatus[i, 0] = 0;
        followerStatus[i, 1] = 1;
        followerStatus[i, 2] = i;

        i = 75;
        cardName[i] = "不思議な小瓶";
        cardExplain[i] = "<color=olive>不思議な小瓶</color>\nコスト：黒4　　　　<b><color=black>Ｃ</color></b>\n自身のシュジンコウのATとDFを-1する。\n\n<i>時には小さいことが役に立つ。</i>";
        cardCost[i, 4] = 4;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 0] = -1;
        followerStatus[i, 1] = -1;

        i = 76;
        cardName[i] = "不思議なケーキ";
        cardExplain[i] = "<color=olive>不思議なケーキ</color>\nコスト：黄10　　　　<b><color=black>Ｃ</color></b>\n自身のシュジンコウのATとDFを+1する。\n\n<i>大きいことはいいことだ。</i>";
        cardCost[i, 4] = 10;
        cardSkill[i, 1] = OWN;
        followerStatus[i, 0] = 1;
        followerStatus[i, 1] = 1;

        i = 77;
        cardName[i] = "コショウ";
        cardExplain[i] = "<color=olive>コショウ</color>\nコスト：黒10　　　　<b><color=black>Ｃ</color></b>\n対戦相手のシュジンコウのATとDFを-1する。\n\n<i>香辛料とハサミは使いよう。</i>";
        cardCost[i, 4] = 10;
        cardSkill[i, 1] = YOURS;
        followerStatus[i, 0] = -1;
        followerStatus[i, 1] = -1;



    }

    //★第二種呪文効果関数★
    public void Card15Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        if (usePlayer == 0) { Slip(1, 1); }
        if (usePlayer == 1) { Slip(0, 1); }
    }
    
    public void Card16Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        if (usePlayer == 0) { Slip(1, 8); }
        if (usePlayer == 1) { Slip(0, 8); }
    }
    
    public void Card19Skill2(int usePlayer)
    {
        //呪文効果はなし。（手札回転用の呪文）
    }

    public void Card22Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[1].PlayOneShot(p1.se[1]);
        StartCoroutine(p1.Damage(usePlayer, 5)); StartCoroutine(p1.LifeDamage(usePlayer));//自身に5点ダメージ
    }

    public void Card25Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        if (usePlayer == 0) { Slip(1, 100); }
        if (usePlayer == 1) { Slip(0, 100); }
    }
    
    public void Card28Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        StartCoroutine(p1.SummonCheck(usePlayer, 2)); //第二種呪文タイミングで召喚。
    }

    public void Card29Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        StartCoroutine(p1.LibraryMake(usePlayer));//ライブラリの再構築
    }

    public void Card33Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[1].PlayOneShot(p1.se[1]);
        if (usePlayer == 0) { StartCoroutine(p1.Damage(1, 1)); StartCoroutine(p1.LifeDamage(1)); }
        if (usePlayer == 1) { StartCoroutine(p1.Damage(0, 1)); StartCoroutine(p1.LifeDamage(0)); }
    }

    public void Card36Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[1].PlayOneShot(p1.se[1]);
        if (usePlayer == 0) { StartCoroutine(p1.Damage(1, 1)); StartCoroutine(p1.LifeDamage(1)); }
        if (usePlayer == 1) { StartCoroutine(p1.Damage(0, 1)); StartCoroutine(p1.LifeDamage(0)); }
    }

    public void Card38Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[1].PlayOneShot(p1.se[13]);
        p1.lifePoint[usePlayer] += 5;
    }

    public void Card39Skill2(int usePlayer)
    {//互いのシュジンコウDFを-5
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[6].PlayOneShot(p1.se[6]);
        if (p1.handFollower[0] > 0 && p1.handFollower[1] > 0)
        {
            p1.followerStatus[0, 1] -= 5;
            p1.followerStatus[1, 1] -= 5;
        }
        else
        {//シュジンコウがいなければ
            StartCoroutine(p1.SpellMiss(usePlayer));
        }
    }

    public void Card45Skill2(int usePlayer)
    {//自身のシュジンコウDFを-10すると10点回復
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        if (p1.handFollower[usePlayer] > 0)
        {
            p1.seAudioSource[6].PlayOneShot(p1.se[6]);
            p1.seAudioSource[13].PlayOneShot(p1.se[13]);
            p1.followerStatus[usePlayer, 1] -= 10;
            p1.lifePoint[usePlayer] += 10;
        }
        else
        {//シュジンコウがいなければ
            StartCoroutine(p1.SpellMiss(usePlayer));
        }
    }

    public void Card46Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        p1.phaseSkipFlag[4] = true;
    }
    public void Card47Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        p1.phaseSkipFlag[2] = true;
    }
    public void Card48Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        p1.phaseSkipFlag[1] = true;
    }
    public void Card49Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        p1.phaseSkipFlag[3] = true;
    }
    public void Card50Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        p1.phaseSkipFlag[0] = true;
    }

    public void Card51Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        //相手の使用しない手札を全廃棄。
        if (usePlayer == 0) { Discard(1); }
        if (usePlayer == 1) { Discard(0); }
    }

    public void Card52Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        //相手のライブラリを３枚破壊
        if (usePlayer == 0) { LibraryBreak(1,3); }
        if (usePlayer == 1) { LibraryBreak(0,3); }
    }

    public void Card53Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[1].PlayOneShot(p1.se[1]);
        //(10-ライブラリの残り枚数)点のダメージ
        if (p1.libraryNum[usePlayer] < 10)
        {
            if (usePlayer == 0) { StartCoroutine(p1.Damage(1, 10 - p1.libraryNum[usePlayer])); StartCoroutine(p1.LifeDamage(1)); }
            if (usePlayer == 1) { StartCoroutine(p1.Damage(0, 10 - p1.libraryNum[usePlayer])); StartCoroutine(p1.LifeDamage(0)); }
        }
    }

    public void Card54Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        //お互いに使用しない手札を全廃棄。
        Discard(0);Discard(1);
    }

    public void Card59Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        //相手のライブラリを１枚破壊
        if (usePlayer == 0) { LibraryBreak(1, 1); }
        if (usePlayer == 1) { LibraryBreak(0, 1); }
    }

    public void Card60Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        //自身のライブラリを３枚破壊
        LibraryBreak(usePlayer, 3);
    }

    public void Card61Skill2(int usePlayer)
    {
        int bufferFollower;
        int[] bufferFollowerStatus=new int[3];
        int bufferFollowerForDraw;

        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[14].PlayOneShot(p1.se[14]);
        //お互いにシュジンコウを持っていれば交換
        if (p1.handFollower[0] != 0 && p1.handFollower[1] != 0)
        {
            //自分のを相手へ
            bufferFollower = p1.handFollower[1];
            p1.handFollower[1] = p1.handFollower[0];
            for (int i = 0; i < 3; i++)
            {
                bufferFollowerStatus[i]=p1.followerStatus[1, i];
                p1.followerStatus[1, i] = p1.followerStatus[0, i];
            }
            //相手の（バッファに逃がしておいたもの）を自分へ
            p1.handFollower[0] = bufferFollower;
            for (int i = 0; i < 3; i++)
            {
                p1.followerStatus[0, i] = bufferFollowerStatus[i];
            }
        }
    }

    public void Card62Skill2(int usePlayer)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        p1.seAudioSource[1].PlayOneShot(p1.se[1]);
        //お互い10点ダメージ
        StartCoroutine(p1.Damage(1, 10)); StartCoroutine(p1.LifeDamage(1));
        StartCoroutine(p1.Damage(0, 10)); StartCoroutine(p1.LifeDamage(0));
    }


    //★第二種呪文効果関数に使用する汎用関数★
    //マナを減少させる効果
    public void Slip(int player,int lostMana)
    {
        int i,j;
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
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
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        for (int i = 0; i < HAND_NUM; i++)
        {
            if (p1.useCard[player, i]==false)
            {
                p1.DrawCard(player, i);
                for (int j = 1; j < BLOCKTYPE_NUM + 1; j++) { p1.cardMana[player, i, j] = 0; }//カードに貯まったマナもリセット
                p1.useCard[player, i] = false;//新たに引いたので、使っていない状態になおす。
                //カードを使用していないので非表示を表示にする処理はいらない。
                StartCoroutine(p1.ShakeCard(player,i));
            }
        }
    }

    //ライブラリを破壊する効果（残り０枚なら効果なし）
    public void LibraryBreak(int player, int count)
    {
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
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
    public void HandEscape(int player,PuzzleSceneManager p1)
    {
        for (int i = 0; i < HAND_NUM; i++)
        {
            if (p1.useCard[player, i] == false)
            {
                LibraryPut(player,p1.handCard[player,i],p1);
                p1.useCard[player, i] = true;
                for (int j = 0; j < SKILL_TYPE; j++) { p1.cardSkill[player, i, j] = 0; }//非表示のカードは効果を発揮しない。
                p1.objCard[player, i].GetComponent<Image>().enabled = false;//戻したので非表示
            }
        }
    }

    //ライブラリの一番上にカードを置く
    public void LibraryPut(int player, int card,PuzzleSceneManager p1)
    {
        int i;
        if (p1.library[player, 0, 0, 0] == 0)//ライブラリが一杯でない時のみ
        {
            p1.libraryNum[player]++;
            CardList();
            p1.library[player, 0, 0, 0] = card;
            //今あるカードの上に持って行く
            for (i = DECKCARD_NUM-1; i >0; i--)
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
                p1.library[player, i, 1, j] = cardCost[p1.library[player, i, 0, 0], j];
            }//カードのコスト
            for (int j = 0; j < SKILL_TYPE; j++)
            {
                p1.library[player, i, 2, j] = cardSkill[p1.library[player, i, 0, 0], j];
            }//カードの効果
        }
    }


    //★シュジンコウの特殊能力関数★
    public void FollowerSkill(int player)
    {
        string name="";
        string explain = "";
        PuzzleSceneManager p1 = GetComponent<PuzzleSceneManager>();
        if (p1.followerStatus[player, 2] == 6)
        {
            name = "＜キャシー＞不屈の心";
            explain = "自身は各色１点のマナを獲得する。";
            Slip(player, -1);
        }
        if (p1.followerStatus[player, 2] == 13)
        {
            name = "＜コフィン＞不幸体質";
            explain = "自身は各色５点のマナを失う。";
            Slip(player, 5);
        }
        if (p1.followerStatus[player, 2] == 14)
        {
            name = "＜二首＞余計な頭";
            explain = "自身のライブラリを上から１枚捨てる。";
            LibraryBreak(player, 1);
        }
        if (p1.followerStatus[player, 2] == 63)
        {
            name = "＜チェシャ猫＞神出鬼没";
            explain = "このターン使用されないカードをライブラリに戻す。";
            HandEscape(player,p1);
        }
        if (p1.followerStatus[player, 2] == 64)
        {
            name = "＜アリス＞空想癖";
            explain = "お互いのライブラリをリセットする。（手札や場の状況には影響しない）";
            StartCoroutine(p1.LibraryMake(0));
            StartCoroutine(p1.LibraryMake(1));
        }
        if (p1.followerStatus[player, 2] == 65)
        {
            name = "＜ハートの女王＞癇癪持ち";
            explain = "このシュジンコウのATを+1する。";
            p1.followerStatus[player,0]++;
        }
        if (p1.followerStatus[player, 2] == 66)
        {
            name = "＜三月ウサギ＞鋼鉄メンタル";
            explain = "このシュジンコウのDFを+1する。";
            p1.followerStatus[player, 1]++;
        }
        if (p1.followerStatus[player, 2] == 67)
        {
            name = "＜白ウサギ＞生真面目";
            explain = "このシュジンコウのDFを-1する。";
            p1.followerStatus[player, 1]--;
        }
        if (p1.followerStatus[player, 2] == 68)
        {
            name = "＜公爵夫人＞アゴ";
            explain = "対戦相手のシュジンコウのDFを-1する。";
            if (player == 0) { p1.followerStatus[1, 1]--;}
            if (player == 1) { p1.followerStatus[0, 1]--;}
        }
        if (p1.followerStatus[player, 2] == 69)
        {
            name = "＜眠りネズミ＞怠惰";
            explain = "対戦相手のシュジンコウのATを-1する。";
            if (player == 0) { p1.followerStatus[1, 0]--; }
            if (player == 1) { p1.followerStatus[0, 0]--; }
        }
        if (p1.followerStatus[player, 2] == 70)
        {
            name = "＜代用ウミガメ＞悲しみ";
            explain = "対戦相手に１点のダメージを与える。";
            p1.seAudioSource[1].PlayOneShot(p1.se[1]);
            if (player == 0) { StartCoroutine(p1.Damage(1, 1)); StartCoroutine(p1.LifeDamage(1)); }
            if (player == 1) { StartCoroutine(p1.Damage(0, 1)); StartCoroutine(p1.LifeDamage(0)); }
        }
        if (p1.followerStatus[player, 2] == 71)
        {
            name = "＜クラブ＞数の暴力";
            explain = "対戦相手に１点のダメージを与える。";
            p1.seAudioSource[1].PlayOneShot(p1.se[1]);
            if (player == 0) { StartCoroutine(p1.Damage(1, 1)); StartCoroutine(p1.LifeDamage(1)); }
            if (player == 1) { StartCoroutine(p1.Damage(0, 1)); StartCoroutine(p1.LifeDamage(0)); }
        }
        if (p1.followerStatus[player, 2] == 72)
        {
            name = "＜ハート＞盗み食い";
            explain = "自身は１点のライフを回復する。";
            p1.lifePoint[player]++;
        }
        if (p1.followerStatus[player, 2] == 73)
        {
            name = "＜スペード＞庭いじり";
            explain = "自身のライブラリの枚数が20枚未満ならば、その一番上に「速読」のカードを置く。";
            LibraryPut(player, 19, p1);
        }
        if (p1.followerStatus[player, 2] == 74)
        {
            name = "＜ダイヤ＞横領";
            explain = "対戦相手のライブラリを上から１枚捨てる。";
            if (player == 0) { LibraryBreak(1, 1); }
            if (player == 1) { LibraryBreak(0, 1); }
        }

        StartCoroutine(p1.FollowerSkillCutIn(player,name,explain));
    }


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}











}
