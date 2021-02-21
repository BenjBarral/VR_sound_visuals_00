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
    private int total_points;

    // Movement
    public AudioSource audioSource;
    public float t0_initial_formation = 13.44f;
    private float t0_move;

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
            //float angle = 2 * Mathf.PI * i / numSpheres;
            //float x = radius * Mathf.Cos(angle);
            //float y = radius * Mathf.Sin(angle);
            //GameObject sphere_i = Instantiate(myPrefab, new Vector3(x, y, z0), Quaternion.identity);
            //objectList.Add(sphere_i);
        }
        total_points = objectList.Count;
        t0_move = audioSource.time;
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

    // Update is called once per frame
    void Update()
    {
        float current_time = audioSource.time;
        float delta_time = current_time - t0_move;
        if (delta_time <= t0_initial_formation)
        {
            MovePointsToFinalPosition(delta_time);
        }
    }
}
