{{
PAGE interpereter 

2011 - trodoss, roadster, JTCook

   Special Thanks to:
    roadster, JTCook, Oldbitcollector, Cardboardguru, Potatohead, Ariba, Baggers, lft,
    Unsoundcode, Chris Savage (and SavageCircuits.com)

Version History:
- 0.1 -  trodoss: initial release
- 0.2 - roadster: added configuration options
         trodoss: added item/inventory handling
- 0.3 -  trodoss: integration of gm_synth
        roadster: integration of MIGS code (input)
- 0.4 -  trodoss: UI improvements
- 0.5 -   JTCook: input fixes, graphics handling fixes
         trodoss: added load/save functionality


See the bottom of the code for terms of use.
}}
CON

'** uncomment the section below that corresponds to your configuration **

'** additional configuration is necessary in the OBJ section, selecting
'   the input device.

'Proto / Demo board
  _clkmode = xtal1 + pll16x
  _xinfreq = 5_000_000

  spiDO     = 0
  spiClk    = 1
  spiDI     = 2
  spiCS     = 3

  Keybd     = 26    
  Tv_base   = 12
  Sound     = 10    


{
' El Jugador
  _clkmode = xtal1 + pll16x
  _xinfreq = 5_000_000

  spiDO     = 0
  spiClk    = 1
  spiDI     = 2
  spiCS     = 3

  Keybd     = 23    'See object changes below for game pad
  Tv_base   = 12
  Sound     = 11
}
{
'Hydra
  _clkmode = xtal1 + pll8x
  _xinfreq = 10_000_000

  spiDO     = 16
  spiClk    = 17
  spiDI     = 18
  spiCS     = 19

  Keybd     = 15    
  Tv_base   = 24
  Sound     = 7
}
{
'Drake Blade
  _clkmode = xtal1 + pll16x
  _xinfreq = 5_000_000

  spiDO     = 12
  spiClk    = 13
  spiDI     = 14
  spiCS     = 15

  Keybd     = 26    
  Tv_base   = 16
  Sound     = 24
 }


'** end CON configuration ***

  _VCFG = ((%10111 | (Tv_base & 4 == 4) & %1000) << 26) | (Tv_base & %111000) << 6 | %111 << ((Tv_base & 4 == 4) & 4)
  _DIRA = %111 << ((Tv_base & 4 == 4) & 4) << ((Tv_base / 8) * 8)


  HORIZONTAL_PIXELS = 80 
  VERTICAL_PIXELS   = 96

  DIR_NORTH = 0
  DIR_SOUTH = 1
  DIR_EAST  = 2
  DIR_WEST  = 3

  FOCUS_MAIN_MENU = 0
  FOCUS_ROOM      = 1
  FOCUS_OPT_MENU  = 2
  FOCUS_ITEM_MENU = 3
  FOCUS_FILE_MENU = 4
  FOCUS_SAY       = 5

  PIC_SIZE    = 4
  PIC_COUNT   = 3
  
  PIC_X       = 0
  PIC_Y       = 1
  PIC_IMG     = 2 
  PIC_VISIBLE = 3

  BACK_SIZE   = 128
  BACK_COUNT  = 3 '(ego + 2 PIC)

  'pic buffer size
  PIC_BUF_SIZE    = 128
  PIC_BUF_COUNT   = 6

  'variable buffer size
  VAR_BUF_SIZE    = 16

  'item buffer size (4 bytes, 8 entries)
  ITEM_BUF_SIZE   = 32

  'inventory buffer size
  INVENTORY_BUF_SIZE = 8

  BACKDROP_SIZE  = 6400
  
  CODE_BUF_SIZE  = 512

  'file type indicators
  FILE_TYPE_IT   = 0
  FILE_TYPE_RM   = 1
  FILE_TYPE_MID  = 2
  FILE_TYPE_SAV  = 3

  'position in the event table of each event location
  on_load     = 0
  on_use      = 2
  on_take     = 4
  on_look     = 6
  on_talk     = 8
  on_give     = 10
  on_hotspot1 = 12
  on_hotspot2 = 14
  on_hotspot3 = 16
  on_hotspot4 = 18

  'file menu actions
  file_load   = 0
  file_save   = 1

  INDEX_HOTSPOT     = 20 'start of the hotspot data
  
  HOTSPOT_SIZE  = 4
  HOTSPOT_X     = 0
  HOTSPOT_Y     = 1
  HOTSPOT_W     = 2
  HOTSPOT_H     = 3

  '*************** OPCODE commands *******************
  CMD_VAR_SET   = 2
  CMD_VAR_MATH  = 3
  CMD_VAR_TEST  = 4
  CMD_GOTO      = 9
  CMD_ROOM_LOAD = 30
  CMD_HOT_TEST  = 40
  CMD_PIC_LOAD  = 50
  CMD_PIC_HIDE  = 52
  CMD_SAY       = 60
  CMD_EGO_LOAD  = 70
  CMD_INV_ADD   = 80
  CMD_INV_TEST  = 82
  CMD_INV_REM   = 84
  CMD_MUS_PLAY  = 90
  CMD_END       = 255
 
