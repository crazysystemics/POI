using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

public enum TrackState
{
    ETF_NOT_DEFINED,
    ETF_AGING_IN,
    ETF_STEADY,
    ETF_AGINGOUT,
    DELETED_FROM_ETF,
    ETF_INSERTED,
    ETF_UPDATED,
    ETF_iDELETE,
    ETF_oDELETE,
    ALL
}

enum TrackAction
{
    INSERT, DELETE, UPDATE
}
class QAction
{
    double WindowSize;
    public void incrementFrequencyWindow(ref EmitterTrackRecord etfRecord)
    {
        //for simplicity WindowSize (delta_delta) is 25% of current window (delta)
        //Later we can pick it randomly
        etfRecord.freqTrackWindow += (etfRecord.freqTrackWindow * 0.25);

    }

    public void donotModifyFrequencyWindow()
    {

    }

    public void decrementFrequencyWindow(ref EmitterTrackRecord etfRecord)
    {
        //for simplicity WindowSize (delta_delta) is 25% of current window (delta)
        //Later we can pick it randomly
        etfRecord.freqTrackWindow -= (etfRecord.freqTrackWindow * 0.25);

    }

}

//This is subset of environment variables

public class EtfSnapshot
{
    public TrackState etfState;
  //  public int tracklen;
    public double freqMin;
    public double freqMax;
    public double freqLatest;

    public EtfSnapshot(TrackState etfState, double freqMin, double freqMax, double freqLatest)
    {
        this.etfState = etfState;
        this.freqMin = freqMin;
        this.freqMax = freqMax;
        this.freqLatest = freqLatest;
    }
}
public class QState
{
    public double freq;
    public double maxWindow;

   // public List<double> ercFrequencies = new List<double>();
   // public List<EtfSnapshot> etfSnapshots = new List<EtfSnapshot>();
}

public class QLearner
{
    public double gamma = 0.7;
    public double eeta = 0.3;

    public int count;
    public double runningSum;
    public double runningAverage;

    public double EXPLORE_PROBABILITY = 1.0;
    public int actionSpaceCount = 3;


    public List<QState> qstates = new List<QState>();
    public List<int> actions = new List<int>();
    public List<List<double>> Qsa = new List<List<double>>();

    public QLearner()
    {
        for(int i= 0; i < 5; i++)
        {
            Qsa.Add(new List<double> { 0.0, 0.0, 0.0 });
        }  
    }

    public double QSaMatrixGet(int state_i, int action_j)
    {
        if (state_i > -1)
            return Qsa.ToArray()[state_i].ToArray()[action_j];
        else
            return -1;
    }

    public void QSaMatrixSet(int state_i, int action_j, double Qval)
    {
        if (state_i > -1)
            Qsa.ToArray()[state_i][action_j] = Qval;
        //Qsa.ToArray()[state_i].ToArray()[action_j] = Qval;
    }

    public double QsaGet(QState state, int action_j)
    {
        int state_i = QsaMatch(state);//qstates.IndexOf(state);
        return QSaMatrixGet(state_i, action_j);
    }

    public void QsaSet(QState state, int action_j, double Qval)
    {
        int state_i = QsaMatch(state);//qstates.IndexOf(state);
        QSaMatrixSet(state_i, action_j, Qval);
    }

    public int QsaMatch(QState state)
    {
       // bool isMatch = true;
        int stateIndex = -1;

        foreach (QState qstate in qstates)
        {
            if(qstate.freq == state.freq && qstate.maxWindow == state.maxWindow)
            {
                stateIndex = qstates.IndexOf(qstate);
                return stateIndex;
            }
        }

        return stateIndex;
    }

    public double Qsa_cap(QState state_t, int action_t, QState state_t1, double reward)
    {
        double reward0, reward1, reward2;

        int action_index = new Random().Next(3);

        double maxQsa = 0.0; 
    
        int action_t1 = 0;
        for(action_t1 = 0; action_t1 < 3; action_t1++)
        {
            if (QsaGet(state_t1, action_t1) > maxQsa)
            {
                maxQsa = QsaGet(state_t1, action_t1);
            }
        }
 
        double qsa = reward + gamma * maxQsa;//QsaGet(state, maxaction);
        QsaSet(state_t, action_t, qsa);
        return qsa;
    }
}
