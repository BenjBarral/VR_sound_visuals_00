using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class set_color : MonoBehaviour
{

    // Effect trigger
    public float trigger_prob = 0.02f;
    private bool change_color = false;
    private float t0;
    public float bpm = 62.5f;
    private float delta_time_bpm;

    // Initial color
    public float r0 = 255f;
    public float g0 = 152f;
    public float b0 = 255f;
    public float range_color = 0.1f;

    public float t1 = 31.2f;


    public AudioSource audio_source;


    // Start is called before the first frame update
    void Start()
    {
        r0 = r0 / 255f;
        g0 = g0 / 255f;
        b0 = b0 / 255f;
        delta_time_bpm = 60f / bpm;
        t0 = 0f;

    }

    void detectTrigger()
    {

        //float sampled_Value = Random.Range(0f, 1f);
        //if (sampled_Value < trigger_prob)
        //{
        //    change_color = true;
        //}
        float current_time = audio_source.time;
        if (current_time - t0 > delta_time_bpm)
        {
            change_color = true;
            t0 = current_time;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float current_time = audio_source.time;
        if (current_time > t1)
        {
            this.detectTrigger();
            if (change_color)
            {
                float r = range_color * Random.Range(-1f, 1f) + r0;
                float g = range_color * Random.Range(-1f, 1f) + g0;
                float b = range_color * Random.Range(-1f, 1f) + b0;
                GetComponent<Light>().color = new Color(r, g, b);
                change_color = false;
            }
        }
        
    }
}
