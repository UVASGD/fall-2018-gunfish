public class README {
/*
LEVEL TEMPLATE INFO

This template was made in an effort to streamline the process of making levels.
Please read this before you make a level, it will save a lot of headaches.

Getting started
/******************************************************************************
 * How to set up a level:
 * Find this level in the project view and press Ctrl + D (Cmd + D on Mac) to
 * duplicate it.
 * 
 * Where to put the level:
 * If it's finished, Assets/Resources/Scenes/Race/
 * If it's in progress, Assets/Resources/Scenes/Race Unused/


Naming convention
/******************************************************************************
 * For the sake of organization, we're trying to keep levels to a naming
 * convention. Your level should be called Level_(levelNumber)_(levelName). The
 * number is so that they appear in the Race folder in sequential order of when
 * they were made, and the name is that so they are easily identifiable without
 * having to open it to see which one it is.


Level building convention
/******************************************************************************
 * When building your level, please keep assets in an organized hierarchy.
 * Such a hierarchy is provided in the template, under "LevelObjects". You don't
 * have to use this specific organization scheme, but I would recommend doing so.
 * Basically, any two objects that are of the same time, such as walls, should
 * be grouped together as children of the "Walls" transform. This makes the level
 * hierarchy easy to read and navigate as your levels get more complex.


Assets
/******************************************************************************
 * All the currently working assets are present in the template. There is 1 type
 * of wall and 9 obstacles. Feel free to delete any assets that you don't want 
 * to use. They are all also located in Assets/Resources/Prefabs/Level_Assets.


Testing the Level
/******************************************************************************
 * This is a networked project, so you can't simply press play on your scene to
 * test it. Currently, the required testing method is kind of annoying.
 * 
 * You have to press play on the Menu scene. Press Ctrl+T at any point to swap
 * between this scene, and the scene you were last working in.
 * 
 * Any time you make an update to the level, you have to rebuild the
 * AssetBundles if you want to test it. This can be done by clicking on
 * Assets/Build AssetBundles in the menu toolbar.
 * 
 * If you want to simply test your one level alone, it's kind of annoying.
 * Move all levels except the one or ones you want to test in the Race folder
 * to the Race Unused folder, then Build AssetBundles, then play from the menu.


Known bugs and their solutions
/******************************************************************************
 * Colliders aren't functioning:
 * This is caused by a glitch with Unity's collider tiling system. To fix,
 * select all of the affected colliders. In the Collider2D component, click the
 * gear icon in the top right and hit "Reset". Then, in the component click on
 * the Auto-Tiling option
 * 
 * When the level loads, it's blank or the camera doesn't move:
 * It is very likely that the camera was not replaced with the appropriate
 * prefab camera. Be sure to replace it. Assets/Resources/Prefabs/Main Camera
 * 
 * The level doesn't load at all:
 * Make sure the level is in the Race folder as outlined above. If it is, be
 * sure to update the AssetBundles. On the menu bar up top, select
 * Assets/Build AssetBundles.
 * If it still doesn't load, make sure to add it to the build settings.
 * Ctrl+Shift+B, and click and drag the scene into the appropriate location in
 * the Scenes list.
*/
}
