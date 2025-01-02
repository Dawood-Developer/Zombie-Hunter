using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ZombieHandler))]
public class ZombieSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Reference to the target object
        ZombieHandler spawner = (ZombieHandler)target;

        DrawDefaultInspector();

        EditorGUILayout.LabelField("Spawn Chances Auto Adjust:");

        int remainingChance = 100;
        for (int i = 0; i < spawner.zombieVariants.Count; i++)
        {
            var zombie = spawner.zombieVariants[i];

            // Check if zombie is null to avoid errors
            if (zombie != null)
            {
                // Display Zombie Prefab Name
                string zombieName = zombie.zombiePrefab != null ? zombie.zombiePrefab.name : "None";
                EditorGUILayout.LabelField($"Zombie {i + 1}: {zombieName}");

                // Set spawn chance within remaining range
                zombie.spawnChance = Mathf.Clamp(zombie.spawnChance, 0, remainingChance);
                zombie.spawnChance = EditorGUILayout.IntSlider("Spawn Chance", zombie.spawnChance, 0, remainingChance);
                remainingChance -= zombie.spawnChance;
            }
            else
            {
                EditorGUILayout.LabelField($"Zombie {i + 1}: Missing Entry");
            }
        }

        if (remainingChance > 0)
        {
            EditorGUILayout.HelpBox($"Remaining {remainingChance}% chance is unused!", MessageType.Warning);
        }
    }
}
