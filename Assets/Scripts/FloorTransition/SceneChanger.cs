using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 移動先のシーン名
    public string targetScene;

    // プレイヤーの出現位置
    public Vector3 spawnPosition;

    // カメラの設定位置
    public Vector3 cameraPosition = new Vector3(0, 0, -10);

    // ワープ機能が有効かどうかを示すフラグ
    bool canWarp = false;

    // プレイヤーの初期位置
    Vector3 playerInitialPosition;

    // プレイヤーオブジェクト
    GameObject player;

    //音源
    public AudioClip sound;
    AudioSource audioSource;

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

    void Update()
    {
        if (player != null)
        {
            // プレイヤーが初期位置から1f以上離れているか確認
            if (Vector3.Distance(player.transform.position, playerInitialPosition) >= 1f)
            {
                canWarp = true;
            }
        }
    }

    // トリガーに触れた際に呼び出される関数
    void OnTriggerEnter2D(Collider2D other)
    {

        // キャラクターがトリガーゾーンに入ったかを確認
        if (canWarp && other.CompareTag("Player"))
        {
            //音源がある場合
            if (sound != null)
            {

                Debug.Log("おとをさいせい");
                //音を鳴らす
                audioSource.PlayOneShot(sound);
                // 音の再生終了を待ってからシーンを変更する
                Debug.Log("おとをさいせい中");
                StartCoroutine(WaitForSoundAndChangeScene());
                Debug.Log("移動完了");
            }
            else
            {
                StartCoroutine(CheckPlayerMoving(other));
            }
        }
    }

    IEnumerator WaitForSoundAndChangeScene()
    {
        // 音源が再生終了するまで待機
        while (audioSource.isPlaying)
        {
            yield return null; // 次のフレームまで待機
        }
        // シーン変更を行う
        Debug.Log("おと終了");
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(targetScene);
    }



    IEnumerator CheckPlayerMoving(Collider2D playerCollider)
    {
        PlayerController playerController = playerCollider.GetComponent<PlayerController>();
        while (playerController.isMoving)
        {
            yield return null; // 次のフレームまで待機
        }

        // シーン変更イベントを登録
        SceneManager.sceneLoaded += OnSceneLoaded;

        // シーンを変更
        SceneManager.LoadScene(targetScene);
    }

    // シーンがロードされた後に呼び出される関数
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Playerオブジェクトを探して位置を更新
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = spawnPosition;

            // プレイヤーの初期位置を更新
            playerInitialPosition = player.transform.position;

            // ワープ機能を再度無効にする
            canWarp = false;
        }

        // カメラオブジェクトを探して位置を更新
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            mainCamera.transform.position = cameraPosition;
        }

        // イベントからこの関数を削除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
