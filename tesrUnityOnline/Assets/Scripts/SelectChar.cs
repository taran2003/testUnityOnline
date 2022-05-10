using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectChar : MonoBehaviour
{
    public int pers;
    public GameObject[] chars;

    public void Next()
    {
        if (pers == chars.Length-1) return;
        chars[pers].SetActive(false); 
        pers++;
        chars[pers].SetActive(true);
    }

    public void Prev()
    {
        if (pers == 0) return;
        chars[pers].SetActive(false);
        pers--;
        chars[pers].SetActive(true);
    }


}
