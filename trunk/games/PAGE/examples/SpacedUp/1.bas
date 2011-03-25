#INCLUDE "global.bi"
Room ROOM_KITCHEN
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("kitchen.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (28, 76, 24, 3)
	Dim hotFood as Hotspot (28, 46, 10, 8)
	
	'-----------------
	'Room pic(s)
	'-----------------
	Dim picFoodBox as Pic ("foodbox.bmp")

	'-----------------
	'Local Variable(s)
	'-----------------
	Dim varTest as Byte

	'-----------------
	'On_Load event
	'-----------------   
	Event On_Load	
	    'hide the food, if it has not been taken yet
	    If (g_varGameProgress = GAME_START) Then
		   picFoodBox.Load(32, 28)
		End If
		
		IF (g_varLastRoom = 0) Then
		   Ego.Load(32, 56, DIR_SOUTH)
		End If

		IF (g_varLastRoom = ROOM_HALLWAY_LEFT) Then
		   Ego.Load(32, 56, DIR_NORTH)
		End If
		
	End Event 
	
	'-----------------
	'On_Take event
	'-----------------   
	Event On_Take
		If (g_varGameProgress > GAME_START) Then
			Say ("nothing to take")
		Else
			If (hotFood.ContainsEgo()) Then
				Say ("You take the food")
				g_sngScore.Play()
				g_varCurrentScore = g_varCurrentScore + 10
				g_varGameProgress = GAME_GOT_FOODBOX
				Inventory.Add(g_itmFood)
				picFoodBox.Hide()
			End If
		End If	
	End Event

	'-----------------
	'On_Look event
	'-----------------   
	Event On_Look	
		Say ("You see the kitchen")
		
		If (g_varGameProgress > GAME_START) Then
			Say ("the shelves are bare")
		Else
		    Say ("you see a food box")
		End If
		
	End Event	

	'-----------------
	'On_Hotspot1 event
	'-----------------   
	Event On_Hotspot1	
		Room.Load(ROOM_HALLWAY_LEFT)
	End Event	

End Room
