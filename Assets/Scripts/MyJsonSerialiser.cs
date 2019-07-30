using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

public static class MyJsonSerializer 
{
	public static JsonSerializer UpdatableSerialiser 
	{
		get 
		{
			if (mUpdatableSerialiser == null)
			{
				mUpdatableSerialiser = new JsonSerializer();
				mUpdatableSerialiser.Formatting = Formatting.Indented;
				mUpdatableSerialiser.TypeNameHandling = TypeNameHandling.Auto;
				mUpdatableSerialiser.Converters.Add(Singleton<UpdateConverter>.Instance);
			    AddBaseConverters(mUpdatableSerialiser);
			}
			return mUpdatableSerialiser;
		}
	}
	private static JsonSerializer mUpdatableSerialiser;

	public static JsonSerializer DefaultSerialiser 
	{
		get 
		{
			if (mDefaultSerialiser == null)
			{
				mDefaultSerialiser = new JsonSerializer();
				mDefaultSerialiser.Formatting = Formatting.Indented;
				mDefaultSerialiser.TypeNameHandling = TypeNameHandling.Auto;
			    AddBaseConverters(mDefaultSerialiser);
			}
			return mDefaultSerialiser;
		}
	}
	private static JsonSerializer mDefaultSerialiser;

    private static void AddBaseConverters(JsonSerializer serializer)
    {
        serializer.Converters.Add(Singleton<Vector2Converter>.Instance);
        serializer.Converters.Add(Singleton<Vector3Converter>.Instance);
        serializer.Converters.Add(Singleton<Vector4Converter>.Instance);
        serializer.Converters.Add(Singleton<Matrix4x4Converter>.Instance);
    }

	public static string Serialize(object obj)
	{
		var serialiser = DefaultSerialiser;
		string result;
		using (var stream = new MemoryStream())
		{
			using (var streamWriter = new StreamWriter(stream))
			{
				serialiser.Serialize(streamWriter, obj);
				streamWriter.Flush();
				stream.Position = 0;

				using (var streamReader = new StreamReader(stream))
				{
					result = streamReader.ReadToEnd();
				}
			}
		}
		return result;
	}

	public static T Deserialize<T>(string data)
	{
		T obj;
		var deserializer = UpdatableSerialiser;
		using (var stream = new MemoryStream())
		using (var streamWriter = new StreamWriter(stream))
		{
			streamWriter.Write(data);
			streamWriter.Flush();
			stream.Position = 0;

			using (var streamReader = new StreamReader(stream))
			using (var jsonReader = new JsonTextReader(streamReader))
			{
				obj = deserializer.Deserialize<T>(jsonReader);
			}
		}
		return obj;
	}
}

public class UpdateConverter : JsonConverter
{
	private bool mSelfUse;

	public override bool CanConvert(Type objectType)
	{
		if (mSelfUse)
		{
			mSelfUse = false;
			return false;
		}
		return typeof(IUpdateable).IsAssignableFrom(objectType);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var token = JToken.ReadFrom(reader);
		if (token.Type == JTokenType.Null)
		{
			existingValue = null;
			return null;
		}
		var lastVersionInfo = objectType.GetField("LastVersion", BindingFlags.NonPublic | BindingFlags.Static);
		var lastVersion = (int)lastVersionInfo.GetValue(null);
		var version = token.Value<int>(Version);
		while (version < lastVersion)
		{
			var migrateInfo = objectType.GetMethod("Migrate_" + version.ToString(), BindingFlags.NonPublic | BindingFlags.Static);
			version = (int)migrateInfo.Invoke(null, new object[1] { token });
		}
		token[Version] = version;
		using (var jsonReaderUpdated = token.CreateReader())
		{
			mSelfUse = true;
			existingValue = serializer.Deserialize(jsonReaderUpdated, objectType);
			mSelfUse = false;
		}
		return existingValue;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		throw new NotSupportedException();
	}

	public override bool CanWrite {
		get {
			return false;
		}
	}

	private const string Version = "Version";
}

public interface IUpdateable
{
	
}

public class Vector2Converter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Vector2);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		throw new NotSupportedException();
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		var vec = (Vector2)value;
		var obj = new JObject();
		obj["x"] = vec.x;
		obj["y"] = vec.y;
		obj.WriteTo(writer);
	}

	public override bool CanRead {
		get {
			return false;
		}
	}
}

public class Vector3Converter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector3);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var vec = (Vector3)value;
        var obj = new JObject();
        obj["x"] = vec.x;
        obj["y"] = vec.y;
        obj["z"] = vec.z;
        obj.WriteTo(writer);
    }

    public override bool CanRead
    {
        get
        {
            return false;
        }
    }
}

public class Vector4Converter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector4);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var vec = (Vector4)value;
        var obj = new JObject();
        obj["x"] = vec.x;
        obj["y"] = vec.y;
        obj["z"] = vec.z;
        obj["w"] = vec.w;
        obj.WriteTo(writer);
    }

    public override bool CanRead
    {
        get
        {
            return false;
        }
    }
}

public class Matrix4x4Converter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Matrix4x4);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotSupportedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var mat = (Matrix4x4)value;
        var obj = new JObject();

        obj["m00"] = mat.m00;
        obj["m01"] = mat.m01;
        obj["m02"] = mat.m02;
        obj["m03"] = mat.m03;
        obj["m10"] = mat.m10;
        obj["m11"] = mat.m11;
        obj["m12"] = mat.m12;
        obj["m13"] = mat.m13;
        obj["m20"] = mat.m20;
        obj["m21"] = mat.m21;
        obj["m22"] = mat.m22;
        obj["m23"] = mat.m23;
        obj["m30"] = mat.m30;
        obj["m31"] = mat.m31;
        obj["m32"] = mat.m32;
        obj["m33"] = mat.m33;

        obj.WriteTo(writer);
    }

    public override bool CanRead
    {
        get
        {
            return false;
        }
    }
}