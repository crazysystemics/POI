#define MAX_BUFF 700

#include<stdio.h>
#include<WinSock2.h>
#include<iostream>
#include "SocketAPI.h"

//#include "Rcds.h"
#pragma warning(disable : 4996)


#ifndef _GlobelVeriable

#define _GlobelVeriable


 WSADATA   Winsockdata;
 int       iWsaStartup;
 int       iWsaCleanup;
 
 SOCKET   TCPServerSocket;
 int      iCloseSocket;
 
 struct  sockaddr_in  TCPServerAdd;
 struct  sockaddr_in  TCPClientAdd;
 int     iTCPClientAdd = sizeof(TCPClientAdd);
 
 int iBind;
 
 int iListen;
 
 SOCKET sAcceptSocket;
 
 int   iSend;
 char  SenderBuffer[MAX_BUFF] = "";
 int   iSenderBuffer = strlen(SenderBuffer) + 1;
 int LenReciveBuffer = 0;
 char RecvDataBuffer[MAX_BUFF];
 u_long iMode = 1;
#endif

int EthernetCreateSocket() {
    // STEP -1 WSAStartUp Fun
    iWsaStartup = WSAStartup(MAKEWORD(2, 2), &Winsockdata);
    if (iWsaStartup != 0)
    {
        printf("WSAStartUp Failed\n");
        return WSAGetLastError();
    }
    printf("WSAStartUp Success\n");

    //STEP -2 Socket Creation
    TCPServerSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (TCPServerSocket == INVALID_SOCKET)
    {
        printf("TCP Server Socket Creation Failed \n");
        return WSAGetLastError();
    }

    //non-blocking socket
    iWsaStartup = ioctlsocket(TCPServerSocket, FIONBIO, &iMode);
    if (iWsaStartup != NO_ERROR) {
        printf("ioctlsocket failed with error: %d\n", WSAGetLastError());
        return WSAGetLastError();
    }
    printf("TCP Server Socket Creation Success \n");
    return 0;
}

int EthernetListen(char* ip, int port) {
    // STEP-3 Fill the Structure
    TCPServerAdd.sin_family = AF_INET;
    TCPServerAdd.sin_addr.s_addr = inet_addr(ip);
    TCPServerAdd.sin_port = htons(port);

    //STEP -4 bing fun
    iBind = bind(
        TCPServerSocket,
        (SOCKADDR*)&TCPServerAdd,
        sizeof(TCPServerAdd));
    if (iBind == SOCKET_ERROR)
    {
        printf("Binding Failed \n");
        return WSAGetLastError();
    }
    printf("Binding Success \n");

    //STEP-5 Listen fun
    iListen = listen(TCPServerSocket, 2);
    if (iListen == SOCKET_ERROR)
    {
        printf("Listen Fun Failed \n");
        return WSAGetLastError();
    }
    printf("Listen Fun Success \n");
    return 0;
}

inline int EthernetConnect() {
    // STEP-6 Accept
    sAcceptSocket = accept(
        TCPServerSocket,
        (SOCKADDR*)&TCPClientAdd,
        &iTCPClientAdd);
    if (sAcceptSocket == INVALID_SOCKET)
    {
        printf( "Accept Failed \n" );
        return WSAGetLastError() ;
    }
    printf( "Connection Accepted \n");
    WSAGetLastError();

    
    return 0;
}

int Ethernetsend(char* data,int datasize) {
    // STEP-7 Send Data to Client
    memcpy(&SenderBuffer, data, datasize);

    iSend = send(sAcceptSocket, SenderBuffer, datasize, 0);
    if (iSend == SOCKET_ERROR)
    {
       printf( "Sending Failed  \n " );
       return WSAGetLastError() ;
    }

    printf("Data Sent Success\n ");
    return 0;
}



int EthernetReceive(char _buffer[MAX_BUFF]) {
    int  iRecv;
    char RecvBuffer[MAX_BUFF];
    int  iRecvBuffer = strlen(RecvBuffer) + 1;

    iRecv = recv(sAcceptSocket, RecvBuffer, iRecvBuffer, 0);
   
    if (iRecv == SOCKET_ERROR)
    {
        printf( "Receive Data Failed " );
        return WSAGetLastError() ;
       
    }
    else if (iRecv > 0) {
        memcpy(_buffer, RecvBuffer,sizeof(RecvBuffer));
        return 0;
    }
    else if (iRecv == 0) {
        //return 1;
        itoa(1, RecvDataBuffer, 10);
    }
    printf( "\nDATA RECEIVED ->  " ); 


    return 1;
}
int EthernetClose() {
    iCloseSocket = closesocket(TCPServerSocket);
    if (iCloseSocket == SOCKET_ERROR)
    {
        printf( "Closing Socket Failed ");
        return WSAGetLastError() ;
    }
    printf( "Closing Socket Success" );
    // STEP-10 CleanUp from DLL
    iWsaCleanup = WSACleanup();
    if (iWsaCleanup == SOCKET_ERROR)
    {
        printf("CleanUp Fun Failed " );
        return WSAGetLastError();
    }
    printf ("CleanUp Fun Success" );
    return 0;
}
