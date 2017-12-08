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

    private int scenarioCount;                                        //現在のシナリオ位置
    private int maxScenarioCount;                                     //現在のシナリオ進行度
    private int timeCount;                                            //シーンが始まってからの時間
    private string nextScene;                                         //次にジャンプするシーン名
    private bool shichoFlag;                                          //シチョウ（キャラクター）が登場しているか
    public string getCardText;                                        //獲得カードテキスト
    public int[] getCard = new int[GETCARD_NUM];                      //カード獲得で得られる3枚

    private List<int> mainStory = new List<int>();                    //パックから黒いカードを入手したフラグ(0が未入手、1が入手したばかり（イベント未読）、2が入手かつイベント既読)

    public GameObject objText;                                               //シナリオテキスト(カード獲得時のpushがあるのでpublic)
    private GameObject objSkipButton;                                        //スキップボタンオブジェクト
    private GameObject objFilm;                                              //回想演出のオブジェクト
    private GameObject objNowLoading;                                        //ナウローディング表示
    private GameObject objItem;                                              //シナリオ中のアイテム表示
    private GameObject objTextImage;                                         //テキスト表示欄の画像
    private GameObject objBackImage;                                         //背景
    private GameObject objBackText;                                          //背景に文字を直書きする際のテキスト
    private GameObject[] objCharacter = new GameObject[MAX_CHARACTER];       //キャラクター表示
    private GameObject objCanvas;                                            //キャンバス
    private GameObject[] objGetCard = new GameObject[GETCARD_NUM + 1];       //カード獲得演出用オブジェクト([GETCARD_NUM]が親オブジェクト)

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
        objBackImage = GameObject.Find("backimage").gameObject as GameObject;
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
        //背景の読み込み
        for (i = 0; i < BACK_IMAGE_NUM + 1; i++)
        {
            backImage.Add(Resources.Load<Sprite>("back" + i.ToString()));
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

        //シナリオコルーチンの実行
        StartCoroutine("ScenarioText" + scenarioCount.ToString());
    }
    

    // Update is called once per frame
    void Update() {
        timeCount++;
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

    //シナリオ本編コルーチン(2章)
    private IEnumerator ScenarioText4()
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
        c1.deckCard[1, 5] = 2;
        c1.deckCard[1, 6] = 11;
        c1.deckCard[1, 7] = 11;
        c1.deckCard[1, 8] = 23;
        c1.deckCard[1, 9] = 23;
        c1.deckCard[1, 10] = 9;
        c1.deckCard[1, 11] = 9;
        c1.deckCard[1, 12] = 9;
        c1.deckCard[1, 13] = 18;
        c1.deckCard[1, 14] = 15;
        c1.deckCard[1, 15] = 15;
        c1.deckCard[1, 16] = 15;
        c1.deckCard[1, 17] = 20;
        c1.deckCard[1, 18] = 20;
        c1.deckCard[1, 19] = 20;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            PlayerPrefs.SetInt("enemyDeckCard" + i.ToString(), c1.deckCard[1, i]);//敵デッキのセーブ
        }
        //マナ獲得能力を設定。
        c1.enemyGetManaPace[1] = 735;
        c1.enemyGetManaPace[2] = 738;
        c1.enemyGetManaPace[3] = 736;
        c1.enemyGetManaPace[4] = 718;
        c1.enemyGetManaPace[5] = 786;
        for (i = 1; i < BLOCKTYPE_NUM + 1; i++)
        {
            PlayerPrefs.SetInt("enemyGetManaPace" + i.ToString(), c1.enemyGetManaPace[i]);//敵マナ獲得ペースのセーブ
        }
        nextScene = "PuzzleScene";//次に行くシーン名

        //シナリオ部
        bgm.Add(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        bgm.Add(Resources.Load<AudioClip>("地下炭鉱"));
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nさて、次は『赤ずきん』の物語――なのですが。", 2, "", 16,true, 21,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n――なのですが？", 2, "", 11,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n『シュジンコウ』の赤ずきん自身が消えてしまった結果、物語が始まってすらいないようです。", 2, "", 11,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそのため、代役の『シュジンコウ』を立てる必要があるのですが――", 2, "", 11,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n猫はナシね。", 2, "", 15, false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……ですよね。何の物語でパッチワークにしましょうか。", 2, "", 15,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……", 2, "", 20,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n…………！　思いついた。私に任せて！", 2, "", 16,false, 21,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nおや、今回はやる気じゃないですか。それでは今回も物語の世界へ――", 2, "", 16,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nレッツゴー！", 2, "", 16,false, 22,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("summon"));
        
        yield return StartCoroutine(ScenarioDraw("", 1, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 2, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.1f);
        u1.BGMStop();
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>赤ずきん</color></b></i>",0,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.BGMPlay(bgm[0]);
        yield return StartCoroutine(ScenarioDraw("――むかしむかし、あるところに赤ずきんちゃんというそれは可愛い侠客がおりました。", 3, "", 0,false, 94,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……一体、何を混ぜたんですかね。", 3, "", 0,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nね、ねぇ？　ユアン。声のトーン低くない？", 3, "", 16,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 3, "", 16,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nガチギレだよ……。", 3, "", 17,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n何を混ぜました？", 3, "", 17,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n任侠映画『仁義の華』。", 3, "", 16,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nこのお馬鹿ーーーーーーッッッ！！！！", 3, "", 19,false, 0,false,0));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("bomb"));
        
        StartCoroutine("ShakeScreen");
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nどう収拾をつける気ですか。出落ちでどうにかなるほど物語は甘いものじゃないんですよ。", 3, "", 19,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nき、きっと大丈夫だよ。義理（ぎり）と侠気（おとこぎ）があれば何でもできる！", 3, "", 15,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.BGMPlay(bgm[1]);
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n嬢ちゃん、いいこと言うじゃねえか。", 3, "", 15,false, 94,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n赤ずきんちゃん、こんにちは！", 3, "", 16,false, 94,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nどうしても『それ』を赤ずきんと言い張るつもりですか。", 3, "", 16,false,0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n義理と侠気か。そうだな。最近は分かる奴がめっきり少なくなった。", 3, "", 0,false, 94,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nウチの家（ファミリー）も、筋の通らねェ抗争ばかりするようになっちまったよ。", 3, "", 0,false, 94,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n家（ファミリー）……。赤ずきんがベースな分だけ微妙に西洋かぶれしてる。", 3, "", 17,true, 94,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n赤ずきんちゃん、そういえばお婆さんのお見舞いに行くんじゃなかったっけ？", 3, "", 11,false, 94,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nああ、そうだ。御隠居の見舞いに行くようにオヤジから頼まれていたんだっけな。", 3, "", 11,false, 94,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nつまらねェ愚痴を聞かせて悪かったな、嬢ちゃん。", 3, "", 11,false, 94,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("car"));
        
        yield return StartCoroutine(ScenarioDraw("\n（バタン）（ブロォォォォ―――）", 3, "", 0,false, 0,false,3));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n赤ずきんちゃん、外車で去った……。", 3, "", 16,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nあの赤ずきんが狼に襲われるビジョンが浮かびませんね。この話、一体どうやったら正しい筋に戻るのやら……。", 3, "", 0,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("pera"));
        
        yield return StartCoroutine(ScenarioDraw("", 1, "<color=white>\n\n\n\n\n\n\n暗転、そして場面転換...</color>",0,false,0,false,0));
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nコスモス、か。", 9, "", 94,true, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\nおゥ、赤ずきんじゃねェか。どうしたよ、こんなところで黄昏て。", 9, "", 94,false, 95,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nオオカミのオジキ。いやね、俺ァ、コスモスを見ていたんですよ。", 9, "", 94,false, 95,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\n柄じゃねェなぁ。俺達みたいな極道者には似合わねェだろ、花なんて。", 9, "", 94,false, 95,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n花はお嫌いですか。", 9, "", 94,false, 95,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\nいや、嫌いじゃねェ。ただ、見てると無性に寂しくなる。", 9, "", 94,false,95,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n寂しく……？", 9, "", 94,false,95,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\nああ、おめェもいつか、そう思うようになる日が来るかもしれねェ。", 9, "", 94,false,95,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\nま、そんな気持ちは知らない方が幸せだがな。", 9, "", 94,false,95,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\nおっと、お喋りが過ぎたな。俺は用事があるから先に行く。", 9, "", 94,false,95,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\n――また会えるといいな。", 9, "", 94,false,95,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n……？", 9, "", 94,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n赤ずきんちゃん、やっぱり花畑に寄ったんだ。", 9, "", 94,false, 11,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nおっと、また嬢ちゃんか。何、便所休憩に寄っただけだ。別に花を見に来た訳じゃねェ。", 9, "", 94,false, 11,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそうだ、赤ずきんちゃんにも一本あげるよ。", 9, "", 94,false, 16,false,4));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nこれは、コスモスの花……？", 9, "", 94,false, 16,false,4));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nホントは公園の花を勝手に採っちゃいけないんだけどね。私も『ワル』になっちゃった。", 9, "", 94,false,16,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n嬢ちゃん、ありがとな。でも、嬢ちゃんは俺みたいになっちゃいけねェ。平凡で幸せな人生が望めるなら、そうした方がいいんだ。", 9, "", 94,false,16,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n平凡で幸せな人生？　でも、私はもう――", 9, "", 94,false,10,false, 0));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("slip"));
        
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(ScenarioDraw("", 0, "",0,false,0,false,0));
        yield return new WaitForSeconds(0.01f);
        yield return StartCoroutine(ScenarioDraw("【ネク】\n平凡で幸せな人生？　私みたいな美人で賢いお嬢様が、そんな人生に満足する訳ないでしょ。", 9, "", 94,false, 15,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n言うねェ、嬢ちゃん。末は博士か大臣か。", 9, "", 94,false, 16,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nふっふっふ。褒めなさい、褒めなさい。", 9, "", 94,false, 16, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nおっと、嬢ちゃん。俺もそろそろ行かないと御隠居に怒鳴られちまう。ごめんな。", 9, "", 94,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n――御隠居は、昔はイイ人だったんだがよ。歳のせいか、だんだんおかしくなって来てるんだ。", 9, "", 94,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n筋の通らねェ抗争をよその家（ファミリー）に仕掛けるよう焚きつけたり、オヤジやオジキに無茶な注文をおしつけたり。", 9, "", 94,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nそれでも、誰も逆らえねェんだ。御隠居に受けて来た義理があるからよ。今はただ、みんながあの人が死ぬのを待ってる。誰も口には出さずに。", 9, "", 94,false,0,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n誰かが止めてやらなきゃいけねェのかもしれねぇ。だが、それは侠気を捨てて不義理を働くってことだ。", 9, "", 94,false,0,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nだから、できねェ。できねェんだよ。", 9, "", 94,false, 0,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nジレンマだね……。", 9, "", 94,false, 17,true,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n盛り上がっているところ申し訳ないのですが、このお話が『赤ずきん』であることをお忘れなく。", 9, "", 0,false, 11,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nいつ本筋を外れて銃撃戦が始まるのかとヒヤヒヤですよ、こっちは。", 9, "", 0,false,11,false,0 ));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n赤ずきんにもドンパチあるじゃん。", 9, "", 0,false, 15,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n狩人が持ってるのは猟銃！　間違っても十四年式じゃないですから。あとドンパチ言わない、はしたない。", 9, "", 0,false, 15,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n意外と乗ってくれるんだね、ユアン……。", 9, "", 0,false, 12,false,0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……何か？", 9, "", 0,false,12,false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nいーえ、なんにも。", 9, "", 0,false,16,false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("slip"));
        
        yield return StartCoroutine(ScenarioDraw("【？？？】\n……。", 9, "", 81, true, 16, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n――うげ。また出た。", 9, "", 81, false, 17, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n何のつもりで邪魔するのさ、もー。", 9, "", 81, false, 15, false, 0));
        yield return StartCoroutine(u1.PushWait());
        //ゲームパートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    //シナリオ本編コルーチン(2章エンディング)
    private IEnumerator ScenarioText5()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        scenarioCount++;
        PlayerPrefs.SetInt("scenarioCount", scenarioCount);//scenarioCountを１足してセーブ
        nextScene = "StoryScene";//次に行くシーン名（自己呼び出し）※scenarioCountが増えているので、次はscenarioCount==3（パック開封）が実行される。
        bgm.Add(Resources.Load<AudioClip>("地下炭鉱"));
        bgm.Add(Resources.Load<AudioClip>("風のダンジョン"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nなんだかんだ、赤ずきんが花畑に寄ってお婆さんの家までやって来ましたね。物語の自己修正力の底知れなさとでも言うべきか。", 10, "", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("doorchime1"));
        
        yield return StartCoroutine(ScenarioDraw("\n（ピンポーン）", 10, "", 94, true, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n御隠居、お見舞いに来ましたぜ。……うん？", 10, "", 94, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("door"));
        
        yield return StartCoroutine(ScenarioDraw("\n（ガチャガチャ――カタン）", 10, "", 94, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nなんだ、鍵が開いてるじゃねェか。不用心だなァ。", 10, "", 94, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n御隠居、入りますよーっと。", 10, "", 94, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("dash"));
        
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\nおゥ、赤ずきん。こんなところでまた会うとはな。ま、おまえで良かった――か。", 10, "", 94, false, 95, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n何のことです？", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\n何でもねェさ。", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n……", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n……ねェ、オオカミのオジキ。どうしてこんなところに居るんです？", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\n御隠居の見舞いにな。", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n……どうして、そんなに息を切らしてんですかい？", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\nインターホンが鳴ったから、慌てて出て来たんだ。", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nねェ、どうして……部屋の奥から血の匂いがするんですかい？", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\nそれは、御隠居を殺したから――。さあ、これ以上問答が必要か？", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("\n（スッ……）", 10, "", 94, false, 95, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n……ッ！！！", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("\n（パン）（パン）（パン）", 10, "", 94, true, 95, false, 10));
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return new WaitForSeconds(0.25f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return new WaitForSeconds(0.25f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("\n（パン）（パン）（パン）", 10, "", 94, true, 95, false, 10));
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return new WaitForSeconds(0.25f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return new WaitForSeconds(0.25f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("\n（パン）（パン）（カチッ）（カチッ）（カチッ）", 10, "", 94, true, 95, false, 10));
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return new WaitForSeconds(0.25f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【大上（おおかみ）】\n……。", 10, "", 94, false, 95, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("followerbreak"));
        
        yield return StartCoroutine(ScenarioDraw("\n（……ドサッ）", 10, "", 94, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nオジキ……。", 10, "", 94, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("pera"));
        
        yield return StartCoroutine(ScenarioDraw("", 1, "<color=white>\n\n\n\n\n\n\n暗転、そして場面転換...</color>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(1.0f);
        u1.BGMPlay(bgm[1]);
        yield return StartCoroutine(ScenarioDraw("【組員】\nなァ、赤ずきん。コンクリ詰めの死体を海に沈めるだけの仕事に、何もアンタが同行する必要はないんだぜ。", 11, "", 94, false, 96, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nいや、いいんだ。最後まで見届けたい。オオカミのオジキには世話になったからな。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【組員】\n可愛がって貰った恩人を殺すことになったのは同情するが、アンタは間違ったことをした訳じゃねェ。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【組員】\n恩義を忘れて弓を引いちゃァいけねェよ。引退したとはいえ、自分のオヤ（親分）だろ。当然の報いだ。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【組員】\nそれによ。もしかすると――だが。あの人はおまえに引き金を引いて欲しかったんじゃないのか。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【組員】\n自分の中での筋を通すために。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【組員】\nだからよ、まァなんだ。気に病むな。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n柄にもなく心配してくれてんのか。ありがとよ。俺ァ大丈夫だ。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n感傷も思い出も、全部ここに置いていく。そのためにここに来たんだ。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【組員】\n――よし、ここいらでいいか。沈めるぞ。", 11, "", 94, false, 96, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n待て。これを一緒に沈めてやってくれ。", 11, "", 94, true, 96, false, 4));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【組員】\nこれは――？", 11, "", 94, false, 96, false, 4));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\n俺達には似合わない感傷。そして、見ていると寂しくなる花だ。", 11, "", 94, false, 96, false, 4));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【組員】\n……？？？　まぁ、いい。沈めるぞ。", 11, "", 94, false, 96, false, 4));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("splash"));
        
        yield return StartCoroutine(ScenarioDraw("コンクリートに括り付けられた恩人の体が、波間へと沈んでいく。水面にはコスモスの花が一輪。", 11, "", 0, false, 0, false, 4));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("――それは、少し肌寒い秋の日のことだった。", 11, "", 0, false, 0, false, 4));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\nオオカミの体は重い石とともに沈んで、\n二度と浮かんではきませんでした。\n老人を殺したわるいオオカミがいなくなって\nみんなは一安心しましたとさ。\n\nめでたしめでたし。", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【赤ずきん】\nわるいオオカミ、か。汚れ役をみんな引き受けてくれたんだな、オジキはよ……。", 11, "", 94, true, 0, false, 4));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n悪いオオカミが沈められて、めでたしめでたし――。", 11, "", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nにわかには信じがたいことですが、『赤ずきんの筋書き通り』という扱いのようですね。", 11, "", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nどやぁ。", 11, "", 16, true, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n登場人物たちのおかげと言うべきか、運命とでも呼ぶべきものに導かれているとでも言うべきか。", 11, "", 16, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nえ、私の頑張りは無視！？", 11, "", 15, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n――まさか、過程は物語の結末に影響を及ぼさない？", 11, "", 15, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nうわ。スルーしたよ、この人。", 11, "", 17, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("<i><color=red>Scenario Clear!</color></i>\n『赤ずきん』を１パック獲得しました。（ブースターパックは周回ごとに何回でも獲得できます）", 3, "", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        //ストーリーシーンへ（自己呼び出し）
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);

        yield return null;
    }

    //シナリオ本編コルーチン(2章パック開封)
    private IEnumerator ScenarioText6()
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
        yield return StartCoroutine("GetPack", 2);//パック２（赤ずきん）を獲得。
        if (mainStory[2] == 1) { yield return StartCoroutine(ScenarioText10006()); }//特定カードを手に入れると回想シーンへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    /// <summary>
    /// 2章回想シーン
    /// </summary>
    private IEnumerator ScenarioText10006()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        nextScene = "SelectScene";//次に行くシーン名
        PlayerPrefs.SetInt("mainStory2", 2);
        objNowLoading.GetComponent<Image>().enabled = true;
        bgm.Add(Resources.Load<AudioClip>("ありふれたしあわせ"));
        bgm.Add(Resources.Load<AudioClip>("危険な香り"));
        bgm.Add(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("【ネク】\nこの呪文（ページ）は……。", 2, "", 11, true, 0, false, 2));
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
        
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第２章　　 広がる世界  　-K9.4.26</color></b></i>", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n――ただいま戻りました。長い間、暇を頂いてしまいすみませんでした。", 2, "", 25, true, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nもー、別に謝るところじゃないのに。立派に勉学を修めて帰って来たんだから、もっと堂々とすればいいんだよ。――おかえり、ユアン。", 2, "", 25, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n少し見ない間に大きくなりましたね、お嬢様。", 2, "", 22, false, 8, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nでしょ？　もう子供じゃないんだから。ユアンとだって対等だよ。", 2, "", 22, false, 16, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n対等――", 2, "", 24, false, 16, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n対等――、ですか。", 2, "", 25, false, 16, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそうそう。私だって、ユアンが内地に行ってた2年半の間にずいぶん成長したんだからね。", 2, "", 25, false, 16, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそのようですね。喜ばしいことです。\n高女での成績ももちろん素晴らしいのでしょう？", 2, "", 25, false, 16, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nうわ、その話題は勘弁。もう少し楽しい話してよ、内地のみやげ話とかさ。", 2, "", 22, false, 17, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそうですね。例えば内地では夏の夜に山に炎で模様を描くのを見る風習があるのですが、去年は教授が一升瓶を片手にやって来て――", 2, "", 21, false, 8, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("pera"));
        
        yield return StartCoroutine(ScenarioDraw("（三十分後）", 2, "", 21, false, 8, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n――二日酔いの頭でやけに床が固いと思うと、そこは警察の留置所で。教授が身分を説明して何とか帰してもらいました。", 2, "", 21, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあははは！　内地で随分無茶したんだね。よく無事に帰って来たものだよ。", 2, "", 25, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nでも、不思議だな。こうして話しているとユアンは全然そんな馬鹿をやりそうにないのに。落ち着いてて物静かで――", 2, "", 25, false, 8, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nここでは猫を被っていますからね。仕事ですし、友達や恋人に接するのとは違いますよ。", 2, "", 22, false, 8, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n…………。", 2, "", 22, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n仕事――かぁ。", 2, "", 22, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nねえ、ユアン。私にはあなたの本当の姿を見せてはくれないの？", 2, "", 22, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nお嬢様？", 2, "", 21, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n私はさ、ずっとユアンの背中を見て歩いて来たの。ユアンはいつでも私の傍に居てくれたけど、同時に私にとってユアンはひどく「遠かった」。", 2, "", 25, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n傍に居ても、同じ場所に立てない。これってどうしてなんだろう？　私はいつまでも「お嬢様」だし、ユアンはいつまでも「執事」のまま。", 2, "", 25, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n私たちは一体どこに向かっているの？　世界が広がる度に、私たちはどんどん遠くなっていく。", 2, "", 25, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあなたが居なかったこの2年半、私は寂しくてたまらなかった。どんなに友達と一緒にいても。", 2, "", 25, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n私は自分の足であなたの隣を歩きたい。あなたに跪いてもらうのでもなく、あなたの背中を追いかけるのでもなく。", 2, "", 25, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nだから、<color=red>遠慮なんていらない</color>。もっと軽口を叩いてよ。笑いたくない時に笑顔を作らないでよ。", 2, "", 25, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 2, "", 25, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n――お嬢様は贅沢です。何かを望むなら、必要な対価を払わなければならないのですよ。人の心を望むなら尚更に。", 2, "", 21, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそれなら、私はいつかきっと――。", 2, "", 25, false, 7, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n自分勝手だ、あなたは！　こちらの気持ちも知らないで……！", 2, "", 26, false, 7, false, 0));
        yield return StartCoroutine(u1.PushWait());
        StartCoroutine(CharacterJump(2));
        yield return StartCoroutine(ScenarioDraw("【ネク】\n――！！！　（びくっ）", 2, "", 26, false, 6, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………すみません、声を荒げてしまって。", 2, "", 27, false, 6, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nしかし、誰にだって「己の行動を決める権利」というものがあるのです。まだ敬語はやめてあげませんし、笑顔を作るのもやめません。あなたが自分の贅沢と無知に気付く日まではね。", 2, "", 22, false, 6, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n私なんて何も知らないってことは、自分でも痛いほど分かってるよ……。", 2, "", 22, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nですが、あなたの気持ちは少しだけ伝わったので――ま、軽口くらいはいいでしょうかね。", 2, "", 22, false, 9, false, 0));
        yield return StartCoroutine(u1.PushWait());
        objFilm.GetComponent<SpriteRenderer>().enabled = false;
        objFilm.GetComponent<Animator>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 2, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("summon"));
        
        u1.BGMPlay(bgm[1]);
        StartCoroutine(ShichoMove(2));
        DrawColor(1.0f, 1.0f, 1.0f);
        yield return StartCoroutine(ScenarioDraw("【？？？】\n本当に喰えない奴だよ、おまえは。", 2, "", 22, true, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n一体、どこまで覚えてるんだ？", 2, "", 22, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n別に咎めようってんじゃない。おまえたちが「設定」を取り戻そうが、本当はどうでもいいんだ。", 2, "", 22, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nただ、取り戻した後に何があるのか覚えているのかって話だよ。", 2, "", 22, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……覚えていたとしたら？", 2, "", 21, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nあの子は覚えちゃいない。だから、おまえが導いてやらないといけないはずだ。", 2, "", 25, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nおまえがあの子を主人扱いしていないのは分かる。だが、少しくらい情はないのか？　おまえのそこがわからない。", 2, "", 25, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nあんなろくでもない結末にもう一度あの子を叩き落としたいとでも？", 2, "", 25, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n――もしかすると。", 2, "", 25, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nおまえは、本当はあの子を憎んでいるんじゃないか？　憎むに十分な理由があるだろう、おまえたちには。", 2, "", 25, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n否定はできませんね。", 2, "", 22, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n味方のふりをして騙し討ちとは悪い奴だ。", 2, "", 22, false, 51, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n僕はどうにも利己的な人間のようです。", 2, "", 27, false, 51, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nまぁ、アタシには関係のないことだ。物語が正しく終わってくれるなら、それが悲劇だろうが喜劇だろうが構わない。", 2, "", 27, false, 52, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n役割を終えた後に演者が物語を最後まで紡ぐことを望むなら、望み通り正しく終わらせるだけだよ。", 2, "", 27, false, 52, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n駄賃を投げ捨てる奴に、無理やり渡す道理はないからね。", 2, "", 27, false, 51, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n偽りのハッピーエンドが駄賃ですか。", 2, "", 21, false, 51, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nまぁ、本当は終わらないんだけどね。でも終わりまで読まれることがないなら、それは終わりと変わらない。", 2, "", 25, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n永遠に終わりが来なければ『結末の正誤』が問われることもない。アタシの仕事に支障はないってわけ。", 2, "", 25, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n――あなたは一体何者です？", 2, "", 21, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nあれ、それ聞く？　覚えてるものだとばっかり思ってた。", 2, "", 25, false, 50, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nそれじゃ改めて。アタシは『シチョウ』。この世界のシステムを司る者、システムキャラクターだ。つまり――おまえたちの言う「神様」みたいなものかな。", 2, "", 25, false, 51, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n「<color=red>物語は用意された結末しか迎えない</color>」。これが、この物語の唯一にして単純なルールだ。システムであるアタシには、それを実現する役割がある。", 2, "", 25, false, 51, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nおまえたちに動いてもらう理由も、このゲームの目的も、全てはその「ルール」が為なのだよ？", 2, "", 25, false, 51, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n今後ともよろしく。我が優秀な、そして性悪な一時の相方。", 2, "", 25, false, 52, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nええ、よろしくお願いします。物語が終わりを迎えるその時まで。", 2, "", 22, false, 52, false, 2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n――「終わりを迎えるまで」か。まったく、悪い奴だ。", 2, "", 22, false, 51, false, 2));
        yield return StartCoroutine(u1.PushWait());
        shichoFlag = false;
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 2, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        u1.BGMPlay(bgm[2]);
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアン、大丈夫？　なんだかうなされてたけど。", 2, "", 25, false, 12, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nいえ、大丈夫です。何でもありません。", 2, "", 22, false, 12, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nならいいけど。隠すからなぁ、この男は。", 2, "", 22, false, 15, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそうですか？", 2, "", 22, false, 15, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそうだよ。いつも本心は隠しててさ。ニコニコしてるだけで腹の中を明かさない。", 2, "", 22, false, 12, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 2, "", 25, false, 12, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nまぁ、私には全部お見通しだけどね。", 2, "", 25, false, 16, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nほら、胃腸薬。調子悪いなら早く言いなよ。無理することないんだから。", 2, "", 25, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 2, "", 24, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n（ぷっ）", 2, "", 22, false, 14, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nハハハ！　いえ、ありがとうございます。ですが大丈夫ですよ。", 2, "", 22, false, 14, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n今の笑う必要あったかなぁ、ユアン？", 2, "", 22, false, 13, false, 0));
        yield return StartCoroutine(u1.PushWait());

        //シーン選択パートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
        yield return null;
    }

    //シナリオ本編コルーチン(3章)
    private IEnumerator ScenarioText7()
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
        c1.deckCard[1, 5] = 2;
        c1.deckCard[1, 6] = 3;
        c1.deckCard[1, 7] = 23;
        c1.deckCard[1, 8] = 42;
        c1.deckCard[1, 9] = 42;
        c1.deckCard[1, 10] = 44;
        c1.deckCard[1, 11] = 44;
        c1.deckCard[1, 12] = 33;
        c1.deckCard[1, 13] = 33;
        c1.deckCard[1, 14] = 33;
        c1.deckCard[1, 15] = 18;
        c1.deckCard[1, 16] = 18;
        c1.deckCard[1, 17] = 11;
        c1.deckCard[1, 18] = 11;
        c1.deckCard[1, 19] = 11;
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            PlayerPrefs.SetInt("enemyDeckCard" + i.ToString(), c1.deckCard[1, i]);//敵デッキのセーブ
        }
        //マナ獲得能力を設定。
        c1.enemyGetManaPace[1] = 435;
        c1.enemyGetManaPace[2] = 438;
        c1.enemyGetManaPace[3] = 436;
        c1.enemyGetManaPace[4] = 418;
        c1.enemyGetManaPace[5] = 486;
        for (i = 1; i < BLOCKTYPE_NUM + 1; i++)
        {
            PlayerPrefs.SetInt("enemyGetManaPace" + i.ToString(), c1.enemyGetManaPace[i]);//敵マナ獲得ペースのセーブ
        }
        nextScene = "PuzzleScene";//次に行くシーン名

        //シナリオ部
        bgm.Add(Resources.Load<AudioClip>("風のダンジョン"));
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n最後の物語は幸福な王子、ですか。", 2, "", 11, true, 21, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n皮肉なお話だよね。結局死んでしまうのに、「幸福」だなんてさ。", 2, "", 10, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそうでしょうか。", 2, "", 10, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアンはあの結末を幸福だと思った？", 2, "", 15, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nええ。少なくとも、僕が燕なら何度でもあの結末を選びますよ。", 2, "", 15, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nへえ、私なら絶対に甲斐性ナシの王子なんて早い所見切りつけて南国で悠々自適の生活するよ。", 2, "", 10, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n<size=16>嘘ばっかり。</size>", 2, "", 10, false, 27, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあれ、何か言った？", 2, "", 11, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそれでは、今回も行ってらっしゃいませ！", 2, "", 11, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("summon"));
        
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 2, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        u1.BGMStop();
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>幸福な王子</color></b></i>", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\nむかしむかし、とある町に\nとても美しい「幸福な王子の像」がありました。\nとある晩秋の日、エジプトへと渡る途中の一匹の燕が\nこの町にやってきました。\n幸福な王子の像は、自分の下で雨宿りをする燕に\n自分の体についた宝石や金を\n貧しい人達に渡してくれるように頼むのでした――", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.BGMPlay(bgm[0]);
        yield return StartCoroutine(ScenarioDraw("【ネク】\nふわ～。豪華だなぁ。これ一体でいくらするんだろう。", 12, "", 8, true, 0, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n税金の無駄遣いですよね。市会議員がいるのにこんな無駄遣いをしている辺り、どうも民主主義のガワを被った独裁政権の匂いが。", 12, "", 8, false, 0, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n童話の世界で夢のない話はやめよう。", 12, "", 15, false, 0, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n元々世知辛いストーリーですけどね、この話。", 12, "", 15, false, 0, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n時代設定も割と現代に近いし、物語の展開もあんまり「昔話」っぽくないよね。", 12, "", 11, false, 0, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nこの童話は、50年ほど前に「オスカー・ワイルド」という作家によって書かれたものです。", 12, "", 11, false, 0, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n物語の時代も恐らく当時の頃を想定しているのではないでしょうか。", 12, "", 11, false, 0, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nへ～。意外と最近にできた話なんだね。", 12, "", 8, false, 0, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nさっきから一人で何をぶつぶつ言っているんです？", 12, "", 8, false, 97, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあ、燕さん。針子のお母さんにルビーはもう届け終わった？", 12, "", 8, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nお嬢さん、なんであっしがルビーを運んでる事を知ってるんです？", 12, "", 8, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n小さい頃から何度も読んでるし――じゃなくて、たまたまくわえてるのを見たんだよ。うん。", 12, "", 16, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nあー、なるほど。……で、入れない理由なんですがね、窓が閉まっているんですよ。どこもかしこも。", 12, "", 16, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアン、これってもしかして。", 12, "", 11, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nええ、物語が壊れているようです。", 12, "", 11, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nやっぱりかー。", 12, "", 17, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nさっきから誰と話しているんですかい？", 12, "", 17, false, 97, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nい、いや。なんでもないよ。", 12, "", 10, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nでも、今回は解決が簡単そうだ。窓の奥にこのルビーを運べばいいんだよね。", 12, "", 13, false, 97, false, 5));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……待ってください、なんで腕まくりをしているんです？", 12, "", 13, false, 0, false, 5));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nもー、わかってるくせに★", 12, "", 8, false, 0, false, 5));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nやめ、はしたな、というか少しはヒロインとしての自覚を――", 12, "", 8, false, 0, false, 5));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nネク投手、振りかぶって第一球を……", 12, "", 8, false, 0, false, 5));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nこの脳筋ーッ！", 12, "", 8, false, 0, false, 5));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……投げましたー！", 12, "", 13, false, 0, false, 5));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("\n（シュッ…………）（コーーン）", 12, "", 0, false, 81, true, 5));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("roll"));
        
        yield return new WaitForSeconds(1.0f);
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("spellmiss"));
        
        StartCoroutine(CharacterJump(2));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nおおっと、ネク投手暴投だ。いつもの敵役選手デッドボールに激昂、マウンドに詰め寄ります！", 12, "", 17, true, 81, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアンー、馬鹿言ってないで助けてよ。", 12, "", 19, false, 81, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n今回ばかりはお嬢様が一方的に悪いと思いますよ？　自業自得です。", 12, "", 19, false, 81, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nですよねー！", 12, "", 17, false, 81, false, 0));
        yield return StartCoroutine(u1.PushWait());

        //ゲームパートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    //シナリオ本編コルーチン(3章エンディング)
    private IEnumerator ScenarioText8()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        scenarioCount++;
        PlayerPrefs.SetInt("scenarioCount", scenarioCount);//scenarioCountを１足してセーブ
        nextScene = "StoryScene";//次に行くシーン名（自己呼び出し）※scenarioCountが増えているので、次はscenarioCount==9（パック開封）が実行される。
        bgm.Add(Resources.Load<AudioClip>("風のダンジョン"));
        bgm.Add(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあのさ、なんで物語を壊そうとするの？　悪役ってそういうものだから？", 12, "", 11, true, 81, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n……私が悪役？　ああ、そうか。悪いことなんだろうな、これは。", 12, "", 11, false, 81, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nだからこそ私は物語を守っていたし、あの子は私を必死で止めたんだと思う。", 12, "", 11, false, 81, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nは？　全然要領を得ないんだけど。", 12, "", 15, false, 81, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nいずれわかるよ。……そして、今はまだわかっちゃいけない。", 12, "", 15, false, 81, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "", 0, false, 0, false, 0));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("slip"));
        
        yield return new WaitForSeconds(0.1f);
        u1.BGMPlay(bgm[1]);
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあっ。思わせぶりな事だけ言って逃げた。こういうシーンで要点かいつまんで説明してくれるキャラがいた試しないよね。みんな説明能力に問題あると思うんだ。", 12, "", 17, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nキャラクター達の的確な報連相によって危機が迫る前に完璧に対処していく、何事も起きないストーリーが面白いとでも？", 12, "", 17, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそれは確かに。", 12, "", 10, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.BGMPlay(bgm[0]);
        yield return StartCoroutine(ScenarioDraw("【燕】\nあのー、さっきから独り言でお忙しそうなところ恐縮ですが、さっきの騒動を野次馬するためにあちこちの家が窓を開けたようなので、今のうちに届けてしまいますね。", 12, "", 11, false, 97, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nうん、頑張れ。", 12, "", 8, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nはい。", 12, "", 8, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……あのさ。", 12, "", 11, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nなんでしょう？", 12, "", 11, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n南には行かなくていいの？　あんた、このままだと死んじゃうよ。", 12, "", 12, false, 97, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nですよねぇ。でも、まぁ放っておけないんですよ。あのお人好しの王子様。", 12, "", 12, false, 97, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nあの人は、世間知らずで甘いんです。どうせ、この宝石や金も貧民は半年も持たずに使い切ってしまう。金の使い方なんて彼らは知らないんですから。", 12, "", 12, false, 97, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nなら、なんで……。", 12, "", 15, false, 97, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nさあ？　まぁ、だから人様のためって訳じゃないのは確かですよ。無駄だってわかってやってるんだから。どうもあっしはとことん利己的なヤツらしい。", 12, "", 15, false, 97, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【燕】\nさ、お嬢さん。お喋りはここまでです。あっしは自分のやりたいようにやらせてもらいますよ。", 12, "", 15, false, 97, false, 6));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n燕は王子の頼むがままに、\nその体から宝石や金を剥ぎ\n貧しい人々に運びました。\nやがて冬が訪れます。\n燕は死に、みすぼらしくなった王子の像は\nスクラップにされました。\n唯一残った鉛の心臓と燕の死骸は、\n同じゴミ捨て場の隅で転がっていました。\n\nめでたしめでたし。", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nやっぱりわからないよ。どうしてこれが「めでたしめでたし」で、「幸福な」王子なんだろう。", 12, "", 20, true, 0, false, 7));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n正確には、王子と燕の魂は神様の目に止まって天国に連れていかれるんですけどね。", 12, "", 20, false, 0, false, 7));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nどっちにしろ死んでるじゃん。", 12, "", 14, false, 0, false, 7));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……もしかして、この本を置いたのは僕たちへの皮肉ですかね。誰かさん。", 12, "", 14, false, 0, false, 7));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……ん？　誰かさんって？", 12, "", 11, false, 0, false, 7));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nいいえ、なんでも。ただ、どこぞの性悪な神様なら「天国で終わらない物語を紡げるんだ、ハッピーエンドだろう？」なんて言うだろうなと思って。", 12, "", 11, false, 0, false, 7));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nいやに具体的だね。", 12, "", 11, false, 0, false, 7));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nまぁ、冗談みたいなものです。気にしないでください。", 12, "", 11, false, 0, false, 7));
        yield return StartCoroutine(u1.PushWait());
        //ストーリーシーンへ（自己呼び出し）
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);

        yield return null;
    }


    //シナリオ本編コルーチン(3章終了。偽エンディング)
    private IEnumerator ScenarioText9()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        if (scenarioCount > maxScenarioCount)
        {
            maxScenarioCount = scenarioCount;
            PlayerPrefs.SetInt("maxScenarioCount", maxScenarioCount);
        }//今までで一番シナリオが進んでいたら進行度(maxScenarioCount)も上げる。
        PlayerPrefs.SetInt("scenarioCount", scenarioCount+1);//次のシーンは+1してカード獲得画面(ScenarioText10)
        nextScene = "StoryScene";//次に行くシーン名
        //偽エンディング(初回のみ)
        if (mainStory[0] == 0)
        {
            yield return StartCoroutine(ScenarioText10000());
        }
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    //シナリオ本編コルーチン(3章パック獲得（偽エンディングだけをSkipで飛ばせるように、パック獲得を隔離）)
    private IEnumerator ScenarioText10()
    {
        Utility u1 = GetComponent<Utility>();
        nextScene = "SelectScene";//次に行くシーン名
        objNowLoading.GetComponent<Image>().enabled = false;
        //シナリオ部
        yield return StartCoroutine(ScenarioDraw("<i><color=red>Scenario Clear!</color></i>\n『幸福な王子』を１パック獲得しました。（ブースターパックは周回ごとに何回でも獲得できます）", 12, "", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine("GetPack", 3);//パック３（幸福な王子）を獲得。
        if (mainStory[3] == 1) { yield return StartCoroutine(ScenarioText10010()); }//特定カードを手に入れると回想シーンへ
        //selectsceneへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    /// <summary>
    /// 偽エンディング
    /// </summary>
    private IEnumerator ScenarioText10000()
    {
        Utility u1 = GetComponent<Utility>();
        if (scenarioCount == 10000) { nextScene = "SelectScene"; }//名も無い本から見に来たなら、終わったら選択画面に戻る。
        bgm.Add(Resources.Load<AudioClip>("good_noon"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        StartCoroutine(ShichoMove(2));
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nゲームクリアおめでとう！　物語は全て無事に修正されました。お疲れ様～！", 13, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nぱちぱちぱち！", 13, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n無事かどうかは割と怪しいところもあるけどね――。ま、書かれた結末とは齟齬がないから問題ないでしょ。", 13, "", 0, false, 52, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nということで！　ちょっと短めだったけど、満足してくれたかな？　ストーリーなんてあってないようなものだけどね。", 13, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n……エンドロール？　ないよ、そんなもの。\n<size=16>だってエンディングじゃないのに。</size>", 13, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n一応この後も続けて遊べるし、遊び足りないならまだまだユーキューネクロマンスの世界を楽しんでね！", 13, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nこの中で遊び続ける分には、いつでも大歓迎だよ。", 13, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nそれじゃ改めて。Congratulations on the fake ending.\nAnd thank you for playing!\n（ゲームクリアおめでとう。そして遊んでくれてありがとう！）", 13, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        if (mainStory[0] == 0)//３章初クリアで見る場合
        {
            PlayerPrefs.SetInt("mainStory0", 2);
            bgm.Clear();//BGMリストを初期化
            shichoFlag = false;
        }
        else//名も無い本から見に来た場合
        {
            
            u1.StartCoroutine("LoadSceneCoroutine", nextScene);
        }
    }

    /// <summary>
    /// 3章回想シーン
    /// </summary>
    private IEnumerator ScenarioText10010()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        nextScene = "SelectScene";//次に行くシーン名
        PlayerPrefs.SetInt("mainStory3", 2);
        objNowLoading.GetComponent<Image>().enabled = true;
        bgm.Add(Resources.Load<AudioClip>("ありふれたしあわせ"));
        bgm.Add(Resources.Load<AudioClip>("危険な香り"));
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
        
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第３章　　　終わりの始まり　　　-K12.8.13</color></b></i>", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそろそろ、この町も危ないかもしれませんね。――まったく、どうして避難しなかったんですか。事前に話が入っていたのに。", 2, "", 21, true, 11, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n私だけ逃げてもユアンはどうすんのさ。", 2, "", 25, false, 15, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n僕が残らざるをえないのはわかっているでしょうに。避難民移送は日本人が対象ですよ。", 2, "", 21, false, 15, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそれに、心配というならお門違いです。仮にも僕は満人ですからね。関東軍の庇護がなくても問題ないんです。", 2, "", 25, false, 15, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそんなことはわかってる。", 2, "", 21, false, 12, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……ならどうして！", 2, "", 26, false, 12, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n今ここで離れてしまったら、二度と会えなくなりそうだから。", 2, "", 26, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 2, "", 24, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nまったく、あなたはこういう時だけは本当に鋭いですね。", 2, "", 25, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nでも。それでも、あなたは残るべきではなかった。残って欲しくなかった。", 2, "", 21, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n悪いね。私って割と利己的だからさ。ユアンの都合ってのは気にしないことにした。", 2, "", 25, false, 8, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n元々片親しかいないところに、父さんまでいなくなっちゃって。頼れる人もいないのに、南満に逃げてどうすんのって感じだよね。", 2, "", 25, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nそれでもあなたは行くべきだった。助けくらい、探せばあるものです。何より、あなたはこれからここでどんな危険があるかわかっていない。", 2, "", 21, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nく～ど～い～！　いいじゃん、私は残りたかったから残ったの。誰にだって「己の行動を決める権利」ってヤツがあるんでしょ？", 2, "", 25, false, 14, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nまだ覚えてますか、その言葉。", 2, "", 24, false, 14, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nふん、一生忘れる気はないからね。", 2, "", 25, false, 15, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……まったく。お嬢様って意外と根に持ちますよね。", 2, "", 22, false, 15, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("off"));
        
        DrawColor(0.3f, 0.3f, 0.3f);
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nあなたが小さかった頃、こうして本をたくさん読み聞かせましたっけ。", 2, "", 27, false, 11, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nあの頃はユアンも、もう少し優しかった気がするよ。", 2, "", 27, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n僕もまだまだ子供でしたからね。仕事だなんて考えずに、あなたを妹みたいに思っていました。", 2, "", 21, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n随分できの悪い妹でしたけどね。", 2, "", 22, false, 10, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nホント、隙あらば憎まれ口叩くよね。", 2, "", 22, false, 12, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nでも今は――。", 2, "", 21, false, 12, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……今は？", 2, "", 21, false, 11, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nいえ、なんでもありません。", 2, "", 27, false, 11, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("bed"));
        
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nお休みなさい、よい夢を。", 2, "", 22, false, 0, false, 0));
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
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそうだ、私たちは――。", 2, "", 6, true, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("summon"));
        
        u1.BGMPlay(bgm[1]);
        StartCoroutine(ShichoMove(2));
        yield return StartCoroutine(ScenarioDraw("【？？？】\nあーあ、思い出しちゃったか。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nお手軽気楽なストーリーのはずだったんだけど、あの執事め。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nでも、そろそろ自分でも分かったんじゃない？", 2, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n<color=red>この部屋から出ちゃいけない</color>よ。それはおまえたちのためにもならないんだ。", 2, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nハッピーエンドなんて最初からないんだから。どうせ<color=red>結末は変えられない</color>。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nエンディングなんて見なければいいんだ。", 2, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\nだからさ、ここにずっと居てよ。", 2, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【？？？】\n例え記憶のページを全部集めても……。", 2, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        shichoFlag = false;
        yield return StartCoroutine(ScenarioDraw("", 2, "", 0, false, 0, false, 0));

        //シーン選択パートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
        yield return null;
    }

    /// <summary>
    /// 4章回想シーン
    /// </summary>
    private IEnumerator ScenarioText10012()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        nextScene = "SelectScene";//次に行くシーン名
        PlayerPrefs.SetInt("mainStory4", 2);
        objNowLoading.GetComponent<Image>().enabled = true;
        bgm.Add(Resources.Load<AudioClip>("ありふれたしあわせ"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 0, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        objFilm.GetComponent<SpriteRenderer>().enabled = true;
        objFilm.GetComponent<Animator>().enabled = true;
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("gun1"));      
        yield return new WaitForSeconds(1.5f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("gun1"));
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n――あれ、私はどうなったんだっけ。", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\nここは天国？　\nだとしたら随分殺風景。\n何もないよ、真っ白だ。", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n\n\n\n\n\n地獄かもなぁ。\n多分私は悪いことをしたんだ。\n心当たりは――ある。", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        StartCoroutine(ShichoMove(2));
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nやっほー、新入りのグイちゃん。", 0, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nおまえさ、あの片割れのことが随分と大事だったんだね。剥がすのが大変だったよ。\n一人ずつじゃないと人の形に作れないからさ。", 0, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nアタシ？　アタシはシステムキャラクターのシチョウ。おまえは――ネクって言うんだね。", 0, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n……ユアン？　ああ、片割れの方か。あっちにはまた別の仕事をしてもらおうと思ってて――", 0, "", 0, false, 52, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nえ、什麼（なに）？　うっわ、わがまま。一緒じゃないと仕事しないって？　足元見てくるね、このグイ。", 0, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nいいよ。自分が何になってるかも分かってないような状況で、それでも要求を通そうとするその根性に免じて認めたげる。", 0, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\nその代わり、とびきりお気楽で馬鹿馬鹿しい冒険活劇をお願いね。物語の修復も忘れずに。", 0, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n……だからそのためにさ、邪魔な過去の設定は全て「無くす」よ！　重苦しい物語なんて望まれてないものね。", 0, "", 0, false, 52, false, 0));
        yield return StartCoroutine(u1.PushWait());
        shichoFlag = false;
        objFilm.GetComponent<SpriteRenderer>().enabled = false;
        objFilm.GetComponent<Animator>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 0, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n――真っ白。どこまでも真っ白だ。", 0, false, 0, false, 0));
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n　　　　　　　ずっと忘れていた気がする。", 0, false, 0, false, 0));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n\n\n\n\n\n\nそれは大事なことだった。　　　　　　　", 0, false, 0, false, 0));
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n\n\n\n\n白い世界で無くした思い出。失った記憶の最後の欠片。", 0, false, 0, false, 0));
        yield return new WaitForSeconds(3.0f);
        yield return StartCoroutine(ScenarioDraw("", 0, "\n\n\n\n\n\n\n\n――取り戻すために、私は永遠を終わらせる――", 0, false, 0, false, 0));
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("followerbreak"));
        
        StartCoroutine("ShakeScreen");
        yield return new WaitForSeconds(2.0f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("pera"));
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　終わりのない物語</color></b></i>", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nあーあー、また布団から転がり落ちて。いい加減に起きなさい。", 2, "", 0, false, 21, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n……ユアン。主人に対する「遠慮」ってものはないの？", 2, "", 9, true, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n必要ありません。", 2, "", 9, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n言い切るのさすがだよね。――覚えててくれてたんだ、最初から。", 2, "", 9, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……お嬢様？", 2, "", 9, false, 24, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n『ヒロインの秘められた過去』とか『襲い来る悪の手先』とか、そういう派手なのは――", 2, "", 10, false, 24, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n残念ながら、そのようなシリアスな設定は「無くなってしまった」のですよ、これが。", 2, "", 10, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n…………。", 2, "", 10, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそれなら、取り返しに行こう。ユアン。私たちの物語をさ。", 2, "", 7, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……お嬢様、思い出したんですね。", 2, "", 7, false, 27, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nどんな結末かが分かっていて、それでも行こうと？", 2, "", 7, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nわかってる。取り返しに行こう、私たちの『バッドエンド』をさ。", 2, "", 10, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 2, "", 10, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\nええ。", 2, "", 10, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());

        //シーン選択パートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);

        yield return null;
    }

    /// <summary>
    /// クライマックス戦闘前
    /// </summary>
    private IEnumerator ScenarioText20000()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        //スキップ可能になる(yield returnを返す)前にシステム上の処理を終える      
        //敵のデッキを作成。
        CardData c1 = GetComponent<CardData>();
        c1.deckCard[1, 0] = 13;
        c1.deckCard[1, 1] = 6;
        c1.deckCard[1, 2] = 14;
        c1.deckCard[1, 3] = 27;
        c1.deckCard[1, 4] = 41;
        c1.deckCard[1, 5] = 56;
        c1.deckCard[1, 6] = 23;
        c1.deckCard[1, 7] = 54;
        c1.deckCard[1, 8] = 54;
        c1.deckCard[1, 9] = 54;
        c1.deckCard[1, 10] = 44;
        c1.deckCard[1, 11] = 44;
        c1.deckCard[1, 12] = 44;
        c1.deckCard[1, 13] = 51;
        c1.deckCard[1, 14] = 51;
        c1.deckCard[1, 15] = 51;
        c1.deckCard[1, 16] = 52;
        c1.deckCard[1, 17] = 52;
        c1.deckCard[1, 18] = 52;
        c1.deckCard[1, 19] = 29;
        for (int i = 0; i < DECKCARD_NUM; i++)
        {
            PlayerPrefs.SetInt("enemyDeckCard" + i.ToString(), c1.deckCard[1, i]);//敵デッキのセーブ
        }
        //マナ獲得能力を設定。
        c1.enemyGetManaPace[1] = 235;
        c1.enemyGetManaPace[2] = 238;
        c1.enemyGetManaPace[3] = 236;
        c1.enemyGetManaPace[4] = 218;
        c1.enemyGetManaPace[5] = 286;
        for (int i = 1; i < BLOCKTYPE_NUM + 1; i++)
        {
            PlayerPrefs.SetInt("enemyGetManaPace" + i.ToString(), c1.enemyGetManaPace[i]);//敵マナ獲得ペースのセーブ
        }
        nextScene = "PuzzleScene";//次に行くシーン名
        bgm.Add(Resources.Load<AudioClip>("危険な香り"));
        bgm.Add(Resources.Load<AudioClip>("夢想高速"));
        u1.BGMPlay(bgm[0]);
        GameObject.Find("BGMManager").GetComponent<BGMManager>().bgmChange(false,1);
        
        objNowLoading.GetComponent<Image>().enabled = false;
        StartCoroutine(ShichoMove(2));
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n扉の先に行くんだ？", 0, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n何が待ってるか分かるよね。やめておいた方がいいよ。", 0, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n…………。", 0, "", 0, false, 51, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n馬鹿じゃないの？　一度頭を冷やしてあげようか。", 0, "", 0, false, 50, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n準備はいーい？", 0, "", 0, false, 52, false, 0));
        yield return StartCoroutine(u1.PushWait());
        u1.BGMPlay(bgm[1]);
        //パズルパートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);

        yield return null;
    }

    /// <summary>
    /// エンディング
    /// </summary>
    private IEnumerator ScenarioText20001()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        nextScene = "StoryScene"; //次に行くシーン名
        PlayerPrefs.SetInt("scenarioCount", scenarioCount + 1);//次のシーンは+1してカード獲得画面(ScenarioText20002)       
        bgm.Add(Resources.Load<AudioClip>("ありふれたしあわせ"));
        bgm.Add(Resources.Load<AudioClip>("good_noon"));
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
        StartCoroutine(ShichoMove(2));
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n救えないね。そこまでの意志があるなら、もう止める理由もないよ。", 0, "", 0, false, 53, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n先に進んでも死が待ち構えているだけだと言うのに。一体何を求めているのか。", 0, "", 0, false, 53, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n<color=red>結末は変えられない</color>。それでもあえておまえたちは行くという。", 0, "", 0, false, 53, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n人間というものは、つくづく愚かだと思うよ。", 0, "", 0, false, 54, false, 0));
        yield return StartCoroutine(u1.PushWait());
        shichoFlag = false;
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("typewriter"));
        
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　終わりのない物語</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　終わり　ない物語</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　終わり　　い物語</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　終わり　　　物語</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　の終わり　　物語</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　語の終わり　　物</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　物語の終わり　　</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　物語の終わり　</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>第４章　物語の終わり</color></b></i>", 0, false, 0, false, 0));
        yield return new WaitForSeconds(2.0f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("pera"));
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n本当にお嬢様には呆れます。シチョウの言葉も尤もでは？", 0, "", 0, false, 21, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアンも人のこと言えないよね。", 0, "", 10, true, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n僕は――。", 0, "", 10, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nううん、確かに色々な気持ちがあるのもわかる。でも、理由はそれじゃないよね。", 0, "", 12, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 0, "", 12, false, 24, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアンが悪意だけで動いてるなら、そもそも私と一緒にグイ（幽霊）になってるはずがないんだよ。", 0, "", 10, false, 24, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n言ったでしょ？　私には全部お見通しだって。", 0, "", 10, false, 24, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 0, "", 10, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n記憶は全然残ってないんだけどね。意識が消える前の銃声と、抱えられた腕の暖かさは覚えてる。", 0, "", 10, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……そろそろ出口のようです。戻りましょう、僕たちの物語に。", 0, "", 10, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 0, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 1, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScenarioDraw("", 0, "", 0, false, 0, false, 0));
        yield return new WaitForSeconds(0.1f);
        u1.SEPlay(gameObject, Resources.Load<AudioClip>("pera"));
        yield return StartCoroutine(ScenarioDraw("", 1, "\n\n\n\n\n\n<i><b><color=white>最終章　U<color=red>n</color>Q<color=red>uenchable</color> Nec<color=red>'s</color> Romance -K12.8.14</color></b></i>", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアン、ほらコスモスが咲いてる。", 15, "", 8, true, 25, true, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……呑気なものですね。ソヴィエト兵にいつ見つかるかも分かったものではないのに。", 15, "", 8, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n見たでしょう、捕まった女性が何をされていたのか。", 15, "", 8, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nだからだよ。どうせもう逃げ切れないだろうしさ。", 15, "", 10, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n…………。", 15, "", 10, false, 24, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nねえ、ユアン。危なくなったら私を置いて逃げてね。", 15, "", 11, false, 24, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアンが死ぬことはないよ。それにさ、私も多分「殺されは」しないし。上手くやればさ。", 15, "", 16, false, 24, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n主人を置いて逃げるような真似ができるわけがないでしょう。", 15, "", 16, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nなら、今日でユアンは使用人をクビね。これで、もう私を守る必要はないよ。", 15, "", 10, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nそもそも、守ったところで無駄死にだしね。軍人相手に一人で敵うはずないじゃん。", 15, "", 12, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n最後に一緒に歩けて楽しかったよ。まるで恋人みたいじゃない？", 15, "", 8, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n高女の皆も、知ったら羨ましがるだろうなぁ、想い人とデートなんてさ。", 15, "", 8, false, 25, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n……君は、本当に自分勝手だ。こちらの気持ちも知らないで。", 15, "", 8, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nユアン……？", 15, "", 11, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n言ったでしょう？　「誰にでも、己の行動を自分で選ぶ権利がある」と。", 15, "", 11, false, 27, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n僕は、君と最後まで一緒にいようと思う。それがソヴィエト兵だろうと、例え運命であろうと、決して邪魔をさせはしない。", 15, "", 11, false, 27, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n日本人も満人もなく、やっと今……僕は君の隣に立てたんだ。例え残された時間が僅かでも、この一秒一秒を無駄にはしたくない。", 15, "", 11, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n――この十四年式、弾丸は二発だけ残っている。", 15, "", 11, false, 21, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n僕は利己的なんだよ。誰にも君を渡すつもりはない。", 15, "", 11, false, 28, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\n最高で最悪な口説き文句だね、それ。", 15, "", 16, false, 28, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ユアン】\n気に入らなかったかな？", 15, "", 16, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("【ネク】\nううん、気に入った。一緒に　行こう。", 15, "", 5, false, 22, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 15, "\n\n\n\n\n<color=white>その日の太陽は暑いくらいに輝いていた。\n内地では秋に咲くというこの花も、\n寒冷なこの地では夏に咲く。</color>", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(ScenarioDraw("", 15, "<color=white>\n\n\n\n\nキャタピラの音が、どこからか聞こえてくる。\nもう少しだけここで、一緒にコスモスの花を――</color>", 0, false, 0, false, 0));
        yield return StartCoroutine(u1.PushWait());
        yield return StartCoroutine(u1.BGMFadeOut(100));
        u1.BGMPlay(bgm[1]);
        StartCoroutine(u1.BGMFadeIn(1));
        yield return StartCoroutine(EndRoll());//エンドロール
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);

        yield return null;
    }

    private IEnumerator EndRoll()
    {
        Utility u1 = GetComponent<Utility>();
        yield return StartCoroutine(ScenarioDraw("", 14, "", 0, false, 0, false, 0));//背景以外のオブジェクトを全て消し、背景を空に。
        objBackImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1280,4000);
        for (int i = 0; i <= 1500; i++)
        {
            objBackImage.GetComponent<RectTransform>().localPosition = new Vector3(0, 1500 - i * 2);
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

    //シナリオ画面の色を変える（暗いシーン等に使用）
    private void DrawColor(float red,float green,float blue)
    {
        int i;
        for (i = 0; i < MAX_CHARACTER; i++)
        {
            objCharacter[i].GetComponent<Image>().color = new Color(red, green, blue);
        }
        objBackImage.GetComponent<Image>().color = new Color(red, green, blue);
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

        objBackImage.GetComponent<Image>().sprite = backImage[back];//背景は必ずいるのでif文はいらない。
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
        u1.BGMPlay(bgm[0]);
        objNowLoading.GetComponent<Image>().enabled = false;
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
            getCardText += "\n";
            if (c1.haveCard[getCard[i]] < 3) { c1.haveCard[getCard[i]]++; }//獲得したカードについて手持ちが3枚以下なら所持カードに追加。
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

}
//シュジンコウ継続特殊能力と通信対戦