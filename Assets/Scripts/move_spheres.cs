using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text;
using System.IO;

public class move_spheres : MonoBehaviour
{

    public GameObject myPrefab;
    public int numSpheres = 10;
    public float radius0 = 10f;
    public float radius = 10f;
    public float z0 = -5f;
    public float rotationSpeed = 0.1f;
    public float radiusSpeed = 1f;


    List<GameObject> objectList;    

    // Start is called before the first frame update
    void Start()
    {
        radius = radius0;
        objectList = new List<GameObject>();
        for (int i = 0; i < numSpheres; i++)
        {
            float angle = 2 * Mathf.PI * i / numSpheres;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            GameObject sphere_i = Instantiate(myPrefab, new Vector3(x, y, z0), Quaternion.identity);
            sphere_i.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            objectList.Add(sphere_i);
        }

    }

    // Update is called once per frame
    void Update()
    {
        radius = radius0 * (1f + 0.5f * Mathf.Sin(radiusSpeed * Time.time));

        for (int i = 0; i < numSpheres; i++)
        {
            GameObject gameObject = objectList[i];
            float angle = 2 * Mathf.PI * ((float)i / numSpheres + rotationSpeed * Time.time);
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            gameObject.transform.position = new Vector3(x, y, z0);
            //gameObject.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
