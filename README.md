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

# MyrientScraper

MyrientScraper is a simple command-line tool for downloading multiple ROMs (game files) at once from the Myrient archive. It is designed to make it easy to scrape and download games quickly, with support for downloading several files simultaneously.

## Key Features

- Download many ROMs at the same time (multi-download)
- Easy to use, no technical knowledge required
- Compatible with Windows and Linux
- Shows download progress for each file
- Saves downloaded files in a dedicated folder

## How It Works

1. Start the program and enter the URL of the Myrient archive folder you want to scrape.
2. The tool will list all available ROM files.
3. Select the games you want to download.
4. The program will download all selected files at once, showing progress for each.
5. Your ROMs will be saved in the `downloads` folder.

## Getting Started

### Requirements
- .NET 10.0 or later
- Windows or Linux

### Build and Run
To build:

```
dotnet build MyrientScraper/MyrientScraper.csproj
```

To run:

```
sh MyrientScraper/run.sh
```

Or:

```
dotnet run --project MyrientScraper/MyrientScraper.csproj
```

## License
MIT License

## Author
Polaper

