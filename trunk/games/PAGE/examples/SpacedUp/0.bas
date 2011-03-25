#INCLUDE "global.bi"
Room ROOM_INTRO
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("title.bmp")

	'-----------------
	'Room hotspot(s)
	'-----------------
	
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
		Say("    2011 Trodoss")
		'start playing the title song
		g_sngTitle.Play()
		
		'automatically load the first room
		Room.Load(ROOM_KITCHEN)			
	End Event 
	
End Room
