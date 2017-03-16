using UnityEngine;
using System.Collections;

public class BasicNote : MonoBehaviour
{
    NOTE myData;

    public GameObject ring;

    void Start()
    {
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
                    Debug.Log("Basic Note Erase : " + myData.sTime);
                    GameObject.Find("Main Camera").GetComponent<GameManager>().noteClick(myData.sTime);
                    Destroy(gameObject);
                }
            }
        }
    }
    IEnumerator descrease()
    {
        yield return new WaitForSeconds(0.01f);

        ring.transform.localScale -= new Vector3(0.025f, 0.025f, 0);


        if (ring.transform.localScale.x <= 0.47f)
        {
            Destroy(gameObject);
        }
        else
            StartCoroutine(descrease());
    }

    public void initData(NOTE no)
    {
        myData = no;
    }
}
