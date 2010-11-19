{

Fantasy Fighter (RPG) Version 0.3
Author: trodoss - 2010

See end of file for terms of use.  
***************************************         
}                  

CON
'' Uncomment section for your configuration type:

  ' Proto/Demo Board setup
   _clkmode  = xtal1 + pll16x
   _xinfreq  = 5_000_000

   TV_BASE_PIN = 12
   KEYSPIN   = 26

{   
  ' Hydra setup
  _CLKMODE = xtal1 + pll8x
  _XINFREQ = 10_000_000 + 0000

   TV_BASE_PIN = 24 
   KEYSPIN  = 13
}

{
' Hybrid Boards
   _clkmode  = xtal1 + pll16x
   _xinfreq  = 6_000_000
   
   TV_BASE_PIN = 24    
   KEYSPIN  = 12
}

   'audio pin configuration
   AUDIO = 10

'' gamepad pin configuration (adjust for your setup)
  JOY_CLK = 16
  JOY_LCH = 17
  JOY_DATAOUT0 = 18
  JOY_DATAOUT1 = 19  
  
  ' NES bit encodings for NES gamepad 0
  NES0_RIGHT    = %00000000_00000001
  NES0_LEFT     = %00000000_00000010
  NES0_DOWN     = %00000000_00000100
  NES0_UP       = %00000000_00001000
  NES0_START    = %00000000_00010000
  NES0_SELECT   = %00000000_00100000
  NES0_B        = %00000000_01000000
  NES0_A        = %00000000_10000000  

  'color constants
  COLOR_BLACK  = $02
  COLOR_BROWN  = $5d
  COLOR_YELLOW = $6e
  COLOR_WHITE  = $07
  COLOR_GREEN  = $ab
  COLOR_LGREEN = $9e
  COLOR_RED    = $4b
  COLOR_PURPLE = $2b
  COLOR_CYAN   = $58
  COLOR_BLUE   = $fb
  COLOR_GREY   = $05

  'direction constants
  DIR_SOUTH = 1
  DIR_NORTH = 4
  DIR_WEST  = 7
  DIR_EAST  = 10

  'text constants
  TEXT_TITLE = 0
  TEXT_AUTHOR = 1
  TEXT_ELEMENT_1 = 2
  TEXT_ELEMENT_2 = 3
  TEXT_ELEMENT_3 = 4
  TEXT_ELEMENT_4 = 5
  TEXT_MONSTER_TYPE_1 = 6
  TEXT_MONSTER_TYPE_2 = 7
  TEXT_MONSTER_TYPE_3 = 8
  TEXT_MONSTER_TYPE_4 = 9
  TEXT_MONSTER_TYPE_5 = 10
  
  'Map/Window constants
  MAP_WIDTH  = 32
  MAP_HEIGHT = 32
  MAP_SIZE_BYTES = MAP_WIDTH * MAP_HEIGHT 
  
  WIN_WIDTH  = 16
  WIN_HEIGHT = 16

  'Pre-calculate the difference between map/window dimensions
  MAP_WIN_H_DIFF = MAP_HEIGHT - WIN_HEIGHT
  MAP_WIN_W_DIFF = MAP_WIDTH - WIN_WIDTH

  '(Set) Movement Event types
  EVENT_TYPE_EXIT = 0   'indicates an exit to a map location
  EVENT_TYPE_TALK = 1   'indicates a talk event
  EVENT_TYPE_SHOP = 2   'indicates a shop event
  EVENT_TYPE_HEAL = 3   'indicates a heal event
  EVENT_TYPE_MONS = 4   'indicates a monster event
  

  EVENT_LENGTH   = 4    'length of the event structure
  MAX_EVENTS     = 8    'maximum number of events (per map)

  MONSTER_LENGTH = 4    'length of the monster structure
  MAX_MONSTERS   = 3    'maximum number of monsters (per map)

  ELEMENT_1      = 0
  ELEMENT_2      = 1
  ELEMENT_3      = 2
  ELEMENT_4      = 3
  
  EVENT_X        = 0    'x position of the event
  EVENT_Y        = 1    'y position of the event
  EVENT_TYPE_EID = 2    'encoded type and event id
  EVENT_DATA     = 3    'data associated with the event

  MAX_ITEMS      = 8    'maximum number of items possible
  
  ITEM_TYPE_ARMOR  = 0
  ITEM_TYPE_WEAPON = 1
  
  'focus constants (UI handling)
  FOCUS_TITLE_SCR  = 0  'title screen (or restart)
  FOCUS_MAP_WIN    = 1  'map (main) window 
  FOCUS_COMBAT_WIN = 2  'combat window
  FOCUS_TEXT_WIN   = 3  'text window (generic)
  FOCUS_SHOP_WIN   = 4  'shop window

  'combat window states (UI/Window handling)
  STATE_PLAYER_ACT  = 0  'waiting for player action
  STATE_PLAYER_RES  = 1  'report player attack/action result
  STATE_MONSTER_RES = 2  'report monster attack result
  STATE_COMBAT_OVER = 3  'report combat result

  'music constants
  MUSIC_TITLE_SCR   = 0
  MUSIC_COMBAT      = 1
  MUSIC_VICTORY     = 2
  
