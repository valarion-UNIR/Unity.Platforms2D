using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class Util
{
    public static void DrawGizmosCircle(Vector3 pos, Vector3 normal, float radius, int numSegments = 32)
    {
        Vector3 temp = (normal.x < normal.z) ? new Vector3(1f, 0f, 0f) : new Vector3(0f, 0f, 1f);
        Vector3 forward = Vector3.Cross(normal, temp).normalized;
        Vector3 right = Vector3.Cross(forward, normal).normalized;

        Vector3 prevPt = pos + (forward * radius);
        float angleStep = (Mathf.PI * 2f) / numSegments;
        for (int i = 0; i < numSegments; i++)
        {
            float angle = (i == numSegments - 1) ? 0f : (i + 1) * angleStep;

            Vector3 nextPtLocal = new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * radius;

            Vector3 nextPt = pos + (right * nextPtLocal.x) + (forward * nextPtLocal.z);

            Gizmos.DrawLine(prevPt, nextPt);

            prevPt = nextPt;
        }
    }

    //public static Collider2D EnsureSelfHitBoxAndChildSensorBox<T>(this Component component) where T : Collider2D
    //    => EnsureChildSensorBox(component, EnsureSelfHitBox<T>(component));


    public static Collider2D EnsureSelfHitBox<T>(this Component component, int layer = 6) where T : Collider2D
    {
        var gameObject = component.gameObject;
        gameObject.layer = layer;

        if (!gameObject.TryGetComponent(out Collider2D hitbox))
            hitbox = gameObject.AddComponent<T>();

        return hitbox;
    }

    public static Collider2D EnsureChildSensorBox<T>(this Component component, Collider2D toCopy = null, int layer = 7) where T : Collider2D
    {
        var gameObject = component.gameObject;
        var sensorbox = gameObject.GetComponentsInChildren<Collider2D>().Where(c => c.gameObject.layer == layer).FirstOrDefault();
        if (sensorbox == null)
        {
            var sensor = new GameObject("Sensor") { layer = layer, };
            sensor.transform.SetParent(gameObject.transform, false);

            sensorbox = toCopy != null ? sensor.AddCopyComponent(toCopy) : sensor.AddComponent<T>();
            sensorbox.isTrigger = true;
        }

        return sensorbox;
    }


    private static readonly string[] Excluded = new[] { "density", "usedByComposite" };
    public static T AddCopyComponent<T>(this GameObject destination, T original) where T : Component
    {
        var type = original.GetType();
        var comp = destination.AddComponent(type);

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default;
        PropertyInfo[] pinfos = type.GetProperties(flags);

        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite && !Excluded.Contains(pinfo.Name))
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(original, null), null);
                }
                catch
                {
                    /*
                     * In case of NotImplementedException being thrown.
                     * For some reason specifying that exception didn't seem to catch it,
                     * so I didn't catch anything specific.
                     */
                }
            }
        }

        FieldInfo[] finfos = type.GetFields(flags);

        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(original));
        }
        return comp as T;
    }
}
