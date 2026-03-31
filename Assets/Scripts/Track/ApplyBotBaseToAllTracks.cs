#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ApplyBotBaseToAllTracks
{
    private const string ScenesFolder = "Assets/Track/Scenes";

    [MenuItem("Tools/Bots/Apply Base To All Track Scenes")]
    public static void Apply()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { ScenesFolder });
        if (sceneGuids == null || sceneGuids.Length == 0)
        {
            Debug.LogWarning("No se encontraron escenas en Assets/Track/Scenes.");
            return;
        }

        string backupRoot = Path.Combine("codex_backups", DateTime.Now.ToString("yyyy-MM-dd") + "_apply_bot_base");
        Directory.CreateDirectory(backupRoot);

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrWhiteSpace(scenePath))
                continue;

            BackupScene(scenePath, backupRoot);

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            bool changed = false;

            foreach (BotAITrackSettings settings in UnityEngine.Object.FindObjectsOfType<BotAITrackSettings>(true))
                changed |= ApplyBotSettings(settings);

            foreach (BotDriftPathTool driftTool in UnityEngine.Object.FindObjectsOfType<BotDriftPathTool>(true))
                changed |= ApplyDriftSettings(driftTool);

            if (changed)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log("Bot base aplicada: " + scenePath);
            }
            else
            {
                Debug.Log("Sin cambios: " + scenePath);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Aplicacion de base bots terminada.");
    }

    private static void BackupScene(string scenePath, string backupRoot)
    {
        string fileName = Path.GetFileName(scenePath) + ".bak";
        string destination = Path.Combine(backupRoot, fileName);
        File.Copy(scenePath, destination, true);
    }

    private static bool ApplyBotSettings(BotAITrackSettings settings)
    {
        bool changed = false;

        changed |= Set(ref settings.enableLaneTestMode, false);
        changed |= Set(ref settings.liveRebuildLanesInPlay, false);
        changed |= Set(ref settings.autoComputeSafeLaneFromTrack, false);
        changed |= Set(ref settings.enableBotSteerWobble, false);
        changed |= Set(ref settings.enableBotAutoDrift, false);
        changed |= Set(ref settings.useOverrides, true);
        changed |= Set(ref settings.botPathLookAheadMin, 7f);
        changed |= Set(ref settings.botPathLookAheadMax, 13f);
        changed |= Set(ref settings.botFrontRayDistance, 8f);
        changed |= Set(ref settings.botSideRayDistance, 6f);
        changed |= Set(ref settings.botAvoidSteerStrength, 1.2f);
        changed |= Set(ref settings.botStuckTicksThreshold, 34);
        changed |= Set(ref settings.botReverseTicks, 24);
        changed |= Set(ref settings.botReverseFromBlockMinStillTicks, 10);
        changed |= Set(ref settings.botReverseDotThreshold, -0.2f);
        changed |= Set(ref settings.botMinMoveSqrPerTick, 0.0035f);

        if (changed)
            EditorUtility.SetDirty(settings);

        return changed;
    }

    private static bool ApplyDriftSettings(BotDriftPathTool driftTool)
    {
        bool changed = false;

        changed |= Set(ref driftTool.activationRadius, 3.2f);
        changed |= Set(ref driftTool.holdDriftTicksAfterExit, 6);
        changed |= Set(ref driftTool.useNearestLaneBoundPath, true);

        if (changed)
            EditorUtility.SetDirty(driftTool);

        return changed;
    }

    private static bool Set(ref bool field, bool value)
    {
        if (field == value)
            return false;
        field = value;
        return true;
    }

    private static bool Set(ref int field, int value)
    {
        if (field == value)
            return false;
        field = value;
        return true;
    }

    private static bool Set(ref float field, float value)
    {
        if (Mathf.Approximately(field, value))
            return false;
        field = value;
        return true;
    }
}
#endif
