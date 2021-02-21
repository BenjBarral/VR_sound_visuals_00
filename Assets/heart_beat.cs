using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heart_beat : MonoBehaviour
{

    // Effect trigger
    public float trigger_prob = 0.02f;
    private bool is_running = false;
    private float t0;
    public float bpm = 125f;
    private float delta_time_bpm;
    private Vector3 scale_0;
    public float duration_effect = 0.3f;
    public float coef_scale = 500f;
    public float power_scale = 4f;
    public float t1 = 14.5f;

    public AudioSource audio_source;

    public float time_offset = 0f;

    // Movement
    public float speed_movement = 1f;
    public GameObject camera;
    public float dist_camera;
    private float dist_camera_0;
    private Vector3 dir_movement;

    // Opposite translation
    public float bpm_translaton = 31.25f;
    private float delta_time_bpm_translation;
    public float t1_translation = 24.72f;
    private float t0_trans = 0f;
    bool is_lerping = false;
    Vector3 start_lerp, end_lerp;
    public float duration_lerp = 0.25f;


    // Start is called before the first frame update
    void Start()
    {
        delta_time_bpm = 60f / bpm;
        delta_time_bpm_translation = 60f / bpm_translaton;
        t0 = 0f;
        scale_0 = this.transform.localScale;
        Vector3 localX = new Vector3();
        Vector3 localY = new Vector3();
        Vector3 sphereMinusCam = this.transform.position - this.camera.transform.position;
        Vector3 sphereMinusCamNorm = Vector3.Normalize(sphereMinusCam);
        dist_camera_0 = (sphereMinusCam).magnitude;
        Vector3.OrthoNormalize(ref sphereMinusCamNorm, ref localX, ref localY);
        Vector3 dir_movement = Random.Range(-1f, 1f) * localX + Random.Range(-1f, 1f) * localY;
        dir_movement.Normalize();
    }

    void detectTrigger()
    {

        //float sampled_Value = Random.Range(0f, 1f);
        //if (sampled_Value < trigger_prob)
        //{
        //    is_running = true;
        //}
        float current_time = audio_source.time - time_offset;
        if (current_time - t0 > delta_time_bpm)
        {
            is_running = true;
            t0 = current_time;
        }

    }
    void detectTriggerKey()
    {
        if (Input.GetKey("up"))
        {
            is_running = true;
        }
    }

    void randomMove()
    {
        Vector3 camPos = camera.transform.position;
        Vector3 sphereMinusCam = this.transform.position - camPos;
        dist_camera = sphereMinusCam.magnitude;
        Vector3 sphereMinusCamNorm = Vector3.Normalize(sphereMinusCam);
        //Vector3 localX = new Vector3();
        Vector3 localY = new Vector3();
        Vector3.OrthoNormalize(ref sphereMinusCamNorm, ref dir_movement, ref localY);
        //Vector3 dir = Random.Range(-1f, 1f) * localX + Random.Range(-1f, 1f) * localY;
        dir_movement = Random.Range(0.7f, 1f) * dir_movement + Random.Range(-0.25f,0.25f) * localY - Random.Range(0f, 0.2f)* (dist_camera- dist_camera_0)* sphereMinusCamNorm;
        dir_movement.Normalize();
        this.transform.Translate(speed_movement * dir_movement * Time.deltaTime);

    }

    void lerpMove(float current_time)
    {
        float interpolationRatio = (current_time - t0_trans) / duration_lerp;
        if (interpolationRatio > 1f)
        {
            is_lerping = false;
            return;
        }
        Vector3 interpolatedPosition = Vector3.Lerp(start_lerp, end_lerp, interpolationRatio);
        this.transform.position = interpolatedPosition;
    }
    // Update is called once per frame
    void Update()
    {
        float current_time = audio_source.time - time_offset;
        if (!is_running && current_time > t1)
        {
            this.detectTrigger();
        }
        float delta_time = current_time - t0;
        if (is_running && delta_time < duration_effect)
        {
            if (delta_time < duration_effect / 2f)
            {
                this.transform.localScale = Mathf.Exp(coef_scale * Mathf.Pow(current_time - t0, power_scale)) * scale_0;
            }
            else
            {
                this.transform.localScale = Mathf.Exp(coef_scale*Mathf.Pow(t0+duration_effect - current_time, power_scale)) * scale_0;
            }
            //this.transform.localScale = Mathf.Exp((duration_effect - current_time)) * scale_0;
        }
        if (is_running && delta_time >= duration_effect)
        {
            is_running = false;
        }
        if (current_time > t1_translation)
        {
            if (current_time - t0_trans > delta_time_bpm_translation)
            {
                t0_trans = current_time;
                end_lerp = 2.0f* camera.transform.position - this.transform.position;
                start_lerp = this.transform.position;
                is_lerping = true;
            }
        }
        if (is_lerping)
        {
            this.lerpMove(current_time);
        }
        else
        {
            this.randomMove();
        }
    }
}