VAR
  long seed  
  word Nes_Pad    

  byte map_x           'x position of the map
  byte map_y           'y position on the map
  
  byte player_dir      'the player direction
  byte player_step     'the step the player is on
  byte player_x        'x position
  byte player_y        'y position

  byte player_hp       'current hp for the player
  byte player_lvl      'current lvl for the player
  byte player_armor    'current armor for the player
  byte player_weapon   'current weapon for the player
  word player_gold     'current gold for the player
  word player_exp      'current experience for the player
  byte max_hp          'maximum HP for the level
  
  'buffer that stores the current map data                                  
  byte map_buffer[MAP_SIZE_BYTES]
  
  'buffer that stores the current event data
  byte event_buffer[EVENT_LENGTH * MAX_EVENTS]

  'buffer that stores the current monster data
  byte monster_buffer[MONSTER_LENGTH * MAX_MONSTERS]

  'buffer to store current text message
  byte text_buffer[16]
  
  byte event_count     'current number of events for the map
  byte has_monsters    'flag for whether or not the map has random monsters
  byte map_music       'current music id for the map music
  byte monster_count   'current number of monsters for the map

  'handles the current input focus
  byte focus

  'combat window
  byte combat_state  'handles the combat state
  byte combat_dmg    'temporary stores combat damage
  
  'handles the current shop level (for shop window)
  byte shop_lvl

  byte monster_type
  byte monster_element
  byte monster_hp
  byte monster_gold
  
  'menu/window select cursor
  byte select_x
  byte select_y
  
OBJ
  text  : "FantasyFighter_ORE_Text_001"  'the TV display driver
  key   : "comboKeyboard"
  music : "FantasyFighter_Music_002"  
  
PUB start 
    seed := cnt
    
    'start the tv text driver
    music.Start(AUDIO)
    text.start(TV_BASE_PIN) 
    key.start(KEYSPIN)

    InitGame

PRI InitGame
    'set the initial focus
    focus := FOCUS_TITLE_SCR

    'set up the starting player info
    player_hp    := 10
    player_lvl   := 1
    max_hp       := 10
    player_gold  := 10
    player_exp   := 0
    
    LoadMap(0,0)

    music.Play(MUSIC_TITLE_SCR)  
   
    'start the main handler 
    MainHandler

PRI MainHandler
    'set up the program loop
    repeat
      text.Cls

      'Draw the game screen
      DrawGameScreen
      
      'handle input from the user
      Nes_Pad := NES_Read_Gamepad

      case focus
         FOCUS_TITLE_SCR:
           HandleTextWindow
           
         FOCUS_MAP_WIN:
           HandleMapWindow
           
         FOCUS_COMBAT_WIN:
           HandleCombatWindow

         FOCUS_TEXT_WIN:
           HandleTextWindow

         FOCUS_SHOP_WIN:
           HandleShopWindow     

      
      'build in a delay for animation
      waitcnt(5_000_000 + cnt)

      'update the screen with changes
      text.UpdateScreen

