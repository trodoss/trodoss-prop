#INCLUDE "global.bi"
Room ROOM_ROCKS
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("rocks.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (70, 23, 4, 56)

	'-----------------
	'On_Load event
	'-----------------   
	Event On_Load	
		Ego.Load(68, 32, DIR_WEST)
				
	End Event 
	
	'-----------------
	'On_Hotspot1 event
	'-----------------   
	Event On_Hotspot1	
		Room.Load(ROOM_BRIDGE)
	End Event	
End Room
