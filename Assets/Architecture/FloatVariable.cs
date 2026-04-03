using UnityEngine;

[CreateAssetMenu(fileName = "New Float Variable", menuName = "Parkour System/Variables/Float Variable")]
public class FloatVariable : ScriptableObject
{
    public float Value;

    // A handy function to easily add to the value!
    public void ApplyChange(float amount)
    {
        Value += amount;
    }
}