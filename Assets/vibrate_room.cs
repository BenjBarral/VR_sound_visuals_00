using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vibrate_room : MonoBehaviour
{
    public float speed = 1000;
    private bool is_opening = false;
    private bool is_closing = false;
    List<Vector3> directions;
    List<float> initialDist;


    public AudioSource audio_source;



    public float time_offset = 0f;
    public float t1 = 46.08f;
    public float t2 = 53.76f;


    public float prob_vibrate = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        directions = new List<Vector3>();
        initialDist = new List<float>();
        foreach (Transform child in transform)
        {
            Vector3 child_pos = child.gameObject.transform.position;
            Vector3 direction = child_pos.normalized;
            Debug.Log(direction);
            directions.Add(direction);
            initialDist.Add(child_pos.magnitude);

        }
    }

    void openRoom()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            Transform child_transform = child.gameObject.transform;
            Vector3 dir_child = directions[i];
            child_transform.Translate(dir_child * speed * Time.deltaTime, Space.World);
            i++;
        }
    }
    void closeRoom()
    {
        int i = 0;
        foreach (Transform child in transform)
        {
            Transform child_transform = child.gameObject.transform;
            if (child_transform.position.magnitude < initialDist[i])
            {
                is_closing = false;
                break;
            }
            Vector3 dir_child = directions[i];
            child_transform.Translate(-dir_child * speed * Time.deltaTime, Space.World);
            i++;
        }
    }

    void detectMovement()
    {
        float sampled_prob = Random.Range(0f, 1f);
        if (sampled_prob < prob_vibrate)
        {
            if (is_opening)
            {
                is_opening = false;
                is_closing = true;
            }
            else if (is_closing)
            {
                is_opening = true;
                is_closing = false;
            }
            else
            {
                is_opening = true;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

        float current_time = audio_source.time - time_offset;
        if (current_time > t1 && current_time < t2)
        {
            this.detectMovement();
            if (is_opening)
            {
                this.openRoom();
            }
            if (is_closing)
            {
                this.closeRoom();
            }
        }
    }
}
