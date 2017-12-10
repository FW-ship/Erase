using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match : Photon.MonoBehaviour {

    const int BLOCKTYPE_NUM = 5;
    const int HAND_NUM = 3;
    const int DECKCARD_NUM = 20;
    public int test;
    public int test2;
    public int[,,] cardMana = new int[2,HAND_NUM,BLOCKTYPE_NUM+1];
    public int[,,,] library = new int[2, DECKCARD_NUM, 3, 10];

    public bool receiveFlag;//自身のデータを送信後、相手側のデータを受信したか。

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
            PuzzleSceneManager p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();
            cardMana = p1.cardMana;
            library = p1.library;
        if (stream.isWriting)
        {
            //データの送信
            for (int i = 0; i < HAND_NUM; i++)
            {
                for (int j = 0; j < BLOCKTYPE_NUM + 1; j++)
                {
                    stream.SendNext(cardMana[0, i, j]);
                }
            }
            for (int i = 0; i < DECKCARD_NUM; i++)
            {

                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        stream.SendNext(library[0, i, j, k]);
                    }
                }
            }
            stream.SendNext(test);
            receiveFlag = false;
        }
        else
        {
            //データの受信
            for (int i = 0; i < HAND_NUM; i++)
            {
                for (int j = 0; j < BLOCKTYPE_NUM + 1; j++)
                {
                    cardMana[1, i, j] = (int)stream.ReceiveNext();//敵のカードマナに送信データを代入
                }
            }
            for (int i = 0; i < DECKCARD_NUM; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 10; k++)
                    {
                        library[1, i, j, k] = (int)stream.ReceiveNext();
                    }
                }
            }
            test2 = (int)stream.ReceiveNext();
            receiveFlag = true;
            p1.cardMana = cardMana;
            p1.library = library;
        }
        
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
//要：同期を待つメカニズム