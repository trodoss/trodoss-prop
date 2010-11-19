{{
****************************************
  Graphics Editor v0.5

  Requirements:
  - TV(NTSC) Capability
  - Keyboard or D-Pad
  - SD Card interface

  Features:
  - Edit/Load/Save 2 Color (1BPP) font characters (128 character set)
  - 16x16 (4 characters) and 8x8 (1 character) edit modes
  - Import of Black/White (1BPP) Bitmap files
  - Save as .Spin code              
***************************************
}}
CON
  '' Uncomment section for your configuration type:

'' Demoboard/Protoboard/SpinStudio Boards
   _clkmode  = xtal1 + pll16x
   _xinfreq  = 5_000_000
   video     = 12 '' I/O for composite video connection
   KEYSPIN   = 26 
{
'' Hydra Boards
   _clkmode  = xtal1 + pll8x
   _xinfreq  = 10_000_000
   video     = 24 '' I/O for composite video connection 
   KEYSPIN  = 13 
}   

{
'' Hybrid Boards
   _clkmode  = xtal1 + pll16x
   _xinfreq  = 6_000_000
   video     = 24 '' I/O for composite video connection 
   KEYSPIN  = 12 
}

  '' gamepad pin configuration (adjust for your setup)
  JOY_CLK = 16
  JOY_LCH = 17
  JOY_DATAOUT0 = 18
  JOY_DATAOUT1 = 19   
  
  '' SD Card pin configuration (adjust for your setup)
   spiDO     = 0
   spiClk    = 1
   spiDI     = 2
   spiCS     = 3   
  
  ' NES bit encodings for NES gamepad 0
  NES0_RIGHT    = %00000000_00000001
  NES0_LEFT     = %00000000_00000010
  NES0_DOWN     = %00000000_00000100
  NES0_UP       = %00000000_00001000
  NES0_START    = %00000000_00010000
  NES0_SELECT   = %00000000_00100000
  NES0_B        = %00000000_01000000
  NES0_A        = %00000000_10000000
  
  BLACK             = $02 
  GREY              = $03
  LWHITE            = $06
  BWHITE            = $07
  LMAGENTA          = $A3 
  WHITE             = $AF
  RED               = $A2
  LRED              = $1A
  LPURPLE           = $1B
  PURPLE            = $03 
  LGREEN            = $1C
  SKYBLUE           = $1D
  YELLOW            = $1E
  BYELLOW           = $A6
  LGREY             = $1F
  DGREY             = $08
  GREEN             = $A4
  ORANGE            = $06
  BLUE              = $A1 
  CYAN              = $1C
  MAGENTA           = $FC
  LYELLOW           = $1E

  'focus constants (primitive UI handling)
  FOCUS_EDIT_PANE = 0
  
  FOCUS_MAIN_MENU = 1
  FOCUS_FILE_MENU = 2
  FOCUS_VIEW_MENU = 3
  FOCUS_HELP_MENU = 4
  
  FOCUS_LOAD_WIN  = 5
  FOCUS_SAVE_WIN  = 6
  FOCUS_KEYS_WIN  = 7
  FOCUS_ABOUT_WIN = 8
  FOCUS_ERROR_WIN = 9

  'file dialog window action constants
  ACT_LOAD_BIN = 0
  ACT_SAVE_BIN = 1
  ACT_SAVE_SPIN = 2
  ACT_LOAD_BMP = 3

  'edit mode constants
  MODE_8x8        = 0
  MODE_16x16      = 1

  'edit buffer size
  BUFFER_SIZE     = 1024

OBJ
     text  : "GraphicsEditor_AiGeneric_Driver"
     fsrw  : "fsrwFemtoX" 
     key   : "comboKeyboard" 

VAR
     word  Nes_Pad
     long ioControl[2]
     byte edit_buffer[BUFFER_SIZE]
     byte error_msg[20]

     'variables for the file dialog window
     byte filename_buf[12]
     word filename_pos
     byte file_act

     'current input focus
     byte focus

     'current edit mode
     byte mode
     
     'edit pane cursor
     word current_chr
     word current_x
     word current_y

     'menu/window select cursor
     word select_x
     word select_y
     byte select_chr
     
