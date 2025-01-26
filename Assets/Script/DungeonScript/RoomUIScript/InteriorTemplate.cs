using System.Collections.Generic;
using UnityEngine;

public class InteriorTemplate : MonoBehaviour
{
    public List<Obstacle> rocks;
    public List<Obstacle> local;
    public List<CombatInfo> monsters = new();
    private InteriorSprites interiorSprites;

    public void RenderInterior(string enviromentName)
    {
        interiorSprites = Resources.Load<InteriorSprites>("Sprite/Enviroment Sprite/" + enviromentName + "/" + enviromentName);
        int localSpriteCount = interiorSprites.localObstacleSprites.Count;
        int localAnimationCount = interiorSprites.localObstacleAnimation.Count;
        int localTotalCount = localAnimationCount + localSpriteCount;
        int rockSpriteCount = interiorSprites.localRockSprites.Count;
        Sprite shadow = interiorSprites.localShadow;

        if (rocks.Count > 0)
        {
            foreach (Obstacle o in rocks)
            {
                int index = Random.Range(0, rockSpriteCount);
                o.SetSprite(interiorSprites.localRockSprites[index]);
                o.SetShadow(shadow);
            }
        }
        if (local.Count > 0)
        {
            foreach (Obstacle o in local)
            {
                int index = Random.Range(0, localTotalCount);
                if (index < localSpriteCount) o.SetSprite(interiorSprites.localObstacleSprites[index]);
                else o.SetSprite(interiorSprites.localObstacleAnimation[index - localSpriteCount]);
                o.SetShadow(shadow);
            }
        }
        
    }
}