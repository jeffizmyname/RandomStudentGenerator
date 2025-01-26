#include <Wire.h>
#include <Adafruit_NeoPixel.h>
#include <Adafruit_NeoMatrix.h>
#include <MPU6050.h>

// Pin configuration
#define MATRIX_PIN 6  // WS2812 DIN connected to GPIO5
#define MATRIX_WIDTH 8
#define MATRIX_HEIGHT 8

// Initialize objects
MPU6050 mpu;
//Adafruit_NeoPixel matrix = Adafruit_NeoPixel(MATRIX_WIDTH * MATRIX_HEIGHT, MATRIX_PIN, NEO_GRB + NEO_KHZ800);
Adafruit_NeoMatrix matrix = Adafruit_NeoMatrix(8, 8, MATRIX_PIN,
    NEO_MATRIX_RIGHT + NEO_MATRIX_LEFT + NEO_MATRIX_COLUMNS + NEO_MATRIX_PROGRESSIVE,
    NEO_GRB + NEO_KHZ800);

// Variables for shake detection
const float shakeThreshold = 2.5;  // Adjust based on sensitivity
long lastShakeTime = 0;
const int debounceTime = 2000;  // Time in ms

void setup() {
    Serial.begin(9600);

    // Initialize MPU6050
    Wire.begin();
    mpu.initialize();
    if (mpu.testConnection()) {
        Serial.println("MPU6050 connected!");
    }
    else {
        Serial.println("Failed to connect to MPU6050!");
        while (1);
    }

    // Initialize WS2812 matrix
    matrix.begin();
    matrix.setTextColor(matrix.Color(128, 0, 0));
    matrix.setTextWrap(false);
    matrix.show();  // Clear matrix
}



void loop() {
    // Read accelerometer data
    int16_t ax, ay, az;
    mpu.getAcceleration(&ax, &ay, &az);

    // Normalize acceleration to Gs
    float gX = ax / 16384.0;
    float gY = ay / 16384.0;
    float gZ = az / 16384.0;

    // Map accelerometer values to matrix indices
    int x = map(gX * 10, -10, 10, 0, MATRIX_WIDTH - 1);
    int y = map(gY * 10, -10, 10, 0, MATRIX_HEIGHT - 1);

    // Ensure values are within bounds
    x = constrain(x, 0, MATRIX_WIDTH - 1);
    y = constrain(y, 0, MATRIX_HEIGHT - 1);

    // Display dot on the matrix
    //matrix.clear();
    //matrix.setPixelColor(x + y * MATRIX_WIDTH, matrix.Color(0, 0, 255));  // Blue dot
    matrix.show();

    if (Serial.available() > 0)
    {
        String receiveVal = Serial.readString();
        //Serial.println(receiveVal);
        matrix.fillScreen(0);
        matrix.setCursor(8, 0);
        printText(receiveVal, 100);
        //matrix.setPixelColor(receiveVal.toInt(), matrix.Color(128, 0, 0));
        matrix.show();
    }

    // Detect shake
    float magnitude = sqrt(gX * gX + gY * gY + gZ * gZ);
    if (magnitude > shakeThreshold) {
        long currentTime = millis();
        if (currentTime - lastShakeTime > debounceTime) {
            triggerShakeEffect();
            lastShakeTime = currentTime;
        }
    }

    delay(50);
}

// Shake effect: Rainbow animation
void triggerShakeEffect() {
    Serial.print("1");
    delay(10);
    Serial.print("0");
}

// Generate rainbow colors across 0-255 positions
uint32_t Wheel(byte WheelPos) {
    WheelPos = 255 - WheelPos;
    if (WheelPos < 85) {
        return matrix.Color(255 - WheelPos * 3, 0, WheelPos * 3);
    }
    else if (WheelPos < 170) {
        WheelPos -= 85;
        return matrix.Color(0, WheelPos * 3, 255 - WheelPos * 3);
    }
    else {
        WheelPos -= 170;
        return matrix.Color(WheelPos * 3, 255 - WheelPos * 3, 0);
    }
}

void printText(String txt, int sped) {
    for (int j = 0; j < 256; j++) {
        for (int i = 0; i < matrix.numPixels(); i++) {
            matrix.setPixelColor(i, Wheel((i + j) & 255));
        }
        matrix.show();
        delay(10);
    }

    int textPos = MATRIX_WIDTH;
    matrix.fillScreen(0);

    int16_t textWidth = 6 * strlen(txt.c_str());

    while (textPos > -textWidth) {
        textPos--;
        matrix.fillScreen(0);
        matrix.setCursor(textPos, 0);
        matrix.print(txt);
        matrix.show();
        delay(sped);
    }
}