PUB mainProgram
  key.start(KEYSPIN)
  text.start(video)
 
  fsrw.start(@ioControl)

  'Check for the SD card, abort if not found.
  if \fsrw.mount(spiDO,spiClk,spiDI,spiCS) < 0
     DrawWindow(5,4,20,3,red, white, string("SD Error"))
     DrawTextAt(7,5,white,string("SD card not read."))
     DrawTextAt(7,6,white,string("Check connections.")) 
     abort

  InitializeEditPane  
  filename_pos := 0

  MainHandler
  
PUB MainHandler
  repeat 
    text.Cls
  
    'handle input from the user
    Nes_Pad := NES_Read_Gamepad

    'based on what has 'focus' use appropriate handler
    case focus
      FOCUS_EDIT_PANE:
       HandleEditPane
      
      FOCUS_MAIN_MENU:
       HandleMainMenu

      FOCUS_FILE_MENU:
       HandleFileMenu

      FOCUS_VIEW_MENU:
       HandleViewMenu

      FOCUS_HELP_MENU:
       HandleHelpMenu

      FOCUS_LOAD_WIN:
       HandleLoadSaveWindow

      FOCUS_SAVE_WIN:
       HandleLoadSaveWindow

      FOCUS_ERROR_WIN:
       HandleStaticWindow

      FOCUS_KEYS_WIN: 
       HandleStaticWindow
       
      FOCUS_ABOUT_WIN: 
       HandleStaticWindow
                          
    DrawMainScreen

    'added: delay - adjust as appropriate
    waitcnt(5_000_000 + cnt)
    
    text.UpdateScreen

PUB InitializeEditPane
  mode := MODE_16x16
  current_chr := 0
  
  're-initialize the edit buffer
  bytefill(@edit_buffer,0,BUFFER_SIZE)
  ReturnFocusToEditPane
    
PUB HandleEditPane | cur_key
       if (Nes_Pad & NES0_RIGHT)
           current_x++
           if mode == MODE_8x8
              if (current_x > 7)
                 current_x := 7
           else
              if (current_x > 15)
                 current_x := 15
           
       if (Nes_Pad & NES0_LEFT)
           if (current_x > 0)
              current_x--
              
       if (Nes_Pad & NES0_DOWN)
           current_y++
           if mode == MODE_8x8
              if (current_y > 7)
                 current_y := 7
           else
              if (current_y > 15)
                 current_y := 15           
           
       if (Nes_Pad & NES0_UP)
           if (current_y > 0)
              current_y--
              
       if (Nes_Pad & NES0_B)
           SetPixelInCharacter

       if (Nes_Pad & NES0_A)
          
          if mode == MODE_8x8
             current_chr++
             if (current_chr > 127)
                 current_chr := 0
          else
             current_chr += 4
             if (current_chr > 124)
                 current_chr := 0             
              
       if (Nes_Pad & NES0_SELECT)
           focus := FOCUS_MAIN_MENU

       'added: check for direct keystrokes
       cur_key := key.key
       if (cur_key == $2B) or (cur_key == $3D)  '+/= key          
          if mode == MODE_8x8
             current_chr++
             if (current_chr > 127)
                 current_chr := 0
          else
             current_chr += 4
             if (current_chr > 124)
                 current_chr := 0             

       if (cur_key == $2D) or (cur_key == $5F) '-/_ key
          if (current_chr > 0)
             if mode == MODE_8x8          
                current_chr--
             else
                current_chr -= 4
                 
PUB HandleMainMenu
       if (Nes_Pad & NES0_RIGHT)
          select_x += 10
          if (select_x > 20)
             select_x := 0
            
       if (Nes_Pad & NES0_LEFT)
          if (select_x > 0)
             select_x -= 10

       if (Nes_Pad & NES0_DOWN)
          select_y := 4
          case select_x
             0: focus := FOCUS_FILE_MENU
            10: focus := FOCUS_VIEW_MENU 
            20: focus := FOCUS_HELP_MENU
             
       if (Nes_Pad & NES0_SELECT)
          ReturnFocusToEditPane

