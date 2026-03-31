using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BotDriftPathTool : MonoBehaviour
{
    [Header("Referencias")]
    [InspectorName("Pista (Track)")]
    public Track track;
    [InspectorName("Raiz DriftPaths")]
    [Tooltip("Si es null, se creara un hijo llamado BotDriftPaths dentro de la pista.")]
    public Transform driftPathsRoot;

    [Header("Runtime")]
    [InspectorName("Radio de Activacion")]
    [Range(0.25f, 12f)] public float activationRadius = 4.5f;
    [InspectorName("Ticks Extra al Salir")]
    [Range(0, 60)] public int holdDriftTicksAfterExit = 8;
    [InspectorName("Usar Paths Cerrados")]
    public bool closePathSegments = false;
    [InspectorName("Drift por Lane Cercano")]
    [Tooltip("Si esta activo, primero intenta activar drift solo con el DriftPath del carril (Lane_XX) mas cercano al bot.")]
    public bool useNearestLaneBoundPath = true;
    [InspectorName("Solo Drift por Lane (estricto)")]
    [Tooltip("Si esta activo, no usa fallback global. Si no encuentra path del lane, no activa drift.")]
    public bool strictLaneBoundPath = false;

    [Header("Gizmos")]
    [InspectorName("Mostrar Gizmos")]
    public bool drawGizmos = true;
    [InspectorName("Dibujar Siempre (Scene)")]
    public bool drawAlways = true;
    [InspectorName("Color de Path")]
    public Color pathColor = new Color(1f, 0.35f, 0.35f, 0.95f);
    [InspectorName("Color de Punto")]
    public Color pointColor = new Color(1f, 0.85f, 0.2f, 0.98f);
    [InspectorName("Grosor Linea")]
    [Range(0.01f, 0.8f)] public float gizmoLineThickness = 0.16f;
    [InspectorName("Radio de Punto")]
    [Range(0.02f, 1.2f)] public float gizmoPointRadius = 0.18f;

    [Header("Edicion Rapida (Scene)")]
    [InspectorName("Modo Edicion Rapida")]
    [Tooltip("Shift + Click izquierdo: agrega punto. Shift + Click derecho: elimina punto cercano.")]
    public bool sceneEditEnabled = true;
    [InspectorName("Path Activo")]
    [Range(1, 20)] public int editPath = 1;
    [InspectorName("Insertar en Segmento Cercano")]
    public bool insertOnNearestSegment = true;
    [InspectorName("Ajustar al Piso al Crear/Mover")]
    public bool snapPointToGround = true;
    [InspectorName("Altura Raycast Snap")]
    [Range(10f, 2000f)] public float snapRayHeight = 300f;
    [InspectorName("Mascara Snap Piso")]
    public LayerMask snapMask = ~0;
    [InspectorName("Distancia Minima entre Puntos")]
    [Range(0.05f, 10f)] public float pointMinSpacing = 0.55f;
    [InspectorName("Radio Eliminacion Punto")]
    [Range(0.1f, 8f)] public float removePointRadius = 1.4f;
    [InspectorName("Mostrar Etiquetas P_XXX")]
    public bool drawPointLabels = false;

    public void ResolveReferences()
    {
        if (track == null)
            track = GetComponent<Track>();
        if (track == null)
            track = GetComponentInParent<Track>();
    }

    public Transform EnsureDriftPathsRoot()
    {
        ResolveReferences();
        if (driftPathsRoot != null)
            return driftPathsRoot;

        Transform parent = track != null ? track.transform : transform;
        Transform found = FindChildByNameRecursive(parent, "BotDriftPaths");
        if (found != null)
        {
            driftPathsRoot = found;
            return driftPathsRoot;
        }

        GameObject go = new GameObject("BotDriftPaths");
        go.transform.SetParent(parent, false);
        driftPathsRoot = go.transform;
        MarkDirtySafe();
        return driftPathsRoot;
    }

    public Transform FindPath(int pathIndex)
    {
        Transform root = EnsureDriftPathsRoot();
        if (root == null)
            return null;

        string pathName = $"DriftPath_{Mathf.Clamp(pathIndex, 1, 99):00}";
        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);
            if (child.name == pathName)
                return child;
        }

        return null;
    }

    public Transform GetOrCreatePath(int pathIndex)
    {
        Transform path = FindPath(pathIndex);
        if (path != null)
            return path;

        Transform root = EnsureDriftPathsRoot();
        if (root == null)
            return null;

        GameObject go = new GameObject($"DriftPath_{Mathf.Clamp(pathIndex, 1, 99):00}");
        go.transform.SetParent(root, false);
        MarkDirtySafe();
        return go.transform;
    }

    public Transform EnsureActivePathWithStarterPoints()
    {
        Transform path = GetOrCreatePath(editPath);
        if (path == null)
            return null;

        // Keep user-authored path unchanged.
        if (path.childCount >= 2)
            return path;

        var starterPoints = BuildStarterPoints();
        for (int i = 0; i < starterPoints.Count; i++)
            CreatePoint(path, starterPoints[i]);

        RenumberPoints(path);
        MarkDirtySafe();
        return path;
    }

    public void AddPointAt(Vector3 worldPoint)
    {
        Transform path = GetOrCreatePath(editPath);
        if (path == null)
            return;

        worldPoint = SnapPoint(worldPoint);
        if (IsTooCloseToAnyPoint(path, worldPoint, Mathf.Max(0.05f, pointMinSpacing)))
            return;

        int insertIndex = path.childCount;
        if (insertOnNearestSegment && path.childCount >= 2)
            insertIndex = FindInsertIndexOnNearestSegment(path, worldPoint, closePathSegments);

        Transform p = CreatePoint(path, worldPoint);
        p.transform.SetSiblingIndex(Mathf.Clamp(insertIndex, 0, path.childCount));
        RenumberPoints(path);
        MarkDirtySafe();
    }

    public void RemoveNearestPointAt(Vector3 worldPoint)
    {
        Transform path = FindPath(editPath);
        if (path == null || path.childCount == 0)
            return;

        int nearestIndex = -1;
        float bestSqr = float.MaxValue;
        for (int i = 0; i < path.childCount; i++)
        {
            float sqr = (path.GetChild(i).position - worldPoint).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                nearestIndex = i;
            }
        }

        if (nearestIndex < 0 || bestSqr > removePointRadius * removePointRadius)
            return;

        GameObject go = path.GetChild(nearestIndex).gameObject;
