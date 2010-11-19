'' MythicFlight_Driver_002 
'' Based on AiGeneric V2.1, a Colaboration of work by: Doug,Hippy,OBC,Baggers

OBJ

  tv_SmallFont_Driver           : "AiGeneric_Driver_TV"

  tv_SmallFont_Font_0           : "Font_MythicFlight"

  
CON

' .-----------------------------------------------------------------------------------------------------.
' |                                                                                                     |
' |     Driver Attributes                                                                               |
' |                                                                                                     |
' `-----------------------------------------------------------------------------------------------------'

  COLUMNS                       = tv_SmallFont_Driver#COLUMNS
  ROWS_PAL                      = tv_SmallFont_Driver#ROWS_PAL
  ROWS_NTSC                     = tv_SmallFont_Driver#ROWS_NTSC
  COLOURS                       = 16
  
  FONT_CHARS                    = tv_SmallFont_Font_0#FONT_CHARS

  ROWS_MAX                      = ROWS_NTSC #> ROWS_PAL
  
  SCREEN_SIZE_CHARS             = COLUMNS * ROWS_MAX

' .-----------------------------------------------------------------------------------------------------.
' |                                                                                                     |
' |     Mode bit settings - Must be the same as AiChip_TvText_001.spin                                  |
' |                                                                                                     |
' `-----------------------------------------------------------------------------------------------------'

  TV_MODE_NTSC                  = %0000_0000
  TV_MODE_PAL                   = %0000_0001

  TV_MODE_LARGE_FONT            = %0000_0000
  TV_MODE_SMALL_FONT            = %0000_0010

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
  TV_MODE_FONT_1                = %0100_0000

  TV_MODE_RUNNING               = %1000_0000

VAR

  word screenBuffer[SCREEN_SIZE_CHARS]
  word drawBuffer[SCREEN_SIZE_CHARS]  'added - ACS
  word colorPalette[COLOURS]

  long params[5]                ' [0] pointer to screen memory
                                ' [1] pointer to font table
                                ' [2] base pin number = 12 for Demo Board / 24 for Hydra
                                ' [3] background and border colour
                                ' [4] PAL=non-zero, NTSC=zero
  byte mode
  byte rows
  word lastRow

' *******************************************************************************************************
' *                                                                                                     *
' *     Device Handling Routines                                                                        *
' *                                                                                                     *
' *******************************************************************************************************
                 
PUB Open( setBasePin, setMode, ptrToPalette )
  
  if mode <> setMode

    Close
    
    mode := setMode

    mode &= ! TV_MODE_PAL                               ' Driver does not support PAL at present
    mode &= ! TV_MODE_FLICKER_FREE                      ' Driver does not support Flicker Free
    mode &= ! TV_MODE_MONOCHROME                        ' Driver does not support Monochrome
    mode &= ! TV_MODE_BROADCAST                         ' Driver does not support Broadcast

    if mode & TV_MODE_PAL
      rows := ROWS_PAL
    else
      rows := ROWS_NTSC
    lastRow := (rows-1) * COLUMNS

    SetPalette(ptrToPalette)
    
    params[0] := @screenBuffer  
    params[1] := GetPtrToFontTable( setMode & TV_MODE_FONT_1 )
    params[2] := setBasePin
    params[3] := byte[ ptrToPalette ]
    params[4] := mode & TV_MODE_PAL
  
    Cls
    
    tv_SmallFont_Driver.Open(@params)

  return mode

PUB Close

  if mode
    tv_SmallFont_Driver.Close
    mode := 0

' *******************************************************************************************************
' *                                                                                                     *
' *     Font Handling Routines                                                                          *
' *                                                                                                     *
' *******************************************************************************************************

PRI GetPtrToFontTable( fontNumber ) | ptr


   result := tv_SmallFont_Font_0.GetPtrToFontTable 
 
 ptr := result
 if byte[ ptr+constant(" "*8) ]~ 
   repeat constant(FONT_CHARS * 8)
     byte[ ptr++ ] ><= 8
    
' *******************************************************************************************************
' *                                                                                                     *
' *     Text Handling Routines                                                                          *
' *                                                                                                     *
' *******************************************************************************************************
PUB Cls

  wordfill(@drawBuffer, " ", SCREEN_SIZE_CHARS)

PUB PokeChar( row, col, colour, c )

'  drawBuffer[row * COLUMNS + col] := ( colorPalette[ colour // COLOURS ] & $FF00 ) | ( c // FONT_CHARS )
  drawBuffer[col * COLUMNS + row] := ( colorPalette[ colour // COLOURS ] & $FF00 ) | ( c // FONT_CHARS )


PUB GetChar( row, col ) 
   'added - strip off color info
'   return drawBuffer[row * COLUMNS + col] & $FF
   return drawBuffer[col * COLUMNS + row] & $FF


PUB PutChar( row, col, chr )

'  drawBuffer[row * COLUMNS + col]:=chr
  drawBuffer[col * COLUMNS + row]:=chr

pub define(c,c0,c1,c2,c3,c4,c5,c6,c7) | p  
tv_SmallFont_Font_0.define(c,c0,c1,c2,c3,c4,c5,c6,c7)

PUB UpdateScreen
  wordmove(@screenBuffer, @drawBuffer, SCREEN_SIZE_CHARS) 


' *******************************************************************************************************
' *                                                                                                     *
' *     Attribute Handling for Video Drivers                                                            *
' *                                                                                                     *
' *******************************************************************************************************

PUB GetColourCount

  result := COLOURS

PUB GetColumnCount

  result := COLUMNS
    
PUB GetRowCount

  result := rows

' *******************************************************************************************************
' *                                                                                                     *
' *     Colour Palette Handling for Video Drivers                                                       *
' *                                                                                                     *
' *******************************************************************************************************

PUB SetPalette( ptrToPalette ) | i, back

  back := byte[ptrToPalette]
  repeat i from 0 to COLOURS-2
    SetPaletteColour(i,byte[ptrToPalette+i+1],back)
  SetPaletteColour(COLOURS-1,byte[ptrToPalette+1],byte[ptrToPalette+COLOURS])

PUB SetPaletteColour( colorIdx, fore, back )

   colorPalette[colorIdx] := fore << 8 | back

 