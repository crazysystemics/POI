#define MAX_PARAM 10
#define MAX_CMDS 10
#define MAX_CMD_BLOCKS 10
#define MAX_MSG 200

#include "Enum.h"

typedef void (*PRE_SCHED_RUN_FUNC_PTR)();
typedef void (*POST_SCHED_RUN_FUNC_PTR)();
typedef void (*PRE_CMD_UNIT_FUNC_PTR)();
typedef void (*POST_CMD_RUN_FUNC_PTR)();
typedef void (*PRE_CMDBLK_FUNC_PTR)();
typedef void (*POST_CMDBLK_FUNC_PTR)();

//PreFunPtr, CMD, Vsource, Source, PostFunPtr
typedef struct CmdUnit
{
	PRE_CMD_UNIT_FUNC_PTR PreCmdUnitProcr;
	Command  Cmd;
	SubSystem VSource;

	/*unsigned int NoOfParam;
	unsigned char Param[MAX_PARAM];*/
	//char Message[MAX_MSG];

   //Sub-System Area
	SubSystem Src;
	//SubSystem Dest;
	/*Address FromSrcReadAddr, ToSrcReadAddr;
	Address FromSrcWriteAddr, ToSrcWriteAddr;

	Address FromDestReadAddr, ToDestReadAddr;
	Address FromDestWriteAddr, ToDestWriteAddr;
	unsigned int srcReadLen, srcWriteLen, destReadLen, destWriteLen;*/

	POST_CMD_RUN_FUNC_PTR PostCmdUnitProcr;
};

typedef struct CmdBlock
{
	//PRE_CMDBLK_FUNC_PTR  PreCmdBlockProcr;
	unsigned int NumCmdUnits;
	CmdUnit CmdUnits[MAX_CMDS];
	//POST_CMDBLK_FUNC_PTR PostCmdBlockProcr;
};

//PreFunPoint, NumCmdBlk, CmdBlk, Interval, PostFunPointer
typedef struct Schedule
{
	PRE_SCHED_RUN_FUNC_PTR  PreSchedRunProcr;
	unsigned int NumCmdBlocks;
	CmdBlock CmdBlocks[MAX_CMD_BLOCKS];
	unsigned int schedInterval;
    POST_SCHED_RUN_FUNC_PTR PostSchedRunProcr;
};