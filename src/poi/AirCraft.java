/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package poi;
import java.util.*;

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
    int bandADuration;
    int nonBandADuration;
    int globalTicks;
    int bandATimer;
    int bandNATimer;
    
    
    
    public AirCraft(int aId, CartesianLocation Ai, CartesianLocation Ae, double vel, int bADuration, int nBADuration) {
        
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
        bandADuration = bADuration - 1;
        nonBandADuration = nBADuration - 1;
        bandATimer = bandADuration; 
        bandNATimer = nonBandADuration;
        
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
        
               
        // BAND A
        if (RWR.getBand() == 1) {
            if (bandATimer <= 0) {
                RWR.setBand(0);
                bandNATimer = nonBandADuration;
            } else {
                bandATimer = bandATimer - 1;
            }            
        } else { // Not BAND A
            if (bandNATimer <= 0) {
                RWR.setBand(1);
                bandATimer = bandADuration;
            } else {
                bandNATimer = bandNATimer - 1;
            }            
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