VAR
  byte displayb[HORIZONTAL_PIXELS * VERTICAL_PIXELS]   'allocate display buffer in RAM
  byte backb[BACK_SIZE * BACK_COUNT]                   'back buffer (for ego/pic)
  
  byte picinfo[PIC_SIZE * PIC_COUNT]                   'pic info buffer
  byte picb[PIC_BUF_SIZE * PIC_BUF_COUNT]              'pic image buffer

  byte codeb[CODE_BUF_SIZE]                            'room code buffer
  byte varb[VAR_BUF_SIZE]                              'variable buffer size
  byte itemb[ITEM_BUF_SIZE]                            'item buffer size
  byte invb[INVENTORY_BUF_SIZE]                        'inventory buffer size

  long ioControl[2]                                    'control address (fsrw)
  byte filenb[12]                                      'filename buffer
  
  long code_ptr
  byte in_event                                        'indicates whether or not
                                                       'we are currently interpreting
                                                       'an event

  byte last_room                                       'store the last room visited
  byte cur_room                                        'store the current room
  byte inv_cnt                                         'current inventory count
                                                       
  byte player_x
  byte player_y
  byte player_step
  byte player_dir
  byte player_visible                                  'is player visible

  byte focus
  byte select_x
  byte select_y
  byte select_val

  byte sub_action                                      'sub action on the menu (for inventory/file menus)

  long time, tq, ts, us                                'MIDI vars
  long wait_cnt
      
OBJ
    tv   : "mashed_potatos_PAGE"
    synth: "pm_synth_20"

    'use this for keyboard
    key  : "migs_keyboard"

    'use the for game pad
    'key   : "migs_nes"

    sd   : "fsrwFemto"    
        
PUB Main
   sd.start(@ioControl)
   sd.mount(spiDO,spiClk,spiDI,spiCS)

   synth.start(Sound,-1,2)

   inv_cnt := 0
   'initially load item data
   LoadItems

   last_room := 0
   LoadRoom(0, false)
   
   tv.start(@displayb, _VCFG, _DIRA)
         
   key.init(Keybd)

   repeat
      case focus
         FOCUS_ROOM:
           HandleRoom

         FOCUS_OPT_MENU:
           HandleOptionsMenu

         FOCUS_ITEM_MENU:
           HandleInventoryMenu

         FOCUS_FILE_MENU:
           HandleFileMenu

         FOCUS_SAY:
           HandleSay
             
      waitcnt(5_000_000 + cnt)


pub DrawPlayer | img, mirror, hot
   '*****Debug Code******
   'Cls
   'DrawNumAt(4, 81, player_x)
   'DrawNumAt(24, 81, player_y)
   '*********************

   GetBackground(player_x,player_y,0)
   mirror := 0
   
   case player_dir
     DIR_NORTH:
       img := 1
       if player_step == 1
          mirror := 1
          
     DIR_SOUTH:
       img := 0
       if player_step == 1
          mirror := 1

     DIR_EAST:
       img := 2
       if player_step == 1
          img := 3

     DIR_WEST:
       img := 2
       if player_step == 1
          img := 3
       mirror := 1   

   DrawSprite(player_x,player_y,img,mirror)

   player_step++
   if player_step > 1
      player_step := 0

   'check to see if a hotspot has been triggered
   repeat hot from 0 to 3
      if CheckHotspot(hot) == true
         Start_Event(on_hotspot1+(2*hot))
         quit

pub DrawPic (pic_num, x, y) | ptr
   ptr := @picinfo + ((pic_num - 3) * PIC_SIZE)
   'store these values for later use
   byte[ptr][PIC_X] := x
   byte[ptr][PIC_Y] := y
   byte[ptr][PIC_IMG] := pic_num
   byte[ptr][PIC_VISIBLE] := true

   GetBackground(byte[ptr][PIC_X], byte[ptr][PIC_Y], pic_num - 3)
   DrawSprite(byte[ptr][PIC_X], byte[ptr][PIC_Y], pic_num, 0)

pub HidePic(pic_num) | ptr
   RestoreBackground(player_x, player_y, 0)

   ptr := @picinfo + ((pic_num - 3) * PIC_SIZE)
   RestoreBackground(byte[ptr][PIC_X], byte[ptr][PIC_Y], pic_num - 3)
   byte[ptr][PIC_VISIBLE] := false

   DrawPlayer

