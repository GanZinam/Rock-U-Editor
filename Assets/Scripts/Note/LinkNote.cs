using UnityEngine;
using System.Collections;

public class LinkNote : MonoBehaviour
{
    public GameObject ring;
    public GameObject link;

    LINKNOTE myData;
    bool isDecrease = false;
    int count = 0;

    void Start()
    {
        StartCoroutine(descrease());
    }
     
    void Update()
    {

        if (ring.transform.localScale.x < 0.47f && !isDecrease)
        {
            isDecrease = true;
            StartCoroutine(countTime());
            StartCoroutine(minusLinkScale());
            ring.SetActive(false);
        }

        if (isDecrease)
        {
            if (count >= myData.eTime)
            {
                Destroy(gameObject);
            }
            //if (link.transform.localScale.x <= 0)
            //{
            //    Destroy(gameObject);
            //}
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            Ray2D ray = new Ray2D(touchPos, Vector2.zero);

            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Link Note Erase");
                    GameObject.Find("Main Camera").GetComponent<GameManager>().linkNoteClick(myData.sTime);
                    Destroy(gameObject);
                }
            }
        }
    }
    public void initData(LINKNOTE baseData)
    {
        myData = baseData;

        link.transform.localScale = new Vector3(myData.eTime * 0.08f, 1, 0);

        link.transform.localScale -= new Vector3(0.064f, 0, 0);

        link.transform.Rotate(new Vector3(0, 0, myData.angle));
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

    // 링크 줄어들기
    IEnumerator minusLinkScale()
    {
        yield return new WaitForSeconds(0.01f);

        if (isDecrease)
        {
            link.transform.localScale -= new Vector3(0.008f, 0, 0);
            StartCoroutine(minusLinkScale());
        }
    }

    // 시간 재기
    IEnumerator countTime()
    {
        yield return new WaitForSeconds(0.1f);

        count++;
        StartCoroutine(countTime());
    }
}
