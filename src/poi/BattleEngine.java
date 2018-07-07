/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package poi;
import java.io.FileNotFoundException;
import java.lang.*;
import java.io.UnsupportedEncodingException;

/**
 *
 * @author srbr
 */
public class BattleEngine {

    

    /**
     * @param args the command line arguments
     */
    
    public static void main(String[] args) throws FileNotFoundException, UnsupportedEncodingException {
        // TODO code application logic here
        int i = 0;
        double C = 300000000.0;
        int TOTAL_TICKS;
        double R;
        double Preceived;
        double aVel;
        double aircraft_hits1 = 0;
        double radar_hits1 = 0;
        double aircraft_hits2 = 0;
        double radar_hits2 = 0;
        double POI1;
        double POI2;
        double finalPOI;
        int iteration = 0;
        CartesianLocation cl;
        
        int j;
        int k;
        
        // Stationary object.
        aVel = 0;
        
        //
        // TUNABLES.
        // EDIT TO GET DIFFERENT POI
        //
        
        // Instantiating the Aircraft and Radar objects here.
        AirCraft A1 = new AirCraft(1, new CartesianLocation(350, 350, 350), new CartesianLocation(0, 0, 0), aVel);
        Radar R1 = new Radar(1, new CartesianLocation(0, 0, 0), 25  * Math.pow(10.0, -6.0), Bands.BANDA);
        Radar R2 = new Radar(2, new CartesianLocation(1, 2, 3), 25  * Math.pow(10.0, -6.0), Bands.BANDB);
        
        //
        // Total number of ticks the simulation
        // should run.
        //
        TOTAL_TICKS = 10000;
        
        
        
        
        
        //for (k = 0; k < 11; k++) {

            //aircraft_hits = 0;
            //radar_hits = 0;
            //bandADuration = k;
            //nonBandADuration = 10 - k;

            // Main loop.
            
            for (i = 0; i < TOTAL_TICKS; i++) {


                cl = A1.getCurLoc();
                

                R = R1.getCurLoc().distanceCart(cl);
                R1.setEchoMatrix(A1, R1, R);
                R1.setEchoMatrixFilledTime((2 * R) / C);
                
                
                if (R1.checkEchoMatrix() == 1) {

                    radar_hits1 = radar_hits1 + 1;
                    if (R1.getSignalBand() == A1.getRWRBand()) {
                        aircraft_hits1 = aircraft_hits1 + 1;
                    }
                }  
                // System.out.println(i + ", " + A1.getRWRBand() + ", " + aircraft_hits1 + ", " + radar_hits1 + ", " + R1.getRadarAntennaAzimuth());
                
                
                R = R2.getCurLoc().distanceCart(cl);
                R2.setEchoMatrix(A1, R2, R);
                R2.setEchoMatrixFilledTime((2 * R) / C);
                
                if (R2.checkEchoMatrix() == 1) {

                    radar_hits2 = radar_hits2 + 1;
                    if (R2.getSignalBand() == A1.getRWRBand()) {
                        aircraft_hits2 = aircraft_hits2 + 1;
                    }
                }  
                //System.out.println(i + ", " + A1.getRWRBand() + ", " + aircraft_hits2 + ", " + radar_hits2 + ", " + R2.getRadarAntennaAzimuth());


                // Update the Radar
                R1.update(i);
                
                // Update the Radar
                R2.update(i);
                
                // Update the Aircraft.
                A1.update(i);

                
            }

            POI1 = (aircraft_hits1 / radar_hits1);
            POI2 = (aircraft_hits2 / radar_hits2);
            finalPOI = (POI1 + POI2) / 2;
            
            System.out.println(aircraft_hits1 + ", " + radar_hits1 + ", " + (aircraft_hits1 / radar_hits1));
            System.out.println(aircraft_hits2 + ", " + radar_hits2 + ", " + (aircraft_hits2 / radar_hits2));
            System.out.println(finalPOI);

        //}
            
        
        
    }
}
