#include<stdio.h>
#include "UDP.h"
#include "IOTask.h"

U32 Compute(U32 a, U32 b)
{
	return a & b;
}

void ofp_var_init()
{
	a = 0.0f;
	b = 0.0f;
	c = 0.0f;
	a1 = 0;
	b1 = 0;
	c1 = 0;
}

void ReadFromGlobalIn()
{
	a1 = GET_BIT(GLOBAL_INPUT_MEMORY[SYSTEM_P_DISCRETE_INDEX], 0);
	b1 = GET_BIT(GLOBAL_INPUT_MEMORY[SYSTEM_Q_DISCRETE_INDEX], 0);
	a = GLOBAL_INPUT_MEMORY[20];
	b = GLOBAL_INPUT_MEMORY[21];
}

void ofp_data_process()
{
	c1 = Compute(a1, b1);
}

void WriteToGlobalOut()
{
	// TODO: eng to raw conversion 
	if (c1 == 1)
	{
		GLOBAL_OUTPUT_MEMORY[0] = SET_BIT(GLOBAL_OUTPUT_MEMORY[0], 0);
		GLOBAL_OUTPUT_MEMORY[0] = SET_BIT(GLOBAL_OUTPUT_MEMORY[0], 1);
		GLOBAL_OUTPUT_MEMORY[0] = SET_BIT(GLOBAL_OUTPUT_MEMORY[0], 2);
	}
	else
		GLOBAL_OUTPUT_MEMORY[0] = CLR_BIT(GLOBAL_OUTPUT_MEMORY[0], 0);

	GLOBAL_OUTPUT_MEMORY[20] = a + b;
	GLOBAL_OUTPUT_MEMORY[40] = GLOBAL_INPUT_MEMORY[40] + GLOBAL_INPUT_MEMORY[41];
	GLOBAL_OUTPUT_MEMORY[60] = GLOBAL_INPUT_MEMORY[60] + GLOBAL_INPUT_MEMORY[61];
}

void ofp_app_task()
{
	ReadFromGlobalIn();
	ofp_data_process();
	WriteToGlobalOut();
}