{  Hi-color-raster-variable-resolution_TV.spin

   Doug Dingus  07/07   Rev 1.0   For HYDRA

  This is a modification to the Easy NTSC driver, first posted by CardboardGuru.  I've
  hardcoded some timing values in order to make selecting resolutions easier.  This
  version of the driver allows variable vertical pixel heights.                    

  The only passed parameter is the beginning of the display bitmap.  Set the pixel
  parameters here, before starting the driver from your spin program.

  Valid horizontal pixel values are:  20, 40, 80, 160, 256, 320

  Valid vertical pixel heights are:  1, 2, 3, 4, etc...

  The last pixel row will be clipped to 192 display lines, therefore, good vertical
  height values are even divisors of 192.

  All display modes consume 1 byte per pixel, resulting in a 125 color display.  This is
  every standard color the Propeller can produce with the reference video circuit,
  without artifacting.  

  At low resolutions, this driver is actually useful.  160x96 only consumes 15K of RAM
  and will produce a useful multi-color display.  Higher resolutions consume most of the
  propeller RAM (30K), with the upper limit (320 x 192) exceeding available RAM space.

  The next version will feature tiles, to take better advantage of the RAM.

  I have the really low resolutions in here for testing overlay sprite type graphics, to
  be running in another COG.

  I've omitted some of CardboardGurus comments for clarity.  See the easy NTSC demo code
  for those areas of this driver init that might not make sense.

  Written for HYDRA -- Doug Dingus   /   CardboardGuru.


}
CON
  ' Set up the processor clock in the standard way for 80MHz
  _CLKMODE = xtal1 + pll16x
  _XINFREQ = 5_000_000
'var                    
'  long BasePin 
   
PUB start(tvpointer, _VCFG, _DirA)
  d_VCFG := _VCFG
  d_DIRA := _DirA 
  
  cognew(@entry,tvpointer)
  
DAT                     org
entry                   jmp     #initialization         'Jump past the constants

DAT  d_VCFG           long  0
DAT  d_DIRA           long  0       

CON  NTSC_color_frequency       =     3_579_545
DAT  NTSC_color_freq            long  NTSC_color_frequency


CON  NTSC_hsync_clocks          =               624
DAT  NTSC_hsync_VSCL              long  160368
DAT  NTSC_control_signal_palette long  $00_00_02_8a
DAT  NTSC_hsync_pixels          long  %%11_0000_1_2222222_11


'***************************************************
'* Blank lines                                     *
'***************************************************
CON  NTSC_active_video_clocks   =     3008
DAT  NTSC_active_video_VSCL     long  NTSC_active_video_clocks



'***************************************************
'* User graphics lines                             *
'***************************************************
' The important lines at last.  You can edit CHOOSE_horizontal_pixels (40, 80, 160, 256, 320)
' You can also edit CHOOSE_vertical_pixel_height.  (0 - 1)  
CON CHOOSE_horizontal_pixels  = 128
CON CHOOSE_vertical_pixel_height = 2   '1++ for variable vertical pixel heights
DAT vpixel                      long    CHOOSE_vertical_pixel_height
CON CHOOSE_clocks_per_gfx_pixel = 2560 / CHOOSE_horizontal_pixels
CON CALC_bytes_per_line = CHOOSE_horizontal_pixels
CON CALC_waitvids = CHOOSE_horizontal_pixels / 4

'4 pixels per waitvid.  We send their colors each time, for one byte per color.

CON CALC_clocks_per_gfx_frame   =  CHOOSE_clocks_per_gfx_pixel*4



DAT CALC_user_data_VSCL         long  CHOOSE_clocks_per_gfx_pixel << 12 + CALC_clocks_per_gfx_frame


CON CALC_frames_per_gfx_line    = CHOOSE_horizontal_pixels/4

CON CALC_overscan = 448 

CON CHOOSE_horizontal_offset    = 00

CON CALC_backporch = 208 + CHOOSE_horizontal_offset    'this must be a multiple of the total
                                                       'pixel clock 16 clocks in this case.
                                                       'only important if one is artifacting
                                                       'for high-color.  Do what you want
                                                       'here that makes sense otherwise.
CON CALC_frontporch = (CALC_overscan - CALC_backporch)

DAT

'***************************************************
'* The code                                        *
'***************************************************
' Start of real code

'all comments in here still valid, so I left them.

