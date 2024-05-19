using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTest : MonoBehaviour
{
    public ParticleSystem ImplosionStart;
    public ParticleSystem ImplosionEnd;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartImplosionEffect()
    {
        Vector3 SpawnPosition = new(transform.position.x-0.1f,transform.position.y+1,transform.position.z);
        Instantiate(ImplosionStart,SpawnPosition,Quaternion.Euler(0f, 0f, 0f));
        yield return new WaitForSeconds(0.75f);
        Instantiate(ImplosionEnd,SpawnPosition,Quaternion.Euler(0f, 0f, 0f));
        Destroy(gameObject);
    }
}
