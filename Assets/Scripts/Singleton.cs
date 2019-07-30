using UnityEngine;

public static class MonoSingleton<T> where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = GameObject.Find(typeof(T).Name).GetComponent<T>();
            if (mInstance == null)
                throw new UnityException("Singleton " + typeof(T).Name + " not found");
            return mInstance;
        }
    }
    private static T mInstance;
}

public static class Singleton<T> where T : class, new()
{
    public static T Instance => mInstance ?? (mInstance = new T());
    private static T mInstance;
}
