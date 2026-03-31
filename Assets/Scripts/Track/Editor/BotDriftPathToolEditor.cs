using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BotDriftPathTool))]
public class BotDriftPathToolEditor : Editor
{
    private BotDriftPathTool Tool => (BotDriftPathTool)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(8f);
        EditorGUILayout.HelpBox(
            "Modo Edicion Drift (Scene):\n" +
            "- Shift + Click izquierdo: agrega punto\n" +
            "- Shift + Click derecho: elimina punto cercano\n" +
            "- Click normal cerca de una linea: activa ese DriftPath\n" +
            "- Arrastra puntos directamente en Scene",
            MessageType.Info);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Crear/Obtener DriftPaths"))
            {
                var root = Tool.EnsureDriftPathsRoot();
                if (root != null)
                {
                    Selection.activeTransform = root;
                    EditorGUIUtility.PingObject(root.gameObject);
                }
            }

            if (GUILayout.Button("Crear Path Activo"))
            {
                var path = Tool.GetOrCreatePath(Tool.editPath);
                if (path != null)
                {
                    Selection.activeTransform = path;
                    EditorGUIUtility.PingObject(path.gameObject);
                }
            }
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Crear Path Activo + Puntos"))
            {
                var path = Tool.EnsureActivePathWithStarterPoints();
                if (path != null)
                {
                    Selection.activeTransform = path;
                    EditorGUIUtility.PingObject(path.gameObject);
                }
            }
        }

        var activePath = Tool.FindPath(Tool.editPath);
        if (activePath != null)
            EditorGUILayout.HelpBox($"Path activo: {activePath.name} | Puntos: {activePath.childCount}", MessageType.None);
    }

    private void OnSceneGUI()
    {
        var tool = Tool;
        if (tool == null || Application.isPlaying || !tool.sceneEditEnabled)
            return;

        DrawSceneOverlay();
        HandleSceneInput(tool);
        DrawAllPathHandles(tool);
    }

    private static void DrawSceneOverlay()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 430, 70), "Bot Drift Edit", "Window");
        GUILayout.Label("Shift + Click Izq = Agregar punto | Shift + Click Der = Eliminar punto");
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static void HandleSceneInput(BotDriftPathTool tool)
    {
        Event e = Event.current;
        if (e == null || e.alt || e.type != EventType.MouseDown)
            return;

        if (!TryGetScenePointFromMouse(tool, e.mousePosition, out Vector3 worldPoint))
            return;

        if (!e.shift && e.button == 0)
        {
            TryActivateNearestPath(tool, worldPoint);
            return;
        }

        if (!e.shift)
            return;

        if (e.button == 0)
        {
            tool.AddPointAt(worldPoint);
            e.Use();
            SceneView.RepaintAll();
        }
        else if (e.button == 1)
        {
            tool.RemoveNearestPointAt(worldPoint);
            e.Use();
            SceneView.RepaintAll();
        }
    }

    private static void DrawAllPathHandles(BotDriftPathTool tool)
    {
        Transform root = tool.EnsureDriftPathsRoot();
        if (root == null)
            return;

        string activeName = $"DriftPath_{Mathf.Clamp(tool.editPath, 1, 99):00}";
        for (int i = 0; i < root.childCount; i++)
        {
            Transform path = root.GetChild(i);
            bool active = path.name == activeName;
            Handles.color = active ? new Color(1f, 0.9f, 0.2f, 0.98f) : new Color(1f, 0.4f, 0.4f, 0.9f);
            DrawSinglePathHandles(tool, path);
        }
    }

    private static void DrawSinglePathHandles(BotDriftPathTool tool, Transform path)
    {
        if (path == null)
            return;

        for (int i = 0; i < path.childCount; i++)
        {
            Transform p = path.GetChild(i);
            Vector3 pos = p.position;
            float size = HandleUtility.GetHandleSize(pos) * 0.08f;

            EditorGUI.BeginChangeCheck();
            Vector3 moved = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "Mover punto BotDriftPath");
                p.position = SnapPoint(tool, moved);
                EditorUtility.SetDirty(p);
                EditorSceneMarkDirty(tool);
            }

            if (tool.drawPointLabels)
                Handles.Label(pos + Vector3.up * (size * 1.6f), p.name);
        }
    }

    private static bool TryGetScenePointFromMouse(BotDriftPathTool tool, Vector2 mousePos, out Vector3 worldPoint)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 5000f, tool.snapMask, QueryTriggerInteraction.Ignore))
        {
            worldPoint = hit.point;
            return true;
        }

        Plane plane = new Plane(Vector3.up, tool.transform.position);
        if (plane.Raycast(ray, out float enter))
        {
            worldPoint = ray.GetPoint(enter);
            return true;
        }

        worldPoint = Vector3.zero;
        return false;
    }

    private static Vector3 SnapPoint(BotDriftPathTool tool, Vector3 point)
    {
        if (!tool.snapPointToGround)
            return point;

        Ray ray = new Ray(point + Vector3.up * Mathf.Max(10f, tool.snapRayHeight), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, tool.snapRayHeight * 2f, tool.snapMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        return point;
    }

    private static void TryActivateNearestPath(BotDriftPathTool tool, Vector3 worldPoint)
    {
        Transform root = tool.EnsureDriftPathsRoot();
        if (root == null || root.childCount == 0)
            return;

        int bestPath = -1;
        float bestDist = float.MaxValue;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform path = root.GetChild(i);
            float d = DistancePointToPath(worldPoint, path, tool.closePathSegments);
            if (d < bestDist)
            {
                bestDist = d;
                bestPath = i;
            }
        }

        if (bestPath < 0 || bestDist > Mathf.Max(2f, tool.removePointRadius * 2.5f))
            return;

        int pathNumber = bestPath + 1;
        if (tool.editPath != pathNumber)
        {
            Undo.RecordObject(tool, "Cambiar DriftPath activo");
            tool.editPath = pathNumber;
            EditorUtility.SetDirty(tool);
        }
    }

    private static float DistancePointToPath(Vector3 point, Transform path, bool closed)
    {
        if (path == null || path.childCount < 2)
            return float.MaxValue;

        float best = float.MaxValue;
        int segments = closed ? path.childCount : path.childCount - 1;
        for (int i = 0; i < segments; i++)
        {
            Vector3 a = path.GetChild(i).position;
            Vector3 b = path.GetChild((i + 1) % path.childCount).position;
            float d = Mathf.Sqrt(DistancePointToSegmentSqr(point, a, b));
            if (d < best)
                best = d;
        }

        return best;
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

    private static void EditorSceneMarkDirty(BotDriftPathTool tool)
    {
        EditorUtility.SetDirty(tool);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(tool.gameObject.scene);
    }
}
