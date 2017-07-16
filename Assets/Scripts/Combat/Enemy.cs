using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    const float DAMAGE_DURATION = 0.5f;

    public EnemyAttack attack;

    public RectTransform canvas;
    public Image fireSprite;
    public Image lightningSprite;
    public Image windSprite;
    public Image waterSprite;
    public Image hpBar;

    public AudioClip lightningHit;
    public AudioClip fireHit;
    public AudioClip waterHit;
    public AudioClip windHit;

    public AudioSource hitAudioSource;


    [HideInInspector] public EnemyInfo info;
    [HideInInspector] public GameObject mySpawner;

    [HideInInspector] public int currentElementIndex;
    [HideInInspector] public float aggroRange;
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public float newAttackCooldown;

    //malo se krece dok ceka combat
    float timeToNextRandomPosition;
    Vector2 nextPosition;

    Animator combatSymbolAnimator;

    //Sprite animation
    float timeToNextSprite = 0f;
    int nextSpriteIndex = 0;
    SpriteRenderer renderer;

    float timeToBlink = -1f;

    public void Init()
    {
        currentElementIndex = 0;
        aggroRange = 10;
        timeToNextRandomPosition = 1;
        combatSymbolAnimator = transform.GetChild(0).GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(info.scale, info.scale, info.scale);
        ChangeStatsSprites();

        Vector3 canvasPosition = canvas.localPosition;
        canvasPosition.y = -1 * renderer.sprite.bounds.size.y;
        canvasPosition.y += info.uiYOffset;
        canvas.localPosition = canvasPosition;
    }

    public void TakeDamage()
    {

        if (currentElementIndex < info.types.Length)
        {
            switch (info.types[currentElementIndex])
            {
                case Save.MagicType.Fire:
                    hitAudioSource.clip = fireHit;
                    break;
                case Save.MagicType.Lightning:
                    hitAudioSource.clip = lightningHit;
                    break;
                case Save.MagicType.Air:
                    hitAudioSource.clip = windHit;
                    break;
                case Save.MagicType.Water:
                    hitAudioSource.clip = waterHit;
                    break;
                default:
                    break;
            }
            hitAudioSource.Play();
        } 
        timeToBlink = 0;
        currentElementIndex++;
        ChangeStatsSprites();
        if (!isAttacking) Attack();
    }

    public void Attack()
    {
        if (newAttackCooldown > 0) return;
        combatSymbolAnimator.SetBool("isInCombat", true);
        isAttacking = true;

        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 1, 1 << 9);
        for (int i = 0; i < nearbyColliders.Length; i++)
        {
            Enemy enemy = nearbyColliders[i].transform.GetComponent<Enemy>();
            if (enemy != null && !enemy.isAttacking && Vector2.Distance(transform.position, enemy.transform.position) < 2) enemy.Attack();
        }
    }

    public void StopAttack()
    {
        combatSymbolAnimator.SetBool("isInCombat", false);
        isAttacking = false;
        aggroRange = info.idleAggroRange;
    }

    public void DropPickup()
    {
		int selectedType = Random.Range(1, info.types.Length + 1);
		if (selectedType > 0) 
		{
			Save.MagicType type = info.types[Random.Range(0, selectedType)];

			GameObject ammoGO = Instantiate ((GameObject)Resources.Load("AmmoPickup"));
			Ammo ammoPickup = ammoGO.GetComponent<Ammo> ();

            int minAmount = Mathf.Max(selectedType - 2, 3);
            int maxAmount = Mathf.Max(selectedType + 2, 4);
			ammoPickup.Init(type, Random.Range(minAmount, maxAmount));
			ammoGO.transform.position = transform.position;
		}
        if(info.explicitDrop.amount > 0)
        {
            GameObject ammoGO = Instantiate((GameObject)Resources.Load("AmmoPickup"));
            Ammo ammoPickup = ammoGO.GetComponent<Ammo>();
            ammoPickup.Init(info.explicitDrop.type, info.explicitDrop.amount);
            ammoGO.transform.position = transform.position + new Vector3(0.3f, 0.3f, 0);
        }
    }

    void Update()
    {
        if (Game.instance.isPaused) return;
        if (newAttackCooldown > 0) newAttackCooldown -= Time.deltaTime;
        if (newAttackCooldown < 0) newAttackCooldown = 0;

        if (!isAttacking)
        {
            timeToNextRandomPosition -= Time.deltaTime;
            if (timeToNextRandomPosition <= 0)
            {
                Vector2 addedPosition = Random.insideUnitCircle * 0.2f;
                nextPosition = new Vector2(transform.position.x + addedPosition.x, transform.position.y + addedPosition.y);

                if (Vector3.Distance(mySpawner.transform.position, nextPosition) > 1)
                {
                    nextPosition = new Vector2(mySpawner.transform.position.x + addedPosition.x, mySpawner.transform.position.y + addedPosition.y);
                }
                timeToNextRandomPosition = Random.Range(1.5f, 3);
            }
        }
        else
        {
            float distanceToSpawner = Vector2.Distance(transform.position, mySpawner.transform.position);
            float distanceToPlayer = Vector2.Distance(transform.position, Game.instance.playerCombat.transform.position);

            if (distanceToSpawner > 5 && info.enemyType == EnemyInfo.EnemyType.Mob)
            {
                StopAttack();
                newAttackCooldown = 7f;
                return;
            }

            if (distanceToPlayer > info.AttackRange) nextPosition = Game.instance.playerCombat.transform.position;
            else
            {
                nextPosition = transform.position;
                if (!attack.started && currentElementIndex < info.types.Length)
                {
                    attack.gameObject.SetActive(true);
                    attack.Init(this, transform.position.x < Game.instance.playerControl.transform.position.x);
                }
            }

        }
        transform.position = Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * 0.1f * info.speed);

        if(timeToBlink > -1)
        {
            float red = Mathf.Abs(Mathf.Sin(timeToBlink * 20));
            Color newColor = new Color(1, red, red);
            renderer.color = newColor;
            timeToBlink += Time.deltaTime;
            if (timeToBlink > DAMAGE_DURATION)
            {
                timeToBlink = -1;
                renderer.color = Color.white;
                if (currentElementIndex >= info.types.Length)
                {
                    DropPickup();
                    Save.current.combatData.currentExp += info.expDrop;
                    if (Save.current.combatData.currentLevel < Save.MAX_LEVEL)
                    {
                        if (Save.current.combatData.currentExp >= Save.current.combatData.nextLevelExp)
                        {
                            Save.LevelUp();
                        }
                    }
                    Game.instance.uiManager.UpdateExpUI();
                    Game.instance.uiManager.UpdateHealthUI();

                    transform.parent.GetComponent<Spawner>().EnemyKilled();
                    Destroy(gameObject);
                }
            }
        }
        UpdateEnemySprite();
    }

    void UpdateEnemySprite()
    {
        Sprite selectedSprite = renderer.sprite;
        if ((Vector2)transform.position != nextPosition)
        {
            if (timeToNextSprite > 0)
            {
                timeToNextSprite -= Time.deltaTime;
            }
            else
            {
                bool right = transform.position.x < nextPosition.x;
                bool up = transform.position.y < nextPosition.y;
                bool side = Mathf.Abs(transform.position.x - nextPosition.x) > Mathf.Abs(transform.position.y - nextPosition.y);

                if (side)
                {
                    if (right) selectedSprite = info.sprites.movingRightSprites[nextSpriteIndex % info.sprites.movingRightSprites.Length];
                    else selectedSprite = info.sprites.movingLeftSprites[nextSpriteIndex % info.sprites.movingLeftSprites.Length];
                }
                else
                {
                    if (up) selectedSprite = info.sprites.movingUpSprites[nextSpriteIndex % info.sprites.movingUpSprites.Length];
                    else selectedSprite = info.sprites.movingDownSprites[nextSpriteIndex % info.sprites.movingDownSprites.Length];
                }
                timeToNextSprite = 0.2f;
                nextSpriteIndex++;
            }
        }
        else
        {
            selectedSprite = info.sprites.idleSprite;
            nextSpriteIndex = 0;
            timeToNextSprite = 0;
        }
        renderer.sprite = selectedSprite;
    }

    void ChangeStatsSprites()
    {
        hpBar.fillAmount = 1 - (float)currentElementIndex / info.types.Length;
        if (currentElementIndex > info.types.Length - 1) return;
        fireSprite.enabled = lightningSprite.enabled = windSprite.enabled = waterSprite.enabled = false;

        switch (info.types[currentElementIndex])
        {
            case Save.MagicType.Fire:
                fireSprite.enabled = true;
                break;
            case Save.MagicType.Lightning:
                lightningSprite.enabled = true;
                break;
            case Save.MagicType.Air:
                windSprite.enabled = true;
                break;
            case Save.MagicType.Water:
                waterSprite.enabled = true;
                break;
        }
    }
}
