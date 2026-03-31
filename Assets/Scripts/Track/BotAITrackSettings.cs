using System;
using System.IO;
using UnityEngine;

public class BotAITrackSettings : MonoBehaviour
{
    [Serializable]
    private class RuntimeSafeLaneConfig
    {
        public bool useOverrides = true;
        public bool enableSafeLaneMode = true;
        public float safeLaneMaxSpeed = 11f;
        public float safeLaneSteerMultiplier = 1.8f;
        public bool safeLaneDisableDrift = false;
        public bool safeLaneDisableSteerWobble = true;
        public float safeLaneAvoidMultiplier = 1.2f;
        public float safeLaneThrottleStart01 = 0.82f;
        public float safeLaneThrottleStop01 = 0.97f;
        public bool autoComputeSafeLaneFromTrack = true;
        public float autoComputeInfluence = 0.85f;
    }

    [Header("General")]
    public bool useOverrides = true;
    [Tooltip("If enabled, bot lanes are rebuilt often during Play Mode so moving points is reflected quickly.")]
    public bool liveRebuildLanesInPlay = false;

    [Header("Manual Lanes")]
    [Tooltip("Optional root transform containing Lane_01..Lane_05 and their point children. If null, system searches for a child named BotLanes.")]
    public Transform botLanesRoot;

    [Header("Lane Routing")]
    public bool enableBotLaneRouting = true;
    [Range(0.5f, 3f)] public float botLaneHalfWidth = 1.2f;
    [Range(2, 12)] public int botLaneSubdivisionsPerSegment = 7;
    [Range(0f, 3f)] public float botPathOffsetRadius = 0.95f;
    [Range(0, 4)] public int botLookAheadCheckpoints = 1;
    public float botPathLookAheadMin = 9f;
    public float botPathLookAheadMax = 18f;

    [Header("Safe Lane Mode")]
    [Tooltip("Modo conservador para que bots sigan carriles con menos riesgo de loops en curvas.")]
    public bool enableSafeLaneMode = true;
    [Tooltip("Velocidad maxima objetivo del bot en modo seguro.")]
    [Range(6f, 30f)] public float safeLaneMaxSpeed = 11f;
    [Tooltip("Factor de giro en modo seguro.")]
    [Range(0.8f, 3f)] public float safeLaneSteerMultiplier = 1.8f;
    [Tooltip("Si esta activo, desactiva drift automatico en modo seguro.")]
    public bool safeLaneDisableDrift = false;
    [Tooltip("Si esta activo, desactiva wobble de giro en modo seguro.")]
    public bool safeLaneDisableSteerWobble = true;
    [Tooltip("Multiplicador de evasion de pared en modo seguro.")]
    [Range(0.5f, 2f)] public float safeLaneAvoidMultiplier = 1.2f;
    [Tooltip("Fraccion para reactivar aceleracion por debajo de la velocidad maxima.")]
    [Range(0.5f, 0.98f)] public float safeLaneThrottleStart01 = 0.82f;
    [Tooltip("Fraccion para cortar aceleracion cerca del tope de velocidad.")]
    [Range(0.6f, 1f)] public float safeLaneThrottleStop01 = 0.97f;

    [Header("Auto Tune (IA por pista)")]
    [Tooltip("Calcula tuning automaticamente segun geometria de lanes/checkpoints de la pista.")]
    public bool autoComputeSafeLaneFromTrack = true;
    [Tooltip("0 = usa solo valores manuales. 1 = usa solo calculo automatico por pista.")]
    [Range(0f, 1f)] public float autoComputeInfluence = 0.85f;

    [Header("Runtime Server Config (JSON)")]
    [Tooltip("Lee ajustes en caliente desde archivo JSON cuando corre en servidor batch (.exe dedicado).")]
    public bool enableServerRuntimeOverride = false;
    [Tooltip("Permite tambien en Editor Play para test rapido, aunque por defecto es solo batch server.")]
    public bool runtimeAllowInEditor = false;
    [Tooltip("Si esta activo, usa persistentDataPath. Si no, usa carpeta Data del juego.")]
    public bool runtimeUsePersistentDataPath = true;
    [Tooltip("Nombre del JSON de override.")]
    public string runtimeConfigFileName = "bot_ai_runtime.json";
    [Tooltip("Ruta absoluta opcional. Si se llena, tiene prioridad sobre el nombre.")]
    public string runtimeConfigAbsolutePath = "";
    [Range(0.2f, 10f)] public float runtimeReloadSeconds = 0.25f;

