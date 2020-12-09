using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlumberInput : MonoBehaviour
{
    //links
    private UiArrow launchArrow;
    //constants
    //public float MaxPower = 500f; is in plumber
    public float MouseSensitivity = 0.1f;

    //predefineds
    private Plumber plumber;
    //locales
    private Vector2 mousePosStart, mousePosEnd;
    private float launchPower, launchAngle;
    private bool lockLaunch = false;

    private void Start()
    {
        plumber = GetComponent<Plumber>();

        launchArrow = Resources.FindObjectsOfTypeAll<UiArrow>()[0]; //if I want to find an inactive GameObject with a certain class with known/uniquie Component, else search throug array
        launchArrow.Activate();
        launchArrow.SetMaxPower(plumber.MaxPower);
        launchArrow.Deactivate();
    }

    private void Update()
    {
        if (plumber.attached)
        {
            InputHandlingFrogger();
        }
    }

    private void FixedUpdate()
    {
        if (plumber.attached)
        {
            FixedInputHandlingFrogger();
        }
    }

    private void InputHandling()
    {
        if (lockLaunch && !Input.GetMouseButton(0))
        {
            //if whanna feel more nice to player remove requirement for plumber.attatched
            lockLaunch = false;
        }
        if (!lockLaunch)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Plumber down");
                mousePosStart = Camera.main.WorldToScreenPoint(Input.mousePosition);
                //launchArrow.Activate();
            }
            if (Input.GetMouseButton(0))
            {
                mousePosEnd = Camera.main.WorldToScreenPoint(Input.mousePosition);
                Vector2 resultingForce = (mousePosStart - mousePosEnd) * MouseSensitivity;
                launchAngle = Vector2.SignedAngle(new Vector2(0, 100), resultingForce);
                launchPower = Mathf.Min(resultingForce.magnitude, plumber.MaxPower);

                if (launchPower > 0)
                {
                    launchArrow.Activate();
                    launchArrow.SetAngle(launchAngle);
                    launchArrow.SetPower(launchPower);
                }

                plumber.orientArm(launchAngle);
            }
            if (Input.GetMouseButtonUp(0))
            {
                if (launchPower > 0.01f * plumber.MaxPower) plumber.LaunchCharacter(launchAngle, launchPower);
                launchArrow.Deactivate();
                lockLaunch = true;
            }
        }
    }

    bool inShooting = false;
    private void InputHandlingFrogger()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosStart = Camera.main.WorldToScreenPoint(this.transform.position);
            launchArrow.Activate();
            launchArrow.SetPower(launchPower);
            inShooting = true;
        }
        if (Input.GetMouseButton(0))
        {
            mousePosEnd = Input.mousePosition;
            Vector2 resultingForce = (mousePosEnd- mousePosStart);
            launchAngle = Vector2.SignedAngle(new Vector2(0, 100), resultingForce);

            launchArrow.SetAngle(launchAngle);
            plumber.orientArm(launchAngle);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (launchPower>0.01f*plumber.MaxPower)plumber.LaunchCharacter(launchAngle, launchPower);
            launchArrow.Deactivate();
            inShooting = false;
            launchPower = 0;
            countingReverse = false;
        }
    }

    bool countingReverse = false;
    private void FixedInputHandlingFrogger()
    {
        if (inShooting)
        {
            if (launchPower >= plumber.MaxPower)
            {
                countingReverse = true;
            }
            else if (launchPower <= 0)
            {
                countingReverse = false;
            }

            if (!countingReverse)
            {
                launchPower+=30;
                launchPower = Mathf.Min(plumber.MaxPower, launchPower);
            }
            else if (countingReverse)
            {
                launchPower-=30;
                launchPower = Mathf.Max(0, launchPower);
            }

            launchArrow.SetPower(launchPower);
        }
    }

    public void LockLaunch()
    {
        lockLaunch = true;

        //v2: Frogger
        inShooting = false;
        launchPower = 0;
        countingReverse = false;

        launchArrow.Deactivate();
    }

    public void SetMaxLaunchArrow(float maxPow, UiArrow.ArrowColors color)
    {
        launchArrow.SetMaxPower(maxPow);
        launchArrow.SetColor(color);
    }
}
