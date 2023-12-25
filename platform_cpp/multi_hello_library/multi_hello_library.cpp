#include <stdio.h>

#define INDIA 0
#define WORLD 1

void greet(unsigned int country)
{
	if (country == INDIA)
	{
		printf("namaste world");
	}
	else
	{
		printf("hello world");
	}
}
//
int main()
{
	greet(INDIA);
	printf("\n");
	greet(WORLD);
}