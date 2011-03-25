#INCLUDE "global.bi"
Room ROOM_HALLWAY_BACK
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("hallway_back.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (20, 56, 37, 4)
	Dim hotspot2 as Hotspot (14, 76, 42, 3)
	
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
		If (g_varLastRoom = ROOM_HALLWAY_BRIDGE) Then
		   Ego.Load(34, 46, DIR_SOUTH)
		End If	

		If (g_varLastRoom = ROOM_ENGINE_ROOM_1) Then
		   Ego.Load(34, 58, DIR_NORTH)
		End If			
	End Event 

	'-----------------
	'On_Look event
	'-----------------   
	Event On_Look	
		Say ("You see an elevator")
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
		Room.Load(ROOM_HALLWAY_BRIDGE)
	End Event	

	'-----------------
	'On_Hotspot2 event
	'-----------------   
	Event On_Hotspot2	
		Room.Load(ROOM_ENGINE_ROOM_1)
	End Event		
End Room
