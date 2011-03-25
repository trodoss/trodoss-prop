'  PINTBasic Example #2 - Adding Hotspots, Inventory
'
'  Hotspot:
'    A Hotspot is a non-visible screen location that 
'    can either trigger an event, or can return whether
'    or not the Ego is within the hotspot boundaries.
'
'  Inventory:
'    The inventory contains items that the ego has
'    collected, and can be used in interacting with 
'    the other game elements (such as "GIVE"ing to 
'    someone, or "USE"ing on/with something).
'
'  Building on the code from the first example, the
'  hotspot ("hotRake") is defined:
'
'  Dim hotRake as Hotspot (10, 62, 10, 17)
'
'  This defines the hotspot at 10, 62, with a width of 
'  10 and a height of 17.  If you use the application
'  PINTRoomViewer.exe (in the /tools/bin directory), and
'  view the source of this room, you will be able to view
'  this hotspot.  It will be displayed on the screen as
'  an orange box in the left-hand side of the screen, in
'  a location below where the rake appears by the old shed.
'
'  The item ("g_itmRake") is defined in the 
'  "items.bi" include, and represents the item that can
'  be added/removed from the inventory entity.  Note that an
'  item can only have a 4 character descriptor in the PINT
'  interpreter.

' -- On_Take Event --
'  The On_Take event is triggered when the "TAKE" action is
'  selected.  
'
'  Event On_Take
'       If (Inventory.Contains(g_itmRake)) Then
'          Say ("nothing to take")
'       Else
'          If (hotRake.ContainsEgo()) Then
'               Say ("You take the rake")
'               Inventory.Add(g_itmRake)
'               picRake.Hide()
'          End If
'       End If
'  End Event
'
'  The first check in the event is to see if the inventory
'  already contains the item "RAKE" (g_itmRake).   If it
'  does, then a message is displayed.
'
'  If the item is not in inventory, the "ContainsEgo" method
'  of the hotspot ("hotRake") is checked to see if it contains
'  the ego object inside the boundaries of the hotspot.  If so
'  then a message is displayed, the item is added to the 
'  inventory, and the picture of the rake ("picRake") is hidden.
'