using UnityEngine;
using TMPro;

[ExecuteAlways]
public class CurvedTextForBook : MonoBehaviour
{
    [Header("Curve Settings")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 10, 1, -10);
    public float scale = 0.5f;

    TMP_Text textMesh;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (textMesh == null) return;

        textMesh.ForceMeshUpdate();
        var textInfo = textMesh.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            var verts = textInfo.meshInfo[textInfo.characterInfo[i].materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                Vector3 orig = verts[textInfo.characterInfo[i].vertexIndex + j];

                // X pozisyonunu normalize et (0-1 arasý)
                float x = Mathf.InverseLerp(
                    textMesh.rectTransform.rect.xMin,
                    textMesh.rectTransform.rect.xMax,
                    orig.x
                );

                // Eðriyi uygula
                orig.y += curve.Evaluate(x) * scale;

                verts[textInfo.characterInfo[i].vertexIndex + j] = orig;
            }
        }

        // Mesh’i güncelle
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textMesh.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
