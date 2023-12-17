#include <stdio.h>
#include "Port.h"

float read(struct Channel localChannel)
{
	if (localChannel.localPort.inbuf == 0)
		return 0;
	else
		return localChannel.localPort.inbuf;
}

struct Port write(struct Channel* localChannel, float val, int pdelay)
{
	if (localChannel->localPort.outbuf == 0.0f)
	{
		localChannel->localPort.outbuf = val;
		localChannel->localPort.propagation_delay = pdelay;
	}
	return localChannel->localPort;
}

struct Channel ClockTick(struct Channel localChannel, int propagationDelay)
{
	//TODO: Implement a delay queue to accommodate
	//bytes written before reading. Right now assumption is that
	//no new data is written till current data is read.

	propagationDelay = 0;
	if (propagationDelay > 0)
		localChannel.localPort.propagation_delay--;

	if (localChannel.localPort.propagation_delay == 0 && localChannel.localPort.outbuf != 0)
	{
		localChannel.remotePort.inbuf = localChannel.localPort.outbuf;
		localChannel.localPort.outbuf = 0;
	}
	return localChannel;
}

void Connect(struct Channel* localChannel, struct Channel* remoteChannel, unsigned int propagation_delay)
{
	propagation_delay = 0;
	localChannel->remotePort = remoteChannel->localPort;
	remoteChannel->remotePort = localChannel->localPort;
	localChannel->localPort.propagation_delay = (int)propagation_delay;

	//return localChannel;
}
