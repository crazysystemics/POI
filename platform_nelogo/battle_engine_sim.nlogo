;class definitions
;resolve how to do id assignments to each agent
;establish connection between aircraft, radar and missile
;correct aircraft HAS to be destroyed

extensions [array]

breed        [radars radar]        ;; [agentset member]
breed        [aircrafts aircraft]
breed        [rwrs rwr]
breed        [missiles missile]

radars-own   [id rx ry freq-band r_a_id r_m_id r_radius dist-rt trck_prblty d_rt d_mt]    ;; defines the variables belonging to	each link
aircrafts-own[id ax ay a_r_id a_m_id a_radius rid a_d_rt array:radar-array rwr-freq-band]
rwrs-own     [id poi]
missiles-own [id mx my m_a_id m_r_id mssl_vel missile_assigned mssl_d_rt mssl_d_mt
             mssl_assgn_launch_prblty marcrft_id mssl_on_trk_prblty mssl_on_trk
             blast_dist ac_hit delay]

globals      [monxcor   max_red_xcor max_blue_xcor mssn_strtd dist                                    ;; defines new global variables
              mssn_succ mssn_effctvnss dtct_prblty tracked_ac_count avg_trckd_ac_cnt rmid maxno     ;; they are accessible by all agents and can be used anywhere in a model
              count2 i border_x_cor monair_xcor
              missile-ac-radius ac-count radar-count rwr-radius
              learning array:Qrx eeta array:rxba
              band_a_tick band_b_tick band_c_tick band_d_tick
              band-ptr array:search-regime rl-il-prblty]

to SetUp                        ;; used	to	begin	a	command	procedure
  clear-all                     ;; combines the effects of clear-globals, clear-ticks, clear-turtles, clear-patches, clear-drawing, clear-all-plots, and clear-output

  set mssn_strtd         1      ;; sets variable to the given value
  set mssn_succ          0
  set mssn_effctvnss     0
  set border_x_cor      15
  set missile-ac-radius  5
  set ac-count           9
  set radar-count        9

   ask aircrafts
   [set array:radar-array array:from-list n-values 10 [0]]
   set array:Qrx array:from-list n-values 4 [0]
   set array:search-regime array:from-list n-values 4 [0]


  create-aircrafts ac-count
  [
        set color      blue                                 ;; holds the color of the turtle or link
        set size         2                                  ;; holds a number that is the turtle's apparent size
        setxy min-pxcor  0                                  ;; these reporters give the minimum x-coordinate and minimum y-coordinate, (respectively) for patches, which determines the size of the worl
        set rid          0
        set a_d_rt       0
        set heading     90                                  ;; indicates the direction the turtle is facing
        set rwr-radius  10
        setxy abs random-xcor * -1  random-ycor                       ;; reports a random floating point number from the allowable range of turtle coordinates along the given axis, x or y
        set shape "airplane"                                ;; holds a string that is the name of the turtle or link's current shape
        set id 1                                            ;; holds the turtle's "who number" or ID number
        ;ask aircraft 0 [ create-links-to other aircrafts ] ;; creates a directed link from the caller to agent
  ]

  ;set maxno 1
  ;set count2 0
  ;set rmid 1
  set i 0

  ;while [count2 < maxno]            ;; if reporter reports false, exit the loop, otherwise run commands and repeat
  ;[
    set rmid -1
    let index 0               ;; list index starts at 0
   create-radars radar-count
    [
        set color red
        set size  2
        ;let loc item index [[15 25] [15 41] [0 0]]           ;; get the next loc from the list
        ;setxy (item 0 loc) (item 1 loc)                      ;; loc itself is a list, first item x, second item y
        ;set index (index + 1)
        setxy abs random-xcor mod 15  random-ycor                                                 ;; increment the index
        set dist 0
        set d_rt 0
        set d_mt 0


        set heading      270    ;;315
        set dtct_prblty  0.7
        set trck_prblty  0.7
        set r_radius      10
        set id rmid
        set band
    ]

    let index1 0
    create-missiles radar-count
    [
    ;let loc item index1 [[15 25] [15 41] [0 0]]
    ;setxy (item 0 loc) (item 1 loc)
    ;set index1 (index1 + 1)
    set heading 270
    set mssl_vel 0.0
    set missile_assigned false
    set mssl_assgn_launch_prblty 0.7
    set mssl_on_trk_prblty 0.6
    set mssl_on_trk false
    set blast_dist 3
    set ac_hit false
    set mssl_d_rt 0
    set mssl_d_mt 0
    set delay 0
    set id rmid
    ]

   ;  ask aircrafts
   ; [foreach n-values radar-count [ ndx -> ndx ] [ ndx -> array:set array:radar-array ndx  0]]
   ; let radar-list (list  0 0 0 0 0)



  set i 1
  ask aircrafts
  [
    set array:radar-array array:from-list n-values radar-count [0]
  ]

  foreach n-values 4 [ ndx2 -> ndx2 ] [ ndx2 -> array:set array:Qrx ndx2  0]
  ask aircrafts
  [array:set array:radar-array 0 0]


  set rmid 0
  while [ i < radar-count ]
  [
    ask one-of radars with [id = -1]
    [set id rmid]

    ask radars with [ id = rmid ]
    [ set freq-band rmid mod 4 ]

    ask one-of missiles with [id = -1]
    [set id rmid]

    set i  (i + 1)
    set rmid (rmid + 1)
 ]

 ask radars
 [
    ask missiles with [id = [id] of myself]
    [ setxy [xcor] of myself [ycor] of myself]
 ]



;   set count2 (count2 + 1)
;   set rmid (rmid + 1)
;   ]

   array:set array:search-regime 0 2000
   array:set array:search-regime 1 20
   array:set array:search-regime 2 20
   array:set array:search-regime 3 20

  set tracked_ac_count 0
  set avg_trckd_ac_cnt 0


  reset-ticks                           ;; resets the tick counter to zero, sets up all plots, then updates all plots (so that the initial state of the world is plotted)
