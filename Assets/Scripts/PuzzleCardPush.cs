using UnityEngine;
using UnityEngine.UI;

public class PuzzleCardPush : MonoBehaviour {
    const int HAND_NUM = 3;
    private int count = 0;
    private int n10=0;
    private int n1=0;
    private GameObject refObj;
    private GameObject[,] objText=new GameObject[2,HAND_NUM];
    private GameObject[,] objImage = new GameObject[2, HAND_NUM];
    PuzzleSceneManager p1;
    CardData c1;
    // Use this for initialization
    void Start () {
        refObj= GameObject.Find("PuzzleSceneManager");
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < HAND_NUM; j++)
            {
                objText[i, j] = GameObject.Find("ExplainText" + i.ToString() + j.ToString());
                objImage[i, j] = GameObject.Find("ExplainBack" + i.ToString() + j.ToString());
            }
        }
        p1 = refObj.GetComponent<PuzzleSceneManager>();
        c1 = refObj.GetComponent<CardData>();
        n10 = int.Parse(name.Substring(4)) / 10;
        n1 = int.Parse(name.Substring(5));
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) &&
            (Input.mousePosition.y < 620 || Input.mousePosition.y > 740 ||
            Input.mousePosition.x < n1 * 100 + 10 + (n10) * 970 || Input.mousePosition.x > n1 * 100 + 100 + (n10) * 970
            )) { OtherPush(); }
	}

    //カードがタップされた場合
    public void PushCard()
    {
        if (count == 1)
        {
            GetComponent<Image>().enabled = false;
            count = 0;
        }
        if (count == 0)
        {
            objImage[n10, n1].GetComponent<Image>().enabled = true;
            objText[n10, n1].GetComponent<Text>().enabled = true;
            string[] explainList = c1.card[p1.handCard[n10, n1].cardNum].cardExplain.Split('\n');
            objText[n10, n1].GetComponent<Text>().text = explainList[0] + "\n" + explainList[1] + "\n" + explainList[2] + "\n" + explainList[3];//テキストを代入
            count++;
        }
    }

    public void OtherPush()
    {
        objImage[n10, n1].GetComponent<Image>().enabled = false;
        objText[n10, n1].GetComponent<Text>().enabled = false;
        count = 0;
    }
}
