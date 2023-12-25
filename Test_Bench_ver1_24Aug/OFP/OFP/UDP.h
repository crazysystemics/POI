#pragma once
#include<Windows.h>
#include<winsock.h>

char Buffer_receive[20];
char Buffer_send[30];

char buff_rec[20];

SOCKET UDPSocketServer;
struct sockaddr_in UDPClient;

SOCKET UDPSocketClient;
struct sockaddr_in UDPServer;

typedef struct UDP
{
	int port_no;
	char Buffer_receive[20];
	char Buffer_send[20];

	SOCKET UDPSocketServer;
	struct sockaddr_in UDPClient;

	SOCKET UDPSocketClient;
	struct sockaddr_in UDPServer;
};

int UDP_Receive(SOCKET UDPSocket, struct sockaddr_in UDPmember);
int UDP_Send(SOCKET UDPSocket, struct sockaddr_in UDPmember, char buff_send[], int length);
void UDP_ServerSocket(int port_no);
void UDP_ClientSocket(int port_no);
void UDP_CloseSocket(SOCKET UDPSocket);