PUB HandleFileMenu
       if (Nes_Pad & NES0_DOWN)
          select_y ++
          if (select_y > 9)
             select_y := 4

       if (Nes_Pad & NES0_UP)
          if (select_y > 4)
             select_y --

       if (Nes_Pad & NES0_START)              
          case select_y
             4:
               InitializeEditPane
             5:
               focus := FOCUS_LOAD_WIN
               select_x := 0
               select_y := 0
               file_act := ACT_LOAD_BIN
             6:
               focus := FOCUS_SAVE_WIN
               select_x := 0
               select_y := 0
               file_act := ACT_SAVE_BIN
             7:
               focus := FOCUS_SAVE_WIN
               select_x := 0
               select_y := 0
               file_act := ACT_SAVE_SPIN                           
             8:
               focus := FOCUS_LOAD_WIN
               select_x := 0
               select_y := 0
               file_act := ACT_LOAD_BMP
             9:
               reboot
                                                
       if (Nes_Pad & NES0_SELECT)
          focus := FOCUS_MAIN_MENU
          
PUB HandleViewMenu
       if (Nes_Pad & NES0_DOWN)
          select_y++
          if (select_y > 5)
             select_y := 4

       if (Nes_Pad & NES0_UP)
          if (select_y > 4)
             select_y--

       if (Nes_Pad & NES0_START)
          current_chr := 0 
          if select_y == 4
             mode := MODE_8x8
          if select_y == 5
             mode := MODE_16x16
             
          'send focus back to the edit pane
          ReturnFocusToEditPane
                      
       if (Nes_Pad & NES0_SELECT)
          focus := FOCUS_MAIN_MENU
          
PUB HandleHelpMenu
       if (Nes_Pad & NES0_DOWN)
          select_y++
          if (select_y > 5)
             select_y := 4

       if (Nes_Pad & NES0_UP)
          if (select_y > 4)
             select_y--

       if (Nes_Pad & NES0_START)
          current_chr := 0 
          if select_y == 4
             focus := FOCUS_KEYS_WIN
             
          if select_y == 5
             focus := FOCUS_ABOUT_WIN              
                               
       if (Nes_Pad & NES0_SELECT)
          focus := FOCUS_MAIN_MENU

PUB HandleLoadSaveWindow | filename_ptr, cur_key
       filename_ptr := @filename_buf
       filename_ptr += filename_pos
       
       if (Nes_Pad & NES0_RIGHT)
           select_x++
           if (select_x > 11)
               select_x := 11

       if (Nes_Pad & NES0_LEFT)
           if (select_x > 0)
               select_x--

       if (Nes_Pad & NES0_DOWN)
           select_y++
           if (select_y > 3)
               select_y := 3

       if (Nes_Pad & NES0_UP)
           if (select_y > 0)
               select_y--

       if (Nes_Pad & NES0_B)
           byte[filename_ptr] := select_chr
           filename_pos++
           if (filename_pos > 11)
               filename_pos := 11

       if (Nes_Pad & NES0_A)
           byte[filename_ptr] := $00
           if (filename_pos > 0)
              filename_pos--           
                             
       if (Nes_Pad & NES0_SELECT)
          ReturnFocusToEditPane

       if (Nes_Pad & NES0_START)
          ReturnFocusToEditPane
          case file_act
             ACT_LOAD_BIN:
               LoadBinaryFile

             ACT_LOAD_BMP:
               Load1BPPBitmapFile
               
             ACT_SAVE_BIN:
               SaveBinaryFile

             ACT_SAVE_SPIN:
               SaveAsSpinFile
               
       'added: check for direct keystrokes
       cur_key := key.key
       if (cur_key > $60) and (cur_key <$7B)
           cur_key -= 32
    
       if (cur_key > $2A) and (cur_key < $5B)
           byte[filename_ptr] := cur_key
           filename_pos++
           if (filename_pos > 11)
               filename_pos := 11

       if (cur_key == $08)
           byte[filename_ptr] := $00
           if (filename_pos > 0)
              filename_pos--           

PUB HandleStaticWindow                           
    if (Nes_Pad & NES0_SELECT)
       ReturnFocusToEditPane

    if (Nes_Pad & NES0_A)
       ReturnFocusToEditPane
       
PUB ReturnFocusToEditPane
    select_x := 0
    select_y := 0
    current_x   := 0
    current_y   := 0    
    focus := FOCUS_EDIT_PANE
              
PUB LoadBinaryFile
    if fsrw.popen(@filename_buf,"r")
      ShowError(string("File not Found"))
    else
       bytefill(@edit_buffer,0,BUFFER_SIZE)
       fsrw.pread(@edit_buffer,BUFFER_SIZE)
       fsrw.pclose
       
