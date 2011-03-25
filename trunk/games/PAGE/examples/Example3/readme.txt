'  PINTBasic Example #3 - Adding Rooms
'
'  In this example, additional rooms are
'  added to the game.
'
'  * Deja Vu?
'     One of the issues you can run into
'     when coding rooms is the situation
'     where an object has been "TAKE"en and
'     you return to the room, and, much to
'     your wonder and amazement, the item is
'     back!  Although that works out great for
'     an "infinite life" scheme... It doesn't
'     work well in an adventure game when you
'     only want an item to be taken/used once.
' 
'     In order to account for this (since there
'     will be multiple rooms in this example),
'     the variable "g_varGameProgress" is used
'     to store the current game progress.  This
'     global/application variable is defined in
'     "gamesetup.bi", along with the CONstants 
'     that are being used to track game progress.
'
'  * Variables ?!??
'     Yes, the PINT interpreter supports 8 "global"
'     variables, and 8 "local" (room) variables.
'     All the variables used in the PINT interpreter
'     are bytes.  There are some that are pre-defined,
'     and some, like "g_varGameProgress", that can be
'     user defined.
'
'     - The global/application variables will retain 
'     their values between rooms.
'
'     - The room variables only exist while the ego is
'     in a particular room.
'
'     - Any of the variables will persist if a LOAD/SAVE
'     action is used in a particular room.
'
'     If you look in the "global.bi" you will find a
'     section with the PINT (application) pre-defined
'     variables.
'
'     The following global variables are pre-defined
'     for use in the PINT interpreter:
' 
'     global 0 = max score
'     global 1 = current score
'     global 2 = last room 
'     global 3 = last item selected 
'
'     These are defined as follows in the "global.bi"
'     -----------------
'      Dim g_varMaxScore As Byte
'      Dim g_varCurrentScore As Byte
'      Dim g_varLastRoom As Byte
'      Dim g_varLastItem As Byte
'
'  ...Back to The Old Shed:
'    In the "On_Take" event, in the nested
'    If statement you will find the line:
'
'    g_varGameProgress = GAME_GOT_RAKE
'
'    This assigns the constant ("GAME_GOT_RAKE"
'    or "1") to the g_varGameProgress variable.
'
'    In the "On_Load" event, there has been an
'    If statement placed around the Load method
'    of "picRake".  If the global variable is set
'    (item taken), then the rake would not be 
'    displayed now when the item is taken and, for
'    example, the Ego enters the old shed.
'
'  The On_Hotspot[x] Events...
'    Much like other events, Hotspot events are triggered
'    when the Ego is within the boundary of the hotspot.
'    The following is important to remember about hotspot
'    events:
'    
'    - The first Hotspot defined in a room is Hotspot 1.
'
'    - Each additional hotspot increments.
'
'    - The On_Hotspot events are defined by the hotspot
'      "ordinal number," (Meaning 1,2,3,4) rather than 
'      their name reference (ex: "hotRake").
'    
'  Event On_Hotspot1
'       Room.Load(ROOM_INSIDE_SHED)
'  End Event
'
'    If you look in the "rooms.bi" you will see a list 
'    of constants used to define the rooms.  You will 
'    see these used in the "Room" declaration, where 
'    each room is defined.
'
'    The "Load" method of the Room object uses the room
'    number when loading the next room.  Once this happens,
'    The current room's code is terminated, and the new
'    room is loaded into the PINT interpreter.