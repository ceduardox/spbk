using System.Collections.Generic;
using UnityEngine;

public class BotLaneProbeRunner : MonoBehaviour
{
    [Header("Referencias")]
    [InspectorName("Pista (Track)")]
    public Track track;
    [InspectorName("Raiz BotLanes")]
    public Transform botLanesRoot;

    [Header("Ejecucion")]
    [InspectorName("Solo Editor")]
    [Tooltip("Si esta activo, nunca corre probes en build/juego final.")]
    public bool editorOnly = true;
    [InspectorName("Auto Iniciar en Play")]
    public bool autoStartOnPlay = true;
    [InspectorName("Activo")]
    public bool running = true;
    [InspectorName("Probar Todos los Carriles")]
    public bool testAllLanes = true;
    [InspectorName("Carril a Probar")]
    [Range(1, 9)] public int laneNumber = 1;
    [InspectorName("Circuito Cerrado")]
    public bool closedLoop = true;

    [Header("Visual del Probe")]
    [InspectorName("Usar Visual Kart")]
    [Tooltip("Si esta activo, usa un kart visual en lugar de esferas.")]
    public bool useKartVisual = true;
    [InspectorName("Solo 1 Kart (Carril seleccionado)")]
    [Tooltip("Prueba un solo carril (laneNumber) con un kart, ideal para tuning rapido.")]
    public bool singleKartOnly = true;
    [InspectorName("Prefab Kart (opcional)")]
    [Tooltip("Si se deja vacio, intentara usar el primer kart prefab del ResourceManager.")]
    public GameObject kartProbePrefab;
    [InspectorName("Escala Visual Kart")]
    public float kartVisualScale = 1f;
    [InspectorName("Desactivar Scripts del Kart Visual")]
    [Tooltip("Desactiva scripts del prefab visual para evitar conflictos con red/gameplay.")]
    public bool disableScriptsOnKartVisual = true;

    [Header("Movimiento Fantasma")]
    [InspectorName("Modelo Cinematico")]
    [Tooltip("Si esta activo, gira/acelera como un bot simplificado. Si no, recorre la lane ideal.")]
    public bool useKinematicModel = true;
    [InspectorName("Velocidad Objetivo")]
    public float targetSpeed = 18f;
    [InspectorName("Aceleracion")]
    public float acceleration = 22f;
    [InspectorName("Frenado")]
    public float braking = 28f;
    [InspectorName("Look Ahead")]
    public float lookAheadDistance = 8f;
    [InspectorName("Giro Max (grados/s)")]
    public float maxTurnDegreesPerSecond = 240f;
    [InspectorName("Distancia Cambio Nodo")]
    public float nodeReachDistance = 1.1f;
    [InspectorName("Altura Sobre Pista")]
    public float probeHeight = 0.45f;

    [Header("Fisica Probe")]
    [InspectorName("Usar Rigidbody")]
    [Tooltip("Si esta activo, el probe usa fisica real con masa/colision (mas cercano a bot real).")]
    public bool useRigidbodyPhysics = true;
    [InspectorName("Masa Probe")]
    public float probeMass = 320f;
    [InspectorName("Drag Lineal")]
    public float probeLinearDrag = 0.2f;
    [InspectorName("Drag Angular")]
    public float probeAngularDrag = 1.2f;
    [InspectorName("Usar Gravedad")]
    public bool probeUseGravity = false;
    [InspectorName("Seguir Altura Carril")]
    [Tooltip("Ayuda en subidas/bajadas usando la altura de la lane.")]
    public bool followLaneHeight = true;
    [InspectorName("Fuerza Vertical")]
    public float verticalFollowStrength = 10f;
    [InspectorName("Velocidad Vertical Max")]
    public float maxVerticalSpeed = 10f;
    [InspectorName("Congelar Rot X/Z")]
    public bool freezeProbeTilt = true;
    [InspectorName("Lerp Velocidad")]
    [Range(0.1f, 30f)] public float velocityLerp = 12f;

