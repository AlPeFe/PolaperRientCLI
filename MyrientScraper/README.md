# Myrient Downloader

Una aplicación de consola en .NET 10 para scrapear y descargar archivos de juegos de Myrient usando Spectre.Console.

## Características


- .NET 10.0 SDK
## Features

- Downloads files from Myrient using HTTP.
- Shows download progress.
- Allows multiple simultaneous connections.
- Easy to integrate into scripts or automated systems.

## Usage

## Instalación

```bash
cd MyrientScraper
dotnet restore
dotnet build
```

## Cómo Ejecutar

### Opción 1: Usar `dotnet run` (desarrollo)

```bash
dotnet run
```

### Opción 2: Usar el ejecutable compilado

```bash
dotnet build
./bin/Debug/net10.0/MyrientScraper
```

### Opción 3: Usar la versión publicada (recomendado)

```bash
dotnet publish -c Release -o ./publish
cd publish
./MyrientScraper
```

## Uso Interactivo

La aplicación es 100% interactiva con Spectre.Console:

1. **Menú Principal**: Selecciona entre:
   - Scrape URL - Scrapear una URL nueva
   - Load JSON - Cargar datos guardados anteriormente
   - Quit - Salir de la aplicación

2. **Scrapear URL**:
   - Ingresa la URL de Myrient (ej: https://myrient.erista.me/files/No-Intro/...)
   - La aplicación scrapeará automáticamente los archivos
   - Los datos se guardan en `./scraped_data/`

3. **Seleccionar Archivos**:
   - Navega con las flechas ↑↓
   - Selecciona/deselecciona con [Espacio]
   - Confirmar selección con [Enter]

4. **Confirmar Descarga**:
   - Revisa el resumen de archivos seleccionados
   - Confirma la descarga
   - Se muestra el progreso de cada archivo

## Navegación

- **↑↓ Flechas**: Navegar entre opciones
- **[Espacio]**: Seleccionar/deseleccionar archivos
- **[Enter]**: Confirmar selección
- **[Esc]**: Cancelar/volver atrás
- **Y/N**: Confirmar diálogos sí/no

## Estructura del Proyecto

- `Models.cs` - Estructuras de datos (GameFile, ScrapedData)
- `Scraper.cs` - Lógica de web scraping con AngleSharp
- `Storage.cs` - Guardar/cargar archivos JSON
- `Downloader.cs` - Gestión de descargas con progreso
- `UserInterface.cs` - Interfaz de usuario con Spectre.Console
- `Program.cs` - Punto de entrada

## Dependencias

- `Spectre.Console` - Biblioteca de UI de terminal
- `AngleSharp` - Biblioteca de parsing HTML

## Ejemplo de Flujo de Uso

1. Ejecutar: `./MyrientScraper`
2. Seleccionar "Scrape URL"
3. Ingresar URL: `https://myrient.erista.me/files/No-Intro/Nintendo%20-%20Nintendo%203DS%20(Decrypted)/`
4. Esperar el scraping...
5. Navegar y seleccionar archivos con flechas y espacio
6. Presionar Enter para confirmar
7. Revisar resumen y confirmar descarga
8. Ver progreso de descargas
9. Los archivos descargados estarán en `./downloads/`

## Notas

- Los archivos scrapeados se guardan automáticamente en `./scraped_data/`
- Los archivos descargados se guardan en `./downloads/`
- Puedes cargar datos previamente scrapeados usando "Load JSON"
- La interfaz es completamente interactiva, no requiere comandos de línea
