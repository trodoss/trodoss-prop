{{
  This is just expiramental proof of concept code

}}
CON

  _clkmode = xtal1 + pll16x
  _xinfreq = 5_000_000
 
  Keybd     = 16 '= 23    'See object changes below for game pad
  Tv_base   = 12
  Sound     = 11  
 
'** end CON configuration ***

  _VCFG = ((%10111 | (Tv_base & 4 == 4) & %1000) << 26) | (Tv_base & %111000) << 6 | %111 << ((Tv_base & 4 == 4) & 4)
  _DIRA = %111 << ((Tv_base & 4 == 4) & 4) << ((Tv_base / 8) * 8)

  HORIZONTAL_PIXELS = 128
  VERTICAL_PIXELS   = 96

  BACK_SIZE   = 128
  BACK_COUNT  = 2  

VAR
  byte displayb[HORIZONTAL_PIXELS * VERTICAL_PIXELS]   'allocate display buffer in RAM
  byte scanb[HORIZONTAL_PIXELS]
  byte backb[BACK_SIZE * BACK_COUNT]                   'back buffer (when sprites are drawn)

  byte player_x
  byte player_y
      
OBJ
    tv   : "mashed_potatos_TV"
        key   : "migs_nes"
        'key  : "migs_keyboard"
        
PUB Main | toggle, delay
   tv.start(@displayb, _VCFG, _DIRA)
   key.init(Keybd)
   Cls
   'initially draw the backdrop
   DrawBackdrop

   toggle := 0
   delay := 0
   Print(4, 85, string("SCORE: 0000  LIVES: 00"), $07)
   player_x := 60
   player_y := 36

   'get the background where the car would be
   GetBackground(player_x, player_y, 0)
   DrawSprite(player_x, player_y, 0)

   repeat
     waitcnt(5_000_000 + cnt)
     RestoreBackground(player_x, player_y, 0)
     ScrollBackdrop
     CheckInput
     
     GetBackground(player_x, player_y, 0)
     DrawSprite(player_x, player_y, toggle)
     delay++
     if delay > 3
        toggle++
        if toggle > 1
           toggle := 0
        delay := 0                     

pub ScrollBackdrop
    'capture last scanline
    BYTEMOVE(@scanb, @displayb + 10112, HORIZONTAL_PIXELS)
    'shift down 1 scanline
    BYTEMOVE(@displayb + HORIZONTAL_PIXELS, @displayb, 10112)  
    'replace scanline
    BYTEMOVE(@displayb, @scanb, HORIZONTAL_PIXELS)

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
     
pub CheckInput
   if(key.Player1_Up == 1)       'Up Arrow
      player_y -= 2

   if(key.Player1_Down == 1)     'Down Arrow
      player_y += 2

   if(key.Player1_Left == 1)     'Left Arrow
      player_x -= 2

   if(key.Player1_Right == 1)    'Right Arrow
      player_x += 2
                 
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

pub Cls | k
  BYTEFILL(@displayb, $02, 12288)

pub DrawTile(x,y,n) | ptr, screen_ptr
   ptr := @tiles + (n<<6)
   ''find location in screen memory to start tile
   screen_ptr:= y * HORIZONTAL_PIXELS+x
   repeat 8
     repeat 8
'        if byte[ptr] > $02
             displayb[screen_ptr] := byte[ptr]
        ptr++
        screen_ptr++
     screen_ptr-=8
     screen_ptr+=HORIZONTAL_PIXELS 'next line

pub DrawSprite(x,y,n) | ptr, screen_ptr
   ptr := @sprites + (n<<7)
   ''find location in screen memory to start sprite
   screen_ptr:=y * HORIZONTAL_PIXELS+x
   repeat 16
      repeat 8
        if byte[ptr] > $02
             displayb[screen_ptr] := byte[ptr]
        ptr++
        screen_ptr++
      screen_ptr-=8
      screen_ptr+=HORIZONTAL_PIXELS 'next line
    
pub GetBackground(x,y,b) | i

   repeat i from 0 to 15
     BYTEMOVE(@backb[i<<3]+(b<<7), screen(x,y+i), 8)


pub RestoreBackground(x,y,b) | i

   repeat i from 0 to 15
     BYTEMOVE(screen(x,y+i), @backb[i<<3]+(b<<7), 8)

pub Screen(x,y)
  result := @displayb +(y*HORIZONTAL_PIXELS)+x
        
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

tilemap byte 0, 1, 4, 4, 4, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        byte 0, 1, 3, 3, 3, 3, 1, 1, 1, 1, 1, 4, 4, 1, 1, 1
        byte 1, 1, 1, 1, 1, 0, 1, 0, 1, 4, 4, 5, 2, 1, 1, 0
        byte 1, 1, 1, 0, 1, 1, 1, 1, 1, 2, 2, 6, 2, 4, 1, 0
        byte 0, 1, 1, 1, 1, 1, 1, 1, 4, 2, 2, 2, 2, 3, 1, 1
        byte 0, 1, 4, 4, 4, 4, 1, 1, 3, 3, 3, 3, 3, 1, 1, 1
        byte 1, 4, 5, 2, 2, 5, 4, 1, 1, 1, 1, 1, 1, 1, 1, 0
        byte 1, 2, 6, 2, 2, 6, 2, 1, 1, 1, 1, 0, 1, 1, 1, 0
        byte 0, 3, 3, 3, 3, 3, 3, 1, 1, 0, 1, 1, 1, 1, 1, 1
        byte 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1
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
 