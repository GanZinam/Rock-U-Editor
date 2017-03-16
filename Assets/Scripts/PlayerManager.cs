using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum TYPE : byte { NOTE, LONG_NOTE, LINK_NOTE }

public static class Define
{
    public const float PERFECT = 0.6f;
    public const float NICE = 0.7f;
    public const float GOOD = 0.8f;
    public const float BAD = 0.9f;
}

#region TYPE_STRUCT

public struct NOTE
{
    public TYPE type;
    public int sTime;
    public Vector2 sPos;

    public NOTE(int time, Vector2 sPos)
    {
        this.type = TYPE.NOTE;
        this.sTime = time;
        this.sPos = sPos;
    }
}

public struct LONGNOTE
{
    public TYPE type;
    public int sTime;
    public Vector2 sPos;
    public int eTime;
    public int moveType;
    public int childs;

    public LONGNOTE(int sTime, Vector2 sPos, int eTime, int moveType, int childs)
    {
        this.type = TYPE.LONG_NOTE;
        this.sTime = sTime;
        this.sPos = sPos;
        this.eTime = eTime;
        this.moveType = moveType;
        this.childs = childs;
    }
}

public struct LINKNOTE
{
    public TYPE type;
    public int sTime;
    public Vector2 sPos;
    public int eTime;
    public float angle;

    public LINKNOTE(int sTime, Vector2 sPos, int eTime, float angle)
    {
        this.type = TYPE.LINK_NOTE;
        this.sTime = sTime;
        this.sPos = sPos;
        this.eTime = eTime;
        this.angle = angle;
    }
}

