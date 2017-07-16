using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RRI/SpriteData")]
public class SpriteData : ScriptableObject
{
    public Sprite idleSprite;
    public Sprite[] movingLeftSprites;
    public Sprite[] movingRightSprites;
    public Sprite[] movingUpSprites;
    public Sprite[] movingDownSprites;
}
