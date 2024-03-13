using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    [SerializeField]
    private RenderTexture texture;
    public ComputeShader shader;



    private struct Point
    {
        public float speed;
        public Vector2 totalposition;
        public float angle;
    }

    private Point[] data;

    [Range(0, 1000000)]
    public int number_of_point;

    [Range(0, 10)]
    public float input_speed;

    [Range(0, 10)]
    public int trailReduction;

    public int width;
    public int height;
    private int size_of_struct;

    [Range(0,1)]
    public float trailBlurAmt;
    [Range(-5,5)]
    public float trailDuration;

    public int sensorLength;

    // Start is called before the first frame update
    void Start()
    {
        texture = new RenderTexture(width, height, 24);
        texture.enableRandomWrite = true;
        texture.Create();

        size_of_struct = sizeof(float) * 4;

        shader.SetTexture(0, "Result", texture);


        data = new Point[number_of_point];

        for (int i = 0; i < number_of_point; i++)
        {
            float xpos = Random.Range(0,texture.width);
            float ypos = Random.Range(0,texture.height);
            data[i].speed = input_speed;
            data[i].totalposition = new Vector2 (xpos, ypos);
            data[i].angle = Random.value * (2*Mathf.PI);

            //Debug.Log(data[i].totalposition);
        }
            
        ComputeBuffer buffer = new ComputeBuffer(data.Length, size_of_struct);
        buffer.SetData(data);

        shader.SetBuffer(0,"buffer", buffer);
        shader.SetInt("number_of_points", number_of_point);
        shader.SetInt("width", width);
        shader.SetInt("height", height);
        
        shader.Dispatch(0,width/64,1,1);
        buffer.GetData(data);
        buffer.Dispose();   
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(texture.width +" "+ texture.height);

        Debug.Log(data[1].totalposition);

        shader.SetTexture(0, "Result", texture);

        shader.SetTexture(1, "Result", texture);
        shader.SetFloat("trailBlurAmt", trailBlurAmt);
        shader.SetFloat("trailDuration", trailDuration);
        shader.SetFloat("deltatime", Time.deltaTime);
        shader.SetInt("sensorlength", sensorLength);
        shader.Dispatch(1, texture.width / 8, texture.height / 8, 1);


        ComputeBuffer buffer = new ComputeBuffer(data.Length, size_of_struct);
        buffer.SetData(data);
        shader.SetBuffer(0, "buffer", buffer);

        shader.SetInt("number_of_points", number_of_point);
        shader.SetInt("width", width);
        shader.SetInt("height", height);

        shader.Dispatch(0,width/128,1,1);
        buffer.GetData(data);
        buffer.Dispose();




    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(texture, dest);
    }
}
