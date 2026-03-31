# Repo Liviano

Este proyecto pesa varios gigas. Este repo esta pensado para:

- versionar codigo
- versionar escenas y prefabs que si tocamos
- tener rollback limpio de cambios
- colaborar sin subir toda la libreria pesada

## Si subir

- `Assets/Scripts`
- `Assets/Track`
- `Assets/Prefabs`
- `Assets/Resources/Scriptable Objects`
- `Assets/Resources/Prefabs/Powerups`
- `Packages`
- `ProjectSettings`

## No subir

- `Library`
- `Temp`
- `Logs`
- `obj`
- `UserSettings`
- `.vs`
- `.vscode`
- `codex_backups`
- arte pesado que casi no cambia
- karts/drivers pesados si solo se trabaja logica

## Carpetas pesadas que se dejan fuera

- `Assets/Art`
- `Assets/Tony`
- `Assets/PolygonSciFiWorlds`
- `Assets/Firebase`
- `Assets/Sprites`
- `Assets/terrenos`
- `Assets/Resources/Prefabs/Drivers`
- `Assets/Resources/Prefabs/KartPrefabs`

## Como trabajar

1. Mantener una copia local completa del proyecto fuera de git.
2. Usar este repo para scripts, escenas, prefabs y configuracion.
3. Si una tarea necesita tocar un asset pesado, decidir si:
   - se sube solo ese asset puntual
   - o se mantiene fuera del repo
4. Hacer commits pequenos y claros para que el rollback sea real.

## Nota

Este repo liviano puede no abrir funcional al 100% en otra maquina si faltan assets grandes. Su objetivo principal es historial, diffs y rollback de trabajo.
