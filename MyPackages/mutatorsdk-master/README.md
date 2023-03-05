# Wannaplay MagnusSdk Mutator

## Dependencies
- `com.wannaplay.magnus-sdk.core` >= `0.0.2`
- `jillejr.newtonsoft.json-for-unity` >= `12.0.201`

## Installation
There are several ways to install the module:
- Via Package Manager from Wannaplay UPR **\*(for Wannaplay employees only)**;
- Clone into your project's Packages directory (use as custom package).

## Usage

```c#
using MagnusSdk.Mutator;

...

await Magnus.Initialize("magnusApiKey"); // Initialize Magnus first

try
{
    Mutator.Initialize();
    await Mutator.FetchConfig();
    MutatorConfig сonfig = await Mutator.Activate();
}
catch (Exception e)
{
    Debug.LogError(e);
}

...

bool boolValue = сonfig.GetValue("bool_config_key").BoolValue;
string stringValue = сonfig.GetValue("string_config_key").StringValue;
double doubleValue = сonfig.GetValue("double_config_key").DoubleValue;
long longValue = сonfig.GetValue("long_config_key").LongValue;
object pureValue = config.GetValue("pure_config_key").Value; 
 ```

## Class API

### `static class Mutator`

**Static** class that provides Mutator api methods.

### Methods

#### `Initialize()` : `void`
Initialize Mutator

#### `async FetchConfig(int cacheDuration = 3600)` : `Task<JObject>`
Fetch and cache mutator config

Params:
- `cacheDuration`: `int` - config cache duration in seconds

**Returns** `Task<JObject>` - Parsed json response from server

#### `async Activate()` : `Task<MutatorConfig>`
Applies conditions and experiments, returns ready-to-use `MutatorConfig` instance

### `class MutatorConfig`

Class that provides ready-to-use configs values (`ParameterVariant`).

#### `GetValue(string key)` : `ParameterVariant`
Returns selected parameter variant

Params:
- `key`: `string` - mutator parameter key

**Returns** `ParameterVariant` - parameter wrapper fot provided key

#### `HasValue(string key)` : `bool`
Checks key for existing in config

Params:
- `key`: `string` - mutator parameter key

**Returns** `bool` - is parameter defined state