pub plot(x,y,c)
   
  displayb[y*HORIZONTAL_PIXELS+x] := c

pub DrawChar(x,y,ch,c) | ptr, fx, fy, b
   ptr := @font + (ch * 2)
   fx := x
   fy := y
    
   repeat 2
      repeat b from 0 to 7
        if (byte[ptr]&(|<(7-b)))
           plot(fx,fy, c)          
        fx++
        if (b == 3)
           fx := x
           fy++
      fx := x
      fy++      
      ptr++

pub Print(x,y,s,c)
  repeat while byte[s][0]
    DrawChar(x,y,byte[s][0]-32,c)
    s++
    x+=4

PUB DrawNumAt (x, y, value) | i, pos
        i := 1_000_000_000
        pos := x
        repeat 10
           if value => i
              DrawChar(pos+=4, y, value / i + 16, $AD)
              value //= i
              result~~
           elseif result or i == 1
              DrawChar(pos+=4, y, 16, $AD)
           i /= 10

pub DrawSprite(x,y,n,mirrored) | ptr, screen_ptr
   ptr := @picb + (n<<7)
   ''find location in screen memory to start sprite
   screen_ptr:=y * HORIZONTAL_PIXELS+x
   repeat 16
     ''break it up into mirrored or not mirrored so we are only checking every line instead
     '' of every pixel for speed up
     if(mirrored)
      screen_ptr+=8
      repeat 8
        if byte[ptr] > $02
             displayb[screen_ptr] := byte[ptr]
        ptr++
        screen_ptr--

     else
      repeat 8
        if byte[ptr] > $02
             displayb[screen_ptr] := byte[ptr]
        ptr++
        screen_ptr++
      screen_ptr-=8
     screen_ptr+=HORIZONTAL_PIXELS 'next line

     
pub CheckPlayerMove(movex, movey) | blocked
    'assume we are not blocked, until we have checked
    'each location the character will occupy
    blocked := 0

    repeat 8
      'a black pixel on the screen in a given location = blocked
      if displayb[(movey + 15)*HORIZONTAL_PIXELS+movex] == $02
         blocked := 1
         quit
      movex++
      
    return blocked
      
pub GetBackground(x,y,b) | i

   repeat i from 0 to 15
     BYTEMOVE(@backb[i<<3]+(b<<7), screen(x,y+i), 8)


pub RestoreBackground(x,y,b) | i

   repeat i from 0 to 15
     BYTEMOVE(screen(x,y+i), @backb[i<<3]+(b<<7), 8)

pub Screen(x,y)
  result := @displayb +(y*HORIZONTAL_PIXELS)+x

pub Cls | k
  BYTEFILL(@displayb + 6400, $02, 1280)
     
'***************** Menu Functions ***********************
pub UpdateSelect
   DrawChar(select_x,select_y,$1E,$AD)

pub DrawOptionsMenu
   CLS
   Print(4,86,string("USE"),$AD)
   Print(4,91,string("TAKE"),$AD)
   Print(24,86,string("LOOK"),$AD)
   Print(24,91,string("TALK"),$AD)
   Print(44,86,string("LOAD"),$AD)
   Print(44,91,string("GIVE"),$AD)   
   Print(64,86,string("SAVE"),$AD)
   Print(64,91,string("QUIT"),$AD)

   UpdateSelect

pub DrawInventoryMenu(action)
   sub_action := action
   select_x := 0
   select_y := 86
   select_val := 0
   UpdateInventoryMenu
   FOCUS := FOCUS_ITEM_MENU

pub UpdateInventoryMenu | curr_x, curr_y, ptr, i
   Cls
   if (sub_action == on_use)
       Print(0,81,string("USE"),$AD)
   else
       Print(0,81,string("GIVE"),$AD)

   curr_x := 4
   curr_y := 86

   ptr := @invb
   repeat i from 0 to 3
     if i < inv_cnt
        GetItemName(byte[ptr+i])
        Print(curr_x,curr_y,@filenb,$AD)
     curr_x += 20

   if inv_cnt > 3
      curr_x := 4
      curr_y := 91
      repeat i from 4 to 7
        if i < inv_cnt
           GetItemName(byte[ptr+i])
           Print(curr_x,curr_y,@filenb,$AD)
        curr_x += 20

   UpdateSelect

pub DrawFileMenu(action)
   sub_action := action
   select_x := 0
   select_y := 86
   select_val := 0
   UpdateFileMenu
   FOCUS := FOCUS_FILE_MENU

