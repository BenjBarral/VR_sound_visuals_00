using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text;
using System.IO;

public class throw_sphere : MonoBehaviour
{

    public GameObject myPrefab;
    public int numSpheres = 100;
    public bool is_running = false;
    public float range_initial_position = 1f;

    // Initial position
    private float x0;
    private float y0;
    private float z0;
    public float distance_0= 10f;

    // Effect trigger
    public float trigger_prob = 0.02f;

    // Sound stuff
    public float bpm = 31.25f;
    public float time_offset = 0f;
    private float delta_time_bpm;
    private float t0_detect=0f;
    public float t1=14.5f;
    public float t2 = 38.76f;

    // Sphere movement
    public float speed=1f;
    private float t0;
    public float time_effect=5f;

    List<GameObject> objectList;
    List<Vector3> directionsList;


    public AudioSource audio_source;

    // Start is called before the first frame update
    void Start()
    {
        objectList = new List<GameObject>();
        directionsList = new List<Vector3>();
        float theta = 2 * Mathf.PI * Random.Range(0f, 1f);
        x0 = distance_0 * Mathf.Cos(theta);
        z0 = distance_0 * Mathf.Cos(theta);
        y0 = Random.Range(-range_initial_position, range_initial_position);
        for (int i = 0; i < numSpheres; i++)
        {
            float angle = 2 * Mathf.PI * i / numSpheres;
            GameObject sphere_i = Instantiate(myPrefab, new Vector3(x0, y0, z0), Quaternion.identity);
            objectList.Add(sphere_i);
            sphere_i.SetActive(false);
        }
        delta_time_bpm = 60f / bpm;
        t0 = 0f;

    }


    bool detectTrigger()
    {

        //float sampled_Value = Random.Range(0f, 1f);
        //if (sampled_Value < trigger_prob)
        //{
        //    change_color = true;
        //}
        float current_time = audio_source.time - time_offset;
        if (current_time - t0_detect > delta_time_bpm)
        {
            t0_detect = current_time;
            return true;
        }
        return false;
    }

    bool detectTriggerRandom()
    {
        if (is_running)
        {
            return false;
        }
        if (Input.GetKey("up")){
            return true;
        }
        float sampled_Value = Random.Range(0f, 1f);
        if (sampled_Value < trigger_prob)
        {
            return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        bool detectTrigger = this.detectTrigger();
        float current_time = audio_source.time;
        if (detectTrigger && (current_time < t1 || current_time > t2))
        {
            is_running = true;
            t0 = current_time;
            float theta = 2 * Mathf.PI * Random.Range(0f, 1f);
            x0 = distance_0 * Mathf.Cos(theta);
            z0 = distance_0 * Mathf.Cos(theta);
            y0 = Random.Range(-range_initial_position, range_initial_position);
            for (int i = 0; i < numSpheres; i++)
            {
                Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                direction = Vector3.Normalize(direction);
                directionsList.Add(direction);
                objectList[i].transform.position = new Vector3(x0, y0, z0);
                objectList[i].SetActive(true);
                objectList[i].GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
        }

        if (is_running)
        {
            for (int i = 0; i < numSpheres; i++)
            {
                GameObject gameObject = objectList[i];
                gameObject.transform.Translate(directionsList[i] * Time.deltaTime * speed);
                //gameObject.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.World);
            }

            if (current_time - t0 > time_effect)
            {
                is_running = false;

                for (int i = 0; i < numSpheres; i++)
                {

                    objectList[i].SetActive(false);
                }
            }
        }
    }
}
