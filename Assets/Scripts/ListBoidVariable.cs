using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/List Gameobject Variable")]
public class ListBoidVariable : ScriptableObject
{
    public List<BoidMovement> boidMovements = new List<BoidMovement>(); 
}
