#INCLUDE "global.bi"
Room ROOM_ENGINE_ROOM_2
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("engine_room_2.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (70, 68, 4, 11)
	
	'Angry bunny hotspot
	Dim hotspot2 as Hotspot (16, 52, 22, 4)
	
	Dim hotDisk as Hotspot(34, 30, 10, 10)
	
	'-----------------
	'Room pic(s)
	'-----------------
	Dim picBunny as Pic ("bunny.bmp")
	Dim picDisk as Pic ("disk.bmp")
	
	'-----------------
	'Local Variable(s)
	'-----------------

	'-----------------
	'On_Load event
	'-----------------   
	Event On_Load			
		If (g_varLastRoom = ROOM_ENGINE_ROOM_1) Then
		   Ego.Load(64, 56, DIR_WEST)
		End If	

	    'show the bunny if you have not fed the others
	    If (g_varGameProgress < GAME_FED_BUNNY) Then
		   picBunny.Load(30, 28)
		End If	
		
	    'if the disk has not been taken yet, show it
	    If (g_varGameProgress < GAME_GOT_DISK) Then
		   picDisk.Load(40, 18)
		End If			
		
	End Event 

	'-----------------
	'On_Look event
	'-----------------   
	Event On_Look
		If (g_varGameProgress < GAME_FED_BUNNY) Then	
			Say ("You see a bunny thug")
		Else
			If (g_varGameProgress < GAME_GOT_DISK) Then
				Say ("You see a disk")
			Else
				Say ("You hear the engine")
			End If
		End If		
	End Event	

	'-----------------
	'On_Take event
	'-----------------   
	Event On_Take	
		If (Inventory.Contains(g_itmDisk)) Then
			Say ("nothing to take")
		Else
			If (hotDisk.ContainsEgo()) Then
				Say ("You take the disk")
				g_sngScore.Play()
				g_varCurrentScore = g_varCurrentScore + 10
				g_varGameProgress = GAME_GOT_DISK
				Inventory.Add(g_itmDisk)
				picDisk.Hide()			
			End If
		End If
		
	End Event		
	
	'-----------------
	'On_Hotspot1 event
	'-----------------   
	Event On_Hotspot1	
		Room.Load(ROOM_ENGINE_ROOM_1)
	End Event	
	
	'-----------------
	'On_Hotspot2 event
	'-----------------   
	Event On_Hotspot2	
		If (g_varGameProgress < GAME_FED_BUNNY) Then
			Say ("bunny thug attacks!")
			Say ("you black out...")
			Room.Load(ROOM_ENGINE_ROOM_1)
		End If
	End Event		
	
End Room
