using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour
{
    Vector3 targetPosition;
    Rigidbody2D rigibody;
    public AudioSource walkingSound;

    float timeGoingDown;

    //Sprite animation
    public SpriteData sprites;
    float timeToNextSprite = 0f;
    int nextSpriteIndex = 0;
    SpriteRenderer renderer;

    public void Init()
    {
        targetPosition = transform.position;
        rigibody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprites.idleSprite;
        walkingSound.Play();
        walkingSound.Pause();
    }

    void Update()
    {
        if (Game.instance.isPaused) return;

        if ((Input.GetMouseButton(0) && !Game.instance.uiManager.eventSystem.IsPointerOverGameObject()))
        { 
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = mousePosition;
        }
        if (Input.GetMouseButtonDown(1))
        {
            Game.instance.playerCombat.DoMagic();
        }

        if (rigibody.velocity.y < -0.1f) timeGoingDown += Time.deltaTime;
        else timeGoingDown = 0;
        if (timeGoingDown > 0.5f) Game.instance.uiManager.InGameUIFadeTime = 0.5f;

        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKeyDown(Save.current.settings.hotbarKeyCodes[i])) Game.instance.uiManager.OnMagic_Click(i);
        }

        //Sprite animation
        UpdateSprite();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            Save.current.combatData.magic[0].amount += 20;
            Save.current.combatData.magic[1].amount += 20;
            Save.current.combatData.magic[2].amount += 20;
            Save.current.combatData.magic[3].amount += 20;
            Save.current.combatData.currentHealth = 100;

            Save.current.combatData.enabledMagics[0] = true;
            Save.current.combatData.enabledMagics[1] = true;
            Save.current.combatData.enabledMagics[2] = true;
            Save.current.combatData.enabledMagics[3] = true;

            Game.instance.uiManager.UpdateHealthUI();
            Game.instance.uiManager.UpdateMagicUI();
        }
    }

    void FixedUpdate()
    {
        if (Game.instance.isPaused)
        {
            rigibody.velocity = Vector2.zero;
            return;
        }

        if (rigibody.velocity != Vector2.zero) ToggleWalkingSound(true);
        else ToggleWalkingSound(false);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            rigibody.velocity = Vector2.zero;
        }
        else
        {
            rigibody.velocity = (targetPosition - transform.position) / Vector2.Distance(transform.position, targetPosition);
            Save.current.playerPositionX = rigibody.position.x;
            Save.current.playerPositionY = rigibody.position.y;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        targetPosition = transform.position;
        rigibody.velocity = Vector2.zero;
    }

    void UpdateSprite()
    {
        Sprite selectedSprite = renderer.sprite;
        if (rigibody.velocity != Vector2.zero)
        {
            if (timeToNextSprite > 0)
            {
                timeToNextSprite -= Time.deltaTime;
            }
            else
            {
                bool right = transform.position.x < targetPosition.x;
                bool up = transform.position.y < targetPosition.y;
                bool side = Mathf.Abs(transform.position.x - targetPosition.x) > Mathf.Abs(transform.position.y - targetPosition.y);

                if (side)
                {
                    if (right) selectedSprite = sprites.movingRightSprites[nextSpriteIndex % sprites.movingRightSprites.Length];
                    else selectedSprite = sprites.movingLeftSprites[nextSpriteIndex % sprites.movingLeftSprites.Length];
                }
                else
                {
                    if (up) selectedSprite = sprites.movingUpSprites[nextSpriteIndex % sprites.movingUpSprites.Length];
                    else selectedSprite = sprites.movingDownSprites[nextSpriteIndex % sprites.movingDownSprites.Length];
                }
                timeToNextSprite = 0.2f;
                nextSpriteIndex++;
            }
        }
        else
        {
            nextSpriteIndex = 0;
            timeToNextSprite = 0;
        }
        renderer.sprite = selectedSprite;
    }

    public void ToggleWalkingSound(bool play)
    {
        if (play)
        {
            if (!walkingSound.isPlaying) walkingSound.UnPause();
        }
        else
        {
            if (walkingSound.isPlaying) walkingSound.Pause();
        }
    }
}
