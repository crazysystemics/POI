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
    ETF_DELETED,
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
    public List<double> ercFrequencies = new List<double>();
    public List<EtfSnapshot> etfSnapshots = new List<EtfSnapshot>();
}

public class QLearner
{
    public double gamma = 0.7;
    public double eeta = 0.3;

    public int count;
    public double runningSum;
    public double runningAverage;

    public List<QState> qstates = new List<QState>();
    public List<int> actions = new List<int>();
    public List<List<double>> Qsa = new List<List<double>>();

    public double QSaMatrixGet(int state_i, int action_j)
    {
        return Qsa.ToArray()[action_j].ToArray()[state_i];
    }

    public void QSaMatrixSet(int state_i, int action_j, double Qval)
    {
        Qsa.ToArray()[action_j].ToArray()[state_i] = Qval;
    }

    public double QsaGet(QState state, int action_j)
    {
        int state_i = qstates.IndexOf(state);
        return QSaMatrixGet(state_i, action_j);
    }

    public void QsaSet(QState state, int action_j, double Qval)
    {
        int state_i = qstates.IndexOf(state);
        QSaMatrixSet(state_i, action_j, Qval);
    }



    public double Qsa_cap(QState state,  double reward)
    {
        double reward0, reward1, reward2;

        int action_index = new Random().Next(3);

        //if (action_index == 0)
        //{

        //}

        //if (action_index == 1)
        //{

        //}

        //if (action_index == 2)
        //{

        //}
        int si;
        foreach (QState qs in qstates)
        {
            if(qs == state)
            {
                si = qstates.IndexOf(qs);
                break;
            }
        }

        double maxQsa;
        int maxaction;//action corresponding max reward


        reward0 = QsaGet(state, 0);
        maxQsa = reward0; maxaction = 0;


        if (QsaGet(state, 1) > maxQsa)
        {
            maxQsa = QsaGet(state, 1); maxaction = 1;
        };


        if (QsaGet(state, 2) > maxQsa)
        {
            maxQsa = QsaGet(state, 2); maxaction = 2;
        };




        double qsa = reward + gamma * maxQsa;//QsaGet(state, maxaction);
        QsaSet(state, maxaction, qsa);
        return qsa;
    }
}