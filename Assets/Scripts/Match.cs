using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]//Matchは他から引用されるのでstartを先行処理させる。
public class Match : Photon.MonoBehaviour {

    const int BLOCKTYPE_NUM = 5;
    const int HAND_NUM = 3;
    const int DECKCARD_NUM = 20;
    const int SKILL_TYPE = 4;                    
    private int[] enemyCardMana0 = new int[BLOCKTYPE_NUM + 1];
    private int[] enemyCardMana1 = new int[BLOCKTYPE_NUM + 1];
    private int[] enemyCardMana2 = new int[BLOCKTYPE_NUM + 1];
    private int[] enemyLibrary = new int[DECKCARD_NUM];
    private int[] enemyHand = new int[HAND_NUM];
    private int enemyLibraryNum;
    private int wait;
    private bool libraryOutFlag;
    public PhotonView m_photonView;

    void OnPhotonSerializeView(PhotonStream i_stream, PhotonMessageInfo i_info)
    {
        //マナ情報を随時更新するならここで（ObserveOptionの変更も必要）
    }

    public void DataChange()
    {
        this.name = "MatchManager";
        PuzzleSceneManager p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();
        for (int i = 0; i < DECKCARD_NUM; i++) { enemyLibrary[i]=p1.library[0, i, 0, 0];}

            for (int j = 0; j < BLOCKTYPE_NUM + 1; j++)
            {
                enemyCardMana0[j] = p1.cardMana[0, 0, j];
                enemyCardMana1[j] = p1.cardMana[0, 1, j];
                enemyCardMana2[j] = p1.cardMana[0, 2, j];
            }
        libraryOutFlag = p1.libraryOutFlag[0];
        enemyLibraryNum = p1.libraryNum[0];
        for (int i = 0; i < HAND_NUM; i++)
        {
            enemyHand[i] = p1.handCard[0,i];
        }
        wait = p1.waitCount[0];
        m_photonView.RPC("ChangeDataInput", PhotonTargets.Others, wait,enemyLibrary,enemyCardMana0, enemyCardMana1, enemyCardMana2, enemyLibraryNum,enemyHand,libraryOutFlag);
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
    private void ChangeDataInput(int wait,int[] data, int[] data2_0, int[] data2_1, int[] data2_2, int data3,int[] data4,bool data5)
    {
        PuzzleSceneManager p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();
        CardData c1 = GameObject.Find("PuzzleSceneManager").GetComponent<CardData>();
        c1.CardList();

        p1.waitCount[1]=wait;

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
            for (int k = 1; k < BLOCKTYPE_NUM + 1; k++)
            {
                p1.cardCost[1, i, k] = c1.cardCost[enemyHand[i],k];//カードのコストを代入
            }
            for (int k = 0; k < SKILL_TYPE; k++)
            {
                p1.cardSkill[1, i, k] = c1.cardSkill[enemyHand[i], k];//カードの効果を代入
            }
        }

        libraryOutFlag = data5;
        p1.libraryOutFlag[1] = libraryOutFlag;
    }
}
//シュジンコウ継続効果実装、カード追加。