#endregion

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] note;

    public UnityEngine.UI.Text countText;

    AudioSource audio;
    public AudioClip[] clip;

    int count = 0;

    List<NOTE> _noteList = new List<NOTE>();
    List<LONGNOTE> _longNoteList = new List<LONGNOTE>();
    List<LINKNOTE> _linkNoteList = new List<LINKNOTE>();

    int dataCountNum = 0;
    int dataCountNum_long = 0;
    int dataCountNum_link = 0;
    bool ready = false;

    void Start()
    {
        Debug.Log(Mathf.Atan2(0.6f, 0.3f) * Mathf.Rad2Deg);

        StartCoroutine(countTime());

        _noteList.Clear();
        _longNoteList.Clear();
        _linkNoteList.Clear();

        readStringFromFile("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/Note/song" + Singleton.getInstance.songNum + ".txt");
        readStringFromFile_long("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/LongNote/song" + Singleton.getInstance.songNum + ".txt");
        readStringFromFile_link("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/LinkNote/song" + Singleton.getInstance.songNum + ".txt");

        audio = GetComponent<AudioSource>();
        audio.clip = clip[Singleton.getInstance.songNum];

        StartCoroutine(playSong());
    }

    void Update()
    {
        if (dataCountNum < _noteList.Count && ready)
        {
            if (_noteList[dataCountNum].sTime <= count)
            {
                GameObject gO = Instantiate(note[(byte)_noteList[dataCountNum].type], new Vector3(_noteList[dataCountNum].sPos.x, _noteList[dataCountNum].sPos.y, 0), Quaternion.identity) as GameObject;
                dataCountNum++;
            }
        }
        if (dataCountNum_long < _longNoteList.Count && ready)
        {
            if (_longNoteList[dataCountNum_long].sTime <= count)
            {
                GameObject gO = Instantiate(note[(byte)_longNoteList[dataCountNum_long].type], new Vector3(_longNoteList[dataCountNum_long].sPos.x, _longNoteList[dataCountNum_long].sPos.y, 0), Quaternion.identity) as GameObject;
                gO.SendMessage("initData", _longNoteList[dataCountNum_long]);
                dataCountNum_long++;
            }
        }
        if (dataCountNum_link < _linkNoteList.Count && ready)
        {
            if (_linkNoteList[dataCountNum_link].sTime <= count)
            {
                GameObject gO = Instantiate(note[(byte)_linkNoteList[dataCountNum_link].type], new Vector3(_linkNoteList[dataCountNum_link].sPos.x, _linkNoteList[dataCountNum_link].sPos.y, 0), Quaternion.identity) as GameObject;
                gO.SendMessage("initData", _linkNoteList[dataCountNum_link]);
                dataCountNum_link++;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.LoadLevel("Select");
        }
    }

    IEnumerator playSong()
    {
        yield return new WaitForSeconds(0.46f);

        ready = true;
        GetComponent<AudioSource>().Play();
    }

    public void Exit()
    {

    }

    #region ReadFile
    /// <summary>
    ///  기본 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile(string fileName)
    {
        string path = pathForDocuments(fileName);

        if (File.Exists(path))
        {
            // 파일 일기
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            string str = null;
            str = sr.ReadToEnd();

            // 단어 분리
            char sp = ',';
            string[] spString = str.Split(sp);

            NOTE note = new NOTE(0, Vector2.zero);

            for (int i = 0; i < spString.Length - 1; i++)
            {
                // string을 float형으로 변환
                float numFloat = System.Convert.ToSingle(spString[i]);

                if (i % 3 == 0)
                {
                    note.sTime = (int)numFloat;
                }
                else if (i % 3 == 1)
                {
                    note.sPos.x = numFloat;
                }
                else if (i % 3 == 2)
                {
                    note.sPos.y = numFloat;
                    _noteList.Add(note);
                }
            }
            sr.Close();
            file.Close();

            return str;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 롱 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile_long(string fileName)
    {
        string path = pathForDocuments(fileName);

        if (File.Exists(path))
        {
            // 파일 일기
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            string str = null;
            str = sr.ReadToEnd();

            // 단어 분리

            char sp = ',';
            string[] spString = str.Split(sp);

            LONGNOTE note = new LONGNOTE(0, Vector2.zero, 0, 0, 0);

            for (int i = 0; i < spString.Length - 1; i++)
            {
                // string을 float형으로 변환
                float numFloat = System.Convert.ToSingle(spString[i]);

                if (i % 6 == 0)
                {
                    note.sTime = (int)numFloat;
                }
                else if (i % 6 == 1)
                {
                    note.sPos.x = numFloat;
                }
                else if (i % 6 == 2)
                {
                    note.sPos.y = numFloat;
                }
                else if (i % 6 == 3)
                {
                    note.eTime = (int)numFloat;
                }
                else if (i % 6 == 4)
                {
                    note.moveType = (int)numFloat;
                }
                else if (i % 6 == 5)
                {
                    note.childs = (int)numFloat;
                    _longNoteList.Add(note);
                }
            }
            sr.Close();
            file.Close();

            return str;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 연결 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile_link(string fileName)
    {
        string path = pathForDocuments(fileName);

        if (File.Exists(path))
        {
            // 파일 일기
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            string str = null;
            str = sr.ReadToEnd();

            // 단어 분리
            char sp = ',';
            string[] spString = str.Split(sp);

            LINKNOTE note = new LINKNOTE(0, Vector2.zero, 0, 0);

            for (int i = 0; i < spString.Length - 1; i++)
            {
                // string을 float형으로 변환
                float numFloat = System.Convert.ToSingle(spString[i]);

                if (i % 5 == 0)
                {
                    note.sTime = (int)numFloat;
                }
                else if (i % 5 == 1)
                {
                    note.sPos.x = numFloat;
                }
                else if (i % 5 == 2)
                {
                    note.sPos.y = numFloat;
                }
                else if (i % 5 == 3)
                {
                    note.eTime = (int)numFloat;
                }
                else if (i % 5 == 4)
                {
                    note.angle = numFloat;
                    _linkNoteList.Add(note);
                }
            }
            sr.Close();
            file.Close();
            return str;
        }
        else
        {
            return null;
        }
    }

    string pathForDocuments(string fileName)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), fileName);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, fileName);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, fileName);
        }
    }
    #endregion


    IEnumerator countTime()
    {
        yield return new WaitForSeconds(0.1f);

        count++;
        countText.text = "" + count;

        StartCoroutine(countTime());
    }
}