    [Header("Colision con Paredes")]
    [InspectorName("Habilitar Colision")]
    public bool collideWithWalls = true;
    [InspectorName("Radio de Colision")]
    [Range(0.05f, 1.5f)] public float probeCollisionRadius = 0.28f;
    [InspectorName("Piel de Colision")]
    [Range(0.001f, 0.2f)] public float collisionSkin = 0.03f;
    [InspectorName("Mascara de Colision")]
    public LayerMask wallCollisionMask = ~0;
    [InspectorName("Normal Min Y para Piso")]
    [Tooltip("Impactos con normal Y mayor a este valor se consideran piso y no pared.")]
    [Range(0f, 1f)] public float floorNormalYThreshold = 0.55f;

    [Header("Deteccion de Zona Problematica")]
    [InspectorName("Marcar Atascos")]
    public bool markStuckZones = true;
    [InspectorName("Velocidad Min Atasco")]
    public float stuckMinSpeed = 0.25f;
    [InspectorName("Segundos para Atasco")]
    public float stuckSeconds = 1.4f;
    [InspectorName("Separacion de Marcadores")]
    public float markerMinDistance = 2.5f;
    [InspectorName("Escala Marcador")]
    public float markerScale = 0.35f;

    [Header("Visual")]
    [InspectorName("Escala Probe")]
    public float probeScale = 0.6f;

    private readonly List<ProbeState> _states = new List<ProbeState>();
    private Transform _probesRoot;
    private Transform _markersRoot;
    private bool _autoBuildTried;

    private class ProbeState
    {
        public string laneName;
        public List<Vector3> lane;
        public Transform probe;
        public Rigidbody rb;
        public bool usesRigid;
        public int nextIndex;
        public float speed;
        public float stuckTimer;
        public Vector3 lastPos;
        public Vector3 lastMarkerPos;
        public bool hasMarker;
        public bool finished;
    }

    private void OnEnable()
    {
        if (ShouldForceDisableForBuild())
        {
            DisableForBuild();
            return;
        }
        if (Application.isPlaying && autoStartOnPlay)
            TryAutoBuild("OnEnable");
    }

    private void Start()
    {
        if (ShouldForceDisableForBuild())
            return;
        if (Application.isPlaying && autoStartOnPlay && _states.Count == 0)
            TryAutoBuild("Start");
    }

    private void Update()
    {
        if (ShouldForceDisableForBuild())
            return;
        if (!Application.isPlaying)
            return;

        if (running && _states.Count == 0 && !_autoBuildTried)
            TryAutoBuild("UpdateRetry");

        if (!running || _states.Count == 0)
            return;

        float dt = Time.deltaTime;
        if (dt <= 0f)
            return;

        for (int i = 0; i < _states.Count; i++)
            StepProbe(_states[i], dt);
    }

    private bool ShouldForceDisableForBuild()
    {
#if UNITY_EDITOR
        return false;
#else
        return editorOnly;
#endif
    }

    private void DisableForBuild()
    {
        running = false;
        _states.Clear();
        _autoBuildTried = true;

        Transform runners = transform.Find("LaneProbe_Runners");
        if (runners != null)
            Destroy(runners.gameObject);

        Transform markers = transform.Find("LaneProbe_Markers");
        if (markers != null)
            Destroy(markers.gameObject);

        enabled = false;
    }

    [ContextMenu("LaneProbe/Iniciar o Rebuild")]
    public void BuildAndStart()
    {
#if !UNITY_EDITOR
        return;
#endif
        ResolveReferences();
        ClearProbes();
        EnsureRoots();

        var lanes = CollectLanes();
        if (lanes.Count == 0)
        {
            Debug.LogWarning("BotLaneProbeRunner: no hay lanes validas para test.");
            return;
        }

        for (int i = 0; i < lanes.Count; i++)
        {
            if (lanes[i].Count < 2)
                continue;

            var first = lanes[i][0] + Vector3.up * probeHeight;
            Vector3 forward = (lanes[i][1] - lanes[i][0]).normalized;
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;
            GameObject probeGO = CreateProbeObject(i, first, forward, out Rigidbody rb);
            if (probeGO == null)
                continue;

            _states.Add(new ProbeState
            {
                laneName = "Lane_" + (i + 1).ToString("00"),
                lane = lanes[i],
                probe = probeGO.transform,
                rb = rb,
                usesRigid = rb != null,
                nextIndex = 1,
                speed = 0f,
                stuckTimer = 0f,
                lastPos = first,
                finished = false
            });
        }

        if (_states.Count == 0)
        {
            Debug.LogWarning("BotLaneProbeRunner: se construyo 0 probes. Revisa BotLanes/LaneNumber.");
            running = false;
            return;
        }

        Debug.Log("BotLaneProbeRunner: probes activos = " + _states.Count);
        running = true;
    }