PUB SaveBinaryFile
    if fsrw.popen(@filename_buf,"w")
      ShowError(string("File Save Error"))
    else
      fsrw.pwrite(@edit_buffer,BUFFER_SIZE)
      fsrw.pclose

PUB Load1BPPBitmapFile | temp, width, height, bpp, ptr, last_line
    if fsrw.popen(@filename_buf,"r")
      ShowError(string("File not Found"))
    else  
      fsrw.pread(@temp, 2) 'identifier (skipped)
      repeat 4
         fsrw.pread(@temp, 4) 'skip over 16 bytes of the header
      fsrw.pread(@width, 4) 'bitmap width
      fsrw.pread(@height, 4) 'bitmap height
      fsrw.pread(@temp, 2) 'planes (skipped)
      fsrw.pread(@bpp, 2) 'bits per pixel
      repeat 8     
         fsrw.pread(@temp, 4) 'skip over 32 bytes of the header
         
      if (bpp == 1)
         if (width == 256)
            if (height == 32)
              'if valid, read actual bitmap data
               ptr := @edit_buffer + 775
               
               repeat 4
                  repeat 8
                    last_line := ptr
                 
                    repeat 32                     
                      fsrw.pread(@temp, 1)
                      'invert bits for correct display
                      byte[ptr] := temp >< 8
                      ptr += 8
   
                    ptr := last_line - 1
                    
                  ptr := last_line - 249 
               fsrw.pclose
               
            else
               fsrw.pclose      
               ShowError(string("Height not 32px"))         
         else 
            fsrw.pclose      
            ShowError(string("Width not 256px"))
      else
         fsrw.pclose
         ShowError(string("Not 1-color bitmap"))
        
PUB SaveAsSpinFile | buf_ptr, i
    if fsrw.popen(@filename_buf,"w")
      ShowError(string("File Save Error"))
    else
       fsrw.pwrite(string("PUB GetPtrToFontTable"), 21)
       fsrw.pputc(13)
       fsrw.pputc(10)
       
       fsrw.pwrite(string("   result := @fonttab"), 21)
       fsrw.pputc(13)
       fsrw.pputc(10)
       
       fsrw.pputc(13)
       fsrw.pputc(10)          
       fsrw.pwrite(string("DAT"),3)
       fsrw.pputc(13)
       fsrw.pputc(10)
       buf_ptr := @edit_buffer

       repeat i from 1 to 128
         fsrw.pputc(32) 'Space
         if i == 1
            fsrw.pwrite(string("fonttab byte "), 13)            
         else
            fsrw.pwrite(string("        byte "),13)
            
         repeat 7
           fsrw.pputc(36) '$
           WriteHexValue(byte[buf_ptr])
           fsrw.pputc(44) ',
           buf_ptr++
           
         fsrw.pputc(36) '$
         WriteHexValue(byte[buf_ptr])
         buf_ptr++
         fsrw.pputc(13)
         fsrw.pputc(10)
         
       fsrw.pclose
       
PUB WriteHexValue (value) | i
  value <<= (6) << 2
  repeat 2
    fsrw.pputc(lookupz((value <-= 4) & $F : "0".."9", "A".."F"))          
                  
PUB DrawMainScreen | i
     DrawTextAt(0,0,white, string("Graphics Editor 0.5"))
     DrawTextAt(1,2,white,string("File"))
     DrawTextAt(11,2, white, string("View"))
     DrawTextAt(21,2, white, string("Help"))
     DrawHLine(0,3,39,red)
     
     DrawEditPane

     case focus
       FOCUS_MAIN_MENU:
         text.pokechar(select_x, 2, white, $7F)
         
       FOCUS_FILE_MENU:        
         DrawFileMenu
         text.pokechar(2, select_y, white, $7F)
         
       FOCUS_VIEW_MENU:
         DrawViewMenu
         text.pokechar(12, select_y, white, $7F)
         
       FOCUS_HELP_MENU:
         DrawHelpMenu
         text.pokechar(24, select_y, white, $7F)

       FOCUS_LOAD_WIN:
         DrawFileWindow(string("Load"))

       FOCUS_SAVE_WIN:
         DrawFileWindow(string("Save"))

       FOCUS_ERROR_WIN:
         DrawErrorWindow

       FOCUS_KEYS_WIN:
         DrawKeysWindow

       FOCUS_ABOUT_WIN:
         DrawAboutWindow    

