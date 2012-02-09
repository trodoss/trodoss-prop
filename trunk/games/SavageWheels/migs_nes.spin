CON

'' MIGS: Multi Interface Game-control Standard
'' Designed by Jeff Ledger / Oldbitcollector
''
'' The idea here is to create a set of software interfaces which permit
'' easy implementation of various game control hardware in spin programs.
''
'' This software will establish some programming standards as well as
'' make it possible to switch out game controls without rewriting the game
'' itself.
''
'' The goal of MIGS isn't to support every function of every device,
'' but to create an easy "standard" for game creation & enjoyment.
''
'' MIGS: NES Interface V1.0  {Two players possible on single controller.}
''
'' Definitions:
'' ------------
'  Player 1 = NES0 Standard A/Fire (Start/Select/B)
'  Player 2 = NES1 Standard A/Fire (no start/select/b)


VAR

  long stack[6]
  long nes_bits
  word Nes_Pad  
  byte JOY_CLK
  byte JOY_LCH 
  byte JOY_DATAOUT0
  byte JOY_DATAOUT1 

PUB init (iopin)

  'Start MIGS.NES with .init(NES_Signal_Pin)
  'Typical CLK Signal Pin is 3
  'Assume Latch,Dataout0, & Dataout1 are in the same row

  nes_bits := 0
  JOY_CLK  := iopin
  JOY_LCH  := iopin+1
  JOY_DATAOUT0 := iopin+2
  JOY_DATAOUT1 := iopin+3
  
PUB Player1_Right

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_RIGHT)  
        return (1)

PUB Player1_Left

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_LEFT)  
        return (1)

PUB Player1_Up

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_UP)  
        return (1)

PUB Player1_Down

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_DOWN)  
        return (1)

PUB Player1_Fire

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_A)  
        return (1)
        
PUB Player2_Right

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES1_RIGHT)  
        return (1)

PUB Player2_Left

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES1_LEFT)  
        return (1)

PUB Player2_Up

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES1_UP)  
        return (1)

PUB Player2_Down

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES1_DOWN)  
        return (1)

PUB Player2_Fire

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES1_A)  
        return (1)
                
PUB Select
    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_SELECT)  
        return (1)

PUB Start

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_START)  
        return (1)
            
PUB A

    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_A)  
        return (1)

PUB B
    Nes_Pad := NES_Read_Gamepad
    if (Nes_Pad & NES0_B)  
        return (1)
           
PUB NES_Read_Gamepad        |  i
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

return nes_bits
      
CON

  ' NES bit encodings for NES gamepad 0
  NES0_RIGHT  = %00000000_00000001
  NES0_LEFT   = %00000000_00000010
  NES0_DOWN   = %00000000_00000100
  NES0_UP     = %00000000_00001000
  NES0_START  = %00000000_00010000
  NES0_SELECT = %00000000_00100000
  NES0_B      = %00000000_01000000
  NES0_A      = %00000000_10000000

  ' NES bit encodings for NES gamepad 1
  NES1_RIGHT  = %00000001_00000000
  NES1_LEFT   = %00000010_00000000
  NES1_DOWN   = %00000100_00000000
  NES1_UP     = %00001000_00000000
  NES1_START  = %00010000_00000000
  NES1_SELECT = %00100000_00000000
  NES1_B      = %01000000_00000000
  NES1_A      = %10000000_00000000
  