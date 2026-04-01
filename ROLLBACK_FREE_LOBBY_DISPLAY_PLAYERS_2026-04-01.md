Checkpoint base antes del ajuste UI-only:
- `f2ab6dd` `Add free lobby display player count`

Archivos modificados:
- [`Assets/Scripts/Networking/GameLauncher.cs`](/g:/SUPERBLOK/PROSUPERBLOK4/PROSuperblock_The_Game_V3/Assets/Scripts/Networking/GameLauncher.cs)
- [`Assets/Scripts/UI/JoinRoom/ContentListRooms.cs`](/g:/SUPERBLOK/PROSUPERBLOK4/PROSuperblock_The_Game_V3/Assets/Scripts/UI/JoinRoom/ContentListRooms.cs)

Backups locales:
- [`codex_backups/2026-04-01_ui_only_display_players/GameLauncher.cs.before_ui_only.bak`](/g:/SUPERBLOK/PROSUPERBLOK4/PROSuperblock_The_Game_V3/codex_backups/2026-04-01_ui_only_display_players/GameLauncher.cs.before_ui_only.bak)
- [`codex_backups/2026-04-01_ui_only_display_players/ContentListRooms.cs.before_ui_only.bak`](/g:/SUPERBLOK/PROSUPERBLOK4/PROSuperblock_The_Game_V3/codex_backups/2026-04-01_ui_only_display_players/ContentListRooms.cs.before_ui_only.bak)

Que hace este cambio:
- deja el conteo real intacto en `itemList.players`
- crea `itemList.displayPlayers` solo para mostrar en la UI
- en salas free abiertas muestra `max(real, fake)`
- en apuestas o salas cerradas muestra el real
- no publica propiedades extra en Fusion

Rollback rapido:
1. Restaurar ambos backups sobre sus archivos actuales
2. O revertir el commit de este ajuste UI-only con git

Prueba minima recomendada:
1. Sala free nueva: la lista y el panel deben mostrar mas de 0
2. Sala de apuesta: la lista y el panel deben seguir mostrando solo reales
3. Sala free con el tiempo: el conteo visual debe subir sin afectar join o inicio
