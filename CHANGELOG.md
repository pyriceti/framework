﻿# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.3.0a1] - 2021-08-19
### Added
- Add `ClickDetector` template class in `_Template_ClickDetector.cs`.
- **\[Breaking Changes\]** Add `EngineManager.subscribe()` and `EngineManager.unsubscribe()` static methods to
  reduce code boilerplate (replace `EngineManager.Instance.subscribe()` and `EngineManager.Instance.unsubscribe()`
  calls).
- Add `EngineObject.aliveCtsList` to properly cancel and dispose alive tokens upon destruction.
- Add `TaskUtil.RefreshToken()` override with a CancellationTokenSource list param to replace old cts ref with new one.
- Add `eUIVisibilityState` enum designed for UI elements.
- Add `MenuPanel` class with visibility methods and logic: `show()`, `hide()`, `Visibility`, `VisibilityState`… Also handle correctly visibility CTS.
- Add `MenuPanel.cm_show()` and `MenuPanel.cm_hide()` methods.
- Add generic `MenuPanel<TController>` class.
- Add `BeforePanelShownEvt`, `PanelShownEvt`, `BeforePanelHiddenEvt` and `PanelHiddenEvt` events.
- Add `MenuPanelTest` class for future tests.
- Add `CanvasGroupExtensions.HideAndDisable()` and `CanvasGroupExtensions.ShowAndEnable()` methods.
- Add `StringExtensions.CustomStartsWith()` and `StringExtensions.CustomEndsWith()` methods.
- Add `VectorExtensions.Offset[X-Z]()` methods.
- Add generic `EngineObject<TController>` class.
- Add `EngineController.getController<T>()` static method.
- Add generic `EngineController<TSettings>` class. 
- Add `AnimationParams.duration` field as an alternative to the speed field.
- Add `AbstractSettings` class.
- Add `ProjectDataWindow` editor custom window with basic features.

### Changed
- Move `ProjectData` in `com.pyriceti.framework` assembly since it may be used during runtime.
- Change `StartupData` with fixed `"PyricetiFramework/StartupData"` menu name entry. 

### Fixed
- Fix `EngineObject.OnDestroy()`, clearing CTS only if cancellation hasn't been requested.
- Fix `Singleton.Quitting` Property not being reset to false on runtime.
- Fix `com.pyriceti.framework.editor` assembly dependencies to add `com.pyriceti.framework` since it relies on it.

## [0.2.0] - 2021-03-26
### Added
- Default example scenes `_Engine` and `_Startup` to get started.
- Static `initDomain()` methods in classes with static fields to support Editor Enter Play Mode Options with "Reload Domain" disabled.
- `EngineManager` now handles a dynamic resizable array of subscribed `EngineObject`. It will call their `updateEngine()` method every frame (Update-like) until they unsubscribe.
  Serialized field `EngineManager.startEngineObjsArrSize` controls the starting array size to prevent too many resizable operations when game loads.
- `ArrayExtensions` script with `RemoveAt()` method.
- `TaskUtil` script with `RefreshToken()` method.
- Tests folder with some (very) simple tests.
- Documentation folder and basic documentation file.

### Changed
- Change package folders tree to provide Runtime non-scripting assets, like Prefabs or Scenes.
- **\[Breaking Changes\]** Rename `ScenesManager.loadScene()` methods to `ScenesManager.loadSceneAsync()`.
- Split custom attributes with their respective custom drawers, and move the latter in their own Editor Assembly.
- **\[Breaking Changes\]** Convert all Engine scripts coroutines into UniTasks.
- **\[Breaking Changes\]** Changed `EngineObject.updateEngine()` to `public` visibility.
- **\[Breaking Changes\]** `EngineObject.updateEngine()` needs a manual call to `EngineManager.Instance.subscribe()` to work.

### Fixed
- Update `ScenesManager.isSceneLoaded()` to better support Editor Enter Play Mode Options with "Reload Scene" disabled.
- Fix `com.pyriceti.framework.editor` platforms scope to Editor only.
- Add `CancellationToken` support in Engine scripts (like `EngineObject`) to prevent going on further in operations, for instance if objects are destroyed.

## [0.1.0] - 2021-03-19
### Added
- Base code for engine (`EngineController`, `EngineManager`, `EngineObject`).
- Editor startup logic with bootstrap behaviour depending on scenes referenced in `StartupData`.
- Base code for controllers and `ControllersProvider` class.
- Some utilities like `ReadOnlyAttribute` or `Singleton` classes.
