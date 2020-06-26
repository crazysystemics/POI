#define MAX_BUFF 700

# include <stdio.h> 
# include <string.h> 
#include <stdbool.h>
#include <fstream>

#include "AxiAPI.h"

#pragma warning(suppress : 4996)

int AxiWrite(char *path, char *data, int _buffSize) {
    // Declare the file pointer 
    FILE* filePointer;
    //"E:\\tcpip\\WriteFlag.dat";

    // Open the existing file GfgTest.c using fopen() 
    // in write mode using "w" attribute 
    while (1) {
        filePointer = fopen(path, "w");
        if (filePointer != NULL)
            break;
    }
    // Check if this filePointer is null 
    // which maybe if the file does not exist 
    if (filePointer == NULL)
    {
        //printf("readfile.txt file failed to open.");
    }
    else
    {

        if (strlen(data) > 0)
        {
            
            fwrite(data, _buffSize, 1, filePointer);
            if (fwrite != 0)
                printf("contents to channel written successfully !\n");
            else
                printf("error writing channel !\n");
        }

        //fgets(dataToBeWritten, sizeof(struct CmdUnit), filePointer);
        //fread(&cmd1, sizeof(struct CmdUnit), 1, filePointer);
        //fread(&cmd, sizeof(struct CmdUnit), 1, filePointer);
        

        
        fclose(filePointer);
        printf("The channel is now closed.");
    }
    return 0;
}

int AxiRead(char* _path,char _dataBuff[MAX_BUFF],int _size) {
    // Declare the file pointer 
    FILE* filePointer;
    
    while (1) {

        filePointer = fopen(_path, "r");
        if (filePointer != NULL)
            break;

    }
    if (filePointer == NULL)
    {
        //printf("readfile.txt file failed to open.");
    }
    else
    {
        fread(_dataBuff, _size,1, filePointer);
        //if (_dataBuff[0] == 1) {
        //    return 1;
        //   /* printf("successfulle");
        //    _RecvAxiBuff[0] = 0;
        //    fputs(_RecvAxiBuff, filePointer);*/
        //}
        //read structure
        //fread(&tb, sizeof(struct TrackBeamRequest), 1, filePointer);
        //fread(&cmd, sizeof(struct CmdUnit), 1, filePointer);

        fclose(filePointer);
        printf("The channel is now closed.");
    }
    return 0;
}
