# Unity-Terrain-Generator-benzo32
The TerrainMaker provides an editor window for Unity, enabling users to create customized terrain layouts within the Unity editor using various prefabricated objects. Users can add different types of objects such as trees, rocks, and grass. Parameters like the total number of objects and the number of prefabs to use can be adjusted through the interface.

The tool also generates a Terrain object that conforms to user-defined dimensions (width and length). A specified number of trees, rocks, and grass are randomly placed on the created terrain. Placement of each object involves attempting to find a suitable position on the designated terrain until a valid location is found.

Additionally, the code manages the saving and loading of object paths via EditorPrefs, allowing users to easily define and persistently use their own prefabs. This ensures that user-defined prefabs can be readily utilized and managed across sessions.