initialization          'set up VCFG

                        mov     VCFG, d_VCFG
                        or      DIRA, d_DirA
                        ' VCFG: setup Video Configuration register and 3-bit tv DAC pins to output
                        'movs    VCFG, #%0111_0000       ' VCFG'S = pinmask (pin31: 0000_0111 : pin24)
                        'movd    VCFG, #1                ' VCFG'D = pingroup (grp. 3 i.e. pins 24-31)

                        'movi    VCFG, #%0_11_111_000    ' baseband video on bottom nibble, 2-bit color, enable chroma on broadcast & baseband
                                                        ' %0_xx_x_x_x_xxx : Not used
                                                        ' %x_10_x_x_x_xxx : Composite video to top nibble, broadcast to bottom nibble
                                                        ' %x_xx_1_x_x_xxx : 4 color mode
                                                        ' %x_xx_x_1_x_xxx : Enable chroma on broadcast
                                                        ' %x_xx_x_x_1_xxx : Enable chroma on baseband
                                                        ' %x_xx_x_x_x_000 : Broadcast Aural FM bits (don't care)

                        'or      DIRA, tvport_mask       ' set DAC pins to output

                        ' 
'                        or      DIRA, #1                ' enable debug LED
'                        mov     OUTA, #1                ' light up debug LED

                        ' CTRA: setup Frequency to Drive Video
                        movi    CTRA,#%00001_111        ' pll internal routed to Video, PHSx+=FRQx (mode 1) + pll(16x)
                        mov     r1, NTSC_color_freq     ' r1: Color frequency in Hz (3.579_545MHz)
                        rdlong  v_clkfreq, #0           ' copy clk frequency. (80Mhz)
                        mov     r2, v_clkfreq           ' r2: CLKFREQ (80MHz)
                        call    #dividefract            ' perform r3 = 2^32 * r1 / r2
                        mov     v_freq, r3              ' v_freq now contains frqa.       (191)
                        mov     FRQA, r3                ' set frequency for counter

                                                
                        mov     A, PAR                  'get bitmap address from PAR register
                        mov     bmp, PAR                'store it elsewhere as A will be written to

                        

'-----------------------------------------------------------------------------
                        'NTSC has 244 "visible" lines, but some of that is overscan etc.
                        '  so pick a number of lines for user graphics e.g. 192
                        '  then display (244-192)/2 = 26 lines of overscan before
                        '  and after the user display.
frame_loop
                        mov     line_loop, #26          '(26 so far)
                        'There's always a horizontal sync at the start of each line.  The clever stuff is
                        '  in the constants described earlier.
:vert_back_porch        mov     VSCL, NTSC_hsync_VSCL
                        waitvid NTSC_control_signal_palette, hsync
                        
                        'This is just a blank line.  Notice how the whole line, apart from the
                        '  hsyc is output with one WAITVID.
                        
                        mov     VSCL, NTSC_active_video_VSCL
                        waitvid brown_border_in_color0, #0
                        djnz    line_loop, #:vert_back_porch
'-----------------------------------------------------------------------------
                        'Time to do the user graphics lines.  We're having 192 of them.
                        mov     line_loop, #192         '(218 so far)
                        mov     vpixel_cnt, vpixel
                        'hsync                        
user_graphics_lines     mov     VSCL, NTSC_hsync_VSCL
                        waitvid NTSC_control_signal_palette, hsync
                        'The overscan before the user graphics.   
                        mov     VSCL,#CALC_backporch
                        waitvid green_border_in_color0, #0 
                        
                        'And now at long last, the user graphics.  Program the VSCL for
                        '  4 pixels per frame.  
                        mov     VSCL, CALC_user_data_VSCL
                        

                        'this is where the bitmap gets drawn.  We load a counter with the
                        'number of waitvids / line, and loop until they are done.

                        mov     r1, #CALC_waitvids  'number of horiz pixels / 4
:draw_pixels            rdlong  B, A    'get four pixels from hub

                        {

                        not sure how to do CLUT translation here yet.  I do know
                        at 320 pixels, there is only room for maybe two opcodes
                        lower, say 160 and below, there is room for several

                        }
                        
                        waitvid B, #%%3210  'draw them to screen
                        add     A, #4    'point to next pixel group
                        djnz    r1, #:draw_pixels  'line done?


                        'We've finished drawing a user graphics line, whether it was color
                        '  bar or flag.  Now do the overscan bit.  
