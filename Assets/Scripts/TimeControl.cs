using static System.Math;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class TimeControl : MonoBehaviour
{
    public float deltaSpeed = 0.1f;
    public float deltaStep = 0.01f;
    public int particleCount = 1000;
    private ParticleSystem m_ParticleSystem;
    private ParticleSystem.Particle[] m_Particles;
    private bool m_IsPaused;
    private bool m_IsReversed;
    private float m_CurrentTime;

    private void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
        m_Particles = GenerateParticles();
        EmitParticles();
        m_ParticleSystem.Play();
    }

    private void Update()
    {
        if (m_IsPaused)
        {
            return;
        }
        var deltaTime = Time.deltaTime;
        if (m_IsReversed)
        {
            UpdateTime(-deltaTime);
            EmitParticles();
            m_ParticleSystem.Simulate(m_CurrentTime, true, false);
        }
        else
        {
            UpdateTime(deltaTime);
        }
    }

    private ParticleSystem.Particle[] GenerateParticles()
    {
        var particles = new ParticleSystem.Particle[particleCount];
        m_ParticleSystem.GetParticles(particles, particleCount);
        for (var i = 0; i < particleCount; ++i)
        {
            var particle = particles[i];
            var x = Random.Range(-1f, 1f);
            var y = Random.Range(-1f, 1f);
            var z = Random.Range(-1f, 1f);
            var vx = Random.Range(-1f, 1f);
            var vy = Random.Range(-1f, 1f);
            var vz = Random.Range(-1f, 1f);
            particle.position = new Vector3(x, y, z);
            particle.velocity = new Vector3(vx, vy, vz);
            particles[i] = particle;
        }
        return particles;
    }

    private void EmitParticles()
    {
        m_ParticleSystem.Clear();
        foreach (var particle in m_Particles)
        {
            var emitParams = new ParticleSystem.EmitParams
            {
                position = particle.position,
                velocity = particle.velocity
            };
            m_ParticleSystem.Emit(emitParams, 1);
        }
    }
    
    public void OnPauseSwitch(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (m_IsPaused)
        {
            m_ParticleSystem.Play();
        }
        else
        {
            m_ParticleSystem.Pause();
        }
        m_IsPaused = !m_IsPaused;
    }

    public void OnForward(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (m_IsPaused)
        {
            UpdateTime(deltaStep);
            EmitParticles();
            m_ParticleSystem.Simulate(m_CurrentTime, true, false);
        }
        else
        {
            m_IsReversed = false;
            m_ParticleSystem.Play();
        }
    }
    
    public void OnBackward(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        if (m_IsPaused)
        {
            UpdateTime(-deltaStep);
            EmitParticles();
            m_ParticleSystem.Simulate(m_CurrentTime, true, false);
        }
        else if (!m_IsReversed)
        {
            m_IsReversed = true;
            m_ParticleSystem.Pause();
        }
    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        var main = m_ParticleSystem.main;
        var newSpeed = main.simulationSpeed + deltaSpeed;
        m_CurrentTime *= main.simulationSpeed / newSpeed;
        main.simulationSpeed = newSpeed;
    }

    public void OnSlowDown(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        var main = m_ParticleSystem.main;
        var newSpeed = Max(deltaSpeed, main.simulationSpeed - deltaSpeed);
        m_CurrentTime *= main.simulationSpeed / newSpeed;
        main.simulationSpeed = newSpeed;
    }

    private void UpdateTime(float delta)
    {
        m_CurrentTime += delta;
        if (m_CurrentTime < 0)
        {
            m_CurrentTime = 0;
        }
    }
}
