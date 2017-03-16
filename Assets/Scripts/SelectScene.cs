using UnityEngine;
using System.Collections;

public class SelectScene : MonoBehaviour
{
    public void setLevel(int level)
    {
        Singleton.getInstance.level = level;
    }

    public void songNum(int num)
    {
        Singleton.getInstance.songNum = num;
    }

    public void start()
    {
        DontDestroyOnLoad(Singleton.getInstance.gameObject);
        Application.LoadLevel("Editor");
    }

    public void play()
    {
        DontDestroyOnLoad(Singleton.getInstance.gameObject);
        Application.LoadLevel("Player");
    }
}
