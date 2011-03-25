#INCLUDE "global.bi"
Room ROOM_HALLWAY_RIGHT
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("hallway_right.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (4, 60, 4, 12)

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
		   Ego.Load(10, 56, DIR_EAST)
		End If	
		
		Say ("Did I see something?")		
	End Event 

	'-----------------
	'On_Look event
	'-----------------   
	Event On_Look	
		Say ("You see peering eyes")
	End Event	

	'-----------------
	'On_Take event
	'-----------------   
	Event On_Take	
		Say ("nothing to take here")
	End Event		

	'-----------------
	'On_Give event
	'-----------------   
	Event On_Give	
		If (g_varGameProgress > GAME_GOT_FOODBOX) Then
			Say ("they don't want it")
		Else
			If (g_varLastItem = g_itmFood) Then
				Say ("space bunnies eat it")
				Inventory.Remove(g_itmFood)
				g_varCurrentScore = g_varCurrentScore + 5
				g_varGameProgress = GAME_FED_BUNNY
				g_sngScore.Play()
			Else
				Say ("they don't want it")
			End If
		End If
	End Event	
	
	'-----------------
	'On_Use event
	'-----------------   
	Event On_Use	
		Say ("can't use this here")
	End Event	
	
	'-----------------
	'On_Hotspot1 event
	'-----------------   
	Event On_Hotspot1	
		Room.Load(ROOM_HALLWAY_BRIDGE)
	End Event	

End Room