    private void TryAutoBuild(string source)
    {
        if (_autoBuildTried)
            return;
        _autoBuildTried = true;
        Debug.Log("BotLaneProbeRunner: auto build desde " + source);
        BuildAndStart();
    }

    [ContextMenu("LaneProbe/Limpiar Probes")]
    public void ClearProbes()
    {
        _states.Clear();
        _autoBuildTried = false;
        if (_probesRoot != null)
        {
            if (Application.isPlaying)
                Destroy(_probesRoot.gameObject);
            else
                DestroyImmediate(_probesRoot.gameObject);
            _probesRoot = null;
        }
    }

    [ContextMenu("LaneProbe/Limpiar Marcadores")]
    public void ClearMarkers()
    {
        if (_markersRoot == null)
            return;
        if (Application.isPlaying)
            Destroy(_markersRoot.gameObject);
        else
            DestroyImmediate(_markersRoot.gameObject);
        _markersRoot = null;
    }

    private void StepProbe(ProbeState s, float dt)
    {
        if (s == null || s.finished || s.lane == null || s.lane.Count < 2 || s.probe == null)
            return;

        int count = s.lane.Count;
        int next = Mathf.Clamp(s.nextIndex, 0, count - 1);
        Vector3 pos = s.probe.position;
        Vector3 targetNode = s.lane[next] + Vector3.up * probeHeight;

        if (Vector3.Distance(pos, targetNode) <= Mathf.Max(0.05f, nodeReachDistance))
        {
            next++;
            if (next >= count)
            {
                if (closedLoop)
                    next = 0;
                else
                {
                    next = count - 1;
                    s.finished = true;
                }
            }
            s.nextIndex = next;
            targetNode = s.lane[next] + Vector3.up * probeHeight;
        }

        Vector3 desiredPoint = useKinematicModel
            ? GetLookAheadPoint(s.lane, pos, next, Mathf.Max(0.5f, lookAheadDistance))
            : targetNode;

        Vector3 toDesired = desiredPoint - pos;
        toDesired.y = 0f;
        if (toDesired.sqrMagnitude < 0.0001f)
            return;

        Vector3 desiredDir = toDesired.normalized;

        if (useKinematicModel)
        {
            Vector3 currentForward = s.probe.forward;
            currentForward.y = 0f;
            if (currentForward.sqrMagnitude < 0.0001f)
                currentForward = desiredDir;

            float maxRad = Mathf.Deg2Rad * Mathf.Max(10f, maxTurnDegreesPerSecond) * dt;
            Vector3 newForward = Vector3.RotateTowards(currentForward.normalized, desiredDir, maxRad, 0f);
            if (newForward.sqrMagnitude < 0.0001f)
                newForward = desiredDir;
            s.probe.forward = newForward;

            float turnAngle = Vector3.Angle(newForward, desiredDir);
            float turn01 = Mathf.Clamp01(turnAngle / 90f);
            float speedCap = Mathf.Lerp(targetSpeed, targetSpeed * 0.35f, turn01);
            float accel = s.speed < speedCap ? acceleration : braking;
            s.speed = Mathf.MoveTowards(s.speed, speedCap, Mathf.Max(0.1f, accel) * dt);
        }
        else
        {
            s.probe.forward = desiredDir;
            s.speed = Mathf.Max(0.1f, targetSpeed);
        }

        bool hitWall = false;
        Vector3 nextPos;
        if (s.usesRigid && s.rb != null)
        {
            Quaternion targetRot = Quaternion.LookRotation(s.probe.forward.sqrMagnitude > 0.0001f ? s.probe.forward : Vector3.forward, Vector3.up);
            s.rb.MoveRotation(Quaternion.RotateTowards(s.rb.rotation, targetRot, Mathf.Max(10f, maxTurnDegreesPerSecond) * dt));

            Vector3 from = s.rb.position;
            Vector3 desiredMove = s.probe.forward * Mathf.Max(0.1f, s.speed) * dt;
            nextPos = from + desiredMove;

            if (probeUseGravity)
            {
                nextPos.y = from.y;
            }
            else if (followLaneHeight)
            {
                float y = Mathf.Lerp(from.y, desiredPoint.y, Mathf.Clamp01(Mathf.Max(0.1f, verticalFollowStrength) * dt));
                nextPos.y = y;
            }
            else
            {
                nextPos.y = from.y;
            }

            if (collideWithWalls)
                nextPos = ResolveWallCollision(from, nextPos, out hitWall);

            s.rb.MovePosition(nextPos);
            if (dt > 0.0001f)
                s.rb.velocity = (nextPos - from) / dt;
        }
        else
        {
            Vector3 move = s.probe.forward * s.speed * dt;
            nextPos = pos + move;

            float y = Mathf.Lerp(pos.y, desiredPoint.y, 0.25f);
            nextPos.y = y;

            if (collideWithWalls)
                nextPos = ResolveWallCollision(pos, nextPos, out hitWall);

            s.probe.position = nextPos;
        }

        if (hitWall)
        {
            s.speed = Mathf.Min(s.speed, targetSpeed * 0.2f);
            s.stuckTimer += dt;
        }

        float realSpeed;
        if (s.usesRigid && s.rb != null)
        {
            Vector3 planarVel = s.rb.velocity;
            planarVel.y = 0f;
            realSpeed = planarVel.magnitude;
        }
        else
        {
            realSpeed = Vector3.Distance(nextPos, s.lastPos) / Mathf.Max(0.0001f, dt);
        }
        if (realSpeed < Mathf.Max(0.01f, stuckMinSpeed))
            s.stuckTimer += dt;
        else
            s.stuckTimer = 0f;

        if (markStuckZones && s.stuckTimer >= Mathf.Max(0.2f, stuckSeconds))
        {
            TryMarkStuck(s, nextPos);
            s.stuckTimer = 0f;
        }

        s.lastPos = nextPos;
    }