PRI DrawGameScreen
   if focus == FOCUS_TITLE_SCR
      DrawTitleScreen
   else
      DrawMapWindow
      DrawStatusWindow
      DrawPlayer
    
      case focus
         FOCUS_COMBAT_WIN:
            DrawCombatWindow
       
         FOCUS_TEXT_WIN:
            DrawTextWindow

         FOCUS_SHOP_WIN:
            DrawShopWindow
            
PRI CheckPlayerMove (move_x, move_y) | blocked
    'assume we are blocked, until we have checked
    'each location the character will occupy
    blocked := 1
    
    'peek the location to see if it is less than($6A), start of blocking
    if (text.PeekChar(move_x - map_x, move_y - map_y-1) < $6A)
       if (text.PeekChar(move_x - map_x, move_y - map_y) < $6A)
          'if both locations were space, then the character
          'would not be blocked
          blocked := 0
          
    return blocked
                 
PRI PlayerMove | event_ptr, type_id, event_id, monster_id
    'advance the player step, and reset when necessary
    player_step += 1
    if (player_step > 2)
       player_step := 1

    'check the events to see if an event has been triggered
    event_ptr := @event_buffer
    repeat event_count
       if (byte[event_ptr][EVENT_X] == player_x) AND (byte[event_ptr][EVENT_Y] == player_y)
          'event has been triggered
           type_id := byte[event_ptr][EVENT_TYPE_EID] >> 4
           event_id := byte[event_ptr][EVENT_TYPE_EID] & %00001111

           'based on the type, execute the action
           case type_id
              EVENT_TYPE_EXIT:
                 LoadMap(byte[event_ptr][EVENT_DATA], event_id)
                 
              EVENT_TYPE_TALK:   
                 ShowTextWindow(GetTextDataPointer(byte[event_ptr][EVENT_DATA]))

              EVENT_TYPE_SHOP:
                 ShowShopWindow(byte[event_ptr][EVENT_DATA])

              EVENT_TYPE_HEAL:
                 ShowTextWindow(string("YOU ARE HEALED"))
                 player_hp := max_hp

              EVENT_TYPE_MONS:
                 ShowCombatWindow(byte[event_ptr][EVENT_DATA])    
                 
           quit
           
       event_ptr+= EVENT_LENGTH

    'check for random monster event (if it is possible)
    if monster_count > 0
       '1/16 chance that a monster is encountered
       if rand &15 == 15
         'pick random monster (0 or 1)
         monster_id := rand &1
         music.Play(MUSIC_COMBAT)
         ShowCombatWindow(monster_id)
                        
PRI DrawPlayer | offset
    offset := player_dir + player_step
    text.DrawChar(player_x-map_x, player_y-map_y-1, COLOR_BROWN,COLOR_BLUE, player_dir)
    text.DrawChar(player_x-map_x, player_y-map_y ,COLOR_BROWN,COLOR_RED, offset)                   

' //////////////////////////////////////////////////////////////////
' Window Handling Routines
' //////////////////////////////////////////////////////////////////
PRI DrawTitleScreen
    text.drawTextAt(3, 9, GetTextDataPointer(TEXT_TITLE))
    text.drawTextAt(5, 10, GetTextDataPointer(TEXT_AUTHOR))

    text.drawTextAt(7, 15, string("START"))

PRI HandleTitleScreen
    if (Nes_Pad & NES0_START) OR (Nes_Pad & NES0_A)
       music.Play(map_music) 
       ReturnFocusToMapWindow

PRI ShowTextWindow (message)
    bytefill(@text_buffer,0,16)
    bytemove(@text_buffer, message, 16)
    focus := FOCUS_TEXT_WIN
     
PRI DrawTextWindow
    text.DrawBox(0, 8, 18, 3)
    text.drawTextAt(1, 9,@text_buffer)
           
PRI HandleTextWindow
    if (Nes_Pad & NES0_START) OR (Nes_Pad & NES0_A)
       ReturnFocusToMapWindow

PRI ReturnFocusToMapWindow
    select_x := 0
    select_y := 0
    focus := FOCUS_MAP_WIN
    
