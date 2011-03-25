#INCLUDE "global.bi"
Room ROOM_BRIDGE
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("bridge.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (0, 23, 4, 56)
	Dim hotspot2 as Hotspot (70, 23, 4, 56)
	
	'-----------------
	'On_Load event
	'-----------------   
	Event On_Load	
		IF (g_varLastRoom = ROOM_OLD_SHED) Then
		   Ego.Load(68, 32, DIR_WEST)
		End If
		
		IF (g_varLastRoom = ROOM_ROCKS) Then
		   Ego.Load(8, 28, DIR_EAST)
		End If				
	End Event 
	
	'-----------------
	'On_Hotspot1 event
	'-----------------   
	Event On_Hotspot1	
		Room.Load(ROOM_ROCKS)
	End Event	

	'-----------------
	'On_Hotspot2 event
	'-----------------   
	Event On_Hotspot2	
		Room.Load(ROOM_OLD_SHED)
	End Event		
	
End Room
