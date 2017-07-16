using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public AudioSource hitSound;
    public AudioSource levelUpSound;

    public static float MAX_TIME_TO_NEXT_AMMO = 5f;
    [HideInInspector] public float[] timesToNextAmmos;

    Collider2D[] nearbyColliders;    

	public void Init()
    {
        timesToNextAmmos = new float[Save.current.combatData.magic.Length];

        for	(int i = 0; i < timesToNextAmmos.Length; i++)
    	{
            timesToNextAmmos[i] = MAX_TIME_TO_NEXT_AMMO;	
    	}
    }

	void Update()
    {
        if (Game.instance.isPaused) return;
        nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 1, 1 << 9);
        for (int i = 0; i < nearbyColliders.Length; i++)
        {
            Enemy enemy = nearbyColliders[i].transform.GetComponent<Enemy>();
            if (enemy != null && !enemy.isAttacking && Vector2.Distance(transform.position, enemy.transform.position) < enemy.info.idleAggroRange) enemy.Attack();
        }
        

        for (int i = 0; i < Save.current.combatData.magic.Length; i++)
        {
	        if(Save.current.combatData.enabledMagics[i] && Save.current.combatData.magic[i].amount < 5)
	        {
                timesToNextAmmos[i] -= Time.deltaTime;
	            if (timesToNextAmmos[i] <= 0)
	            {
	                timesToNextAmmos[i] = MAX_TIME_TO_NEXT_AMMO;
	                Save.current.combatData.magic[i].amount++;
	                Game.instance.uiManager.UpdateMagicUI();
	            }
	            Game.instance.uiManager.UpdateNextAmmoUI();
	        }
        }
    }

    public void DoMagic()
    {
        int selectedType = (int)Save.current.combatData.selectedMagic;

        if(Save.current.combatData.magic[selectedType].amount > 0)
        {
            string attackPath = "error";
            switch ((Save.MagicType)selectedType)
            {
                case Save.MagicType.Fire:
                    attackPath = "fireAttack";
                    break;
                case Save.MagicType.Lightning:
                    attackPath = "lightningAttack";
                    break;
                case Save.MagicType.Air:
                    attackPath = "airAttack";
                    break;
                case Save.MagicType.Water:
                    attackPath = "waterAttack";
                    break;
                default:
                    break;
            }
            GameObject magicGO = Instantiate((GameObject)Resources.Load("Attacks/" + attackPath));
            MagicAttack magicAttack = magicGO.GetComponent<MagicAttack>();
            Vector3 magicPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Save.current.combatData.magic[selectedType].amount -= 1;
            
            Enemy hoveredEnemy = GetHoveredEnemy();
            if (hoveredEnemy != null && hoveredEnemy.currentElementIndex < hoveredEnemy.info.types.Length)
            {
                Save.MagicType enemyType = hoveredEnemy.info.types[hoveredEnemy.currentElementIndex];
                if (enemyType == (Save.MagicType)selectedType)
                {
                    hoveredEnemy.TakeDamage();
                    magicPosition = hoveredEnemy.transform.position;
                }
            }

            magicPosition.z = 0;
            magicAttack.Init(magicPosition);
        }
        Game.instance.uiManager.UpdateMagicUI();
    }

    public void ChangeMagic(int newMagicIndex)
    {
        if (Save.current.combatData.enabledMagics[newMagicIndex])
        {
            Save.current.combatData.selectedMagic = (Save.MagicType)newMagicIndex;
        }
    }

    public void AddAmmo(Save.MagicType type, int amount)
    {
        Save.current.combatData.magic[(int)type].amount += amount;
        Game.instance.uiManager.UpdateMagicUI();
    }

    public Enemy GetHoveredEnemy()
    {
        Enemy output = null;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
        for (int i = 0; i < hits.Length; i++)
        {
            Enemy enemy = hits[i].collider.GetComponent<Enemy>();
            if (enemy != null) output = enemy;
        }
        return output;
    }

    public void PlayHitSound()
    {
        hitSound.Play();
    }

    public void PlayLevelUpSound()
    {
        levelUpSound.Play();
    }
}