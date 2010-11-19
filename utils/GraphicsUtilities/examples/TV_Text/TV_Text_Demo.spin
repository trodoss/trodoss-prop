'' Demo of usage of FontGenerator 16x32
'' v0.2 tech preview
CON

  _clkmode = xtal1 + pll16x
  _xinfreq = 5_000_000

OBJ

  text : "tv_texttweaked"
'  text : "tv_text"
'  text : "tv_textw"
  font : "big_digit_font"

#ifdef sDebug
        mon     : "SerialMirror"
#endif

PUB start | i , p

#ifdef sDebug
  mon.start(31, 30, 0, 115200)
#endif

  'start tv text
  text.start(0)

  font.Init

  ' print the list of custom font chars
  p:=font.GetPtrToFontTable
  repeat i from 0 to font#FONT_CHARS - 1
    text.dec(i)
    text.printCustom(p, i)
    text.out(" ")
  nl

  ' now draw a big '8'
  text.printCustom(p, 0)
  text.printCustom(p, 1)
  nl
  text.printCustom(p, 12)
  text.printCustom(p, 13)
  nl
  text.printCustom(p, 2)
  text.printCustom(p, 3)
  nl
  text.str(string("bye"))
  ' darn on my monitor the last line is half off the screen. hmmm

  ' todo probably do a 2x2 variant instead/aswell.

  ' create a method to write a 3x2/2x2 big digit onto the screen
  ' Simply a lookup table from the digit value to the indexes
  ' into the font for each section of the digit. (fit in a long!)

pub nl
  text.str(string($D))

pub dump(adr)
  mon.str(string("val [$"))
  mon.hex(adr,4)
  mon.str(string("] = $"))
  mon.hex(BYTE[adr][0], 2)
  mon.hex(BYTE[adr][1], 2)
  mon.hex(BYTE[adr][2], 2)
  mon.hex(BYTE[adr][3], 2)
  mon.tx(13)



'''
'┌─┐
'│ │
'└─┘
'
' │
' │
' │
'──┐
'┌─┘
'└──
'──┐
' ─┤
'──┘
'
'││
'└┼
' │
'┌──
'└─┐
'──┘
'┌──
'├─┐
'└─┘
'──┐
'  │
'  │
'┌─┐
'├─┤
'└─┘
'┌─┐
'└─┤
'──┘