end_of_flag_line        mov    VSCL,#CALC_frontporch
                        waitvid blue_border_in_color0 , border

                        'here is where we deal with vertical height.  Things are either
                        'one scanline high, or two.  So and the line_loop counter with
                        'a binary 1 to sort out even from odd lines and subtract
                        'from the bitmap memory pointer depending.

                        sub     vpixel_cnt, #1  wz
               if_z     mov     vpixel_cnt, vpixel
               if_nz    sub     A, #CALC_bytes_per_line         

                        
                        'keep doing the 192 active display area
                        djnz    line_loop, #user_graphics_lines


                        'Reset raster display pointer for next frame
                        mov     A, bmp
'-----------------------------------------------------------------------------
                        'Overscan at the bottom of screen.  
                        mov     line_loop, #26          '(244)
                        'hsync
vert_front_porch        mov     VSCL, NTSC_hsync_VSCL
                        waitvid NTSC_control_signal_palette, hsync
                        mov     VSCL, NTSC_active_video_VSCL
                        waitvid magenta_border_in_color0, #0
                        djnz    line_loop, #vert_front_porch
'-----------------------------------------------------------------------------
                        'This is the vertical sync.  It consists of 3 sections of 6 lines each.
                        mov     line_loop, #6           '(250)
:vsync_higha            mov     VSCL, NTSC_hsync_VSCL
                        waitvid NTSC_control_signal_palette, vsync_high_1
                        mov     VSCL, NTSC_active_video_VSCL
                        waitvid NTSC_control_signal_palette, vsync_high_2
                        djnz    line_loop, #:vsync_higha
'-----------------------------------------------------------------------------
                        mov     line_loop, #6           '(256)
:vsync_low              mov     VSCL, NTSC_hsync_VSCL
                        waitvid NTSC_control_signal_palette, vsync_low_1
                        mov     VSCL, NTSC_active_video_VSCL
                        waitvid NTSC_control_signal_palette, vsync_low_2
                        djnz    line_loop, #:vsync_low
'-----------------------------------------------------------------------------
                        mov     line_loop, #6           '(250)
:vsync_highb            mov     VSCL, NTSC_hsync_VSCL
                        waitvid NTSC_control_signal_palette, vsync_high_1
                        mov     VSCL, NTSC_active_video_VSCL
                        waitvid NTSC_control_signal_palette, vsync_high_2
                        djnz    line_loop, #:vsync_highb
'-----------------------------------------------------------------------------
'                        mov     OUTA, #0

                        jmp     #frame_loop






' /////////////////////////////////////////////////////////////////////////////
' dividefract:
' Perform 2^32 * r1/r2, result stored in r3 (useful for TV calc)
' This is taken from the tv driver.
' NOTE: It divides a bottom heavy fraction e.g. 1/2 and gives the result as a 32-bit fraction.
' /////////////////////////////////////////////////////////////////////////////
dividefract                                     
                        mov     r0,#32+1
:loop                   cmpsub  r1,r2           wc
                        rcl     r3,#1
                        shl     r1,#1
                        djnz    r0,#:loop

dividefract_ret         ret                             '+140


' General Purpose Registers (un initialized)
r0                      long                    $0    ' should typically equal 0
r1                      long                    $0
r2                      long                    $0
r3                      long                    $0

A                       long                    $0  'coupla more general purpose registers
B                       long                    $0
bmp                     long                    $0  'tvpointer ends up here
C                       long                    $0
vpixel_cnt              long                    $0  'counter for vertical pixel height

'This is the scan line counter
line_loop               long    0




' Video (TV) Registers
tvport_mask             long                    %0000_0111<<12

v_freq                  long                    0

' Graphics related vars.
v_coffset               long                    $02020202  ' color offset (every color is added by $02)
v_clkfreq               long                    $0




'Initialized data below here


'Pixel streams
'  These are shifted out of the VSU to the right, so lowest bits are actioned first.
'
hsync                   long    %%11_0000_1_2222222_11  ' Used with NTSC_control_signal_palette so:
                                                        '      0 = blanking level
                                                        '      1 = Black
                                                        '      2 = Color NTSC_control_signal_palette (yellow at zero value)
vsync_high_1            long    %%11111111111_222_11
vsync_high_2            long    %%1111111111111111
vsync_low_1             long    %%22222222222222_11
vsync_low_2             long    %%1_222222222222222
all_black               long    %%1111111111111111
border                  long    %%0000000000000000


'Palettes
'  These are always 4 colors (or blanking level) stored in reverse order:
'                               Color3_Color2_Color1_Color0

blue_border_in_color0   long    $03030303     'All set to grey
green_border_in_color0  long    $03030303     'Feel free to change these for interesting 
magenta_border_in_color0 long   $03030303     'border colors
brown_border_in_color0  long    $03030303



                                   