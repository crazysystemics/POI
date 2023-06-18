Layer appl, presnl, sesnl;

//layer's payload is layer's data
//Payload is content-stuff of the Layer
appl.payload = <"c:\hello.txt"> ; 
appl.head=<"appl:ftp">;
appl.tail=<"/appl">

//Layer Packet is  layer[head + payload + tail]
tolower.enqueue(appl.packet)

