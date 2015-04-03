# Propeller Microcontroller-based projects by trodoss #

## Game Code: ##

**Fantasy Fighter:**
This project is a very basic RPG-style game, inspired by the "Dragon Warrior Type Game" thread (http://forums.parallax.com/showthread.php?t=119035),

**FFDataBuilder:**
A command-line application (uses a specific format, see "Game\_Data.txt" in the .zip file) to create/edit the data used in the game.

FFDataBuilder is a C# command-line applications (source included) compiled under .NET framework 2.0 (v2.0.50727). If you have the Mono runtime installed on Linux, you should be able to run these as well.


**Mythic Flight:**
This project is a side-scrolling flying game.

The game version 0.9 (AIGeneric) uses AIGeneric v2.1 driver, with some alterations.
The game version 1.1 (ORE) uses the ORE Text driver (4-color), also with some alterations.

The game version 0.9 uses the Map Generator v0.6, and the game version 1.1 uses Map Generator v0.7


**PAGE (Propeller Adventure Game Engine):**
This project is an adventure game engine in the same style of LucasArt's SCUMM (Script Utility for Maniac Mansion) and Sierra-Online's AGI (Adventure Game Engine).


## Utility Code: ##

**Font Generator:**
A command-line application used to create "font" (1bpp, 2bpp 8x8 characters) to be used with AIGeneric or Potato\_Text-based code. It converts a black/white bitmap image into the Spin source that can be included with your applications.

**Graphics Convert:**
A command-line application used to convert various graphics format files to output usable by either Font Generator or Propeller-based display objects.

Font Generator an Graphics Convert are C# command-line applications (source included) compiled under .NET framework 2.0 (v2.0.50727). If you have the Mono runtime installed on Linux, you should be able to it as well.

Both of the applications are bundled int the Graphics Utilities archive.


_Propeller-Based Utility Code_

**Graphics Editor:**
A Propeller (Spin) application used to create sprite/graphics in multiple modes and dimensions.

Graphics Editor, unlike the Font Generator, will supports editing/viewing of the characters in multiple "View Modes" (such as 8x8 or 16x16), so that game-style sprite graphics can be created with the characters.
