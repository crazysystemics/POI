Layer appl, presnl, sesnl;

//layer's payload is layer's data
//Payload is content-stuff of the Layer
appl.payload = <"c:\hello.txt"> ; 
appl.head=<"appl:ftp">;
appl.tail=<"/appl">

//Layer Packet is  layer[head + payload + tail]
tolower.enqueue(appl.packet)


//recommending new-age-out
>show params of etf_id 14

pri   : 150us
frerq : 6500MHz
pw    : 25us
agein :  1
ageout : 1

>optimize [etf_id 14, ageout]
....trying to optimize ageout of etf_id 14
....etf_id 14 records are breaking and appearing after 4 cycles
.....making the ageout 5 will unify tracks. Penalty is that when
.....genuinely deleted, it will artificially sustain for 4 cycles more
.....can ageout be updated?

>yes

>setting [etf_id 14] to 5
>set auto_optimize ageout
>automatic optimization on
