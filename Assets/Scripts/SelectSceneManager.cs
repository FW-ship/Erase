using UnityEngine;
using UnityEngine.UI;

public class SelectSceneManager : MonoBehaviour {

    const int CHAPTER_NUM = 6;                                        //名も無い本（回想）の章の数
    const int BOOK_NUM = 3;                                           //通常の本の数

    private bool chapterSelectFlag;                                 //章選択表示をしているかのフラグ
    
    private GameObject[] objBook = new GameObject[BOOK_NUM];                  //本のオブジェクト
    private GameObject[] objBlackBookChapter = new GameObject[CHAPTER_NUM];   //名も無い本の章選択オブジェクト

    // Use this for initialization
    void Start() {

        //BGM読み込みと再生
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("森林ループ"));

        //オブジェクト読み込み
        objBook[0] = GameObject.Find("ButtonBlueBook").gameObject as GameObject;
        objBook[1] = GameObject.Find("ButtonRedBook").gameObject as GameObject;
        objBook[2] = GameObject.Find("ButtonGreenBook").gameObject as GameObject;
        for (int i = 0; i < CHAPTER_NUM; i++) { objBlackBookChapter[i] = GameObject.Find("ButtonBlackBook" + i.ToString()).gameObject as GameObject; }

        //名も無い本のチャプター表示はボタンを押すまでは非表示
        for (int i = 0; i < CHAPTER_NUM; i++)
        {
            objBlackBookChapter[i].gameObject.SetActive(false);
        }

        if (PlayerPrefs.GetInt("maxScenarioCount", 1) < 5)//進行度に応じた本しか表示されない。
        {
            objBook[2].gameObject.SetActive(false);
        }
        else {
            objBook[2].gameObject.SetActive(true);
        }
        if (PlayerPrefs.GetInt("maxScenarioCount", 1) < 3)//進行度に応じた本しか表示されない。
        {
            objBook[1].gameObject.SetActive(false);
        }
        else
        {
            objBook[1].gameObject.SetActive(true);
        }

        //１～３章の回想を全部集め、４章をまだ見てないならば４章に飛ぶ
        if (PlayerPrefs.GetInt("mainStory1", 0) == 2 && PlayerPrefs.GetInt("mainStory2", 0) == 2 && PlayerPrefs.GetInt("mainStory3", 0) == 2 && PlayerPrefs.GetInt("mainStory4", 0) == 0)
        {
            PlayerPrefs.SetInt("scenarioCount", 10012);
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
        }

        //4章を見ていなければ扉は出ない。
        if (PlayerPrefs.GetInt("mainStory4", 0) < 2) { GameObject.Find("ButtonLeave").gameObject.SetActive(false); }

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PushMakeBookButton()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "MakeBookScene");
    }

    public void PushBlueBookButton()
    {
        PlayerPrefs.SetInt("scenarioCount", 1);
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }

    public void PushRedBookButton()
    {
        PlayerPrefs.SetInt("scenarioCount", 4);
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }

    public void PushGreenBookButton()
    {
        PlayerPrefs.SetInt("scenarioCount", 7);
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }

    public void PushBlackBookButton()
    {
        if (chapterSelectFlag==false)//章選択ボタンが出ていなければ出す。
        {
            for (int i = 0; i < CHAPTER_NUM; i++)
            {
                if (PlayerPrefs.GetInt("mainStory" + i.ToString(), 0) == 2)//既に回想を見ているなら
                {
                    objBlackBookChapter[i].gameObject.SetActive(true);
                }
                else
                {
                    objBlackBookChapter[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < BOOK_NUM; i++) { objBook[i].gameObject.SetActive(false); }
            chapterSelectFlag = true;
        }
        else//出ていれば消す
        {
            for (int i = 0; i < CHAPTER_NUM; i++)
            {
                objBlackBookChapter[i].gameObject.SetActive(false);
            }
            objBook[0].gameObject.SetActive(true);
            if (PlayerPrefs.GetInt("maxScenarioCount", 1) < 5)//進行度に応じた本しか表示されない。
            {
                objBook[2].gameObject.SetActive(false);
            }
            else
            {
                objBook[2].gameObject.SetActive(true);
            }
            if (PlayerPrefs.GetInt("maxScenarioCount", 1) < 3)//進行度に応じた本しか表示されない。
            {
                objBook[1].gameObject.SetActive(false);
            }
            else
            {
                objBook[1].gameObject.SetActive(true);
            }
            chapterSelectFlag = false;
        }
    }

    public void PushBlackBook1Button()
    {
        PlayerPrefs.SetInt("scenarioCount", 10003);//1章回想へ
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }
    public void PushBlackBook2Button()
    {
        PlayerPrefs.SetInt("scenarioCount", 10006);//2章回想へ
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }
    public void PushBlackBook3Button()
    {
        PlayerPrefs.SetInt("scenarioCount", 10010);//3章回想へ
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }
    public void PushBlackBook4Button()
    {
        PlayerPrefs.SetInt("scenarioCount", 10012);//4章回想へ
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }
    public void PushBlackBook5Button()
    {
        PlayerPrefs.SetInt("scenarioCount", 0);//導入回想へ
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }
    public void PushBlackBook0Button()
    {
        PlayerPrefs.SetInt("scenarioCount", 10000);//偽エンディングへ
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }

    public void PushLeaveButton()
    {
        PlayerPrefs.SetInt("scenarioCount", 20000);//クライマックスへ
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");
    }

    public void PushConnectButton()
    {
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "MatchScene");
    }


}