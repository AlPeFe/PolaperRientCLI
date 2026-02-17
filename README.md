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

# MyrientScraper

MyrientScraper is a command-line tool for downloading and scraping game files from the Myrient archive. It is designed for fast, efficient downloads and provides progress reporting and file size formatting utilities.

## Features
- Download files from Myrient with simulated browser headers
- Progress reporting for downloads
- File size parsing and formatting utilities
- Customizable HTTP client settings for performance

## Usage

### Requirements
- .NET 10.0 or later
- Linux (tested)

### Build
To build the project:

```
dotnet build MyrientScraper/MyrientScraper.csproj
```

### Run
To run the scraper:

```
sh MyrientScraper/run.sh
```

Or directly:

```
dotnet run --project MyrientScraper/MyrientScraper.csproj
```

### Downloaded Files
Downloaded files are saved in the `downloads/` directory.

## Project Structure
- Downloader.cs: Handles file downloads and progress reporting.
- Scraper.cs: Main scraping logic.
- Models.cs: Data models for game files.
- Storage.cs: Handles storage and file management.
- UserInterface.cs: Command-line interface utilities.
- StringExtensions.cs: String helper methods.

## License
This project is licensed under the MIT License.

## Author
Polaper
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

