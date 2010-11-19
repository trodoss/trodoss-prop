{{
****************************************
Mythic Flight - (With apologies to any other side-scrolling flying game ;) 
trodoss

Version 0.9 (Feature Complete!)
- added additional values for monsters/max levels/max music
- simplified music constants

Versions 0.1 - 0.8 (game features):
- pseudo-scrolling (horizontal) gameplay 
- gamepad (NES) or keyboard input
- basic sound driver MythicFlight_Music
- 'options' menu, allowing the selection/adjustment of lives, starting level, etc.
- loading/use of level/enemy objects from MapGenerator data

  Keyboard mapping
  ----------------
  Arrow Keys = (D)irectional Pad movement
  Enter      = Select
  Space      = B button (fire)
  
Uses modified version of AIGeneric V2.1, a Colaboration of work by: Doug,Hippy,OBC,Baggers
- Thanks to Baggers for help with many .spin concepts

See end of file for terms of use.  
***************************************
}}
CON
  '' Uncomment section for your configuration type:

'' Demoboard/Protoboard/SpinStudio Boards
   _clkmode  = xtal1 + pll16x
   _xinfreq  = 5_000_000
   video     = 12 '' I/O for composite video connection 
   audio     = 10
   KEYSPIN   = 26 
{
'' Hydra Boards
   _clkmode  = xtal1 + pll8x
   _xinfreq  = 10_000_000
   video    = 24 '' I/O for composite video connection 
   audio     = 10       
   KEYSPIN  = 13 
}   