end                                     ;; used to conclude a procedure

to dump_var

    print "id of aircrafts"            ;; prints value in the Command Center, followed by a carriage return
    show [id] of aircrafts             ;; prints value in the Command Center, preceded by this agent, and followed by a carriage return

    print "id of radars"
    show [id] of radars

    print "id of missiles"
    show [id] of missiles

    print "a_m_id of aircrafts"
    show [a_m_id] of aircrafts

    print "a_r_id of aircrafts"
    show [a_r_id] of aircrafts

    print "r_a_id of radars"
    show [r_a_id] of radars

    print "r_m_id of radars"
    show [r_m_id] of radars

    print "m_a_id of missiles"
    show [m_a_id] of missiles

    print "m_r_id of missiles"
    show [m_r_id] of missiles

end

to target_hit [rdrid m_d_rt]
  ask missiles
  [ifelse random-float 1.0 < mssl_on_trk_prblty  [set mssl_on_trk true ]  [ set mssl_on_trk false]              ;; reporter must report a boolean (true or false) value
     if mssl_on_trk
     [
      fd 0.76         ;; turtle moves forward by number steps, one step at a time
      display         ;; causes the view to be updated immediately
      ask missiles with [id = rdrid]      ;; specified agent or agentset runs the given commands ;; takes two inputs: on the left, an agentset (usually "turtles" or "patches"). On the right, a boolean reporter
      [
          set delay m_d_rt
      ]
     ]
  ]

end

to-report getry [prid]        ;; used to begin a reporter procedure
  report 0                    ;; immediately exits from the current to-report procedure and reports value as the result of that procedure
end

to-report getrx [prid]
  report 0
end

to-report getay [paid]
  report 0
end

to-report getax [paid]
  report 0
end

