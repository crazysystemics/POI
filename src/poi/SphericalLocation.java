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
class SphericalLocation {
    
    double Rho;
    double Phi;
    double Theta;
    
    public SphericalLocation(double rho, double phi, double theta){
        Rho = rho;
        Phi = phi;
        Theta = theta;
    }
    
    public SphericalLocation add(SphericalLocation sl) {
        double r;
        double p;
        double t;
        
        r = Rho + sl.getRho();
        p = Phi + sl.getPhi();
        t = Theta + sl.getTheta();
        
        return new SphericalLocation(r, p, t);
    }
    
    public void updateLocation(double dr, double dp, double dt) {
        Rho = Rho + dr;
        Phi = Phi + dp;
        Theta = Theta + dt;
    }

    public void changeLocation(double r, double p, double t){
        Rho = r;
        Phi = p;
        Theta = t;
    }
    
    public double getRho(){
        return Rho;
    }
    public double getPhi(){
        return Phi;
    }
    
    public double getDPhi(){
        return (Phi * (180 / 3.14159));
    }
    
    public double getTheta(){
        return Theta;
    }
    
    public double getDTheta(){
        return (Theta * (180 / 3.14159));
    }
    public CartesianLocation getCartLoc(){
        double x;
        double y;
        double z;
        double pi = 3.14159;
        
        x = Rho * Math.sin(Phi + (pi/2)) * Math.cos(Theta);
        y = Rho * Math.sin(Phi + (pi/2)) * Math.sin(Theta);
        z = Rho * Math.cos(Phi+ (pi/2));
        
        x = Math.abs(x);
        y = Math.abs(y);
        z = Math.abs(z);
        
        return new CartesianLocation(x, y, z);
        
    }
}
