using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyantObject : MonoBehaviour
{
    private readonly Color red = new(0.92f, 0.25f, 0.2f);
    private readonly Color green = new(0.2f, 0.92f, 0.51f);
    private readonly Color blue = new(0.2f, 0.67f, 0.92f);
    private readonly Color orange = new(0.97f, 0.79f, 0.26f);

    [Header("Water")]
    [SerializeField] private float waterHeight = 0.0f;
    
    [Header("Waves")]
    [SerializeField] float steepness;
    [SerializeField] float wavelength;
    [SerializeField] float speed;
    [SerializeField] float[] directions = new float[4];

    [Header("Buoyancy")]
    [Range(1, 5)] public float strength = 1f;
    [Range(0.2f, 5)] public float objectDepth = 1f;

    public float velocityDrag = 0.99f;
    public float angularDrag = 0.5f;
    
    [Header("Effectors")]
    public Transform[] effectors;

    private Rigidbody rb;
    private Vector3[] effectorProjections;
    
    private static Vector3 GerstnerWave(Vector3 position, float steepness, float wavelength, float speed, float direction)
    {
        direction = direction * 2 - 1;
        Vector2 d = new Vector2(Mathf.Cos(Mathf.PI * direction), Mathf.Sin(Mathf.PI * direction)).normalized;
        float k = 2 * Mathf.PI / wavelength;
        float a = steepness / k;
        float f = k * (Vector2.Dot(d, new Vector2(position.x, position.z)) - speed * Time.time);

        return new Vector3(d.x * a * Mathf.Cos(f), a * Mathf.Sin(f), d.y * a * Mathf.Cos(f));
    }

    public static Vector3 GetWaveDisplacement(Vector3 position, float steepness, float wavelength, float speed, float[] directions)
    {
        Vector3 offset = Vector3.zero;

        offset += GerstnerWave(position, steepness, wavelength, speed, directions[0]);
        offset += GerstnerWave(position, steepness, wavelength, speed, directions[1]);
        offset += GerstnerWave(position, steepness, wavelength, speed, directions[2]);
        offset += GerstnerWave(position, steepness, wavelength, speed, directions[3]);

        return offset;
    }
    
    private void Awake()
    {
        // Get rigidbody
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        effectorProjections = new Vector3[effectors.Length];
        for (var i = 0; i < effectors.Length; i++) effectorProjections[i] = effectors[i].position;
    }

    private void OnDisable()
    {
        rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        var effectorAmount = effectors.Length;

        for (var i = 0; i < effectorAmount; i++)
        {
            var effectorPosition = effectors[i].position;

            effectorProjections[i] = effectorPosition;
            effectorProjections[i].y = waterHeight + GetWaveDisplacement(effectorPosition, steepness, wavelength, speed, directions).y;

            // gravity
            rb.AddForceAtPosition(new Vector3(0.0f, -10.2f, 0.0f) / effectorAmount, effectorPosition, ForceMode.Acceleration);

            var waveHeight = effectorProjections[i].y;
            var effectorHeight = effectorPosition.y;

            if (!(effectorHeight < waveHeight)) continue; // submerged
            
            var submersion = Mathf.Clamp01(waveHeight - effectorHeight) / objectDepth;
            var buoyancy = Mathf.Abs(Physics.gravity.y) * submersion * strength;

            // buoyancy
            rb.AddForceAtPosition(Vector3.up * buoyancy, effectorPosition, ForceMode.Acceleration);

            // drag
            rb.AddForce(-rb.velocity * (velocityDrag * Time.fixedDeltaTime), ForceMode.VelocityChange);

            // torque
            rb.AddTorque(-rb.angularVelocity * (angularDrag * Time.fixedDeltaTime), ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        if (effectors == null) return;

        for (var i = 0; i < effectors.Length; i++)
        {
            if (!Application.isPlaying && effectors[i] != null)
            {
                Gizmos.color = green;
                Gizmos.DrawSphere(effectors[i].position, 0.06f);
            }

            else
            {
                if (effectors[i] == null) return;

                Gizmos.color = effectors[i].position.y < effectorProjections[i].y ? red : green; // submerged

                Gizmos.DrawSphere(effectors[i].position, 0.06f);

                Gizmos.color = orange;
                Gizmos.DrawSphere(effectorProjections[i], 0.06f);

                Gizmos.color = blue;
                Gizmos.DrawLine(effectors[i].position, effectorProjections[i]);
            }
        }
    }
}