    [Header("Lane Test (Debug)")]
    [Tooltip("Permite forzar un carril especifico para testear bots por lane.")]
    public bool enableLaneTestMode = false;
    [Tooltip("Carril a forzar (1..9). Si hay menos carriles, se ajusta al maximo disponible.")]
    [Range(1, 9)] public int forcedLaneNumber = 1;
    [Tooltip("Si esta activo, BotLaneAuthoringTool dibuja solo el carril forzado.")]
    public bool showOnlyForcedLaneGizmo = false;
    [Header("Generated Lanes")]
    [Tooltip("Used only when manual BotLanes are not found.")]
    [Range(3, 9)] public int generatedLaneCount = 5;
    [Tooltip("How much generated lanes shrink in sharp turns. Lower = tighter inside line, higher = wider lanes.")]
    [Range(0.1f, 1f)] public float generatedLaneWidthScaleOnCurve = 0.45f;

    [Header("Curve Control")]
    [Range(0f, 1f)] public float botCurveBrakeStart = 0.42f;
    [Range(0f, 1f)] public float botCurveBrakeSpeed01 = 0.62f;
    [Range(0f, 1f)] public float botCurveHardBrakeSpeed01 = 0.76f;
    [Range(1f, 2.5f)] public float botCurveSteerBoost = 1.95f;

    [Header("Auto Drift")]
    public bool enableBotAutoDrift = true;
    [Range(0f, 1f)] public float botDriftCurveStart = 0.44f;
    [Range(0f, 1f)] public float botDriftMinSpeed01 = 0.3f;
    [Range(0f, 1f)] public float botDriftSteerThreshold = 0.26f;
    public int botDriftStartGuardTicks = 10;

    [Header("Steering Noise")]
    public bool enableBotSteerWobble = false;
    [Range(0f, 0.4f)] public float botSteerWobble = 0.02f;
    [Range(0.5f, 4f)] public float botSteerMultiplier = 2.1f;

    [Header("Obstacle Avoidance")]
    public float botFrontRayDistance = 8f;
    public float botSideRayDistance = 6f;
    [Range(0.5f, 2.5f)] public float botAvoidSteerStrength = 1.35f;

    [Header("Recovery")]
    public int botStuckTicksThreshold = 45;
    public int botReverseTicks = 16;
    [Tooltip("Ticks de espera antes de permitir una nueva reversa, para evitar loops adelante/atras.")]
    public int botReverseReentryDelayTicks = 90;
    [Tooltip("Minimo de ticks casi quieto con bloqueo frontal para disparar reversa corta.")]
    public int botReverseFromBlockMinStillTicks = 14;
    public float botReverseDotThreshold = -0.12f;
    public float botMinMoveSqrPerTick = 0.0008f;

    private string _runtimeResolvedPath;
    private float _runtimeNextReloadAt;
    private string _runtimeLastJson;
    private bool _runtimeInitDone;
    [NonSerialized] public int runtimeConfigRevision = 0;

