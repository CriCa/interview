using UnityEngine;

[ExecuteInEditMode]
public class UnderwaterFX : MonoBehaviour
{
    #region public data
    public float fadeColorStrength = 0.125F;
    public float blur = 1f;
    public Color fadeColor = Color.white;
    public Shader underwaterShader;
    #endregion

    #region private data
    private Material shaderMat;
    #endregion

    void Start() { if (underwaterShader) shaderMat = new Material(underwaterShader); }

    #region post processing
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture rendTex = RenderTexture.GetTemporary(source.width, source.height);

        shaderMat.SetColor("depthColor", fadeColor);
        shaderMat.SetFloat("underwaterColorFade", fadeColorStrength);

        shaderMat.SetVector("offsets", new Vector4(blur, 0.0f, 0.0f, 0.0f));
        Graphics.Blit(source, rendTex, shaderMat, 0);
        shaderMat.SetVector("offsets", new Vector4(0.0f, blur, 0.0f, 0.0f));
        Graphics.Blit(rendTex, destination, shaderMat, 0);

        RenderTexture.ReleaseTemporary(rendTex);
    }
    #endregion
}