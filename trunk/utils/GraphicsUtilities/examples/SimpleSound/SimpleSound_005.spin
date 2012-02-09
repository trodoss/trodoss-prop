{    Simple Sound player
     2012 - trodoss
         
         Version 0.5 - Reduced the data values from LONG to WORD size data
                     - Addressed CNT roll-over condition
         Version 0.4 - Corrected for timing on beats per minute
         Version 0.3 - Added second 'channel' for sfx use
         Version 0.2 - Corrected frequency values/octave logic
         Version 0.1 - Initial release
         
     Based off of Frequency Synthesizer demo by Chip Gracey and Beau Schwabe
         
         This is a simple, two channel sound driver that uses data converted by
         the SoundConvert utility.  The data being used was converted from RTTTL
         (Ring Tone Text Transfer Language).  
         
         Note, the DAT section contains the music/index files being included.  If you
         use different names for these you will need to adjust accordingly.
         
         See end of file for terms of use.  
}
CON  
   'Octave 8 frequency values 
   NOTE_PAUSE  = 0 
   NOTE_A      = 7040 
   NOTE_ASHARP = 7459
   NOTE_B      = 7902   
   NOTE_C      = 4186
   NOTE_CSHARP = 4435
   NOTE_D      = 4699
   NOTE_DSHARP = 4978
   NOTE_E      = 5274
   NOTE_F      = 5588
   NOTE_FSHARP = 5920
   NOTE_G      = 6272
   NOTE_GSHARP = 6645
   
   'Note data structure
   NOTE_VAL_OCT = 0 'note value / octave
   NOTE_DUR_FLG = 1 'note duration / flag (0=none, 1=dotted, 9=EOF)
   
   NOTE_DAT_BLOCK = 2   'length of note data block
   
   STATE_STARTED    = 0
   STATE_START_SONG = 1
   STATE_PLAY_SONG  = 2
   STATE_EOF_SONG   = 3
   STATE_PAUSE_SONG = 4
   STATE_STOPPED    = 9

VAR
    long time
    long tempo_tick
    long sound_pin
    byte cog
    byte driver_state
    long current_song
    byte in_note

    long interval
    long song_ptr
    long index_ptr
    long tempo
    long cur_note
    long cur_oct
    long cur_dur
    long cur_flg
        
    long note_end 
    
    long stack[50]

PUB Start(SoundPin) : Success
    sound_pin := SoundPin
    Stop
    Success := (cog := COGNEW(MainLoop, @stack) +1)
    driver_state := STATE_STARTED
    in_note := false   
        
PUB Stop
  if cog
    cogstop(cog~ - 1)
  driver_state := STATE_STOPPED       

PUB Pause
  driver_state := STATE_PAUSE_SONG
  in_note := false

PUB Play (songNumber) 
  driver_state := STATE_START_SONG
  current_song := songNumber
  in_note := false 

PUB MainLoop 
   repeat
    'if we are in a note, then wait until the end
    'to process the rest of the events
    if in_note
       if cnt - time => note_end
          'turn off sound on music channel
          Synth("A",0)                 

          'indicate we are not in a note
          in_note := false
    else

       case driver_state
         STATE_START_SONG:
           song_ptr := @Music_Set
           if current_song > 0
               index_ptr := @Music_Index_Set
               'set the song pointer advanced to the start of the song (if not the first)
               song_ptr += long[index_ptr][current_song-1]
                   
           tempo := long[song_ptr]
           song_ptr += 4
           SetTempo(tempo)
           driver_state := STATE_PLAY_SONG
           cur_oct := 0
           cur_note := 0
           cur_dur := 0
           cur_flg  := 0
           in_note := false
                   
         STATE_PLAY_SONG:
           case byte[song_ptr][NOTE_VAL_OCT] & %00001111
               5: cur_oct := 8
               6: cur_oct := 4
               7: cur_oct := 2
               8: cur_oct := 1

           cur_flg  := byte[song_ptr][NOTE_DUR_FLG] & %00001111
           
           case byte[song_ptr][NOTE_VAL_OCT] >> 4
               0: cur_note := NOTE_PAUSE
               1: cur_note := NOTE_A
               2: cur_note := NOTE_ASHARP
               3: cur_note := NOTE_B
               4: cur_note := NOTE_C
               5: cur_note := NOTE_CSHARP
               6: cur_note := NOTE_D
               7: cur_note := NOTE_DSHARP
               8: cur_note := NOTE_E
               9: cur_note := NOTE_F
              10: cur_note := NOTE_FSHARP
              11: cur_note := NOTE_G
              12: cur_note := NOTE_GSHARP
                          
           case byte[song_ptr][NOTE_DUR_FLG] >> 4
               0: cur_dur := 128
               1: cur_dur := 64
               2: cur_dur := 32
               3: cur_dur := 16
               4: cur_dur := 8
               5: cur_dur := 4
               6: cur_dur := 2
               7: cur_dur := 1

           PlayNote(cur_note,cur_oct,cur_dur,cur_flg)
           
           'exit if this was the last note
           if cur_flg == 9 
              driver_state := STATE_EOF_SONG
             'turn off sound on music channel
              Synth("A",0)

             'indicate we are not in a note
              in_note := false
           else  
              'otherwise, advance to the next note data block   
              song_ptr += NOTE_DAT_BLOCK           
                   
         STATE_EOF_SONG:
          'tell the driver to play the current song again
           driver_state := STATE_START_SONG
                       
PUB SetTempo(BeatsMinute)
    'calculate milliseconds per whole note from the tempo value
    tempo_tick := (60_000 / beatsMinute) * 4
        
PUB PlayNote(freq1, octave, duration1,flag) | counter
    freq1 := freq1 / octave
    Synth("A",freq1)
    in_note := true

    'if this is a dotted note it is 1.5 in length
    if (flag == 1)
       duration1 += (duration1 / 2)
    counter := tempo_tick / duration1
    
    time := cnt    
    'calculate when the note should end
    note_end := ((clkfreq / 1_000 * counter - 3932) #> 381) 
    
PUB Synth(CTR_AB, Freq1) | s, d, ctr, frq

  Freq1 := Freq1 #> 0 <# 128_000_000     'limit frequency range

  if Freq1 < 500_000                    'if 0 to 499_999 Hz,
    ctr := constant(%00100 << 26)      '..set NCO mode
    s := 1                             '..shift = 1
  else                                 'if 500_000 to 128_000_000 Hz,
    ctr := constant(%00010 << 26)      '..set PLL mode
    d := >|((Freq1 - 1) / 1_000_000)   'determine PLLDIV
    s := 4 - d                         'determine shift
    ctr |= d << 23                     'set PLLDIV

  frq := fraction(Freq1, CLKFREQ, s)    'Compute FRQA/FRQB value
  ctr |= sound_pin                           'set PINA to complete CTRA/CTRB value

  if CTR_AB == "A"
     CTRA := ctr                        'set CTRA
     FRQA := frq                        'set FRQA                   
     DIRA[sound_pin]~~                  'make pin output
     
  if CTR_AB == "B"
     CTRB := ctr                        'set CTRB
     FRQB := frq                        'set FRQB                   
     DIRA[sound_pin]~~                  'make pin output

PRI fraction(a, b, shift) : f

  if shift > 0                         'if shift, pre-shift a or b left
    a <<= shift                        'to maintain significant bits while
  if shift < 0                         'insuring proper result
    b <<= -shift

  repeat 32                            'perform long division of a/b
    f <<= 1
    if a => b
      a -= b
      f++
    a <<= 1
        
DAT 
Music_Set        file "sound.snd"   

Music_Index_Set  file "sndindex.six"             
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