PRI DrawMapWindow | ptr, x, y, line_remainder
    ptr := @map_buffer
    'add to the offset based on current map_y, to position correctly in the data
    ptr += map_y * MAP_WIDTH

    'pre-calulate the remaider of the width to where the window is in the data 
    line_remainder := MAP_WIDTH - (map_x + WIN_WIDTH)  
    
    repeat y from 1 to WIN_HEIGHT
       'add in the map offset, to start at the right point in the data
       ptr += map_x
       repeat x from 1 to WIN_WIDTH
          'set the colors on the map
          case byte[ptr]
             $00..$65:
                text.PokeChar(x,y,COLOR_WHITE,COLOR_GREEN,COLOR_LGREEN, byte[ptr])
             $6C:
                text.PokeChar(x,y,COLOR_WHITE,COLOR_GREY,COLOR_LGREEN, byte[ptr])
             $66..$6B, $6D..$71:
                text.PokeChar(x,y,COLOR_WHITE,COLOR_BROWN, COLOR_GREY, byte[ptr])
             $72..$7F:
                text.PokeChar(x,y,COLOR_WHITE,COLOR_BROWN, COLOR_BLUE, byte[ptr])
          ptr++
       'add in the line remainder   
       ptr += line_remainder

PRI HandleMapWindow
     'gamepad has been moved down (south)               
     if (Nes_Pad & NES0_DOWN)
        player_dir := DIR_SOUTH

        'check to make sure the space the character is moving into
        'is blocked
        if (CheckPlayerMove(player_x,player_y+1) == 0)     
           player_y++
           'if (player_y > 7) AND (player_y < 23)
           if (player_y - map_y) > 7
              map_y++
              'Prevent scrolling past the end of the map data;
              'When the map_y is greater than the difference between the
              'window height and map data (indicating you have scrolled as
              'far as you can, without leaving the map boundary) do not go
              'any further
              if (map_y > MAP_WIN_H_DIFF)
                  map_y := MAP_WIN_H_DIFF
           if (player_y > MAP_HEIGHT)
              player_y := MAP_HEIGHT        
           PlayerMove
                
     'gamepad has been moved up (north)                
     if (Nes_Pad & NES0_UP)
        player_dir := DIR_NORTH 
        if (CheckPlayerMove(player_x,player_y-1) == 0)
           if (player_y > 2)
              player_y--
              'if (player_y > 7) AND (player_y < 23)
              if (player_y - map_y) < 9
                'Prevent scrolling past the start of map data
                if (map_y > 0)
                   map_y--
           PlayerMove
                
     'gamepad has been moved left (west)
     'Prevent scrolling past the start of map data           
     if (Nes_Pad & NES0_LEFT)
        player_dir := DIR_WEST
        if (CheckPlayerMove(player_x-1,player_y) == 0)
           if (player_x > 1)
              player_x--
              'if (player_x > 7) AND (player_x < 23)
              if (player_x - map_x) < 9 
                 if (map_x > 0)
                    map_x--
           PlayerMove
                
     'gamepad has been moved right (east)                        
     if (Nes_Pad & NES0_RIGHT)
        player_dir := DIR_EAST
        if (CheckPlayerMove(player_x+1,player_y) == 0)
           player_x++
           'if (player_x > 7) AND (player_x < 23)
           if (player_x - map_x) > 7 
              map_x++     
              if (map_x > MAP_WIN_W_DIFF)
                  map_x := MAP_WIN_W_DIFF
           if (player_x > MAP_WIDTH)
              player_x := MAP_WIDTH      
           PlayerMove

PRI DrawStatusWindow
    text.DrawBox(0, 17, 18, 6)
    text.drawTextAt(1, 18,string("H:"))
    text.drawNumAt(3,18, player_hp)
    text.drawTextAt(3, 19,string("/"))
    text.drawNumAt(5,19, max_hp)
    
    text.drawTextAt(11, 19,string("G:"))
    text.drawNumAt(13,19, player_gold)
    text.drawTextAt(11, 18,string("L:"))
    text.drawNumAt(13,18, player_lvl)

    text.pokeChar(1, 21, COLOR_WHITE, COLOR_GREY, COLOR_BLACK, 14)
    text.pokeChar(11, 21, COLOR_WHITE, COLOR_GREY, COLOR_BLACK, 13)
    text.drawTextAt(2, 21, GetTextDataPointer(TEXT_ELEMENT_1 + player_weapon))
    text.drawTextAt(12,21, GetTextDataPointer(TEXT_ELEMENT_1 + player_armor))
    
