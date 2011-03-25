'-----------------
'CONstants
'-----------------
'Ego direction constants
Const DIR_NORTH = 0
Const DIR_SOUTH = 1
Const DIR_EAST  = 2
Const DIR_WEST  = 3

'include the room constants
#INCLUDE "rooms.bi"

'include the items in the game
#INCLUDE "items.bi"

'include the songs in the game
#INCLUDE "songs.bi"

'-----------------
'PICtures for the Ego
'-----------------
' The first Pic images defined 
' are always the Ego's images
'-----------------
Dim g_picEgo1 As Pic ("ego1.bmp")
Dim g_picEgo2 As Pic ("ego2.bmp")
Dim g_picEgo3 As Pic ("ego3.bmp")
Dim g_picEgo4 As Pic ("ego4.bmp")

'-----------------
'Default global variables
'-----------------
' The following global variables
' are pre-defined for use in
' the PINT interpreter:
' 
' global 0 = max score
' global 1 = current score
' global 2 = last room 
' global 3 = last item selected 
'-----------------
Dim g_varMaxScore As Byte
Dim g_varCurrentScore As Byte
Dim g_varLastRoom As Byte
Dim g_varLastItem As Byte

'include game setup
#INCLUDE "gamesetup.bi"
