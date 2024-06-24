using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [SerializeField] private int baseValue;

    public List<int> modifiers;

    public int GetValue()
    {
        int finalVaslue = baseValue;

        foreach (var item in modifiers)
        {
            finalVaslue += item;
        }

        return finalVaslue;
    }

    public void SetDefaultValue(int _value)
    {
        baseValue = _value;
    }

    public void AddModifier(int _modifier)
    {
        modifiers.Add(_modifier);
    }

    public void RemoveModifier(int _modifier)
    {
        modifiers.RemoveAt(_modifier);
    }
}
