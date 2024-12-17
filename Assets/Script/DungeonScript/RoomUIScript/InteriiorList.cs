using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Template List", menuName = "Room/Template List")]
public class InteriiorList : ScriptableObject
{
    public List<InteriorTemplate> templates;
}
