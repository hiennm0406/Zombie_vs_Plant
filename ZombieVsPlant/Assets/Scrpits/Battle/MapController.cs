using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<List<Transform>> Map = new List<List<Transform>>();
    // Start is called before the first frame update
    void Start()
    {       
        int row = 0;
        int col = 0;
        foreach(Transform child in transform)
        {
            if(row == Map.Count)
            {
                Map.Add(new List<Transform>());
            }
            Debug.Log(row+" - "+col);
            Map[row].Add(child);
            col++;
            if(col == 10)
            {
                col = 0;
                row++;
            }
        }
    }

}
