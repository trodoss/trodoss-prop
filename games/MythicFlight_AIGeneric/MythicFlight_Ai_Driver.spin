'' MythicFlight_Driver
'' Based on AiGeneric V2.1, a Colaboration of work by: Doug,Hippy,OBC,Baggers

CON
  
  TV_MODE                       = TV_MODE_NTSC + TV_MODE_FONT_0

OBJ

  tv_SmallFont                  : "MythicFlight_Ai_Driver_002"
  
CON

  TV_MODE_NTSC                  = %0000_0000
  TV_MODE_PAL                   = %0000_0001

  TV_MODE_FAST_UPDATE           = %0000_0000
  TV_MODE_FLICKER_FREE          = %0000_0100

  TV_MODE_COLOR                 = %0000_0000
  TV_MODE_COLOUR                = %0000_0000
  TV_MODE_MONOCHROME            = %0000_1000
  
  TV_MODE_INTERLACED            = %0000_0000
  TV_MODE_NON_INTERLACED        = %0001_0000

  TV_MODE_COMPOSITE             = %0000_0000
  TV_MODE_BASEBAND              = %0000_0000
  TV_MODE_BROADCAST             = %0010_0000

  TV_MODE_FONT_0                = %0000_0000

  TV_MODE_RUNNING               = %1000_0000
    
  RGB_BLACK                     = $02 
  RGB_GREY                      = $03
  RGB_WHITE                     = $06
  RGB_BRIGHT_WHITE              = $07
  RGB_CYAN                      = $3D
  RGB_LIGHT_CYAN                = $3E 
  RGB_LIGHT_GREEN               = $5E 
  RGB_LIGHT_YELLOW              = $8E 
  RGB_LIGHT_RED                 = $CE
  RGB_LIGHT_MAGENTA             = $FE 

  RGB_RED                       = $BC          
  RGB_YELLOW                    = $8D
  RGB_GREEN                     = $5C
  RGB_BLUE                      = $1C
  RGB_MAGENTA                   = $FC
  RGB_LIGHT_BLUE                = $1E

  CR                            = $0D
  LF                            = $0A
  TAB                           = $09
  BKS                           = $08
  
VAR

  byte rows
  byte columns

  byte row
  byte column
  byte colour

  byte mode

PUB Start(TV_PIN)
  result := Open (TV_PIN, TV_MODE)
  Cls 


PUB out( b )

  case b
    $08   : column--
    $0D   : TxCrLf
    $00   : Cls
    other : TxRawByte( b )

PUB str(x, y, txtPtr )
  column := x
  row := y
  repeat strsize( txtPtr )
    out( byte[ txtPtr++ ] )

PUB dec(x,y, value) | i
   column := x
   row := y
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

' *******************************************************************************************************
' *                                                                                                     *
' *     Device Handling Routines                                                                        *
' *                                                                                                     *
' *******************************************************************************************************

PUB Open( setBasePin, setMode )

  setMode |= TV_MODE_RUNNING

  if mode <> setMode

    Close
    
    mode := tv_SmallFont.Open( setBasePin, setMode, @PALETTE )
    rows := tv_SmallFont.GetRowCount
    columns := tv_SmallFont.GetColumnCount

    Cls
    
  return mode
  
PUB Close

  if mode
    tv_SmallFont.Close
    mode := 0

' *******************************************************************************************************
' *                                                                                                     *
' *     Text Handling Routines                                                                          *
' *                                                                                                     *
' *******************************************************************************************************
'added - ACS
PUB UpdateScreen
   tv_SmallFont.UpdateScreen

PUB TxRawByte( b )

  if column => columns
    TxCrLf

  tv_SmallFont.PokeChar(column, row, colour, b )
  column++

PUB TxCrLf
  row++
  column := 0
      
PUB Cls

  tv_SmallFont.Cls
  colour := 0
  Home
  
PUB Home

  column := row := 0
              
PUB Color( useColour )

  colour := useColour
  
pub redefine(c,c0,c1,c2,c3,c4,c5,c6,c7) | p

   tv_SmallFont.define(c,c0,c1,c2,c3,c4,c5,c6,c7)

PUB PokeChar( rrow, ccol, ccolour, cc )

    tv_SmallFont.PokeChar(rrow,ccol,ccolour,cc)

PUB GetChar( rrow, ccol ) 

    return tv_SmallFont.GetChar(rrow,ccol)
    
PUB PutChar( rrow, ccol , chr)

    tv_SmallFont.PutChar(rrow,ccol,chr)
      
DAT

' *******************************************************************************************************
' *                                                                                                     *
' *     Colour Palette for Video Drivers                                                                *
' *                                                                                                     *
' *******************************************************************************************************

' Basic QB-style colours
'
'       0 White                 8  Grey
'       1 Blue                  9  Light Blue
'       2 Green                 10 Light Green
'       3 Cyan                  11 Light Cyan
'       4 Red                   12 Light Red
'       5 Magenta               13 Light Magenta
'       6 Yellow                14 Light Yellow
'       7 Bright White          15 White on Red
        
PALETTE byte    RGB_BLACK              ' Background

        byte    RGB_WHITE              ' 0
        byte    RGB_BLUE               ' 1
        byte    RGB_GREEN              ' 2  
        byte    RGB_CYAN               ' 3  
        byte    RGB_RED                ' 4  
        byte    RGB_MAGENTA            ' 5  
        byte    RGB_YELLOW             ' 6  
        byte    RGB_BRIGHT_WHITE       ' 7
          
        byte    RGB_GREY               ' 8 
        byte    RGB_LIGHT_BLUE         ' 9
        byte    RGB_LIGHT_GREEN        ' 10
        byte    RGB_LIGHT_CYAN         ' 11
        byte    RGB_LIGHT_RED          ' 12
        byte    RGB_LIGHT_MAGENTA      ' 13
        byte    RGB_LIGHT_YELLOW       ' 14

        byte    RGB_RED                ' 15 - Background with foreground colour 0