using UnityEngine;
using UnityEngine.UI;

public class GetCardPush : MonoBehaviour {

    public GameObject refObj;
    Scenario s1;
    CardData c1;
    // Use this for initialization
    void Start () {
        refObj= GameObject.Find("StorySceneManager");
        s1 = refObj.GetComponent<Scenario>();
        c1 = refObj.GetComponent<CardData>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //獲得カードがタップされた場合
    public void PushCard()
    {
        refObj.GetComponent<Utility>().pushObjectFlag = true;
        s1.objTextImage.GetComponentInChildren<Text>().text = "<size=36>" + c1.card[s1.getCard[int.Parse(name.Substring(4))]].cardExplain + "</size>";//テキストを説明文に変更
    }
    //獲得カードから指が離れた場合
    public void LeaveCard()
    {
        s1.objTextImage.GetComponentInChildren<Text>().text = s1.getCardText;//テキストを戻す
        refObj.GetComponent<Utility>().pushObjectFlag = false;
    }


}
