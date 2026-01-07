using UnityEngine;

public class ComputeShaderNewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] ComputeShader computeShader = default!;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int x=8;
        int y=8;
        ComputeBuffer computeBuffer = new ComputeBuffer(4 * x * y, sizeof(float));

        int kernelHandle = computeShader.FindKernel("CSMain");//カーネル関数のハンドルを取得
        computeShader.SetBuffer(kernelHandle, "Result", computeBuffer);//バッファをセット
        computeShader.Dispatch(kernelHandle, x / 8, y / 8, 1);//カーネル関数を実行

        float[] result = new float[4 * x * y];
        computeBuffer.GetData(result);//CPUからGPUへデータを転送
        computeBuffer.Release();//バッファの解放

        //確認用に立方体を表示してみる
        for(int i=0; i<x ;i++)
        {
            for(int j=0; j<y ;j++)
            {
                float cs_x = result[4 * (i + j * x) + 0];
                float cs_y = result[4 * (i + j * x) + 1] * 10.0f;
                float cs_z = result[4 * (i + j * x) + 2] * 10.0f;
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(cs_x, cs_y, cs_z);
                cube.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);//隙間を開ける
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
