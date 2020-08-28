#Discord
https://discord.gg/fy7JGze

Changelog; wrapped API in try catch
Now contains ability to do minion skin replacements

#### Defaults
To output default survivor skins; enter 'DumpSurvivorSkins' into the console (Can be opened via   ctr + alt + `   while in game)
To output all characterbody skins; enter 'DumpAllSkins' into the console (Can be opened via   ctr + alt + `   while in game)

#### Directory
Navigate to BepInEx/plugins/defaultskins after outputting defaults.
Skins are loaded from BepInEx/plugins/[MyModsName]/plugins/[FolderWithLotsOfSkins]/[FolderWithOneSkin]
if you do not adhere to the above, you will crash it for everyone involved.

in BepInEx/plugins/defaultskins there will be multiple folders that are automatically generated from the default skins.
Duplicate this entire folder, or rename it, and edit the skins found inside.

#### Creating a skin
go through the textures with some form of editing software, photoshop, or otherwise.
The textures must have the same names as is saved in the boilerplate.
The textures must be supplied as a PNG.
The game will automagically find all skins in BepInEx/skins on startup if they've been done correctly.

#### Skinnable Characters
If you've made a character than can be skinned, please do the following; `MyReallyCoolCharacterGameObject.tag = "Player"`