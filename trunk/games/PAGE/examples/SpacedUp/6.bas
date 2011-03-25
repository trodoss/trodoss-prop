#INCLUDE "global.bi"
Room ROOM_ENGINE_ROOM_1
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("engine_room_1.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (26, 36, 10, 4)
	Dim hotspot2 as Hotspot (0, 67, 4, 12)
	
	'-----------------
	'Room pic(s)
	'-----------------

	'-----------------
	'Local Variable(s)
	'-----------------

	'-----------------
	'On_Load event
	'-----------------   
	Event On_Load			
		If (g_varLastRoom = ROOM_HALLWAY_BACK) Then
		   Ego.Load(30, 26, DIR_SOUTH)
		End If	

		If (g_varLastRoom = ROOM_ENGINE_ROOM_2) Then
		   Ego.Load(8, 56, DIR_EAST)
		End If			
	End Event 

	'-----------------
	'On_Look event
	'-----------------   
	Event On_Look	
		Say ("You hear the engine")
	End Event	

	'-----------------
	'On_Take event
	'-----------------   
	Event On_Take	
		Say ("nothing to take here")
	End Event		
	
	'-----------------
	'On_Hotspot1 event
	'-----------------   
	Event On_Hotspot1	
		Room.Load(ROOM_HALLWAY_BACK)
	End Event	

	'-----------------
	'On_Hotspot2 event
	'-----------------   
	Event On_Hotspot2	
		Room.Load(ROOM_ENGINE_ROOM_2)
	End Event		
End Room
