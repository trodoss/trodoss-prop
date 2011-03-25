#INCLUDE "global.bi"
Room ROOM_INSIDE_SHED
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("inside_shed.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
    Dim hotDoor as Hotspot (32, 62, 12, 4)

	'-----------------
	'On_Load event
	'-----------------   
	Event On_Load	
		Ego.Load(36, 44, DIR_NORTH)
				
	End Event 
	
	'-----------------
	'On_Hotspot1 event
	'-----------------   
	Event On_Hotspot1	
		Room.Load(ROOM_OLD_SHED)
	End Event		
			
End Room
