using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWR_POC_GUI
{
    public class SimpleRadar : Radar
    {
        public SimpleRadar(Position position, int radius, int beamWidth, int txTick, int id, int startFrameAzimuth = 135, int endFrameAzimuth = 45)
        {
            this.pulseRepetitionInterval = Globals.randomNumberGenerator.Next(900, 1400);
            int pulseWidth = (Globals.randomNumberGenerator.Next(10, 15) * pulseRepetitionInterval) / 100;
            this.txPulse = new Pulse(pulseWidth, 10, Globals.randomNumberGenerator.Next(3500, 4000), 0, 0);
            this.activePulse = txPulse;
            this.rPos = position;
            this.position = position;
            this.id = id;
            this.radius = radius;
            this.txTick = txTick;
            this.effectiveRadiatedPower = 128;
            this.radarType = Globals.RadarTypes.SIMPLE;
            this.beamWidth = beamWidth;
            this.endToEndDuration = 1;
        }

        public override bool beamContains(Position targetPosition)
        {

            // Since Simple Radar has no main beam, i.e. it has a roughly spherical/hemispherical scan
            // beamContains only checks for aircrafts within its scan range/radius.

            int dist = PhysicalSimulationEngine.GetDistance(rPos, targetPosition);
            if(dist < radius)
                return true;
            else 
                return false;
        }

        public override List<Pulse> GeneratePulseTrain(int startTime, double angle)
        {
            return null;
        }

        public override OutParameter Get()
        {
            Out radarOutput = new Out(txPulse, position, txTick, 1);
            return radarOutput;
        }

        public override void OnTick()
        {
            
        }

        public override void Set(List<InParameter> inParameters)
        {
            
        }
    }
}
