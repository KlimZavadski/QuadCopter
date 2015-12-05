#include <Servo.h>
#include <Wire.h>

#include <BMP180.h>
#include <Ultrasonic.h>

#include <MemoryFree.h>

#define uint unsigned int
#define ulong unsigned long

#pragma region -Pins-------------------------

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

#pragma endregion

#pragma region -Drivers Controller-----------

Servo driver1, driver2, driver3, driver4;
int idleValue = 87;
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
    delay(50);
    setDriversSpeed(87);
    delay(50);
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

#pragma endregion

#pragma region -Sensors Controller-----------

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
    //_barometer = BMP180();
    //if (_barometer.EnsureConnected())
    {
    //    _barometer.SoftReset();
    //    _barometer.Initialize();
    }
    //else
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
            //if (_barometer.IsConnected)
            //{
            //    long pressure = _barometer.GetPressure(); // In pascals.
            //    float altitude = _barometer.GetAltitude(_seaLevelPressure); // In meters.
            //    float temperature = _barometer.GetTemperature(); // In celcius.

            //    barData.pressure = _lowpass(barData.pressure, pressure);
            //    barData.altitude = _lowpass(barData.altitude, altitude);
            //    barData.temperature = _lowpass(barData.temperature, altitude);
            //}
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

#pragma endregion

#pragma region -WiFi Controller--------------

int receiveData(byte buffer[])
{
    int count = 0;
    char buf[10];

    //if (Serial.available())
    //{
    //    Serial.readBytes(&buf[0], 6);

    //    for (int i = 0; i < 6; i++)
    //    {
    //        buffer[i] = (byte)buf[i];
    //    }

    //    Serial.flush();
    //    return 6;
    //}

    ///*while (Serial.available())
    //{
    //    buffer[count] = (byte)Serial.read();
    //    count++;
    //}*/

    //

    //Serial.flush();

    return count;
}

int transmiteData(byte buffer[], int size)
{
    int count = 0;

    /*while (count < size)
    {
        Serial.write((int)buffer[count]);
        count++;
    }
    Serial.flush();*/

    return count;
}

#pragma endregion

#pragma region -Neural Network---------------

#define Sigma(x) 1.0 / (1.0 + exp(-x))

// Weights of neurons
// 0-32767, 15bits + sign
int Weights[] = {
    // w0
    1951, 3137,
    -691, 1386,
    3727, 585,
    2911, -2101,
    -1704, 2825,
    -94, -618,
    359, 1569,
    964, 2434,
    // w1
    950, 776, 87, 877, 373, 914, 260, 138,
    840, -1050, 3532, 3346, -2431, -616, -295, 225,
    531, 648, -1119, -1470, 1452, -563, 148, 624,
    1086, 708, 605, 1124, 259, 960, 1065, 285,
    670, 63, 434, 37, 655, 42, 921, 970,
    571, 621, 332, 557, 1031, 595, 388, 906,
    895, 608, 203, 1150, 560, 727, 984, 307,
    3652, 209, 2085, -1303, 1479, -2392, 714, 2449,
    // w2
    -773, 1973, -92, -2063, -736, -1106, -1516, 6535,
    -780, 4760, -2373, -484, -1147, -1277, -468, -1124,
    -53, -2261, -758, 690, -753, -177, 140, -6190,
    -541, -5503, 1212, -496, -445, -660, -327, 1738,
};

// Input array for neurons.
// 0-32767, 15bits without sign.
uint Input[8];

// Output array for neurons.
// 0-32767, 15bits without sign.
uint Output[8];

void computeNeuron(byte num)
{
    if (num < 8)  // First layer.
    {
        long sum = 0;

        for (byte i = 0; i < 4; i++)
        {
            sum += (long) Input[i] * Weights[num * 4 + i];
        }

        double norm = sum / 1073676289.0;  // normalization to (0,1).
        uint result = Sigma(norm) * 32767.0;  // restore to uint.
        Output[num] = result;
    }
    else if (num < 16)  // Second layer.
    {
        long sum = 0;

        for (byte i = 0; i < 8; i++)
        {
            sum += (long) Output[i] * Weights[num * 8 + i];
        }

        double norm = sum / 1073676289.0;  // normalization to (0,1).
        uint result = Sigma(norm) * 32767.0;  // restore to uint.
        Input[num] = result;
    }
    if (num < 20)
    {
        long sum = 0;

        for (byte i = 0; i < 8; i++)
        {
            sum += (long) Input[i] * Weights[num * 8 + i];
        }

        double norm = sum / 1073676289.0;  // normalization to (0,1).
        uint result = Sigma(norm) * 32767.0;  // restore to uint.
        Output[num] = result;
    }
}

#pragma endregion

#pragma region printf.h

int serial_putc(char c, FILE *)
{
    Serial.write(c);
    return c;
}

void printf_begin(void)
{
    fdevopen(&serial_putc, 0);
}

#pragma endregion


void setup()
{
    Serial.begin(115200);

    //pinMode(13, OUTPUT);
    pinMode(8, OUTPUT);
    digitalWrite(8, LOW);

    pinMode(14, INPUT);
    pinMode(15, INPUT);
    pinMode(16, INPUT);
    pinMode(17, INPUT);
    pinMode(19, OUTPUT);
    digitalWrite(19, LOW);

    initDrivers();
    startDrivers();
    //initSensors();
}

void loop()
{
    Serial.println(sizeof(Weights) + sizeof(Input) * 2);
    Serial.println(2048 - freeMemory());
    Serial.println();

    Input[0] = 0;
    Input[1] = 240;
    Input[2] = 0;
    Input[3] = 220;

    size_t time = micros();

    for (byte n = 0; n < 20; n++)
    {
        computeNeuron(n);
    }

    time = micros() - time;
    Serial.println();
    Serial.println(time);

    while (true);

    /*int x = dx - analogRead(3);
    int y = dy - analogRead(2);
    int z = dz - analogRead(1);

    Serial.print("X=");
    Serial.print(x);
    Serial.print("  Y=");
    Serial.println(y);*/


    if (Serial.available())
    {
        //int speed = (int)Serial.parseInt();
        char bytes[6];
        int r = Serial.readBytes(bytes, 6);
        int speed = (int)bytes[0];

        Serial.read();
        Serial.flush();
        Serial.print("Speed: ");
        Serial.print(speed);

        speed = map(speed, 0, 128, 97, 87);
        Serial.println(speed);
        setDriversSpeed(speed);
    }

    delay(500);
}

void control(byte data[6])
{
    /*int val = map(data[3], 0, 128, 110, 90);
    setDriversSpeed(val);*/
}
