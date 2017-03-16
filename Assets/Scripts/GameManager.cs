using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

struct dataNote
{
    TYPE type;
    int time;
    Vector2 sPos;   // start Pos
}

struct dataLongNote
{
    TYPE type;
    int time;
    Vector2 sPos;
    int endTime;
    Vector2 ePos;
    int childs;
}

struct dataLinkNote
{
    TYPE type;
    int time;
    Vector2 sPos;
    int endTime;
    float angle;
}

public class GameManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] notes;

    public AudioSource audio;
    public AudioClip[] clip;

    public GameObject note;
    public GameObject asdf;
    public UnityEngine.UI.Text text;

    //  타입, 시간, 좌표X, 좌표Y, 시간, 좌표X, 좌표Y
    // float[] arrData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };
    float[] noteData = new float[3] { 0, 0, 0 };
    float[] longNoteData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };
    float[] linkNoteData = new float[5] { 0, 0, 0, 0, 0 };

    List<NOTE> _noteList = new List<NOTE>();
    List<LONGNOTE> _longNoteList = new List<LONGNOTE>();
    List<LINKNOTE> _linkNoteList = new List<LINKNOTE>();

    int count = 0;

    TYPE holdType = TYPE.NOTE;

    string path = "";
    FileStream file = null;
    StreamWriter swNote = null;
    StreamWriter swLongNote = null;
    StreamWriter swLinkNote = null;

    bool ready = false;
    bool symmetry = false;      // 좌우 대칭
    bool symmetra = false;      // 원점 대칭
    bool pause = false;

    Animator anim;
    Animation ani;

    int dataCountNum = 0;
    int dataCountNum_long = 0;
    int dataCountNum_link = 0;

    void Start()
    {
        audio.clip = clip[Singleton.getInstance.songNum];
        Time.timeScale = 1;

        _noteList.Clear();
        _longNoteList.Clear();
        _linkNoteList.Clear();

        readStringFromFile("Data/Level" + Singleton.getInstance.level + "/Note/song" + Singleton.getInstance.songNum, 0);
        readStringFromFile_long("Data/Level" + Singleton.getInstance.level + "/LongNote/song" + Singleton.getInstance.songNum, 0);
        readStringFromFile_link("Data/Level" + Singleton.getInstance.level + "/LinkNote/song" + Singleton.getInstance.songNum, 0);

        Debug.Log("Level : " + Singleton.getInstance.level + ", Song : " + Singleton.getInstance.songNum);

        path = pathForDocuments("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/Note" + "/song" + Singleton.getInstance.songNum + ".txt");
        file = new FileStream(path, FileMode.Create, FileAccess.Write);
        swNote = new StreamWriter(file);

        path = pathForDocuments("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/LongNote" + "/song" + Singleton.getInstance.songNum + ".txt");
        file = new FileStream(path, FileMode.Create, FileAccess.Write);
        swLongNote = new StreamWriter(file);

        path = pathForDocuments("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/LinkNote" + "/song" + Singleton.getInstance.songNum + ".txt");
        file = new FileStream(path, FileMode.Create, FileAccess.Write);
        swLinkNote = new StreamWriter(file);

        StartCoroutine(playSong());
    }

    IEnumerator countTime()
    {
        yield return new WaitForSeconds(0.1f);

        if (!pause)
        {
            count++;
            text.text = "" + count;
            StartCoroutine(countTime());
        }
    }

    IEnumerator playSong()
    {
        yield return new WaitForSeconds(2.0f);

        ready = true;
        GetComponent<AudioSource>().Play();
        StartCoroutine(countTime());
    }

    void Update()
    {
        Debug.Log(audio.time);

        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    audio.time = audio.time - 5;

        //    count -= 50;
        //}
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            audio.time += 5;

            count += 50;

            while (_noteList[dataCountNum].sTime <= count)
            {
                GameObject gO = Instantiate(notes[(byte)_noteList[dataCountNum].type], new Vector3(_noteList[dataCountNum].sPos.x, _noteList[dataCountNum].sPos.y, 0), Quaternion.identity) as GameObject;
                gO.SendMessage("initData", _noteList[dataCountNum]);

                string str = "";
                str += _noteList[dataCountNum].sTime;
                str += ", ";
                str += _noteList[dataCountNum].sPos.x;
                str += ", ";
                str += _noteList[dataCountNum].sPos.y;
                str += ", ";

                swNote.WriteLine(str);

                dataCountNum++;
            }

            while (_longNoteList[dataCountNum_long].sTime <= count)
            {
                GameObject gO = Instantiate(notes[(byte)_longNoteList[dataCountNum_long].type], new Vector3(_longNoteList[dataCountNum_long].sPos.x, _longNoteList[dataCountNum_long].sPos.y, 0), Quaternion.identity) as GameObject;
                gO.SendMessage("initData", _longNoteList[dataCountNum_long]);

                string str = "";
                str += _longNoteList[dataCountNum_long].sTime;
                str += ", ";
                str += _longNoteList[dataCountNum_long].sPos.x;
                str += ", ";
                str += _longNoteList[dataCountNum_long].sPos.y;
                str += ", ";
                str += _longNoteList[dataCountNum_long].eTime;
                str += ", ";
                str += _longNoteList[dataCountNum_long].moveType;
                str += ", ";
                str += _longNoteList[dataCountNum_long].childs;
                str += ", ";

                swLongNote.WriteLine(str);

                dataCountNum_long++;
            }

            while (_linkNoteList[dataCountNum_link].sTime <= count)
            {
                GameObject gO = Instantiate(notes[(byte)_linkNoteList[dataCountNum_link].type], new Vector3(_linkNoteList[dataCountNum_link].sPos.x, _linkNoteList[dataCountNum_link].sPos.y, 0), Quaternion.identity) as GameObject;
                gO.SendMessage("initData", _linkNoteList[dataCountNum_link]);

                string str = "";
                str += _linkNoteList[dataCountNum_link].sTime;
                str += ", ";
                str += _linkNoteList[dataCountNum_link].sPos.x;
                str += ", ";
                str += _linkNoteList[dataCountNum_link].sPos.y;
                str += ", ";
                str += _linkNoteList[dataCountNum_link].eTime;
                str += ", ";
                str += _linkNoteList[dataCountNum_link].angle;
                str += ", ";
                swLinkNote.WriteLine(str);

                dataCountNum_link++;
            }

        }


        if (ready)
        {
            if (dataCountNum < _noteList.Count)
            {
                if (_noteList[dataCountNum].sTime <= count)
                {
                    GameObject gO = Instantiate(notes[(byte)_noteList[dataCountNum].type], new Vector3(_noteList[dataCountNum].sPos.x, _noteList[dataCountNum].sPos.y, 0), Quaternion.identity) as GameObject;
                    gO.SendMessage("initData", _noteList[dataCountNum]);

                    string str = "";
                    str += _noteList[dataCountNum].sTime;
                    str += ", ";
                    str += _noteList[dataCountNum].sPos.x;
                    str += ", ";
                    str += _noteList[dataCountNum].sPos.y;
                    str += ", ";

                    swNote.WriteLine(str);

                    dataCountNum++;
                }
            }
            if (dataCountNum_long < _longNoteList.Count)
            {
                if (_longNoteList[dataCountNum_long].sTime <= count)
                {
                    GameObject gO = Instantiate(notes[(byte)_longNoteList[dataCountNum_long].type], new Vector3(_longNoteList[dataCountNum_long].sPos.x, _longNoteList[dataCountNum_long].sPos.y, 0), Quaternion.identity) as GameObject;
                    gO.SendMessage("initData", _longNoteList[dataCountNum_long]);

                    string str = "";
                    str += _longNoteList[dataCountNum_long].sTime;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].sPos.x;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].sPos.y;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].eTime;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].moveType;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].childs;
                    str += ", ";

                    swLongNote.WriteLine(str);

                    dataCountNum_long++;
                }
            }
            if (dataCountNum_link < _linkNoteList.Count)
            {
                if (_linkNoteList[dataCountNum_link].sTime <= count)
                {
                    GameObject gO = Instantiate(notes[(byte)_linkNoteList[dataCountNum_link].type], new Vector3(_linkNoteList[dataCountNum_link].sPos.x, _linkNoteList[dataCountNum_link].sPos.y, 0), Quaternion.identity) as GameObject;
                    gO.SendMessage("initData", _linkNoteList[dataCountNum_link]);

                    string str = "";
                    str += _linkNoteList[dataCountNum_link].sTime;
                    str += ", ";
                    str += _linkNoteList[dataCountNum_link].sPos.x;
                    str += ", ";
                    str += _linkNoteList[dataCountNum_link].sPos.y;
                    str += ", ";
                    str += _linkNoteList[dataCountNum_link].eTime;
                    str += ", ";
                    str += _linkNoteList[dataCountNum_link].angle;
                    str += ", ";
                    swLinkNote.WriteLine(str);

                    dataCountNum_link++;
                }
            }

            //count++;
            if (Input.GetKeyDown(KeyCode.C))
            {
                symmetry = true;
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                symmetry = false;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                symmetra = true;
            }
            else if (Input.GetKeyUp(KeyCode.V))
            {
                symmetra = false;
            }


            if (Input.GetKey(KeyCode.Z))
            {
                holdType = TYPE.LONG_NOTE;
            }
            else if (Input.GetKey(KeyCode.X))
            {
                holdType = TYPE.LINK_NOTE;
            }
            else
            {
                holdType = TYPE.NOTE;
            }

            if (Input.GetMouseButtonDown(0) && !pause)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;

                Instantiate(note, pos, Quaternion.identity);

                if (holdType == TYPE.NOTE)
                {
                    noteData[0] = count;
                    noteData[1] = pos.x;
                    noteData[2] = pos.y;

                    string str = "";

                    for (int i = 0; i < 3; i++)
                    {
                        str += noteData[i];
                        str += ", ";
                    }

                    swNote.WriteLine(str);

                    // ======================================
                    if (symmetry)
                    {
                        noteData[1] = -pos.x;

                        str = "";

                        for (int i = 0; i < 3; i++)
                        {
                            str += noteData[i];
                            str += ", ";
                        }

                        swNote.WriteLine(str);
                    }
                    else if (symmetra)
                    {
                        noteData[1] = -pos.x;
                        noteData[2] = -pos.y;

                        str = "";

                        for (int i = 0; i < 3; i++)
                        {
                            str += noteData[i];
                            str += ", ";
                        }

                        swNote.WriteLine(str);

                    }

                }
                else if (holdType == TYPE.LONG_NOTE)
                {
                    longNoteData[0] = count;
                    longNoteData[1] = pos.x;
                    longNoteData[2] = pos.y;

                    asdf.SetActive(true);
                    asdf.transform.position = pos;
                }
                else
                {
                    linkNoteData[0] = count;
                    linkNoteData[1] = pos.x;
                    linkNoteData[2] = pos.y;
                }
            }
            else if (Input.GetMouseButtonUp(0) && !pause)
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos.z = 0;

                Instantiate(note, pos, Quaternion.identity);

                int che = 0;

                if (holdType == TYPE.LONG_NOTE)
                {
                    Ray2D ray = new Ray2D(pos, Vector2.zero);

                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                    if (hit.collider != null)
                    {

                        if (hit.collider.gameObject.CompareTag("0"))
                            che = 0;
                        else if (hit.collider.gameObject.CompareTag("1"))
                            che = 1;
                        else if (hit.collider.gameObject.CompareTag("2"))
                            che = 2;
                        else if (hit.collider.gameObject.CompareTag("3"))
                            che = 3;
                        else if (hit.collider.gameObject.CompareTag("4"))
                            che = 4;
                        else if (hit.collider.gameObject.CompareTag("5"))
                            che = 5;
                        else if (hit.collider.gameObject.CompareTag("6"))
                            che = 6;
                        else if (hit.collider.gameObject.CompareTag("7"))
                            che = 7;
                        else if (hit.collider.gameObject.CompareTag("8"))
                            che = 8;
                        else if (hit.collider.gameObject.CompareTag("9"))
                            che = 9;
                        else if (hit.collider.gameObject.CompareTag("10"))
                            che = 10;
                        else if (hit.collider.gameObject.CompareTag("11"))
                            che = 11;
                    }

                    longNoteData[3] = (int)count - longNoteData[0];
                    longNoteData[4] = che;
                    longNoteData[5] = 0;

                    asdf.SetActive(false);
                    asdf.transform.position = pos;

                    string str = "";

                    for (int i = 0; i < 6; i++)
                    {
                        str += longNoteData[i];
                        str += ", ";
                    }

                    swLongNote.WriteLine(str);

                    // ===============================================

                    if (symmetry)
                    {
                        longNoteData[1] = -pos.x;

                        str = "";

                        for (int i = 0; i < 6; i++)
                        {
                            str += longNoteData[i];
                            str += ", ";
                        }

                        swLongNote.WriteLine(str);
                    }
                    else if (symmetra)
                    {
                        switch (che)
                        {
                            case 1:
                                che = 11;
                                break;
                            case 2:
                                che = 10;
                                break;
                            case 3:
                                che = 9;
                                break;
                            case 4:
                                che = 8;
                                break;
                            case 5:
                                che = 7;
                                break;
                            case 7:
                                che = 5;
                                break;
                            case 8:
                                che = 4;
                                break;
                            case 9:
                                che = 3;
                                break;
                            case 10:
                                che = 2;
                                break;
                            case 11:
                                che = 1;
                                break;
                        }

                        longNoteData[1] = -pos.x;
                        longNoteData[2] = -pos.y;
                        longNoteData[4] = che;


                        str = "";

                        for (int i = 0; i < 6; i++)
                        {
                            str += longNoteData[i];
                            str += ", ";
                        }

                        swLongNote.WriteLine(str);
                    }
                }
                else if (holdType == TYPE.LINK_NOTE)
                {
                    float dX = linkNoteData[1] - pos.x;
                    float dY = linkNoteData[2] - pos.y;

                    float distance = Mathf.Sqrt(dX * dX + dY * dY);
                    float angle = Mathf.Atan2(dY, dX);

                    linkNoteData[3] = (int)(count - linkNoteData[0]);
                    linkNoteData[4] = (angle * Mathf.Rad2Deg + 180) % 360;                    // angle

                    string str = "";

                    for (int i = 0; i < 5; i++)
                    {
                        str += linkNoteData[i];
                        str += ", ";
                    }

                    swLinkNote.WriteLine(str);

                    // ===============================================

                    if (symmetry)
                    {
                        longNoteData[1] = -pos.x;

                        str = "";

                        for (int i = 0; i < 5; i++)
                        {
                            str += linkNoteData[i];
                            str += ", ";
                        }

                        swLongNote.WriteLine(str);
                    }
                    else if (symmetra)
                    {
                        longNoteData[1] = -pos.x;
                        longNoteData[2] = -pos.y;

                        str = "";

                        for (int i = 0; i < 5; i++)
                        {
                            str += linkNoteData[i];
                            str += ", ";
                        }

                        swLongNote.WriteLine(str);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!pause)
                {
                    pause = true;
                    Time.timeScale = 0;
                    audio.Pause();
                }
                else
                {
                    pause = false;
                    StartCoroutine(countTime());
                    Time.timeScale = 1;
                    audio.Play();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                while (dataCountNum < _noteList.Count)
                {
                    GameObject gO = Instantiate(notes[(byte)_noteList[dataCountNum].type], new Vector3(_noteList[dataCountNum].sPos.x, _noteList[dataCountNum].sPos.y, 0), Quaternion.identity) as GameObject;
                    gO.SendMessage("initData", _noteList[dataCountNum]);

                    string str = "";
                    str += _noteList[dataCountNum].sTime;
                    str += ", ";
                    str += _noteList[dataCountNum].sPos.x;
                    str += ", ";
                    str += _noteList[dataCountNum].sPos.y;
                    str += ", ";

                    swNote.WriteLine(str);

                    dataCountNum++;
                }
                while (dataCountNum_long < _longNoteList.Count)
                {
                    GameObject gO = Instantiate(notes[(byte)_longNoteList[dataCountNum_long].type], new Vector3(_longNoteList[dataCountNum_long].sPos.x, _longNoteList[dataCountNum_long].sPos.y, 0), Quaternion.identity) as GameObject;
                    gO.SendMessage("initData", _longNoteList[dataCountNum_long]);

                    string str = "";
                    str += _longNoteList[dataCountNum_long].sTime;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].sPos.x;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].sPos.y;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].eTime;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].moveType;
                    str += ", ";
                    str += _longNoteList[dataCountNum_long].childs;
                    str += ", ";

                    swLongNote.WriteLine(str);

                    dataCountNum_long++;
                }
                while (dataCountNum_link < _linkNoteList.Count)
                {
                    GameObject gO = Instantiate(notes[(byte)_linkNoteList[dataCountNum_link].type], new Vector3(_linkNoteList[dataCountNum_link].sPos.x, _linkNoteList[dataCountNum_link].sPos.y, 0), Quaternion.identity) as GameObject;
                    gO.SendMessage("initData", _linkNoteList[dataCountNum_link]);

                    string str = "";
                    str += _linkNoteList[dataCountNum_link].sTime;
                    str += ", ";
                    str += _linkNoteList[dataCountNum_link].sPos.x;
                    str += ", ";
                    str += _linkNoteList[dataCountNum_link].sPos.y;
                    str += ", ";
                    str += _linkNoteList[dataCountNum_link].eTime;
                    str += ", ";
                    str += _linkNoteList[dataCountNum_link].angle;
                    str += ", ";
                    swLinkNote.WriteLine(str);

                    dataCountNum_link++;
                }


                swNote.Close();
                swLongNote.Close();
                swLinkNote.Close();
                file.Close();
                Application.LoadLevel("Select");
            }
        }
    }

    string pathForDocuments(string fileName)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = "jar:file://" + Application.dataPath + "!/assets/";
            //path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, fileName);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, fileName);
        }
    }

    /// <summary>
    ///  기본 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile(string fileName, int sTime)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        NOTE note = new NOTE(0, Vector2.zero);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 3 == 0)
            {
                if (sTime == 0)
                {
                    note.sTime = (int)numFloat;
                }
                else if (((int)numFloat) == sTime)
                {
                    i += 2;
                    return str;
                }
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
        return str;
    }

    /// <summary>
    /// 롱 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile_long(string fileName, int sTime)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        LONGNOTE note = new LONGNOTE(0, Vector2.zero, 0, 0, 0);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 6 == 0)
            {
                if (sTime == 0)
                {
                    note.sTime = (int)numFloat;
                }
                else if (((int)numFloat) == sTime)
                {
                    i += 5;
                    return str;
                }
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
        return str;
    }

    /// <summary>
    /// 연결 노트
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    string readStringFromFile_link(string fileName, int sTime)
    {
        TextAsset textAsset = Resources.Load(fileName) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        LINKNOTE note = new LINKNOTE(0, Vector2.zero, 0, 0);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 5 == 0)
            {
                if (sTime == 0)
                {
                    note.sTime = (int)numFloat;
                }
                else if (((int)numFloat) == sTime)
                {
                    i += 4;
                    return str;
                }
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
        return str;
    }


    public void noteClick(int sTime)
    {
        // NOTE CLICK
        Debug.Log("Note Erase");

        swNote.Close();     // 지금 까지 한것을 저장 

        // 그리고 다시 읽기
        TextAsset textAsset = Resources.Load("Data/Level" + Singleton.getInstance.level + "/Note/song" + Singleton.getInstance.songNum) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        NOTE note = new NOTE(0, Vector2.zero);

        // 다시 쓰기
        path = pathForDocuments("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/Note" + "/song" + Singleton.getInstance.songNum + ".txt");
        file = new FileStream(path, FileMode.Create, FileAccess.Write);
        swNote = new StreamWriter(file);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 3 == 0)
            {
                if (((int)numFloat) == sTime)
                {
                    note.sTime = -1;
                    i += 2;
                    break;
                }
                else
                {
                    note.sTime = (int)numFloat;
                }
            }
            else if (i % 3 == 1)
            {
                note.sPos.x = numFloat;
            }
            else if (i % 3 == 2)
            {
                note.sPos.y = numFloat;

                swNote.WriteLine(note.sTime + ", " + note.sPos.x + ", " + note.sPos.y + ", ");
            }
        }
    }

    public void longNoteClick(int sTime)
    {
        // NOTE CLICK
        Debug.Log("Note Erase");

        swLongNote.Close();     // 지금 까지 한것을 저장 

        // 그리고 다시 읽기
        TextAsset textAsset = Resources.Load("Data/Level" + Singleton.getInstance.level + "/LongNote/song" + Singleton.getInstance.songNum) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        LONGNOTE note = new LONGNOTE(0, Vector2.zero, 0, 0, 0);

        // 다시 쓰기
        path = pathForDocuments("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/LongNote" + "/song" + Singleton.getInstance.songNum + ".txt");
        file = new FileStream(path, FileMode.Create, FileAccess.Write);
        swLongNote = new StreamWriter(file);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 6 == 0)
            {
                if (((int)numFloat) == sTime)
                {
                    note.sTime = -1;
                    i += 5;
                    break;
                }
                else
                {
                    note.sTime = (int)numFloat;
                }
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
                swLongNote.WriteLine(note.sTime + ", " + note.sPos.x + ", " + note.sPos.y + ", " + note.eTime + ", " + note.moveType + ", " + note.childs + ", ");
            }
        }
    }

    public void linkNoteClick(int sTime)
    {
        // NOTE CLICK
        Debug.Log("Note Erase");

        swLinkNote.Close();     // 지금 까지 한것을 저장 

        // 그리고 다시 읽기
        TextAsset textAsset = Resources.Load("Data/Level" + Singleton.getInstance.level + "/LinkNote/song" + Singleton.getInstance.songNum) as TextAsset;
        string str = textAsset.text;

        char sp = ',';
        string[] spString = str.Split(sp);

        LINKNOTE note = new LINKNOTE(0, Vector2.zero, 0, 0);

        // 다시 쓰기
        path = pathForDocuments("Assets/Resources/Data/Level" + Singleton.getInstance.level + "/LinkNote" + "/song" + Singleton.getInstance.songNum + ".txt");
        file = new FileStream(path, FileMode.Create, FileAccess.Write);
        swLinkNote = new StreamWriter(file);

        for (int i = 0; i < spString.Length - 1; i++)
        {
            // string을 float형으로 변환
            float numFloat = System.Convert.ToSingle(spString[i]);

            if (i % 5 == 0)
            {
                if (((int)numFloat) == sTime)
                {
                    note.sTime = -1;
                    i += 4;
                    break;
                }
                else
                {
                    note.sTime = (int)numFloat;
                }
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
                swLinkNote.WriteLine(note.sTime + ", " + note.sPos.x + ", " + note.sPos.y + ", " + note.eTime + ", " + note.angle + ", ");
            }
        }
    }
}