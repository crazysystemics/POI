#include<stdio.h>
#include<Windows.h>
#include<winsock.h>
#include "UDP.h"
#include <fcntl.h>

void UDP_ServerSocket(int port_no)
{
	WSADATA WinSockData;
	int iWsaStartup;
	int iWSaCleanup;

	int iBind;
	int ireceivefrom;
	int iUDPclientLen = sizeof(UDPClient);
	int icloseSocket;

	int iBufferLen = strlen(Buffer_receive) + 1;
	int iSendto;

	struct timeval read_timeout;
	read_timeout.tv_sec = 0;
	read_timeout.tv_usec = 1000;

	unsigned long int noBlock = 1;

	// step 1 initializing Winsock

	iWsaStartup = WSAStartup(MAKEWORD(2, 2), &WinSockData);
	if (iWsaStartup != 0)
	{
		printf("WSAStartup FUN Failed\n");
	}
	else
	{

	}

	UDPSocketServer = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (UDPSocketServer == INVALID_SOCKET)
	{
		fprintf(stderr, "Could not create socket.\n");
		WSACleanup();
		exit(0);
	}

	setsockopt(UDPSocketServer, SOL_SOCKET, SO_RCVTIMEO, &read_timeout, sizeof(read_timeout));

	// step 2  Fill the UDPClient(Socket ADdress)
	UDPClient.sin_family = AF_INET;
	UDPClient.sin_addr.s_addr = inet_addr("127.0.0.1");
	UDPClient.sin_port = htons(port_no);

	// step 3 Socket creation

	if (UDPSocketServer == INVALID_SOCKET)
	{
		printf("Socket Creation Failed\n");
	}
	else
	{

	}

	// step 4 bind the server
	iBind = bind(UDPSocketServer, (SOCKADDR*)&UDPClient, sizeof(UDPClient));
	if (iBind == SOCKET_ERROR)
	{
		printf("Binding Failed\n");
	}
	else
	{

	}
	int nRet = ioctlsocket(UDPSocketServer, FIONBIO, &noBlock);
	if (nRet == SOCKET_ERROR)
	{
		fprintf(stderr, "Could not make Non blocking name to socket.\n");
	}
}

void UDP_ClientSocket(int port_no)
{
	WSADATA WinSockData;
	int iWsaStartup;
	int iWSaCleanup;

	int iSendto;

	int iBufferLen = strlen(Buffer_send) + 1;
	int iUDPServerLen = sizeof(UDPServer);
	int icloseSocket;

	// step 1

	iWsaStartup = WSAStartup(MAKEWORD(2, 2), &WinSockData);
	if (iWsaStartup != 0)
	{
		printf("WSAStartup FUN Failed\n");
	}
	else
	{

	}

	// step 2

	UDPServer.sin_family = AF_INET;
	UDPServer.sin_addr.s_addr = inet_addr("127.0.0.1");
	UDPServer.sin_port = htons(port_no);

	// step 3

	UDPSocketClient = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (UDPSocketClient == INVALID_SOCKET)
	{
		printf("Socket Creation Failed\n");
	}
	else
	{

	}
}

int UDP_Send(SOCKET UDPSocket, struct sockaddr_in UDPmember, char buff_send[], int length)
{
	int iSendto = sendto(UDPSocket, buff_send, length, MSG_DONTROUTE, (SOCKADDR*)&UDPmember, sizeof(UDPmember));
	if (iSendto == SOCKET_ERROR)
	{
		return iSendto;
	}
	else
	{

	}
}


int UDP_Receive(SOCKET UDPSocket, struct sockaddr_in UDPmember)
{
	int iUDPclientLen = sizeof(UDPmember);
	memcpy(Buffer_receive, buff_rec, sizeof(buff_rec));

	int ireceivefrom = recvfrom(UDPSocket, &Buffer_receive, sizeof(Buffer_receive), 0, (SOCKADDR*)&UDPmember, &iUDPclientLen);
	if (ireceivefrom == SOCKET_ERROR)
	{
		return ireceivefrom;
	}
	else
	{

	}
}

void UDP_CloseSocket(SOCKET UDPSocket)
{
	// step 6

	int icloseSocket = closesocket(UDPSocket);
	if (icloseSocket == SOCKET_ERROR)
	{
		printf("Socket Closing Failed\n");
	}
	else
	{
		printf("Socket Closing Success\n");
	}

	// step 7
	int iWSaCleanup = WSACleanup();
	if (iWSaCleanup == SOCKET_ERROR)
	{
		printf("WSACleanup Failed\n");
	}
	else
	{
		printf("WSACleanup Success\n");
	}
}
