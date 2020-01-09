using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level", order = 2)]
public class LevelData : ScriptableObject
{
	public string Name;
	public int AsteroidCount;
	public float[] FlyingSuacerTimings;
}
