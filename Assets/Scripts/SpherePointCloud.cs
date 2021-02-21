using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePointCloud : MonoBehaviour
{
    public GameObject myPrefab;
    public int num_spheres_per_row_min = 5;
    public int num_spheres_per_row_max = 200;
    public int num_rows = 50;
    public float shape_radius = 10f;
    public float point_radius = 0.1f;
    List<GameObject> objectList;
    List<Vector3> localShapePositions;
    List<Vector3> localShapePositionsInitial;
    List<float> listPhi=new List<float>();
    List<float> listTheta = new List<float>();
    private int total_points;

    // Movement
    public AudioSource audioSource;
    public float t0_initial_formation = 13.44f;
    private float t0_move;


    // Wave effects
    public float wave_duration = 0.25f;
    private float t0_wave;
    private bool wave_effect_active=false;
    Vector2 wave_effect_angle;
    private Vector3 wave_origin_position;
    public float range_wave_effect = 0.7f;
    private List<int> indices_points_range;
    private List<float> ratio_points_range;
    public float wave_effect_extension =0.75f;
    private float t0_detect = 0f;
    public float t1_trigger_effect=38.76f;
    public float bpm = 31.25f;
    private float delta_time_bpm;
    // Start is called before the first frame update
    void Start()
    { 
        objectList = new List<GameObject>();
        localShapePositions = new List<Vector3>();
        localShapePositionsInitial = new List<Vector3>();
        int half_row = (int)(num_rows / 2);
        for (int i = 0; i < num_rows; i++)
        {
            float phi = Mathf.PI * (i + 0.5f) / num_rows;
            int num_spheres_per_row;
            if (i < half_row)
            {
                num_spheres_per_row = (int)(num_spheres_per_row_min + i * (num_spheres_per_row_max - num_spheres_per_row_min) / (num_rows / 2));
            }
            else
            {
                num_spheres_per_row = (int)(num_spheres_per_row_min + (num_rows-i-1) * (num_spheres_per_row_max - num_spheres_per_row_min) / (num_rows / 2));
            }
            
            for (int j = 0; j < num_spheres_per_row; j++)
            {
                float theta = 2.0f * Mathf.PI * (j + 0.5f) / num_spheres_per_row;
                listPhi.Add(phi);
                listTheta.Add(theta);
                GameObject sphere_i = Instantiate(myPrefab, transform);
                sphere_i.transform.localScale = new Vector3(point_radius, point_radius, point_radius);
                float y = shape_radius * Mathf.Cos(phi);
                float x = shape_radius * Mathf.Sin(phi) * Mathf.Cos(theta);
                float z = shape_radius * Mathf.Sin(phi) * Mathf.Sin(theta);
                Vector3 local_shape_position_final = new Vector3(x, y, z);
                Vector3 displacement = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                Vector3 local_shape_position_initial = local_shape_position_final + 4.0f*shape_radius * displacement;
                localShapePositions.Add(local_shape_position_final);
                localShapePositionsInitial.Add(local_shape_position_initial);
                sphere_i.transform.localPosition = local_shape_position_initial;
                objectList.Add(sphere_i);
            }
        }
        total_points = objectList.Count;
        t0_move = audioSource.time;


        delta_time_bpm = 60f / bpm;
    }

    void MovePointsToFinalPosition(float delta_time)
    {
        float lambda_interp =Mathf.Pow(delta_time / t0_initial_formation, 0.3f);
        for (int k = 0; k < total_points; k++)
        {
            GameObject gameObject = objectList[k];
            Vector3 initialPosition = localShapePositionsInitial[k];
            Vector3 targetPosition = localShapePositions[k];
            Vector3 positionInterp = lambda_interp * targetPosition + (1.0f - lambda_interp) * initialPosition;
            
            gameObject.transform.localPosition = positionInterp;
        }
    }

    bool TestAngleRange(float angle, float angle_0, float range_wave_effect, float lim_PI)
    {
        float angle_min = angle_0 - range_wave_effect;
        float angle_max = angle_0 + range_wave_effect;

        bool result = angle > angle_min || angle > lim_PI + angle_min; // 
        result = result && (angle < angle_max || angle < angle_max-lim_PI); // 
        return result;
    }

    bool TestPhiThetaRange(float phi, float theta, float phi_effect, float theta_effect)
    {
        //bool result = phi < phi_effect + range_wave_effect * Mathf.PI && phi > phi_effect - range_wave_effect * Mathf.PI;
        //result = result && (theta < theta_effect + range_wave_effect * Mathf.PI && theta > theta_effect - range_wave_effect * Mathf.PI);
        float range_wave_effect_euler = range_wave_effect * Mathf.PI;
        bool result = TestAngleRange(phi, phi_effect, range_wave_effect_euler, Mathf.PI);
        result = result && TestAngleRange(theta, theta_effect, range_wave_effect_euler, Mathf.PI);
        return result;
    }

    float TestPositionRatio(Vector3 origin_position, Vector3 point_position, float range_distance)
    {
        float result= (range_distance-Vector3.Distance(origin_position, point_position)) / range_distance;
        if (result < 0)
        {
            return -1;
        }
        else
        {
            return Mathf.Pow(result, 2);
        }
    }

    void SampleWaveEffectAngle()
    {
        float theta_effect = Random.Range(0f, 1f) * Mathf.PI;
        float phi_effect = Random.Range(0f, 1f) * 2 * Mathf.PI;

        float y = shape_radius * Mathf.Cos(phi_effect);
        float x = shape_radius * Mathf.Sin(phi_effect) * Mathf.Cos(theta_effect);
        float z = shape_radius * Mathf.Sin(phi_effect) * Mathf.Sin(theta_effect);
        wave_origin_position = new Vector3(x, y, z);

        wave_effect_angle = new Vector2(theta_effect, phi_effect);
        indices_points_range = new List<int>();
        ratio_points_range = new List<float>();
        for (int k = 0; k < total_points; k++)
        {
            //float theta = listTheta[k];
            //float phi = listPhi[k];
            //if (TestPhiThetaRange(phi, theta, phi_effect, theta_effect))
            //{
            float ratio_point_effect = TestPositionRatio(wave_origin_position, objectList[k].transform.localPosition, range_wave_effect * shape_radius);
            if (ratio_point_effect > 0) 
            {
                indices_points_range.Add(k);
                ratio_points_range.Add(ratio_point_effect);
                //objectList[k].SetActive(false);
            }
        }
    }

    float FuncTimeCoeff(float delta_time, float wave_duration)
    {
        float half_wave_duration = wave_duration / 2f;
        float result;
        if (delta_time < half_wave_duration)
        {
            result = Mathf.Pow(delta_time / half_wave_duration, 2f);
        }
        else
        {
            result = Mathf.Pow((wave_duration - delta_time) / half_wave_duration, 2f);
        }
        return result;
    }

    void WaveEffectUpdate(float current_time)
    {
        float delta_time = current_time - t0_wave;
        float theta_effect = wave_effect_angle.x;
        float phi_effect = wave_effect_angle.y;


        if (delta_time > wave_duration)
        {
            //foreach (int k in indices_points_range)
            //{
            //    objectList[k].SetActive(true);
            //}
            wave_effect_active = false;
            foreach (int k in indices_points_range)
            {
                objectList[k].transform.localPosition = localShapePositions[k];
            }
            return;
        }

        float time_coeff = FuncTimeCoeff(delta_time, wave_duration);
        Debug.Log(time_coeff);

        int index = 0;
        foreach (int k in indices_points_range)
        {
            GameObject point_k =objectList[k];
            Vector3 canonicPosition = localShapePositions[k];
            Vector3 normal = canonicPosition.normalized;
            //float ratio_phi = (range_wave_effect * Mathf.PI - Mathf.Abs(listPhi[k] - phi_effect)) / (range_wave_effect * Mathf.PI);
            //float ratio_theta = (range_wave_effect * Mathf.PI - Mathf.Abs(listTheta[k] - theta_effect)) / (range_wave_effect * Mathf.PI);
            //float ratio_angle = (ratio_phi + ratio_theta) / 2.0f;
            float ratio_point_range = ratio_points_range[index];
            index++;
            Vector3 effect_position = canonicPosition + normal * wave_effect_extension * shape_radius * ratio_point_range * time_coeff;
            point_k.transform.localPosition = effect_position;
        }

    }

    bool TestBPM(float current_time)
    {
        bool result = false;
        if (current_time > t1_trigger_effect)
        {
            if (current_time - t0_detect > delta_time_bpm){
                t0_detect = current_time;
                result = true;
            }
        }
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        float current_time = audioSource.time;
        if (current_time <= t0_initial_formation)
        {
            MovePointsToFinalPosition(current_time);
        }
        if (TestBPM(current_time))
        {
            wave_effect_active = true;
            t0_wave = current_time;
            SampleWaveEffectAngle();
        }
        if (wave_effect_active)
        {
            WaveEffectUpdate(current_time);
        }
    }
}
