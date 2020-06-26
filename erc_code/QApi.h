#define MAX_BUFF 700

extern int QPush(char _dest[][MAX_BUFF], char _src[MAX_BUFF], int _size, int& _front, int& _rear);
extern int QPop(char _dest[MAX_BUFF], char _src[][MAX_BUFF], int &_front, int &_rear);
extern int QCount(char _src[][MAX_BUFF], int &_front, int &_rear);
