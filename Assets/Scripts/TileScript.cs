using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private Color originalColor;
    private Renderer rend;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnMouseEnter()
	{
        rend.material.color = Color.cyan;
	}

	private void OnMouseExit()
	{
        rend.material.color = originalColor;
	}
}
