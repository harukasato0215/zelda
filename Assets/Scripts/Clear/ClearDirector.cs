using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClearDirector : MonoBehaviour
{
    // 移動先のシーン名
    public string targetScene;

    void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        StatusController statusBar = FindObjectOfType<StatusController>();
        HelpManager helpManager = FindObjectOfType<HelpManager>();
        if (player != null)
        {
            // プレイヤーオブジェクトを削除
            Destroy(player.gameObject);
        }

        if (statusBar != null)
        {
            // ステータスバーオブジェクトを削除
            Destroy(statusBar.gameObject);
        }

        if (helpManager != null)
        {
            // ヘルプオブジェクトを削除
            Destroy(helpManager.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
