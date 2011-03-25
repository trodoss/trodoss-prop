'  PINTBasic Example #1 - Single Room
'
'  Before diving in too deeply into how the code
'  works, I thought I should offer a few definitions
'  and cover a few topics:
'
'  Room:
'    The container of both the visual elements and
'    the interactions (events) that can occur on 
'    a single screen.
'
'
'  Backdrop: 
'    Image (80 x 80 Bitmap) that represents the
'    area on the screen where the player (Ego)
'    can explore.
'
'    The PAGE interpreter uses the color black
'    to indicate a border (an area that the Ego
'    cannot move through).
'
'
'  Pic (Picture):
'    An image (8 x 16 Bitmap) that can represent
'    an item (or character) in the game.
'
'    PINTBasic accepts a 8 x 16 bitmap image, where
'    the color black is the "transparent" color.
'
'
'  Ego:
'    This represents the interactive player character
'    that can interact with elements in the room. 
'    The Pics that make up the ego are defined on the
'    'application' level (and not in the room).
'
'
'  * Note on PINTBasic Palette:
'
'    When a bitmap is being converted in the format
'    used by the PAGE interpreter, the colors mapped
'    to the Propeller palette are based on the first
'    16 colors that were the default palette in 
'    applications such as "Paint" in Windows 2000/XP/Vista.
'    If you are using another application/OS, you can
'    match the palette to the "palette.bmp" in this
'    directory.
'
'
'  * Why Are the Graphics Distorted (Streched Horizontally)
'    When it Runs on the PAGE Interpreter?
'
'    This has to do with the rendering of the display.
'    The 80 x 96 image is being streched across a 192 x 96 
'    display.  This is a 2.4:1 distortion.  Is this done
'    to make it look more retro?  Well, not really.  It is
'    to conserve resources, since there is only 32K to work
'    with on the Propeller.  Looking retro is a nice side
'    effect.  If you play some of the early Sierra Online 
'    games, you will see a similar sort of distortion on 
'    their early CGA/EGA games (first editions of Kings
'    Quest I, Space Quest I, etc.).
'
'    In order to get an idea of what the room will look like 
'    when it runs under the PAGE interpreter on the Propeller,
'    you can use the PINTRoomViewer.exe application (found in
'    the /tools/bin sub-directory).
'
'  
'  ---- Coding the Room ----
'    The first line of code in this file is an #INCLUDE 
'    pre-processor directive to include the "globabl.bi" (Basic
'    Include) file.  If you examine this file you will see a
'    number of global/application level definitions that are 
'    used when compiling the room.
'
'    The backdrop object "backBackdrop" is defined, as well as the
'    "picRake" pic object.  Note that the pic object is not 
'    immediately displayed on the screen, however the backdrop
'    will be displayed once the room is loaded.
'
'  -- On_Load Event --
'    The "On_Load" event is triggered when the room is loaded
'    into the PINT interpreter, so any code present in this 
'    method will be triggered immediately.
'
'  Event On_Load
'    'Draw the rake by the shed
'    picRake.Load(14, 53)
'
'     'Load the ego (player) on the screen
'     Ego.Load(36, 60, DIR_SOUTH)
'   End Event 
'
'    In this case, the pic object "picRake" is loaded (displayed)
'    at the pixel location (14, 53), and the intrinsic object Ego's
'    "Load" method is invoked to display the Ego in the room as well.
'    The constant "DIR_SOUTH" is used to indicate direction, which
'    is defined in the "global.bi" include file.
'
'    When the room is loaded you will see the rake pic and the ego
'    character displayed on the old shed backdrop, and the ego would
'    be able to move around the room.
'
'
'  -- On_Look Event --
'    The "On_Look" event is triggered when the "LOOK" action is
'    selected from the actions menu (that appears at the bottom 
'    of the PAGE display.
'
'  Event On_Look
'    Say ("You see an old shed")
'  End Event
'
'    The intrinsic "Say" command is used to display the message to 
'    the player.
'