to go
  ;using agentsets for selecting turtles with blue
  ask aircrafts [if pxcor = min-pxcor [set mssn_strtd mssn_strtd + 1]]
  ;ask aircrafts [if pxcor = max-pxcor [set mssn_cmpltd mssn_cmpltd + 1]]
  repeat 3 [ ask aircrafts [ fd 0.2 ] display ]                            ;; runs commands number times

  let dotick 1            ;; creates a new local variable and gives it the given value

  ;Radar countering strategy
  ;1. Open up reception window, and receive Radar Signals
  ;2. Launch Anti-Radiation Missile (1-2) per aircraft
  ;3. Launch chaff and flare
  ;ABOVE THREE ARE AIRCRAFT-RADAR NEUTRAL. So if one aircraft takesa action, others should be quiet
  ;4. Active Jamming - Aircraft vs Tracking Radar (Jammming Techniques RGPI, RGPO, VGPI, VGPO)
  ;5. Toy Decoy Launching against specific Missile


  ;RWRs (i.e.. aircrafts) see Radars ###
  ask aircrafts
    [ask radars in-radius rwr-radius
      [if id > 0
        [if freq-band = [rwr-freq-band] of myself [array:set [array:radar-array] of myself  [id] of self 1]]]]

  set i      0
  let Fbmax  0
  let Qmax   0

 ;while [i < radar-count]

  ask aircrafts
   [
     ifelse learning = 0

     ;Routinely increment band to next learning
    [set rwr-freq-band ((rwr-freq-band + 1) mod 4  )]

    ;else part
    [
      ifelse random-float 1.0 < rl-il-prblty
      ;updating based on reinforcement learning
      [

        let rxb [rwr-freq-band] of self
        let num_hits 0
        set i 0

        while [i < radar-count]
        [
          if array:item array:radar-array i =  1 [array:set array:rxba rwr-freq-band ((array:item array:rxba rwr-freq-band) + 1)]
          set i (i + 1)
        ]

        ;Compute value of next tuning
        ;There are four actions possible here => 0, 1, 2, 3
        ;Can be done through Roulette-Wheel method also
        while [i < 4]
        [
          array:set array:Qrx i (array:item array:Qrx i + eeta * (array:item array:rxba i - array:item array:Qrx i))
          if array:item array:Qrx i > Qmax
          [
            set Qmax array:item array:Qrx i
            set Fbmax i
          ]
          set i (i + 1)
        ]

        let Qtotal 0.0
        let array:Qcdf [0.0 0.0 0.0 0.0]
        set i 0
        while [i < 4]
        [
          ; build cdf
          set Qtotal Qtotal + array:item array:Qrx i

        ]


        set i 0
        while [i < 4]
        [
          array:set array:Qcdf i  array:item array:Qrx i / Qtotal
        ]

         let Qrandom 0.0
         set Qrandom random-float 1.0

         set i 0
         let band 0
         while [i < 4]
         [
          if Qrandom < array:item array:Qcdf i
          [
            set band i
            set i 10
          ]
         ]
        set rwr-freq-band band
       ]
       [
        ;Imitation Learning
         set rwr-freq-band band-ptr
         array:set array:search-regime band-ptr (array:item array:search-regime band-ptr - 1)
         if array:item array:search-regime band-ptr  = 0
         [set band-ptr (band-ptr + 1) mod 3]


      ] ;else part
    ]
  ]



  ;Assumption is that if aircraft sees Radar, Radar also will see aircraft
  ;sooner or later. They are in line of sight. So, form clusters or groups
  ;based on that. For each Radar, there will be group.



