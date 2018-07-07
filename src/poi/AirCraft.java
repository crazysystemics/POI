/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package poi;

import java.util.Random;


/**
 *
 * @author srbr
 */
public class AirCraft {
    
    int aircraftId;
    CartesianLocation AInitial;
    CartesianLocation  AEnd;
    
    double velocity;
    CartesianLocation ACurrentLoc;
    java.util.ArrayList<Radar> radarList;
    int jammingRadarId;
    double jammingProbability;
    radarWarningReceiver RWR;
    int RWRHits;
    int globalTicks;
    
    int bands[];
    int bandDuration[];
    int bandTimer[];
    int CURRENTBAND;
    int NEXTBAND;
    
    
    
    
    public AirCraft(int aId, CartesianLocation Ai, CartesianLocation Ae, double vel) {
                        
        aircraftId = aId;
        AInitial = Ai;
        AEnd = Ae;
        ACurrentLoc = Ai;
        assert (ACurrentLoc.getX() > 0):"Aircraft not in first cordant";
        assert (ACurrentLoc.getY() > 0):"Aircraft not in first cordant";
        assert (ACurrentLoc.getZ() > 0):"Aircraft not in first cordant";
        
        radarList = new java.util.ArrayList<Radar>();
        jammingRadarId = -1;
        jammingProbability = 0.7;
        
        velocity = vel;
        
        //
        // Tuning radarWarningReceiver to band 1 during instantiation
        //
        RWR = new radarWarningReceiver(1);
        RWRHits = 0;
                
        CURRENTBAND = 0;
        NEXTBAND = 1;
        bands = new int[2];
        bandDuration = new int[2];
        bandTimer = new int[2];
        
        int i;
        for (i = 0; i < 2; i++) {
            bands[i] = 0;
            bandTimer[i] = 0;
            bandDuration[i] = 0;
        }
        
        
    }
    
    public void setRWRBand(int RWRBand) {
        RWR.setBand(RWRBand);
    }
    
    public int getRWRBand() {
        return (RWR.getBand());
    }
    
    public void incRWRHits() {
        RWRHits = RWRHits + 1;
    }
    
    public int getRWRHits(){
        return RWRHits;
    }
    
    public void changeInitial (CartesianLocation aI) {
        AInitial = aI;
        ACurrentLoc = AInitial;
    }
    
    public void changeEnd (CartesianLocation aE) {
        AEnd = aE;
    }
    
    public void changeVelocity (double vel) {
        this.velocity = vel;
    }
    
    public String getStatus() {
        String status = "Aircraft "+this.aircraftId+","+this.getCurLoc().getX()+","+this.getCurLoc().getY()+","+this.getCurLoc().getZ();       
        return status;
    }
    
    public int getJammingRadarId() {
        return jammingRadarId;
    }
    
    public double getJammingProbability() {
        return jammingProbability;
    }
    
    public CartesianLocation getCurLoc(){
        return ACurrentLoc;
    }
    
    public void assignBand() {
        
        int bandVal;
        Random rand = new Random();
        
        // Set a random value between
        // 1 to 4 to bandVal
        // Based on a switch case we
        // assign appropriate band
        // to bands[NEXTBAND]
        bandVal = rand.nextInt(4) + 1;
    
        // Set Duration
        bandDuration[NEXTBAND] = rand.nextInt(11);
    
        // Set Band.
        switch (bandVal) 
        {
            case 1:  bands[NEXTBAND] = Bands.BANDA;
                     break;
            case 2:  bands[NEXTBAND] = Bands.BANDB;
                     break;
            case 3:  bands[NEXTBAND] = Bands.BANDC;
                     break;
            case 4:  bands[NEXTBAND] = Bands.BANDD;
                     break;
            default: break;
        }    
        
    }
    
    public void update(int gTicks){
        
        
        
        SphericalLocation sl = ACurrentLoc.getSphLoc();
        sl.updateLocation((velocity/1000), 0.0, 0.0); 
        ACurrentLoc = sl.getCartLoc();
        assert (ACurrentLoc.getX() > 0):"Aircraft not in first cordant";
        assert (ACurrentLoc.getY() > 0):"Aircraft not in first cordant";
        assert (ACurrentLoc.getZ() > 0):"Aircraft not in first cordant";
        
        // pick the first element in the list to begin jamming.
        /*
        if (this.radarList.size() > 0) {
            jammingRadarId = radarList.get(0).getRadarId();
        } 
        */
        globalTicks = gTicks;
        
            
        /*
         * We Remain in the current band till the timer
         * expires and move on to next after that.
        */
        if (bandTimer[CURRENTBAND] > 0) {
            bandTimer[CURRENTBAND]--;
        } else {
            /*
             * Decide the next band and the duration we have
             * to stay in it.
            */
            assignBand();
            RWR.setBand(bands[NEXTBAND]);
            bands[CURRENTBAND] = bands[NEXTBAND];
            bandDuration[CURRENTBAND] = bandDuration[NEXTBAND];
            bandTimer[CURRENTBAND] = bandDuration[NEXTBAND];
        }
       
    }
    
    public void addToRadarList(Radar r) {
        this.radarList.add(r);
    }
    
    public void clearRadarList() {
        if (this.radarList.size() > 0) {
            this.radarList.clear();
        }
        
    }
}
