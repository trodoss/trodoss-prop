{{
  Doggiedoc's Savage Wheels

  inspired by:
  LCD_Game.spin
  by Paul A. Willoughby, DVM (Doggiedoc)
  
}}
CON

  _clkmode = xtal1 + pll16x
  _xinfreq = 5_000_000
 
  Keybd     = 23 '16 '= 23    'See object changes below for game pad
  Tv_base   = 12 '24 for Hydra
  Sound     = 11  
 
'** end CON configuration ***

  _VCFG = ((%10111 | (Tv_base & 4 == 4) & %1000) << 26) | (Tv_base & %111000) << 6 | %111 << ((Tv_base & 4 == 4) & 4)
  _DIRA = %111 << ((Tv_base & 4 == 4) & 4) << ((Tv_base / 8) * 8)

  HORIZONTAL_PIXELS = 128
  VERTICAL_PIXELS   = 96

  BACK_SIZE   = 128 
  SPRITE_COUNT   = 4                       'number of sprites
  SPRITE_COUNT_0 = SPRITE_COUNT - 1        'number of sprites, 0 based


  SPRITE_SIZE     = 4       'sprite size
  SPRITE_VISIBLE  = 0       'sprite visible (1 = visible, 0 = invisible)
  SPRITE_X        = 1       'sprite x value
  SPRITE_Y        = 2       'sprite y value
  SPRITE_IMG      = 3       'sprite image 

  STATE_START     = 0       'start/restart the game
  STATE_TITLE     = 1       'showing the title screen
  STATE_PLAYING   = 2       'playing the game
  STATE_GAMEOVER  = 3       'handling Game Over

VAR
  byte displayb[HORIZONTAL_PIXELS * VERTICAL_PIXELS]   'allocate display buffer in RAM
  byte scanb[HORIZONTAL_PIXELS]
  byte backb[BACK_SIZE * SPRITE_COUNT]                 'back buffer (when sprites are drawn)
  byte spriteb[SPRITE_SIZE * SPRITE_COUNT]

  byte player_x
  byte player_y
  byte game_state
  
  'variables to store info on the other car
  byte car_x
  byte car_y
  byte car_active

  byte toggle
  byte delay

  'pointer to the rendering cog
  long draw_cog
  
  'pointer to video status
  long vstatus


  long seed

  'interface variables
  long draw_cmd
  long draw_scrptr
  long draw_datptr
  long draw_bkgptr

  'score variable
  long curr_score

OBJ
    tv   : "mashed_potatos_TV"
    key  : "migs_nes"
   'key  : "migs_keyboard"
        
PUB Main
   tv.start(@displayb, _VCFG, _DIRA, @vstatus)
   key.init(Keybd)

   seed := cnt

   InitRendering
   
   game_state := STATE_START
   
   repeat
      HandleEvents
  
	 
pub ShowTitleScreen
    game_state := STATE_TITLE
	
    Cls($07)
    BYTEMOVE(@displayb + 2304, @logo, 7680)

pub ShowGameOver
    Print(44, 32, string("GAME OVER"), $07)
    game_state := STATE_GAMEOVER
	
pub StartGame
   game_state := STATE_PLAYING
   Cls($02)
   curr_score := 0

   'initially draw the backdrop
   DrawBackdrop

   UpdateScore

   player_x := 60
   player_y := 36
   AddSprite(0)
   SetSprite(0, player_x, player_y, 0)

   toggle :=0
   delay := 0
   
   car_active := 1
   car_x := 24
   car_y := 0
   AddSprite(1)
   SetSprite(1, car_x, car_y, 0)

   RenderSprites
 
pub RenderGameGraphics | img0, img1
     delay++
     if delay > 1
        toggle++
        if toggle > 1
           toggle := 0
        delay := 0
     img0 := toggle
     img1 := toggle + 2
	
     'if in game over mode, switch the car image
     if (game_state == STATE_GAMEOVER)
        img0 := 4

     'wait for the vsync event to render the graphics
     repeat until vstatus == 1
    
     RenderBackdrop

     SetSprite(0, player_x, player_y, img0)
     SetSprite(1, car_x, car_y, img1)
          
     RenderSprites  
 

