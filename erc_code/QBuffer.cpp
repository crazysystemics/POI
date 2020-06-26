#define DEBUG 1
# define QMAX 20
#define MAX_BUFF 700


#include<stdio.h>
#include<string.h>
#include "QApi.h"


int QPush(char _dest[][MAX_BUFF], char _src[MAX_BUFF], int _size, int &_front, int &_rear)
{
	if ((_front == 0 && _rear == QMAX - 1) || (_front == _rear + 1))
	{
#if DEBUG
		printf("Queue Overflow n\n");
#endif
		return 1;
	}
	if (_front == -1)
	{
		_front = 0;
		_rear = 0;
	}
	else
	{
		if (_rear == QMAX - 1)
			_rear = 0;
		else
			_rear = _rear + 1;
	}
	memcpy(&_dest[_rear], _src, _size);
	return 0;
}

int QPop(char _dest[MAX_BUFF], char _src[][MAX_BUFF], int &_front, int &_rear)
{
	if (_front == -1)
	{
#if DEBUG
		printf("Queue Underflown\n");
#endif
		return 1;
	}
	memcpy(_dest, &_src[_front], sizeof(_src[_front]));
	//memcpy(&_dest, &_src,_size);
#if DEBUG
	printf("Element deleted from queue is : %s\n", _src[_front]);
#endif
	if (_front == _rear)
	{
		_front = -1;
		_rear = -1;
	}
	else
	{
		if (_front == QMAX - 1)
			_front = 0;
		else
			_front = _front + 1;
	}
	return 0;
}

int QCount(char _src[][MAX_BUFF], int &_front, int &_rear)
{
	int _front_pos = _front, _rear_pos = _rear;
	if (_front == -1)
	{
#if DEBUG
		printf("Queue is emptyn\n");
#endif
		return 0;
	}
#if DEBUG
	printf("Queue elements :n\n");
#endif
	if (_front_pos <= _rear_pos)
		while (_front_pos <= _rear_pos)
		{
#if DEBUG
			printf("%s \n", _src[_front_pos]);
#endif
			_front_pos++;
		}
	else
	{
		while (_front_pos <= QMAX - 1)
		{
#if DEBUG
			printf("%s \n", _src[_front_pos]);
#endif
			_front_pos++;
		}
		_front_pos = 0;
		while (_front_pos <= _rear_pos)
		{
#if DEBUG
			printf("%s \n", _src[_front_pos]);
#endif
			_front_pos++;
		}
	}
	printf("No of CMD - %d\n", _front_pos);

	return _front_pos;
}