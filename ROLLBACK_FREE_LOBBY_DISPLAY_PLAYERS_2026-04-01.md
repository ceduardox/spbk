Checkpoint base antes del cambio:
- `e0768a8` `Checkpoint before free lobby display players`

Archivo modificado:
- [`Assets/Scripts/Networking/GameLauncher.cs`](/g:/SUPERBLOK/PROSUPERBLOK4/PROSuperblock_The_Game_V3/Assets/Scripts/Networking/GameLauncher.cs)

Backup local:
- [`codex_backups/2026-04-01_free_lobby_display_players/GameLauncher.cs.bak`](/g:/SUPERBLOK/PROSUPERBLOK4/PROSuperblock_The_Game_V3/codex_backups/2026-04-01_free_lobby_display_players/GameLauncher.cs.bak)

Que hace este cambio:
- publica `DisplayPlayers` solo en salas free
- no toca `session.PlayerCount`
- no toca apuestas
- la lista usa `DisplayPlayers` solo si existe y solo si la sala esta abierta y `Bet <= 0`

Rollback rapido:
1. Restaurar el archivo backup sobre `Assets/Scripts/Networking/GameLauncher.cs`
2. O revertir el commit del cambio con git

Prueba minima recomendada:
1. Sala free recien creada: la lista debe mostrar un numero visual mayor que cero
2. Sala free con bots entrando: la lista debe subir sin romper join
3. Sala de apuesta: la lista debe seguir mostrando el conteo original