pub HandleEvents
  case game_state
    STATE_START:
       ShowTitleScreen
	
    STATE_TITLE:
      'handle in title screen
       if (key.Player1_Fire == 1)
          StartGame

    STATE_GAMEOVER:
      'handle in game over
       if (key.Player1_Fire == 1)
          StartGame
		  
    STATE_PLAYING:
      'handle input in game play
      if(key.Player1_Up == 1)       'Up Arrow
         if (player_y > 2)
           player_y -= 2

      if(key.Player1_Down == 1)     'Down Arrow
        if (player_y < 64)
            player_y += 2

      if(key.Player1_Left == 1)     'Left Arrow
        if (player_x > 18)
            player_x -= 2
        else
	    ShowGameOver

      if(key.Player1_Right == 1)    'Right Arrow
        if (player_x < 102)
            player_x += 2
        else
            ShowGameOver		
	  
      if (car_active == 0)
	     AddSprite(1)
	     car_active := 1
             car_y := 0
             car_x := (byte[rand] // 64) + 24
      else
	     if(car_y < 63)
	       car_y += 1
             else
	       car_active := 0
               HideSprite(1)
               'add to score and update display
               curr_score += 50
               UpdateScore

      'check to see if the other car has crashed into our car
      if (SpriteCollision(0, 1))
             game_state := STATE_GAMEOVER

      RenderGameGraphics

pub UpdateScore
  BYTEFILL(@displayb + 10240, $02, 2048)
  Print(4, 85, string("SCORE:"), $07)
  Dec(28, 85, curr_score, $07)


' //////////////////////////////////////////////////////////////////
' Random Number Generation Routine
' //////////////////////////////////////////////////////////////////
PUB rand
    seed := seed * 1103515245 + 12345 + CNT / 7777
    return seed

'------------------------------------------------------------

pub InitRendering | ok
    draw_cmd := 0

    ok := draw_cog := cognew(@entry, @draw_cmd) + 1       ' start the sprite rendering cog
    return ok
	
pub Cls (color) | k
  BYTEFILL(@displayb, color, 12288)

pub plot(x,y,c)
   
  displayb[y<<7+x] := c          '(y * HORIZONTAL_PIXELS) + x 

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

PUB dec(x, y, value, c) | i

  i := 1_000_000_000

  repeat 10
    if value => i
      DrawChar(x,y,value / i + 16,c)
      x += 4
      value //= i
      result~~
    elseif result or i == 1
      DrawChar(x,y,16,c)
      x += 4
    i /= 10

pub DrawBackdrop | roadptr, road_x, road_y, i, j
   roadptr := @tilemap
   road_x := 0
   road_y := 0
   repeat j from 0 to 9
     repeat i from 0 to 15
       DrawTile(road_x,road_y,byte[roadptr])
        
       road_x += 8
       roadptr++ 
     road_y += 7     

pub DrawTile(x,y,n) | i
    repeat i from 0 to 7
     BYTEMOVE(screen(x,y+i), @tiles[i<<3]+(n<<6), 8)

pub Screen(x,y)
  result := @displayb +(y<<7)+x                          '(y * HORIZONTAL_PIXELS) + x

pub AddSprite(sprite_no) | ptr
   ptr := @spriteb + (sprite_no << 2)
   byte[ptr][SPRITE_VISIBLE] := 1

pub HideSprite(sprite_no) | ptr
   ptr := @spriteb + (sprite_no << 2)
   byte[ptr][SPRITE_VISIBLE] := 0
   
pub SetSprite(sprite_no, spr_x, spr_y, spr_img) | ptr
   ptr := @spriteb + (sprite_no << 2)
   byte[ptr][SPRITE_X] := spr_x
   byte[ptr][SPRITE_Y] := spr_y
   byte[ptr][SPRITE_IMG] := spr_img

pub RenderBackdrop |i, ptr
   ptr := @spriteb
   repeat i from 0 to SPRITE_COUNT_0
     if byte[ptr][SPRITE_VISIBLE] == 1
         RestoreBackground(byte[ptr][SPRITE_X], byte[ptr][SPRITE_Y], i)
     ptr += SPRITE_SIZE    
   ScrollBackdrop
                
pub RenderSprites | i, ptr
   ptr := @spriteb + 12 'start at the end
   repeat i from SPRITE_COUNT_0 to 0 
      if byte[ptr][SPRITE_VISIBLE] == 1
         'get the background where the player would be
         GetBackground(byte[ptr][SPRITE_X], byte[ptr][SPRITE_Y], i)      
         'draw the sprite
         DrawSprite(byte[ptr][SPRITE_X], byte[ptr][SPRITE_Y], byte[ptr][SPRITE_IMG])
      ptr -= SPRITE_SIZE
	  
pub SpriteCollision (spr1, spr2) | ptr1, ptr2
    ptr1 := @spriteb + (spr1 << 2)
    ptr2 := @spriteb + (spr2 << 2)
    'test for collision using bounding box
    if ((byte[ptr1][SPRITE_Y] + 16) < byte[ptr2][SPRITE_Y])
      return 0
    if (byte[ptr1][SPRITE_Y] > (byte[ptr2][SPRITE_Y]+16))
      return 0
    if ((byte[ptr1][SPRITE_X] + 8) < byte[ptr2][SPRITE_X])
      return 0
    if (byte[ptr1][SPRITE_X] > (byte[ptr2][SPRITE_X] + 8))
      return 0

    return 1
      
pub DrawSprite(x, y, n)                  
    repeat while draw_cmd <> 0                          ' wait if presently busy
    draw_scrptr := @displayb + (y<<7)+x                 ' determine screen location
    draw_datptr := @sprites + (n<<7)
    draw_cmd    := 1                                    ' set the command to trigger PASM

pub GetBackground(x,y,b)
    repeat while draw_cmd <> 0
    draw_scrptr := @displayb +(y<<7)+x
    draw_bkgptr := @backb+(b<<7)
    draw_cmd    := 2

pub RestoreBackground(x,y,b)
    repeat while draw_cmd <> 0
    draw_scrptr := @displayb +(y<<7)+x
    draw_bkgptr := @backb+(b<<7)
    draw_cmd    := 3

pub ScrollBackdrop    
    repeat while draw_cmd <> 0
    draw_scrptr := @displayb
    draw_datptr := @scanb
    draw_bkgptr := @displayb
    draw_cmd    := 4    

DAT
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

tiles   file "tiles.dat"

sprites file "sprites.dat"

logo    file "logo.dat"

tilemap byte 0, 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 0, 1
        byte 0, 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 0, 1
        byte 1, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 1, 0
        byte 1, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 1, 0
        byte 0, 1, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 0, 1
        byte 0, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 0, 1
        byte 1, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 1, 0
        byte 1, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 0, 0
        byte 0, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 0, 1
        byte 0, 0, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 4, 0, 1
 
'--- start of PASM code -----------------------------------------------------------
              org      0
        
entry         mov      tmp1, par                        'retrieve the parameters
              mov      cmdptr, tmp1                     'command pointer
              add      tmp1, #4
              mov      scrptr, tmp1                     'screen pointer
              add      tmp1, #4
              mov      datptr, tmp1                     'data pointer
              add      tmp1, #4
              mov      bkgptr, tmp1                     'background pointer
                           
getcmd                                                  'main loop: look for a command
              rdlong   tmp1, cmdptr    wz
      if_z    jmp      #getcmd
                                                        'check the command
checkcmd
              cmp      tmp1, #1        wz               'if it is 1, then execute the draw command
      if_e    jmp      #cmddraw
              cmp      tmp1, #2        wz               'if it is 2, then execute the get command
      if_e    jmp      #cmdget
              cmp      tmp1, #3        wz               'if it is 3, then execute the put command
      if_e    jmp      #cmdput
              cmp      tmp1, #4        wz               'if it is 4, then execute the scroll command
      if_e    jmp      #cmdscroll

cmddone       mov      tmp1, #0                         'reset the command pointer to 0
              wrlong   tmp1, cmdptr
              
              jmp      #getcmd              

'draw at a location ----------------------------------------------------------------
cmddraw
              rdlong   tmp2, scrptr                     'read current screen pointer into temp varialbe
              rdlong   tmp3, datptr                     'read current data pointer into temp variable
              mov      cnty, #16
v_loop
              mov      cntx, #8
h_loop
              rdbyte   tmp1, tmp3
              cmp      tmp1, #2       wz                'test for the transparent color ($02)
     if_e     jmp      #h_loop_skip                     'if it is, skip writing
              
              wrbyte   tmp1, tmp2                       'write to current screen location
h_loop_skip
              add      tmp2, #1                         'advance the pointer
              add      tmp3, #1               
              djnz     cntx, #h_loop

              add      tmp2, #120                      'otherwise, add in an entire screen line less the last 8 pixels (128-8)
              djnz     cnty, #v_loop

              jmp      #cmddone

'get pixels at a location -----------------------------------------------------------
cmdget
              rdlong   tmp2, scrptr                    'read the current screen pointer to a temp variable
              rdlong   tmp3, bkgptr                    'read the current background pointer to a temp variable
              mov      cnty, #16
get_v_loop
              mov      cntx, #8
get_h_loop              
              rdbyte   tmp1, tmp2
              add      tmp2, #1
              wrbyte   tmp1, tmp3
              add      tmp3, #1
              djnz     cntx, #get_h_loop

              add      tmp2, #120                      'add in an entire screen line less the last 8 pixels (128-8)
              djnz     cnty, #get_v_loop
         
              jmp      #cmddone

'put pixels at a location -----------------------------------------------------------
cmdput
              rdlong   tmp2, scrptr                    'read the current screen pointer to a temp variable
              rdlong   tmp3, bkgptr                    'read the current background pointer to a temp variable
              mov      cnty, #16
put_v_loop
              mov      cntx, #8
put_h_loop              
              rdbyte   tmp1, tmp3
              add      tmp3, #1
              wrbyte   tmp1, tmp2
              add      tmp2, #1
              djnz     cntx, #put_h_loop

              add      tmp2, #120                      'add in an entire screen line less the last 8 pixels (128-8)
              djnz     cnty, #put_v_loop
         
              jmp      #cmddone

'scroll the entire screen -----------------------------------------------------------
cmdscroll

              rdlong   tmp2, scrptr                    'read the screen pointer to a temp variable
              rdlong   tmp3, datptr                    'read the current data/scanline buffer
              
              mov      cntx, #127                     'set the loop count to 128
              add      tmp2, bufbyte  
              
scr_last_loop
              rdbyte   tmp1, tmp2
              add      tmp2, #1 
              wrbyte   tmp1, tmp3
              add      tmp3, #1
              djnz     cntx, #scr_last_loop
             
'start of middle loop
              rdlong   tmp2, bkgptr
              rdlong   tmp3, scrptr
              
              add      tmp2, bufbyte
              sub      tmp2, #4
              add      tmp3, bufbyte2
              
              mov      cntx, buflong
scr_mid_loop
              rdlong   tmp1, tmp2
              sub      tmp2, #4       
              wrlong   tmp1, tmp3
              sub      tmp3, #4
                             
              djnz     cntx, #scr_mid_loop

'start of top loop
              rdlong   tmp2, scrptr                    'read the current screen pointer to a temp variable
              rdlong   tmp3, datptr                    'read the current data/scanline buffer
              mov      cntx, #127                      'start at the end, and work backwords
                            
scr_top_loop
              rdbyte   tmp1, tmp3
              add      tmp3, #1
              wrbyte   tmp1, tmp2
              add      tmp2, #1
              djnz     cntx, #scr_top_loop             'decrement, jump if not equal
         
              jmp      #cmddone                        
' --------------------------------------------------------------------------------------------------
bufbyte          long    $2780                           '10112
bufbyte2         long    $27FC                           '10240 - 4 (to align correctly)
buflong          long    $9E0                            '2528 

cmdptr           res     1                               ' command pointer
scrptr           res     1                               ' screen pointer
datptr           res     1                               ' data pointer
bkgptr           res     1                               ' background pointer

cntx             res     1                               ' x counter
cnty             res     1                               ' y counter

tmp1             res     1                               ' work vars
tmp2             res     1
tmp3             res     1

                 fit     492
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
