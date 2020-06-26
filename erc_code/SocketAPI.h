#define MAX_BUFF 700

extern int EthernetCreateSocket();
extern int EthernetListen(char* ip, int port);
extern int EthernetConnect();
extern int Ethernetsend(char* data,int datasize);
extern int EthernetReceive(char Data[MAX_BUFF]);
extern int EthernetClose();
//extern int aa(char* RcvDataBuffer);
//extern void PacketParser(char* Data)

//extern void Erc_Executive(Schedule s, char* data)


//variables
extern bool CheckRcdsNdf;
extern char RecvDataBuffer[700];
//extern int iRecvBuffer;



