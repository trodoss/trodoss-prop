{
        MythicFlight_ORE_Text - 001
}
CON
   BUFFER_BYTES  = 1920 'allocate max display buffer in RAM  20x24 (longs)
   BUFFER_LONGS  = 480
   MAX_ROWS      = 24
   MAX_COLS      = 20
   
   FONT_CHARS    = 128

OBJ
  tv                  : "ORE_TV_005"
  font                : "Font_MythicFlight"
   
VAR
  byte          displayb[BUFFER_BYTES] 'Display Buffer
  byte          workingb[BUFFER_BYTES] 'Working Buffer
  long          params[4]

  byte rows
  byte columns

  byte row
  byte column
  byte color1
  byte color2
  byte color3
  
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
    
    color1 := $07
    color2 := $07
    color3 := $02

' *******************************************************************************************************
' *                                                                                                     *
' *     Text Handling Routines                                                                          *
' *                                                                                                     *
' *******************************************************************************************************

PUB out( b )
  PokeChar( row, column, color1, color2, color3, b )
  row++

PUB str(x, y, txtPtr )
  row := x
  column := y
  repeat strsize( txtPtr )
    out( byte[ txtPtr++ ] )

PUB dec(x, y, value) | i
   row := x
   column := y
'' Print a decimal number

  if value < 0
    -value
    out("-")

  i := 1_000_000_000

  repeat 10
    if value => i
      out(value / i + "0")
      value //= i
      result~~
    elseif result or i == 1
      out("0")
    i /= 10

PUB Cls
  longfill(@workingb, $20_02_02_07, BUFFER_LONGS)
  Home 

PUB Home
  column := row := 0
            
PUB Color( useColor1, useColor2, useColor3)
  color1 := useColor1
  color2 := useColor2
  color3 := useColor3

PUB PokeChar( rrow, ccol, col1, col2, col3, cc ) | startpos
  startpos := ((ccol * MAX_COLS) + rrow) * 4
  workingb[startpos] := col1
  workingb[startpos + 1] := col2
  workingb[startpos + 2] := col3
  workingb[startpos + 3] := cc
    
PUB PeekChar( rrow, ccol ) | startpos
  startpos := ((ccol * MAX_COLS) + rrow) * 4
  return workingb[startpos + 3]
        
PUB UpdateScreen
    'read the VsyncPtr value from the driver
    repeat until params[2] == 1  
        
    'put the working buffer in the display buffer
    longmove(@displayb, @workingb, BUFFER_LONGS)   

PRI GetPtrToFontTable | ptr

   result := font.GetPtrToFontTable

PUB Redefine(c,redifine_ptr) | ptr
   ptr := font.GetPtrToFontTable +(c*16)
   wordmove (ptr, redifine_ptr, 8)
                 
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