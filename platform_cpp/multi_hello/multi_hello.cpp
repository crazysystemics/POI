#include <stdio.h>
#define INDIA
#define INDIA_HELLO  "namaste2"
#define GLOBAL_HELLO "hello"



int main()
{
#ifdef INDIA
	printf(INDIA_HELLO);
#else
	printf(GLOBAL_HELLO);
#endif

	printf(" world\n");

	return 0;

}