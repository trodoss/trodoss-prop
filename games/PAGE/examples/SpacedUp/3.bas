#INCLUDE "global.bi"
Room ROOM_HALLWAY_BRIDGE
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("hallway_to_bridge.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (4, 64, 4, 15)
	Dim hotspot2 as Hotspot (70, 64, 4, 15)
	Dim hotspot3 as Hotspot (32, 52, 10, 4)
	Dim hotspot4 as Hotspot (28, 76, 28, 3)
	
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
		If (g_varLastRoom = ROOM_HALLWAY_LEFT) Then
		   Ego.Load(10, 56, DIR_EAST)
		End If	

		If (g_varLastRoom = ROOM_HALLWAY_RIGHT) Then
		   Ego.Load(68, 56, DIR_WEST)
		End If	

		If (g_varLastRoom = ROOM_HALLWAY_BACK) Then
		   Ego.Load(34, 58, DIR_NORTH)
		End If			

		If (g_varLastRoom = ROOM_BRIDGE) Then
		   Ego.Load(34, 58, DIR_SOUTH)
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
		Room.Load(ROOM_HALLWAY_LEFT)
	End Event	

	'-----------------
	'On_Hotspot2 event
	'-----------------   
	Event On_Hotspot2	
		Room.Load(ROOM_HALLWAY_RIGHT)
	End Event	
	
	'-----------------
	'On_Hotspot3 event
	'-----------------   
	Event On_Hotspot3	
		Room.Load(ROOM_BRIDGE)
	End Event	

	'-----------------
	'On_Hotspot4 event
	'-----------------   
	Event On_Hotspot4	
		Room.Load(ROOM_HALLWAY_BACK)
	End Event	
End Room
