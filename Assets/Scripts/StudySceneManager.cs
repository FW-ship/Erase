using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class StudySceneManager : MonoBehaviour {

    private int timeCount;                                           //シーン開始からの経過時間
    
    private GameObject objShicho;                                    //シチョウ（キャラクター）のオブジェクト
    private GameObject objText;                                      //説明テキストのオブジェクト
    private GameObject objImage;                                     //説明画像のオブジェクト
    private GameObject objBlackBoardFirst;                           //解説選択の黒板表示
    private List<Sprite> shichoImage = new List<Sprite>();           //シチョウのスプライト
    private List<Sprite> blackBoardImage = new List<Sprite>();       //黒板内容のスプライト

    // Use this for initialization
    void Start () {

        //BGM読み込みと再生
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("おもしろすぎてどっかん"));
        //効果音設定
        GetComponent<Utility>().SEAdd(gameObject);//効果音用のオーディオソースはシーン間引継ぎの必要がないのでシーンマネージャーに追加。


        //オブジェクト読み込み
        objShicho = GameObject.Find("Shicho").gameObject as GameObject;
        objText = GameObject.Find("ShichoText").gameObject as GameObject;
        objImage = GameObject.Find("BackImage").gameObject as GameObject;
        objBlackBoardFirst= GameObject.Find("BlackBoardFirst").gameObject as GameObject;

        //画像読み込み
        shichoImage.Add(Resources.Load<Sprite>("character51"));
        shichoImage.Add(Resources.Load<Sprite>("character52"));
        for (int i = 0; i < 12 + 1; i++)
        {
            blackBoardImage.Add(Resources.Load<Sprite>("study" + i.ToString()));
        }
    }
	
	// Update is called once per frame
	void Update () {
        timeCount++;
        objShicho.GetComponent<RectTransform>().localPosition = new Vector3(500, -40 - 10 * (Mathf.Cos((float)timeCount / 10)), 1);
    }

    public void PushTitleButton()
    {
        Utility u1 = GetComponent<Utility>();
        if (u1.pushObjectFlag == false)//押している間SkipButtonはずっと呼ばれるので、フラグが立った後はコルーチンを呼ばないようにする。（Onclickは押す→離すまでの一連の処理をトリガーとするので、押下のタイミングでは反応を返さない。故にGetCardPushやWaitPushとイベント判定タイミングを『押下時』で統一するためonclickは使わない。判定タイミングが異なるとWaitPushのpushObjectFlag判定が正しく行えない）
        {
            GameObject.Find("ButtonTitle").GetComponent<Animator>().enabled = true;
            GameObject.Find("ButtonTitle").GetComponent<Animator>().Play("Pressed", 0, 0);
            u1.StartCoroutine("LoadSceneCoroutine", "Title");
        }
        u1.pushObjectFlag = true;
    }

    public void PushBaseRuleButton()
    {
        StartCoroutine(BaseRule());
    }

    public void PushPazzleRuleButton()
    {
        StartCoroutine(PuzzleRule());
    }

    public void PushCardRuleButton()
    {
        StartCoroutine(CardRule());
    }

    public void PushIntroductionButton()
    {
        StartCoroutine(Introduction());
    }

    public void PushCharacterButton()
    {
        StartCoroutine(Character());
    }

    private IEnumerator Introduction()
    {
        Utility u1 = GetComponent<Utility>();
        //黒板を消す
        objBlackBoardFirst.gameObject.SetActive(false);
        objImage.GetComponent<Image>().enabled = true;
        objImage.GetComponent<Image>().sprite = blackBoardImage[8];
        objShicho.GetComponent<Image>().sprite = shichoImage[1];
        objText.GetComponent<Text>().text = "それじゃ、ゲームのストーリーについて。";
        yield return StartCoroutine(u1.PushWait());
        objShicho.GetComponent<Image>().sprite = shichoImage[0];
        objText.GetComponent<Text>().text = "女の子と執事の二人組が、物語の世界に入って壊れた物語を修正しちゃうよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "物語の世界を舞台にした冒険活劇(?)ってのがコンセプト。たまに元型を留めないストーリーになるけどそれは御愛嬌ってことで……。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "どうして彼らに物語を変える力があるのかって？　そんなこと考えたら負けだよ！　そういうものなんだって。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "決まりきった結末に、暗い設定もぜーんぶ無くしたお手軽気楽なストーリー。それが「ユーキューネクロマンス」。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "疲れた現代社会にぴったりな、ストレスフリーな物語だと思わない？";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "他に聞きたいことはある？";
        //黒板を戻す
        objBlackBoardFirst.gameObject.SetActive(true);
        objImage.GetComponent<Image>().enabled = false;
    }

    private IEnumerator Character()
    {
        Utility u1 = GetComponent<Utility>();
        //黒板を消す
        objBlackBoardFirst.gameObject.SetActive(false);
        objImage.GetComponent<Image>().enabled = true;
        objImage.GetComponent<Image>().sprite = blackBoardImage[9];
        objShicho.GetComponent<Image>().sprite = shichoImage[1];
        objText.GetComponent<Text>().text = "それじゃ、登場キャラクターについて。";
        yield return StartCoroutine(u1.PushWait());
        objShicho.GetComponent<Image>().sprite = shichoImage[0];
        objText.GetComponent<Text>().text = "まずはメインの二人組。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[10];
        objText.GetComponent<Text>().text = "ネク：お嬢様。10代半ばの女の子だね。感情とノリで生きてる。トラブルメーカーだけど、割と憎めないな。大抵自爆だし。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[11];
        objText.GetComponent<Text>().text = "ユアン：執事。20代半ばの青年だね。意外と頭がいい奴だよ。何考えてるかちょっとわからない所があるかな。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[9];
        objText.GetComponent<Text>().text = "……あんまり深い設定はないよ？　ユーキューネクロマンスは小さな部屋の中で完結するシンプルなストーリーだし。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "あとはネクちゃんたちが使うシュジンコウの紹介っと。……まぁ、大体原作準拠なんだけど。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[12];
        objText.GetComponent<Text>().text = "長靴を履いた猫：八百長が得意。クンロク大関のサポートにどうぞ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "マッドハッター：水銀中毒の帽子屋さん。錬丹術を追い求めた中国の皇帝も水銀中毒だったって言うよね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "グイ：ちゃんと発音したらグウェイかな？　ネクロマンス（降霊術）には欠かせないよね。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[9];
        objShicho.GetComponent<Image>().sprite = shichoImage[1];
        objText.GetComponent<Text>().text = "あと、忘れちゃいけないのがこのアタシ。システムキャラクターのシチョウちゃんです。";
        yield return StartCoroutine(u1.PushWait());
        objShicho.GetComponent<Image>().sprite = shichoImage[0];
        objText.GetComponent<Text>().text = "他に聞きたいことはある？";
        //黒板を戻す
        objBlackBoardFirst.gameObject.SetActive(true);
        objImage.GetComponent<Image>().enabled = false;
    }


    private IEnumerator BaseRule()
    {
        Utility u1 = GetComponent<Utility>();
        //黒板を消す
        objBlackBoardFirst.gameObject.SetActive(false);
        objImage.GetComponent<Image>().enabled = true;
        objImage.GetComponent<Image>().sprite = blackBoardImage[5];
        objShicho.GetComponent<Image>().sprite = shichoImage[1];
        objText.GetComponent<Text>().text = "それじゃ、ゲームの基本ルールについて。";
        yield return StartCoroutine(u1.PushWait());
        objShicho.GetComponent<Image>().sprite = shichoImage[0];
        objText.GetComponent<Text>().text = "１．目的\n自分のLPやライブラリが０以下になったら負け。勝とう！";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "２．遊び方\nブロックを縦か横に４つ繋げると消える。消すと呪文に使う魔力（マナ）が貯まるよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "魔力が貯まれば呪文は自動で起動する。撃ちまくって敵を倒せ！";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "３．各呪文の効果は、呪文をタップすれば確認できるよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "MakeBook画面では、ゲームで使うデッキ（ライブラリ）を作成できるから、良い呪文を拾ったらこまめに入れ替えるといいね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "呪文には<b><color=black>Ｃ</color></b>（よく見る）、<b><color=#a06000ff>ＵＣ</color></b>（たまに見る）、<b><color=#ff5000ff>Ｒ</color></b>（希少）の三段階のレアリティがあるから、そこに注目してもいいかも。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "他に聞きたいことはある？";
        //黒板を戻す
        objBlackBoardFirst.gameObject.SetActive(true);
        objImage.GetComponent<Image>().enabled = false;
    }

    private IEnumerator PuzzleRule()
    {
        Utility u1 = GetComponent<Utility>();
        //黒板を消す
        objBlackBoardFirst.gameObject.SetActive(false);
        objImage.GetComponent<Image>().enabled = true;
        objImage.GetComponent<Image>().sprite = blackBoardImage[6];
        objShicho.GetComponent<Image>().sprite = shichoImage[1];
        objText.GetComponent<Text>().text = "それじゃ、パズル部分の細かいルールについて。";
        yield return StartCoroutine(u1.PushWait());
        objShicho.GetComponent<Image>().sprite = shichoImage[0];
        objText.GetComponent<Text>().text = "１．連鎖判定\n気付いてるかもしれないけど、ブロック落下中も動けるんだよ、このゲーム。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "だから、連鎖の判定方法がちょっと特殊なの。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "「着地したら消えるブロックがある限り、ブロックを消すごとに連鎖としてカウントされる」これがこのゲームの連鎖の定義。";
        yield return StartCoroutine(u1.PushWait());
        objShicho.GetComponent<Image>().sprite = shichoImage[1];
        objText.GetComponent<Text>().text = "普通の落ち物パズルとあんまり変わらないって？　定義をよく見てみると、いろいろと悪用の仕方が――";
        yield return StartCoroutine(u1.PushWait());
        objShicho.GetComponent<Image>().sprite = shichoImage[0];
        objText.GetComponent<Text>().text = "げふんごふん。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "２．連鎖による魔力ボーナス\n獲得できる魔力は（消した時の連鎖数）×（消したブロック数）だよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "獲得魔力を計算して呪文を撃つタイミングをコントロールできるようになれば、初心者脱却かな。多重召喚とか避けたいしね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "３．ブレイク\n新しいブロックが出る場所までブロックを積み上げると、「ブレイク」が発生するよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "効果は、「全ての呪文を捨てる」と「場のブロックの全消去」。もちろん捨てた分の呪文は新しく引ける。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "効果を見て気付いたかもしれないけど、ブレイクは決してペナルティじゃないよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "どうしても手札を入れ替えたい時なんかには、わざとブレイクしてもいいかもね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "他に聞きたいことはある？";
        //黒板を戻す
        objBlackBoardFirst.gameObject.SetActive(true);
        objImage.GetComponent<Image>().enabled = false;
    }

    private IEnumerator CardRule()
    {
        Utility u1 = GetComponent<Utility>();
        //黒板を消す
        objBlackBoardFirst.gameObject.SetActive(false);
        objImage.GetComponent<Image>().enabled = true;
        objImage.GetComponent<Image>().sprite = blackBoardImage[7];
        objShicho.GetComponent<Image>().sprite = shichoImage[1];
        objText.GetComponent<Text>().text = "それじゃ、カードゲーム部分の細かいルールについて。";
        yield return StartCoroutine(u1.PushWait());
        objShicho.GetComponent<Image>().sprite = shichoImage[0];
        objText.GetComponent<Text>().text = "１．ターン構成\n特殊呪文フェイズ→強化呪文フェイズ→攻撃呪文フェイズ→戦闘フェイズ→召喚フェイズ";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "この順番でターンは進んでいくよ。各フェイズを順番に解説するね。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[0];
        objText.GetComponent<Text>().text = "２．特殊呪文フェイズ\nターンの最初のフェイズ。「特殊呪文」を唱えるフェイズだよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "特殊呪文には右上に青い走る人のマークが入ってるからそれで見分けてね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "他の呪文種別のマークを持った特殊呪文もあるけど、それも特殊呪文として扱われるよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "特殊呪文は広く色んなことをする呪文で、他の呪文種別には当てはまらない特殊な効果を持つものも多いよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "それに、効果は他の呪文と同じでも起動タイミングが特殊呪文ってだけで先制できるだとかメリットは色々。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "ちょっとトリッキーだけど、使いがいがある呪文が揃ってるね。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[1];
        objText.GetComponent<Text>().text = "３．強化呪文フェイズ\nシュジンコウを永続的に強化/弱体化させる「強化呪文」のフェイズだよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "自分のシュジンコウを強化する呪文は剣や盾のマーク、相手のシュジンコウを弱体化させる呪文は青い矢印が書かれてるね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "赤い矢印の呪文は自分のシュジンコウを弱体化させる呪文。罪深い呪文だね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "書かれた数字の色と数でAT（赤）やDF（青）の増減が示されてるよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "そうそう、言い忘れてたけど場に対象にできるシュジンコウがいなかったら呪文は失敗するよ。当然だけど。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[2];
        objText.GetComponent<Text>().text = "４．攻撃呪文フェイズ\n対戦相手を攻撃する「攻撃呪文」のフェイズだよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "攻撃呪文は炎のマークが描かれた呪文。対戦相手にダメージを与えるよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "基本的に、シュジンコウがいればシュジンコウは受けるダメージを肩代わりしてくれる。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "各フェイズの終了時にダメージの値がDFを越えてたり、DFそのものが0以下になったらシュジンコウは倒れちゃうけどね。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[3];
        objText.GetComponent<Text>().text = "５．戦闘フェイズ\nシュジンコウが攻撃する「戦闘」のフェイズだよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "シュジンコウは放っておいても毎ターン勝手に攻撃してくれるよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "戦闘では、対戦相手に自分のAT（赤色の数字）分のダメージを与えてくれる。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "ま、相手にもシュジンコウがいれば、ダメージはそっちが肩代わりするんだけどね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "このフェイズが終わったら、シュジンコウの受けたダメージがリセットされる（０になる）よ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "逆に言えば、ターン中はこのフェイズの終了まではダメージが累積するってこと。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "あと、＜常在能力＞を持つシュジンコウがいる場合は、戦闘フェイズの最初に常在能力フェイズが発生するよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "常在能力はシュジンコウがいる間は毎ターン発生するから、意外と効果が大きいね。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[4];
        objText.GetComponent<Text>().text = "６．召喚フェイズ\nシュジンコウを召喚する「召喚呪文」のフェイズだよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "召喚呪文の見た目はこんなかんじ、左下に赤、右下に青の数字が入ってるのが特徴かな。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "赤い数字がAT（攻撃力）青い数字がDF（防御力）を表してるよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "ATは戦闘で与えるダメージ、DFは１ターンに耐えられる累積ダメージの量。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "毎ターン攻撃したり守ったりしてくれるのは強力だよ。シュジンコウは上手く使おう。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "シュジンコウがいる時に召喚呪文を使ったり、２つ以上の召喚呪文を同時に使うと呪文が失敗するから気を付けて。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "このフェイズが終わると、使用した呪文を捨てて、新たな呪文をライブラリから引くことになるよ。";
        yield return StartCoroutine(u1.PushWait());
        objImage.GetComponent<Image>().sprite = blackBoardImage[7];
        objText.GetComponent<Text>().text = "７．その他（①ターン処理の先攻後攻）\nターンの処理は、各フェイズごとにプレイヤー先攻で処理されるよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "８．その他（②ターン処理中のゲーム進行）\n使用される呪文はターン開始時に確定するよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "ターン処理中に呪文の魔力が貯まりきっても次のターンまで使用できないし、";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "使用確定している呪文の魔力をスリップで失わせる、なんてのも不可能。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "９．その他（③呪文の失敗）\n呪文の対象がいなかったり、シュジンコウを複数出そうとすると呪文が失敗するよ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "ペナルティはないけど、魔力と呪文が無駄になるからそれだけで十分痛いよね。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "こんな感じかなぁ。";
        yield return StartCoroutine(u1.PushWait());
        objText.GetComponent<Text>().text = "他に聞きたいことはある？";
        //黒板を戻す
        objBlackBoardFirst.gameObject.SetActive(true);
        objImage.GetComponent<Image>().enabled = false;
    }

}