'    text.drawNumAt(1, 21, player_x)
'    text.drawNumAt(11,21, player_y)
         
PRI DrawShopWindow  | item_ptr, item_lvl, item_type, element, cost, line_index
    text.DrawBox(1, 6, 13, 6)
    item_ptr := @iteminfo

    line_index := 7
    repeat MAX_ITEMS
      item_lvl  := byte[item_ptr] >> 4
      item_type := byte[item_ptr] & %00001111
      item_ptr++
      element   := byte[item_ptr]
      item_ptr++
      cost      := word[item_ptr]
      item_ptr += 2

      if item_lvl == shop_lvl
         text.pokeChar(3, line_index, COLOR_WHITE, COLOR_GREY, COLOR_BLACK, 13 + item_type)
         text.drawTextAt(4, line_index, GetTextDataPointer(TEXT_ELEMENT_1 + element))
         text.drawNumAt(9, line_index, cost)
         line_index++

    text.pokeChar(select_x, select_y, COLOR_WHITE, COLOR_GREY, COLOR_BLACK, 127)
          
PRI ShowShopWindow (level)
    shop_lvl := level
    focus := FOCUS_SHOP_WIN
    select_x := 2
    select_y := 7

PRI HandleShopWindow | selected, item_ptr, item_type, element, cost
    if (Nes_Pad & NES0_SELECT)
       ReturnFocusToMapWindow

    if (Nes_Pad & NES0_DOWN)
       if select_y < 10
          select_y++

    if (Nes_Pad & NES0_UP)
       if select_y > 7
          select_y--
          
    if (Nes_Pad & NES0_A) OR (Nes_Pad & NES0_START)
       selected := select_y - 7
       'find the selected item in the data
       item_ptr := @iteminfo + (selected * 4) + (shop_lvl * 16)
       item_type := byte[item_ptr] & %00001111
       item_ptr++
       element   := byte[item_ptr]
       item_ptr++
       cost      := word[item_ptr]
       if cost > player_gold
          ShowTextWindow(string("NOT ENOUGH G"))
       else
          'otherwise, take the cost out of player gold, and assign
          'the purchased item
          player_gold -= cost
          if item_type == ITEM_TYPE_ARMOR
             player_armor := element
          else
             player_weapon := element
             
          ReturnFocusToMapWindow

