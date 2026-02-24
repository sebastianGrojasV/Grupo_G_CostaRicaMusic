# Mejoras sugeridas para convertir Costa Rica Music en un prototipo tipo Spotify

## Diagnóstico rápido del estado actual
- La página principal funciona como un **MVP funcional**: lista de canciones + reproductor embebido con `audio` HTML5.
- Actualmente se muestra una única vista con dos paneles y reproducción al hacer clic.
- En backend ya existen piezas de dominio útiles (artistas, álbumes, canciones, playlists, agregar/quitar canciones), pero con datos en memoria.

## Prioridad 1 — Experiencia core de reproducción (imprescindible)
1. **Barra de reproducción fija global (footer player)**
   - Mostrar portada, título, artista, progreso, volumen, botón play/pause, siguiente/anterior.
   - Mantenerse visible aunque el usuario cambie de página.
2. **Cola de reproducción (Up Next)**
   - Reproducir siguiente canción automáticamente.
   - Permitir reordenar cola (drag/drop) y activar repeat/shuffle.
3. **Estados visuales por canción**
   - `playing`, `paused`, `selected` y `hover` para que el usuario entienda qué suena.

## Prioridad 2 — Navegación tipo Spotify
1. **Layout de 3 áreas**
   - Sidebar izquierda: Inicio, Buscar, Tu biblioteca, playlists.
   - Contenido central: vista dinámica (home, search, album, artista, playlist).
   - Panel derecho opcional: cola o “ahora sonando”.
2. **Ruteo por vistas**
   - Páginas separadas para Artista, Álbum y Playlist con URL amigables.
3. **Cabecera de contexto**
   - Buscador global y breadcrumbs para mejorar descubrimiento.

## Prioridad 3 — Catálogo y descubrimiento
1. **Búsqueda unificada real**
   - Buscar por canción, artista y álbum en una sola caja con resultados agrupados.
2. **Secciones de descubrimiento**
   - “Hecho para ti”, “Tendencias CR”, “Recientes”, “Más reproducidas”.
3. **Metadatos enriquecidos**
   - Imágenes de artistas/álbumes, duración formateada, año, género.

## Prioridad 4 — Playlists y biblioteca personal
1. **CRUD completo de playlists en UI**
   - Crear, editar nombre/descripcion, eliminar, agregar/quitar canciones.
2. **Like/Favoritos**
   - Corazón por canción/álbum para feed de preferencias.
3. **Biblioteca del usuario**
   - Tabs: canciones guardadas, álbumes guardados, artistas seguidos.

## Prioridad 5 — Autenticación y personalización
1. **Login/registro con sesión persistente**
   - JWT/cookies + expiración controlada.
2. **Perfiles de usuario**
   - Nombre, avatar, preferencias iniciales.
3. **Recomendaciones básicas**
   - Primero reglas simples (popularidad + afinidad por artista), luego modelo más avanzado.

## Prioridad 6 — Backend y arquitectura
1. **Migrar de in-memory a base de datos relacional**
   - SQL Server/PostgreSQL con EF Core.
2. **API REST consistente**
   - Endpoints para catálogo, búsqueda, biblioteca, historial y recomendaciones.
3. **Paginación, filtros y ordenamientos**
   - Escalabilidad para catálogos grandes.
4. **Observabilidad mínima**
   - Logs estructurados, métricas de reproducción, health checks.

## Prioridad 7 — Calidad visual y UX
1. **Sistema visual coherente (design tokens)**
   - Espaciado, tipografía, colores, estados y componentes reutilizables.
2. **Modo responsive real**
   - Mobile first: navegación inferior en celular y sidebar colapsable.
3. **Microinteracciones**
   - Animaciones suaves en hover, cambio de vista, skeleton loaders.
4. **Accesibilidad**
   - Contraste AA, navegación por teclado, labels ARIA y foco visible.

## Prioridad 8 — Métricas de producto
1. **Eventos clave**
   - Play, pause, skip, search, add-to-playlist, like.
2. **Embudo de activación**
   - Registro -> primera reproducción -> primera playlist -> segunda sesión.
3. **Dashboards básicos**
   - Canciones más reproducidas, retención diaria y uso por feature.

## Roadmap sugerido (8 semanas)
- **Semana 1-2:** layout tipo Spotify + player global + estados de reproducción.
- **Semana 3-4:** búsqueda unificada + vistas artista/álbum + metadatos visuales.
- **Semana 5-6:** playlists completas + biblioteca del usuario + login básico.
- **Semana 7:** migración a DB + hardening API + paginación.
- **Semana 8:** recomendaciones iniciales + métricas + ajustes UX/accessibility.

## Quick wins (pueden hacerse en días)
1. Agregar portada por canción en el listado.
2. Destacar visualmente la canción activa.
3. Crear barra de búsqueda simple en la parte superior.
4. Separar “Inicio” y “Playlist” en vistas distintas.
5. Persistir última canción escuchada en `localStorage`.