{
'' Hybrid Boards
   _clkmode  = xtal1 + pll16x
   _xinfreq  = 6_000_000
   video    = 24 '' I/O for composite video connection 
   audio     = 10       
   KEYSPIN  = 12 
}

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
  
  BLACK             = $02 
  GREY              = $03
  LWHITE            = $06
  BWHITE            = $07
  LMAGENTA          = $A3 
  WHITE             = $AF
  RED               = $A2
  LRED              = $1A
  LPURPLE           = $1B
  PURPLE            = $03 
  LGREEN            = $1C
  SKYBLUE           = $1D
  YELLOW            = $1E
  BYELLOW           = $A6
  LGREY             = $1F
  DGREY             = $08
  GREEN             = $A4
  ORANGE            = $06
  BLUE              = $A1 
  CYAN              = $1C
  MAGENTA           = $FC
  LYELLOW           = $1E

  CR                = $0D
  LF                = $0A
  TAB               = $09
  BKS               = $08

  'constants dealing with background data
  BACKGROUND_V_OFFSET = 2           'background vertical offset length
  BACKGROUND_V_OFFSET_ZERO = 1      'background vertical offset length (0 index)
  BACKGROUND_SECTION_LEN  = 16      'background section length
  BACKGROUND_SECTION_LEN_ZERO  = 15 'background section length (0 index)               
  BACKGROUND_SECTION_LEN_OFF = 18   'background section with length offset included
  
  BACKGROUND_SCREEN_WIDTH = 24      'background screen width
  BACKGROUND_SCREEN_WIDTH_ZERO = 23 'background screen width (0 index)
  BACKGROUND_SCREEN_WIDTH_ZERO2 = 22 'background screen width (0 index), adjusted for 16x16 block
  
  BACKGROUND_MAP_END      = 168 '(BACKGROUND_SCREEN_WIDTH * 8) - BACKGROUND_SCREEN_WIDTH
  BACKGROUND_MAP_LENGTH   = 192 '(BACKGROUND_SCREEN_WIDTH * 8) 

  'defines the position in the background data of the screens (pre-defined displays)
  BACKGROUND_TITLE         = 0   'offset of where the title screen starts in the data  
  BACKGROUND_GAMEOVER      = 24  'offset of where game over screen starts
  BACKGROUND_LVL_COMPLETE  = 48  'offset of where 'level completed' screen starts
  BACKGROUND_VICTORY       = 72  'offset of where the game completed(victory) screen starts
  
  'position where 'actual' level data starts in the background data
  BACKGROUND_LVL_START     = 96  'offset of where actual level data starts

  'when the position is tested, result
  TEST_EMPTY     = 0
  TEST_BLOCK     = 1
  TEST_ENEMY     = 2
  TEST_CHARACTER = 3
  TEST_ARROW     = 4
  
  'constants used when accessing enemy data
  ENEMY_SECTION = 0 'defines the section when this enemy activates
  ENEMY_ALIVE   = 1 'toggles alive/dead
  ENEMY_TYPE    = 2 'defines enemy type
  ENEMY_ACTION  = 3 'defines the current action of the enemy
  ENEMY_XPOS    = 4 'X position of the enemy
  ENEMY_YPOS    = 5 'Y position of the enemy
  ENEMY_FRAME   = 6 'defines animation frame of the enemy
  ENEMY_LIFE    = 7 'defines how many 'hits' the enemy can take before dying (set by type)
  
  'length of the structure
  ENEMY_LENGTH = 8

  'constant for the number of enemies that can be present on a given level
  ENEMY_LEVEL_MAX = 8
  
  'length of the block (in bytes) that contains the enemy data for the level
  ENEMY_LEVEL_BLOCK = 64

  'length of the block (in bytes) that contains the boss data
  ENEMY_BOSS_BLOCK = 64

  'length of the block (in bytes) that contains th boss action data
  ENEMY_BOSS_ACT_BLOCK = 8

  'constants for enemy actions
  ENEMY_ACTION_SHOW  = 0
  ENEMY_ACTION_MOVE  = 1

  'constants for boss actions
  BOSS_ACTION_N  = 1
  BOSS_ACTION_NE = 2  
  BOSS_ACTION_E  = 3
  BOSS_ACTION_SE = 4
  BOSS_ACTION_S  = 5
  BOSS_ACTION_SW = 6
  BOSS_ACTION_W  = 7
  BOSS_ACTION_NW = 8

  'arrow block definition
  ARROW_ACTIVE = 0 'toggles active/inactive
  ARROW_XPOS   = 1 'X position of the arrow
  ARROW_YPOS   = 2 'Y position of the arrow
  ARROW_COUNT  = 3 'counter for the arrow

  'length of the arrow structure
  ARROW_LENGTH = 4

  'maximum number of arrows
  ARROW_MAX = 4
  
  'constant for the block (in bytes) for the arrow data
  ARROW_BLOCK = 16

  'constants for the music associated with screens
  MUSIC_TITLE       = 1
  MUSIC_GAMEOVER    = 2
  MUSIC_LVL_COMP    = 3
  MUSIC_VICTORY     = 4
  MUSIC_LVL_OFFSET  = 4 'offset for the music where level music starts (0-based)

  LEVEL_MAX      = 7  'maximum (end) level in the game (0-based)
  MUSIC_MAX      = 12 'maximum (end) music entry for the game (*1-based)
OBJ
  text  : "MythicFlight_Ai_Driver"
  key   : "comboKeyboard"    
  music : "MythicFlight_Music_002"    

VAR
  word Nes_Pad              
  byte enemydata[ENEMY_LEVEL_BLOCK]
  byte boss_actions[ENEMY_BOSS_ACT_BLOCK]
  byte arrows[ARROW_BLOCK]

  'definition of the boss weapon (only 1, so no structure)
  byte weapon_active
  byte weapon_x
  byte weapon_y
  byte weapon_counter  
  
  byte map_scrolling
  long player_lives
  long max_lives
  long player_score
  byte current_level
  byte timing_offset
    
PUB MainProgram
  text.start(video)
  key.start(KEYSPIN)
  music.start(audio)
  
  'set the default maximum lives
  max_lives := 3 
  GameMainHandler
  
PUB GameMainHandler
  player_lives := max_lives
  player_score := 0
  current_level := 0
  timing_offset := 0
  
  'show the title screen
  ShowScreen(BACKGROUND_TITLE, MUSIC_TITLE)
  'start the game
  RunLevel(current_level)

