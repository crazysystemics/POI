/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package poi;
import java.lang.Math.*;

/**
 *
 * @author srbr
 */
public class EchoMatrix {
    int isEchoMatrixSet;
    int NUM_RECEPTORS;
    
    double [][] ECHOMATRIX;
    
    double [][] receptorAzimuthMatrix;
    double [][] receptorElevationMatrix;
    
    double [][] GAINMATRIX;
    double MAX_BEAM_HEIGHT;
    double MAX_BEAM_WIDTH;
    
    double Pt;
    double Pr;
    double PThreshold;
    
    public EchoMatrix() {
        
        int i;
        int j;
        int sqrt_NUM_RECEPTORS;
                
        NUM_RECEPTORS = 9;
        sqrt_NUM_RECEPTORS = (int)Math.sqrt(NUM_RECEPTORS);
        
        Pt = Math.pow(10, 4);
        Pr = 0;
        PThreshold = Math.pow(10, -8);;
        
        
        MAX_BEAM_HEIGHT = 5;
        MAX_BEAM_WIDTH  = 5;
        GAINMATRIX = new double [(int)MAX_BEAM_WIDTH][(int)MAX_BEAM_HEIGHT];
        
        for (i = 0; i < MAX_BEAM_WIDTH; i++) {
            for (j = 0; j < MAX_BEAM_HEIGHT; j++) {
                GAINMATRIX[i][j] = 0.1;
            }
        }
        
        GAINMATRIX[0][2] = 0.1;
        GAINMATRIX[1][2] = 0.5;
        GAINMATRIX[2][2] = 1.0;
        GAINMATRIX[3][2] = 0.5;
        GAINMATRIX[4][2] = 0.1;
        
        GAINMATRIX[2][0] = 0.1;
        GAINMATRIX[2][1] = 0.5;
        GAINMATRIX[2][2] = 1.0;
        GAINMATRIX[2][3] = 0.5;
        GAINMATRIX[2][4] = 0.1;
        
        receptorAzimuthMatrix = new double [sqrt_NUM_RECEPTORS][sqrt_NUM_RECEPTORS];
        receptorElevationMatrix = new double [sqrt_NUM_RECEPTORS][sqrt_NUM_RECEPTORS];
        
        ECHOMATRIX = new double [sqrt_NUM_RECEPTORS][sqrt_NUM_RECEPTORS];
        isEchoMatrixSet = 0;
        
        for (i = 0; i < sqrt_NUM_RECEPTORS; i++) {
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                receptorAzimuthMatrix[i][j] = 0.0;
                receptorElevationMatrix[i][j] = 0.0;
                ECHOMATRIX[i][j] = 0.0;
            }
        }       
    }
    
    public void clear() {
        int i;
        int j;
        int sqrt_NUM_RECEPTORS;
                
        sqrt_NUM_RECEPTORS = (int)Math.sqrt(NUM_RECEPTORS);
        
        for (i = 0; i < sqrt_NUM_RECEPTORS; i++) {
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                receptorAzimuthMatrix[i][j] = 0.0;
                receptorElevationMatrix[i][j] = 0.0;
                ECHOMATRIX[i][j] = 0.0;
            }
        }
        isEchoMatrixSet = 0;
    }
    
    public void setEchoMatrix(AirCraft Ac, Radar R, double Preceived) {
        
        int i;
        int j;
        int sqrt_NUM_RECEPTORS;
        
        
        double currentReceptorAzimuth;
        double currentReceptorElevation;
        
        double NRho;
        double NPhi;
        double NTheta;
        double pi = 3.14159;
        
        //
        // Set Pr to the recevied power if greater than the threshold.
        // if lesser than that return without setting the echo-matrix.
        //
        if (Preceived >= PThreshold) {
            Pr = Preceived;
        } else {
            Pr = 0;
            return;
        }
        
        
        SphericalLocation AcSph;
        CartesianLocation tempcl;
        tempcl = Ac.getCurLoc().sub(R.getCurLoc());
        
        double radarAntennaAzimuth = R.getRadarAntennaAzimuth();
        double radarAntennaElevation  = R.getRadarAntennaElevation();
        
        if ((tempcl.getX() < 0) || (tempcl.getY() < 0) || (tempcl.getZ() < 0)) {
            return;
        }
        
        
        AcSph= tempcl.getSphLoc();
        
        NRho = AcSph.getRho();
        NPhi = AcSph.getPhi();
        NPhi = Math.floor(NPhi * (180 / pi));
        NTheta = AcSph.getTheta();
        NTheta = Math.floor(NTheta * (180 / pi));
        
        sqrt_NUM_RECEPTORS = (int)Math.sqrt(NUM_RECEPTORS);
        
        for (i = 0; i < sqrt_NUM_RECEPTORS; i++) {
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                receptorAzimuthMatrix[i][j] = 0.0;
                receptorElevationMatrix[i][j] = 0.0;
                ECHOMATRIX[i][j] = 0.0;
                isEchoMatrixSet = 0;
            }
        }
        
        //
        // Based on the Aircraft's jamming probability and if the current radar
        // is picked to be jammed, we return without filling the EchoMatrix.
        // Indicating that we are jamming the Radar.
        //
        java.lang.Math.random();
        double probability = java.lang.Math.random();
        if ((Ac.getJammingRadarId() == R.getRadarId()) && (probability < Ac.getJammingProbability())) {
            System.out.printf("JAMMER,%d\n", R.getRadarId());
            return;
        }
        
        //
        // w.r.t the Radar antenna azimuth and elevation we calculate the
        // angle of each receptor
        //
        for (i  = 0; i < sqrt_NUM_RECEPTORS; i++){
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                if (j == 0)
                    receptorAzimuthMatrix[i][j] = radarAntennaAzimuth + (MAX_BEAM_WIDTH / 2);
                if (j == 1)
                    receptorAzimuthMatrix[i][j] = radarAntennaAzimuth;
                if (j == 2)
                    receptorAzimuthMatrix[i][j] = radarAntennaAzimuth - (MAX_BEAM_WIDTH / 2);
                if (i == 0)
                    receptorElevationMatrix[i][j] = radarAntennaElevation + (MAX_BEAM_HEIGHT / 2);
                if (i == 1)
                    receptorElevationMatrix[i][j] = radarAntennaElevation;
                if (i == 2)
                    receptorElevationMatrix[i][j] = radarAntennaElevation - (MAX_BEAM_HEIGHT / 2);
            }
        }
        
        /*System.out.println("\n\n***** DEBUG START ***** \n\n");
        for (i  = 0; i < sqrt_NUM_RECEPTORS; i++){
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                System.out.print(receptorAzimuthMatrix[i][j] + " ");
            }
            System.out.println("\n");
        }
        System.out.println("\n\n***** DEBUG END ***** \n\n");*/
        
        
        //
        // We calculate the echo-matrix based on the gain matrix.
        //
        for (i  = 0; i < sqrt_NUM_RECEPTORS; i++){
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                currentReceptorAzimuth = receptorAzimuthMatrix[i][j];
                currentReceptorElevation = receptorElevationMatrix[i][j];
                
                if (((currentReceptorAzimuth + 2.5) >= NTheta) && (NTheta >= (currentReceptorAzimuth + 1.5))) {
      
                    if (((currentReceptorElevation + 2.5) > NPhi) && (NPhi >= (currentReceptorElevation + 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[0][0];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 1.5) > NPhi) && (NPhi >= (currentReceptorElevation + 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[0][1];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[0][2];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[0][3];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 1.5) > NPhi) && (NPhi >= (currentReceptorElevation - 2.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[0][4];
                        isEchoMatrixSet = 1;
                    }
      
                } else if (((currentReceptorAzimuth + 1.5) > NTheta) && (NTheta >= (currentReceptorAzimuth + 0.5))) {
      
                    if (((currentReceptorElevation + 2.5) > NPhi) && (NPhi >= (currentReceptorElevation + 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[1][0];
                        isEchoMatrixSet = 1; 
                    } else if (((currentReceptorElevation + 1.5) > NPhi) && (NPhi >= (currentReceptorElevation + 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[1][1];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[1][2];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[1][3];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 1.5) > NPhi) && (NPhi >= (currentReceptorElevation - 2.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[1][4];
                        isEchoMatrixSet = 1;
                    }
                } else if (((currentReceptorAzimuth + 0.5) > NTheta) && (NTheta >= (currentReceptorAzimuth - 0.5))) {
      
                    if (((currentReceptorElevation + 2.5) > NPhi) && (NPhi >= (currentReceptorElevation + 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[2][0]; 
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 1.5) > NPhi) && (NPhi >= (currentReceptorElevation + 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[2][1];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[2][2];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[2][3];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 1.5) > NPhi) && (NPhi >= (currentReceptorElevation - 2.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[2][4];
                        isEchoMatrixSet = 1;
                    }
                } else if (((currentReceptorAzimuth - 0.5) > NTheta) && (NTheta >= (currentReceptorAzimuth - 1.5))) {
      
                    if (((currentReceptorElevation + 2.5) > NPhi) && (NPhi >= (currentReceptorElevation + 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[3][0]; 
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 1.5) > NPhi) && (NPhi >= (currentReceptorElevation + 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[3][1];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[3][2];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[3][3];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 1.5) > NPhi) && (NPhi >= (currentReceptorElevation - 2.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[3][4];
                        isEchoMatrixSet = 1;
                    }
                } else if (((currentReceptorAzimuth - 1.5) > NTheta) && (NTheta >= (currentReceptorAzimuth - 2.5))) {
      
                    if (((currentReceptorElevation + 2.5) > NPhi) && (NPhi >= (currentReceptorElevation + 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[4][0]; 
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 1.5) > NPhi) && (NPhi >= (currentReceptorElevation + 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[4][1];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation + 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 0.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[4][2];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 0.5) > NPhi) && (NPhi >= (currentReceptorElevation - 1.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[4][3];
                        isEchoMatrixSet = 1;
                    } else if (((currentReceptorElevation - 1.5) > NPhi) && (NPhi >= (currentReceptorElevation - 2.5))) {
                        ECHOMATRIX[i][j] = Pr * GAINMATRIX[4][4];
                        isEchoMatrixSet = 1;
                    }
                }
               
            }
        }
    }
    
    public int getIsEchoMatrixSet() {
        return isEchoMatrixSet;
    }
    
    public double getReceptorAzimuth(int i, int j) {
        return receptorAzimuthMatrix[i][j];
    }
    public double getReceptorElevation(int i, int j) {
        return receptorElevationMatrix[i][j];
    }
    public int getRowMaxElement() {
        
        int row = -1;
        double max;
        int i;
        int j;
        int sqrt_NUM_RECEPTORS = (int)Math.sqrt(NUM_RECEPTORS);
        
        max = ECHOMATRIX[0][0];
        if (max > 0)
            row = 0;
        for (i = 0; i < sqrt_NUM_RECEPTORS; i++) {
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                if (ECHOMATRIX[i][j] > max) {
                    max = ECHOMATRIX[i][j];
                    row = i;
                }
            }
        }
        
        
        return row;
    }
    
    public int getColMaxElement() {
        int col = -1;
        double max;
        int i;
        int j;
        int sqrt_NUM_RECEPTORS = (int)Math.sqrt(NUM_RECEPTORS);
        
        max = ECHOMATRIX[0][0];
        if (max > 0)
            col = 0;
        for (i = 0; i < sqrt_NUM_RECEPTORS; i++) {
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                if (ECHOMATRIX[i][j] > max) {
                    max = ECHOMATRIX[i][j];
                    col = j;
                }
            }
        }
        
        
        return col;
    }
    
    public double getMaxElement() {
        double max;
        int i;
        int j;
        int sqrt_NUM_RECEPTORS = (int)Math.sqrt(NUM_RECEPTORS);
        
        max = ECHOMATRIX[0][0];
        for (i = 0; i < sqrt_NUM_RECEPTORS; i++) {
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                if (ECHOMATRIX[i][j] > max) {
                    max = ECHOMATRIX[i][j];
                }
            }
        }
        return max;
    }
    
    public void show() {
        int i;
        int j;
        int sqrt_NUM_RECEPTORS = (int)Math.sqrt(NUM_RECEPTORS);
        
        
        for (i = 0; i < sqrt_NUM_RECEPTORS; i++) {
            for (j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                System.out.printf("%f,",ECHOMATRIX[i][j]);
            }
        }
        
        System.out.printf("%d,%d\n", getRowMaxElement(), getColMaxElement());
        
    }
    
    public String getStatus() {
        String status = "";
        int sqrt_NUM_RECEPTORS = (int)Math.sqrt(NUM_RECEPTORS);
        
        for (int i = 0; i < sqrt_NUM_RECEPTORS; i++) {
            for (int j = 0; j < sqrt_NUM_RECEPTORS; j++) {
                status = status + ECHOMATRIX[i][j] + ",";
            }
        }
        status = status + this.getIsEchoMatrixSet() + ",";
        return status;        
    }
    
    public double getTransmittedPower(){
        return Pt;
    }
    
    
}
