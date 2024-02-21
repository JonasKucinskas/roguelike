using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Card : MonoBehaviour
{
    public void OnMouseEnter()
    {
        transform.Translate(Vector3.up);
    }
    
    public void OnMouseOver()
    {
    }

    public void OnMouseExit()
    {
        transform.Translate(Vector3.down);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
