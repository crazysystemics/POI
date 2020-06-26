#include <windows.h>
#include "ErcExeCmd.h"


void ErcSleep(int time) {
	Sleep(time);
}


void ErcExecutive(Schedule s)
{
	while (TRUE)
	{
		if(s.PreSchedRunProcr!=NULL)
			s.PreSchedRunProcr();

		for (int icb = 0; icb < s.NumCmdBlocks; icb++)
		{
			CmdBlock cb = s.CmdBlocks[icb];
			//cb.PreCmdBlockProcr();
			for (int icunit = 0; icunit < cb.NumCmdUnits; icunit++)
			{
				CmdUnit cu = cb.CmdUnits[icunit];
				if (cu.PreCmdUnitProcr!=NULL)
					cu.PreCmdUnitProcr();

				ExecuteErcCmd(cu.Cmd);

				if (cu.PostCmdUnitProcr!=NULL)
					cu.PostCmdUnitProcr();
			}
			//cb.PostCmdBlockProcr();
		}
		ErcSleep(s.schedInterval);

		if(s.PostSchedRunProcr !=NULL)
			s.PostSchedRunProcr();
	}
}


