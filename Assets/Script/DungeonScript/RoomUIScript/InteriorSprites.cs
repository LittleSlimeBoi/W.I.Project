using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInteriorSprites", menuName = "Room/InteriorSprites")]
public class InteriorSprites : ScriptableObject
{
    public List<Sprite> localObstacleSprites;
    public List<AnimationClip> localObstacleAnimation;
    public List<Sprite> localRockSprites;
}