PRI DrawCombatWindow | mon_start_char, mon_color_1, mon_color_2
    text.DrawBox(2, 2, 14, 7)
    text.DrawBox(2,10,14,4)
    'draw monster
    mon_start_char := 33
    if monster_type > 2
       mon_start_char := 10
    mon_start_char += (monster_type * 4)
    
    case monster_element
       ELEMENT_1:
          mon_color_1 := COLOR_RED
          mon_color_2 := COLOR_GREY
       ELEMENT_2:
          mon_color_1 := COLOR_YELLOW
          mon_color_2 := COLOR_CYAN
       ELEMENT_3:
          mon_color_1 := COLOR_CYAN
          mon_color_2 := COLOR_BLUE
       ELEMENT_4:
          mon_color_1 := COLOR_YELLOW
          mon_color_2 := COLOR_RED

    text.pokeChar(7, 6, COLOR_WHITE, mon_color_1, mon_color_2, mon_start_char)
    text.pokeChar(8, 6, COLOR_WHITE, mon_color_1, mon_color_2, mon_start_char + 1)
    text.pokeChar(7, 7, COLOR_WHITE, mon_color_1, mon_color_2, mon_start_char + 2)
    text.pokeChar(8, 7, COLOR_WHITE, mon_color_1, mon_color_2, mon_start_char + 3)   

    text.drawTextAt(4, 4, GetTextDataPointer(TEXT_ELEMENT_1 + monster_element))
    text.drawTextAt(9, 4, GetTextDataPointer(TEXT_MONSTER_TYPE_1 + monster_type))

    case combat_state
      STATE_PLAYER_ACT:
         text.drawTextAt(4, 11, string("FIGHT"))
         text.drawTextAt(4, 12, string("RUN"))
         text.pokeChar(select_x, select_y, COLOR_WHITE, COLOR_GREY, COLOR_BLACK, 127)
         
      STATE_PLAYER_RES:
         text.drawTextAt(3, 11, string("HERO"))
         if combat_dmg > 0
            text.drawNumAt(4, 12, combat_dmg)
            text.drawTextAt(8, 12, string("DMG"))
         else
            text.drawTextAt(4, 12, string("MISS"))
            
      STATE_MONSTER_RES:
         text.drawTextAt(3, 11, GetTextDataPointer(TEXT_ELEMENT_1 + monster_element))
         text.drawTextAt(8, 11, GetTextDataPointer(TEXT_MONSTER_TYPE_1 + monster_type))

         if combat_dmg > 0
            text.drawNumAt(4, 12, combat_dmg)
            text.drawTextAt(8, 12, string("DMG"))
         else
            text.drawTextAt(4, 12, string("MISS"))

      STATE_COMBAT_OVER:
         if combat_dmg <> 9
            text.drawTextAt(3, 11, string("VICTORY"))
            if combat_dmg > 0
               text.drawTextAt(3,12, string ("GAIN L"))
               text.drawNumAt(11,12, player_lvl)
            else
               text.drawTextAt(3,12, string ("GAIN G"))
               text.drawNumAt(11,12, monster_gold)
         else
            text.drawTextAt(3, 11, string("DEFEAT"))
            
PRI ShowCombatWindow(mon_id) | dat_ptr
   dat_ptr := @monster_buffer + (MONSTER_LENGTH * mon_id)
   
   monster_type := byte[dat_ptr] >> 4
   monster_element := byte[dat_ptr] & %00001111
   dat_ptr++
   monster_hp := byte[dat_ptr]
   dat_ptr++
   monster_gold := word[dat_ptr]
   
   focus := FOCUS_COMBAT_WIN
   combat_state := STATE_PLAYER_ACT
   select_x := 3
   select_y := 11

PRI HandleCombatWindow | attack, next_lvl_exp
    case combat_state
       STATE_PLAYER_ACT:
          if (Nes_Pad & NES0_DOWN)
             select_y := 12

          if (Nes_Pad & NES0_UP)
             select_y := 11
             
          if (Nes_Pad & NES0_A) OR (Nes_Pad & NES0_START)
             if select_y == 12
                ShowTextWindow(string("RAN AWAY"))
                music.Play(map_music)
             else
                
                'determine if the hero hit or not
                combat_dmg := 0
                
                'start off with the assumption that we will
                'go onto the player result
                combat_state := STATE_PLAYER_RES
                
                attack := player_weapon + (rand &9)
                if attack >  (monster_element + 3)
                   'determine combat damage
                   combat_dmg := attack + player_lvl
                   if monster_hp =< combat_dmg
                   'monster has been defeated:

                      music.Play(MUSIC_VICTORY) 
                      '*** check for level up, etc.***
                      player_gold += monster_gold
                      player_exp += monster_gold
                      next_lvl_exp := player_lvl * (player_lvl - 1) * 5
                      'we re-use this as a flag of whether or not the
                      'player leveled up
                      combat_dmg := 0
                      if player_exp => next_lvl_exp
                         combat_dmg := 1
                         player_lvl++
                         max_hp := player_lvl * 10
                      combat_state := STATE_COMBAT_OVER
                   else
                      monster_hp -= combat_dmg
                      
            
       STATE_PLAYER_RES:
          if (Nes_Pad & NES0_START) OR (Nes_Pad & NES0_A)
             'determine if the monster hit or not
              combat_dmg := 0

              'start off with the assumption that we will
              'go onto the monster result
              combat_state := STATE_MONSTER_RES
                          
              attack := monster_element + (rand &9)
              if attack > (player_armor + 3)
                 combat_dmg := attack + monster_element
                 if player_hp =< combat_dmg
                    'player has been defeated

                    combat_state := STATE_COMBAT_OVER
                    'indicate that player has been defeated
                    combat_dmg := 9 
                 else
                     player_hp -= combat_dmg             
                     
          
       STATE_MONSTER_RES:
          if (Nes_Pad & NES0_START) OR (Nes_Pad & NES0_A)
             combat_state := STATE_PLAYER_ACT
          
       STATE_COMBAT_OVER:
          if (Nes_Pad & NES0_START) OR (Nes_Pad & NES0_A)
              if combat_dmg <> 9
                 music.Play(map_music)
                 ReturnFocusToMapWindow
              else
                 'otherwise we now need to reset the game
                 InitGame
              