pub UpdateFileMenu | curr_x, curr_y, ptr, i
   Cls
   if (sub_action == file_load)
       Print(0,81,string("LOAD"),$AD)
   else
       Print(0,81,string("SAVE"),$AD)

   curr_x := 4
   curr_y := 86
   repeat i from 1 to 4
     DrawNumAt(curr_x, curr_y, i)
     curr_x += 20

   curr_x := 4
   curr_y := 91
   repeat i from 5 to 8
     DrawNumAt(curr_x, curr_y, i)
     curr_x += 20

   UpdateSelect

pub HandleRoom
    if in_event
      Interpret_Next_Command

    else
       if player_visible
          if(key.Player1_Up == 1)       'Up Arrow
            if CheckPlayerMove(player_x, player_y-2)  == 0
               RestoreBackground(player_x,player_y,0)
               player_y -= 2
               player_dir := DIR_NORTH
               DrawPlayer
       
          if(key.Player1_Down == 1)       'Down Arrow
            if CheckPlayerMove(player_x, player_y+2)  == 0
               RestoreBackground(player_x,player_y,0)
               player_y += 2
               player_dir := DIR_SOUTH
               DrawPlayer
            
          if(key.Player1_Left == 1)       'Left Arrow
            if CheckPlayerMove(player_x - 2, player_y)  == 0
               RestoreBackground(player_x,player_y,0)
               player_x -= 2
               player_dir := DIR_WEST
               DrawPlayer
      
          if(key.Player1_Right == 1)       'Right Arrow
            if CheckPlayerMove(player_x + 2, player_y)  == 0
               RestoreBackground(player_x,player_y,0)
               player_x += 2
               player_dir := DIR_EAST
               DrawPlayer

       if(key.Player1_Fire)       'Enter key
            select_x := 0
            select_y := 86
            select_val := 1
            DrawOptionsMenu
            focus := FOCUS_OPT_MENU
      
pub HandleOptionsMenu
   if(key.Select)       '--now "a" key
      Cls
      focus := FOCUS_ROOM

   if(key.Player1_Up == 1)       'Up Arrow
      if (select_y == 91)
          select_y := 86
          select_val++
          DrawOptionsMenu

   if(key.Player1_Down == 1)       'Down Arrow
      if (select_y == 86)
          select_y := 91
          select_val--
          DrawOptionsMenu
   
   if(key.Player1_Left == 1)       'Left Arrow
      if (select_x > 0)
          select_x-= 20
          select_val-=2
          DrawOptionsMenu
   
   if(key.Player1_Right == 1)       'Right Arrow
      if (select_x < 60)
          select_x+= 20
          select_val+=2
          DrawOptionsMenu

   if(key.Player1_Fire == 1)       'Enter key
          Cls
          case select_val
             'TAKE selected
             0:
               focus := FOCUS_ROOM
               Start_Event(on_take)

             'USE selected
             1:
               DrawInventoryMenu(on_use)

             'TALK selected
             2:
               focus := FOCUS_ROOM
               Start_Event(on_talk)

             'LOOK selected
             3:
               focus := FOCUS_ROOM
               Start_Event(on_look)

             'GIVE selected
             4:
               DrawInventoryMenu(on_give)

             'LOAD selected
             5:
               DrawFileMenu(file_load)

             'QUIT selected
             6:
               reboot

             'SAVE selected
             7:
               DrawFileMenu(file_save)


pub HandleInventoryMenu | ptr, var_ptr
   if(key.Select)       '--now "a" key
      Cls
      focus := FOCUS_ROOM

   if(key.Player1_Up == 1)       'Up Arrow
      if (select_y == 91)
          select_y := 86
          select_val-=4
          UpdateInventoryMenu

   if(key.Player1_Down == 1)       'Down Arrow
      if (select_y == 86)
          select_y := 91
          select_val+= 4
          UpdateInventoryMenu

   if(key.Player1_Left == 1)       'Left Arrow
      if (select_x > 0)
          select_x-= 20
          select_val--
          UpdateInventoryMenu

   if(key.Player1_Right == 1)       'Right Arrow
      if (select_x < 60)
          select_x+= 20
          select_val++
          UpdateInventoryMenu

   if(key.Player1_Fire == 1)       'Enter key
      if select_val < inv_cnt
         ptr := @invb
         var_ptr := @varb
         'add the selected item to the last selected item variable
         byte[var_ptr + 3] := byte[ptr+select_val]

      Cls
      focus := FOCUS_ROOM
      'initiate the event that was specified prior
      Start_Event(sub_action)

