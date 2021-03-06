{
        ORE_TV_Text - 010
        Corrections to PokeCHAR, and PeekChar reversing row & columns -jl
        Added a number of items from MythicFlight's ORE_TEXT_001
        Added Poke8x16
        
}
CON
   BUFFER_BYTES  = 1920 'allocate max display buffer in RAM  20x24 (longs)
   BUFFER_LONGS  = 480
   MAX_ROWS      = 20
   MAX_COLS      = 24
   
   FONT_CHARS    = 128

   COLOR1        = $07
   COLOR2        = $02
   COLOR3        = $05

OBJ
  tv                  : "ORE_TV_005"
  font                : "Font_FantasyFighter"
   
VAR
  byte          displayb[BUFFER_BYTES] 'Display Buffer
  byte          workingb[BUFFER_BYTES] 'Working Buffer
  long          params[4]

  byte rows
  byte columns

  byte row
  byte column
  byte fore
  byte back
    
' *******************************************************************************************************
' *                                                                                                     *
' *     Device Handling Routines                                                                        *
' *                                                                                                     *
' *******************************************************************************************************
PUB Start(TV_PIN)
  Cls 
  result := Open (TV_PIN)

PUB Open( setBasePin )
    params[0] := @displayb
    params[1] := GetPtrToFontTable
    params[2] := 0
    params[3] := setBasePin & (!3)   
        
    tv.start(@params)    'start the tv cog & pass it the parameter block

' *******************************************************************************************************
' *                                                                                                     *
' *     Text Handling Routines                                                                          *
' *                                                                                                     *
' *******************************************************************************************************


PUB out( b )
  PokeChar( row, column, COLOR1, COLOR3, COLOR2, b )
  row++
    
PUB Cls | fill
  fill[0] := COLOR2
  fill[1] := COLOR1
  fill[2] := COLOR2
  fill[3] := $20
  longfill(@workingb, fill, BUFFER_LONGS)
  Home

PUB Home
  column := row := 0

PUB PokeChar( rrow, ccol, col1, col2, col3, cc ) | startpos
  startpos := ((ccol * MAX_ROWS) + rrow ) * 4
  workingb[startpos] := col1
  workingb[startpos + 1] := col2
  workingb[startpos + 2] := col3
  workingb[startpos + 3] := cc

PUB DrawChar(rrow,ccol,col1,col2,cc) | col3
  col3 := PeekColor(rrow, ccol, 3)
  PokeChar(rrow, ccol, col1,col2,col3,cc)
  
PUB PeekChar( rrow, ccol ) | startpos
  startpos := ((ccol * MAX_ROWS) + rrow) * 4
  return workingb[startpos + 3]
  
PUB PeekColor (rrow, ccol, ccolor) | startpos  
  startpos := ((ccol * MAX_ROWS) + rrow) * 4
  return workingb[startpos + (ccolor-1)]

PUB DrawTextAt (x, y, txtPtr) | i, text_end_pos
        text_end_pos := (strsize( txtPtr ) + (x-1)) 
        repeat i from x to text_end_pos
           pokechar(i, y, COLOR1, COLOR3, COLOR2, byte[ txtPtr++ ])

PUB DrawNumAt (x, y, value) | i, pos
        i := 1_000_000_000
        pos := x
        repeat 10
           if value => i
              pokechar(pos++, y, COLOR1, COLOR3, COLOR2, value / i + "0")
              value //= i
              result~~
           elseif result or i == 1
              pokechar(pos++, y, COLOR1, COLOR3, COLOR2, "0")    
           i /= 10    
 
PUB UpdateScreen
    'read the VsyncPtr value from the driver
    repeat until params[2] == 1  
        
    'put the working buffer in the display buffer
    longmove(@displayb, @workingb, BUFFER_LONGS)   

' //////////////////////////////////////////////////////////////////
' Text/Window Handling Routines
' //////////////////////////////////////////////////////////////////
PUB DrawHLine(x, y, l) | i
    repeat i from x to l
      PokeChar(i, y, COLOR1, COLOR3, COLOR2, 17)
        
PUB DrawBox (x, y, w, h) | i, j,  h_border, v_border
        h_border := (w - 2) + x
        v_border := (h - 2) + y 
        pokechar(x,y,COLOR1, COLOR3, COLOR2,16)

        DrawHLine(x+1, y, h_border)
        pokechar(h_border+1, y,COLOR1, COLOR3, COLOR2,18)
        
        repeat i from y+1 to v_border
           pokechar(x, i, COLOR1, COLOR3, COLOR2, 21)  
           repeat j from x+1 to h_border
              pokechar(j,i,COLOR1, COLOR3, COLOR2,32)
           pokechar(h_border+1,i,COLOR1, COLOR3, COLOR2, 21)
           
        pokechar(x,v_border+1,COLOR1, COLOR3, COLOR2,19) 

        DrawHLine(x+1, v_border+1, h_border)
        pokechar(h_border+1,v_border+1,COLOR1, COLOR3, COLOR2,20)
     
PRI GetPtrToFontTable | ptr

   result := font.GetPtrToFontTable              
{{
┌──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐
│                                                   TERMS OF USE: MIT License                                                  │
├──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤
│Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation    │
│files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,    │
│modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software│
│is furnished to do so, subject to the following conditions:                                                                   │
│                                                                                                                              │
│The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.│
│                                                                                                                              │
│THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE          │
│WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR         │
│COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,   │
│ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                         │
└──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
}}