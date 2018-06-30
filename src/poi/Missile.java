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
public class Missile {
    
    SphericalLocation MLoc;
    int destroyedAircraft;
    
    public Missile(SphericalLocation loc) {
        MLoc = loc;
        destroyedAircraft = 0;
    }
    
    public SphericalLocation getCurLoc() {
        return MLoc;
    }
    
    public void updateMissileLocation (double dRho, double dPhi, double dTheta) {
        MLoc = MLoc.add(new SphericalLocation(1, 1, 1));
    }
    
    public void setDestroyedAircraft(int val) {
        assert ((val == 0) || (val == 1));
        destroyedAircraft = val;
    }
    
    
}
