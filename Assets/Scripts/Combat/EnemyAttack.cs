using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAttack : MonoBehaviour
{
    public Sprite[] fireSprites;
    public Sprite[] lightningSprites;
    public Sprite[] airSprites;
    public Sprite[] waterSprites;
    public bool started;

    Animator anim;
    Enemy enemy;
    SpriteRenderer renderer;

    public void Init(Enemy enemy, bool right)
    {
        started = true;
        this.enemy = enemy;
        anim = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        if (right)  anim.SetTrigger("AttackRight");
        else        anim.SetTrigger("AttackLeft");
        switch (enemy.info.types[enemy.currentElementIndex])
        {
            case Save.MagicType.Fire:
                renderer.sprite = fireSprites[Random.Range(0, fireSprites.Length)];
                break;
            case Save.MagicType.Lightning:
                renderer.sprite = lightningSprites[Random.Range(0, lightningSprites.Length)];
                break;
            case Save.MagicType.Air:
                renderer.sprite = airSprites[Random.Range(0, airSprites.Length)];
                break;
            case Save.MagicType.Water:
                renderer.sprite = waterSprites[Random.Range(0, waterSprites.Length)];
                break;
        }
        StartCoroutine(DoDamage());
    }
    public IEnumerator DoDamage()
    {
        yield return new WaitForSeconds(0.5f);
        started = false;
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttackAnimIdle"))
        {
            if (Mathf.Abs(Vector2.Distance(enemy.transform.position, Game.instance.playerCombat.transform.position)) - 0.1f <= enemy.info.AttackRange)
            {
                Save.current.combatData.currentHealth -= 1;
                Game.instance.uiManager.UpdateHealthUI();
                Game.instance.playerCombat.PlayHitSound();
                if (Save.current.combatData.currentHealth <= 0)
                {
                    Game.instance.isPaused = true;
                    Game.instance.postEffectManager.currentEffect = PostEffects.Effect.Dying;
                    Game.instance.uiManager.UpdateDiedUI();
                }
            }
        }
    }
}
