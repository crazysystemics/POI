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
public class Radar {
    //test comment
    int RadarId;
    CartesianLocation radarLoc;
    SphericalLocation radarsphLoc;
    double PRI;
    double radarAntennaAzimuthalDirection;
    double radarAntennaElevationDirection;
    int reverseAzimuthalScan;
    int reverseElevationScan;
    int signalBand;
            
    int N;
    int DELTABINS;
    int THETA;
    int PHI;
    int [][][][] DATACUBE;
    double [] PRIDeltaBins;
    
    double MAX_BEAM_HEIGHT;
    double MAX_BEAM_WIDTH;
    
    double MIN_SCAN_AZIMUTH;
    double MAX_SCAN_AZIMUTH;
    double MIN_SCAN_ELEVATION;
    double MAX_SCAN_ELEVATION;
    double SCAN_RATE;
    
    EchoMatrix EM;
    private double dt;
    private int pulseNumber;
    private int pulse;
    private double pulseSentTime;
    private double echoMatrixFilledTime;
    
    // Variables for alpha - beta filter.
    double R;
    double theta;
    double phi;
    double C;
    double abDelt;
    CartesianLocation xk_1;
    CartesianLocation vk_1;
    CartesianLocation xm;
    CartesianLocation xk;
    CartesianLocation vk;
    CartesianLocation rk;
    double a;
    double b;
    int globalTicks;
    
    
    public Radar(int id, CartesianLocation rLoc, double pri){
        int i;        
        
        RadarId = id;
        PRI = pri;
        radarLoc = rLoc;
        assert (radarLoc.getX() >= 0):"Radar not in first cordant";
        assert (radarLoc.getY() >= 0):"Radar not in first cordant";
        assert (radarLoc.getZ() >= 0):"Radar not in first cordant";
        
        
        N = 10;
        DELTABINS = 100;
        THETA = 180;
        PHI = 180;

        DATACUBE = new int [N][DELTABINS][THETA][PHI];
        
        MAX_BEAM_HEIGHT = 5;
        MAX_BEAM_WIDTH  = 5;

        MIN_SCAN_AZIMUTH = 0 + (MAX_BEAM_WIDTH/2);
        MAX_SCAN_AZIMUTH = THETA - (MAX_BEAM_WIDTH/2);
        MIN_SCAN_ELEVATION = 34;//0 + (MAX_BEAM_HEIGHT/2);
        MAX_SCAN_ELEVATION = 34;//PHI - (MAX_BEAM_HEIGHT/2);
        SCAN_RATE = 7.5;
        
        radarAntennaAzimuthalDirection = MIN_SCAN_AZIMUTH;
        radarAntennaElevationDirection = MIN_SCAN_ELEVATION;
        reverseAzimuthalScan = 0;
        reverseElevationScan = 0;
        signalBand = 1;
        
        EM = new EchoMatrix();
        
        PRIDeltaBins = new double [DELTABINS];
        for(i = 0; i < DELTABINS; i++) {
            PRIDeltaBins[i] = ((i * PRI) / DELTABINS);
        }
        
        dt = 0.0;
        pulseSentTime = 0.0;
        echoMatrixFilledTime = 0.0;
        
        R = 0.0;
        theta = 0.0;
        phi = 0;
        C = 300000000.0;
        abDelt = 0.5;
        xk_1 = new CartesianLocation(0, 0, 0);
        vk_1 = new CartesianLocation(0, 0, 0);
        
        a = 0.4;
        b = 0.005;
        
    }
    
    public void setSignalBand(int sB) {
        signalBand = sB;
    }
    public int getSignalBand() {
        return signalBand;
    }
    
    public void setScanRate(double sr) {
        this.SCAN_RATE = sr;
    }
    
    public void reset() {
        int i;
        
        radarAntennaAzimuthalDirection = MIN_SCAN_AZIMUTH;
        radarAntennaElevationDirection = MIN_SCAN_ELEVATION;
        reverseAzimuthalScan = 0;
        reverseElevationScan = 0;
        
        EM = new EchoMatrix();
        
        dt = 0.0;
        pulseSentTime = 0.0;
        echoMatrixFilledTime = 0.0;
        
        R = 0.0;
        theta = 0.0;
        phi = 0;
        C = 300000000.0;
        abDelt = 0.5;
        xk_1 = new CartesianLocation(0, 0, 0);
        vk_1 = new CartesianLocation(0, 0, 0);
        
        a = 0.4;
        b = 0.005;
    }
    
    public void setMainBeamAzimuth(double azimuth, double elevation) {
        radarAntennaAzimuthalDirection = azimuth;
        radarAntennaElevationDirection = elevation;        
    }
    
    public void setradarAntennaAzimuthalDirection(double azimuth) {
        radarAntennaAzimuthalDirection = azimuth;
    }
    
    public void setradarAntennaElevationDirection(double elevation) {
        radarAntennaElevationDirection = elevation;
    }
    
    public void setEchoMatrixFilledTime(double val) {
        echoMatrixFilledTime = val;
    }
    
    public String getStatus() {
        String status = "Radar "+this.getRadarId()+","+getRadarAntennaElevation()+","+getRadarAntennaAzimuth()+","+EM.getStatus();
        status = status + xk_1.getX() + "," + xk_1.getY() + "," + xk_1.getZ() + ",";
        status  = status + Math.ceil(xk_1.getSphLoc().getRho()) + "," + xk_1.getSphLoc().getDPhi() + "," + xk_1.getSphLoc().getDTheta();
        return status;
    }
    
