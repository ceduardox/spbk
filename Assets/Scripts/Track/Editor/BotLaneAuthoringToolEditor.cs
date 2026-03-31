using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BotLaneAuthoringTool))]
public class BotLaneAuthoringToolEditor : Editor
{
    private BotLaneAuthoringTool Tool => (BotLaneAuthoringTool)target;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Tool.editReferenceLaneInScene && !Tool.sceneEditEnabled)
        {
            EditorGUILayout.HelpBox(
                "Para editar la linea roja, tambien debes activar 'Modo Edicion Rapida'.",
                MessageType.Warning);
            if (GUILayout.Button("Activar Modo Edicion Rapida"))
            {
                Undo.RecordObject(Tool, "Activar modo edicion rapida");
                Tool.sceneEditEnabled = true;
                EditorUtility.SetDirty(Tool);
            }
        }

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("Acciones Rapidas", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Generar Carriles"))
                Tool.AutoBuildFromCheckpoints();
            if (GUILayout.Button("Uniformar Puntos"))
                Tool.UniformResampleLanes();
        }
        if (GUILayout.Button("Auto Ajuste Inteligente"))
            Tool.SmartAutoAdjustLanes();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Generar desde Carril Base"))
                Tool.BuildFromReferenceLane();
            if (GUILayout.Button("Base + Uniformar + Proyectar"))
                Tool.BuildFromReferenceUniformAndProject();
        }
        if (GUILayout.Button("Crear/Actualizar Carril Base (Linea Roja)"))
            Tool.RebuildReferenceLaneFromCheckpoints();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Proyectar al Carril"))
                Tool.ProjectLanesToRoad();
            if (GUILayout.Button("Generar + Uniformar + Proyectar"))
                Tool.AutoBuildUniformAndProjectLanes();
        }
        if (GUILayout.Button("Generar + Uniformar (Recomendado)"))
            Tool.AutoBuildAndUniformLanes();
        if (GUILayout.Button("Limpiar Carriles"))
            Tool.ClearGeneratedLanes();

        EditorGUILayout.Space(6f);
        EditorGUILayout.HelpBox(
            "Modo Edicion Rapida (Scene):\n" +
            "- Shift + Click izquierdo: agrega punto\n" +
            "- Shift + Click derecho: elimina punto cercano\n" +
            "- Puedes arrastrar puntos del carril activo directamente en Scene\n" +
            "- Si activas 'Editar Carril Base en Scene', editas la linea roja\n" +
            "- Si no aparecen puntos rojos, usa 'Crear/Actualizar Carril Base (Linea Roja)'",
            MessageType.Info);
    }

    private void OnSceneGUI()
    {
        var tool = Tool;
        if (tool == null || Application.isPlaying || !tool.sceneEditEnabled)
            return;

        DrawSceneOverlay();
        HandleSceneInput(tool);
        DrawAllLanePointHandles(tool);
    }

    private static void DrawSceneOverlay()
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, 10, 430, 70), "Bot Lane Edit", "Window");
        GUILayout.Label("Shift + Click Izq = Agregar punto | Shift + Click Der = Eliminar punto");
        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private static void HandleSceneInput(BotLaneAuthoringTool tool)
    {
        Event e = Event.current;
        if (e == null)
            return;

        if (e.type != EventType.MouseDown || e.alt)
            return;

        if (!TryGetScenePointFromMouse(tool, e.mousePosition, out Vector3 worldPoint))
            return;

        // Click normal: activa el carril mas cercano para que luego Shift+Click edite ese carril.
        if (!e.shift && e.button == 0)
        {
            if (tool.editReferenceLaneInScene)
                return;
            TryActivateNearestLane(tool, worldPoint);
            return;
        }

        if (!e.shift)
            return;

        if (e.button == 0)
        {
            AddPoint(tool, worldPoint);
            e.Use();
            SceneView.RepaintAll();
        }
        else if (e.button == 1)
        {
            RemoveNearestPoint(tool, worldPoint);
            e.Use();
            SceneView.RepaintAll();
        }
    }

    private static void TryActivateNearestLane(BotLaneAuthoringTool tool, Vector3 worldPoint)
    {
        Transform root = tool.botLanesRoot;
        if (root == null || root.childCount == 0)
            return;

        int bestLane = -1;
        float bestDist = float.MaxValue;
        for (int l = 0; l < root.childCount; l++)
        {
            Transform lane = root.GetChild(l);
            float d = DistancePointToLane(worldPoint, lane, tool.closedLoop);
            if (d < bestDist)
            {
                bestDist = d;
                bestLane = l;
            }
        }

        if (bestLane < 0)
            return;

        float pickThreshold = Mathf.Max(2.0f, tool.removePointRadius * 2.5f);
        if (bestDist > pickThreshold)
            return;

        int laneNumber = bestLane + 1;
        if (tool.editLane != laneNumber)
        {
            Undo.RecordObject(tool, "Cambiar carril activo");
            tool.editLane = laneNumber;
            EditorUtility.SetDirty(tool);
            SceneView.RepaintAll();
        }
    }

    private static void DrawAllLanePointHandles(BotLaneAuthoringTool tool)
    {
        if (tool.editReferenceLaneInScene)
        {
            Transform referenceLane = GetOrCreateReferenceLane(tool);
            if (referenceLane != null && referenceLane.childCount < 2 && tool.autoSeedReferenceLaneWhenEdit)
            {
                tool.RebuildReferenceLaneFromCheckpoints();
                referenceLane = GetOrCreateReferenceLane(tool);
            }
            if (referenceLane == null)
                return;

            Handles.color = new Color(1f, 0.35f, 0.35f, 0.98f);
            DrawSingleLaneHandles(tool, referenceLane, true);
            return;
        }

        Transform root = tool.botLanesRoot;
        if (root == null)
            return;

        string activeLaneName = $"Lane_{Mathf.Clamp(tool.editLane, 1, 99):00}";
        for (int l = 0; l < root.childCount; l++)
        {
            Transform lane = root.GetChild(l);
            bool isActiveLane = lane.name == activeLaneName;
            Handles.color = isActiveLane
                ? new Color(1f, 0.9f, 0.2f, 0.98f)
                : new Color(0.75f, 0.85f, 1f, 0.9f);

            DrawSingleLaneHandles(tool, lane, isActiveLane);
        }
    }

    private static void DrawSingleLaneHandles(BotLaneAuthoringTool tool, Transform lane, bool active)
    {
        if (lane == null)
            return;

        for (int i = 0; i < lane.childCount; i++)
        {
            Transform p = lane.GetChild(i);
            Vector3 pos = p.position;
            float baseSize = HandleUtility.GetHandleSize(pos) * 0.06f;
            float size = active ? baseSize * 1.15f : baseSize * 0.9f;

            EditorGUI.BeginChangeCheck();
            Vector3 moved = Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.SphereHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(p, "Mover punto BotLane");
                p.position = SnapPoint(tool, moved);
                MarkDirty(tool, p.gameObject);
            }

            if (tool.drawPointLabels)
                Handles.Label(pos + Vector3.up * (size * 1.6f), p.name);
        }
    }

    private static void AddPoint(BotLaneAuthoringTool tool, Vector3 worldPoint)
    {
        Transform lane = GetOrCreateLane(tool);
        if (lane == null)
            return;

        worldPoint = SnapPoint(tool, worldPoint);

        if (IsTooCloseToAnyPoint(lane, worldPoint, Mathf.Max(0.05f, tool.pointMinSpacing)))
            return;

        int insertIndex = lane.childCount;
        if (tool.insertOnNearestSegment && lane.childCount >= 2)
            insertIndex = FindInsertIndexOnNearestSegment(lane, worldPoint, tool.closedLoop);

        GameObject p = new GameObject("P_000");
        Undo.RegisterCreatedObjectUndo(p, "Crear punto BotLane");
        p.transform.SetParent(lane, false);
        p.transform.position = worldPoint;
        p.transform.SetSiblingIndex(Mathf.Clamp(insertIndex, 0, lane.childCount));

        RenumberPoints(lane);
        Selection.activeTransform = p.transform;
        MarkDirty(tool, p);
    }

    private static void RemoveNearestPoint(BotLaneAuthoringTool tool, Vector3 worldPoint)
    {
        Transform lane = FindLane(tool);
        if (lane == null || lane.childCount == 0)
            return;

        int nearestIndex = -1;
        float bestSqr = float.MaxValue;
        for (int i = 0; i < lane.childCount; i++)
        {
            float sqr = (lane.GetChild(i).position - worldPoint).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                nearestIndex = i;
            }
        }

        if (nearestIndex < 0)
            return;
        if (Mathf.Sqrt(bestSqr) > Mathf.Max(0.1f, tool.removePointRadius))
            return;

        Transform target = lane.GetChild(nearestIndex);
        if (Selection.activeTransform == target)
            Selection.activeTransform = null;
        Undo.DestroyObjectImmediate(target.gameObject);
        RenumberPoints(lane);
        MarkDirty(tool, lane.gameObject);
    }

    private static bool TryGetScenePointFromMouse(BotLaneAuthoringTool tool, Vector2 mouse, out Vector3 worldPoint)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(mouse);

        if (Physics.Raycast(ray, out RaycastHit hit, 5000f, ~0, QueryTriggerInteraction.Ignore))
        {
            worldPoint = hit.point;
            return true;
        }

        float y = tool.track != null ? tool.track.transform.position.y : tool.transform.position.y;
        Plane plane = new Plane(Vector3.up, new Vector3(0f, y, 0f));
        if (plane.Raycast(ray, out float enter))
        {
            worldPoint = ray.GetPoint(enter);
            return true;
        }

        worldPoint = Vector3.zero;
        return false;
    }

    private static Vector3 SnapPoint(BotLaneAuthoringTool tool, Vector3 point)
    {
        if (!tool.snapPointToGround)
            return point;

        float h = Mathf.Max(10f, tool.snapRayHeight);
        Vector3 originTop = point + Vector3.up * h;
        Vector3 originBottom = point - Vector3.up * h;

        if (Physics.Raycast(originTop, Vector3.down, out RaycastHit hitDown, h * 2f, ~0, QueryTriggerInteraction.Ignore))
            return hitDown.point;
        if (Physics.Raycast(originBottom, Vector3.up, out RaycastHit hitUp, h * 2f, ~0, QueryTriggerInteraction.Ignore))
            return hitUp.point;

        return point;
    }

    private static bool IsTooCloseToAnyPoint(Transform lane, Vector3 point, float minDistance)
    {
        float minSqr = minDistance * minDistance;
        for (int i = 0; i < lane.childCount; i++)
        {
            if ((lane.GetChild(i).position - point).sqrMagnitude <= minSqr)
                return true;
        }
        return false;
    }

    private static int FindInsertIndexOnNearestSegment(Transform lane, Vector3 point, bool closedLoop)
    {
        int count = lane.childCount;
        if (count < 2)
            return count;

        int bestSeg = 0;
        float bestDist = float.MaxValue;
        int segments = closedLoop ? count : count - 1;

        for (int i = 0; i < segments; i++)
        {
            int j = (i + 1) % count;
            Vector3 a = lane.GetChild(i).position;
            Vector3 b = lane.GetChild(j).position;
            float d = DistancePointToSegment(point, a, b);
            if (d < bestDist)
            {
                bestDist = d;
                bestSeg = i;
            }
        }

        return Mathf.Clamp(bestSeg + 1, 0, count);
    }

    private static float DistancePointToLane(Vector3 point, Transform lane, bool closedLoop)
    {
        if (lane == null || lane.childCount == 0)
            return float.MaxValue;
        if (lane.childCount == 1)
            return Vector3.Distance(point, lane.GetChild(0).position);

        float best = float.MaxValue;
        int count = lane.childCount;
        int segments = closedLoop ? count : count - 1;
        for (int i = 0; i < segments; i++)
        {
            int j = (i + 1) % count;
            Vector3 a = lane.GetChild(i).position;
            Vector3 b = lane.GetChild(j).position;
            float d = DistancePointToSegment(point, a, b);
            if (d < best)
                best = d;
        }

        return best;
    }

    private static float DistancePointToSegment(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float abSqr = ab.sqrMagnitude;
        if (abSqr < 0.000001f)
            return Vector3.Distance(p, a);
        float t = Mathf.Clamp01(Vector3.Dot(p - a, ab) / abSqr);
        Vector3 q = a + ab * t;
        return Vector3.Distance(p, q);
    }

    private static Transform GetOrCreateLane(BotLaneAuthoringTool tool)
    {
        if (tool.editReferenceLaneInScene)
            return GetOrCreateReferenceLane(tool);

        Transform root = tool.botLanesRoot;
        if (root == null)
        {
            root = FindChildByNameRecursive(tool.track != null ? tool.track.transform : tool.transform, "BotLanes");
            if (root == null)
            {
                var rootGo = new GameObject("BotLanes");
                Undo.RegisterCreatedObjectUndo(rootGo, "Crear BotLanes");
                rootGo.transform.SetParent(tool.track != null ? tool.track.transform : tool.transform, false);
                root = rootGo.transform;
            }
            Undo.RecordObject(tool, "Asignar BotLanesRoot");
            tool.botLanesRoot = root;
            MarkDirty(tool, root.gameObject);
        }

        string laneName = $"Lane_{Mathf.Clamp(tool.editLane, 1, 99):00}";
        Transform lane = FindDirectChildByName(root, laneName);
        if (lane == null)
        {
            var laneGo = new GameObject(laneName);
            Undo.RegisterCreatedObjectUndo(laneGo, "Crear carril BotLane");
            laneGo.transform.SetParent(root, false);
            lane = laneGo.transform;
            MarkDirty(tool, laneGo);
        }
        return lane;
    }

    private static Transform FindLane(BotLaneAuthoringTool tool)
    {
        if (tool.editReferenceLaneInScene)
            return tool.referenceLaneRoot;

        Transform root = tool.botLanesRoot;
        if (root == null)
            return null;
        string laneName = $"Lane_{Mathf.Clamp(tool.editLane, 1, 99):00}";
        return FindDirectChildByName(root, laneName);
    }

    private static Transform GetOrCreateReferenceLane(BotLaneAuthoringTool tool)
    {
        if (tool.referenceLaneRoot != null)
            return tool.referenceLaneRoot;

        Transform parent = tool.track != null ? tool.track.transform : tool.transform;
        Transform lane = FindChildByNameRecursive(parent, "CarrilBase_Ref");
        if (lane == null)
        {
            var laneGo = new GameObject("CarrilBase_Ref");
            Undo.RegisterCreatedObjectUndo(laneGo, "Crear carril base");
            laneGo.transform.SetParent(parent, false);
            lane = laneGo.transform;
        }

        Undo.RecordObject(tool, "Asignar carril base");
        tool.referenceLaneRoot = lane;
        MarkDirty(tool, lane.gameObject);
        return lane;
    }

    private static Transform FindDirectChildByName(Transform root, string name)
    {
        if (root == null)
            return null;
        for (int i = 0; i < root.childCount; i++)
        {
            Transform c = root.GetChild(i);
            if (c.name == name)
                return c;
        }
        return null;
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

    private static void RenumberPoints(Transform lane)
    {
        if (lane == null)
            return;
        for (int i = 0; i < lane.childCount; i++)
            lane.GetChild(i).name = $"P_{i:000}";
    }

    private static void MarkDirty(BotLaneAuthoringTool tool, GameObject go)
    {
        if (go != null)
            EditorUtility.SetDirty(go);
        if (tool != null)
            EditorUtility.SetDirty(tool);
        if (tool != null && tool.track != null)
            EditorUtility.SetDirty(tool.track.gameObject);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
