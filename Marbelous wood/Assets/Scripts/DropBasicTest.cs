using UnityEngine;
using UnityEngine.UI;

public class DropBasicTest : MonoBehaviour
{
    public Material drawMaterial;
    public Camera mainCamera;
    public float dropRadius = 0.1f;

    private RenderTexture renderTexture; // **存储颜色信息**
    private Vector4 lastClickPosition = new Vector4(0, 0, 0, 0);
    private Color selectedColor = Color.white;

    void Start()
    {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTexture.enableRandomWrite = true; // 允许写入
        renderTexture.Create(); // 显式创建 RenderTexture
        drawMaterial.SetTexture("_PreviousFrame", renderTexture);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                MeshRenderer meshRenderer = hit.collider.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    Vector2 pixelUV = hit.textureCoord;

                    // **更新滴落点**
                    lastClickPosition = new Vector4(pixelUV.x, pixelUV.y, 0, 0);
                    drawMaterial.SetVector("_ClickPos", lastClickPosition);
                    drawMaterial.SetColor("_MainColor", selectedColor);
                    drawMaterial.SetFloat("_DropRadius", dropRadius);
                }
            }
        }
    }

    public void SetSelectedColor(Color newColor)
    {
        selectedColor = newColor;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        RenderTexture tempTexture = RenderTexture.GetTemporary(src.width, src.height, 0);

    // 将当前帧绘制到临时 RenderTexture
    Graphics.Blit(src, tempTexture, drawMaterial);

    // 将临时 RenderTexture 复制到 renderTexture 中，保留前一帧数据
    Graphics.Blit(tempTexture, renderTexture);

    // 将最终结果输出到目标 RenderTexture
    Graphics.Blit(renderTexture, dest);

    // 释放临时 RenderTexture
    RenderTexture.ReleaseTemporary(tempTexture);
    }
}