PUB ShowScreen (screen, this_music) | option_selected, select_y
   option_selected := false
   select_y := 12

   'if music is specified, play it
   if this_music > 0
       music.Play(this_music - 1)
          
   repeat until option_selected == true
     text.cls
     DrawBackground(screen)  
     'if this is not the title screen, show the HUD (to display score)
     if (screen <> BACKGROUND_TITLE)
          DrawHUD
          text.str (8, 12, string("CONTINUE"))
     else
          text.str (7, 10, string("2009 TRODOSS"))     
          text.str (8, 12, string("START GAME"))
           
     text.str (8, 13, string("OPTIONS"))

     Nes_Pad := NES_Read_Gamepad
     if (Nes_Pad & NES0_START)
         option_selected := true
                  
     if(Nes_Pad & NES0_DOWN)
        select_y++
        if (select_y > 13)         
           select_y := 12

     if (Nes_Pad & NES0_UP)
        select_y--                
        if (select_y < 12)
           select_y := 13

     text.pokechar(6, select_y, white, $7F)
     waitcnt(8_000_000 + cnt)
     text.UpdateScreen

   if select_y == 13
      ShowOptionsScreen   
                  
PUB ShowOptionsScreen | exit_selected, select_y, cur_music
   exit_selected := false
   select_y := 10
   cur_music := 0
   music.Pause

   repeat until exit_selected == true
      text.cls
      text.str (10, 4, string("OPTIONS"))
      text.str (10, 10, string("LIVES:"))
      text.dec(18, 10, max_lives)
      text.str (10, 11, string("LEVEL:"))
      text.dec(18, 11,current_level + 1)
      text.str (10, 12, string("MUSIC:"))
      text.dec(18, 12, cur_music)      
      text.str (10, 13, string("EXIT"))
      text.pokechar(8, select_y, white, $7F)

      Nes_Pad := NES_Read_Gamepad
      if (Nes_Pad & NES0_B)
          case select_y
             10 : max_lives++
                  if max_lives > 5
                     max_lives := 3
                  player_lives := max_lives
             11 : current_level++
                  if current_level > LEVEL_MAX
                     current_level := 0
             12 : cur_music++
                  if cur_music > MUSIC_MAX
                     cur_music := 0
                     music.Pause
                  else
                     music.Play(cur_music - 1)
                                                            
      if (Nes_Pad & NES0_START)                              
          if select_y == 13
             exit_selected := true
                  
      if(Nes_Pad & NES0_DOWN)
         select_y++
         if (select_y > 13)         
            select_y := 10
                   
      if (Nes_Pad & NES0_UP)
         select_y--                
         if (select_y < 10)
            select_y := 13
            
      waitcnt(8_000_000 + cnt)
      text.UpdateScreen 
     
