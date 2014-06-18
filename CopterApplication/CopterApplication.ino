#include <Servo.h>
#include <Wire.h>

#include <BMP180.h>
#include <Ultrasonic.h>
//#include <iBoardRF24.h>
//#include <HMC5883L.h>
//#include <MPU6050.h>

//----------Pins----------
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


//----------DriversController----------
#ifndef DriversController

Servo driver1, driver2, driver3, driver4;
int idleValue = 85;
int stopValue = 80;
int driver1Speed, driver2Speed, driver3Speed, driver4Speed;

void initDrivers()
{
    driver1.attach(DRIVER_1_PIN);
    driver2.attach(DRIVER_2_PIN);
    driver3.attach(DRIVER_3_PIN);
    driver4.attach(DRIVER_4_PIN);
    delay(10);

    for (int i = 10; i < 80; i++)
    {
        setDriversSpeed(i);
    }
}

void startDrivers()
{
    setDriversSpeed(90);
    delay(100);
    setDriversSpeed(87);
    delay(100);
    setDriversSpeed(idleValue);
}

void stopDrivers()
{
    setDriversSpeed(stopValue);
}

void setDriversSpeed()
{
    delay(10);
    driver1.write(driver1Speed);
    driver2.write(driver2Speed);
    driver3.write(driver3Speed);
    driver4.write(driver4Speed);
}

void setDriversSpeed(int value)
{
    driver1Speed = driver2Speed = driver3Speed = driver4Speed = value;
    setDriversSpeed();
}

#endif //endregion


//----------SensorsController----------
#ifndef SensorsController

float _a = 0.45 / (0.99 + 0.45);

enum SensorType {
    Gyro,
    Acc,
    Bar,
    Magnet,
    Usonic
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

typedef struct UsonicData {
    float distance;
};
UsonicData usonicData;
Ultrasonic _ultrasonic(TRIG_PIN, ECHO_PIN);


void initSensors()
{
    // Gyro.
    pinMode(GS_PIN, OUTPUT);
    digitalWrite(GS_PIN, HIGH);
    pinMode(X_PIN, INPUT);
    pinMode(Y_PIN, INPUT);
    pinMode(Z_PIN, INPUT);

    // Acc.

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

    // Usonic.
    //
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
        case Usonic:
        {
            usonicData.distance = _ultrasonic.Ranging(CM);
            break;
        }
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


//----------RTController----------
#ifndef RTController

int receiveData(byte buffer[])
{
    int count = 0;
    char buf[10];

    if (Serial.available())
    {
        Serial.readBytes(&buf[0], 6);

        for (int i = 0; i < 6; i++)
        {
            buffer[i] = (byte)buf[i];
        }

        Serial.flush();
        return 6;
    }

    /*while (Serial.available())
    {
        buffer[count] = (byte)Serial.read();
        count++;
    }*/

    

    Serial.flush();

    return count;
}

int transmiteData(byte buffer[], int size)
{
    int count = 0;

    while (count < size)
    {
        Serial.write((int)buffer[count]);
        count++;
    }
    Serial.flush();

    return count;
}

#endif //endregion



void setup()
{
    // Init Bluetooth.
    Serial.begin(115200);
    //pinMode(13, OUTPUT);
    pinMode(8, OUTPUT);
    digitalWrite(8, LOW);

    initDrivers();
    startDrivers();
    //initSensors();
}

void loop()
{
    byte data[10];
    int size = receiveData(data);
    if (size != 0)
    {
        control(data);
        //transmiteData(data, size);
    }

    delay(50);
}

void control(byte data[6])
{
    int val = map(data[3], 0, 128, 110, 90);
    setDriversSpeed(val);
}
