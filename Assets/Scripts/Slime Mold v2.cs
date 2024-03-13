using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMoldv2 : MonoBehaviour
{
    public ComputeShader shader;
    [SerializeField]
    private RenderTexture texture;


    // data used for each point
    public struct Point
    {
        public Vector2 pos;
        public float angle_in_radians;
    }

    private Point[] data; // data used as the compute buffer


    public GameData gamedata;

    int buffer_size;

    // Start is called before the first frame update
    void Start()
    {
        texture = new RenderTexture(gamedata.texture_width, gamedata.texture_height, 24);
        texture.enableRandomWrite = true;
        texture.Create();

        buffer_size = sizeof(float) * 3;

        shader.SetTexture(0, "Result", texture);

        data = new Point[gamedata.num_of_points];

        for (int i = 0; i < gamedata.num_of_points; i++)
        {
            data[i].pos = new Vector2(Random.Range(0, gamedata.texture_width), Random.Range(0, gamedata.texture_height));
           // data[i].pos = new Vector2(gamedata.texture_width/2, gamedata.texture_height/2);
            data[i].angle_in_radians = Random.value * (2 * Mathf.PI);
        }

        ComputeBuffer buffer = new ComputeBuffer(data.Length, buffer_size);
        buffer.SetData(data);

        shader.SetBuffer(0, "buffer", buffer);
        shader.SetInt("num_of_points", gamedata.num_of_points);
        shader.SetInt("width", gamedata.texture_width);
        shader.SetInt("height", gamedata.texture_height);

        shader.Dispatch(0, gamedata.texture_width / 64, 1, 1);
        buffer.GetData(data);
        buffer.Dispose();



    }

    // Update is called once per frame
    void Update()
    {
        shader.SetTexture(0, "Result", texture);
        shader.SetTexture(2, "Result", texture);
        shader.Dispatch(2, gamedata.texture_width / 8, gamedata.texture_height / 8, 1);


        ComputeBuffer buffer = new ComputeBuffer(data.Length, buffer_size);
        buffer.SetData(data);
        shader.SetBuffer(0, "buffer", buffer);



        shader.SetInt("number_of_points", gamedata.num_of_points);
        shader.SetInt("magnitude", gamedata.speed_of_points);
        shader.SetInt("width", gamedata.texture_width);
        shader.SetInt("height", gamedata.texture_height);
        shader.SetInt("sensor_length", gamedata.sensor_offset);
        shader.SetFloat("trail_diffuse_amount", gamedata.trail_diffuse_amount);
        shader.SetFloat("trail_blur_amount", gamedata.trail_blur_amount);
        shader.SetFloat("attraction_value", gamedata.attraction_value);
        shader.SetFloat("deltatime", Time.deltaTime);
        shader.SetInt("sensor_distance", gamedata.sensor_distance);
        shader.SetInt("sensor_size", gamedata.sensor_size);
        shader.SetFloat("sensor_angle_offset", gamedata.sensor_angle_offset * (2*Mathf.PI));

        shader.Dispatch(0, gamedata.num_of_points/ 64, 1, 1);
        buffer.GetData(data);
        buffer.Dispose();

        
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(texture, destination);
    }
}
