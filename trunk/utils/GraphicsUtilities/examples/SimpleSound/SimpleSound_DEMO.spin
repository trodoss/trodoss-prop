'' SimpleSound Demo

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

CON
  MAX_SONG_INDEX = 3
   
OBJ
  sound  : "SimpleSound_003"
  key   : "comboKeyboard" 
     
PUB mainProgram  | current , i
  
  key.start(KEYSPIN)
  sound.start(audio)
  current := 0
  
  'start playing the current song
  sound.Play(current)

  i := 7902
  repeat
    if key.keystate($0D)
       sound.Synth("B", i)
       i -= 50
       
    if key.keystate($20)
       current++
       if current > MAX_SONG_INDEX
          current := 0
           
       sound.Pause
       sound.Play(current)
     
  