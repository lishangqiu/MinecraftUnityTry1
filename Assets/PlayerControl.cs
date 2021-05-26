using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

static class ExtensionMethods
{
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }

    public static bool ValInside(this Vector3 vector3, int valueCheck)
    {
        if (valueCheck == vector3.x || valueCheck == vector3.y || valueCheck == vector3.z) return true;
        return false;
    }

    public static Vector3 Abs(this Vector3 vector3)
    {
        return new Vector3(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y), Mathf.Abs(vector3.z));
    }
}

public static class keyStates
{
    public static bool LEFT { get; private set; }
    public static bool RIGHT { get; private set; }
    public static bool FORWARD { get; private set; }
    public static bool BACKWARD { get; private set; }
    public static bool JUMP { get; private set; }

    public static void reset()
    {
        LEFT = RIGHT = FORWARD = BACKWARD = JUMP = false;
    }

    public static void setKeyState(String keyName, bool state)
    {
        switch (keyName)
        {
            case "a":
                LEFT = state;
                break;
            case "d":
                RIGHT = state;
                break;
            case "w":
                FORWARD = state;
                break;
            case "s":
                BACKWARD = state;
                break;
            case " ":
                JUMP = state;
                break;
        }
    }
}

public class PlayerControl : MonoBehaviour
{
    private Rigidbody rb;
    public Controller controller;
    public Vector3 pastDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PhysicTemp.currRB = rb;
    }
    // Update is called once per frame
    void Update()
    {
        keyStates.reset();

        keyStates.setKeyState("a", Input.GetKey("a"));
        keyStates.setKeyState("d", Input.GetKey("d"));
        keyStates.setKeyState("w", Input.GetKey("w"));
        keyStates.setKeyState("s", Input.GetKey("s"));
        keyStates.setKeyState(" ", Input.GetKey(KeyCode.Space));

        if ((Input.GetKey("escape")))
        {
            Application.Quit();
        }
        //Debug.Log("near blocks: "+checkBumpedBlock());
    }

    void FixedUpdate()
    { 
        if (transform.position.y > -20)
        {
            PhysicTemp.resetForce();

            // assign a movement direction to every type of 
            Vector3 movementDirection = Vector3.zero;

            if (keyStates.LEFT) movementDirection += new Vector3(-1, 0, 0);
            if (keyStates.RIGHT) movementDirection += new Vector3(1, 0, 0);
            if (keyStates.FORWARD) movementDirection += new Vector3(0, 0, 1);
            if (keyStates.BACKWARD) movementDirection += new Vector3(0, 0, -1);

            if (keyStates.JUMP) PhysicTemp.addJumpForce();

            if (movementDirection != new Vector3(0, 0, 0))
            {
                PhysicTemp.addGroundForce(gameSettings.slideMovement * movementDirection);
            }

            //Debug.Log(movementDirection);
            PhysicTemp.applyFriction(Vector3.one - movementDirection.Abs()); 
            PhysicTemp.applyForce();
            //if (checkOnGround())
            //{
            //    PhysicTemp.applyFriction();
            //}
            //

            //Debug.Log("force: "+PhysicTemp.currForce);
            //Debug.Log("rb: "+rb.velocity);
        }
    }
    

    public bool checkOnGround()
    {
        int[] allX = new int[] { (int) Math.Floor((double) transform.position.x), (int) Math.Ceiling((double)transform.position.x) }; // in case to player is standing in the middle of 2/4 blocks
        int[] allZ = new int[] { (int) Math.Floor((double) transform.position.z), (int) Math.Ceiling((double)transform.position.z) }; // need to loop over all of them

        foreach (int x in allX)
        {
            foreach(int z in allZ)
            {
                Vector3 blockPos = new Vector3(x, transform.position.y, z);
                Vector3 chunkPos = controller.getChunk(blockPos);
                if (controller.mapDict.ContainsKey(chunkPos))
                {
                    //Debug.Log(blockPos - chunkPos - new Vector3(0, 1.5f, 0));
                    if (controller.mapDict[chunkPos].ContainsKey((blockPos - chunkPos - new Vector3(0,1.5f,0)).Round(2)))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool checkBumpedBlock()
    {
        int[] allX = new int[] { (int)Math.Floor((double)transform.position.x), (int)Math.Ceiling((double)transform.position.x) }; // see all four sides
        int[] allZ = new int[] { (int)Math.Floor((double)transform.position.z), (int)Math.Ceiling((double)transform.position.z) };
        foreach (int x in allX)
        {
            foreach (int z in allZ)
            {
                Vector3 blockPos = new Vector3(x, (int) Math.Floor(transform.position.y), z);
                Vector3 chunkPos = controller.getChunk(blockPos);
                if (controller.mapDict[chunkPos].ContainsKey((blockPos - chunkPos).Round(2)))
                {
                    return true;
                }
            }
        }
        return false;
    }
}


public static class PhysicTemp
{
    public static Vector3 currForce;
    public static Rigidbody currRB;
    public static bool moving = false;
    public static int currCount = 0;


    private static Vector3 MaxVelocity(Vector3 vector3)
    {
        float newX = 0f;
        float newZ = 0f;

        if (vector3.x < 0)
        {
            if (currRB.velocity.x + vector3.x < -gameSettings.PlayerMaxSpeed && currRB.velocity.x < 0) newX = -gameSettings.PlayerMaxSpeed - currRB.velocity.x;
            else newX = vector3.x;
        }
        else
        {
            if (currRB.velocity.x + vector3.x > gameSettings.PlayerMaxSpeed && currRB.velocity.x > 0) newX = gameSettings.PlayerMaxSpeed - currRB.velocity.x;
            else newX = vector3.x;
        }

        if (vector3.z < 0)
        {
            if (currRB.velocity.z + vector3.z < -gameSettings.PlayerMaxSpeed && currRB.velocity.z < 0) newZ = -gameSettings.PlayerMaxSpeed - currRB.velocity.z;
            else newZ = vector3.z;
        }
        else
        {
            if (currRB.velocity.z + vector3.z > gameSettings.PlayerMaxSpeed && currRB.velocity.z > 0) newZ = gameSettings.PlayerMaxSpeed - currRB.velocity.z;
            else newZ = vector3.z;
        }

        return new Vector3(newX, vector3.y, newZ);
    }

    public static void resetForce()
    {
        currForce = Vector3.zero;
        moving = false;
    }

    public static void addGroundForce(Vector3 addedForce)
    {
        currForce = MaxVelocity(currForce + addedForce);
        moving = true;
    }


    public static void addJumpForce()
    {
        currForce += new Vector3(0, gameSettings.jumpMovement, 0);
    }

    public static bool checkPressing()
    {
        if (!moving)
        {
            currCount++;
            if (currCount >= 5)
            {
                currCount = 0;
                return true;
            }
        }
        else
        {
            currCount = 0;
        }
        
        return false;
    }

    public static void applyFriction(Vector3 frictionVector)
    {
        /*if (currForce.x != 0 && currForce.z != 0)
        {
            Debug.LogError("Force exists while calling stop() function");
        }*/
        Vector3 newVelocity = currRB.velocity;
        float x_friction = frictionVector.x * gameSettings.groundFriction;
        float z_friction = frictionVector.z * gameSettings.groundFriction;

        if (x_friction != 0)
        {
            newVelocity.x *= x_friction;
        }
        
        if (z_friction != 0)
        {
            newVelocity.z *= z_friction;
        }
        
        currRB.velocity = newVelocity;

    }

    public static void applyForce()
    {
        currRB.AddForce(PhysicTemp.currForce);
        Debug.Log(PhysicTemp.currForce);
    }
}