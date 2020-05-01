using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Scenario : MonoBehaviour {

    const int MAX_CHARACTER = 5;                                            //画面に表示するキャラクターの最大人数
    const int DECKCARD_NUM = 20;                                            //デッキのカード枚数
    const int BLOCKTYPE_NUM = 5;                                            //ブロックの色の種類数
    const int GETCARD_NUM = 3;                                              //1パックで獲得できるカードの数
    const int GETCARD_FROM_NUM = 1000;                                      //1パックでランダムチョイスされる母数
    const int CHARACTER0_X = -400;                                          //キャラクター０オブジェクトのＸ座標
    const int CHARACTER1_X = 400;                                           //キャラクター１オブジェクトのＸ座標
    const int CHARACTER_Y = 160;                                            //キャラクターのＹ座標（番号に関わらず一定）
    const int CHARACTER_NUM = 100;                                          //キャラクター立ち絵の数
    const int BACK_IMAGE_NUM = 20;                                          //背景の数
    const int ITEM_NUM = 10;                                                //アイテムの数
    const int SCENARIO_NUM = 3;                                             //シナリオの数

    private int packNum;                                              //選択したパック番号
    private int coin;                                                 //現在の所持コイン数
    private int coinForDraw;                                          //表示用の所持コイン数（増加エフェクト用）
    private int scenarioCount;                                        //現在のシナリオ位置
    private int maxScenarioCount;                                     //現在のシナリオ進行度
    private int timeCount;                                            //シーンが始まってからの時間
    private string nextScene;                                         //次にジャンプするシーン名
    private bool shichoFlag;                                          //シチョウ（キャラクター）が登場しているか
    public string getCardText;                                        //獲得カードテキスト
    public int[] getCard = new int[GETCARD_NUM];                      //カード獲得で得られる3枚
    private int[] layerDepth=new int[3];                                 //

    private List<int> mainStory = new List<int>();                    //パックから黒いカードを入手したフラグ(0が未入手、1が入手したばかり（イベント未読）、2が入手かつイベント既読)

    public GameObject objText;                                               //シナリオテキスト(カード獲得時のpushがあるのでpublic)
    private GameObject objSkipButton;                                        //スキップボタンオブジェクト
    private GameObject objFilm;                                              //回想演出のオブジェクト
    private GameObject objCoin;                                              //コイン表示のオブジェクト
    private GameObject objBookHolder;                                        //本（パック）選択ボタンのオブジェクト
    private GameObject objNowLoading;                                        //ナウローディング表示
    private GameObject objItem;                                              //シナリオ中のアイテム表示
    private GameObject objTextImage;                                         //テキスト表示欄の画像
    private GameObject objBackText;                                          //背景に文字を直書きする際のテキスト
    private GameObject[] objCharacter = new GameObject[MAX_CHARACTER];       //キャラクター表示
    private GameObject objCanvas;                                            //キャンバス
    private GameObject[] objGetCard = new GameObject[GETCARD_NUM + 1];       //カード獲得演出用オブジェクト([GETCARD_NUM]が親オブジェクト)
    public GameObject[] objBG = new GameObject[3];

    private List<AudioClip> bgm=new List<AudioClip>();                       //BGMのオーディオクリップ
    private List<Sprite> characterImage = new List<Sprite>();                //キャラクター立ち絵
    private List<Sprite> backImage = new List<Sprite>();                     //背景
    private List<Sprite> itemImage = new List<Sprite>();                     //アイテム

    // Use this for initialization
    void Start() {
        int i;
        //オブジェクトの読み込み
        objText = GameObject.Find("text").gameObject as GameObject;
        objTextImage = GameObject.Find("textback").gameObject as GameObject;
        objBackText = GameObject.Find("backtext").gameObject as GameObject;
        objCanvas = GameObject.Find("canvas0").gameObject as GameObject;
        objNowLoading = GameObject.Find("NowLoading").gameObject as GameObject;
        objFilm = GameObject.Find("film").gameObject as GameObject;
        objSkipButton= GameObject.Find("SkipButton").gameObject as GameObject;
        objNowLoading.GetComponent<Image>().enabled = true;
        for (i = 0; i < MAX_CHARACTER; i++)
        {
            objCharacter[i] = GameObject.Find("character" + i.ToString()).gameObject as GameObject;
        }

        objGetCard[GETCARD_NUM] = GameObject.Find("packopen").gameObject as GameObject;
        for (i = 0; i < GETCARD_NUM; i++)
        {
            objGetCard[i] = GameObject.Find("card" + i.ToString()).gameObject as GameObject;
        }
        objGetCard[GETCARD_NUM].gameObject.SetActive(false);//カード獲得画面は普段のシナリオ中では見えない

        objItem= GameObject.Find("item").gameObject as GameObject;

        //キャラクター画像の読み込み
        for (i = 0; i < CHARACTER_NUM + 1; i++)
        {
            characterImage.Add(Resources.Load<Sprite>("character" + i.ToString()));
        }
        //アイテム画像の読み込み
        for (i = 0; i < ITEM_NUM + 1; i++)
        {
            itemImage.Add(Resources.Load<Sprite>("item" + i.ToString()));
        }

        //シナリオ進行状況の読み込み
        for (i = 0; i < SCENARIO_NUM + 1; i++)
        {
            mainStory.Add(PlayerPrefs.GetInt("mainStory" + i.ToString(), 0));//メインストーリーのどれを見たかのロード
        }
        scenarioCount = PlayerPrefs.GetInt("scenarioCount", 0);//これからプレイするシナリオ番号のロード
        maxScenarioCount = PlayerPrefs.GetInt("maxScenarioCount", 0);//シナリオをどこまで進めたかのロード
        //音響設定
        GetComponent<Utility>().SEAdd(gameObject);//効果音用のオーディオソースはシーン間引継ぎの必要がないのでシーンマネージャーに追加。
        coin=PlayerPrefs.GetInt("coin", 0);
        coinForDraw = coin;
        objCoin= GameObject.Find("ButtonCoin").gameObject as GameObject;
        objCoin.GetComponentInChildren<Text>().text ="Coin:" + coin.ToString();
        objCoin.SetActive(false);
        objBookHolder = GameObject.Find("BookHolder").gameObject as GameObject;
        objBookHolder.SetActive(false);
        //シナリオコルーチンの実行
        StartCoroutine("ScenarioText" + scenarioCount.ToString());
    }
    

    // Update is called once per frame
    void Update() {
        timeCount++;
        if (coinForDraw<coin && timeCount%3==0) { coinForDraw++; objCoin.GetComponentInChildren<Text>().text = "Coin:" + coinForDraw.ToString(); }
        if (coinForDraw > coin && timeCount % 3 == 0) { coinForDraw--; objCoin.GetComponentInChildren<Text>().text = "Coin:" + coinForDraw.ToString(); }
    }

    //シナリオ本編コルーチン(導入)
    private IEnumerator ScenarioText0()
    {
        Utility u1=GetComponent<Utility>();
        //システム処理
        //スキップ可能になる(yield returnを返す)前にシステム上の処理を終える
        scenarioCount++;
        PlayerPrefs.SetInt("scenarioCount", scenarioCount);//シナリオ進行度のセーブ
        PlayerPrefs.SetInt("mainStory5", 2);
        if (scenarioCount > maxScenarioCount)
        {
            maxScenarioCount = scenarioCount;
            PlayerPrefs.SetInt("maxScenarioCount", maxScenarioCount);
        }//今までで一番シナリオが進んでいたら進行度(maxScenarioCount)も上げる。
        nextScene = "SelectScene";//次に行くシーン名

        //シナリオ部
        //BGM読み込みと再生
        
        bgm.Add(Resources.Load<AudioClip>("ありふれたしあわせ"));
        bgm.Add(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n――真っ白。どこまでも真っ白だ。", 0,false, 0,false,0));
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n　　　　　　　何か忘れてる気がする。", 0,false, 0,false,0));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n\n\n\n\n\n\n多分、大事なこと。　　　　　　　", 0,false,0,false,0));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n\n\n\n\nどうしてここは、こんなに白いんだっけ？", 0,false, 0,false,0));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n\n\n――考えていた私を、突然の衝撃が襲った――", 0,false, 0,false,0));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("followerbreak"));
        StartCoroutine("ShakeScreen");
        yield return new WaitForSeconds(2.0f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("pera"));
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　終わりのない物語</color></b></i>",0,false,0,false,0));
        yield return new WaitForSeconds(0.01f);
        u1.BGMPlay(bgm[1]);
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nあーあー、また布団から転がり落ちて。天井が白いのは当たり前でしょう。いい加減に起きなさい。", 2, "", 0,false, 21,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nはァ？　人がシリアスに導入始めようとした途端にこれだよ。主人に対する遠慮ってないの？", 2, "", 14,true, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n必要ありません。", 2, "", 14,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n言い切るのさすがだよね。すごいと思う。", 2, "", 16,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nお褒めの言葉ありがとうございます。", 2, "", 16,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n褒めてないから。", 2, "", 17,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n存じています。", 2, "", 17,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……もっとこうさ『ヒロインの秘められた過去』とか『襲い来る悪の手先』とか、そういう派手なのはないの？", 2, "", 15,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n残念ながら、そのようなシリアスな設定は無くなってしまったのですよ、これが。", 2, "", 15,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n物語の世界を巡って壊れたお話を修正する、だっけ。実際のところ、やるのはこの部屋で本を読むことだけだよね？　スケール小さくない？", 2, "", 12,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nええ。ですが、これはそういうお話ですので。", 2, "", 12,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nうへぇ。", 2, "", 17,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());

        //シーン選択パートへ
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    //シナリオ本編コルーチン(1章)
    private IEnumerator ScenarioText1()
    {
        int i;
        Utility u1 = GetComponent<Utility>();
        //システム処理
        //スキップ可能になる(yield returnを返す)前にシステム上の処理を終える      
        //敵のデッキを作成。
        CardData c1 = GetComponent<CardData>();
        c1.deckCard[1, 0] = 1;
        c1.deckCard[1, 1] = 1;
        c1.deckCard[1, 2] = 1;
        c1.deckCard[1, 3] = 2;
        c1.deckCard[1, 4] = 2;
        c1.deckCard[1, 5] = 3;
        c1.deckCard[1, 6] = 3;
        c1.deckCard[1, 7] = 3;
        c1.deckCard[1, 8] = 4;
        c1.deckCard[1, 9] = 4;
        c1.deckCard[1, 10] = 5;
        c1.deckCard[1, 11] = 5;
        c1.deckCard[1, 12] = 5;
        c1.deckCard[1, 13] = 7;
        c1.deckCard[1, 14] = 7;
        c1.deckCard[1, 15] = 7;
        c1.deckCard[1, 16] = 8;
        c1.deckCard[1, 17] = 8;
        c1.deckCard[1, 18] = 8;
        c1.deckCard[1, 19] = 9;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            PlayerPrefs.SetInt("enemyDeckCard" + i.ToString(), c1.deckCard[1, i]);//敵デッキのセーブ
        }
        //マナ獲得能力を設定。
        c1.enemyGetManaPace[1] = 1235;
        c1.enemyGetManaPace[2] = 1238;
        c1.enemyGetManaPace[3] = 1236;
        c1.enemyGetManaPace[4] = 1218;
        c1.enemyGetManaPace[5] = 1186;
        for (i = 1; i < BLOCKTYPE_NUM + 1; i++)
        {
            PlayerPrefs.SetInt("enemyGetManaPace" + i.ToString(), c1.enemyGetManaPace[i]);//敵マナ獲得ペースのセーブ
        }
        nextScene = "PuzzleScene";//次に行くシーン名

        //シナリオ部      
        bgm.Add(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        bgm.Add(Resources.Load<AudioClip>("一気に進もう"));
        objNowLoading.GetComponent<Image>().enabled = false;
        layerDepth[0] = 5;
        layerDepth[1] = 2;
        layerDepth[2] = 1;
        objBG[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("back11");        //背景の読み込み
        objBG[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("back2");        //背景の読み込み
        objBG[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("back3");        //背景の読み込み
        StartCoroutine(BGMove(500,0,300));
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nさて。『桃太郎』の物語は覚えていらっしゃいますでしょうか、お嬢様？", 2, "", 11,true, 21,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nねえ、もしかして馬鹿にしてる？　馬鹿にしてるよね。知ってるよ桃太郎くらい。", 2, "", 15,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nこれは驚いた。", 2, "", 15,false, 24,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n――これ、私怒っていいよね？", 2, "", 14,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそこに直れユアン。", 2, "", 13,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nおっと、それじゃあお嬢様。さっそく鬼退治の旅へ行ってらっしゃいませ！", 2, "", 13,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 1, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.01f);
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあっ、くそ、帰ったら覚え――", 2, "", 14,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("summon"));
        yield return StartCoroutine(ScenarioDraw("", 1, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 2, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.1f);
        u1.BGMStop();
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>桃太郎</color></b></i>",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\nむかしむかし、あるところに\nお爺さんとお婆さんがおりました。\nお爺さんは山へ柴刈りに、お婆さんは川へ洗濯に。\nお婆さんが川で洗濯をしていると、川上から大きな桃が。\nお婆さんは桃を拾って帰り、\nお爺さんと共に食べようとすると\n桃の中からは元気な男の子が。\n桃から生まれたその子は\n『桃太郎』と名付けられすくすくと育ち\n逞しく育った桃太郎は\n鬼を退治すべく旅に出るのでした――",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.BGMPlay(bgm[0]);
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nさて、上手く物語の中に入れたようですね。", 3, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nねえ、今気付いたんだけど、これ役割分担おかしくない？", 3, "", 12,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nなんで執事が安全地帯（元の世界）で留守番してて、主人が汗水垂らさなきゃいけないわけ？", 3, "", 15,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n留守番とは失敬な。アドバイザーと呼んでください。こうして、影から助言する役割なんですから。", 3, "", 15,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n文句言うのも馬鹿馬鹿しくなってきたよ。", 3, "", 17,false,0,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそれは良かった。", 3, "", 17,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n良くないから。", 3, "", 14,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n存じています。", 3, "", 14,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nあの……お嬢さん。さっきから一人で何を喋っておるのですか。", 3, "", 14,false, 31,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあ、桃太郎。", 3, "", 11,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nはぁ、桃太郎ですが。", 3, "", 11,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nまだこんなところでうろうろしてんの？　さっさとお供集めて鬼退治に行きなよ。", 3, "", 12,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nそれが、誰もおらんのですよ。猫の子一匹。", 3, "", 12,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nは？　犬猿雉は？", 3, "", 15,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nお嬢様、『物語が壊れている』のかと。いくつかの登場人物が物語から排除されているようです。", 3, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nこういう時は、他の物語のページを使って――", 3, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n面倒くさいな。残ってる人を使えばいいでしょ。", 3, "", 15,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nは？", 3, "", 15,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【犬？（お爺さん）】\nわおーん。", 3, "", 0,false, 32,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【猿？（お婆さん）】\nしくしく、うっきー。", 3, "", 0,false, 33,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n連れて来たよ。", 3, "", 16,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n人手の足りない素人演劇みたいな真似をしない！　お婆さんなんて泣いてるじゃないですか。", 3, "", 16,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n今回は手元の『長靴をはいた猫』の物語でパッチワークしましょう。これでどうにかなるはずです。", 3, "", 17,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nちぇー。いいアイデアだと思ったのに。どうせエンディングまでもう出番がない駄キャラなんだし、少しくらいお役に立てって話よね。", 3, "", 15,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【猿？（お婆さん）】\n鬼じゃ、この娘こそが鬼じゃ！", 3, "", 0,false, 34,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n仕方ないなぁ。とりあえず、コレを使えばいいんでしょ？　『長靴をはいた猫』！", 3, "", 18,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "",0,false,0,false,0));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("summon"));
        
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("【長靴をはいた猫】\nごしゅじん。およびですか？　私は亡き父君が遺された一番の宝物。どんな用事でも、必ずお役に立ってみせましょう。", 3, "", 18,false, 91,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nわあ、出た。可愛い！　それに主人への接し方がよく分かってるわ。<size=64>誰かと違って！</size>", 3, "", 16,false, 91,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nお嬢様って、意外と根に持ちますよね。", 3, "", 16,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nとりあえず、長靴をはいた猫さん。あなたは桃太郎のお伴をして鬼退治に行ってくれる？", 3, "", 16,false, 91,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【長靴をはいた猫】\nおやすいごようです。", 3, "", 16,false, 91,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n\n移動中...",0,false,0,false,0));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("walk"));
        
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(ScenarioDraw("【長靴をはいた猫】\nさて、桃太郎さん。鬼は事前に平らげておきましたので、刀を持ってそこでポーズしてください。", 5, "", 91,true, 31,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nこ、こうかのう？", 5, "", 91,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【長靴をはいた猫】\nはい、チーズ。", 5, "", 91,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("camera"));
        
        yield return StartCoroutine(ScenarioDraw("", 0, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("【長靴をはいた猫】\nさて。後はこの写真を王様に送りつけましょう。桃太郎さんは褒美とお姫様を頂けるはずです。これにてめでたしめでたし。", 5, "", 91,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nひどいヤラセの現場を目撃してしまった……。", 5, "", 17,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n『長靴をはいた猫』は正にそんな物語ですからね。", 5, "", 17,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n知ってて唆したの？", 5, "", 14,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n勿論です。", 5, "", 14,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nなんだか不正に手を貸してしまった気分だよ。", 5, "", 17,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n物語は元の筋に戻ったのですから問題ありませんよ。", 5, "", 17, false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("slip"));
        
        yield return StartCoroutine(ScenarioDraw("【？？？】\n……。", 5, "", 17,false, 81,true,0));
        yield return StartCoroutine(u1.PushWait());
        u1.BGMStop();
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあれ、桃太郎にこんな登場人物いたっけ？", 5, "", 12,false, 81,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nお嬢様、退いてください！", 5, "", 12,false,0,false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("bomb"));
        
        yield return StartCoroutine(ScenarioDraw("", 0, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.1f);
        u1.BGMPlay(bgm[1]);
        GameObject.Find("BGMManager").GetComponent<BGMManager>().bgmChange(false, 1);//パズルシーンもBGMはそのまま
        StartCoroutine("ShakeScreen");
        yield return StartCoroutine(ScenarioDraw("【ネク】\nひえぇ、岩が。", 5, "", 19,true, 81,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nこれは早めに葬儀屋を確保しておかないと。できる執事は常に一歩先を見据えて動くのです。", 5, "", 19,false, 81,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n言ってる場合か！　アドバイザーなんでしょ、なんとかしてよ。", 5, "", 19,false, 81,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそれでは戦い方のアドバイスをば。\n<color=red>※ゲームのルール説明を行います。</color>", 5, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n要は落ちものパズルです。同じ色のブロックを４つくっつけると消えます。どこかで見たルールですね。", 6, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n消えたブロックはその色の魔力（マナ）として獲得できるので、魔力を貯めて呪文を撃ちましょう。連鎖(chain)すると、獲得魔力にボーナスが入ります。", 8, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n呪文カードの下にあるバーと数字が、その呪文に必要な残り魔力です。呪文を使うと、新たな呪文がライブラリから補充されます。", 7, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n自分のＬＰかライブラリが尽きたら負けです。その前に相手のＬＰやライブラリを０にしましょう。", 7, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nまた、キャラクターが書かれた「シュジンコウ」召喚呪文は一度使うと破壊されるまで毎ターン自動で攻撃してくれる優秀な攻撃要員です。彼らを上手く使うことが戦略の鍵になるでしょう。", 7, "", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nま、詳しいことはやりながら何となく覚えるか<size=64>タイトル画面から参照できる説明書(Guide)を見てください</size>。", 7, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nおお、メタいメタい……。", 5, "", 15,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nああ、そうだ。物語のシュジンコウは一人と相場が決まっています。一人で２キャラクター以上のシュジンコウを使役しようとすると詠唱失敗するのでお気をつけて。", 5, "", 11,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        //ゲームパートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    //シナリオ本編コルーチン(1章エンディング)
    private IEnumerator ScenarioText2()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        scenarioCount++;
        PlayerPrefs.SetInt("scenarioCount", scenarioCount);//scenarioCountを１足してセーブ
        nextScene = "StoryScene";//次に行くシーン名（自己呼び出し）※scenarioCountが増えているので、次はscenarioCount==3（パック開封）が実行される。

        bgm.Add(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("【ネク】\nやるじゃん私。", 5, "", 16,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n……。", 5, "", 16,false, 81,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "",0,false,0,false,0));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("slip"));
        
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあっ逃げた。", 5, "", 14,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n物語は無事に元に戻ったんですし、あまり深追いしてはいけませんよ。", 5, "", 14,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nぐぬぬ。", 5, "", 14,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\nこうして桃太郎は宝を持ち帰り、\nお姫様に見初められて幸せに暮らしましたとさ。\n\nめでたしめでたし。",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nふゥ、はァ、ふゥふゥ。うーん、動けェ……！", 3, "", 0,false, 31,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nちょっと待ってください、置いてかないで。……これだけの財宝、重くて動かすのが大変なんです。", 3, "", 0,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n（本来３匹と１人で運ぶものだからなぁ）", 3, "", 17,true, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nふゥはァ、やっと――やっと家が見えた。", 3, "", 17,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそういえば、あんなに役に立つ助っ人を紹介したんだし。お礼っていうものがあってもいいよね？", 3, "", 16,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nもちろんですとも。", 3, "", 16,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nお宝の一割を――。", 3, "", 16,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n…………。", 3, "", 15,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nさ、三割でどうにか――。", 3, "", 15,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nうーん、そうだなぁ。……はい。", 3, "", 12,false, 31,false,1));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【桃太郎】\nこの古ぼけた首飾りが何か……？", 3, "", 12,false, 31,false,1));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそれがあなたの取り分。残りは貰って行くわね。", 3, "", 16,false, 31,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【猿？（お婆さん）】\nやっぱり鬼じゃ、この娘！", 3, "", 0,false, 34,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("<i><color=red>Scenario Clear!</color></i>\n『桃太郎』を１パック獲得しました。（ブースターパックは周回ごとに何回でも獲得できます）", 3, "",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        //ストーリーシーンへ（自己呼び出し）
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);

        yield return null;
    }

    //シナリオ本編コルーチン(1章パック開封)
    private IEnumerator ScenarioText3()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        if (scenarioCount > maxScenarioCount)
        {
            maxScenarioCount = scenarioCount;
            PlayerPrefs.SetInt("maxScenarioCount", maxScenarioCount);
        }//今までで一番シナリオが進んでいたら進行度(maxScenarioCount)も上げる。
        nextScene = "SelectScene";//次に行くシーン名
        //シナリオ部
        yield return StartCoroutine("GetPack", 1);//パック１（桃太郎）を獲得。
        if (mainStory[1] == 1){ yield return StartCoroutine(ScenarioText10003()); }//特定カードを手に入れると回想シーンへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    /// <summary>
    /// 1章回想シーン
    /// </summary>
    private IEnumerator ScenarioText10003()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        nextScene = "SelectScene";//次に行くシーン名
        PlayerPrefs.SetInt("mainStory1", 2);
        objNowLoading.GetComponent<Image>().enabled = true;
        bgm.Add(Resources.Load<AudioClip>("ありふれたしあわせ"));
        bgm.Add(Resources.Load<AudioClip>("危険な香り"));
        bgm.Add(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあれ？　なんだろう、この呪文（ページ）……？ 　これだけ雰囲気が違う。", 2, "", 11, true, 0, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 2, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        objFilm.GetComponent<SpriteRenderer>().enabled = true;
        objFilm.GetComponent<Animator>().enabled = true;
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("pera"));
        
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第１章　　　絵本と執事　　　-K1.11.13</color></b></i>", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n――こうして、粉挽きの息子は猫のおかげですっかり幸せになりましたとさ。めでたしめでたし。", 2, "", 22, true, 61, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nねえ、ユアン。つぎのごほんをよんで！", 2, "", 22, false, 62, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nもう８時です。良い子は寝る時間ですよ。", 2, "", 21, false, 62, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nえーっ。", 2, "", 21, false, 63, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……仕方ありませんね。あと１冊だけですよ。ただし、ベッドに入って大人しく聞くこと。", 2, "", 22, false, 63, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nわーい！", 2, "", 22, false, 62, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("off"));
        
        DrawColor(0.3f, 0.3f, 0.3f);
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそれでは、お話の始まりです。「幸福な王子」――", 2, "", 21, false, 61, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("（ああ、これ昔の私だ。そうそう、ユアンも昔は割と優しかったんだよね。）", 2, "", 21, false, 61, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそして、だんだん王子の像はみすぼらしく――", 2, "", 21, false, 64, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……すーすー", 2, "", 21, false, 64, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n眠ってしまいましたか。", 2, "", 22, false, 64, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nおやすみなさい、良い夢を。", 2, "", 22, false, 64, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("bed"));
        
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 2, "", 22, false, 64, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n――神様。僕にはこの歪な国がいつまでも続くとは思えないのです。ああ、関帝よ。願わくばこの子に幸せな未来を与えてあげてください。", 2, "", 21, false, 64, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……むにゃ、ユアンいる？", 2, "", 21, false, 64, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nええ、いますよ。安心してください、あなたを置いてはどこにも行きませんから。", 2, "", 22, false, 64, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nむにゃ、えへへ……。", 2, "", 22, false, 65, false, 0));
        yield return StartCoroutine(u1.PushWait());
        objFilm.GetComponent<SpriteRenderer>().enabled = false;
        objFilm.GetComponent<Animator>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 2, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        DrawColor(1.0f, 1.0f, 1.0f);
        yield return StartCoroutine(ScenarioDraw("【ネク】\nどうしてこんなことを思い出したんだろ？　シナリオには関係ないのに。", 2, "", 20, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n…………。", 2, "", 20, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n――あれ、『シナリオ』って何？　どうして私はここにいるんだっけ？", 2, "", 20, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n私は――", 2, "", 20, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("summon"));
        
        u1.BGMPlay(bgm[1]);
        StartCoroutine(ShichoMove(2));
        yield return StartCoroutine(ScenarioDraw("【？？？】\n待った。そこまでだ。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nあちゃー、面倒なことをしてくれたね。物語のページに記憶を潜ませたか。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n物語を修復させるほどに、おまえたちは設定を取り戻していくと。こんな悪知恵が働くのは恐らくアイツだな。この様子だとアイツ自身もどこまで記憶を残してるやら。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nいいよ。持って行くがいいさ、その記憶。どうせ知ったところで<color=red>結末は一つも変わらない</color>。それに働いてもらわないと私も困る。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nま、そもそもさ。アタシは悪役でもなんでもないんだよ、むしろおかしいのはあっちだし。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nそれに、気付いてないかもしれないけど、おまえたちにとってもこれが最善なんだよ？", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nだからさ、あんまり深く考えずに頑張ってくれると嬉しいなって。", 2, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n何を言って――", 2, "", 12, true, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nそれじゃ、「ゲームクリア」の時にまた会おうね！　ちゃんと来てくれるのを待ってるよ。", 2, "", 12, false, 52, false, 0));
        yield return StartCoroutine(u1.PushWait());
        shichoFlag = false;
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 2, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        u1.BGMPlay(bgm[2]);
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……目が覚めましたか。さて、それでは次の物語を修正するとしましょう。", 2, "", 12, true, 22, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nねえ、ユアン。変な夢を見たよ。変な女の子の夢。", 2, "", 11, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……もしや、その子は黒髪のショートカットで執事への当たりがキツかったりしませんか。", 2, "", 11, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nよし、ユアン。そこに正座！", 2, "", 15, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nほら、当たりがキツい！", 2, "", 13, false, 23, false, 0));
        yield return StartCoroutine(u1.PushWait());

        //シーン選択パートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);

        yield return null;
    }



    private IEnumerator EndRoll()
    {
        Utility u1 = GetComponent<Utility>();
        yield return StartCoroutine(ScenarioDraw("", 14, "", 0, false, 0, false, 0));//背景以外のオブジェクトを全て消し、背景を空に。
        objBG[0].GetComponent<RectTransform>().sizeDelta = new Vector2(1280,4000);
        for (int i = 0; i <= 1500; i++)
        {
            objBG[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 1500 - i * 2);
            if (i == 200)
            {
                StartCoroutine(ScenarioDraw("", 14, "", 0, false, 0, false, 8));
                objItem.GetComponent<Image>().enabled = true;
                objItem.GetComponent<RectTransform>().sizeDelta = new Vector2(615, 375);
            }
            objItem.GetComponent<RectTransform>().localPosition = new Vector2(-300, i * 2 - 1500);
            objItem.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, (float)(i - 200) / 100);
            yield return null;

            objBackText.GetComponent<Text>().fontSize = 36;
            objBackText.GetComponent<RectTransform>().localPosition = new Vector2(300, i * 2 - 1500);
            if (i == 300) { StartCoroutine(ScenarioDraw("", 14, "\n\n\n\nScenario:#6", 0, false, 0, false, 8)); }
            if (i == 480) { StartCoroutine(ScenarioDraw("", 14, "\n\n\n\n\n\nProgram:#6", 0, false, 0, false, 8)); }
            if(i==660) { StartCoroutine(ScenarioDraw("", 14, "\n\n\n\n\n\n\n\nMainCharacters-Illustration:#6", 0, false, 0, false, 8)); }
            if(i==840) { StartCoroutine(ScenarioDraw("", 14, "\n\n\n[SpecialThanks]\n\nIllustration-materials:イラストAC\n 　　                       いらすとや\nPhoto-materials:写真AC\nMusic-materials:ハヤシユウ\nSE-materials:効果音ラボ", 0, false, 0, false, 8)); }
            if(i==1020) { StartCoroutine(ScenarioDraw("", 14, "\n\n\n\n[参考文献]\n\n満州集団自決　著：新海均\n14歳＜フォーティーン＞　著：澤地久枝\n満州文化物語　著：喜多由浩", 0, false, 0, false, 8)); }
            if(i==1200) { StartCoroutine(ScenarioDraw("", 14, "\n\n\n\n[制作]\n\nBrainmixer", 0, false, 0, false, 8)); }
            if(i==1380) { StartCoroutine(ScenarioDraw("", 14, "\n\n\n\n\n\nThank you for playing.", 0, false, 0, false, 8)); }
            objBackText.GetComponent<Text>().color = new Color(0.15f, 0.15f, 0.15f, (float)((i - 300)%180) / 50);
        }
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("gun1"));    
        yield return new WaitForSeconds(1.5f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return new WaitForSeconds(1.5f);
    }

    //エンディング報酬(おまけパック獲得（エンディングだけをSkipで飛ばせるように、パック獲得を隔離）)
    private IEnumerator ScenarioText20002()
    {
        Utility u1 = GetComponent<Utility>();
        nextScene = "Title";//次に行くシーン名
        objNowLoading.GetComponent<Image>().enabled = false;
        PlayerPrefs.SetInt("mainStory6", 2);
        //シナリオ部
        yield return StartCoroutine(ScenarioDraw("<i><color=red>Congratulations!</color></i>\n『UQNecRomance』を１パック獲得しました。（ブースターパックは周回ごとに何回でも獲得できます）", 0, "", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine("GetPack", 4);//パック４（おまけパック）を獲得。
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    //
    private IEnumerator ScenarioText100000()
    {
        objCoin.SetActive(true);
        objBookHolder.SetActive(true);
        objSkipButton.GetComponentInChildren<Text>().text="Back";
        Utility u1 = GetComponent<Utility>();
        //システム処理
        nextScene = "SelectScene"; //次に行くシーン名  
        bgm.Add(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        StartCoroutine(ShichoMove(2));
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n――ここでは、コインをパックと交換してあげるよ。\nOver（3枚以上）になったカードはコインに変換されるから、パズルが苦手でも、地道にやれば高難度ステージのカードが手に入るね。", 2, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.SelectWait());
        //パックを選択させる。
        yield return StartCoroutine("GetPack", packNum);//選択したパックを獲得。
        //シーン選択パートへ
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
        yield return null;
    }

    public void PushPack(int num)
    {
        Utility u1 = GetComponent<Utility>();
        if (num == 1)
        {
            if (coin>=10)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 10;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", 2, "", 0, false, 51, false, 0));
            }
        }
        if (num == 2)
        {
            if (coin >= 10)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 10;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", 2, "", 0, false, 51, false, 0));
            }
        }
        if (num == 3)
        {
            if (coin >= 10)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 10;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", 2, "", 0, false, 51, false, 0));
            }
        }
        if (num == 4)
        {
            if (coin >= 20)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 20;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", 2, "", 0, false, 51, false, 0));
            }
        }
        if (num == 5)
        {
            if (coin >= 20)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 20;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", 2, "", 0, false, 51, false, 0));
            }
        }
    }

    //シナリオ画面の色を変える（暗いシーン等に使用）
    private void DrawColor(float red,float green,float blue)
    {
        int i;
        for (i = 0; i < MAX_CHARACTER; i++)
        {
            objCharacter[i].GetComponent<Image>().color = new Color(red, green, blue);
        }
        for (i = 0; i < 3; i++)
        {
            objBG[i].GetComponent<Image>().color = new Color(red, green, blue);
        }
        objItem.GetComponent<Image>().color = new Color(red, green, blue);
    }

    //描画内容関数。text：シナリオテキスト、back:背景の種類、backtext:背景に直書きする文字、character1,character2:キャラクター画像の番号
    private IEnumerator ScenarioDraw(string text, int back, string backtext, int character1, bool inFlag1, int character2, bool inFlag2, int item)
    {
        int i;
        objText.GetComponent<Text>().text = "";//テキストの初期化

        if (text == "")//テキストの背景だけ先に出す。（本文はキャラクター移動が終わった後で）
        {
            objTextImage.GetComponent<Image>().enabled = false;
        }
        else
        {
            objTextImage.GetComponent<Image>().enabled = true;
        }
        if (backtext == "")//背景テキスト
        {
            objBackText.GetComponent<Text>().enabled = false;
        }
        else
        {
            objBackText.GetComponent<Text>().enabled = true;
            objBackText.GetComponent<Text>().text = backtext;
        }
        if (character1 == 0)//キャラクター１
        {
            objCharacter[0].GetComponent<Image>().enabled = false;
        }
        else
        {
            objCharacter[0].GetComponent<Image>().enabled = true;
            objCharacter[0].GetComponent<Image>().sprite = characterImage[character1];
        }
        if (character2 == 0)//キャラクター２
        {
            objCharacter[1].GetComponent<Image>().enabled = false;
        }
        else
        {
            objCharacter[1].GetComponent<Image>().enabled = true;
            objCharacter[1].GetComponent<Image>().sprite = characterImage[character2];
        }
        if (item == 0)//アイテム
        {
            objItem.GetComponent<Image>().enabled = false;
        }
        else
        {
            objItem.GetComponent<Image>().enabled = true;
            objItem.GetComponent<Image>().sprite = itemImage[item];
        }
        for (i = 0; i < 5; i++)//キャラクター移動
        {
            if (inFlag1 == true)
            {
                objCharacter[0].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER0_X + i * 20, CHARACTER_Y, 0);
            }
            if (inFlag2 == true)
            {
                objCharacter[1].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER1_X - i * 20, CHARACTER_Y, 0);
            }
            if (inFlag1 == true || inFlag2 == true) { yield return null; }//キャラクターの移動があるならyield return で処理を返す
        }
        if (text == "")//テキストの表示はキャラクターの移動が終わるまで待つ。（このコルーチンはyield returnで呼ぶので、他コルーチンと前後関係が狂う心配はない）
        {
            objText.GetComponent<Text>().enabled = false;
        }
        else
        {
            objText.GetComponent<Text>().enabled = true;
            objText.GetComponent<Text>().text = text;
        }
    }

    //シチョウが会話イベント中も動くコルーチン
    private IEnumerator ShichoMove(int character)
    {
        shichoFlag = true;
        //シチョウ（characterImage[50]~[59]）はイラストが大きい＋動くので専用処理
        if (character==1)
        {
            objCharacter[0].GetComponent<RectTransform>().sizeDelta = new Vector2(600, 800);
        }
        if (character==2)
        {
            objCharacter[1].GetComponent<RectTransform>().sizeDelta = new Vector3(600, 800);
        }
        while (shichoFlag==true)
        {
            if (character==1)
            {
                objCharacter[0].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER0_X, CHARACTER_Y - 60 - 30 * (Mathf.Cos((float)timeCount / 10)), 1);
            }
            if (character==2)
            {
                objCharacter[1].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER1_X, CHARACTER_Y - 60 - 30 * (Mathf.Cos((float)timeCount / 10)), 1);
            }
            yield return null;
        }
        //シチョウを表示したオブジェクトを元に戻す
        if (character==1)
        {
            objCharacter[0].GetComponent<RectTransform>().sizeDelta = new Vector2(400, 500);
            objCharacter[0].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER0_X, CHARACTER_Y, 1);
        }
        if (character==2)
        {
            objCharacter[1].GetComponent<RectTransform>().sizeDelta = new Vector2(400, 500);
            objCharacter[1].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER1_X, CHARACTER_Y, 1);
        }
    }

    //キャラクターの小ジャンプ
    private IEnumerator CharacterJump(int character)
    {
        if (character == 1)
        {
            for (int i = 0; i < 7; i++)
            {
                objCharacter[0].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER0_X, CHARACTER_Y+i*2, 1);
                yield return null;
            }
            for (int i = 7; i > 0; i--)
            {
                objCharacter[0].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER0_X, CHARACTER_Y + i*2, 1);
                yield return null;
            }
            objCharacter[0].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER0_X, CHARACTER_Y, 1);
        }
        if (character == 2)
        {
            for (int i = 0; i < 7; i++)
            {
                objCharacter[1].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER1_X, CHARACTER_Y + i*2, 1);
                yield return null;
            }
            for (int i = 7; i > 0; i--)
            {
                objCharacter[1].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER1_X, CHARACTER_Y + i*2, 1);
                yield return null;
            }
            objCharacter[1].GetComponent<RectTransform>().localPosition = new Vector3(CHARACTER1_X, CHARACTER_Y, 1);
        }
    }


    //画面振動する関数
    private IEnumerator ShakeScreen()
    {
        int i;
        for (i = 0; i < 30; i++)
        {
            objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0 - 5 + 10 * (i % 2));
            yield return null;
        }
        objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
    }

    //パック獲得関数
    private IEnumerator GetPack(int packNum)
    {
        Utility u1 = GetComponent<Utility>();
        const int COMMON = 100;//Cは10%(100/1000)
        const int UNCOMMON = 50;//UCは5%(50/1000)
        const int RARE = 10;//Rは1%(10/1000)
        int i, j;
        int choice;
        int[] packCard = new int[GETCARD_FROM_NUM];//ランダムチョイスの母集団のカード番号配列
        int[] packCardRarity = new int[GETCARD_FROM_NUM];//ランダムチョイスの母集団のレアリティ配列
        int[] getCardRarity = new int[GETCARD_NUM];//得られる3枚のレアリティ
        Sprite[] cardImage = new Sprite[GETCARD_FROM_NUM];
        bgm.Add(Resources.Load<AudioClip>("わくわくショッピング"));
        objCoin.SetActive(true);
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        objSkipButton.SetActive(false);
        yield return StartCoroutine(ScenarioDraw("獲得ページ", 2, "",0,false,0,false,0));
        objGetCard[GETCARD_NUM].gameObject.SetActive(true);
        CardData c1 = GetComponent<CardData>();
        c1.CardList();//カードリストのロード
        c1.LoadHaveCard(0);//カード所持状況のロード
        yield return StartCoroutine(u1.PushWait());
        for (i = 0; i < GETCARD_NUM; i++)
        {
            objGetCard[i].GetComponent<Image>().raycastTarget = true;//表向きカードは説明表示あり
        }
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("up"));
        
        //パック内容
        if (packNum == 1)//パック１（桃太郎）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < COMMON + j; i++) { packCard[i] = 1; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 2; packCardRarity[i] = COMMON; }//C
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 3; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < COMMON + j; i++) { packCard[i] = 5; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 7; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 8; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 9; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 15; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 16; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 21; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < RARE + j; i++) { packCard[i] = 22; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 23; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 24; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 25; packCardRarity[i] = RARE; }//R 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 26; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 27; packCardRarity[i] = UNCOMMON; }//UC                       
            for (j = i; i < RARE + j; i++) { packCard[i] = 28; packCardRarity[i] = RARE; }//R     
        }

        if (packNum == 2)//パック２（赤ずきん）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < COMMON + j; i++) { packCard[i] = 32; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 34; packCardRarity[i] = COMMON; }//C
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 30; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < COMMON + j; i++) { packCard[i] = 36; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 37; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 38; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 31; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 39; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 33; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 40; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < RARE + j; i++) { packCard[i] = 29; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 41; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 42; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 44; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 35; packCardRarity[i] = RARE; }//R
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 43; packCardRarity[i] = UNCOMMON; }//UC                       
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 45; packCardRarity[i] = UNCOMMON; }//UC     
        }
        if (packNum == 3)//パック３（幸福な王子）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < COMMON + j; i++) { packCard[i] = 54; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 55; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 56; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 57; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 58; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 59; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 60; packCardRarity[i] = COMMON; }//C
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 46; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 47; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 48; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 49; packCardRarity[i] = UNCOMMON; }//UC                       
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 50; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < RARE + j; i++) { packCard[i] = 51; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 52; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 53; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 61; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 62; packCardRarity[i] = RARE; }//R
        }
        if (packNum == 4)//パック４（エンディングおまけ）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 46; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 47; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 48; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 49; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 35; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 33; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 30; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 31; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 26; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 27; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 3; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 9; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 11; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 12; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 50; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 6; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 10; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 14; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < RARE + j; i++) { packCard[i] = 4; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 13; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 17; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 18; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 20; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 19; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 53; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 61; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 51; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 52; packCardRarity[i] = RARE; }//R
        }
        if (packNum == 5)//パック５（アリス）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < COMMON + j; i++) { packCard[i] = 71; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 72; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 73; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 74; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 75; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 76; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 77; packCardRarity[i] = COMMON; }//C
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 10; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 66; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 68; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 69; packCardRarity[i] = UNCOMMON; }//UC                       
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 70; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < RARE + j; i++) { packCard[i] = 63; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 64; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 65; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 29; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 67; packCardRarity[i] = RARE; }//R
        }

        getCardText = "";//獲得カードテキストの初期化
        for (i = 0; i < GETCARD_NUM; i++)
        {
            choice = Random.Range(0, GETCARD_FROM_NUM);
            getCard[i] = packCard[choice];//獲得するカードをランダムで決定
            cardImage[i] = Resources.Load<Sprite>("card" + getCard[i].ToString());//獲得したカードの画像をロード
            objGetCard[i].GetComponent<Image>().sprite = cardImage[i];//カード画像を表示
            getCardText += c1.cardName[getCard[i]];//カード名を表示
            //空白を入れて幅を合わせる
            for (j = 0; j < 10 - c1.cardName[getCard[i]].Length; j++) { getCardText += "　"; }
            //カードのレアリティを表示
            if (packCardRarity[choice] == COMMON) { getCardText += "<b><color=black>Ｃ　</color></b>";getCardRarity[i] = COMMON; }
            if (packCardRarity[choice] == UNCOMMON) { getCardText += "<b><color=#a06000ff>ＵＣ</color></b>";getCardRarity[i] = UNCOMMON; }
            if (packCardRarity[choice] == RARE) { getCardText += "<b><color=#ff5000ff>Ｒ　</color></b>";getCardRarity[i] = RARE; }
            //新しいカードならNew!表示、既に3枚持っているならOver表示
            if (c1.haveCard[getCard[i]] == 0) { getCardText += "　<color=red>New!</color>"; }
            if (c1.haveCard[getCard[i]] >= 3) { getCardText += "　<color=blue>Over...</color>"; }
            if (c1.haveCard[getCard[i]] < 3)
            {
                c1.haveCard[getCard[i]]++;
            }//獲得したカードについて手持ちが3枚以下なら所持カードに追加。
            else
            {
                if (packCardRarity[choice] == RARE) { coin += 10; getCardText += "→<color=olive>+10 Coin</color>"; }
                if (packCardRarity[choice] == UNCOMMON) { coin += 2; getCardText += "→<color=olive>+2 Coin</color>"; }
                if (packCardRarity[choice] == COMMON) { coin++; getCardText += "→<color=olive>+1 Coin</color>"; }

            }//手持ちが3枚以上ならコインに変換。
            getCardText += "\n";
            PlayerPrefs.SetInt("coin", coin);
            PlayerPrefs.SetInt("haveCard" + getCard[i].ToString(), c1.haveCard[getCard[i]]);//セーブ
            if (getCard[i] == 15 && mainStory[1] == 0) { mainStory[1] = 1; PlayerPrefs.SetInt("mainStory1", 1); }//黒のカードを引いたらメインストーリーイベントのフラグをたてる。
            if (getCard[i] == 45 && mainStory[2] == 0) { mainStory[2] = 1; PlayerPrefs.SetInt("mainStory2", 1); }//黒のカードを引いたらメインストーリーイベントのフラグをたてる。
            if (getCard[i] == 62 && mainStory[3] == 0) { mainStory[3] = 1; PlayerPrefs.SetInt("mainStory3", 1); }//黒のカードを引いたらメインストーリーイベントのフラグをたてる。
            PlayerPrefs.Save();//カード取得のデータを確実に残すためセーブデータを書き込み。
        }
        StartCoroutine(getRareCard(getCardRarity));//レアカード判定に使う配列getCardRarityを引数の形でコルーチンに渡す
        objText.GetComponent<Text>().text=getCardText;//テキストをオブジェクトに表示
        yield return StartCoroutine(u1.PushWait());
        objGetCard[GETCARD_NUM].gameObject.SetActive(false);
        coinForDraw = coin;
        objCoin.SetActive(false);
        objSkipButton.SetActive(true);
        bgm.RemoveAt(0);//カード獲得画面のbgmは消しておく。（そのまま裏シナリオに入る場合があるのでややこしくなるのを避ける）
        yield return null;
    }

    //レアカードの点滅演出
    private IEnumerator getRareCard(int[] getCardRarity)
    {
        const int RARE = 10;//Rは1%(10/1000)
        int i;
        while (true)
        {
            for (i = 0; i < GETCARD_NUM; i++)
            {
                if (getCardRarity[i] == RARE)
                {
                    objGetCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.7f - 0.2f * (Mathf.Cos((float)timeCount / 10)));
                }
            }
            yield return null;
        }
    }

    private void SkipButton()
    {
        Utility u1 = GetComponent<Utility>();
        if (u1.pushObjectFlag == false)//押している間SkipButtonはずっと呼ばれるので、フラグが立った後はコルーチンを呼ばないようにする。（Onclickは押す→離すまでの一連の処理をトリガーとするので、押下のタイミングでは反応を返さない。故にGetCardPushやWaitPushとイベント判定タイミングを『押下時』で統一するためonclickは使わない。判定タイミングが異なるとWaitPushのpushObjectFlag判定が正しく行えない）
        {
            objFilm.GetComponent<SpriteRenderer>().enabled = false;
            objFilm.GetComponent<Animator>().enabled = false;
            //次のパートへ

            objSkipButton.GetComponent<Animator>().enabled = true;
            objSkipButton.GetComponent<Animator>().Play("Pressed",0,0);
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().b1.bgmChangeFlag == false) { u1.BGMPlay(bgm[GameObject.Find("BGMManager").GetComponent<BGMManager>().b1.bgmNum]); };
            u1.StartCoroutine("LoadSceneCoroutine", nextScene);
        }
        u1.pushObjectFlag = true;
    }

    private IEnumerator BGMove(int XMove,int YMove=0,int time=300)
    {
        for (int j=0;j<time;j++)
        {
            for (int i = 0; i < 3; i++)
            {
                objBG[i].GetComponent<RectTransform>().localPosition = new Vector2((float)XMove / layerDepth[i]/ time + objBG[i].GetComponent<RectTransform>().localPosition.x, (float)YMove / layerDepth[i]/time + objBG[i].GetComponent<RectTransform>().localPosition.y);
            }
            yield return null;
        }
    }
















}