pub HandleFileMenu | ptr, var_ptr
   if(key.Select)       '--now "a" key
      Cls
      focus := FOCUS_ROOM

   if(key.Player1_Up == 1)       'Up Arrow
      if (select_y == 91)
          select_y := 86
          select_val-=4
          UpdateFileMenu

   if(key.Player1_Down == 1)       'Down Arrow
      if (select_y == 86)
          select_y := 91
          select_val+= 4
          UpdateFileMenu

   if(key.Player1_Left == 1)       'Left Arrow
      if (select_x > 0)
          select_x-= 20
          select_val--
          UpdateFileMenu

   if(key.Player1_Right == 1)       'Right Arrow
      if (select_x < 60)
          select_x+= 20
          select_val++
          UpdateFileMenu

   if(key.Player1_Fire == 1)       'Enter key
      if (sub_action == file_load)
        LoadGame(select_val)

        'load in the current room
        LoadRoom(cur_room, true)

        'trigger the drawing of the player (if visible)
        if (player_visible)
           DrawPlayer

      else
        SaveGame(select_val)

      Cls
      focus := FOCUS_ROOM

'***************** Room Load  *********************** 
pub Start_Event (event_ptr) | ptr
   ptr := @codeb + event_ptr

   'only trigger event if there has been one specified
   if byte[ptr] > 0 
      code_ptr := @codeb + word[ptr]
      in_event := true
   
      'start interpreting commands
      Interpret_Next_Command
   
pub Interpret_Next_Command | vptr, vptr2, op, met
   case byte[code_ptr]
     CMD_VAR_SET:
        vptr := @varb + byte[code_ptr + 1]
        'if this is > 0, this indicates
        'setting to another variables value
        if byte[code_ptr + 2] > 0
           vptr2 := @varb + byte[code_ptr + 3]
           byte[vptr] := byte[vptr2]
        else
           byte[vptr] := byte[code_ptr + 3]
        code_ptr += 4

     CMD_VAR_MATH:
        vptr := @varb + byte[code_ptr + 1]
        op := byte[code_ptr + 2]
        if op > 3
          vptr2 := @varb + byte[code_ptr + 3]
          case op
            4:
              byte[vptr] += byte[vptr2]
            5:
              byte[vptr] -= byte[vptr2]
            6:
              byte[vptr] *= byte[vptr2]
            7:
              byte[vptr] /= byte[vptr2]
        else
          case op
            0:
              byte[vptr] += byte[code_ptr + 3]
            1:
              byte[vptr] -= byte[code_ptr + 3]
            2:
              byte[vptr] *= byte[code_ptr + 3]
            3:
              byte[vptr] /= byte[code_ptr + 3]

        code_ptr += 4

     CMD_VAR_TEST:
        vptr := @varb + (byte[code_ptr + 1] >> 4)
        op := byte[code_ptr + 1] & %00001111
        met := false
        if op > 5
          vptr2 := @varb + byte[code_ptr + 2]
          case op
            6:
              if byte[vptr] > byte[vptr2]
                 met := true
            7:
              if byte[vptr] == byte[vptr2]
                 met := true
            8:
              if byte[vptr] < byte[vptr2]
                 met := true
            9:
              if byte[vptr] => byte[vptr2]
                 met := true
            10:
              if byte[vptr] =< byte[vptr2]
                 met := true
            11:
              if byte[vptr] <> byte[vptr2]
                 met := true
        else
          case op
            0:
              if byte[vptr] > byte[code_ptr + 2]
                 met := true
            1:
              if byte[vptr] == byte[code_ptr + 2]
                 met := true
            2:
              if byte[vptr] < byte[code_ptr + 2]
                 met := true
            3:
              if byte[vptr] => byte[code_ptr + 2]
                 met := true
            4:
              if byte[vptr] =< byte[code_ptr + 2]
                 met := true
            5:
              if byte[vptr] <> byte[code_ptr + 2]
                 met := true

        if met == true
           code_ptr += 4
        else
           code_ptr := @codeb + byte[code_ptr + 3]

     CMD_GOTO:
        code_ptr := @codeb + byte[code_ptr + 2]
        
     CMD_ROOM_LOAD:
         LoadRoom(word[code_ptr +2], false)
        
     CMD_HOT_TEST:
        met := CheckHotspot(byte[code_ptr + 2])
        if met == true
           code_ptr += 4
        else
           code_ptr := @codeb + byte[code_ptr + 3]

     CMD_PIC_LOAD:
        DrawPic (byte[code_ptr + 1], byte[code_ptr + 2], byte[code_ptr + 3])
        code_ptr += 4

     CMD_PIC_HIDE:
        HidePic (byte[code_ptr + 2])
        code_ptr += 4

     CMD_SAY:
        ShowSay(word[code_ptr + 2])
        code_ptr += 4
        
     CMD_EGO_LOAD:
        player_x   := byte[code_ptr + 1]
        player_y   := byte[code_ptr + 2]
        player_dir := byte[code_ptr + 3]
        player_visible := true
        'force the redraw of the player
        DrawPlayer
        code_ptr += 4
     
     CMD_INV_ADD:
        InventoryAdd(byte[code_ptr + 2])
        code_ptr += 4

     CMD_INV_TEST:
        met := InventoryTest(byte[code_ptr + 2])
        if met == true
           code_ptr += 4
        else
           code_ptr := @codeb + byte[code_ptr + 3]

     CMD_INV_REM:
        InventoryRemove(byte[code_ptr + 2])
        code_ptr += 4

     CMD_MUS_PLAY:
        MusicPlay(byte[code_ptr + 2])
        code_ptr += 4

     CMD_END:
        in_event := false

