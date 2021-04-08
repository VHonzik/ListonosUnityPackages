# Listonos' Time System

Listonos' Time System is a real-world time simulation system that supports pausing, two faster time passage presets, callbacks system and comes with UI elements prefabs and scripts. The main use case is grand strategy style games.

The UI elements consist of `TimeDisplay` and `TimeControls` prefabs and scripts. `TimeDisplay` displays a formatted time, format being completely customizable, into TextMeshPro GUI text. `TimeControls` is a group of three Listonos's UI buttons for resuming/pausing, fast and super speed time passage changes.


## Usage

For using only the system itself simply add `TimeSystem` behavior to any game object. Since it acts as singleton you can reference it anywhere from code with `TimeSystem.Instance`. It will simulate itself by using the Unity's `Update` message.

For the UI elements simply drag `TimeDisplay` or `TimeControls` prefabs into a canvas hierarchy.

