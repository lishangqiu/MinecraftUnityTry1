using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gameSettings
{
    //landscape gen settings
    public const int length = 16;
    public const int width = 16;
    //public const int height = 1;

    public const int minRenderDistance = 2; // It is the actual distance divided by 2
    public const int maxRenderDistance = 8;
    // Noise Settings
    public const float noiseConcentration = 4;
    public const float noiseMultiplication = 3;
    public const FastNoiseLite.NoiseType noiseType = FastNoiseLite.NoiseType.Perlin;


    //player controls settings
    public const float slideMovement = 17;
    public const float jumpMovement = 150f;
    public const float groundFriction = 0.65f;
    public const float PlayerMaxSpeed = 5f;
    public const float breakCount = 3f;

} 