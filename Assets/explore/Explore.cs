using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //this.GetComponent<Button>.
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            Debug.Log("hello,  xxxx");
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
