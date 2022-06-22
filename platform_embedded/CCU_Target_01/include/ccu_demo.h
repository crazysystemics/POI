#pragma once


#ifndef QUEUE


#define NUMBER_OF_INPUT_CHANNELS 3
#define NUMBER_OF_OUTPUT_CHANNELS 3

enum Interface {
	RS422, ARINC
};

int InputChannelInterfaceMap[3] = { RS422, ARINC, RS422 };

//Input Queues
//Queue q_spu_symbols, q_spj_symbols, q_du_pushbuttons;
extern int *new_data_flags;

//Input Queue Array
//Queue* ccu_input_queues[NUMBER_OF_INPUT_CHANNELS];
//Queue* ccu_output_queues[NUMBER_OF_OUTPUT_CHANNELS];
//=====================TO BE FILLED================
//OutputChannelInterfaceMap
//Output Queues
//Output Queue Array

int OutputChannelInterfaceMap[3] = { RS422, ARINC, RS422 };

//Output Queues
//Queue q_spu_symbols, q_spj_symbols, q_du_pushbuttons;
extern int *space_data_flags;

//Output Queue Array
//Queue* ccu_output_queues[NUMBER_OF_OUTPUT_CHANNELS];

int input_output_map[NUMBER_OF_INPUT_CHANNELS];

void init_input_output_map();

void init_ccu_input_queues();
void init_ccu_output_queues();

void input_foreground_task();
void output_foreground_task();
void background_task();

#define QUEUE
#endif