PUB RunLevel  (level) |  level_offset, character_x, character_y, map_offset, background_offset
  character_x := 3           'character x position
  character_y := 4           'character y position
  map_offset := 0            'map offset - relative position within the map data
  background_offset := 0     'background offset - map offset + level offset
  map_scrolling := true      'map scrolling indicator
  timing_offset := 0  
  
  level_offset := BACKGROUND_LVL_START + (BACKGROUND_MAP_LENGTH * level) 'level offset = position where the map data begins for the level

  'clear/do the initial level processing
  text.Cls
  
  'start the music for the level
  music.Play(MUSIC_LVL_OFFSET + level)
  
  background_offset := level_offset + map_offset
  LoadEnemyData(level)
  DrawBackground(background_offset)
  HandleEnemies(map_offset)
  
  'start with the character on the screen  
  DrawCharacter16x16 (character_x, character_y, blue,  10, 3, 7 ,5)
  DrawHUD
  text.UpdateScreen

  repeat
    text.cls

    'make sure the map is still scrolling before trying to increment
    if map_scrolling
       map_offset++
       if (map_offset > BACKGROUND_MAP_END)
       
          'at the end of the level -- stop scrolling
           map_offset := BACKGROUND_MAP_END
           map_scrolling := false

    'calculate the background offset
    background_offset := level_offset + map_offset       
    DrawBackground(background_offset)
    HandleEnemies(map_offset)

    'if the character now occupies a space that is 'blocked,' adjust back
    if (TestForBlocked(character_x+1, character_y+1) == TEST_BLOCK)
        character_x--
    else
        if (TestForBlocked(character_x+1, character_y+1) == TEST_ENEMY)
            PlayerDead
        else
            if (TestForBlocked(character_x, character_y+1) == TEST_BLOCK) 
                character_x--
            else
                if (TestForBlocked(character_x, character_y+1) == TEST_ENEMY)
                    PlayerDead
                       
    if map_scrolling           
       'if the check pushed you to the back of the screen = dead
       if (character_x < 1)
           PlayerDead
                                                     
    Nes_Pad := NES_Read_Gamepad
         
    'process movement
    if (Nes_Pad & NES0_RIGHT)
        if (TestForBlocked(character_x+2, character_y+1) == TEST_EMPTY)
            if (character_x + 2 <  BACKGROUND_SCREEN_WIDTH_ZERO)
               character_x++
     
    if (Nes_Pad & NES0_LEFT)
        if (TestForBlocked(character_x-1, character_y+1) == TEST_EMPTY)
            if (character_x-1 > 0)
               character_x--

    if (Nes_Pad & NES0_DOWN)
        if (TestForBlocked(character_x+1, character_y+2) == TEST_EMPTY)
            if (character_y +2 < BACKGROUND_SECTION_LEN_OFF)
               character_y++
     
    if (Nes_Pad & NES0_UP)
        if (TestForBlocked(character_x+1, character_y-1) == TEST_EMPTY)
            if (character_y - 1 > BACKGROUND_V_OFFSET_ZERO)
               character_y--

    'process firing the arrow
    if key.keystate($f2) or key.keystate($f3) or (Nes_Pad & NES0_B)
        AddArrow(character_x + 2, character_y + 1)

    'handle arrows, if any are active
    HandleArrows
            
    'handle character animation    
    if (timing_offset == 0)
        DrawCharacter16x16 (character_x, character_y, blue, 10, 3, 7, 5)
    else 
        DrawCharacter16x16 (character_x, character_y, blue, 11, 3, 8, 5)

    DrawHUD
    waitcnt(8_000_000 + cnt)
   
    text.UpdateScreen  
    timing_offset++
    if (timing_offset > 1)
        timing_offset := 0

PUB TestForBlocked(test_x, test_y) | test_chr
   case text.GetChar( test_x, test_y)
      $03..$0B : test_chr := TEST_CHARACTER
      $0C      : test_chr := TEST_ARROW
      $10..$1F : test_chr := TEST_ENEMY    'standard enemy range
      $20      : test_chr := TEST_EMPTY    
      $21..$2F : test_chr := TEST_ENEMY    'boss range
      $60..$7F : test_chr := TEST_BLOCK    'block (background) range
   return test_chr
   
PUB DrawCharacter16x16 (char_x, char_y, color, chr_top_left, chr_top_right, chr_bot_left, chr_bot_right)
   text.pokechar (char_x, char_y, color, chr_top_left)
   text.pokechar (char_x+1, char_y, color, chr_top_right)
   text.pokechar (char_x,char_y+1,color, chr_bot_left)
   text.pokechar (char_x+1,char_y+1,color, chr_bot_right)      
         
PUB DrawBackground (offset) | i, j, pattern_ptr, pattern_chr, pattern_chr_color, back_level_ptr, back_pattern_ptr
   back_level_ptr := @Background_Level_Map
   back_pattern_ptr := @Background_Pattern_Set
   repeat i from 0 to BACKGROUND_SCREEN_WIDTH_ZERO
      pattern_ptr := (byte[back_level_ptr][offset+ i] * BACKGROUND_SECTION_LEN)

      repeat j from 0 to BACKGROUND_SECTION_LEN_ZERO
          pattern_chr := byte[back_pattern_ptr][pattern_ptr + j]
                 
          case pattern_chr
              $60..$63 : pattern_chr_color := white
              $64..$67 : pattern_chr_color := blue
              $68..$6B : pattern_chr_color := skyblue
              $6C..$6F : pattern_chr_color := purple
              $70..$73 : pattern_chr_color := lpurple
              $74..$77 : pattern_chr_color := green
              $78..$7B : pattern_chr_color := red
              $7C..$7F : pattern_chr_color := lred
              other    : pattern_chr_color := white
                         
          text.pokechar (i , j+ BACKGROUND_V_OFFSET , pattern_chr_color, pattern_chr)

