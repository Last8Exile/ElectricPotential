using UnityEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;

public static class Extensions {

	public static void RemoveChilds(this Transform transform)
	{
		foreach (Transform child in transform)
			GameObject.Destroy (child.gameObject);
	}

	public static void SortChilds(this Transform transform)
	{
		var list = new List<Transform>(transform.childCount);
	    list.AddRange(transform.Cast<Transform>());
	    transform.DetachChildren();
		list.OrderBy(x => x.gameObject.name).ForEach(x => x.SetParent(transform));
	}

    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
	{
		foreach (var item in collection)
			action(item);
	}

	public static bool HasFlag(this Enum variable, Enum value)
	{
		var num = Convert.ToUInt16(value);
		return (Convert.ToUInt16(variable) & num) == num;
	}

	public static int Pow(int value, int degree)
	{
		if (degree < 0)
			throw new ArgumentOutOfRangeException(nameof(degree), "degree должен быть не меньше нуля");
		if (degree == 0)
			return 1;
		if (degree == 1)
			return value;
		
		var x = Pow(value,degree / 2);
		if (degree % 2 == 1)
			return value * x * x;
		return x * x;
	}

    #region VectorStuff

    public static Vector4 ToVector4(this Vector3 vector, float value = 0)
    {
        return new Vector4(vector.x, vector.y, vector.z, value);
    }

    public static Vector3 ToVector3(this Vector4 vector, Axis sacrifise = Axis.w)
    {
        switch (sacrifise)
        {
            case Axis.x:
                return new Vector3(vector.y, vector.z, vector.w);
            case Axis.y:
                return new Vector3(vector.x, vector.z, vector.w);
            case Axis.z:
                return new Vector3(vector.x, vector.y, vector.w);
            case Axis.w:
                return new Vector3(vector.x, vector.y, vector.z);
        }
        throw new UnityException();
    }

    public static Vector3 ToVector3(this Vector2 vector, float value = 0)
    {
        return new Vector3(vector.x, vector.y, value);
    }

    public static Vector2 ToVector2(this Vector3 vector, Axis sacrifise = Axis.z)
    {
        switch (sacrifise)
        {
            case Axis.x:
                return new Vector2(vector.y, vector.z);
            case Axis.y:
                return new Vector2(vector.x, vector.z);
            case Axis.z:
                return new Vector2(vector.x, vector.y);
        }
        throw new UnityException();
    }

    public static Vector3 Change(this Vector3 vec, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.x:
                vec.x = value;
                break;
            case Axis.y:
                vec.y = value;
                break;
            case Axis.z:
                vec.z = value;
                break;
        }
        return vec;
    }

    public static Vector4 Change(this Vector4 vec, Axis axis, float value)
    {
        switch (axis)
        {
            case Axis.x:
                vec.x = value;
                break;
            case Axis.y:
                vec.y = value;
                break;
            case Axis.z:
                vec.z = value;
                break;
            case Axis.w:
                vec.w = value;
                break;
        }
        return vec;
    }

    public static float Get(this Vector3 vec, Axis axis)
    {
        switch (axis)
        {
            case Axis.x:
                return vec.x;
            case Axis.y:
                return vec.y;
            case Axis.z:
                return vec.z;
        }
        throw new ArgumentException("Support only x,y,z axes");
    }

    public static Vector3 ScreenToWorldPos(Vector2 point)
    {
        return Camera.main.ScreenToWorldPoint(point.ToVector3());
    }

    public static Vector3 Multiply(this Vector3 vec, float x, float y, float z)
    {
        return new Vector3(vec.x * x, vec.y * y, vec.z * z);
    }

    public static Vector3 Multiply(this Vector3 vec, Vector3 target)
    {
        return new Vector3(vec.x * target.x, vec.y * target.y, vec.z * target.z);
    }

    public static Vector3 Swap(this Vector3 vec, Axis first, Axis second)
    {
        if (first == second)
            return vec;

        var value = vec.Get(first);
        vec = vec.Change(first, vec.Get(second));
        return vec.Change(second, value);
    }

    #endregion

    public static float[] ToArray(this Matrix4x4 matrix)
    {
        return new float[]
        {
            matrix[0, 0], matrix[1, 0], matrix[2, 0], matrix[3, 0],
            matrix[0, 1], matrix[1, 1], matrix[2, 1], matrix[3, 1],
            matrix[0, 2], matrix[1, 2], matrix[2, 2], matrix[3, 2],
            matrix[0, 3], matrix[1, 3], matrix[2, 3], matrix[3, 3]
        };
    }

    public static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
    {
        RenderTexture.active = dest;

        fxMaterial.SetTexture("_MainTex", source);

        GL.PushMatrix();
        GL.LoadOrtho(); // Note: z value of vertices don't make a difference because we are using ortho projection

        fxMaterial.SetPass(passNr);

        GL.Begin(GL.QUADS);

        // Here, GL.MultitexCoord2(0, x, y) assigns the value (x, y) to the TEXCOORD0 slot in the shader.
        // GL.Vertex3(x,y,z) queues up a vertex at position (x, y, z) to be drawn.  Note that we are storing
        // our own custom frustum information in the z coordinate.
        GL.MultiTexCoord2(0, 0.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

        GL.MultiTexCoord2(0, 1.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

        GL.MultiTexCoord2(0, 1.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

        GL.MultiTexCoord2(0, 0.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

        GL.End();
        GL.PopMatrix();
    }

    ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
    ///<param name="items">The enumerable to search.</param>
    ///<param name="predicate">The expression to test the items against.</param>
    ///<returns>The index of the first matching item, or -1 if no items match.</returns>
    public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        if (items == null) throw new ArgumentNullException("items");
        if (predicate == null) throw new ArgumentNullException("predicate");

        int retVal = 0;
        foreach (var item in items)
        {
            if (predicate(item)) return retVal;
            retVal++;
        }
        return -1;
    }

    ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
    ///<param name="items">The enumerable to search.</param>
    ///<param name="item">The item to find.</param>
    ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
    public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }

    public static string GetDescription(this Enum value)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name != null)
        {
            FieldInfo field = type.GetField(name);
            if (field != null)
            {
                DescriptionAttribute attr =
                    Attribute.GetCustomAttribute(field,
                        typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null)
                {
                    return attr.Description;
                }
            }
        }
        return null;
    }
}

public enum Axis
{
	x,
	y,
	z,
    w
}

[Serializable]
public class IntEvent : UnityEvent<int> { }

[Serializable]
public class FloatEvent : UnityEvent<float> { }

[Serializable]
public class Vector3Event : UnityEvent<Vector3> { }

[Serializable]
public class Vector4Event : UnityEvent<Vector4> { }