pub CheckHotspot (hot_id) | ptr, met
   met := false
   ptr:= @codeb + INDEX_HOTSPOT + (hot_id * HOTSPOT_SIZE)
   if (player_x > byte[ptr][HOTSPOT_X])
     if (player_x < (byte[ptr][HOTSPOT_X] + byte[ptr][HOTSPOT_W]))
        if (player_y > byte[ptr][HOTSPOT_Y])
           if (player_y < (byte[ptr][HOTSPOT_Y] + byte[ptr][HOTSPOT_H]))
              met := true
   return met

pub InventoryAdd (item_id) | ptr
   ptr := @invb + inv_cnt
   byte[ptr] := item_id
   inv_cnt++

pub InventoryItemAt(item_id) | ptr, i, ret_val
   ret_val := -1
   repeat i from 0 to inv_cnt
      if byte[ptr][i] == item_id
         ret_val := i
         quit
   return ret_val

pub InventoryTest (item_id) | ptr, i, ret_val
   ret_val := false
   if InventoryItemAt(item_id) > -1
      ret_val := true
   return ret_val

pub InventoryRemove (item_id) | loc
   loc := InventoryItemAt(item_id)
   if loc > -1
      bytemove(@invb, @invb +(loc+1), inv_cnt-1)
      inv_cnt--

pub ShowSay (text_ptr) | ptr
   ptr := @codeb + text_ptr
   CLS
   Print(0,86,ptr,$07)
   focus := FOCUS_SAY
   wait_cnt := 0
       
pub HandleSay
   wait_cnt++
   if (wait_cnt > 32)
      Cls
      focus := FOCUS_ROOM

   if(key.Player1_Fire == 1)       'Enter key
      Cls
      focus := FOCUS_ROOM

pub LoadRoom(roomid, from_file) | i, ptr
   GetFileName(roomid, FILE_TYPE_RM)
   i:=sd.popen(@filenb, "r")

   sd.pread(@displayb, BACKDROP_SIZE)
   Cls

   'set the current room id
   cur_room := roomid

   'store last room variable
   ptr := @varb
   byte[ptr+2] := last_room

   'load pic data
   sd.pread(@picb, 768)

   'load the PINT code
   sd.pread(@codeb, CODE_BUF_SIZE)
   sd.pclose

   'set the focus to the room
   focus := FOCUS_ROOM

   if (from_file == false)
      'clear out local variables
      bytefill(@varb+8,0, 8)

      'initialize the player variables
      player_x := 0
      player_y := 0
      player_step := 0
      player_dir := DIR_SOUTH
      player_visible := false

      last_room := roomid

      'start On_Load event
      Start_Event(on_load)

pub LoadItems | i
  GetFileName(0, FILE_TYPE_IT)
  i := sd.popen(@filenb, "r")
  sd.pread(@itemb, ITEM_BUF_SIZE)
  sd.pclose

pub GetFileName(value, type)| ptr, i
  bytefill(@filenb, 0, 12)
  ptr := @filenb

  i := 1_000_000_000
  repeat 10
     if value => i
        byte[ptr++] := value / i + "0"
        value //= i
        result~~
     elseif result or i == 1
        byte[ptr++] := "0"
     i /= 10
  byte[ptr++] := "."

  case type
     FILE_TYPE_IT:
       byte[ptr++] := "I"
       byte[ptr++] := "T"

     FILE_TYPE_RM:
       byte[ptr++] := "R"
       byte[ptr++] := "M"

     FILE_TYPE_MID:
       byte[ptr++] := "M"
       byte[ptr++] := "I"
       byte[ptr++] := "D"

     FILE_TYPE_SAV:
       byte[ptr++] := "S"
       byte[ptr++] := "A"
       byte[ptr++] := "V"

