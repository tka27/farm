# Wannaplay MagnusSdk Core

## Dependencies
- `com.google.external-dependency-manager` >= `1.2.164`
- `jillejr.newtonsoft.json-for-unity` >= `12.0.201`

## Installation
There are several ways to install the module:
- Via Package Manager from Wannaplay UPR **\*(for Wannaplay employees only)**;
- Clone into your project's Packages directory (use as custom package).

## Usage

```c#
using MagnusSdk.Core;

...

Magnus.SetAppsFlyerIdGetter(AppsFlyer.getAppsFlyerId); // Oprional step: set AppsFlyer id getter
await Magnus.Initialize(_analyticsKeys.magnusApiKey);
```

## Class API

### `static class Magnus`

**Static** class that provides Magnus api methods.

### Methods

#### `async Initialize(string token)` : `Task`

Initializes Magnus SDK, fetch and cache all devices and users properties uses by another SDK libs

Params:
- `token`: `string` - Application Magnus API token

**Returns** `Task`

#### `SetAppsFlyerIdGetter(Func<string> getter)` : `void`

Sets AppsFlyer id getter to provide that to another SDK libs

Params:
- `getter`: `Func<string>` - Function that returns AppsFlyer user id