    private void OnValidate()
    {
        // Guard rails for accidental extreme values from inspector edits.
        if (botPathLookAheadMin > 60f || botPathLookAheadMax > 90f || botPathLookAheadMax < botPathLookAheadMin)
        {
            botPathLookAheadMin = 9f;
            botPathLookAheadMax = 18f;
        }

        botPathLookAheadMin = Mathf.Clamp(botPathLookAheadMin, 4f, 30f);
        botPathLookAheadMax = Mathf.Clamp(botPathLookAheadMax, botPathLookAheadMin + 2f, 45f);
        safeLaneMaxSpeed = Mathf.Clamp(safeLaneMaxSpeed, 6f, 30f);
        safeLaneSteerMultiplier = Mathf.Clamp(safeLaneSteerMultiplier, 0.8f, 3f);
        safeLaneAvoidMultiplier = Mathf.Clamp(safeLaneAvoidMultiplier, 0.5f, 2f);
        safeLaneThrottleStart01 = Mathf.Clamp(safeLaneThrottleStart01, 0.5f, 0.98f);
        safeLaneThrottleStop01 = Mathf.Clamp(safeLaneThrottleStop01, safeLaneThrottleStart01 + 0.05f, 1f);
        autoComputeInfluence = Mathf.Clamp01(autoComputeInfluence);
        botLaneHalfWidth = Mathf.Clamp(botLaneHalfWidth, 0.5f, 3f);
        botLaneSubdivisionsPerSegment = Mathf.Clamp(botLaneSubdivisionsPerSegment, 2, 12);
        botPathOffsetRadius = Mathf.Clamp(botPathOffsetRadius, 0f, 3f);
        botLookAheadCheckpoints = Mathf.Clamp(botLookAheadCheckpoints, 0, 4);

        botCurveBrakeStart = Mathf.Clamp01(botCurveBrakeStart);
        botCurveBrakeSpeed01 = Mathf.Clamp01(botCurveBrakeSpeed01);
        botCurveHardBrakeSpeed01 = Mathf.Clamp(botCurveHardBrakeSpeed01, botCurveBrakeSpeed01, 1f);
        botCurveSteerBoost = Mathf.Clamp(botCurveSteerBoost, 1f, 2.5f);

        botDriftCurveStart = Mathf.Clamp01(botDriftCurveStart);
        botDriftMinSpeed01 = Mathf.Clamp01(botDriftMinSpeed01);
        botDriftSteerThreshold = Mathf.Clamp01(botDriftSteerThreshold);
        botDriftStartGuardTicks = Mathf.Max(1, botDriftStartGuardTicks);
        botSteerWobble = Mathf.Clamp(botSteerWobble, 0f, 0.4f);
        botSteerMultiplier = Mathf.Clamp(botSteerMultiplier, 0.5f, 4f);

        botFrontRayDistance = Mathf.Clamp(botFrontRayDistance, 1f, 18f);
        botSideRayDistance = Mathf.Clamp(botSideRayDistance, 1f, 14f);
        botAvoidSteerStrength = Mathf.Clamp(botAvoidSteerStrength, 0.5f, 2.5f);

        botStuckTicksThreshold = Mathf.Max(10, botStuckTicksThreshold);
        botReverseTicks = Mathf.Max(4, botReverseTicks);
        botReverseReentryDelayTicks = Mathf.Max(10, botReverseReentryDelayTicks);
        botReverseFromBlockMinStillTicks = Mathf.Max(4, botReverseFromBlockMinStillTicks);
        botReverseDotThreshold = Mathf.Clamp(botReverseDotThreshold, -1f, 0.2f);
        botMinMoveSqrPerTick = Mathf.Max(0.00001f, botMinMoveSqrPerTick);
        forcedLaneNumber = Mathf.Clamp(forcedLaneNumber, 1, 9);
        runtimeReloadSeconds = Mathf.Clamp(runtimeReloadSeconds, 0.2f, 10f);
    }

    private void OnEnable()
    {
        EnsureRuntimePathResolved();
    }

    private void Update()
    {
        if (!ShouldUseRuntimeOverride())
            return;

        EnsureRuntimePathResolved();

        if (Time.unscaledTime < _runtimeNextReloadAt)
            return;

        _runtimeNextReloadAt = Time.unscaledTime + runtimeReloadSeconds;
        ReloadRuntimeOverrideFromJson();
    }

    private bool ShouldUseRuntimeOverride()
    {
        if (!enableServerRuntimeOverride || !Application.isPlaying)
            return false;

        if (Application.isBatchMode)
            return true;

        return runtimeAllowInEditor;
    }

    private void EnsureRuntimePathResolved()
    {
        if (_runtimeInitDone && !string.IsNullOrEmpty(_runtimeResolvedPath))
            return;

        _runtimeResolvedPath = ResolveRuntimePath();
        _runtimeInitDone = true;
    }

    private string ResolveRuntimePath()
    {
        if (!string.IsNullOrWhiteSpace(runtimeConfigAbsolutePath))
            return runtimeConfigAbsolutePath.Trim();

        string fileName = string.IsNullOrWhiteSpace(runtimeConfigFileName) ? "bot_ai_runtime.json" : runtimeConfigFileName.Trim();
        string basePath = runtimeUsePersistentDataPath
            ? Application.persistentDataPath
            : Application.dataPath;
        return Path.Combine(basePath, fileName);
    }