pub GetItemName(itemid) | ptr
  bytefill(@filenb, 0, 12)
  bytemove(@filenb, @itemb + (4 * itemid), 4)

pub MusicPlay(songid) | lg, dt, b, v, st, d1, d2, ch
  us := clkfreq / 1_000_000                             'ticks / microsecond
  b := st := d1 := d2 := v := 0

  GetFileName(songid, FILE_TYPE_MID)
  ifnot sd.popen(@filenb, "r")
    sd.pread(@v,4)
    if v == "M" + "T"<<8 + "h"<<16 + "d"<<24            'Header ID
      sd.pread(@v,4)                                    'Header Len
      v~
      sd.pread(@v,2)                                    'Format
      if v <> 0
       'only Format 0 supported
      else
        sd.pread(@v,2)                                  'Tracks
        sd.pread(@v,2)                                  'Ticks/Quarter
        tq := v>>8 + (v&255)<<8
        ts := 500_000 / tq                              'default Tempo 120 BPM
        sd.pread(@v,4)                                  '"MTrk"
        sd.pread(@v,4)                                  'Track length
        lg := v>>24 + (v>>16&255)<<8 + (v>>8&255)<<16
        time := cnt
        repeat
          sd.pread(@b,1)                                'get delta time
          lg--
          dt := b & $7F
          repeat while b>127
            sd.pread(@b,1)
            lg--
            dt := dt<<7 + (b & $7F)
          time := time + dt*us*ts
          repeat until time-cnt < 0                     'wait deltatime
          b~
          sd.pread(@b,1)                                'MIDI byte
          if b > 127
            st := b                                     'new status byte
          elseif st==$FF
            st~
          ch := st & $0F
          d1~
          d2~
          case st & $F0                                 'decode MIDI Event
            $90: sd.pread(@d1,1)                        'Note
                 sd.pread(@d2,1)                        'Velocity
                 lg -= 3
                 if d2>0
                   synth.noteOn(d1,ch,d2)               'Note On
                 else
                   synth.noteOff(d1,ch)                 'Note Off if Vel=0
            $80: sd.pread(@d1,1)                        'Note Off
                 sd.pread(@d2,1)                        'Velocity
                 lg -= 3
                 synth.noteOff(d1,ch)
            $A0: sd.pread(@d1,1)                        'Poly AfterTouch
                 sd.pread(@d2,1)                        '(not supported)
                 lg -= 3
            $C0: sd.pread(@d1,1)                        'Prg Change
                 lg -= 2
                 synth.prgChange(d1,ch)
            $D0: sd.pread(@d1,1)                        'Mono AfterTouch
                 lg -= 2                                '(not supported)
            $B0: sd.pread(@d1,1)                        'Controller (nr)
                 sd.pread(@d2,1)                        'value
                 lg -= 3
                 if d1==7
                   synth.volContr(d2,ch)                '7=Vol
                 if d1==10
                   synth.panContr(d2,ch)                '10=Pan
            $E0: sd.pread(@d1,1)                        'Pitch Bender
                 sd.pread(@d2,1)                        '(not supported!)
                 lg -= 3
            $F0: sd.pread(@d1,1)                        'Meta Event
                 v~
                 if st==$F0 or st==$F7                  'SysEx
                   repeat
                     sd.pread(@v,1)
                     lg--
                     d1--
                   until v==$F7 or b=< 0
                   d1~
                 else
                   lg -= 2
                   sd.pread(@b,1)                       'Len
                   repeat while b>0                     'read meta data
                     sd.pread(@d2,1)
                     v := v<<8 + d2
                     b--
                     lg--
                   if d1==81                            'Tempo
                     ts := v / tq
            0:   sd.pread(@d1,1)                        'Meta Event Running status
                 lg -= 2
                 sd.pread(@b,1)                         'Len
                 repeat while b>0                       'read meta data
                   sd.pread(@d2,1)
                   v := v<<8 + d2
                   b--
                   lg--

        until lg < 1
    else
     'No MIDI file
    sd.pclose
    synth.allOff
  else
   'File not found

PUB SaveGame (fileid) | i
   GetFileName(fileid, FILE_TYPE_SAV)
   i:=sd.popen(@filenb, "w")

   'Write out internal variables
   sd.pputc(last_room)
   sd.pputc(cur_room)
   sd.pputc(inv_cnt)
   sd.pputc(player_x)
   sd.pputc(player_y)
   sd.pputc(player_step)
   sd.pputc(player_dir)
   sd.pputc(player_visible)

   'write out the variable buffer
   sd.pwrite(@varb, VAR_BUF_SIZE)

   'write out the inventory buffer
   sd.pwrite(@invb, INVENTORY_BUF_SIZE)

   sd.pclose

