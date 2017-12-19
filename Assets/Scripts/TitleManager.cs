using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    private int timeCount;                                           //シーン開始からのフレーム数
    private GameObject objShicho;                                    //シチョウのイメージオブジェクト

    // Use this for initialization
    void Start()
    {
        //オブジェクト読み込み
        objShicho = GameObject.Find("shicho").gameObject as GameObject;
        //スライダーの現在位置をセーブされていた位置にする。
        GameObject.Find("SliderBGM").GetComponent<Slider>().value = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        GameObject.Find("SliderSE").GetComponent<Slider>().value = PlayerPrefs.GetFloat("SEVolume", 0.8f);
        //BGM再生
        DontDestroyOnLoad(GameObject.Find("BGMManager"));//BGMマネージャーのオブジェクトはタイトル画面で作ってゲーム終了までそれを使用。
        GameObject.Find("BGMManager").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("BGMVolume", 0.8f);
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("ソウルトゥーレディオ"));
        if (PlayerPrefs.GetInt("mainStory6", 0) < 2) { GameObject.Find("ButtonMask").SetActive(false); }//クリア前には評価誘導＋ver2暗示のボタンを出さない
        GameObject.Find("BGMManager").GetComponent<BGMManager>().bgmChange(true, 0);//BGMManager内部変数の初期化
    }

    // Update is called once per frame
    void Update()
    {
        timeCount++;
        objShicho.GetComponent<RectTransform>().localPosition = new Vector3(-370, -40  - 10 * (Mathf.Cos((float)timeCount / 10)), 1);
    }

    public void PushStartButton()
    {
        if (PlayerPrefs.GetInt("maxScenarioCount", 0) == 0)//初回スタートならシナリオからスタート
        {
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
        }else{//２回目以降ならシーン選択からスタート
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
        }
    }

    public void PushGuideButton()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StudyScene");
    }

    public void PushMaskButton()
    {
        if (GameObject.Find("MaskTextBack").GetComponent<Image>().enabled == false)//マスクボタンが起動中でなければ(起動中はmasktextbackがenabledになる。)
        {
            GameObject.Find("MaskTextBack").GetComponent<Image>().enabled = true;
            GameObject.Find("MaskText").GetComponent<Text>().text = "<color=black><size=36>If you want to see the another end, write 'Choice the mask' in your review.\nIt's helpful to get 'ver2.0'.\n（もしも気に入って頂けたなら、レビューを書いていただけると幸いです。ver2.0制作の原動力になるかもしれません。）</size></color>";
            StartCoroutine(GetComponent<Utility>().GoToURL("https://play.google.com/store/apps/details?id=com.brainmixer.UQnecromance",2.0f));
        }
        else
        {
            GameObject.Find("MaskTextBack").GetComponent<Image>().enabled = false;
            GameObject.Find("MaskText").GetComponent<Text>().text = "???  ";
        }
    }


}