# Wannaplay MagnusSdk EvTruck

## Dependencies
- `com.wannaplay.magnus-sdk.core` >= `0.0.1`
- `jillejr.newtonsoft.json-for-unity` >= `12.0.201`

## Installation
There are several ways to install the module:
- Via Package Manager from Wannaplay UPR **\*(for Wannaplay employees only)**;
- Clone into your project's Packages directory (use as custom package).

## Usage

```c#
using MagnusSdk.EvTruck;

...

await Magnus.Initialize("magnusApiKey"); // Initialize Magnus first
EvTruck.Initialize();

...

EvTruck.SetUserProperty("user_id", userId);
EvTruck.SetUserProperty("install_source", installSource);
 
...
 
EvTruck.TrackEvent(eventName, parameters);

EvTruck.TrackEvent("navigation", new Dictionary<string, object>
{
    {"screenName", screenName}
});
 
 ```

## Class API

### `static class EvTruck`

**Static** class that provides EvTruck api methods.

### Methods

#### `Initialize()` : `void`
Initialize EvTruck

#### `SetUserProperty(string propName, object propValue)` : `void`
Sets the analytic property of the current user

Params:
- `propName`: `string` - Name of user property
- `propValue`: `object` - Value of user property

#### `TrackEvent(string eventName, Dictionary<string, object> eventParams = null)` : `void`
Tracks user's events

Params:
- `eventName`: `string` - Name of event
- `eventParams`: `Dictionary<string, object>` - **Optional** event parameters