' //////////////////////////////////////////////////////////////////
' Random Number Generation Routine
' //////////////////////////////////////////////////////////////////
PUB rand
    seed := seed * 1103515245 + 12345 + CNT / 7777
    return seed

' //////////////////////////////////////////////////////////////////
' Map Loading Routine
' //////////////////////////////////////////////////////////////////
PRI LoadMap(MapID, EventID) | event_ptr
   LoadMapData(MapID)
   LoadEventData(MapID)
   event_ptr := @event_buffer
   event_ptr += (EventID * EVENT_LENGTH)

   'set up the initial character location/facing
   player_dir  := DIR_SOUTH
   player_step := 1
   
   player_x    := byte[event_ptr][EVENT_X]
   player_y    := byte[event_ptr][EVENT_Y]

   map_x := 0
   map_y := 0
   
   'calculate mapx, mapy from the info
   if (player_x > 7)
       map_x  := player_x - 7
       if (map_x > 16)
          map_x := 16
          
   if (player_y > 7)
       map_y  := player_y - 7
       if (map_y > 16)
          map_y := 16   

   'start map music
   music.Play(map_music)
   
PRI LoadMapData(MapID) | dat_ptr, buf_ptr, index_ptr, data, count, len
   
   dat_ptr := @mapinfo
   'if this is not the first map we will have to advance the pointer
   if MapID > 0     
      index_ptr := @mapindex
      'set the map pointer advanced to the start of the next map
      dat_ptr += long[index_ptr][MapID-1]
      
   buf_ptr := @map_buffer
   repeat
      'if the data is greater than 128, it indicates that it is an encoded "run."  
      if byte[dat_ptr] => 128
         '128 is subtracted, leaving the actual character data
         data := byte[dat_ptr] - 128
         dat_ptr++
         
         'the next byte indicates the "run length" (or number of times this repeats)
         len :=  byte[dat_ptr]
         dat_ptr++
         
         'fill the map buffer with the data for the number of bytes specified
         bytefill(buf_ptr, data, len)
         buf_ptr += len
         count += len
      else
         'otherwise, the map data can be copied directly
         byte[buf_ptr] := byte[dat_ptr]
         dat_ptr++
         buf_ptr++
         count++

      'when we have written a number of bytes to fill the map buffer, quit   
      if count => MAP_SIZE_BYTES  
         quit

PRI LoadEventData(MapID) | dat_ptr, index_ptr
   dat_ptr := @eventinfo
   'if this is not the first map we will have to advance the pointer
   if MapID > 0     
      index_ptr := @eventindex
      'set the map pointer advanced to the start of the next map
      dat_ptr += long[index_ptr][MapID-1]
      
   map_music := byte[dat_ptr]
   dat_ptr++

   has_monsters := byte[dat_ptr]
   dat_ptr++

   event_count := byte[dat_ptr]
   dat_ptr++

   monster_count := byte[dat_ptr]
   dat_ptr++   

   'load event data
   bytemove (@event_buffer, dat_ptr, event_count * EVENT_LENGTH)
   dat_ptr += event_count * EVENT_LENGTH

   'load monster data (if present)
   if monster_count > 0
      bytemove (@monster_buffer, dat_ptr, monster_count * MONSTER_LENGTH)
      