    private Vector3 ResolveWallCollision(Vector3 from, Vector3 to, out bool hitWall)
    {
        hitWall = false;

        Vector3 horizontal = to - from;
        horizontal.y = 0f;
        float dist = horizontal.magnitude;
        if (dist <= 0.0001f)
            return to;

        Vector3 dir = horizontal / dist;
        float radius = Mathf.Max(0.03f, probeCollisionRadius * Mathf.Max(0.2f, probeScale));
        float skin = Mathf.Max(0.001f, collisionSkin);
        float castDist = dist + skin;
        Vector3 origin = from + Vector3.up * 0.02f;

        if (Physics.SphereCast(origin, radius, dir, out RaycastHit hit, castDist, wallCollisionMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider != null && !hit.collider.isTrigger && !hit.collider.transform.IsChildOf(transform))
            {
                if (IsLikelyFloor(hit.normal))
                    return to;
                float allowed = Mathf.Max(0f, hit.distance - skin);
                Vector3 blocked = from + dir * allowed;
                blocked.y = Mathf.Lerp(from.y, to.y, 0.15f);
                hitWall = true;
                return blocked;
            }
        }

        return to;
    }

    private Vector3 GetLookAheadPoint(List<Vector3> lane, Vector3 fromPos, int nextIndex, float distance)
    {
        if (lane == null || lane.Count == 0)
            return fromPos;

        int count = lane.Count;
        Vector3 from = fromPos;
        int idx = Mathf.Clamp(nextIndex, 0, count - 1);
        float remaining = Mathf.Max(0f, distance);
        int guard = count + 8;

        while (guard-- > 0)
        {
            Vector3 to = lane[idx] + Vector3.up * probeHeight;
            Vector3 seg = to - from;
            float len = seg.magnitude;
            if (len > 0.0001f)
            {
                if (remaining <= len)
                    return from + seg.normalized * remaining;
                remaining -= len;
            }

            from = to;
            idx++;
            if (idx >= count)
            {
                if (closedLoop)
                    idx = 0;
                else
                    return lane[count - 1] + Vector3.up * probeHeight;
            }
        }

        return from;
    }