    private void ReloadRuntimeOverrideFromJson()
    {
        if (string.IsNullOrWhiteSpace(_runtimeResolvedPath))
            return;

        try
        {
            if (!File.Exists(_runtimeResolvedPath))
            {
                WriteRuntimeTemplateIfMissing();
                return;
            }

            string json = File.ReadAllText(_runtimeResolvedPath);
            if (string.IsNullOrWhiteSpace(json) || string.Equals(json, _runtimeLastJson, StringComparison.Ordinal))
                return;

            RuntimeSafeLaneConfig config = JsonUtility.FromJson<RuntimeSafeLaneConfig>(json);
            if (config == null)
            {
                Debug.LogWarning($"BotAITrackSettings: JSON invalido en '{_runtimeResolvedPath}'.");
                return;
            }

            ApplyRuntimeConfig(config);
            _runtimeLastJson = json;
            Debug.Log($"BotAITrackSettings: runtime JSON aplicado ({_runtimeResolvedPath}).");
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"BotAITrackSettings: no se pudo leer runtime JSON '{_runtimeResolvedPath}'. {ex.Message}");
        }
    }

    [ContextMenu("BotAI/Export Runtime JSON Template")]
    private void ExportRuntimeJsonTemplate()
    {
        EnsureRuntimePathResolved();
        WriteRuntimeTemplate(forceOverwrite: true);
        Debug.Log($"BotAITrackSettings: template exportado en '{_runtimeResolvedPath}'.");
    }

    private void WriteRuntimeTemplateIfMissing()
    {
        if (File.Exists(_runtimeResolvedPath))
            return;
        WriteRuntimeTemplate(forceOverwrite: false);
    }

    private void WriteRuntimeTemplate(bool forceOverwrite)
    {
        if (string.IsNullOrWhiteSpace(_runtimeResolvedPath))
            return;

        try
        {
            string dir = Path.GetDirectoryName(_runtimeResolvedPath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!forceOverwrite && File.Exists(_runtimeResolvedPath))
                return;

            var current = SnapshotRuntimeConfig();
            string json = JsonUtility.ToJson(current, true);
            File.WriteAllText(_runtimeResolvedPath, json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"BotAITrackSettings: no se pudo escribir template JSON '{_runtimeResolvedPath}'. {ex.Message}");
        }
    }

    private RuntimeSafeLaneConfig SnapshotRuntimeConfig()
    {
        return new RuntimeSafeLaneConfig
        {
            useOverrides = useOverrides,
            enableSafeLaneMode = enableSafeLaneMode,
            safeLaneMaxSpeed = safeLaneMaxSpeed,
            safeLaneSteerMultiplier = safeLaneSteerMultiplier,
            safeLaneDisableDrift = safeLaneDisableDrift,
            safeLaneDisableSteerWobble = safeLaneDisableSteerWobble,
            safeLaneAvoidMultiplier = safeLaneAvoidMultiplier,
            safeLaneThrottleStart01 = safeLaneThrottleStart01,
            safeLaneThrottleStop01 = safeLaneThrottleStop01,
            autoComputeSafeLaneFromTrack = autoComputeSafeLaneFromTrack,
            autoComputeInfluence = autoComputeInfluence
        };
    }

    private void ApplyRuntimeConfig(RuntimeSafeLaneConfig config)
    {
        useOverrides = config.useOverrides;
        enableSafeLaneMode = config.enableSafeLaneMode;
        safeLaneMaxSpeed = config.safeLaneMaxSpeed;
        safeLaneSteerMultiplier = config.safeLaneSteerMultiplier;
        safeLaneDisableDrift = config.safeLaneDisableDrift;
        safeLaneDisableSteerWobble = config.safeLaneDisableSteerWobble;
        safeLaneAvoidMultiplier = config.safeLaneAvoidMultiplier;
        safeLaneThrottleStart01 = config.safeLaneThrottleStart01;
        safeLaneThrottleStop01 = config.safeLaneThrottleStop01;
        autoComputeSafeLaneFromTrack = config.autoComputeSafeLaneFromTrack;
        autoComputeInfluence = config.autoComputeInfluence;
        OnValidate();
        runtimeConfigRevision = runtimeConfigRevision == int.MaxValue ? 0 : runtimeConfigRevision + 1;
    }
}
