using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneGimmickController : MonoBehaviour
{// プレイヤーの初期位置
    Vector3 playerInitialPosition;

    // プレイヤーオブジェクト
    GameObject player;

    // 判定エリアのBoxCollider2D
    private BoxCollider2D boxCollider2D;

    //敵１
    public GameObject enemy1;

    //敵２
    public GameObject enemy2;
    //敵3
    public GameObject enemy3;


    //壊すオブジェクト
    public GameObject destroyObject;
    public GameObject destroyObject2;

    //音源
    public AudioClip sound;
    AudioSource audioSource;
    //音源を一回鳴らしたか
    bool firstSound = false;


    void Start()
    {
        // プレイヤーオブジェクトを探して初期位置を取得
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInitialPosition = player.transform.position;
        }
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    { // キャラクターがトリガーゾーンに入ったかを確認
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーがギミックエリアにはいりました");
            StartCoroutine(CheckHp());
        }
    }


    IEnumerator CheckHp()
    {
        Debug.Log("コルーチン起動");

        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log("敵のHPを調査中");


            if (enemy1 == null && enemy2 == null && enemy3 == null)
            {
                Debug.Log("敵は全員死にました");

                //音源がある場合
                if (sound != null && !firstSound)
                {
                    firstSound = true;
                    Debug.Log("おとをさいせい");
                    //音を鳴らす
                    audioSource.PlayOneShot(sound);
                    // 音の再生終了を待つ
                    Debug.Log("おとをさいせい中");

                }
                Destroy();
                break;
            }

        }

    }


    void Destroy()
    {
        Debug.Log("オブジェクトを壊します");
        Destroy(destroyObject);
        Destroy(destroyObject2);
    }

}
