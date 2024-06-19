using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DealDamage : MonoBehaviour
{
    public float hp = 1;
    public float maxHp = 3;
    public float defense = 1f;
    public bool isDead = false; // 行動停止フラグ

    // ダメージを受けたときのイベント
    public event Action<float> OnDamageTaken;
    // 死亡時のイベント
    public event Action OnDeath;

    // 死亡時にドロップするアイテム
    public GameObject dropItem;

    // 詰み防止用無敵フラグ
    bool isNoDamage = false;

    // HealthDisplay（HPのGUI）
    HealthDisplay healthDisplay;

    // SpriteRenderer
    SpriteRenderer spriteRenderer;
    // ダメージを受けた時のスタン時間
    public float stunTime = 1.0f;
    //プレイヤーがダメージを受けた時の音
    public AudioClip playerDamageSound;
    private AudioSource audioSource;

    //ものが壊れる音
    public AudioClip breakObject;

    void Start()
    {
        // プレイヤーのHP GUI表示
        if (this.gameObject.name == "Player")
        {
            healthDisplay = FindObjectOfType<HealthDisplay>();
            if (healthDisplay != null)
            {
                healthDisplay.SetHealth(hp, maxHp);
            }
        }

        // ダメージ演出用のSpriteRendererを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Debug.Log(gameObject.name + "のspriteRendererを取得しました");
        }
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

    }

    public void Damage(float damage)
    {
        //プレイヤーがダメージを受けたら音を鳴らす
        if (this.gameObject.name == "Player")
        {
            PlayerDamageSound();
        }
        if (!isNoDamage && !isDead) // 無敵じゃないときかつ死亡していないとき
        {
            float actualDamage = damage * defense;
            if (actualDamage <= 0)
            {
                Debug.Log("0ダメージ");
                return;
            }
            hp -= actualDamage;
            Debug.Log(this.gameObject.name + "のHPが" + actualDamage + "減った");

            // ダメージを受けたことを通知する
            OnDamageTaken?.Invoke(damage);

            if (hp <= 0)
            {
                Die();
            }

            StartCoroutine(DamageFlash());

            // 主人公のHP GUI更新
            if (healthDisplay != null && this.gameObject.name == "Player")
            {
                healthDisplay.SetHealth(hp, maxHp);
            }
        }
    }

    void Die()
    {
        isDead = true; // 行動停止フラグを設定
        OnDeath?.Invoke(); // 死亡イベントを通知
        if (this.gameObject.name == "Player")
        {
            Debug.Log("ゲームオーバー！");
            StartCoroutine(PlayerDie());
        }
        else if (this.gameObject.name == "Boss_Demon(Clone)")
        {
            StartCoroutine(DemonDie());
        }
        else
        {
            if (breakObject != null)
            {
                audioSource.PlayOneShot(breakObject);
            }
            StartCoroutine(FadeOutAndDestroy());
            Debug.Log(this.gameObject.name + "は倒れた");
            if (dropItem != null)
            {
                Instantiate(dropItem, transform.position, Quaternion.identity);
            }
        }
    }

    IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 1f; // フェードアウトにかかる時間
        Color color = spriteRenderer.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 0;
        spriteRenderer.color = color;
        Destroy(gameObject); // 完全にフェードアウトしたらオブジェクトを消滅させる
    }

    IEnumerator DamageFlash()
    {
        // 無敵フラグオン
        isNoDamage = true;

        // 経過時間
        float elapsedTime = 0f;

        while (elapsedTime < stunTime)
        {
            //0.2秒ごとに
            yield return new WaitForSeconds(0.1f);

            //spriteRendererをオフ
            spriteRenderer.enabled = false;

            //0.2秒待機
            yield return new WaitForSeconds(0.1f);

            //spriteRendererをオン
            spriteRenderer.enabled = true;

            // 経過時間の更新
            elapsedTime += 0.2f;

            yield return null;
        }

        // 無敵フラグオフ
        isNoDamage = false;
    }

    IEnumerator PlayerDie()
    {
        yield return StartCoroutine(FadeOutAndDestroy());
        SceneManager.LoadScene("Scenes/GameOverScene");
    }

    IEnumerator DemonDie()
    {
        yield return StartCoroutine(FadeOutAndDestroy());
        SceneManager.LoadScene("ClearScene");
    }

    // HP回復
    public void Heal(float amount)
    {
        hp = Mathf.Min(hp + amount, maxHp);

        healthDisplay.SetHealth(hp, maxHp);
    }

    // HP上限アップ
    public void IncreaseMaxHealth(float amount)
    {
        maxHp += amount;

        healthDisplay.SetHealth(hp, maxHp);
    }

    public void PlayerDamageSound()
    {
        audioSource.PlayOneShot(playerDamageSound);
    }
}
