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
        interiorSprites = Resources.Load<InteriorSprites>("Stage Recources/" + enviromentName + "/" + enviromentName);
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

    // Again 2d array being fucky wucky
    public int[,] InitRoomArea(int width, int height)
    {
        int[,] area = new int[height, width];
        foreach (Obstacle rock in rocks)
        {
            int x = width / 2 + (int)rock.transform.localPosition.x;
            int y = height / 2 - (int)rock.transform.localPosition.y;
            area[y, x] = -1;
        }
        foreach (Obstacle obstacle in local)
        {
            int x = width / 2 + (int)obstacle.transform.localPosition.x;
            int y = height / 2 - (int)obstacle.transform.localPosition.y;
            area[y, x] = -1;
        }
        return area;
    }
}