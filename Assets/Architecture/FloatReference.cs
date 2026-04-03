using System;

[Serializable]
public class FloatReference
{
    public bool UseConstant = true;
    public float ConstantValue;
    public FloatVariable Variable;

    // This makes it so if the game asks for the Value, it automatically checks the dropdown to see which one to give you!
    public float Value
    {
        get { return UseConstant ? ConstantValue : Variable.Value; }
    }
}