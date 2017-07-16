using UnityEngine;

[CreateAssetMenu(menuName = "RRI/Enemy")]
public class EnemyInfo : ScriptableObject
{
    static int idCounter = 0;
    [HideInInspector] public int enemyId = idCounter++;
    //public int maxHealth;
    public int speed;
    public float scale = 1;
    public float idleAggroRange;
    //public float AttackingAggroRange;
    public float AttackRange;
    public string enemyName;
    public int expDrop;
    public SpriteData sprites;
    public Save.MagicType[] types;
    public float uiYOffset;
    public EnemyType enemyType;

    public Save.MagicData explicitDrop;

    public enum EnemyType
    {
        Mob,
        Boss
    }
}