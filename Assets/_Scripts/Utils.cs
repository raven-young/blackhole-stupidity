using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public static class Vector2Extension
{
    public static Vector2 Rotate(this Vector2 v, float degree)
    {
        float sin = Mathf.Sin(degree * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degree * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}

// code below from chatgpt
public static class ObjectExtensions
{
    public static List<T> GetNestedFieldValuesOfType<T>(this object obj)
    {
        List<T> values = new List<T>();
        Type type = typeof(T);

        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == type)
            {
                T fieldValue = (T)field.GetValue(obj);
                values.Add(fieldValue);
            }
        }

        return values;
    }
}