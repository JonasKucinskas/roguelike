using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChangeScript : MonoBehaviour
{
    // Reference to the directional light

    // Colors to interpolate between
    public Color startColor;
    public Color endColor;


    // Speed at which the color changes
    public float speed;
    private float currColour=0;
    private float neededColour=0;
    private float timer;
    public float timeInterval;
    private Light lightComponent;
    private void Start()
	{
        lightComponent = GetComponent<Light>();
        if (!lightComponent) return;
        lightComponent.color = startColor;
    }
    void Update()
    {
        if (!lightComponent) return;
        timer += Time.deltaTime;
        float neededColourTemp= PlayerPrefs.GetFloat("TemperatureDisplay");
        neededColour=(neededColourTemp - 35.5f) / (45f - 35.5f);
        //changes colour
        if (timer > timeInterval)
		{
            if (Mathf.Abs(currColour - neededColour) > 0.03) {
                if (currColour < neededColour)
                {
                    currColour += speed;
                }
                else
                {
                    currColour -= speed;
                }
            }
            lightComponent.color = Color.Lerp(startColor, endColor, currColour);
            timer = 0;
		}
    }
}
