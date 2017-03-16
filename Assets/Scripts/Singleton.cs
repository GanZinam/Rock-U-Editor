using UnityEngine;
using System.Collections;

public class Singleton : MonoBehaviour
{
    public int level = 0;
    public int songNum = 0;

    private static Singleton instance = null;

    public static Singleton getInstance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType(typeof(Singleton)) as Singleton;
                if (instance == null)
                {
                    Debug.Log("No Instance");
                }
            }
            return instance;
        }
    }
}