#if UNITY_EDITOR
        UnityEditor.Undo.DestroyObjectImmediate(go);
#else
        Destroy(go);
#endif
        RenumberPoints(path);
        MarkDirtySafe();
    }

    public bool TryGetActiveDriftAt(Vector3 worldPoint, out int holdTicks)
    {
        holdTicks = Mathf.Max(0, holdDriftTicksAfterExit);
        Transform root = driftPathsRoot;
        if (root == null)
            root = EnsureDriftPathsRoot();
        if (root == null || root.childCount == 0)
            return false;

        float radiusSqr = Mathf.Max(0.01f, activationRadius * activationRadius);
        if (useNearestLaneBoundPath)
        {
            int laneIndex = FindNearestLaneIndex(worldPoint);
            if (laneIndex > 0)
            {
                Transform lanePath = FindPath(laneIndex);
                if (IsPointNearPath(worldPoint, lanePath, radiusSqr))
                    return true;

                if (strictLaneBoundPath)
                    return false;
            }
            else if (strictLaneBoundPath)
            {
                return false;
            }
        }

        for (int i = 0; i < root.childCount; i++)
        {
            Transform path = root.GetChild(i);
            if (IsPointNearPath(worldPoint, path, radiusSqr))
                return true;
        }

        return false;
    }

    private bool IsPointNearPath(Vector3 worldPoint, Transform path, float radiusSqr)
    {
        if (path == null || path.childCount < 2)
            return false;

        int segments = closePathSegments ? path.childCount : path.childCount - 1;
        for (int p = 0; p < segments; p++)
        {
            Vector3 a = path.GetChild(p).position;
            Vector3 b = path.GetChild((p + 1) % path.childCount).position;
            if (DistancePointToSegmentSqr(worldPoint, a, b) <= radiusSqr)
                return true;
        }

        return false;
    }

    private int FindNearestLaneIndex(Vector3 worldPoint)
    {
        Transform lanesRoot = GetBotLanesRoot();
        if (lanesRoot == null || lanesRoot.childCount == 0)
            return -1;

        int bestLaneIndex = -1;
        float bestSqr = float.MaxValue;
        for (int i = 0; i < lanesRoot.childCount; i++)
        {
            Transform lane = lanesRoot.GetChild(i);
            if (lane == null || lane.childCount < 2)
                continue;

            float distSqr = DistancePointToPathSqr(worldPoint, lane, true);
            if (distSqr < bestSqr)
            {
                bestSqr = distSqr;
                bestLaneIndex = ExtractLaneIndex(lane.name, i + 1);
            }
        }

        return bestLaneIndex;
    }

    private Transform GetBotLanesRoot()
    {
        ResolveReferences();
        if (track == null)
            return null;

        var settings = track.GetComponent<BotAITrackSettings>();
        if (settings != null && settings.botLanesRoot != null)
            return settings.botLanesRoot;

        return FindChildByNameRecursive(track.transform, "BotLanes");
    }

    private static int ExtractLaneIndex(string laneName, int fallback)
    {
        if (string.IsNullOrEmpty(laneName))
            return Mathf.Max(1, fallback);

        int n = 0;
        for (int i = 0; i < laneName.Length; i++)
        {
            char c = laneName[i];
            if (c >= '0' && c <= '9')
                n = n * 10 + (c - '0');
        }

        return n > 0 ? n : Mathf.Max(1, fallback);
    }

    private static float DistancePointToPathSqr(Vector3 point, Transform path, bool closed)
    {
        if (path == null || path.childCount < 2)
            return float.MaxValue;

        float best = float.MaxValue;
        int segments = closed ? path.childCount : path.childCount - 1;
        for (int i = 0; i < segments; i++)
        {
            Vector3 a = path.GetChild(i).position;
            Vector3 b = path.GetChild((i + 1) % path.childCount).position;
            float d = DistancePointToSegmentSqr(point, a, b);
            if (d < best)
                best = d;
        }

        return best;
    }

    private Vector3 SnapPoint(Vector3 point)
    {
        if (!snapPointToGround)
            return point;

        Ray ray = new Ray(point + Vector3.up * Mathf.Max(10f, snapRayHeight), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, snapRayHeight * 2f, snapMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        return point;
    }

    private List<Vector3> BuildStarterPoints()
    {
        var result = new List<Vector3>(3);

        ResolveReferences();
        Vector3 anchor = transform.position;
        Vector3 forward = transform.forward;

        if (track != null)
        {
            anchor = track.transform.position;
            forward = track.transform.forward;

            if (track.finishLine != null)
            {
                anchor = track.finishLine.transform.position;
                forward = track.finishLine.transform.forward;
            }
            else if (track.spawnpoints != null && track.spawnpoints.Length > 0 && track.spawnpoints[0] != null)
            {
                anchor = track.spawnpoints[0].position;
                forward = track.spawnpoints[0].forward;
            }
        }

        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f)
            forward = Vector3.forward;
        forward.Normalize();

        result.Add(SnapPoint(anchor + forward * 1.5f));
        result.Add(SnapPoint(anchor + forward * 7.0f));
        result.Add(SnapPoint(anchor + forward * 12.5f));
        return result;
    }

    private static Transform CreatePoint(Transform path, Vector3 worldPoint)
    {
        GameObject p = new GameObject("P_000");
#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(p, "Crear punto BotDriftPath");
#endif
        p.transform.SetParent(path, false);
        p.transform.position = worldPoint;
        return p.transform;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || !drawAlways)
            return;
        DrawPathsGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos)
            return;
        DrawPathsGizmos();
    }

    private void DrawPathsGizmos()
    {
        Transform root = driftPathsRoot;
        if (root == null)
            return;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform path = root.GetChild(i);
            if (path == null || path.childCount == 0)
                continue;

            Gizmos.color = pathColor;
            for (int p = 0; p < path.childCount; p++)
            {
                Vector3 pos = path.GetChild(p).position;
                Gizmos.DrawSphere(pos, gizmoPointRadius);

                if (p < path.childCount - 1)
                    DrawThickLine(path.GetChild(p).position, path.GetChild(p + 1).position, gizmoLineThickness);
                else if (closePathSegments && path.childCount > 2)
                    DrawThickLine(path.GetChild(p).position, path.GetChild(0).position, gizmoLineThickness);
            }
        }
    }

    private static void DrawThickLine(Vector3 a, Vector3 b, float radius)
    {
        Gizmos.DrawLine(a, b);
        if (radius > 0.001f)
        {
            Gizmos.DrawSphere(a, radius * 0.25f);
            Gizmos.DrawSphere(b, radius * 0.25f);
        }
    }

    private static bool IsTooCloseToAnyPoint(Transform path, Vector3 worldPoint, float minDistance)
    {
        float minSqr = minDistance * minDistance;
        for (int i = 0; i < path.childCount; i++)
        {
            if ((path.GetChild(i).position - worldPoint).sqrMagnitude <= minSqr)
                return true;
        }
        return false;
    }

    private static int FindInsertIndexOnNearestSegment(Transform path, Vector3 worldPoint, bool closed)
    {
        int bestInsert = path.childCount;
        float bestSqr = float.MaxValue;
        int segments = closed ? path.childCount : path.childCount - 1;
        for (int i = 0; i < segments; i++)
        {
            Vector3 a = path.GetChild(i).position;
            Vector3 b = path.GetChild((i + 1) % path.childCount).position;
            float sqr = DistancePointToSegmentSqr(worldPoint, a, b);
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                bestInsert = i + 1;
            }
        }

        return bestInsert;
    }

    private static float DistancePointToSegmentSqr(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float denom = Vector3.Dot(ab, ab);
        if (denom <= 0.0001f)
            return (point - a).sqrMagnitude;

        float t = Mathf.Clamp01(Vector3.Dot(point - a, ab) / denom);
        Vector3 closest = a + ab * t;
        return (point - closest).sqrMagnitude;
    }

    private static void RenumberPoints(Transform path)
    {
        if (path == null)
            return;

        for (int i = 0; i < path.childCount; i++)
            path.GetChild(i).name = $"P_{i:000}";
    }

    private static Transform FindChildByNameRecursive(Transform root, string name)
    {
        if (root == null || string.IsNullOrEmpty(name))
            return null;
        if (root.name == name)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform found = FindChildByNameRecursive(root.GetChild(i), name);
            if (found != null)
                return found;
        }
        return null;
    }

    private void MarkDirtySafe()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        if (driftPathsRoot != null)
            UnityEditor.EditorUtility.SetDirty(driftPathsRoot.gameObject);
        if (track != null)
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }
}
