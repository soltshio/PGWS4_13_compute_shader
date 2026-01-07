using UnityEngine;
using System.Runtime.InteropServices;

struct Particle
{
    public Vector3 Position;
    public Vector3 Velocity;
    public Vector3 Color;
}

public class ParticleMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] Material material = default!;
    [SerializeField] ComputeShader computeShader = default!;

    private int updateKernel;
    private ComputeBuffer buffer;

    private const int THREAD_NUM = 60;
    private const int PARTICLE_NUM = ((65536 + THREAD_NUM - 1) / THREAD_NUM) * THREAD_NUM;

    private void OnEnable()
    {
        //パーティクルの情報を格納するバッファ
        buffer = new ComputeBuffer(
            PARTICLE_NUM,
            Marshal.SizeOf(typeof(Particle)),
            ComputeBufferType.Default);

        //初期化
        int initKernel = computeShader.FindKernel("CSInitialize");
        computeShader.SetBuffer(initKernel, "Particles", buffer);
        computeShader.Dispatch(initKernel, PARTICLE_NUM / THREAD_NUM, 1, 1);

        //更新後の設定
        updateKernel = computeShader.FindKernel("CSUpdate");
        computeShader.SetBuffer(updateKernel, "Particles", buffer);

        //描画用のマテリアルの設定
        material.SetBuffer("Particles", buffer);
    }

    private void OnDisable()
    {
        buffer.Release();
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.Dispatch(updateKernel, PARTICLE_NUM / THREAD_NUM, 1, 1);
    }

    private void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, PARTICLE_NUM);
    }
}
