using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]//Matchは他から引用されるのでstartを先行処理させる。
public class Match : Photon.MonoBehaviour {

    const int BLOCKTYPE_NUM = 5;
    const int HAND_NUM = 3;
    const int DECKCARD_NUM = 20;
    private int[] enemyCardMana0 = new int[BLOCKTYPE_NUM + 1];
    private int[] enemyCardMana1 = new int[BLOCKTYPE_NUM + 1];
    private int[] enemyCardMana2 = new int[BLOCKTYPE_NUM + 1];
    private int[] enemyLibrary = new int[DECKCARD_NUM];
    private int[] enemyHand = new int[HAND_NUM];
    private int enemyLibraryNum; 
    public PhotonView m_photonView;

    void OnPhotonSerializeView(PhotonStream i_stream, PhotonMessageInfo i_info)
    {
        //マナ情報を随時更新するならここで（ObserveOptionの変更も必要）
    }

    public void DataChange(bool wait)
    {
        this.name = "MatchManager";
        PuzzleSceneManager p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();
        for (int i = 0; i < DECKCARD_NUM; i++) { enemyLibrary[i]=p1.library[0, i, 0, 0];}

            for (int j = 0; j < BLOCKTYPE_NUM + 1; j++)
            {
                enemyCardMana0[j] = p1.cardMana[0, 0, j];
                enemyCardMana1[j] = p1.cardMana[0, 0, j];
                enemyCardMana2[j] = p1.cardMana[0, 0, j];
            }
        
        enemyLibraryNum = p1.libraryNum[0];
        for (int i = 0; i < HAND_NUM; i++)
        {
            enemyHand[i] = p1.handCard[0,i];
        }
        m_photonView.RPC("ChangeDataInput", PhotonTargets.Others, wait,enemyLibrary,enemyCardMana0, enemyCardMana1, enemyCardMana2, enemyLibraryNum,enemyHand);
    }

    public void MatchEnd()
    {
        PhotonNetwork.Disconnect();
    }

    // Use this for initialization
    void Start()
    {
        m_photonView = this.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    private void ChangeDataInput(bool wait,int[] data, int[] data2_0, int[] data2_1, int[] data2_2, int data3,int[] data4)
    {
        PuzzleSceneManager p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();

        enemyLibrary = data;
        for (int i = 0; i < DECKCARD_NUM; i++)
        {
            p1.library[1, i, 0, 0] = enemyLibrary[i];
        }

        enemyCardMana0 = data2_0;
        enemyCardMana1 = data2_1;
        enemyCardMana2 = data2_2;
        for (int j = 0; j < BLOCKTYPE_NUM + 1; j++)
        {
            p1.cardMana[1, 0, j] = enemyCardMana0[j];
            p1.cardMana[1, 1, j] = enemyCardMana1[j];
            p1.cardMana[1, 2, j] = enemyCardMana2[j];
        }

        enemyLibraryNum = data3;
        p1.libraryNum[1] = enemyLibraryNum;

        enemyHand = data4;
        for (int i = 0; i < HAND_NUM; i++)
        {
            p1.handCard[1, i]=enemyHand[i];
        }

        if (wait == true) { p1.waitFlag = false; }
    }
}
//ロビーに入る前に先に誰かが部屋を作っていても見つけられない？ １ターン目開始時に携帯側だけカード画像が消える