PUB LoadEnemyData (level) | i, data_ptr, boss_ptr, boss_chr
    data_ptr := @Enemy_Data_Set + (ENEMY_LEVEL_BLOCK * level)
    bytemove(@enemydata, data_ptr, ENEMY_LEVEL_BLOCK)
    'each level past the first level, load boss data (char redefines) 
    if (level > 0)
        boss_chr := $21
        boss_ptr := @Enemy_Boss_Data_Set + (ENEMY_BOSS_BLOCK * (level - 1))
        repeat i from 0 to 8
           text.redefine(boss_chr, byte[boss_ptr][0], byte[boss_ptr][1], byte[boss_ptr][2], byte[boss_ptr][3], byte[boss_ptr][4], byte[boss_ptr][5], byte[boss_ptr][6], byte[boss_ptr][7])  
           boss_ptr += 8
           boss_chr++
           
    'load the boss movement action block for the level       
    data_ptr := @Enemy_Boss_Movement_Set + (ENEMY_BOSS_ACT_BLOCK * level)
    bytemove(@boss_actions, data_ptr, ENEMY_BOSS_ACT_BLOCK)

    'set the (boss) weapon information
    weapon_active := false
        

PUB HandleEnemies (offset) | i, ptr, display_offset, enemy_color, enemy_boss, boss_action_ptr, boss_action
   ptr := @enemydata
   'adjust the offset to check the width of the screen
   display_offset :=  offset + BACKGROUND_SCREEN_WIDTH_ZERO 
   
   repeat i from 1 to ENEMY_LEVEL_MAX
      enemy_boss := false
      
      'do not start processing until the enemy would be encountered
      if display_offset > byte[ptr][ENEMY_SECTION]
      
         'make sure the enemy is alive 
         if byte[ptr][ENEMY_ALIVE]

             'while the map is scrolling, automatically move enemy
             if map_scrolling      
                'make sure this is not their initial action
                if byte[ptr][ENEMY_ACTION] <> ENEMY_ACTION_SHOW
                      byte[ptr][ENEMY_XPOS]--
         
                   'if this moves them off the edge, they are 'dead' (disappear) the next cycle
                   if (byte[ptr][ENEMY_XPOS] == 0)
                       byte[ptr][ENEMY_ALIVE] := false

            'handle animation
             byte[ptr][ENEMY_FRAME]++
                if (byte[ptr][ENEMY_FRAME] > 1)
                    byte[ptr][ENEMY_FRAME] := 0

             'determine color based on type
             case byte[ptr][ENEMY_TYPE]
               0 :  enemy_color := green
               1 :  enemy_color := skyblue
               2 :  enemy_color := purple
               3 :  enemy_color := white
               4 :  enemy_color := purple
               5 :  enemy_color := red
               6 :  enemy_color := white
               8 :  enemy_color := yellow
               9 :  enemy_color := white
               10:  enemy_color := blue
               11:  enemy_color := green
               12:  enemy_color := white
               13:  enemy_color := red
               14:  enemy_color := purple
               15:  enemy_color := green

             if (byte[ptr][ENEMY_TYPE] > 7)
                 enemy_boss := true
                 
             'handle movement/drawing the enemy to the screen
             if enemy_boss
                'make sure that the current enemy action is set
                if byte[ptr][ENEMY_ACTION] == ENEMY_ACTION_SHOW
                   byte[ptr][ENEMY_ACTION] := 1

                'bounds checking for the boss character
                if (byte[ptr][ENEMY_XPOS] < 1)
                    byte[ptr][ENEMY_XPOS] := 1
                    byte[ptr][ENEMY_ACTION]++
                else
                    if (byte[ptr][ENEMY_XPOS] > BACKGROUND_SCREEN_WIDTH_ZERO2)
                       byte[ptr][ENEMY_XPOS] := BACKGROUND_SCREEN_WIDTH_ZERO2                  
                       byte[ptr][ENEMY_ACTION]++
                       
                if (byte[ptr][ENEMY_YPOS] < BACKGROUND_V_OFFSET)
                    byte[ptr][ENEMY_YPOS] := BACKGROUND_V_OFFSET
                    byte[ptr][ENEMY_ACTION]++
                else
                    if (byte[ptr][ENEMY_YPOS] > BACKGROUND_SECTION_LEN_OFF)
                       byte[ptr][ENEMY_YPOS] := BACKGROUND_SECTION_LEN_OFF                  
                       byte[ptr][ENEMY_ACTION]++                 

                'test above
                if (TestForBlocked(byte[ptr][ENEMY_XPOS],byte[ptr][ENEMY_YPOS]-1) == TEST_BLOCK)
                   byte[ptr][ENEMY_YPOS]++
                   byte[ptr][ENEMY_ACTION]++
                else
                   'test below
                   if (TestForBlocked(byte[ptr][ENEMY_XPOS],byte[ptr][ENEMY_YPOS]+3) == TEST_BLOCK)
                      byte[ptr][ENEMY_YPOS]--
                      byte[ptr][ENEMY_ACTION]++
                   else
                      'test left
                      if (TestForBlocked(byte[ptr][ENEMY_XPOS]-1,byte[ptr][ENEMY_YPOS]) == TEST_BLOCK)
                          byte[ptr][ENEMY_XPOS]++
                          byte[ptr][ENEMY_ACTION]++
                      else
                          'test right
                          if (TestForBlocked(byte[ptr][ENEMY_XPOS]+3,byte[ptr][ENEMY_YPOS]) == TEST_BLOCK)
                             byte[ptr][ENEMY_XPOS]--
                             byte[ptr][ENEMY_ACTION]++                      
                                 

                'if the action pointer would be advanced past the size of the block, reset
                if byte[ptr][ENEMY_ACTION] > ENEMY_BOSS_ACT_BLOCK
                   byte[ptr][ENEMY_ACTION] := 1
                   
                boss_action_ptr := @boss_actions
                boss_action := byte[boss_action_ptr][byte[ptr][ENEMY_ACTION] - 1]
                
                case boss_action
                   BOSS_ACTION_N  :
                      byte[ptr][ENEMY_YPOS]--
                   BOSS_ACTION_NE :
                      byte[ptr][ENEMY_YPOS]--
                      byte[ptr][ENEMY_XPOS]++
                   BOSS_ACTION_E  :
                      byte[ptr][ENEMY_XPOS]++
                   BOSS_ACTION_SE :
                      byte[ptr][ENEMY_YPOS]++
                      byte[ptr][ENEMY_XPOS]++
                   BOSS_ACTION_S  :
                      byte[ptr][ENEMY_YPOS]++
                   BOSS_ACTION_SW :
                      byte[ptr][ENEMY_YPOS]++
                      byte[ptr][ENEMY_XPOS]--
                   BOSS_ACTION_W  :
                      byte[ptr][ENEMY_XPOS]--
                   BOSS_ACTION_NW :
                      byte[ptr][ENEMY_YPOS]--
                      byte[ptr][ENEMY_XPOS]--                                     

                'Handle boss weapon
                if weapon_active
                   HandleWeapon
                else
                   'only try to 'shoot' when they are at least 10 spaces away from the edge
                   if (byte[ptr][ENEMY_XPOS] > 10)                
                      AddWeapon(byte[ptr][ENEMY_XPOS]-2, byte[ptr][ENEMY_YPOS])
                                           
                'the boss characters are fixed sets of characters, so they will always be drawn the same
                if (byte[ptr][ENEMY_FRAME] == 0)
                    DrawCharacter16x16 (byte[ptr][ENEMY_XPOS], byte[ptr][ENEMY_YPOS] + BACKGROUND_V_OFFSET, enemy_color, $21, $22, $23, $24)
                else
                    DrawCharacter16x16 (byte[ptr][ENEMY_XPOS], byte[ptr][ENEMY_YPOS] + BACKGROUND_V_OFFSET, enemy_color, $25, $26, $27, $28)
                    
             else
                byte[ptr][ENEMY_ACTION] := ENEMY_ACTION_MOVE
            
                'test the next position (left) for blocking
                if (TestForBlocked(byte[ptr][ENEMY_XPOS] -1 , byte[ptr][ENEMY_YPOS] + BACKGROUND_V_OFFSET) == TEST_EMPTY)
                    byte[ptr][ENEMY_XPOS]--

                   'if this moves them off the edge, they are 'dead' the next cycle
                   if (byte[ptr][ENEMY_XPOS] == 0)
                       byte[ptr][ENEMY_ALIVE] := false

                else
                   'test to see if the position above is blocked
                   if (TestForBlocked(byte[ptr][ENEMY_XPOS], byte[ptr][ENEMY_YPOS]+ BACKGROUND_V_OFFSET -1) == TEST_EMPTY)
                       byte[ptr][ENEMY_YPOS]--                       

                      'if this moves them to the top they stay there
                      if (byte[ptr][ENEMY_YPOS] < 0)
                          byte[ptr][ENEMY_YPOS] := 0
                   else
                      'test to see if the position below is blocked
                      if (TestForBlocked(byte[ptr][ENEMY_XPOS], byte[ptr][ENEMY_YPOS] + BACKGROUND_V_OFFSET + 1) == TEST_EMPTY)
                          byte[ptr][ENEMY_YPOS]++

                          'test to make sure that they don't try to sneak off the bottom
                          if (byte[ptr][ENEMY_YPOS] > BACKGROUND_SECTION_LEN_OFF)
                              byte[ptr][ENEMY_YPOS] := BACKGROUND_SECTION_LEN_OFF
                              
                'draw enemy
                text.pokechar(byte[ptr][ENEMY_XPOS], byte[ptr][ENEMY_YPOS] +BACKGROUND_V_OFFSET, enemy_color, byte[ptr][ENEMY_FRAME] + (byte[ptr][ENEMY_TYPE] * 2) + $10)
             
      ptr += ENEMY_LENGTH