PUB LoadGame (fileid) | i
   GetFileName(fileid, FILE_TYPE_SAV)
   i:=sd.popen(@filenb, "r")

   'read in internal variables
   last_room := sd.pgetc
   cur_room := sd.pgetc
   inv_cnt := sd.pgetc
   player_x := sd.pgetc
   player_y := sd.pgetc
   player_step := sd.pgetc
   player_dir := sd.pgetc
   player_visible := sd.pgetc

   'read in the variable buffer
   sd.pread(@varb, VAR_BUF_SIZE)

   'read in the inventory buffer
   sd.pread(@invb, INVENTORY_BUF_SIZE)

   sd.pclose

DAT

'0 = black
'1 = blue
'2 = green
'3 = cyan
'4 = red
'5 = magenta
'6 = brown      
'7 = lt. gray
'8 = dark gray
'9 = bright blue
'10 = bright green
'11 = bright cyan
'12 = bright red
'13 = bright magenta
'14 = bright yellow
'15 = white


'0=black
'1=dk gray
'2=red
'3=brown
'4=dk green
'5=cyan
'6=dk blue
'7=magenta

'8=white
'9=lt gray
'10=ltred
'11=yellow
'12=lt green
'13=bright cyan
'14=bright blue
'15=lt magenta


'Windows Bitmap
'clut
'clut0   byte  byte  $02, $03, $3d, $5c, $ab, $bc, $fc, $1b
'clut1   byte  byte  $07, $06, $3e, $8d, $ce, $5e, $fd, $1c

font    byte %00000000 '(space)
        byte %00000000

        byte %01000100 '!
        byte %00000100

        byte %10100000 '"
        byte %00000000

        byte %10101110 '#
        byte %11101010

        byte %01001110 '$
        byte %11000110

        byte %10100010 '%
        byte %01001010

        byte %01101010 '&
        byte %11000110

        byte %00100000 '
        byte %00000000

        byte %01001000 '(
        byte %10000100

        byte %10000100 ')
        byte %01001000

        byte %10100100 '*
        byte %10100000

        byte %01001110 '+
        byte %01000000

        byte %00000000 ',
        byte %01001000
        
        byte %00001110 '-
        byte %00000000

        byte %00000000 '.
        byte %00000100

        byte %00100100 '/
        byte %01001000

        byte %11001010 '0
        byte %10100110

        byte %01001100 '1
        byte %01001110

        byte %11100110 '2
        byte %10001110

        byte %11100110 '3
        byte %00101110

        byte %10101110 '4
        byte %00100010

        byte %11101000 '5
        byte %01101110

        byte %01101000 '6
        byte %11101110

        byte %11100010 '7
        byte %01000100

        byte %11001100 '8
        byte %01100110

        byte %11101110 '9
        byte %00100010

        byte %01000000 ':
        byte %01000000

        byte %01000000 ';
        byte %01001000

        byte %01001000 '<
        byte %01000000

        byte %11100000 '=
        byte %11100000

        byte %10000100 '>
        byte %10000000

        byte %11100010 '?
        byte %00000100

        byte %11101010 '@
        byte %10001110
   
        byte %01001010 'A
        byte %11101010

        byte %11101100 'B
        byte %10101110

        byte %01101000 'C
        byte %10000110

        byte %11001010 'D
        byte %10101100

        byte %11101100 'E
        byte %10001110

        byte %11101100 'F
        byte %10001000

        byte %01101000 'G
        byte %10100110

        byte %10001110 'H
        byte %10101010

        byte %11100100 'I
        byte %01001110

        byte %00100010 'J
        byte %10100100

        byte %10101100 'K
        byte %10101010

        byte %10001000 'L
        byte %10001110

        byte %10101110 'M
        byte %10101010

        byte %01101010 'N
        byte %10101010

        byte %01001010 'O
        byte %10100100

        byte %11101110 'P
        byte %10001000

        byte %11101010 'Q
        byte %11000110

        byte %11101010 'R
        byte %11001010

        byte %11101000 'S
        byte %01101110

        byte %11100100 'T
        byte %01000100

        byte %10101010 'U
        byte %10101100

        byte %10101010 'V
        byte %10100100

        byte %10101010 'W
        byte %11101010

        byte %10100100 'X
        byte %10101010

        byte %10100100 'Y
        byte %01000100

        byte %11100010 'Z
        byte %01001110

        byte %11001000 '[
        byte %10001100

        byte %10000100 '\
        byte %01000010

        byte %01100010 ']
        byte %00100110

        byte %01001010 '^
        byte %00000000

        byte %00000000 '_
        byte %00001110
               
{{

                            TERMS OF USE: MIT License

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
}}
 
