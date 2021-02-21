using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stretch_room : MonoBehaviour
{
    public float speed = 10f;
    private bool is_opening = false; 
    private bool is_closing = false; 
     List<Vector3> directions;
    List<float> initialDist;

    // Effect BPM

    public float bpm = 31.25f;
    public float time_offset = 0f;
    private float delta_time_bpm;
    private float t0_detect;
    public float t1 = 53.76f;
    public float t2 = 92.16f;
    public float t3 = 142.08f;


    public AudioSource audio_source;



    public float prob_vibrate = 0.2f;
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

        delta_time_bpm = 60f / bpm;
        t0_detect = 0f;
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

    void detectMovementBPM()
    {

        float current_time = audio_source.time - time_offset;
        bool trigger = false;
        if (current_time - t0_detect > delta_time_bpm)
        {
            t0_detect = current_time;
            trigger = true;
        }
        if (trigger)
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

    void detectMovementKeys()
    {
        bool open = Input.GetKey("up");
        bool close = Input.GetKey("down");
        if (open)
        {
            if (!is_opening)
            {
                is_opening = true;
            }
            if (is_closing)
            {
                is_closing = false;
            }
        }
        if (close)
        {
            if (is_opening)
            {
                is_opening = false;
            }
            if (!is_closing)
            {
                is_closing = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        float current_time = audio_source.time - time_offset;
        if (current_time > t1 && current_time < t2)
        {
            this.detectMovementBPM();
        }
        if (current_time > t2 && current_time < t3 && is_opening)
        {
            is_closing = true;
            is_opening = false;
        }
        if (current_time > t3 && !is_opening)
        {
            is_closing = false;
            is_opening = true;
        }

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