PUB HitEnemy (hit_x, hit_y) | i, ptr
   ptr := @enemydata
   repeat i from 1 to ENEMY_LEVEL_MAX
      if (byte[ptr][ENEMY_XPOS] == hit_x)
         if (byte[ptr][ENEMY_YPOS] + BACKGROUND_V_OFFSET == hit_y)
            byte[ptr][ENEMY_LIFE]--
            if (byte[ptr][ENEMY_LIFE] == 0)
                byte[ptr][ENEMY_ALIVE] := false
                'increase score
                player_score += (byte[ptr][ENEMY_TYPE] + 1) * 100

                if (byte[ptr][ENEMY_TYPE] > 7)
                    'this 'dead' event ends the level
                    LevelComplete
                else
                    'put a 'dead' character for 1 cycle at the enemy's present location
                    text.pokechar(byte[ptr][ENEMY_XPOS], byte[ptr][ENEMY_YPOS] +BACKGROUND_V_OFFSET, red, 14)
                    quit
                    
      ptr += ENEMY_LENGTH

PUB HandleArrows | ptr, i
   ptr := @arrows
   repeat i from 1 to ARROW_MAX
      if byte[ptr][ARROW_ACTIVE]
         byte[ptr][ARROW_COUNT]++

         'give each arrow a limited lifespan
         if (byte[ptr][ARROW_COUNT] > 10)
              byte[ptr][ARROW_ACTIVE] := false
              
         if (TestForBlocked(byte[ptr][ARROW_XPOS]+1 , byte[ptr][ARROW_YPOS]) == TEST_EMPTY)
             if (TestForBlocked(byte[ptr][ARROW_XPOS]+1 , byte[ptr][ARROW_YPOS]-1) == TEST_EMPTY)
                if (TestForBlocked(byte[ptr][ARROW_XPOS] , byte[ptr][ARROW_YPOS]) == TEST_EMPTY)
                   byte[ptr][ARROW_XPOS]++
                          
                   if (byte[ptr][ARROW_XPOS] > BACKGROUND_SCREEN_WIDTH_ZERO)
                       byte[ptr][ARROW_ACTIVE] := false
                   else
                       text.pokechar(byte[ptr][ARROW_XPOS], byte[ptr][ARROW_YPOS], white, 12)
                else
                   if (TestForBlocked(byte[ptr][ARROW_XPOS], byte[ptr][ARROW_YPOS]) == TEST_ENEMY)
                      HitEnemy(byte[ptr][ARROW_XPOS], byte[ptr][ARROW_YPOS])
                    
                   'it 'hit' something, so it is no longer active
                   byte[ptr][ARROW_ACTIVE] := false
             else
                if (TestForBlocked(byte[ptr][ARROW_XPOS]+1, byte[ptr][ARROW_YPOS]-1) == TEST_ENEMY)
                    HitEnemy(byte[ptr][ARROW_XPOS]+1, byte[ptr][ARROW_YPOS]-1)
                    
                'it 'hit' something, so it is no longer active
                byte[ptr][ARROW_ACTIVE] := false                         
         else
             if (TestForBlocked(byte[ptr][ARROW_XPOS]+1, byte[ptr][ARROW_YPOS]) == TEST_ENEMY)
                 HitEnemy(byte[ptr][ARROW_XPOS]+1, byte[ptr][ARROW_YPOS])
                   
             'it 'hit' something, so it is no longer active   
             byte[ptr][ARROW_ACTIVE] := false
                  
      ptr += ARROW_LENGTH