PUB DrawEditPane | cursor_chr
    if mode == MODE_8x8
       DrawWindow (3,5,10,10, red, white, string("8x8"))
    else
       DrawWindow (3,5,18,18, red, white, string("16x16"))

    DrawHexAt(1,6,white, current_chr)
    DrawCharacterInPane(current_chr, 4, 6, $16)
    
    if mode == MODE_16x16
       DrawHexAt(21,6,white, current_chr + 1)
       DrawHexAt(1,14,white, current_chr + 2)
       DrawHexAt(21,14,white, current_chr + 3)
       DrawCharacterInPane(current_chr + 1, 12, 6, $17)
       DrawCharacterInPane(current_chr + 2, 4, 14, $18)
       DrawCharacterInPane(current_chr + 3, 12, 14, $19)

    DrawTextAt(25, 5, white, string("Preview"))
    
    if mode == MODE_8x8
       text.pokechar(25, 7, red, $5B)
       text.pokechar(27, 7, red, $5D)
    else
       DrawBox(25,6,4,4,red)

    'Draw the representing characters onscreen
    text.pokechar(26, 7, white, $16)

    if mode == MODE_16x16
       text.pokechar(27, 7, white, $17)     
       text.pokechar(26, 8, white, $18)     
       text.pokechar(27, 8, white, $19)
    
    'draw edit pane cursor
    cursor_chr := text.getchar(current_x + 4, current_y + 6)
    text.pokechar(current_x + 4, current_y + 6, green, cursor_chr)
        
PUB SetPixelInCharacter | ptr, this_char, char_x, char_y
    this_char := current_chr
    char_x := current_x
    char_y := current_y

    'deterimine the actual current character by position
    if (current_y > 7)
       char_y -= 8
       this_char +=2

    if (current_x > 7)
       char_x -= 8
       this_char +=1
       
    ptr := @edit_buffer
    ptr += (this_char * 8)

    byte[ptr][char_y] := ToggleBitInByte(byte[ptr][char_y], char_x)

pub ToggleBitInByte(value, bit)
  return value ^ (|< bit)
  
PUB DrawCharacterInPane (cur_char, start_x, start_y, redefine_chr) | ptr, i, j, cur_mask
    ptr := @edit_buffer
    ptr += (cur_char * 8)
    
    'redifine the character being displayed
    text.redefine(redefine_chr, byte[ptr][0], byte[ptr][1], byte[ptr][2], byte[ptr][3], byte[ptr][4], byte[ptr][5], byte[ptr][6], byte[ptr][7])
    
    repeat j from start_y to start_y + 7
       cur_mask := 1
       repeat i from start_x to start_x + 7
          if byte[ ptr ] & cur_mask
              text.pokechar(i, j, white, $1B)
          else
              text.pokechar(i, j, white, $1A)  
          cur_mask <-= 1
       ptr++
       
PUB DrawFileMenu
     DrawBox(1,3,15,8,blue)
     DrawTextAt(3,4,white,string("New"))
     DrawTextAt(3,5,white,string("Load"))
     DrawTextAt(3,6,white,string("Save"))
     DrawTextAt(3,7,white,string("Save As Spin"))
     DrawTextAt(3,8,white,string("Import BMP"))
     DrawTextAt(3,9,white,string("Exit"))

PUB DrawViewMenu
     DrawBox(11,3,8,4,blue)
     DrawTextAt(13,4,white,string("8x8"))
     DrawTextAt(13,5,white,string("16x16"))

PUB DrawHelpMenu
     DrawBox(23,3,8,4,blue)
     DrawTextAt(25,4,white,string("Keys"))
     DrawTextAt(25,5,white,string("About"))

PUB DrawFileWindow (name) | i, j, start
     DrawWindow(11,8,14,8,blue,white,name)
     start := $2B
     repeat i from 1 to 4
       repeat j from 1 to 12
          text.pokechar(j+11, i+10, white, start)
          start++
     DrawHLine(12,10,23,blue)

     'draw edit pane cursor
     select_chr := text.getchar(select_x + 12, select_y + 11)
     text.pokechar(select_x + 12, select_y + 11, green, select_chr)

     if filename_pos > 0
        'draw the current filename info
        DrawTextAt(12,9,white, @filename_buf)         

