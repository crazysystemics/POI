/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package poi;

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
    int bandBDuration;
    int bandCDuration;
    int bandDDuration;
    
    int globalTicks;
    int bandATimer;
    int bandBTimer;
    int bandCTimer;
    int bandDTimer;
    
    
    
    
    
    public AirCraft(int aId, CartesianLocation Ai, CartesianLocation Ae, double vel, int bADuration, int bBDuration,
            int bCDuration, int bDDuration) {
            
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
        if (bADuration > 0) {
            bandADuration = bADuration - 1;
        }
        if (bBDuration > 0) {
            bandBDuration = bBDuration - 1;
        }
        if (bCDuration > 0) {
            bandCDuration = bCDuration - 1;
        }
        if (bDDuration > 0) {
            bandDDuration = bDDuration - 1;
        }
        
        
        bandATimer = bandADuration; 
        bandBTimer = bandBDuration;
        bandCTimer = bandCDuration;
        bandDTimer = bandDDuration;
        
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
        
               
        
        if (RWR.getBand() == Bands.BANDA) {
            if (bandATimer > 0) {
                bandATimer = bandATimer - 1;           
            } else if (bandBDuration != 0) {
                RWR.setBand(Bands.BANDB);
                bandBTimer = bandBDuration;
            } else if (bandCDuration != 0) {
                RWR.setBand(Bands.BANDC);
                bandCTimer = bandCDuration;
            } else if (bandDDuration != 0) {
                RWR.setBand(Bands.BANDD);
                bandDTimer = bandDDuration;
            }
        } else if (RWR.getBand() == Bands.BANDB) {
           if (bandBTimer > 0) {
                bandBTimer = bandBTimer - 1;           
            } else if (bandCDuration != 0) {
                RWR.setBand(Bands.BANDC);
                bandCTimer = bandCDuration;
            } else if (bandDDuration != 0) {
                RWR.setBand(Bands.BANDD);
                bandDTimer = bandDDuration;
            } else if (bandADuration != 0) {
                RWR.setBand(Bands.BANDA);
                bandATimer = bandADuration;
            } 
        } else if (RWR.getBand() == Bands.BANDC) {
            if (bandCTimer > 0) {
                bandCTimer = bandCTimer - 1;           
            } else if (bandDDuration != 0) {
                RWR.setBand(Bands.BANDD);
                bandDTimer = bandDDuration;
            } else if (bandADuration != 0) {
                RWR.setBand(Bands.BANDA);
                bandATimer = bandADuration;
            } else if (bandBDuration != 0) {
                RWR.setBand(Bands.BANDB);
                bandBTimer = bandBDuration;
            }
        } else if  (RWR.getBand() == Bands.BANDD) {
            if (bandDTimer > 0) {
                bandDTimer = bandDTimer - 1;           
            } else if (bandADuration != 0) {
                RWR.setBand(Bands.BANDA);
                bandATimer = bandADuration;
            } else if (bandBDuration != 0) {
                RWR.setBand(Bands.BANDB);
                bandBTimer = bandBDuration;
            } else if (bandCDuration != 0) {
                RWR.setBand(Bands.BANDC);
                bandCTimer = bandCDuration;
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