PUB AddArrow (add_arrow_x, add_arrow_y) | ptr, i
   ptr := @arrows
   repeat i from 1 to ARROW_MAX
      'find the first available arrow slot
      if (byte[ptr][ARROW_ACTIVE] == false)
          byte[ptr][ARROW_ACTIVE] := true
          byte[ptr][ARROW_XPOS] := add_arrow_x
          byte[ptr][ARROW_YPOS] := add_arrow_y
          byte[ptr][ARROW_COUNT] := 0 
          quit
      ptr += ARROW_LENGTH       

PUB HandleWeapon
    if weapon_active
       weapon_counter++

       'weapon has a limited lifesapn
       if weapon_counter > 10
          weapon_active := false

       if (TestForBlocked(weapon_x-1 , weapon_y) == TEST_EMPTY)
           if (TestForBlocked(weapon_x , weapon_y) == TEST_EMPTY)
              weapon_x--
              if weapon_x < 1
                 weapon_active := false
              else
                 text.pokechar(weapon_x, weapon_y, white, $1E)
           else
              weapon_active := false      
       else
           weapon_active := false
           
PUB AddWeapon (add_weapon_x, add_weapon_y)
   weapon_active := true
   weapon_x := add_weapon_x
   weapon_y := add_weapon_y
   weapon_counter := 0
           
