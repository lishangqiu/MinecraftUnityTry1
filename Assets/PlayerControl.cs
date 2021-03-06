using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

static class ExtensionMethods
{
    /// <summary>
    /// Rounds Vector3.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="decimalPlaces"></param>
    /// <returns></returns>
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
}

    public class PlayerControl : MonoBehaviour
{
    public float slideMovement = 8;
    public float jumpMovement = 1f;

    private Rigidbody rb;
    private List<int> moveType = new List<int>();
    public Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) && (!moveType.Contains(1)))
        {
            moveType.Add(1);
        }

        if (Input.GetKey(KeyCode.RightArrow) && (!moveType.Contains(2)))
        {
            moveType.Add(2);
        }
        if (Input.GetKey(KeyCode.Space) && checkOnGround())
        {
            moveType.Add(3);
        }
        if ((Input.GetKey("escape")))
        {
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
        if (transform.position.y > -20)
        {
            //Debug.Log(currOnGround);
            if (moveType.Contains(1))
            {
                if (Math.Abs(rb.velocity.x) < PlayerSettings.PlayerMaxSpeed)
                {
                    rb.AddForce(-slideMovement, 0, 0, ForceMode.VelocityChange);
                }
                moveType.Remove(1);
            }
            if (moveType.Contains(2))
            {
                if (Math.Abs(rb.velocity.x) < PlayerSettings.PlayerMaxSpeed)
                {
                    rb.AddForce(slideMovement, 0, 0, ForceMode.VelocityChange);
                }
                moveType.Remove(2);
            }
            if (moveType.Contains(3))
            {
                rb.AddForce(0, jumpMovement, 0, ForceMode.VelocityChange);
                moveType.Remove(3);
            }
        }
    }
    
    public bool checkOnGround()
    {
        int[] a = new int[] { (int) Math.Floor((double) transform.position.x), (int) Math.Ceiling((double)transform.position.x) };
        int[] b = new int[] { (int) Math.Floor((double) transform.position.z), (int) Math.Ceiling((double)transform.position.z) };

        foreach (int x in a)
        {
            foreach(int z in b)
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
}

public static class PlayerSettings
{
    public static int PlayerMaxSpeed = 10;
}