using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LongNote : MonoBehaviour
{
    public GameObject note;
    public GameObject ring;
    public GameObject bar;
    public GameObject dot;
    public GameObject[] effect;
    GameObject myEffect = null;
    GameObject myBar;
    
    LONGNOTE myData;
    bool isTouch = true;
    bool isDecrease = false;
    int count = 0;

    float moveX = 0, moveY = 0;

    List<Vector2> _myChilds = new List<Vector2>();

    void Start()
    {
        _myChilds.Clear();
        
        StartCoroutine(descrease());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Ray2D ray = new Ray2D(touchPos, Vector2.zero);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Long Note Erase");
                    GameObject.Find("Main Camera").GetComponent<GameManager>().longNoteClick(myData.sTime);
                    Destroy(gameObject);
                }
            }
        }

        if (ring.transform.localScale.x < Define.PERFECT && !isDecrease)
        {
            isDecrease = true;
            StartCoroutine(countTime());
            StartCoroutine(moveTo());
            ring.SetActive(false);
        }

        if (isDecrease)
        {
            if (count >= myData.eTime)//&& note.transform.position.Equals(_myChilds[0]))
            {
                Destroy(gameObject);
                Destroy(myEffect);
            }
        }
    }

    public void initData(LONGNOTE baseData)
    {
        myData = baseData;

        //_myChilds.Add(myData.ePos);
        int angle = 0;

        switch(myData.moveType)
        {
            case 0:
                moveX = 0; moveY = 0.02f;
                break;
            case 1:
                moveX = 0.0066f; moveY = 0.0166f;
                break;
            case 2:
                moveX = 0.0166f; moveY = 0.0066f;
                break;
            case 3:
                moveX = 0.02f; moveY = 0;
                break;
            case 4:
                moveX = 0.0166f; moveY = -0.0066f;
                break;
            case 5:
                moveX = 0.0066f; moveY = -0.0166f;
                break;
            case 6:
                moveX = 0; moveY = -0.02f;
                break;
            case 7:
                moveX = -0.0066f; moveY = -0.0166f;
                break;
            case 8:
                moveX = -0.0166f; moveY = -0.0066f;
                break;
            case 9:
                moveX = -0.02f; moveY = 0;
                break;
            case 10:
                moveX = -0.0166f; moveY = 0.0066f;
                break;
            case 11:
                moveX = -0.0066f; moveY = 0.0166f;
                break;
        }

        Vector2 ePos;
        ePos.x = moveX * myData.eTime * 30 * 0.75f; // 1 * 5 * 30 == 150
        ePos.y = moveY * myData.eTime * 30 * 0.75f;


        GameObject a = Instantiate(dot, ePos, Quaternion.identity) as GameObject;
        a.transform.parent = transform;
        a.transform.localPosition = ePos;

        myBar = Instantiate(bar, a.transform.position, Quaternion.identity) as GameObject;
        myBar.transform.parent = a.transform;
        myBar.transform.localScale = new Vector3(0.3f * myData.eTime * 0.75f, 1, 0);
        myBar.transform.Rotate(new Vector3(0, 0, (Mathf.Atan2(ePos.y, ePos.x) * Mathf.Rad2Deg + 180) % 360));
    }

    // 원 줄어들기
    IEnumerator descrease()
    {
        yield return new WaitForSeconds(0.01f);

        if (ring.active)
        {
            ring.transform.localScale -= new Vector3(0.025f, 0.025f, 0);

            if (ring.transform.localScale.x <= 0.47f)
            {

             //   Destroy(gameObject);
            }
            else
                StartCoroutine(descrease());
        }
    }

    // 노드 움직이기
    IEnumerator moveTo()
    {
        yield return new WaitForSeconds(0.01f);
        
        note.transform.position += new Vector3(moveX * 3, moveY * 3, 0);

        if (myEffect != null)
            myEffect.transform.position = note.transform.position;

        myBar.transform.localScale -= new Vector3(0.03f, 0, 0);

        StartCoroutine(moveTo());
    }
    
    
    // 시간 재기
    IEnumerator countTime()
    {
        yield return new WaitForSeconds(0.1f);

        count++;
        StartCoroutine(countTime());
    }
}
