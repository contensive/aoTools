# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture Patterns

Contensive5 follows several key patterns documented in the public repository:

- [Core architecture overview](https://github.com/contensive/contensive5/blob/master/patterns/contensive-architecture.md)


## Project Overview

**aoTools** is a Contensive CMS addon collection providing administrative tools and reusable design block components. The codebase demonstrates professional addon development patterns using an MVC-inspired architecture layered on top of the Contensive platform.

## Build and Development Commands

### Building the Project

```cmd
# Navigate to scripts directory
cd scripts

# Run build script (auto-generates version number based on date)
build.cmd

# Alternative build using dotnet CLI directly
cd server
dotnet clean tools.sln
dotnet build tools/tools.csproj --configuration Debug
```

The build script (`scripts/build.cmd`):
- Auto-generates version numbers in format YY.MM.DD.revision
- Builds the C# solution using dotnet CLI
- Packages UI files into `uiTools.zip`
- Copies DLLs to `Collections/Tool Basics/`
- Creates `Tool Basics.zip` addon collection
- Deploys to `C:\Deployments\aoTools\Dev\{version}\`

### Project Structure

```
server/tools/Tools.sln    # Main Visual Studio solution
server/tools/Tools.csproj # C# project file (.NET Framework 4.7.2)
scripts/build.cmd         # Build and packaging script
Collections/Tool Basics/  # Addon collection output
ui/                       # UI files (HTML forms)
```

## Architecture

### MVC + Addon-Based Pattern

```
Contensive Platform (CPBaseClass)
    |
    +-- Addons (entry points) → Execute(CPBaseClass cp)
        |
        +-- Controllers (business logic & utilities)
        +-- Models (data access & transformation)
            +-- Db Models (database entities)
            +-- View Models (DTOs for presentation)
            +-- Domain Models (business entities)
        +-- Views (UI rendering addons)
```

### Code Organization

**Addons/** - Entry points inheriting from `AddonBaseClass`
- All override `Execute(CPBaseClass cp)` method
- Examples: CacheToolClass, SqlSchemaTool, InviteUsersTool, OnInstallClass

**Controllers/** - Static utility classes
- `GenericController` - Date/time, string validation utilities
- `DesignBlockController` - Design block lifecycle management
- `ApplicationController` - JSON API response handling

**Models/** - Three-tier model organization
- **Db/** - Entity models mapping to database tables (inherit from `DbBaseModel`)
- **View/** - DTOs optimized for presentation
- **Domain/** - Business entities with reflection-based mapping

**Views/** - Addon classes that render UI/content
- Design block addons for drag-and-drop page components
- Form/page addons for user interactions

### Key External Dependencies

- **Contensive.DbModels** (v25.7.18.3) - Database entity base classes
- **Contensive.CPBaseClass** (v25.7.18.3) - Main platform API (`CPBaseClass cp`)
- **Contensive.DesignBlockBase** (v25.4.26.1) - Design block framework

### CPBaseClass API (`cp` parameter)

The `CPBaseClass cp` parameter provides comprehensive platform access:

- `cp.AdminUI` - Build admin forms and UI components
- `cp.Cache` - Caching operations (GetObject, Store, Invalidate)
- `cp.Content` - Content management (GetRecordName, GetEditWrapper)
- `cp.Db` / `cp.CSNew()` - Database operations and cursors
- `cp.Doc` - Request data (GetText, GetInteger, GetBoolean)
- `cp.Email` - Email sending
- `cp.JSON` - JSON serialization/deserialization
- `cp.Mustache` - Template rendering
- `cp.Site` - Site operations (ErrorReport for logging)
- `cp.User` - User management and authentication

## Design Patterns

### Factory Pattern for Models

Models use static factory methods for creation:

```csharp
// Create or fetch existing model
public static SampleModel createOrAddSettings(CPBaseClass cp, string settingsGuid) {
    SampleModel result = create<SampleModel>(cp, settingsGuid);
    if (result == null) {
        result = addDefault<SampleModel>(cp);
        result.ccguid = settingsGuid;
        result.save(cp);
    }
    return result;
}
```

Common patterns: `create<T>()`, `createOrAdd<T>()`, `addDefault<T>()`, `createList<T>()`

### Design Block Execution Flow

1. Get/create instance ID using `DesignBlockController.getSettingsGuid()`
2. Load or create settings model with `createOrAddSettings()`
3. Transform to view model
4. Render using Mustache templates: `cp.Mustache.Render(template, viewModel)`
5. Wrap for editing: `cp.Content.GetEditWrapper(html, contentName, recordId)`

### Model Transformation Pipeline

```
Database Model (Db/)
    ↓ (static factory method)
View Model (View/)
    ↓ (cp.Mustache.Render)
HTML Output
```

### Error Handling Convention

```csharp
try {
    // addon logic
} catch (Exception ex) {
    cp.Site.ErrorReport(ex);  // Log to platform
    throw;                     // Re-throw
}
```

## Development Conventions

1. **Naming**: PascalCase for classes, camelCase for properties
2. **Addon Entry Point**: All addons inherit from `AddonBaseClass` and override `Execute(CPBaseClass cp)`
3. **Constants**: Centralized in `constants.cs` (GUIDs, error codes, route endpoints)
4. **Generic Programming**: Use type parameters for reusable model operations
5. **UI Building**: Use `cp.AdminUI` builders for consistent admin styling
6. **Templates**: Use Mustache for template-based rendering
7. **Caching**: Use `cp.Cache` for frequently accessed data
8. **Reflection**: BaseDomainModel uses reflection for dynamic property mapping from database cursors

## Common Addon Types

- **Admin Tools** - Utility addons for site administration (Cache Tool, SQL Schema Tool)
- **Design Blocks** - Reusable UI components added via drag-and-drop
- **Form Handlers** - Process form submissions (SubmitInviteProfile)
- **Lifecycle Hooks** - Initialization handlers (OnInstallClass)
- **Database Tools** - Schema management utilities

## Namespace Structure

```
Contensive.Addons.Tools (root namespace)
├── Addons/
├── Views/
├── Controllers/
└── Models/
    ├── Db/
    ├── View/
    └── Domain/
```

## Build Output

- **Target Framework**: .NET Framework 4.7.2
- **Assembly**: Tools.dll (strong-named with ToolsKey.snk)
- **Output Path**: `server/tools/bin/Debug/` or `bin/Release/`
- **Collection Package**: `Collections/Tool Basics/Tool Basics.zip`
- **Embedded Resources**: HTML layouts and SQL scripts compiled into assembly
