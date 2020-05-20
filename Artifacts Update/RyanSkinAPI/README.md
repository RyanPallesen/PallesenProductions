## General information
This mod allows you to load custom skins into the game very easily.
tutorial below.

To get the default skins for editing, set the config in BepInEx/skins to true.

#Discord
https://discord.gg/fy7JGze

Changelog; fixed an issue where custom characters did not have a default skin.

#Tutorial

Creating skins is easy as pie.
#### Directory
Navigate to BepInEx/Skins.
Skins are ALWAYS loaded from BepInEx/skins/[FolderWithLotsOfSkins]/[FolderWithOneSkin]
if you do not adhere to the above, you will crash it for everyone involved.

#### Defaults
in BepInEx/Skins there will be multiple folders that are automatically generated from the default skins.
At the end of the folder is a number denoting which slot those skins are from.

#### Meta
each skin folder contains a Meta.txt
each skin folder must contain a Meta.txt
a Meta.txt contains three lines
	Line 1: Name of the character this skin is for.
	Line 2: The model replacement to use as a base for this skin. (E.G, merc's samurai outfit vs normal outfit)
	Line 3: The name of this skin. if left as "BOILERPLATE", it will not load.

#### Creating a skin
Duplicate any of the existing default folders into your own folder in BepInEx/skins
go through the textures with some form of editing software, photoshop, or otherwise.
The textures must have the same names as is saved in the boilerplate.
The textures must be supplied as a PNG.
set up the meta.txt correctly.
The game will automagically find all skins in BepInEx/skins on startup if they've been done correctly.