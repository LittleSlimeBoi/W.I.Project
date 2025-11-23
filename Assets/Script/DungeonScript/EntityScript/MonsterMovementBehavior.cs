using UnityEngine;

[CreateAssetMenu(fileName = "Movement Behavior", menuName = "Scriptable Object/Monster/Movement/Generic")]
public class MonsterMovementBehavior : ScriptableObject
{
    public static bool CanTraverse(ObstacleType obstacleType, MonsterMoveType moveTypes)
    {
        if ((moveTypes & MonsterMoveType.Flying) != 0) 
            return true;
        if (obstacleType == ObstacleType.None) 
            return (moveTypes & MonsterMoveType.Walking) != 0;
        if (obstacleType == ObstacleType.Water) 
            return (moveTypes & MonsterMoveType.Swimming) != 0;
        return false;
    }

    public virtual void OnObstacleEnter(ObstacleType obstacleType, MonsterMoveType moveTypes, ref MonsterMoveType currentType)
    {

    }
}
