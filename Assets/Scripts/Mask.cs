using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    [Tooltip("Distance to [Plumber] after which {Mask} wakes up")]
    public float AwakeningProximity = 8f;
    [Tooltip("MovementSpeed of {Mask} towards [Plumber]")]
    public float MoveSpeed = 1f;

    private bool IsAwake = false;

    private Rigidbody2D rBody;
    private Transform plumberTransform;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        plumberTransform = GameObject.Find("Plumber").GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistancePlumber();


        if (IsAwake)
        {
            FollowPlumber();
        }
    }

    private void CheckDistancePlumber()
    {
        Vector2 distance = plumberTransform.position - rBody.transform.position;
        if (distance.magnitude <= AwakeningProximity)
            SetAwake(true);
        else
            SetAwake(false);
    }

    private void FollowPlumber()
    {
        Vector2 segment = plumberTransform.position - rBody.transform.position;
        rBody.velocity = segment.normalized * MoveSpeed;
    }

    /// <summary>
    /// Sets the [IsAwake] of Script & Animation, if it differs
    /// </summary>
    /// <param name="newAwake">Parameter value to pass.</param>
    /// <returns>Returns void.</returns>
    public void SetAwake(bool newAwake)
    {
        if (newAwake != IsAwake) {
            IsAwake = newAwake;
            animator.SetBool("IsAwake", IsAwake);
            //stop the old velocity
            if (IsAwake == false)
                rBody.velocity = Vector2.zero;
        }
    }

}
