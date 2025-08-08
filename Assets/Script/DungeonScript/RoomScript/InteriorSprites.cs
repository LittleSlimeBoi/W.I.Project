using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInteriorSprites", menuName = "Scriptable Object/Room/Interior Sprites")]
public class InteriorSprites : ScriptableObject
{
    public List<Sprite> localObstacleSprites;
    public List<AnimationClip> localObstacleAnimation;
    public List<Sprite> localRockSprites;
    public Sprite localShadow;
}
