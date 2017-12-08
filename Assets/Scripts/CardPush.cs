using UnityEngine;
using UnityEngine.UI;

public class CardPush : MonoBehaviour {

    public GameObject refObj;
    public GameObject refObj2;

    // Use this for initialization
    void Start () {
        refObj = GameObject.Find("card1");
        refObj2= GameObject.Find("MakeBookSceneManager");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PushCard()
    {
        int i;
        Drag d1 = refObj.GetComponent<Drag>();
        d1.objSelectCardText.GetComponent<Text>().enabled = true;
        d1.objSelectCardImage.GetComponent<Image>().enabled = true;
        CardData c1 = refObj2.GetComponent<CardData>();
        c1.CardList();
        i = c1.deckCard[0,int.Parse(name.Substring(8))];
        d1.objSelectCardText.GetComponent<Text>().text = c1.cardExplain[i];
    }

    public void LeaveCard()
    {
        Drag d1 = refObj.GetComponent<Drag>();
        d1.objSelectCardText.GetComponent<Text>().enabled = false;
        d1.objSelectCardImage.GetComponent<Image>().enabled = false;
    }


}
