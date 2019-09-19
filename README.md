# Unity Screen Log
With this script you can display unity logs on the screen by simply adding the component UiLog to any gameObject in the scene.
- The log will persist even if a new scene load.
- The log will pause displaying if the game object is destroyed or deactivated manually, but if a new scene loads it will display all logs again.
- You can clear the log by script calling UiLog.Clear().
