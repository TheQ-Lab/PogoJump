using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mask : MonoBehaviour
{
    [Tooltip("Distance to [Plumber] after which {Mask} wakes up")]
    public float AwakeningProximity = 8f;
    [Tooltip("MovementSpeed of {Mask} towards [Plumber]")]
    public float MoveSpeed = 1f;

    //private bool IsDisabled = false; replaced by enable Script
    private bool IsAwake = false;

    private Rigidbody2D rBody;
    public Vector3 originalLocalPosition;
    private Transform plumberTransform, moduleHighestPos, thisPos;
    private Animator animator;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        plumberTransform = GameObject.Find("Plumber").GetComponent<Transform>();
        animator = GetComponent<Animator>();

        moduleHighestPos = transform.parent.Find("HighestPoint").transform;
        thisPos = gameObject.transform;

        
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistancePlumber();

        if (IsAwake)
        {
            FollowPlumber();
        }

        CheckDisable();
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


    private void CheckDisable()
    {
        //LEFT: mask stepping over level boundries  RIGHT: plumber too high above (happens when asleep before hitting boundries) - to save on resources
        if(thisPos.position.y >= moduleHighestPos.position.y || plumberTransform.position.y > thisPos.position.y+20f)
        {
            this.SetAwake(false);
            animator.SetTrigger("Disable");
            this.enabled = false;
        }
    }

    public void Reset()
    {
        transform.localPosition = originalLocalPosition;
        SetAwake(false);
        Debug.Log("Mask reset");
    }

}
