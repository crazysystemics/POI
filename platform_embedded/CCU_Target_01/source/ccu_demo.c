

#pragma once
#include "queue.h"
#include "ccu_demo.h"



#define NUMBER_OF_INPUT_CHANNELS 3
#define NUMBER_OF_OUTPUT_CHANNELS 3
//
//enum Interface {
//	RS422, ARINC
//};
//
//int InputChannelInterfaceMap[3] = { RS422, ARINC, RS422 };
//
////Input Queues
//Queue q_spu_symbols, q_spj_symbols, q_du_pushbuttons;
//extern int* new_data_flags;
//
////Input Queue Array
//Queue* ccu_input_queues[NUMBER_OF_INPUT_CHANNELS];
//Queue* ccu_output_queues[NUMBER_OF_OUTPUT_CHANNELS];
////=====================TO BE FILLED================
////OutputChannelInterfaceMap
////Output Queues
////Output Queue Array
//Queue* ccu_output_queues;
//
//int input_output_map[NUMBER_OF_INPUT_CHANNELS];
//int OutputChannelInterfaceMap[3] = { RS422, ARINC, RS422 };

void init_input_output_map()
{
	input_output_map[0] = 1;// spu symbols, goes to du
	input_output_map[1] = 1;// spj to du
	input_output_map[2] = 2;//spu
	input_output_map[3] = 3; //spj
}

//void init_ccu_input_queues()
//{
//	ccu_input_queues[0] = &q_spu_symbols;
//	ccu_input_queues[1] = &q_spj_symbols;
//	ccu_input_queues[2] = &q_du_pushbuttons;
//}

//void init_ccu_output_queues()
//{
//	ccu_output_queues[0] = &q_spu_symbols;
//	ccu_output_queues[1] = &q_spj_symbols;
//	ccu_output_queues[2] = &q_du_pushbuttons;

//}

void input_foreground_task()
{
	//////for (int index = 0; index < NUMBER_OF_INPUT_CHANNELS; index++)
	//////{
	//////	if (new_data_flags[index])
	//////	{
	//////		if (InputChannelInterfaceMap[index] == RS422)
	//////		{
	//////			int x;
	//////			int status = read_data_from_rs422(Port0, &x);

	//////		

	//////			if (status == 0)
	//////			{
	//////				status = Enqueue(ccu_input_queues[index], x);

	//////				if (status < 0)
	//////				{
	//////					//ERROR
	//////				}

	//////			}
	//////		}
	//////		else if (InputChannelInterfaceMap[index] == ARINC)
	//////		{
	//////			int x;
	//////			int status = ARINC_Read(Port0, &x);

	//////			

	//////			if (status == 0)
	//////			{
	//////				status = Enqueue(ccu_input_queues[index], x);

	//////				if (status < 0)
	//////				{
	//////					//ERROR
	//////				}

	//////			}

	//////		}
	//////	}
	//////}
}

void output_foreground_task();


void background_task()
{
	//int prev_time = GetTime();
	//int cur_time;
	//int input_task_overrun, output_task_overrun;
	//int background_task_overrun;

	//input_task_overrun = output_task_overrun =
	//	background_task_overrun = 0;

	//input_foreground_task();

	//cur_time = GetTime();
	//if (cur_time - prev_time > INPUT TASK_TIME)
	//{
	//	input_task_overrun = 1;

	//}
	//prev_time = cur_time;

	////background task
	//for (int index = 0; index < NUMBER_OF_INPUT_CHANNELS; index++)
	//{
	//	int output_pq = input_output_map[index];
	//	int status;


	//	int x = 0;

	//	if (GetLength(ccu_input_queues[index]) > 0)
	//	{
	//		status = Dequeue(ccu_input_queues[index], &x);

	//		if (status == 0)
	//		{
	//			Enqueue(ccu_output_queues[output_pq], x);
	//		}
	//	}

	//	cur_time = GetTime();
	//	if (cur_time - prev_time > BACKGROUND_TASK_TIME)
	//	{
	//		background_task_overrun = 1;

	//	}
	//	prev_time = cur_time;


	//	output_foreground_task();

	//	cur_time = GetTime();
	//	if (cur_time - prev_time > OUTPUT_TASK_TIME)
	//	{
	//		output_task_overrun = 1;

	//	}
	//	prev_time = cur_time;

	//	if (input_task_overrun || background_task_overrun || output_task_overrun)
	//	{
	//		//blink some LED
	//	}

	//}
}
//OUTPUT_FOREGROUND_TASK
void output_foreground_task()
{
	//for (int index = 0; index < NUMBER_OF_OUTPUT_CHANNELS; index++)
	//{
		//if(space_available_flags[index])
		//{

		//	if (OutputChannelInterfaceMap[index] == RS422)
		//	{	
	//			
		//			status = Dequeue(ccu_output_queues[index], &x);

		//			if (status < 0)
		//			{
		//				//ERROR
		//			}

		//			status = RS422_send(PORTX, x);

		//			if (status < 0)
		//			{
		//				printf("error after RS422 Send for Symbol Packet to DU");
		//			}
		//		
		//	}
		//	else if (OutputChannelInterfaceMap[index] == ARINC)
		//	{		
		//			//status = Dequeue(ccu_output_queues[index], &x);

		//			//if (status < 0)
		//			//{
		//			//	//ERROR
		//			//}

		//			//status = ARINC_send(PORTX, x);

		//			//if (status < 0)
		//			//{
		//			//	printf("error after ARINC Send for Symbol Packet to SPJ");
		//			//}


	}






