#include<stdio.h>
#include <string.h>
#include "UDP.h"
#include "IOTask.h"

void ofp_executive()
{
	ofp_var_init();
	while (TRUE)
	{
		tick = GetTick();
		if (GetSlot(IOTASKIN_OFFSET))			// Slot 1 - IO task - in
		{
			input_task();
		}
		if (GetSlot(OFP_OFFSET))				// Slot 2 - ofp
		{
			ofp_app_task();
		}
		if (GetSlot(IOTASKOUT_OFFSET))			// Slot 3 - IO task - out
		{
			output_task();
		}
	}
}

void main()
{
	Ethernet_init();
	HW_init();
	GetIOTaskMapping();
	ofp_executive();
}

//TODO : Comments
// 1. rx_arinc()		- To map received labels with global_in    [x] 
// 2. rx_discrete()		- To integrate global_in index and bit numbers with icd (should consider offset)
// 3. rx_milbus()		- To change while(k<32) and increment k, integrate subaddress and word count with icd
// 4. tx_discrete()		- To unpack the bits and store it in send_buffer
// 5. tx_milbus()		- To integrate different parameters with icd
// 6. WriteToGlobalOut	- eng to raw conversion 
// 7. ReadFromGlobalIn	- raw to eng conversion 
