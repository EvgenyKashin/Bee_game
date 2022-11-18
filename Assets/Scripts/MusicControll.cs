using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicControll : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