PRI GetTextDataPointer(TextID) | dat_ptr, index_ptr
   dat_ptr := @textinfo
   if TextID > 0
      index_ptr := @textindex
      dat_ptr += long[index_ptr][TextID-1]
      
   return dat_ptr                 
PRI NES_Read_Gamepad : nes_bits        |  i
' //////////////////////////////////////////////////////////////////
' NES Game Paddle Read
' //////////////////////////////////////////////////////////////////       
' reads both gamepads in parallel encodes 8-bits for each in format
' right game pad #1 [15..8] : left game pad #0 [7..0]
'
' set I/O ports to proper direction
' P3 = JOY_CLK      (4)
' P4 = JOY_SH/LDn   (5) 
' P5 = JOY_DATAOUT0 (6)
' P6 = JOY_DATAOUT1 (7)
' NES Bit Encoding
'
' RIGHT  = %00000001
' LEFT   = %00000010
' DOWN   = %00000100
' UP     = %00001000
' START  = %00010000
' SELECT = %00100000
' B      = %01000000
' A      = %10000000

' step 1: set I/Os
DIRA [JOY_CLK] := 1 ' output
DIRA [JOY_LCH] := 1 ' output
DIRA [JOY_DATAOUT0] := 0 ' input
DIRA [JOY_DATAOUT1] := 0 ' input

' step 2: set clock and latch to 0
OUTA [JOY_CLK] := 0 ' JOY_CLK = 0
OUTA [JOY_LCH] := 0 ' JOY_SH/LDn = 0
'Delay(1)

' step 3: set latch to 1
OUTA [JOY_LCH] := 1 ' JOY_SH/LDn = 1
'Delay(1)                            

' step 4: set latch to 0
OUTA [JOY_LCH] := 0 ' JOY_SH/LDn = 0

' data is now ready to shift out, clear storage
nes_bits := 0

' step 5: read 8 bits, 1st bits are already latched and ready, simply save and clock remaining bits
repeat i from 0 to 7

 nes_bits := (nes_bits << 1)
 nes_bits := nes_bits | INA[JOY_DATAOUT0] | (INA[JOY_DATAOUT1] << 8)

 OUTA [JOY_CLK] := 1 ' JOY_CLK = 1
 'Delay(1)             
 OUTA [JOY_CLK] := 0 ' JOY_CLK = 0
 
 'Delay(1)             
' invert bits to make positive logic
nes_bits := (!nes_bits & $FFFF)

if nes_bits&$ff==$ff
    nes_bits:=nes_bits&$ff00
        
if nes_bits&$ff00==$ff00
    nes_bits:=nes_bits&$ff

' Keyboard (mapped onto NES buttons)
    if(key.keystate($C2))       'Up Arrow
      nes_bits|=NES0_UP
    if(key.keystate($C3))       'Down Arrow
      nes_bits|=NES0_DOWN
    if(key.keystate($C0))       'Left Arrow
      nes_bits|=NES0_LEFT
    if(key.keystate($C1))       'Right Arrow
      nes_bits|=NES0_RIGHT
    if(key.keystate($0D))       'Enter
      nes_bits|=NES0_START
    if(key.keystate(" "))       'Space
      nes_bits|=NES0_B

' //////////////////////////////////////////////////////////////////
' End NES Game Paddle Read
' //////////////////////////////////////////////////////////////////             
DAT

textindex        file "ffdata.ssx"

eventindex       file "ffdata.eix"

mapindex         file "ffdata.mix"

textinfo         file "ffdata.sst"

iteminfo         file "ffdata.itm"

eventinfo        file "ffdata.evt"

mapinfo          file "ffdata.map"
                 byte 0, 0, 0           'long alignment adjustment
{{
+------------------------------------------------------------------------------------------------------------------------------+
                                                   TERMS OF USE: MIT License                                                                                                              
+------------------------------------------------------------------------------------------------------------------------------
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation     
files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,    
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
is furnished to do so, subject to the following conditions:                                                                   
                                                                                                                              
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
                                                                                                                              
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE          
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR         
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,   
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                         
+------------------------------------------------------------------------------------------------------------------------------+
}}                                     