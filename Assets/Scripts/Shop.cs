using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject[] button;
    [SerializeField] GameObject[] panel;

    [SerializeField] List<GameObject> buttonList = new List<GameObject>();

    private void OnEnable()
    {
        
    }
}