;  if (learning = 0)
;  [
;  each aircraft w




  ask aircrafts
  [


    let x      -1
    let rdrid  -1
    let msslid -1



    ask radars with [(distance myself < r_radius) and (pxcor > [pxcor] of myself)]       ;; reports the distance from this agent to the given turtle or patch
    [if random-float 1.0 < dtct_prblty
      [set color yellow]
      set dist sqrt ((getay id - ry ) * (getay id - ry ) + (getax id - rx ) * (getax id - rx ))  ;; reports the square root of number

      ;calculates delay for missiles
      target_hit id dist

      set rdrid [id] of self
                                         ;; "self" and "myself" are very different. "self" is simple; it means "me". "myself" means "the agent who asked me to do what I'm doing right now"
      set r_a_id [id] of myself
      set x r_a_id

      ask aircrafts with [id = [id] of myself]
      [set a_r_id [id] of myself]

      ask missiles with [id = [id] of myself]
      [set m_a_id  x
       set msslid id ]
    ]

    set a_r_id rdrid
    set a_m_id msslid

    ;set debug_var (x + 1)

    ask radars with [distance myself >= r_radius or pxcor < [pxcor] of myself]        ;; pxcor is greater than or equal to min-pxcor and less than or equal to max-pxcor;
                                                                                      ;; similarly for pycor and min-pycor and max-pycor
    [set color red]
  ]

  ask radars
  [
    ask aircrafts with [(distance myself < a_radius) and (pxcor < [pxcor] of myself)]
    [if random-float 1.0 < dtct_prblty                     ;; if number is positive, reports a random floating point number greater than or equal to 0 but strictly less than number
      [set color yellow ]                                  ;; if number is negative, reports a random floating point number less than or equal to 0, but strictly greater than number
    ]                                                      ;; if number is zero, the result is always 0
    ask aircrafts with [distance myself >= a_radius or pxcor > [pxcor] of myself  ]
                  [set color blue]
  ]



  ask missiles with [delay > 0]
  [
  if delay > 0
    [set delay delay - 1]
    if delay = 1
    [ ifelse random-float 1.0 < mssl_on_trk_prblty
       [set ac_hit true] [set ac_hit false]
       ;set debug_var a_m_id
       set dotick 0
    ]
  ]


  ;ask aircrafts with [ aircrafts with a_r_id = [a_r_id] of myself ]
  ;[
  ;]

  let radar-band 0
  ask missiles
  [ask aircrafts in-radius missile-ac-radius        ;; reports an agentset that includes only those agents from the original agentset whose distance from the caller is less than or equal to number
    [
      ask one-of radars with [r_a_id = id]
      [set radar-band band]

      ifelse radar-band = rwr-freq-band
      [ set jam-prblty 0.7]
      [ set jam-prblty 0]

      if (a_m_id >= 0) and (any? missiles with [id = [a_m_id] of myself]) and (random-float 1.0 > jam-prblty)     ;; reports true if the given agentset is non-empty, false otherwise
        [die]]]   ;; the turtle or link dies


ask aircrafts with [pxcor > border_x_cor]
  [set mssn_succ (mssn_succ + 1)
    die]

 set max_red_xcor  [xcor] of radars                                             ;; holds the current x coordinate of the turtle
 set max_blue_xcor [xcor] of aircrafts
 set tracked_ac_count tracked_ac_count + count turtles with [color = yellow]   ;; reports the number of agents in the given agentset
 if ticks > 0  [set avg_trckd_ac_cnt tracked_ac_count / ticks]

 set monxcor [pxcor] of aircrafts
 set mssn_effctvnss mssn_succ / mssn_strtd

 if dotick = 1 [tick]
 tick
 end
@#$#@#$#@
GRAPHICS-WINDOW
210
10
647
448
-1
-1
13.0
1
10
1
1
1
0
1
1
1
-16
16
-16
16
1
1
1
ticks
30.0

BUTTON
20
63
83
96
go
go
T
1
T
OBSERVER
NIL
G
NIL
NIL
1

BUTTON
113
64
178
97
SetUp
SetUp
NIL
1
T
OBSERVER
NIL
NIL
NIL
NIL
1

MONITOR
676
10
764
55
NIL
mssn_strtd
17
1
11

MONITOR
785
14
845
59
NIL
monxcor
17
1
11

PLOT
684
70
884
220
range  vs tick
tick
range
-16.0
16.0
-16.0
16.0
true
true
"" ""
PENS
"default" 1.0 0 -16777216 true "" "plot abs max_blue_xcor - max_red_xcor"

MONITOR
870
10
982
55
NIL
tracked_ac_count
17
1
11

MONITOR
1014
10
1126
55
NIL
avg_trckd_ac_cnt
17
1
11

SLIDER
22
134
194
167
dtct-prblty-2
dtct-prblty-2
0
1.0
0.0
0.1
1
NIL
HORIZONTAL

MONITOR
923
70
980
115
NIL
ticks
17
1
11

INPUTBOX
933
154
1162
214
radius
0
1
0
String

MONITOR
726
257
870
302
mission success counter
mssn_succ
17
1
11

MONITOR
723
322
912
367
monair_pxcor
monair_xcor
17
1
11

BUTTON
996
285
1059
318
NIL
NIL
NIL
1
T
OBSERVER
NIL
NIL
NIL
NIL
1

@#$#@#$#@
## WHAT IS IT?

(a general understanding of what the model is trying to show or explain)

## HOW IT WORKS

(what rules the agents use to create the overall behavior of the model)

## HOW TO USE IT

(how to use the model, including a description of each of the items in the Interface tab)

## THINGS TO NOTICE

(suggested things for the user to notice while running the model)

## THINGS TO TRY

(suggested things for the user to try to do (move sliders, switches, etc.) with the model)

## EXTENDING THE MODEL

(suggested things to add or change in the Code tab to make the model more complicated, detailed, accurate, etc.)

## NETLOGO FEATURES

(interesting or unusual features of NetLogo that the model uses, particularly in the Code tab; or where workarounds were needed for missing features)

## RELATED MODELS

(models in the NetLogo Models Library and elsewhere which are of related interest)

## CREDITS AND REFERENCES

(a reference to the model's URL on the web if it has one, as well as any other necessary credits, citations, and links)
@#$#@#$#@
default
true
0
Polygon -7500403 true true 150 5 40 250 150 205 260 250

airplane
true
0
Polygon -7500403 true true 150 0 135 15 120 60 120 105 15 165 15 195 120 180 135 240 105 270 120 285 150 270 180 285 210 270 165 240 180 180 285 195 285 165 180 105 180 60 165 15

arrow
true
0
Polygon -7500403 true true 150 0 0 150 105 150 105 293 195 293 195 150 300 150

box
false
0
Polygon -7500403 true true 150 285 285 225 285 75 150 135
Polygon -7500403 true true 150 135 15 75 150 15 285 75
Polygon -7500403 true true 15 75 15 225 150 285 150 135
Line -16777216 false 150 285 150 135
Line -16777216 false 150 135 15 75
Line -16777216 false 150 135 285 75

bug
true
0
Circle -7500403 true true 96 182 108
Circle -7500403 true true 110 127 80
Circle -7500403 true true 110 75 80
Line -7500403 true 150 100 80 30
Line -7500403 true 150 100 220 30

butterfly
true
0
Polygon -7500403 true true 150 165 209 199 225 225 225 255 195 270 165 255 150 240
Polygon -7500403 true true 150 165 89 198 75 225 75 255 105 270 135 255 150 240
Polygon -7500403 true true 139 148 100 105 55 90 25 90 10 105 10 135 25 180 40 195 85 194 139 163
Polygon -7500403 true true 162 150 200 105 245 90 275 90 290 105 290 135 275 180 260 195 215 195 162 165
Polygon -16777216 true false 150 255 135 225 120 150 135 120 150 105 165 120 180 150 165 225
Circle -16777216 true false 135 90 30
Line -16777216 false 150 105 195 60
Line -16777216 false 150 105 105 60

car
false
0
Polygon -7500403 true true 300 180 279 164 261 144 240 135 226 132 213 106 203 84 185 63 159 50 135 50 75 60 0 150 0 165 0 225 300 225 300 180
Circle -16777216 true false 180 180 90
Circle -16777216 true false 30 180 90
Polygon -16777216 true false 162 80 132 78 134 135 209 135 194 105 189 96 180 89
Circle -7500403 true true 47 195 58
Circle -7500403 true true 195 195 58

circle
false
0
Circle -7500403 true true 0 0 300

circle 2
false
0
Circle -7500403 true true 0 0 300
Circle -16777216 true false 30 30 240

cow
false
0
Polygon -7500403 true true 200 193 197 249 179 249 177 196 166 187 140 189 93 191 78 179 72 211 49 209 48 181 37 149 25 120 25 89 45 72 103 84 179 75 198 76 252 64 272 81 293 103 285 121 255 121 242 118 224 167
Polygon -7500403 true true 73 210 86 251 62 249 48 208
Polygon -7500403 true true 25 114 16 195 9 204 23 213 25 200 39 123

cylinder
false
0
Circle -7500403 true true 0 0 300

dot
false
0
Circle -7500403 true true 90 90 120

face happy
false
0
Circle -7500403 true true 8 8 285
Circle -16777216 true false 60 75 60
Circle -16777216 true false 180 75 60
Polygon -16777216 true false 150 255 90 239 62 213 47 191 67 179 90 203 109 218 150 225 192 218 210 203 227 181 251 194 236 217 212 240

face neutral
false
0
Circle -7500403 true true 8 7 285
Circle -16777216 true false 60 75 60
Circle -16777216 true false 180 75 60
Rectangle -16777216 true false 60 195 240 225

face sad
false
0
Circle -7500403 true true 8 8 285
Circle -16777216 true false 60 75 60
Circle -16777216 true false 180 75 60
Polygon -16777216 true false 150 168 90 184 62 210 47 232 67 244 90 220 109 205 150 198 192 205 210 220 227 242 251 229 236 206 212 183

fish
false
0
Polygon -1 true false 44 131 21 87 15 86 0 120 15 150 0 180 13 214 20 212 45 166
Polygon -1 true false 135 195 119 235 95 218 76 210 46 204 60 165
Polygon -1 true false 75 45 83 77 71 103 86 114 166 78 135 60
Polygon -7500403 true true 30 136 151 77 226 81 280 119 292 146 292 160 287 170 270 195 195 210 151 212 30 166
Circle -16777216 true false 215 106 30

flag
false
0
Rectangle -7500403 true true 60 15 75 300
Polygon -7500403 true true 90 150 270 90 90 30
Line -7500403 true 75 135 90 135
Line -7500403 true 75 45 90 45

flower
false
0
Polygon -10899396 true false 135 120 165 165 180 210 180 240 150 300 165 300 195 240 195 195 165 135
Circle -7500403 true true 85 132 38
Circle -7500403 true true 130 147 38
Circle -7500403 true true 192 85 38
Circle -7500403 true true 85 40 38
Circle -7500403 true true 177 40 38
Circle -7500403 true true 177 132 38
Circle -7500403 true true 70 85 38
Circle -7500403 true true 130 25 38
Circle -7500403 true true 96 51 108
Circle -16777216 true false 113 68 74
Polygon -10899396 true false 189 233 219 188 249 173 279 188 234 218
Polygon -10899396 true false 180 255 150 210 105 210 75 240 135 240

house
false
0
Rectangle -7500403 true true 45 120 255 285
Rectangle -16777216 true false 120 210 180 285
Polygon -7500403 true true 15 120 150 15 285 120
Line -16777216 false 30 120 270 120

leaf
false
0
Polygon -7500403 true true 150 210 135 195 120 210 60 210 30 195 60 180 60 165 15 135 30 120 15 105 40 104 45 90 60 90 90 105 105 120 120 120 105 60 120 60 135 30 150 15 165 30 180 60 195 60 180 120 195 120 210 105 240 90 255 90 263 104 285 105 270 120 285 135 240 165 240 180 270 195 240 210 180 210 165 195
Polygon -7500403 true true 135 195 135 240 120 255 105 255 105 285 135 285 165 240 165 195

line
true
0
Line -7500403 true 150 0 150 300

line half
true
0
Line -7500403 true 150 0 150 150

pentagon
false
0
Polygon -7500403 true true 150 15 15 120 60 285 240 285 285 120

person
false
0
Circle -7500403 true true 110 5 80
Polygon -7500403 true true 105 90 120 195 90 285 105 300 135 300 150 225 165 300 195 300 210 285 180 195 195 90
Rectangle -7500403 true true 127 79 172 94
Polygon -7500403 true true 195 90 240 150 225 180 165 105
Polygon -7500403 true true 105 90 60 150 75 180 135 105

plant
false
0
Rectangle -7500403 true true 135 90 165 300
Polygon -7500403 true true 135 255 90 210 45 195 75 255 135 285
Polygon -7500403 true true 165 255 210 210 255 195 225 255 165 285
Polygon -7500403 true true 135 180 90 135 45 120 75 180 135 210
Polygon -7500403 true true 165 180 165 210 225 180 255 120 210 135
Polygon -7500403 true true 135 105 90 60 45 45 75 105 135 135
Polygon -7500403 true true 165 105 165 135 225 105 255 45 210 60
Polygon -7500403 true true 135 90 120 45 150 15 180 45 165 90

sheep
false
15
Circle -1 true true 203 65 88
Circle -1 true true 70 65 162
Circle -1 true true 150 105 120
Polygon -7500403 true false 218 120 240 165 255 165 278 120
Circle -7500403 true false 214 72 67
Rectangle -1 true true 164 223 179 298
Polygon -1 true true 45 285 30 285 30 240 15 195 45 210
Circle -1 true true 3 83 150
Rectangle -1 true true 65 221 80 296
Polygon -1 true true 195 285 210 285 210 240 240 210 195 210
Polygon -7500403 true false 276 85 285 105 302 99 294 83
Polygon -7500403 true false 219 85 210 105 193 99 201 83

square
false
0
Rectangle -7500403 true true 30 30 270 270

square 2
false
0
Rectangle -7500403 true true 30 30 270 270
Rectangle -16777216 true false 60 60 240 240

star
false
0
Polygon -7500403 true true 151 1 185 108 298 108 207 175 242 282 151 216 59 282 94 175 3 108 116 108

target
false
0
Circle -7500403 true true 0 0 300
Circle -16777216 true false 30 30 240
Circle -7500403 true true 60 60 180
Circle -16777216 true false 90 90 120
Circle -7500403 true true 120 120 60

tree
false
0
Circle -7500403 true true 118 3 94
Rectangle -6459832 true false 120 195 180 300
Circle -7500403 true true 65 21 108
Circle -7500403 true true 116 41 127
Circle -7500403 true true 45 90 120
Circle -7500403 true true 104 74 152

triangle
false
0
Polygon -7500403 true true 150 30 15 255 285 255

triangle 2
false
0
Polygon -7500403 true true 150 30 15 255 285 255
Polygon -16777216 true false 151 99 225 223 75 224

truck
false
0
Rectangle -7500403 true true 4 45 195 187
Polygon -7500403 true true 296 193 296 150 259 134 244 104 208 104 207 194
Rectangle -1 true false 195 60 195 105
Polygon -16777216 true false 238 112 252 141 219 141 218 112
Circle -16777216 true false 234 174 42
Rectangle -7500403 true true 181 185 214 194
Circle -16777216 true false 144 174 42
Circle -16777216 true false 24 174 42
Circle -7500403 false true 24 174 42
Circle -7500403 false true 144 174 42
Circle -7500403 false true 234 174 42

turtle
true
0
Polygon -10899396 true false 215 204 240 233 246 254 228 266 215 252 193 210
Polygon -10899396 true false 195 90 225 75 245 75 260 89 269 108 261 124 240 105 225 105 210 105
Polygon -10899396 true false 105 90 75 75 55 75 40 89 31 108 39 124 60 105 75 105 90 105
Polygon -10899396 true false 132 85 134 64 107 51 108 17 150 2 192 18 192 52 169 65 172 87
Polygon -10899396 true false 85 204 60 233 54 254 72 266 85 252 107 210
Polygon -7500403 true true 119 75 179 75 209 101 224 135 220 225 175 261 128 261 81 224 74 135 88 99

wheel
false
0
Circle -7500403 true true 3 3 294
Circle -16777216 true false 30 30 240
Line -7500403 true 150 285 150 15
Line -7500403 true 15 150 285 150
Circle -7500403 true true 120 120 60
Line -7500403 true 216 40 79 269
Line -7500403 true 40 84 269 221
Line -7500403 true 40 216 269 79
Line -7500403 true 84 40 221 269

wolf
false
0
Polygon -16777216 true false 253 133 245 131 245 133
Polygon -7500403 true true 2 194 13 197 30 191 38 193 38 205 20 226 20 257 27 265 38 266 40 260 31 253 31 230 60 206 68 198 75 209 66 228 65 243 82 261 84 268 100 267 103 261 77 239 79 231 100 207 98 196 119 201 143 202 160 195 166 210 172 213 173 238 167 251 160 248 154 265 169 264 178 247 186 240 198 260 200 271 217 271 219 262 207 258 195 230 192 198 210 184 227 164 242 144 259 145 284 151 277 141 293 140 299 134 297 127 273 119 270 105
Polygon -7500403 true true -1 195 14 180 36 166 40 153 53 140 82 131 134 133 159 126 188 115 227 108 236 102 238 98 268 86 269 92 281 87 269 103 269 113

x
false
0
Polygon -7500403 true true 270 75 225 30 30 225 75 270
Polygon -7500403 true true 30 75 75 30 270 225 225 270
@#$#@#$#@
NetLogo 6.1.0
@#$#@#$#@
@#$#@#$#@
@#$#@#$#@
@#$#@#$#@
@#$#@#$#@
default
0.0
-0.2 0 0.0 1.0
0.0 1 1.0 0.0
0.2 0 0.0 1.0
link direction
true
0
Line -7500403 true 150 150 90 180
Line -7500403 true 150 150 210 180
@#$#@#$#@
0
@#$#@#$#@