    public double getCalculatedRho() {
        return xk_1.getSphLoc().getRho();
    }
    
    public double getCalculatedPhi() {
        return xk_1.getSphLoc().getDPhi();
    }
    
    public double getCalculatedTheta() {
        return xk_1.getSphLoc().getDTheta();
    }
    
    
    public void scan() {
        //if (EM.getIsEchoMatrixSet() == 0) {
            if (reverseAzimuthalScan == 1) {
                radarAntennaAzimuthalDirection = radarAntennaAzimuthalDirection - SCAN_RATE;
            }
            if (reverseAzimuthalScan == 0) {
                radarAntennaAzimuthalDirection = radarAntennaAzimuthalDirection + SCAN_RATE;
            }
            
            if (radarAntennaAzimuthalDirection > MAX_SCAN_AZIMUTH) {
                reverseAzimuthalScan = 1;
                if (reverseElevationScan == 1) {
                    radarAntennaElevationDirection = radarAntennaElevationDirection - SCAN_RATE;
                } else {
                    radarAntennaElevationDirection = radarAntennaElevationDirection + SCAN_RATE;
                }               
            }
            
            if (radarAntennaAzimuthalDirection < MIN_SCAN_AZIMUTH) {
                reverseAzimuthalScan = 0;
                if (reverseElevationScan == 1) {
                    radarAntennaElevationDirection = radarAntennaElevationDirection - 7.5;
                } else {
                    radarAntennaElevationDirection = radarAntennaElevationDirection + 7.5;
                }
            }
            
            if (radarAntennaElevationDirection > MAX_SCAN_ELEVATION) {
                radarAntennaElevationDirection = MAX_SCAN_ELEVATION;
                this.reverseElevationScan = 1;
            }
            
            if (radarAntennaElevationDirection < MIN_SCAN_ELEVATION) {
                radarAntennaElevationDirection = MIN_SCAN_ELEVATION;
                reverseElevationScan = 0;
            }
        //}
    }
    
    public void acquire() {
        int x;
        int y;
        
        if (EM.getIsEchoMatrixSet() == 1) {
            
            // Fill the DATACUBE before fine tuning the 
            // radar antenna.
            fillDataCube();
            
            x = EM.getRowMaxElement();
            y = EM.getColMaxElement();
            
            if ((x != -1) && (y != -1)) {
                radarAntennaAzimuthalDirection = EM.getReceptorAzimuth(x, y);
                radarAntennaElevationDirection = EM.getReceptorElevation(x, y);
            }            
        }
    }
    
    public int checkEchoMatrix() {
       return EM.getIsEchoMatrixSet();
    }
    
    public void fillDataCube() {
        int index = 0;
        int i;
        
        dt = echoMatrixFilledTime - pulseSentTime;
        
        for (i = 0; i < (DELTABINS - 1); i++) {
            if ((dt >= PRIDeltaBins[i]) && (dt < PRIDeltaBins[i + 1])) {
                index = i;
                break;
            }
        }
        
        pulse = pulseNumber % N;
        double phi;
        double theta;
        theta = radarAntennaAzimuthalDirection;
        phi = radarAntennaElevationDirection;
        if(((theta >= 0) && (theta < THETA)) && ((phi >= 0) && (phi < PHI))) {
            DATACUBE[pulse][index][(int)Math.floor(theta)][(int)Math.floor(phi)] = 1; 
        }
                
    }
    
    public void track() {
        
        SphericalLocation sl;
        
        if (EM.getIsEchoMatrixSet() == 1) {
            R = (dt * C) / 2;
            theta = radarAntennaAzimuthalDirection * (3.14159/180);
            phi = radarAntennaElevationDirection * (3.14159/180);
                       
            xm = new SphericalLocation(R, phi, theta).getCartLoc();
                        
            xk = xk_1.add((vk_1.mul(abDelt)));            
            vk = vk_1;
            rk = xm.sub(xk);
            
            xk = xk.add(rk.mul(a));
            vk = vk.add(rk.mul(b).div(abDelt));
            
            xk_1 = xk;
            vk_1 = vk;            
        }
    }
    
    public void update(int gTicks) {
        
        globalTicks = gTicks;
        scan();
        //System.out.printf("Radar R%d : Radar antenna azimuth = %f, elevation = %f\n", this.RadarId, getRadarAntennaAzimuth(), getRadarAntennaElevation());
        //System.out.printf("Radar %d,%f,%f,", this.RadarId, getRadarAntennaAzimuth(), getRadarAntennaElevation());
        //showEchomatrix();
        EM.clear();
        acquire();
        track();
    }
    public CartesianLocation getCurLoc(){
        return radarLoc;
    }
    
    public double getRadarAntennaAzimuth() {
        return radarAntennaAzimuthalDirection;
    }
    public double getRadarAntennaElevation() {
        return radarAntennaElevationDirection;
    }
    
    public void setEchoMatrix(AirCraft Ac, Radar R, double acDist) {
        double Preceived = EM.getTransmittedPower() / Math.pow(acDist, 4);
        EM.setEchoMatrix(Ac, R, Preceived);
    }
    
    public void showEchomatrix(){
        EM.show();
    }
    
    public int isEchoMatrixSet() {
        return EM.getIsEchoMatrixSet();
    }
    
    public int getRadarId() {
        return RadarId;
    }
    
    
}
