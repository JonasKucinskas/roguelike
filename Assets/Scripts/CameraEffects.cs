using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ShakeCamera()
	{
		Camera camera= Camera.main;
        camera.GetComponentInParent<CameraController>().LockCameraLocation=true;
		Vector3 StartingPos=new(camera.transform.position.x,camera.transform.position.y,camera.transform.position.z);
        float ShakeStrength=0.5f;
		float Length=1f;
        yield return new WaitForSeconds(0.75f);
		while (Length>0)
		{
			camera.transform.position=StartingPos+Random.insideUnitSphere*ShakeStrength;
			yield return new WaitForSeconds(0.05f);
			Length-=0.2f;
            ShakeStrength-=0.1f;
		}
		camera.transform.position=StartingPos;
        camera.GetComponentInParent<CameraController>().LockCameraLocation=false;
	}
}
