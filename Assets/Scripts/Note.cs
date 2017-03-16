using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour
{
    public GameObject ring;

    void Start()
    {
        StartCoroutine(remove(0));
        Instantiate(ring, transform.position, Quaternion.identity);
    }
    
    IEnumerator remove(int count)
    {
        yield return new WaitForSeconds(0.1f);

        count++;

        //ring.transform.localScale -= new Vector3(0.01f, 0.01f, 0);

        if (count >= 10)
        {
            Destroy(gameObject);
           // Destroy(ring.gameObject);
        }
        else
        {
            StartCoroutine(remove(count));
        }

    }
}
