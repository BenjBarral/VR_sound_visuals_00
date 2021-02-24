using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveParticles : MonoBehaviour
{

    public ParticleSystem particleSystem;
    public SkinnedMeshRenderer skin;
    Mesh mesh;
    int updateCount = 0;
    Vector3[] particlePositions;
    float lapsedTimeNormalised = 0;
    float timeToReachTarget = 15;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lapsedTimeNormalised += Time.deltaTime / timeToReachTarget;
        if (updateCount == 0) {
            updateCount++;
        } else if (updateCount == 1) {
            int particleCount = particleSystem.particleCount;
            System.Random random = new System.Random();
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
            particleSystem.GetParticles(particles);
            particlePositions = new Vector3[particleCount];
            for (int i = 0; i < particleCount; i++)
            {
                particlePositions[i] = new Vector3((float)random.NextDouble() * 5f, ((float)random.NextDouble() * 5f) - 1.5f, ((float)random.NextDouble() * 5f) + 3.5f);
                particles[i].position = particlePositions[i];
            }
            particleSystem.SetParticles(particles, particleCount);
            updateCount++;
        }
    }

    void LateUpdate() {
        int particleCount = particleSystem.particleCount;
        mesh = new Mesh();
        skin.BakeMesh(mesh);
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleCount];
        particleSystem.GetParticles(particles);
        int meshVertexIterator = (int)System.Math.Round(mesh.vertices.Length / (double)particleCount) - 1;
        // Debug.Log(mesh.vertices.Length);
        for (int i = 0; i < particleCount; i++)
        {
            particles[i].position = Vector3.Lerp(particlePositions[i], transform.TransformPoint(mesh.vertices[(meshVertexIterator) * i]), lapsedTimeNormalised);
        }
        particleSystem.SetParticles(particles, particleCount);
    }
}
