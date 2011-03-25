#INCLUDE "global.bi"
Room ROOM_HALLWAY_LEFT
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("hallway_left.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (44, 62, 10, 4)
	Dim hotspot2 as Hotspot (70, 62, 4, 13)
	
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
		IF (g_varLastRoom = 0) Then
		   Ego.Load(46, 52, DIR_SOUTH)
		End If
		
		IF (g_varLastRoom = ROOM_KITCHEN) Then
		   Ego.Load(46, 52, DIR_SOUTH)
		End If
		
		IF (g_varLastRoom = ROOM_HALLWAY_BRIDGE) Then
		   Ego.Load(68, 50, DIR_WEST)
		End If
				
	End Event 

	'-----------------
	'On_Look event
	'-----------------   
	Event On_Look	
		Say ("You see a hallway")
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
		Room.Load(ROOM_KITCHEN)
	End Event	

	'-----------------
	'On_Hotspot2 event
	'-----------------   
	Event On_Hotspot2	
		Room.Load(ROOM_HALLWAY_BRIDGE)
	End Event	
End Room
