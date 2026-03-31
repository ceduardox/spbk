using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BotLaneAuthoringTool : MonoBehaviour
{
    [Header("Referencias")]
    [InspectorName("Pista (Track)")]
    public Track track;
    [InspectorName("Raiz de Checkpoints")]
    [Tooltip("Raiz opcional de respaldo para leer checkpoints cuando Track.checkpoints esta vacio.")]
    public Transform checkpointsRoot;
    [InspectorName("Raiz BotLanes")]
    [Tooltip("Si es null, se creara un hijo llamado BotLanes dentro de la pista.")]
    public Transform botLanesRoot;

    [Header("Generacion de Carriles")]
    [InspectorName("Cantidad de Carriles")]
    [Range(3, 9)] public int laneCount = 5;
    [InspectorName("Subdivisiones por Segmento")]
    [Range(2, 20)] public int subdivisionsPerSegment = 6;
    [InspectorName("Ancho Medio de Carril")]
    [Range(0.5f, 5f)] public float laneHalfWidth = 1.8f;
    [InspectorName("Escala Ancho en Curva")]
    [Range(0.1f, 1f)] public float widthScaleOnCurve = 0.45f;
    [InspectorName("Circuito Cerrado (Loop)")]
    public bool closedLoop = true;
    [InspectorName("Alinear Altura al Centro")]
    public bool alignPointHeightToCenter = true;

    [Header("Uniformado de Carriles")]
    [InspectorName("Espaciado Uniforme")]
    [Tooltip("Distancia objetivo entre puntos al uniformar cada lane.")]
    [Range(0.2f, 20f)] public float uniformPointSpacing = 1.2f;
    [InspectorName("Minimo de Puntos")]
    [Tooltip("Cantidad minima de puntos por lane despues del uniformado.")]
    [Range(2, 400)] public int minUniformPoints = 48;

    [Header("Auto Ajuste Inteligente")]
    [InspectorName("Espaciado Minimo Inteligente")]
    [Range(0.2f, 20f)] public float smartSpacingMin = 0.9f;
    [InspectorName("Espaciado Maximo Inteligente")]
    [Range(0.2f, 20f)] public float smartSpacingMax = 2.2f;
    [InspectorName("Influencia de Curvas")]
    [Tooltip("0 = prioriza longitud total, 1 = prioriza curvatura local.")]
    [Range(0f, 1f)] public float smartCurveInfluence = 0.65f;
    [InspectorName("Puntos Minimos Inteligentes")]
    [Range(8, 800)] public int smartTargetPointsMin = 90;
    [InspectorName("Puntos Maximos Inteligentes")]
    [Range(8, 1200)] public int smartTargetPointsMax = 260;
    [InspectorName("Suavizado Inteligente")]
    [Tooltip("Suaviza micro-zigzag despues del remuestreo.")]
    [Range(0f, 0.5f)] public float smartSmoothing = 0.12f;
    [InspectorName("Iteraciones de Suavizado")]
    [Range(0, 4)] public int smartSmoothingPasses = 1;

    [Header("Proyeccion al Carril (Opcional)")]
    [InspectorName("Mascara de Carril")]
    [Tooltip("Capas usadas para detectar superficie de pista al proyectar puntos.")]
    public LayerMask roadProjectionMask = ~0;
    [InspectorName("Altura Raycast Proyeccion")]
    [Range(10f, 3000f)] public float roadProjectionRayHeight = 450f;
    [InspectorName("Radio Proyeccion")]
    [Range(0f, 3f)] public float roadProjectionRadius = 0.22f;
    [InspectorName("Offset sobre Suelo")]
    [Range(0f, 1f)] public float roadProjectionSurfaceOffset = 0.03f;
    [InspectorName("Max Delta Y")]
    [Tooltip("Si la proyeccion vertical supera este valor, se ignora para evitar saltos a otra geometria.")]
    [Range(0.1f, 300f)] public float roadProjectionMaxDeltaY = 35f;

    [Header("Carril Base Manual (Opcional)")]
    [InspectorName("Carril Base (Centerline)")]
    [Tooltip("Si lo asignas, puedes generar todos los carriles siguiendo EXACTAMENTE esta ruta base dibujada a mano.")]
    public Transform referenceLaneRoot;
    [InspectorName("Editar Carril Base en Scene")]
    [Tooltip("Si esta activo, Shift+Click y drag en Scene editan el carril base (linea roja) en vez del carril activo.")]
    public bool editReferenceLaneInScene = false;
    [InspectorName("Auto Inicializar Carril Base")]
    [Tooltip("Si activas edicion de carril base y no hay puntos, crea la base automaticamente desde checkpoints.")]
    public bool autoSeedReferenceLaneWhenEdit = true;

    [Header("Gizmos")]
    [InspectorName("Mostrar Gizmos")]
    public bool drawGizmos = true;
    [InspectorName("Dibujar Siempre (Scene)")]
    [Tooltip("Si esta activo, las lineas se ven siempre en Scene aun sin seleccionar el objeto.")]
    public bool drawAlways = true;
    [InspectorName("Color de Centro")]
    public Color centerColor = new Color(0.95f, 0.95f, 0.95f, 0.9f);
    [InspectorName("Color de Carriles")]
    public Color laneColor = new Color(0.25f, 0.85f, 1f, 0.9f);
    [InspectorName("Grosor de Linea")]
    [Range(0.01f, 0.8f)] public float gizmoLineThickness = 0.14f;
    [InspectorName("Radio de Puntos")]
    [Range(0.02f, 1.2f)] public float gizmoPointRadius = 0.1f;

    [Header("Edicion Rapida (Scene)")]
    [InspectorName("Modo Edicion Rapida")]
    [Tooltip("Shift + Click izquierdo: agrega punto. Shift + Click derecho: elimina punto cercano.")]
    public bool sceneEditEnabled = false;
    [InspectorName("Carril Activo")]
    [Range(1, 9)] public int editLane = 1;
    [InspectorName("Insertar en Segmento Cercano")]
    [Tooltip("Si esta activo, inserta el punto en el tramo mas cercano. Si no, lo agrega al final.")]
    public bool insertOnNearestSegment = true;
    [InspectorName("Ajustar al Piso al Crear/Mover")]
    public bool snapPointToGround = true;
    [InspectorName("Altura de Raycast Snap")]
    [Range(10f, 2000f)] public float snapRayHeight = 300f;
    [InspectorName("Distancia Minima entre Puntos")]
    [Range(0.05f, 10f)] public float pointMinSpacing = 0.45f;
    [InspectorName("Radio Eliminacion Punto")]
    [Range(0.1f, 8f)] public float removePointRadius = 1.2f;
    [InspectorName("Mostrar Etiquetas P_XXX")]
    public bool drawPointLabels = false;

    [ContextMenu("BotLanes/Generar desde Checkpoints")]
    public void AutoBuildFromCheckpoints()
    {
        ResolveReferences();

        var anchors = CollectAnchorPoints();
        if (anchors.Count < 2)
        {
            Debug.LogWarning("BotLaneAuthoringTool: No hay checkpoints suficientes para generar BotLanes.");
            return;
        }

        if (botLanesRoot == null)
            botLanesRoot = EnsureBotLanesRoot();
        if (botLanesRoot == null)
            return;

        ClearLanesInternal();

        int sub = Mathf.Max(2, subdivisionsPerSegment);
        int lanes = Mathf.Clamp(laneCount, 3, 9);
        float width = Mathf.Max(0.2f, laneHalfWidth);
        float widthCurve = Mathf.Clamp(widthScaleOnCurve, 0.1f, 1f);

        var center = BuildCenterline(anchors, sub, closedLoop);
        if (center.Count < 4)
        {
            Debug.LogWarning("BotLaneAuthoringTool: La centerline generada es insuficiente.");
            return;
        }

        float[] factors = BuildLaneFactors(lanes);
        var laneRoots = new Transform[lanes];
        for (int l = 0; l < lanes; l++)
        {
            var laneGo = new GameObject($"Lane_{(l + 1):00}");
            laneGo.transform.SetParent(botLanesRoot, false);
            laneRoots[l] = laneGo.transform;
        }

        for (int i = 0; i < center.Count; i++)
        {
            Vector3 prev = center[(i - 1 + center.Count) % center.Count];
            Vector3 curr = center[i];
            Vector3 next = center[(i + 1) % center.Count];

            Vector3 tan1 = (curr - prev).normalized;
            Vector3 tan2 = (next - curr).normalized;
            Vector3 tangent = tan1 + tan2;
            if (tangent.sqrMagnitude < 0.0001f)
                tangent = (next - prev).normalized;
            if (tangent.sqrMagnitude < 0.0001f)
                tangent = Vector3.forward;

            float angle = Vector3.Angle(tan1, tan2);
            float curve = Mathf.Clamp01((angle - 5f) / 85f);
            float widthScale = Mathf.Lerp(1f, widthCurve, curve);
            Vector3 right = Vector3.Cross(Vector3.up, tangent.normalized).normalized;

            for (int l = 0; l < lanes; l++)
            {
                float offset = factors[l] * width * widthScale;
                Vector3 point = curr + right * offset;
                if (alignPointHeightToCenter)
                    point.y = curr.y;

                var pGo = new GameObject($"P_{i:000}");
                pGo.transform.SetParent(laneRoots[l], false);
                pGo.transform.position = point;
            }
        }

        LinkTrackSettings(botLanesRoot, lanes);
        MarkDirtySafe();
        Debug.Log($"BotLaneAuthoringTool: Generadas {lanes} lineas en '{botLanesRoot.name}'.");
    }

    [ContextMenu("BotLanes/Generar + Uniformar (Recomendado)")]
    public void AutoBuildAndUniformLanes()
    {
        AutoBuildFromCheckpoints();
        UniformResampleLanes();
    }

    [ContextMenu("BotLanes/Generar desde Carril Base (Mapa)")]
    public void BuildFromReferenceLane()
    {
        ResolveReferences();

        Transform sourceLane = referenceLaneRoot;
        if (sourceLane == null)
            sourceLane = FindLaneByEditIndex();
        if (sourceLane == null)
        {
            Debug.LogWarning("BotLaneAuthoringTool: asigna 'Carril Base (Centerline)' o define un 'Carril Activo' valido.");
            return;
        }

        var center = CollectPointsFromLane(sourceLane);
        if (center.Count < 3)
        {
            Debug.LogWarning("BotLaneAuthoringTool: el carril base necesita al menos 3 puntos.");
            return;
        }

        if (closedLoop)
            RemoveClosingDuplicate(center);

        if (botLanesRoot == null)
            botLanesRoot = EnsureBotLanesRoot();
        if (botLanesRoot == null)
            return;

        ClearLanesInternal();
        BuildLanesFromCenterPoints(center, sourceLane.name);
    }

    [ContextMenu("Carril Base/Crear o Actualizar desde Checkpoints")]
    public void RebuildReferenceLaneFromCheckpoints()
    {
        ResolveReferences();
        var anchors = CollectAnchorPoints();
        if (anchors.Count < 2)
        {
            Debug.LogWarning("BotLaneAuthoringTool: no hay checkpoints suficientes para reconstruir carril base.");
            return;
        }

        int sub = Mathf.Max(2, subdivisionsPerSegment);
        var center = BuildCenterline(anchors, sub, closedLoop);
        if (center.Count < 3)
        {
            Debug.LogWarning("BotLaneAuthoringTool: centerline insuficiente para carril base.");
            return;
        }

        Transform lane = EnsureReferenceLaneRoot();
        if (lane == null)
            return;

        ReplaceLanePoints(lane, center);
        MarkDirtySafe();
        Debug.Log($"BotLaneAuthoringTool: carril base actualizado ({center.Count} puntos).");
    }

    [ContextMenu("BotLanes/Proyectar Puntos al Carril (Opcional)")]
    public void ProjectLanesToRoad()
    {
        ResolveReferences();
        if (botLanesRoot == null)
        {
            Debug.LogWarning("BotLaneAuthoringTool: no existe BotLanes para proyectar.");
            return;
        }

        int total = 0;
        int projected = 0;
        int ignored = 0;
        for (int laneIdx = 0; laneIdx < botLanesRoot.childCount; laneIdx++)
        {
            Transform lane = botLanesRoot.GetChild(laneIdx);
            for (int i = 0; i < lane.childCount; i++)
            {
                Transform p = lane.GetChild(i);
                total++;

                if (TryProjectPointToRoad(p.position, out Vector3 projectedPos))
                {
                    float dy = Mathf.Abs(projectedPos.y - p.position.y);
                    if (dy <= roadProjectionMaxDeltaY)
                    {
                        p.position = projectedPos;
                        projected++;
                    }
                    else
                    {
                        ignored++;
                    }
                }
                else
                {
                    ignored++;
                }
            }
        }

        MarkDirtySafe();
        Debug.Log($"BotLaneAuthoringTool: proyectados {projected}/{total} puntos al carril (ignorados: {ignored}).");
    }

    [ContextMenu("BotLanes/Generar + Uniformar + Proyectar (Opcional)")]
    public void AutoBuildUniformAndProjectLanes()
    {
        AutoBuildFromCheckpoints();
        UniformResampleLanes();
        ProjectLanesToRoad();
    }

    [ContextMenu("BotLanes/Generar (Carril Base) + Uniformar + Proyectar")]
    public void BuildFromReferenceUniformAndProject()
    {
        BuildFromReferenceLane();
        UniformResampleLanes();
        ProjectLanesToRoad();
    }

    [ContextMenu("BotLanes/Limpiar Carriles Generados")]
    public void ClearGeneratedLanes()
    {
        ResolveReferences();
        if (botLanesRoot == null)
            return;
        ClearLanesInternal();
        MarkDirtySafe();
    }

    [ContextMenu("BotLanes/Uniformar Puntos de Carriles")]
    public void UniformResampleLanes()
    {
        ResolveReferences();
        if (botLanesRoot == null)
        {
            Debug.LogWarning("BotLaneAuthoringTool: no existe BotLanes para uniformar.");
            return;
        }

        float spacing = Mathf.Max(0.2f, uniformPointSpacing);
        int minPoints = Mathf.Max(2, minUniformPoints);
        int updated = 0;

        for (int i = 0; i < botLanesRoot.childCount; i++)
        {
            Transform laneRoot = botLanesRoot.GetChild(i);
            var points = new List<Vector3>(laneRoot.childCount);
            for (int p = 0; p < laneRoot.childCount; p++)
                points.Add(laneRoot.GetChild(p).position);

            if (points.Count < 2)
                continue;

            bool loop = closedLoop && points.Count > 2;
            var sampled = ResamplePolyline(points, spacing, loop, minPoints);
            if (sampled.Count < 2)
                continue;

            if (!loop)
            {
                sampled[0] = points[0];
                sampled[sampled.Count - 1] = points[points.Count - 1];
            }

            ReplaceLanePoints(laneRoot, sampled);
            updated++;
        }

        MarkDirtySafe();
        Debug.Log($"BotLaneAuthoringTool: lanes uniformadas = {updated} (espaciado ~{spacing:0.##}).");
    }

    [ContextMenu("BotLanes/Auto Ajuste Inteligente (Curvas/Rectas)")]
    public void SmartAutoAdjustLanes()
    {
        ResolveReferences();
        if (botLanesRoot == null)
        {
            Debug.LogWarning("BotLaneAuthoringTool: no existe BotLanes para auto-ajuste.");
            return;
        }

        float minSpacing = Mathf.Max(0.2f, Mathf.Min(smartSpacingMin, smartSpacingMax));
        float maxSpacing = Mathf.Max(minSpacing, Mathf.Max(smartSpacingMin, smartSpacingMax));
        int minPtsClamp = Mathf.Max(8, Mathf.Min(smartTargetPointsMin, smartTargetPointsMax));
        int maxPtsClamp = Mathf.Max(minPtsClamp, Mathf.Max(smartTargetPointsMin, smartTargetPointsMax));
        float curveInfluence = Mathf.Clamp01(smartCurveInfluence);

        int updated = 0;
        float accumSpacing = 0f;
        for (int i = 0; i < botLanesRoot.childCount; i++)
        {
            Transform laneRoot = botLanesRoot.GetChild(i);
            var points = new List<Vector3>(laneRoot.childCount);
            for (int p = 0; p < laneRoot.childCount; p++)
                points.Add(laneRoot.GetChild(p).position);

            if (points.Count < 2)
                continue;

            bool loop = closedLoop && points.Count > 2;
            float length = GetPolylineLength(points, loop);
            if (length <= 0.001f)
                continue;

            float curve01 = EstimateLaneCurvature01(points, loop);

            // spacing by total length target
            float targetPointsByLength = Mathf.Lerp(maxPtsClamp, minPtsClamp, Mathf.Clamp01(length / 220f));
            float baseSpacing = length / Mathf.Max(2f, targetPointsByLength);
            // spacing by curvature (curvier -> denser)
            float curveSpacing = Mathf.Lerp(maxSpacing, minSpacing, curve01);
            float spacing = Mathf.Lerp(baseSpacing, curveSpacing, curveInfluence);
            spacing = Mathf.Clamp(spacing, minSpacing, maxSpacing);

            int minPoints = Mathf.Clamp(Mathf.RoundToInt(length / Mathf.Max(0.05f, spacing)), minPtsClamp, maxPtsClamp);
            var sampled = ResamplePolyline(points, spacing, loop, minPoints);
            if (sampled.Count < 2)
                continue;

            if (smartSmoothingPasses > 0 && smartSmoothing > 0f)
                SmoothLanePoints(sampled, loop, smartSmoothingPasses, smartSmoothing);

            if (!loop)
            {
                sampled[0] = points[0];
                sampled[sampled.Count - 1] = points[points.Count - 1];
            }

            ReplaceLanePoints(laneRoot, sampled);
            updated++;
            accumSpacing += spacing;
        }

        MarkDirtySafe();
        if (updated <= 0)
            Debug.LogWarning("BotLaneAuthoringTool: auto-ajuste no encontro lanes validos.");
        else
            Debug.Log($"BotLaneAuthoringTool: auto-ajuste aplicado a {updated} lanes (espaciado medio ~{(accumSpacing / updated):0.##}).");
    }

    private void ResolveReferences()
    {
        if (track == null)
            track = GetComponent<Track>();
        if (track == null)
            track = GetComponentInParent<Track>();

        if (checkpointsRoot == null && track != null)
            checkpointsRoot = track.checkpointsList;
    }

    private Transform EnsureBotLanesRoot()
    {
        Transform parent = track != null ? track.transform : transform;
        Transform found = FindChildByNameRecursive(parent, "BotLanes");
        if (found != null)
            return found;

        var root = new GameObject("BotLanes");
        root.transform.SetParent(parent, false);
        return root.transform;
    }

    private Transform EnsureReferenceLaneRoot()
    {
        if (referenceLaneRoot != null)
            return referenceLaneRoot;

        Transform parent = track != null ? track.transform : transform;
        Transform found = FindChildByNameRecursive(parent, "CarrilBase_Ref");
        if (found != null)
        {
            referenceLaneRoot = found;
            return referenceLaneRoot;
        }

        var go = new GameObject("CarrilBase_Ref");
        go.transform.SetParent(parent, false);
        referenceLaneRoot = go.transform;
        return referenceLaneRoot;
    }

    private void ClearLanesInternal()
    {
        if (botLanesRoot == null)
            return;

        var toDelete = new List<GameObject>();
        for (int i = 0; i < botLanesRoot.childCount; i++)
            toDelete.Add(botLanesRoot.GetChild(i).gameObject);

        ClearEditorSelectionIfDeleting(toDelete);
        for (int i = 0; i < toDelete.Count; i++)
        {
            if (Application.isPlaying)
                Destroy(toDelete[i]);
            else
                DestroyImmediate(toDelete[i]);
        }
    }

    private List<Vector3> CollectAnchorPoints()
    {
        var anchorsFromTrackArray = new List<Vector3>();
        var anchorsFromRoot = new List<Vector3>();

        if (track != null && track.checkpoints != null && track.checkpoints.Length > 0)
        {
            for (int i = 0; i < track.checkpoints.Length; i++)
            {
                var cp = track.checkpoints[i];
                if (cp != null)
                    anchorsFromTrackArray.Add(cp.transform.position);
            }
        }

        if (checkpointsRoot != null)
        {
            for (int i = 0; i < checkpointsRoot.childCount; i++)
                anchorsFromRoot.Add(checkpointsRoot.GetChild(i).position);
        }

        // In editor, Track.checkpoints can be partially initialized.
        // Prefer the root list when it has more complete data.
        var anchors = new List<Vector3>();
        if (anchorsFromRoot.Count >= 2 && anchorsFromRoot.Count >= anchorsFromTrackArray.Count)
            anchors.AddRange(anchorsFromRoot);
        else if (anchorsFromTrackArray.Count >= 2)
            anchors.AddRange(anchorsFromTrackArray);
        else if (anchorsFromRoot.Count > 0)
            anchors.AddRange(anchorsFromRoot);
        else
            anchors.AddRange(anchorsFromTrackArray);

        // Only append finish line when the path already has enough anchors.
        // This prevents generating a tiny straight lane from incomplete data.
        if (anchors.Count >= 2)
            TryAddFinishLineAnchor(anchors);

        return anchors;
    }

    private void TryAddFinishLineAnchor(List<Vector3> anchors)
    {
        if (anchors == null || track == null || track.finishLine == null)
            return;

        Vector3 finishPos = track.finishLine.transform.position;
        const float minSqrDist = 2.0f * 2.0f;
        for (int i = 0; i < anchors.Count; i++)
        {
            if ((anchors[i] - finishPos).sqrMagnitude <= minSqrDist)
                return;
        }

        anchors.Add(finishPos);
    }

    private static List<Vector3> BuildCenterline(List<Vector3> anchors, int subdivisions, bool loop)
    {
        var result = new List<Vector3>(anchors.Count * (subdivisions + 1));
        int n = anchors.Count;
        if (n < 2)
            return result;

        int segments = loop ? n : n - 1;
        for (int i = 0; i < segments; i++)
        {
            Vector3 p0 = anchors[Wrap(i - 1, n)];
            Vector3 p1 = anchors[Wrap(i, n)];
            Vector3 p2 = anchors[Wrap(i + 1, n)];
            Vector3 p3 = anchors[Wrap(i + 2, n)];

            if (!loop)
            {
                p0 = anchors[Mathf.Max(0, i - 1)];
                p1 = anchors[i];
                p2 = anchors[Mathf.Min(n - 1, i + 1)];
                p3 = anchors[Mathf.Min(n - 1, i + 2)];
            }

            for (int s = 0; s < subdivisions; s++)
            {
                float t = (float)s / subdivisions;
                result.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }

        if (!loop)
            result.Add(anchors[n - 1]);
        return result;
    }

    private void BuildLanesFromCenterPoints(List<Vector3> center, string sourceLaneName)
    {
        if (center == null || center.Count < 2)
            return;

        int lanes = Mathf.Clamp(laneCount, 3, 9);
        float width = Mathf.Max(0.2f, laneHalfWidth);
        float widthCurve = Mathf.Clamp(widthScaleOnCurve, 0.1f, 1f);
        float[] factors = BuildLaneFactors(lanes);

        var laneRoots = new Transform[lanes];
        for (int l = 0; l < lanes; l++)
        {
            var laneGo = new GameObject($"Lane_{(l + 1):00}");
            laneGo.transform.SetParent(botLanesRoot, false);
            laneRoots[l] = laneGo.transform;
        }

        int count = center.Count;
        for (int i = 0; i < count; i++)
        {
            Vector3 prev = closedLoop ? center[(i - 1 + count) % count] : center[Mathf.Max(0, i - 1)];
            Vector3 curr = center[i];
            Vector3 next = closedLoop ? center[(i + 1) % count] : center[Mathf.Min(count - 1, i + 1)];

            Vector3 tan1 = (curr - prev).normalized;
            Vector3 tan2 = (next - curr).normalized;
            Vector3 tangent = tan1 + tan2;
            if (tangent.sqrMagnitude < 0.0001f)
                tangent = (next - prev).normalized;
            if (tangent.sqrMagnitude < 0.0001f)
                tangent = Vector3.forward;

            float angle = Vector3.Angle(tan1, tan2);
            float curve = Mathf.Clamp01((angle - 5f) / 85f);
            float widthScale = Mathf.Lerp(1f, widthCurve, curve);
            Vector3 right = Vector3.Cross(Vector3.up, tangent.normalized).normalized;

            for (int l = 0; l < lanes; l++)
            {
                float offset = factors[l] * width * widthScale;
                Vector3 point = curr + right * offset;
                if (alignPointHeightToCenter)
                    point.y = curr.y;

                var pGo = new GameObject($"P_{i:000}");
                pGo.transform.SetParent(laneRoots[l], false);
                pGo.transform.position = point;
            }
        }

        LinkTrackSettings(botLanesRoot, lanes);
        MarkDirtySafe();
        Debug.Log($"BotLaneAuthoringTool: generadas {lanes} lanes desde carril base '{sourceLaneName}'.");
    }

    private Transform FindLaneByEditIndex()
    {
        if (botLanesRoot == null)
            return null;
        string laneName = $"Lane_{Mathf.Clamp(editLane, 1, 99):00}";
        for (int i = 0; i < botLanesRoot.childCount; i++)
        {
            var c = botLanesRoot.GetChild(i);
            if (c.name == laneName)
                return c;
        }
        return null;
    }

    private static List<Vector3> CollectPointsFromLane(Transform laneRoot)
    {
        var points = new List<Vector3>();
        if (laneRoot == null)
            return points;
        for (int i = 0; i < laneRoot.childCount; i++)
            points.Add(laneRoot.GetChild(i).position);
        return points;
    }

    private static void RemoveClosingDuplicate(List<Vector3> points)
    {
        if (points == null || points.Count < 3)
            return;
        if ((points[0] - points[points.Count - 1]).sqrMagnitude <= 0.0004f)
            points.RemoveAt(points.Count - 1);
    }

    private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    private static int Wrap(int index, int length)
    {
        if (length <= 0)
            return 0;
        int wrapped = index % length;
        return wrapped < 0 ? wrapped + length : wrapped;
    }

    private static float[] BuildLaneFactors(int lanes)
    {
        int count = Mathf.Max(1, lanes);
        var factors = new float[count];
        if (count == 1)
        {
            factors[0] = 0f;
            return factors;
        }

        float denom = count - 1f;
        for (int i = 0; i < count; i++)
            factors[i] = Mathf.Lerp(-1f, 1f, i / denom);
        return factors;
    }

    private static List<Vector3> ResamplePolyline(List<Vector3> points, float spacing, bool loop, int minPoints)
    {
        var result = new List<Vector3>();
        if (points == null || points.Count < 2)
            return result;

        float total = GetPolylineLength(points, loop);
        if (total <= 0.0001f)
        {
            result.Add(points[0]);
            result.Add(points[points.Count - 1]);
            return result;
        }

        int count;
        if (loop)
        {
            count = Mathf.Max(minPoints, Mathf.RoundToInt(total / Mathf.Max(0.05f, spacing)));
            count = Mathf.Max(3, count);
            float step = total / count;
            for (int i = 0; i < count; i++)
                result.Add(EvaluateAtDistance(points, loop, step * i));
        }
        else
        {
            count = Mathf.Max(minPoints, Mathf.RoundToInt(total / Mathf.Max(0.05f, spacing)) + 1);
            count = Mathf.Max(2, count);
            float step = total / (count - 1);
            for (int i = 0; i < count; i++)
                result.Add(EvaluateAtDistance(points, loop, step * i));
        }

        return result;
    }

    private static float GetPolylineLength(List<Vector3> points, bool loop)
    {
        if (points == null || points.Count < 2)
            return 0f;

        float total = 0f;
        int segments = loop ? points.Count : points.Count - 1;
        for (int i = 0; i < segments; i++)
        {
            int j = (i + 1) % points.Count;
            total += Vector3.Distance(points[i], points[j]);
        }

        return total;
    }

    private static float EstimateLaneCurvature01(List<Vector3> points, bool loop)
    {
        if (points == null || points.Count < 3)
            return 0.2f;

        int count = points.Count;
        int start = loop ? 0 : 1;
        int endExclusive = loop ? count : count - 1;

        float sum = 0f;
        int n = 0;
        for (int i = start; i < endExclusive; i++)
        {
            int i0 = loop ? (i - 1 + count) % count : i - 1;
            int i1 = i;
            int i2 = loop ? (i + 1) % count : i + 1;

            Vector3 a = (points[i1] - points[i0]).normalized;
            Vector3 b = (points[i2] - points[i1]).normalized;
            if (a.sqrMagnitude < 0.0001f || b.sqrMagnitude < 0.0001f)
                continue;

            float angle = Vector3.Angle(a, b);
            sum += angle;
            n++;
        }

        if (n == 0)
            return 0.2f;

        float avg = sum / n;
        return Mathf.Clamp01((avg - 4f) / 38f);
    }

    private static void SmoothLanePoints(List<Vector3> points, bool loop, int passes, float strength)
    {
        if (points == null || points.Count < 3 || passes <= 0 || strength <= 0f)
            return;

        strength = Mathf.Clamp(strength, 0f, 0.5f);

        for (int pass = 0; pass < passes; pass++)
        {
            var tmp = new List<Vector3>(points);
            int count = points.Count;
            int start = loop ? 0 : 1;
            int endExclusive = loop ? count : count - 1;

            for (int i = start; i < endExclusive; i++)
            {
                int i0 = loop ? (i - 1 + count) % count : i - 1;
                int i2 = loop ? (i + 1) % count : i + 1;
                Vector3 neighborAvg = (points[i0] + points[i2]) * 0.5f;
                tmp[i] = Vector3.Lerp(points[i], neighborAvg, strength);
            }

            for (int i = start; i < endExclusive; i++)
                points[i] = tmp[i];
        }
    }

    private static Vector3 EvaluateAtDistance(List<Vector3> points, bool loop, float distance)
    {
        if (points == null || points.Count == 0)
            return Vector3.zero;
        if (points.Count == 1)
            return points[0];

        float total = GetPolylineLength(points, loop);
        if (total <= 0.0001f)
            return points[0];

        float d = loop ? Mathf.Repeat(distance, total) : Mathf.Clamp(distance, 0f, total);
        int segments = loop ? points.Count : points.Count - 1;
        float accum = 0f;

        for (int i = 0; i < segments; i++)
        {
            int j = (i + 1) % points.Count;
            Vector3 a = points[i];
            Vector3 b = points[j];
            float len = Vector3.Distance(a, b);
            if (len <= 0.00001f)
                continue;

            if (accum + len >= d)
            {
                float t = (d - accum) / len;
                return Vector3.Lerp(a, b, t);
            }

            accum += len;
        }

        return loop ? points[0] : points[points.Count - 1];
    }

    private void ReplaceLanePoints(Transform laneRoot, List<Vector3> points)
    {
        if (laneRoot == null || points == null || points.Count == 0)
            return;

        var toDelete = new List<GameObject>(laneRoot.childCount);
        for (int i = 0; i < laneRoot.childCount; i++)
            toDelete.Add(laneRoot.GetChild(i).gameObject);

        ClearEditorSelectionIfDeleting(toDelete);
        for (int i = 0; i < toDelete.Count; i++)
        {
            if (Application.isPlaying)
                Destroy(toDelete[i]);
            else
                DestroyImmediate(toDelete[i]);
        }

        for (int i = 0; i < points.Count; i++)
        {
            var pGo = new GameObject($"P_{i:000}");
            pGo.transform.SetParent(laneRoot, false);
            pGo.transform.position = points[i];
        }
    }

    private bool TryProjectPointToRoad(Vector3 source, out Vector3 projected)
    {
        float h = Mathf.Max(10f, roadProjectionRayHeight);
        float r = Mathf.Max(0f, roadProjectionRadius);
        float maxDist = h * 2f;
        int mask = roadProjectionMask.value;
        if (mask == 0)
            mask = ~0;

        // 1) Cast from above to down.
        Vector3 fromTop = source + Vector3.up * h;
        if (CastToRoad(new Ray(fromTop, Vector3.down), r, maxDist, mask, out RaycastHit hitDown))
        {
            projected = hitDown.point + hitDown.normal * roadProjectionSurfaceOffset;
            return true;
        }

        // 2) Fallback from below to up.
        Vector3 fromBottom = source - Vector3.up * h;
        if (CastToRoad(new Ray(fromBottom, Vector3.up), r, maxDist, mask, out RaycastHit hitUp))
        {
            projected = hitUp.point + hitUp.normal * roadProjectionSurfaceOffset;
            return true;
        }

        projected = source;
        return false;
    }

    private static bool CastToRoad(Ray ray, float radius, float maxDistance, int mask, out RaycastHit hit)
    {
        if (radius <= 0.0001f)
            return Physics.Raycast(ray, out hit, maxDistance, mask, QueryTriggerInteraction.Ignore);
        return Physics.SphereCast(ray, radius, out hit, maxDistance, mask, QueryTriggerInteraction.Ignore);
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

    private void LinkTrackSettings(Transform lanesRoot, int lanes)
    {
        if (lanesRoot == null)
            return;

        BotAITrackSettings settings = null;
        if (track != null)
            settings = track.GetComponent<BotAITrackSettings>();
        if (settings == null)
            settings = GetComponent<BotAITrackSettings>();
        if (settings == null)
            settings = gameObject.AddComponent<BotAITrackSettings>();

        settings.botLanesRoot = lanesRoot;
        settings.generatedLaneCount = Mathf.Clamp(lanes, 3, 9);
    }

    private void OnDrawGizmos()
    {
        if (!drawAlways)
            return;
        DrawLaneGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (drawAlways)
            return;
        DrawLaneGizmos();
    }

    private void DrawLaneGizmos()
    {
        if (!drawGizmos)
            return;

        ResolveReferences();
        if (botLanesRoot == null)
            return;
        int onlyLaneIndex = GetOnlyForcedLaneIndex();

        Gizmos.color = centerColor;
        var centerPoints = new List<Vector3>();
        if (referenceLaneRoot != null && referenceLaneRoot.childCount >= 2)
            centerPoints = CollectPointsFromLane(referenceLaneRoot);
        else
            centerPoints = CollectAnchorPoints();

        for (int i = 0; i < centerPoints.Count; i++)
        {
            int j = (i + 1) % centerPoints.Count;
            if (i < centerPoints.Count - 1 || closedLoop)
                DrawThickLine(centerPoints[i], centerPoints[j], gizmoLineThickness * 0.9f);
            Gizmos.DrawSphere(centerPoints[i], gizmoPointRadius * 0.75f);
        }

        Gizmos.color = laneColor;
        for (int l = 0; l < botLanesRoot.childCount; l++)
        {
            if (onlyLaneIndex >= 0 && l != onlyLaneIndex)
                continue;

            var lane = botLanesRoot.GetChild(l);
            if (lane.childCount < 2)
                continue;

            for (int i = 0; i < lane.childCount; i++)
            {
                int j = (i + 1) % lane.childCount;
                if (i < lane.childCount - 1 || closedLoop)
                    DrawThickLine(lane.GetChild(i).position, lane.GetChild(j).position, gizmoLineThickness);
                Gizmos.DrawSphere(lane.GetChild(i).position, gizmoPointRadius);
            }
        }
    }

    private static void DrawThickLine(Vector3 a, Vector3 b, float thickness)
    {
        if (thickness <= 0.001f)
        {
            Gizmos.DrawLine(a, b);
            return;
        }

        Vector3 dir = (b - a).normalized;
        if (dir.sqrMagnitude < 0.0001f)
            return;

        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        if (right.sqrMagnitude < 0.0001f)
            right = Vector3.Cross(Vector3.right, dir).normalized;
        Vector3 up = Vector3.Cross(dir, right).normalized;

        float t = thickness * 0.5f;
        Gizmos.DrawLine(a, b);
        Gizmos.DrawLine(a + right * t, b + right * t);
        Gizmos.DrawLine(a - right * t, b - right * t);
        Gizmos.DrawLine(a + up * t, b + up * t);
        Gizmos.DrawLine(a - up * t, b - up * t);
    }

    private int GetOnlyForcedLaneIndex()
    {
        if (botLanesRoot == null || botLanesRoot.childCount == 0 || track == null)
            return -1;

        var settings = track.GetComponent<BotAITrackSettings>();
        if (settings == null || !settings.useOverrides || !settings.enableLaneTestMode || !settings.showOnlyForcedLaneGizmo)
            return -1;

        return Mathf.Clamp(settings.forcedLaneNumber - 1, 0, botLanesRoot.childCount - 1);
    }

    private void MarkDirtySafe()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
        if (botLanesRoot != null)
            UnityEditor.EditorUtility.SetDirty(botLanesRoot.gameObject);
        if (track != null)
            UnityEditor.EditorUtility.SetDirty(track.gameObject);
        if (referenceLaneRoot != null)
            UnityEditor.EditorUtility.SetDirty(referenceLaneRoot.gameObject);
#endif
    }

    private static void ClearEditorSelectionIfDeleting(List<GameObject> toDelete)
    {
#if UNITY_EDITOR
        if (toDelete == null || toDelete.Count == 0)
            return;

        var selected = UnityEditor.Selection.activeTransform;
        if (selected == null)
            return;

        for (int i = 0; i < toDelete.Count; i++)
        {
            var go = toDelete[i];
            if (go == null)
                continue;

            if (selected == go.transform || selected.IsChildOf(go.transform))
            {
                UnityEditor.Selection.activeTransform = null;
                return;
            }
        }
#endif
    }

    private void OnValidate()
    {
        laneCount = Mathf.Clamp(laneCount, 3, 9);
        subdivisionsPerSegment = Mathf.Clamp(subdivisionsPerSegment, 2, 20);
        laneHalfWidth = Mathf.Clamp(laneHalfWidth, 0.5f, 5f);
        widthScaleOnCurve = Mathf.Clamp(widthScaleOnCurve, 0.1f, 1f);
        uniformPointSpacing = Mathf.Clamp(uniformPointSpacing, 0.2f, 20f);
        minUniformPoints = Mathf.Clamp(minUniformPoints, 2, 400);
        smartSpacingMin = Mathf.Clamp(smartSpacingMin, 0.2f, 20f);
        smartSpacingMax = Mathf.Clamp(smartSpacingMax, 0.2f, 20f);
        smartCurveInfluence = Mathf.Clamp01(smartCurveInfluence);
        smartTargetPointsMin = Mathf.Clamp(smartTargetPointsMin, 8, 800);
        smartTargetPointsMax = Mathf.Clamp(smartTargetPointsMax, 8, 1200);
        smartSmoothing = Mathf.Clamp(smartSmoothing, 0f, 0.5f);
        smartSmoothingPasses = Mathf.Clamp(smartSmoothingPasses, 0, 4);
        roadProjectionRayHeight = Mathf.Clamp(roadProjectionRayHeight, 10f, 3000f);
        roadProjectionRadius = Mathf.Clamp(roadProjectionRadius, 0f, 3f);
        roadProjectionSurfaceOffset = Mathf.Clamp(roadProjectionSurfaceOffset, 0f, 1f);
        roadProjectionMaxDeltaY = Mathf.Clamp(roadProjectionMaxDeltaY, 0.1f, 300f);
        editLane = Mathf.Clamp(editLane, 1, 9);
        snapRayHeight = Mathf.Max(10f, snapRayHeight);
        pointMinSpacing = Mathf.Max(0.05f, pointMinSpacing);
        removePointRadius = Mathf.Max(0.1f, removePointRadius);
        if (referenceLaneRoot == null)
        {
            // Keep this safe in editor: no auto-create, only avoid accidental stale destroyed refs.
        }
    }
}
