using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 nowPosition;
    public int speed;

    private void OnEnable()
    {
        StartCoroutine(drop());
    }
   
    IEnumerator drop()
    {
        nowPosition = transform.position;
        float x = Random.Range(-0.2f, 0.2f);
        Vector3 moveTo = new Vector3(nowPosition.x + x, nowPosition.y + 0.5f, nowPosition.z);
        while (move(moveTo))
        {
            yield return null;
        }
        moveTo = new Vector3(nowPosition.x + 2*x, nowPosition.y - 0.5f, nowPosition.z);
        while (move(moveTo))
        {
            yield return null;
        }
    }

   private bool move(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime));
    }
}
