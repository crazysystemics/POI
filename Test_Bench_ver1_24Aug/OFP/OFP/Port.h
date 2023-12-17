#pragma once

typedef struct Port
{
	int id;
	int propagation_delay;
	float inbuf;
	float outbuf;
};

typedef struct Channel
{
	struct Port localPort;
	struct Port remotePort;
};

typedef struct AnalogChannel
{
	struct Channel channel;
	float minVolt, maxVolt;
	float minValidVolt, maxValidVolt;
};

typedef struct DiscreteChannel
{
	struct Channel channel;
	int level;
	float min1v, max1v, min0v, max0v;
	float minVolt, maxVolt;
};

float read(struct Channel localChannel);
struct Port write(struct Channel* localChannel, float val, int pdelay);
struct Channel ClockTick(struct Channel localChannel, int propagationDelay);
void Connect(struct Channel* localChannel, struct Channel* remoteChannel, unsigned int propagation_delay);
