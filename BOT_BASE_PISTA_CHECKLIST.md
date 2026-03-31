# Base Bots Por Pista

Usa esta lista como base comun para nuevas pistas.

## Copiar Como Base

- `enableLaneTestMode = 0`
- `liveRebuildLanesInPlay = 0`
- `autoComputeSafeLaneFromTrack = 0`
- `enableBotSteerWobble = 0`
- `enableBotAutoDrift = 0`
- `useNearestLaneBoundPath = 1`
- `botPathLookAheadMin = 7`
- `botPathLookAheadMax = 13`
- `botFrontRayDistance = 8`
- `botSideRayDistance = 6`
- `botAvoidSteerStrength = 1.2`
- `botStuckTicksThreshold = 34`
- `botReverseTicks = 24`
- `botReverseFromBlockMinStillTicks = 10`
- `botReverseDotThreshold = -0.2`
- `botMinMoveSqrPerTick = 0.0035`

## Ajustes De Drift A Revisar

- `activationRadius = 3.2`
- `holdDriftTicksAfterExit = 6`

Estos dos se pueden usar como base, pero revisalos segun la curva de cada pista.

## No Copiar A Ciegas

- `BotLanes`
- `BotDriftPaths`
- checkpoints
- cualquier linea manual pegada a muros

Eso depende del trazado real de cada mapa.

## Orden Recomendado

1. Copiar los parametros base.
2. Revisar que `BotLanes` no vayan pegados a pared.
3. Revisar que `BotDriftPaths` entren antes de la curva.
4. Probar la pista.
5. Solo si falla, ajustar drift path o lane, no veinte numeros a la vez.

## Idea General

- recta: seguir linea
- curva cerrada: usar drift
- choque inmediato: corregir poco
- atasco: reversa y volver a linea
