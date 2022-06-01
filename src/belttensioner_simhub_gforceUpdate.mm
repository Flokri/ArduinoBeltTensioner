// G-forces from SimHub properties
var Gy =  $prop('AccelerationSway'); // lateral (yaw) acceleration
var Gs = $prop('AccelerationSurge'); // deceleration
var tmax = $prop('Settings.tmax');    // limit servos

// increase the yaw and decel values
Gy *= $prop('Settings.yaw_gain');
Gs *= $prop('Settings.decel_gain');

// non-negative deceleration
if (0 > Gs){ Gs = 0; }

// convert speed and yaw changes to left and right tension values
// turning right should increase right harness tension (body pushed left)
var rightForce = Math.sqrt(Gs*Gs + Gy*Gy);
var leftForce = Gs + Gs - rightForce;

if (0 > Gy) {
    var t = rightForce;  // negative Gy increases left tension
    rightForce = leftForce;
    leftForce = t;
}

// Low-pass IIR filtering of left and right tension values
if (null == root["lb4"]) {
    // initialize
    root["rb4"] = rightForce;
    root["lb4"] = leftForce;
}

var rb4 = root["rb4"];
var lb4 = root["lb4"]; // previously filtered values
var tc = 1 + $prop('Settings.smooth');
rb4 += (rightForce - rb4) / tc;
lb4 += (leftForce - lb4) / tc;
root["lb4"] = lb4;
root["rb4"] = rb4;

leftForce = lb4;
rightForce = rb4; // filtered tensions;  comment out for unfiltered (or set Settings.smooth = 1)

if (leftForce > tmax)
  leftForce = tmax;

if (rightForce > tmax)
  rightForce = tmax;  
  
var s = [0x42 | ((0xE0 & leftForce)>>2)];
s[1] = 0x7F & leftForce;
    
s[2] = 0x43 | ((0xE0 & rightForce)>>2);
s[3] = 0x7F & rightForce;

var str = String.fromCharCode.apply(null,s);