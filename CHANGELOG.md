# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## Unreleased
* Added zoom slider
* Reduced UI managed allocations

## [0.4.2-preview] - 2021-02-01
* Fixed detection of API calls using a derived type
* Fixed reporting of *Editor Default Resources* shaders
* Fixed *ReflectionTypeLoadException*
* Added *SRP Batcher* column to Shader tab
* Added support for parsing *Player.log* to identify which shader variants are compiled (or not-compiled) at runtime
* Added Shader errors/warnings reporting via Shader 'severity' icon
* Added [Shader Requirements](https://docs.unity3d.com/ScriptReference/Rendering.ShaderRequirements.html) column to Shader tab
* Fixed exception when switching focus from Area/Assembly window
* Fixed null ref exception on invalid shader or vfx shader
* Fixed null ref exception when building AssetBundles
* Fixed shader variants reporting due to *OnPreprocessBuild* callback default order

## [0.4.1-preview] - 2020-12-14
* Improved Shaders auditing to report both shaders and variants in their respective tables
* Added support for analyzing Editor only code-paths
* Added *reuseCollisionCallbacks* physics API diagnostic
* Fixed Assembly-CSharp-firstpass asmdef warning
* Fixed backwards compatibility

## [0.4.0-preview] - 2020-11-24
* Added Shader variants auditing
* Added "Collapse/Expand All" buttons
* Refactoring and code quality improvements

## [0.3.1-preview] - 2020-10-23
* Page up/down key bug fixes
* Added dependencies view to Assets tab
* Move call tree to the bottom of the window
* Double-click on an asset selects it in the Project Window
* Fixed Unity 2017 compatibility
* Fixed default selected assemblies
* Fixed Area names filtering
* Changed case-sensitive string search to be optional
* Added CI information to documentation
* Fixed call-tree serialization

## [0.3.0-preview] - 2020-10-07
* Added auditing of assets in Resources folders
* Added shader warmup issues
* Reorganized UI filters and mute/unmute buttons in separate foldouts
* Fixed issues sorting within a group
* ExportToCSV improvements
* Better names for project settings issues

## [0.2.1-preview] - 2020-05-22
* Improved text search UX
* Improved test coverage
* Fixed background assembly analysis
* Fixed lost issue location after domain reload
* Fix tree view selection when background analysis is enabled
* Fixed Yamato configuration
* Updated documentation

## [0.2.0-preview] - 2020-04-27
* Added Boxing allocation analyzer
* Added Empty *MonoBehaviour* method analyzer
* Added *GameObject.tag* issue type to built-in analyzer
* Added *StaticBatchingAndHybridPackage* analyzer
* Added *Object.Instantiate* and *GameObject.AddComponent* issue types to built-in analyzer
* Added *String.Concat* issue type to built-in analyzer
* Added "experimental" allocation analyzer
* Added performance critical context analysis
* Detect *MonoBehaviour.Update/LateUpdate/FixedUpdate* as perf critical contexts
* Detect *ComponentSystem/JobComponentSystem.OnUpdate* as perf critical contexts
* Added critical-only UI filter
* Optimized UI refresh performance and Assembly analysis
* Added profiler markers
* Added background analysis support

## [0.1.0-preview] - 2019-11-20
* Added Config asset support
* Added Mute/Unmute buttons
* Replaced Filters checkboxes with Popups
* Added Assembly column

## [0.0.4-preview] - 2019-10-11
* Added Calling Method information
* Added Grouped view to Script issues
* Removed "Resolved" checkboxes
* Lots of bug fixes

## [0.0.3-preview] - 2019-09-04
* Fixed Unity 2017.x backwards compatibility
* Added Progress bar
* Added Package whitelist
* Added Tooltips

## [0.0.2-preview] - 2019-08-22

### First usable version

*Replaced placeholder database with real issues to look for*. This version also allows the user to Resolve issues.

## [0.0.1-preview] - 2019-07-23

### This is the first release of *Project Auditor*

*Proof of concept, mostly developed during Hackweek 2019*.
