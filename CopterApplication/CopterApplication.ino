#include <Servo.h>
#include <Wire.h>

#include <BMP180.h>
#include <Ultrasonic.h>


#ifndef Pins

#define DRIVER_1_PIN 2
#define DRIVER_2_PIN 3
#define DRIVER_3_PIN 4
#define DRIVER_4_PIN 5

#define GS_PIN 6
#define X_PIN 7 //analog
#define Y_PIN 8 //analog
#define Z_PIN 9 //analog

#define TRIG_PIN 10
#define ECHO_PIN 11


#endif //endregion

#ifndef DriversController

Servo driver1, driver2, driver3, driver4;
int idleValue = 85;
int stopValue = 80;

void initDrivers()
{
    driver1.attach(DRIVER_1_PIN);
    driver2.attach(DRIVER_2_PIN);
    driver3.attach(DRIVER_3_PIN);
    driver4.attach(DRIVER_4_PIN);
    delay(10);

    for (int i = 10; i < 80; i++)
    {
        _writeToDrivers(i);
    }
}

void startDrivers()
{
    _writeToDrivers(90);
    delay(100);
    _writeToDrivers(87);
    delay(100);
    _writeToDrivers(idleValue);
}

void stopDrivers()
{
    _writeToDrivers(stopValue);
}

void _writeToDrivers(int value)
{
    delay(10);
    driver1.write(value);
    driver2.write(value);
    driver3.write(value);
    driver4.write(value);
}

#endif //endregion

#ifndef SensorsController

float _a = 0.45 / (0.99 + 0.45);
enum SensorType {
    Gyro,
    Acc,
    Usonic,
    Bar,
    Magnet
};


typedef struct GyroData {
    int x;
    int y;
    int z;
};
GyroData gyroData;
int _dx = 349;
int _dy = 357;
int _dz = 509;

typedef struct AccData {
};
AccData accData;

typedef struct UsonicData {
    float distance;
};
UsonicData usonicData;
Ultrasonic _ultrasonic(TRIG_PIN, ECHO_PIN);

typedef struct BarData {
    long pressure;
    float altitude;
    float temperature;
};
BarData barData;
BMP180 _barometer;
float _seaLevelPressure = 101325;

typedef struct MagnetData {
};
MagnetData magnetData;

//typedef union SensorData {
//    GyroData gyro;
//    AccData acc;
//    UsonicData usonic;
//    BarData bar;
//    MagnetData magnet;
//};


void initSensors()
{
    // Gyro.
    pinMode(GS_PIN, OUTPUT);
    digitalWrite(GS_PIN, HIGH);
    pinMode(X_PIN, INPUT);
    pinMode(Y_PIN, INPUT);
    pinMode(Z_PIN, INPUT);

    // Acc.

    // Usonic.
    //

    // Bar.
    _barometer = BMP180();
    if (_barometer.EnsureConnected())
    {
        _barometer.SoftReset();
        _barometer.Initialize();
    }
    else
    {
        // TODO: Handle it.
    }
}

void updateSensorData(int type)
{
    switch (type)
    {
        case Gyro:
        {
            int x = _dx - analogRead(X_PIN);
            int y = _dy - analogRead(Y_PIN);
            int z = _dz - analogRead(Z_PIN);

            gyroData.x = _lowpass(gyroData.x, x);
            gyroData.y = _lowpass(gyroData.y, y);
            gyroData.z = _lowpass(gyroData.z, z);
            break;
        }
        case Acc:
            break;
        case Usonic:
        {
            float distance = _ultrasonic.Ranging(CM);

            usonicData.distance = _lowpass(usonicData.distance, distance);
            break;
        }
        case Bar:
        {
            if (_barometer.IsConnected)
            {
                long pressure = _barometer.GetPressure(); // In pascals.
                float altitude = _barometer.GetAltitude(_seaLevelPressure); // In meters.
                float temperature = _barometer.GetTemperature(); // In celcius.

                barData.pressure = _lowpass(barData.pressure, pressure);
                barData.altitude = _lowpass(barData.altitude, altitude);
                barData.temperature = _lowpass(barData.temperature, altitude);
            }
        }
            break;
        case Magnet:
            break;
        default:
            break;
    }
}

int _lowpass(int prev, int curr)
{
    return _a * curr + (1 - _a) * prev;
}

long _lowpass(long prev, long curr)
{
    return _a * curr + (1 - _a) * prev;
}

float _lowpass(float prev, float curr)
{
    return _a * curr + (1 - _a) * prev;
}

#endif //endregion

#ifndef RTController

#endif //endregion


void setup()
{

  /* add setup code here */

}

void loop()
{

  /* add main program code here */

}