PUB PlayerDead
    player_lives -=1
    if (player_lives < 1)
        ShowScreen(BACKGROUND_GAMEOVER, MUSIC_GAMEOVER)
        player_lives := max_lives
        player_score := 0

        'restart the current level
        RunLevel(current_level)
    else
        'restart the current level
        RunLevel(current_level)

PUB LevelComplete
    current_level++
    'add an extra life (if possible) after completing the level
    player_lives++
    if player_lives > max_lives
       player_lives := max_lives
       
    'if all the levels are completed -- show the victory screen
    if (current_level > LEVEL_MAX)
        ShowScreen(BACKGROUND_VICTORY, MUSIC_VICTORY)
        GameMainHandler
    else 
        ShowScreen(BACKGROUND_LVL_COMPLETE, MUSIC_LVL_COMP)
        RunLevel(current_level)

PUB DrawHUD | i
    if player_lives > 0
       repeat i from 1 to player_lives
          text.pokechar(i, 0, red, 123)
      
    text.dec(16, 0, player_score)
        
PUB NES_Read_Gamepad : nes_bits        |  i
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

' ////////////////////////////////////////////////////////////////////
DAT 
Background_Pattern_Set  file "level_data.pat"

Background_Level_Map    file "level_data.map"

Enemy_Data_Set          file "level_data.emy"

Enemy_Boss_Data_Set     file "level_data.bos"  

Enemy_Boss_Movement_Set file "level_data.bmv"  
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