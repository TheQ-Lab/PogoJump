using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour
{
    [Header("Constants")]
    [Tooltip("Startgeschwindigkeit")]
    public float velocityStart = 0.8f;
    [Tooltip("Endgeschwindigkeit")]
    public float velocityFinal = 2f;
    [Tooltip("Inkrement, um den die Acid pro geschafftem Modul bis [VelocityFinal] schneller wird")]
    public float incrementVelocity = 0.1f;
    [Tooltip("y-Distanz zum [Plumber], nach der die Acid sich hinterher teleportieren wird")]
    public float maxDistanceToPlumber = 25f;

    private Rigidbody2D rBody;
    private Transform plumber;
    private Transform highestPoint; //Transform saves reference, Vector3 only raw numbers, so this is always up to date
    [Header("Internals, public for ease of access from other scrips or debugging")]
    public float velocity = 0f;
    public bool isRising = false;


    void Start()
    {
        velocity = velocityStart;
        plumber = GameObject.Find("Plumber").GetComponent<Transform>();
        rBody = GetComponent<Rigidbody2D>();
        highestPoint = this.transform.Find("HighestPoint");
    }

    private void Update()
    {
        if (isRising)
            CloseUpToPlayer();
    }

    private void CloseUpToPlayer()
    {
        float yThis = highestPoint.position.y;
        float yPlumber = plumber.position.y;
        if (yPlumber - yThis > maxDistanceToPlumber)
            rBody.position = new Vector2(rBody.position.x, yPlumber - maxDistanceToPlumber);
    }

    public void RaiseVelocityByIncrement(int Increments)
    {
        float newVelocity = velocity + Increments * incrementVelocity;
        newVelocity = Mathf.Clamp(newVelocity, 0f, velocityFinal);
        SetVelocity(newVelocity);
    }

    public void SetVelocity(float newVelocity)
    {
        velocity = newVelocity;
        rBody.velocity = new Vector2(0f, velocity); ;
    }

    public void StartRising()
    {
        rBody.velocity = new Vector2(0f, velocity);
        isRising = true;
    }

    public void StopRising()
    {
        rBody.velocity = Vector2.zero;
        isRising = false;
    }

    public bool GetRising()
    {
        return isRising;
    }
}