PUB DrawKeysWindow
     DrawWindow(11,8,22,8,blue,white,string("Keys"))
     DrawTextAt(12,9,white,string("Arrow/D-Pad = Move"))
     DrawTextAt(12,10,white,string("Enter/Start = Select"))
     DrawTextAt(12,11,white,string("ESC/Select = Menu/BK"))
     DrawTextAt(12,12,white,string("Space/B = Toggle/BK"))
     DrawTextAt(12,13,white,string("Tab/A = Select/Adv"))
     DrawTextAt(12,14,white,string("Pane: + Adv /- Rev"))

PUB DrawAboutWindow
     DrawWindow(11,8,22,4,blue,white,string("About"))
     DrawTextAt(12,9,white,string("Graphics Editor 0.5"))
     DrawTextAt(12,10,white,string("2009/2010 - trodoss"))
                    
PUB DrawErrorWindow
     DrawWindow(11,8,22,4,red,white,string("Error"))
     DrawTextAt(12,9,white,@error_msg)
     
PUB ShowError (message)
    bytefill(@error_msg,0,20)
    bytemove(@error_msg, message,20)
    focus := FOCUS_ERROR_WIN
            
PUB DrawHLine(x, y, l, c) | i
    repeat i from x to l
      text.pokechar(i, y, c, 17)
        
PUB DrawBox (x, y, w, h, c) | i, j,  h_border, v_border
        h_border := (w - 2) + x
        v_border := (h - 2) + y 
        text.pokechar(x,y,c,16)

        DrawHLine(x+1, y, h_border, c)
        text.pokechar(h_border+1, y,c,18)
        
        repeat i from y+1 to v_border
           text.pokechar(x, i, c, 21)  
           repeat j from x+1 to h_border
              text.pokechar(j,i,c,32)
           text.pokechar(h_border+1,i, c, 21)
           
        text.pokechar(x,v_border+1,c,19) 

        DrawHLine(x+1, v_border+1, h_border, c)
        text.pokechar(h_border+1,v_border+1,c,20)

PUB DrawTextAt (x, y, c, txtPtr) | i, text_end_pos
        text_end_pos := (strsize( txtPtr ) + (x-1)) 
        repeat i from x to text_end_pos
           text.pokechar(i, y, c, byte[ txtPtr++ ])

PUB DrawHexAt (x, y, c, value) | i 
  value <<= (6) << 2
  repeat i from x to x+1
    text.pokechar(i, y, c, lookupz((value <-= 4) & $F : "0".."9", "A".."F"))          

PUB DrawWindow (x, y, w, h, winColor, titleColor, title)
     drawBox (x, y, w, h, winColor)
     drawTextAt(x+1, y, titleColor, title)
              
PUB NES_Read_Gamepad : nes_bits        |  i
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
'Delay(1)                            

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

' Keyboard (mapped onto NES buttons)
    if(key.keystate($C2))       'Up Arrow
      nes_bits|=NES0_UP
    if(key.keystate($C3))       'Down Arrow
      nes_bits|=NES0_DOWN
    if(key.keystate($C0))       'Left Arrow
      nes_bits|=NES0_LEFT
    if(key.keystate($C1))       'Right Arrow
      nes_bits|=NES0_RIGHT
    if(key.keystate($0D))       'Enter
      nes_bits|=NES0_START
    if(key.keystate($1B))       'Escape
      nes_bits|=NES0_SELECT      
    if(key.keystate(" "))       'Space
      nes_bits|=NES0_B
    if(key.keystate($09))       'Tab
      nes_bits|=NES0_A


' //////////////////////////////////////////////////////////////////
' End NES Game Paddle Read
' //////////////////////////////////////////////////////////////////       

' ////////////////////////////////////////////////////////////////////                         
{{
+------------------------------------------------------------------------------------------------------------------------------+
                                                   TERMS OF USE: MIT License                                                                                                              
+------------------------------------------------------------------------------------------------------------------------------
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation     
files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy,    
modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
is furnished to do so, subject to the following conditions:                                                                   
                                                                                                                              
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
                                                                                                                              
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE          
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR         
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,   
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                         
+------------------------------------------------------------------------------------------------------------------------------+
}}                                                                                                   