#include <stdio.h>

void main()
{
	char* command_line =(char *) "echo hello_world";
	

	//split based on space
	char* argv[10];
    //argv=command_line.split();
	//argv[0]="echo"
	//argv[1]="hello"
	//argv[2]="world"
	if (argv[0] == "echo")
	{
		printf(argv[1]);
	}
}