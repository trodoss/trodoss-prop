#INCLUDE "global.bi"
Room ROOM_END
	'-----------------
	'Room backdrop
	'-----------------
	Dim backBackdrop as Backdrop ("end.bmp")

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
		Say("the black hole ahead")
		Say("you have to wonder..")
		Say("what is beyond?")
		'start playing the title song
		g_sngTitle.Play()
		
		Say("..to be continued")
	End Event 
	
End Room
