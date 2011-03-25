#INCLUDE "global.bi"
Room ROOM_BRIDGE
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("bridge.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	Dim hotspot1 as Hotspot (28, 76, 18, 3)
	Dim hotConsole as Hotspot(32, 40, 10, 8)
	
	'-----------------
	'Room pic(s)
	'-----------------
	Dim picPow as Pic ("pow.bmp")

	'-----------------
	'Local Variable(s)
	'-----------------

	'-----------------
	'On_Load event
	'-----------------   
	Event On_Load				
		IF (g_varLastRoom = ROOM_HALLWAY_BRIDGE) Then
		   Ego.Load(32, 56, DIR_NORTH)
		End If	
		
		Say ("So.. why the sirens?")
		Say ("Oh, that's why!")	
	End Event 

	'-----------------
	'On_Look event
	'-----------------   
	Event On_Look	
		Say ("You see a black hole")
	End Event	

	'-----------------
	'On_Take event
	'-----------------   
	Event On_Take	
		Say ("nothing to take here")
	End Event		
	
	'-----------------
	'On_Use event
	'-----------------   
	Event On_Use
		If (g_varLastItem = g_itmDisk) Then	
			If (hotConsole.ContainsEgo()) Then
				Say ("Insert into drive:a")
				picPow.Load(46,8)
				Say ("Uh oh!")
				picPow.Hide()
				Say ("Smoke is a bad sign!")
				Say ("Where's that manual?")
				Room.Load(ROOM_END)
			Else
				Say ("Not close enough")
			End If
		End If
	End Event		
	
	'-----------------
	'On_Hotspot1 event
	'-----------------   
	Event On_Hotspot1	
		Room.Load(ROOM_HALLWAY_BRIDGE)
	End Event	
	
End Room
