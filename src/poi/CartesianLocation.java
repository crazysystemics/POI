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
class CartesianLocation {
    
    double X;
    double Y;
    double Z;

    public CartesianLocation(double x, double y, double z) {
        X = x;
        Y = y;
        Z = z;
    
    }

    public void updateLocation(double dx, double dy, double dz) {
        X = X + dx;
        Y = Y + dy;
        Z = Z + dz;
    }

    public void changeLocation(double x, double y, double z){
        X = x;
        Y = y;
        Z = z;
    }

    public double getX() {
        return X;
    }
    public double getY() {
        return Y;
    }
    public double getZ() {
        return Z;
    }
    public  SphericalLocation getSphLoc() {
        double rho;
        double phi;
        double theta;
        double pi = 3.14159;
    
        rho = java.lang.Math.sqrt(((X * X) + (Y * Y) + (Z * Z)));
        phi = java.lang.Math.atan(java.lang.Math.sqrt(((X * X) + (Y * Y))) /  Z);
        phi = (pi/2) - phi;
        theta = java.lang.Math.atan(Y / X);
    
        return new SphericalLocation(rho, phi, theta);
    }

    public CartesianLocation add(CartesianLocation n){
    
        double x;
        double y;
        double z;
    
        x = X + n.getX();
        y = Y + n.getY();
        z = Z + n.getZ();
    
        return new CartesianLocation(x, y, z);
        
    }

    public CartesianLocation sub(CartesianLocation n) {

        double x;
        double y;
        double z;
    
        x = X - n.getX();
        y = Y - n.getY();
        z = Z - n.getZ();
    
        return new CartesianLocation(x, y, z);
    }
    
    public CartesianLocation mul(double m) {
        
        return new CartesianLocation((m * X), (m * Y), (m * Z));
    }
    
    public CartesianLocation div(double d) {
        return new CartesianLocation((X / d), (Y / d), (Z / d));
    }

    public double distanceCart(CartesianLocation n) {
        double x = X - n.getX();
        double y = Y - n.getY();
        double z = Z - n.getZ();
    
        return (Math.sqrt((x * x) + (y * y) + (z * z)));
    }

}