    private void TryMarkStuck(ProbeState s, Vector3 pos)
    {
        EnsureRoots();
        if (_markersRoot == null)
            return;

        if (s.hasMarker && Vector3.Distance(s.lastMarkerPos, pos) < Mathf.Max(0.25f, markerMinDistance))
            return;

        var m = GameObject.CreatePrimitive(PrimitiveType.Cube);
        m.name = "Stuck_" + s.laneName;
        m.transform.SetParent(_markersRoot, true);
        m.transform.position = pos;
        m.transform.localScale = Vector3.one * Mathf.Max(0.05f, markerScale);
        var col = m.GetComponent<Collider>();
        if (col != null)
            Destroy(col);

        var renderer = m.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = Color.red;

        s.hasMarker = true;
        s.lastMarkerPos = pos;
        Debug.Log("LaneProbe: zona problematica detectada en " + s.laneName + " pos=" + pos);
    }

    private void ResolveReferences()
    {
        if (track == null)
            track = GetComponent<Track>();
        if (track == null)
            track = GetComponentInParent<Track>();
        if (track == null)
            track = FindObjectOfType<Track>();

        if (botLanesRoot == null && track != null)
            botLanesRoot = FindChildByNameRecursive(track.transform, "BotLanes");
        if (botLanesRoot == null)
        {
            var all = FindObjectsOfType<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != null && all[i].name == "BotLanes")
                {
                    botLanesRoot = all[i];
                    break;
                }
            }
        }
    }

    private void EnsureRoots()
    {
        if (_probesRoot == null)
        {
            var go = new GameObject("LaneProbe_Runners");
            _probesRoot = go.transform;
            _probesRoot.SetParent(transform, false);
        }

        if (_markersRoot == null)
        {
            var go = new GameObject("LaneProbe_Markers");
            _markersRoot = go.transform;
            _markersRoot.SetParent(transform, false);
        }
    }

    private List<List<Vector3>> CollectLanes()
    {
        var result = new List<List<Vector3>>();
        if (botLanesRoot == null)
            return result;

        if (singleKartOnly)
        {
            int wanted = Mathf.Clamp(laneNumber, 1, 99) - 1;
            if (wanted < botLanesRoot.childCount)
            {
                Transform lane = botLanesRoot.GetChild(wanted);
                var points = new List<Vector3>();
                for (int p = 0; p < lane.childCount; p++)
                    points.Add(lane.GetChild(p).position);
                if (points.Count >= 2)
                    result.Add(points);
            }

            if (result.Count == 0)
            {
                for (int i = 0; i < botLanesRoot.childCount; i++)
                {
                    Transform lane = botLanesRoot.GetChild(i);
                    var points = new List<Vector3>();
                    for (int p = 0; p < lane.childCount; p++)
                        points.Add(lane.GetChild(p).position);
                    if (points.Count >= 2)
                    {
                        result.Add(points);
                        Debug.LogWarning("BotLaneProbeRunner: laneNumber invalido o vacio. Usando primer carril valido.");
                        break;
                    }
                }
            }
            return result;
        }

        for (int i = 0; i < botLanesRoot.childCount; i++)
        {
            Transform lane = botLanesRoot.GetChild(i);
            var points = new List<Vector3>();
            for (int p = 0; p < lane.childCount; p++)
                points.Add(lane.GetChild(p).position);

            if (points.Count < 2)
                continue;

            if (testAllLanes)
            {
                result.Add(points);
            }
            else
            {
                int wanted = Mathf.Clamp(laneNumber, 1, 99) - 1;
                if (i == wanted)
                {
                    result.Add(points);
                    break;
                }
            }
        }

        return result;
    }

    private GameObject CreateProbeObject(int laneIndex, Vector3 startPos, Vector3 forward, out Rigidbody rb)
    {
        rb = null;

        var probeGO = new GameObject("Probe_" + (laneIndex + 1).ToString("00"));
        probeGO.transform.SetParent(_probesRoot, false);
        probeGO.transform.position = startPos;
        probeGO.transform.forward = forward;

        bool visualCreated = false;
        if (useKartVisual)
        {
            GameObject prefab = ResolveKartProbePrefab();
            if (prefab != null)
            {
                var visual = Instantiate(prefab, probeGO.transform);
                visual.name = "KartVisual";
                visual.transform.localPosition = Vector3.zero;
                visual.transform.localRotation = Quaternion.identity;
                visual.transform.localScale = Vector3.one * Mathf.Max(0.05f, kartVisualScale);
                if (disableScriptsOnKartVisual)
                    DisableMonoBehavioursRecursive(visual);
                visualCreated = true;
            }
            else
            {
                Debug.LogWarning("BotLaneProbeRunner: no hay Prefab Kart asignado ni disponible en ResourceManager. Se usa esfera.");
            }
        }

        if (!visualCreated)
        {
            var visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "ProbeVisual";
            visual.transform.SetParent(probeGO.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one * probeScale;

            var renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                float hue = Mathf.Repeat((laneIndex * 0.137f), 1f);
                renderer.material.color = Color.HSVToRGB(hue, 0.85f, 1f);
            }
        }

        if (useRigidbodyPhysics)
        {
            EnsureProbeCollider(probeGO);
            rb = probeGO.GetComponent<Rigidbody>();
            if (rb == null)
                rb = probeGO.AddComponent<Rigidbody>();

            rb.mass = Mathf.Max(1f, probeMass);
            rb.drag = Mathf.Max(0f, probeLinearDrag);
            rb.angularDrag = Mathf.Max(0f, probeAngularDrag);
            rb.useGravity = probeUseGravity;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = freezeProbeTilt
                ? RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ
                : RigidbodyConstraints.None;
        }
        else
        {
            var rigid = probeGO.GetComponent<Rigidbody>();
            if (rigid != null)
                Destroy(rigid);

            // En modo no-fisico usamos colision por spherecast manual.
            var allCols = probeGO.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < allCols.Length; i++)
                Destroy(allCols[i]);
        }

        return probeGO;
    }

    private GameObject ResolveKartProbePrefab()
    {
        if (kartProbePrefab != null)
            return kartProbePrefab;
        // Evitar dependencias de carga global durante pruebas de editor.
        // Si no hay prefab asignado manualmente, se usa esfera fallback.
        return null;
    }

    private void EnsureProbeCollider(GameObject probeGO)
    {
        bool hasCollider = false;
        var cols = probeGO.GetComponentsInChildren<Collider>(true);
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i] != null && !cols[i].isTrigger)
            {
                hasCollider = true;
                break;
            }
        }

        if (!hasCollider)
        {
            var c = probeGO.AddComponent<SphereCollider>();
            c.radius = Mathf.Max(0.05f, probeCollisionRadius);
            c.isTrigger = false;
        }
    }

    private static void DisableMonoBehavioursRecursive(GameObject root)
    {
        if (root == null)
            return;

        var monoList = root.GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < monoList.Length; i++)
        {
            var mb = monoList[i];
            if (mb == null)
                continue;
            mb.enabled = false;
        }
    }

    private bool IsLikelyFloor(Vector3 normal)
    {
        return normal.y >= Mathf.Clamp01(floorNormalYThreshold);
    }

    private static Transform FindChildByNameRecursive(Transform root, string name)
    {
        if (root == null || string.IsNullOrEmpty(name))
            return null;
        if (root.name == name)
            return root;
        for (int i = 0; i < root.childCount; i++)
        {
            var found = FindChildByNameRecursive(root.GetChild(i), name);
            if (found != null)
                return found;
        }
        return null;
    }
}
