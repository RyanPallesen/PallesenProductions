

#Discord
https://discord.gg/fy7JGze

Changelog; Made skins opt-in. Redid readme, fixed errors with custom characters.

#Tutorial

To load skins and skin packs into the game; place them in BepInEx/Plugins

Outputting default skins; Click "Output Skins" In the menu.

Creating skins; rename the root folder of the outputted skins, edit the relevant image files.
	MainTex -> The texture on this skin
	Material.cfg -> A list of variables you can change, such as emission colour and level.
	Skin.cfg -> A list of values related to the skin itself, such as the character it is for and the colours used for it's sprite.

Do not change the name of folders that are not the root folder.
Do not change the name of any files.

#Compatability

For custom survivors that work with skins, or if you would like to test, Please do the following:
myCharacterGameObject.tag = "SkinReady";



