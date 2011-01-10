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
'' MIGS: PS2 Keyboard Interface V1.0  {Two players possible on single controller.}
''
'' Definitions:
'' ------------
'  Player 1 = Dpad & Space/Fire
'  Player 2 = NumberPad & 0 Button/Fire
'  Select = Z key
'  Start = Enter key
'  A/B = A/B keys  
'

OBJ
    key  : "comboKeyboard"

PUB Init (iopin)
    
    'Start MIGS.KEYBOARD with .init(Keyboard_Base_Signal_Pin)
    'Typical Base Signal pin is 26
    
    key.start(iopin)

PUB Player1_Right

    if(key.keystate($C1))
       return (1)

PUB Player1_Left

    if(key.keystate($C0))
       return (1)

PUB Player1_Up

    if(key.keystate($C2))
       return (1)

PUB Player1_Down

    if(key.keystate($C3))
       return (1)

PUB Player1_Fire

    if(key.keystate($0D))
       return (1)

PUB Player2_Right

    if(key.keystate("6"))
       return (1)

PUB Player2_Left

    if(key.keystate("4"))
       return (1)

PUB Player2_Up

    if(key.keystate("8"))
       return (1)

PUB Player2_Down

    if(key.keystate("2"))
       return (1)

PUB Player2_Fire

    if(key.keystate("0"))
       return (1)

PUB Select

    if(key.keystate("a"))
       return (1)
       
PUB Start

    if(key.keystate($0D))
       return (1)
            
PUB A

    if(key.keystate("a"))
       return (1)

PUB B

    if(key.keystate("b"))
       return (1)
              