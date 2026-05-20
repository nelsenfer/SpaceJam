using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Reflection;

[ExecuteInEditMode]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(ShadowCaster2D))]
public class SyncShadow : MonoBehaviour
{
    PolygonCollider2D polygonCollider;
    ShadowCaster2D shadowCaster;
    FieldInfo shapeField;

    int lastPointCount;
    Vector2[] lastPoints;

    void Start()
    {
        SyncShape();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            if (polygonCollider == null) polygonCollider = GetComponent<PolygonCollider2D>();

            if (polygonCollider.points.Length != lastPointCount || IsPointsChanged())
            {
                SyncShape();
                lastPointCount = polygonCollider.points.Length;
                lastPoints = polygonCollider.points;
            }
        }
    }

    public void SyncShape()
    {
        if (polygonCollider == null) polygonCollider = GetComponent<PolygonCollider2D>();
        if (shadowCaster == null) shadowCaster = GetComponent<ShadowCaster2D>();

        if (polygonCollider != null && shadowCaster != null)
        {
            Vector3[] newShadowPoints = new Vector3[polygonCollider.points.Length];

            for (int i = 0; i < polygonCollider.points.Length; i++)
            {
                newShadowPoints[i] = (Vector3)polygonCollider.points[i];
            }

            if (shapeField == null)
            {
                shapeField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            if (shapeField != null)
            {
                shapeField.SetValue(shadowCaster, newShadowPoints);
            }
        }
    }

    bool IsPointsChanged()
    {
        if (lastPoints == null || lastPoints.Length != polygonCollider.points.Length) return true;

        for (int i = 0; i < polygonCollider.points.Length; i++)
        {
            if (polygonCollider.points[i] != lastPoints[i]) return true;
        }
        return false;
    }
}