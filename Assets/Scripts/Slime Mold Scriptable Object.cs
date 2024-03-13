using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "data", order = 1)]
public class GameData : ScriptableObject
{
    [Header("Points Settings")]
    public int num_of_points;

    public int speed_of_points;

    public int sensor_offset;


    [Header("Texture Settings")]
    public int texture_width;
    public int texture_height;

    [Header("Trail Settings")]
    public float trail_diffuse_amount;
    public float trail_blur_amount;
    public float attraction_value;


    [Header("Sensor Settings")]
    public int sensor_distance;
    public float sensor_angle_offset;
    public int sensor_size;
}
