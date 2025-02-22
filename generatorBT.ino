#include <Wire.h>
//#include <Adafruit_NeoPixel.h>
#include <Adafruit_NeoMatrix.h>
#include <MPU6050.h>
#include <vector>
#include <BluetoothSerial.h>

// Pin configuration
#define MATRIX_PIN 13
#define MATRIX_WIDTH 8
#define MATRIX_HEIGHT 8

// Initialize objects
MPU6050 mpu;
Adafruit_NeoPixel pixel = Adafruit_NeoPixel(MATRIX_WIDTH * MATRIX_HEIGHT, MATRIX_PIN, NEO_GRB + NEO_KHZ800);
Adafruit_NeoMatrix matrix = Adafruit_NeoMatrix(8, 8, MATRIX_PIN,
    NEO_MATRIX_RIGHT + NEO_MATRIX_LEFT + NEO_MATRIX_COLUMNS + NEO_MATRIX_PROGRESSIVE,
    NEO_GRB + NEO_KHZ800);

// Variables for shake detection
const float shakeThreshold = 2.5; 
long lastShakeTime = 0;
const int debounceTime = 2000;

BluetoothSerial SerialBT;

void setup() {
    Serial.begin(9600);
    SerialBT.begin("ESP32_LED_MATRIX");

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
    matrix.setTextColor(matrix.Color(128, 128, 128));
    matrix.setTextWrap(false);
    matrix.show();  
}

// Effects and colors
uint32_t colorEffect = pixel.Color(255, 255, 255);
uint32_t colorText = matrix.Color(255, 255, 255);

String effect = "Flashy";

std::vector<std::vector<std::pair<int, int>>> rings = {
  { {0,0},{1,0},{2,0},{3,0},{4,0},{5,0},{6,0},{7,0},{7,1},{7,2},{7,3},{7,4},{7,5},{7,6},{7,7},{6,7},
    {5,7},{4,7},{3,7},{2,7},{1,7},{0,7},{0,6},{0,5},{0,4},{0,3},{0,2},{0,1} }, 
  { {1,1},{2,1},{3,1},{4,1},{5,1},{6,1},{6,2},{6,3},{6,4},{6,5},{6,6},{5,6},{4,6},{3,6},{2,6},{1,6},
    {1,5},{1,4},{1,3},{1,2} }, 
  { {2,2},{3,2},{4,2},{5,2},{5,3},{5,4},{5,5},{4,5},{3,5},{2,5},{2,4},{2,3} }, 
  { {3,3},{4,3},{4,4},{3,4} } 
};

void loop() {
    int16_t ax, ay, az;
    mpu.getAcceleration(&ax, &ay, &az);

    float gX = ax / 16384.0;
    float gY = ay / 16384.0;
    float gZ = az / 16384.0;

    int x = map(gX * 10, -10, 10, 0, MATRIX_WIDTH - 1);
    int y = map(gY * 10, -10, 10, 0, MATRIX_HEIGHT - 1);

    x = constrain(x, 0, MATRIX_WIDTH - 1);
    y = constrain(y, 0, MATRIX_HEIGHT - 1);

    // Display dot on the matrix
    //matrix.clear();
    //matrix.setPixelColor(x + y * MATRIX_WIDTH, matrix.Color(0, 0, 255));  // Blue dot
    matrix.show();

    if (SerialBT.available())
    {
        String receiveVal = SerialBT.readString();

        matrix.fillScreen(0);
        matrix.setCursor(8, 0);

        if (receiveVal.startsWith("command:")) {
            if (receiveVal.indexOf("effects") > 0) {
                SerialBT.println("effects:Flashy,Rainbow,Loading,Explode");
            }
            if (receiveVal.indexOf("colors") > 0) {
                SerialBT.println("colors:white,red,green,blue,yellow,cyan");
            }
            if (receiveVal.indexOf("setEffect") > 0) {
                receiveVal.replace("command:setEffect:", "");
                receiveVal.trim();
                effect = receiveVal;
            }
            if (receiveVal.indexOf("setColor") > 0) {
                receiveVal.replace("command:setColor:", "");
                receiveVal.trim();

                if (receiveVal == "red") { colorText = matrix.Color(128, 0, 0); colorEffect = pixel.Color(128, 0, 0); }
                if (receiveVal == "green") { colorText = matrix.Color(0, 128, 0); colorEffect = pixel.Color(0, 128, 0); }
                if (receiveVal == "blue") { colorText = matrix.Color(0, 0, 128); colorEffect = pixel.Color(0, 0, 128); }
                if (receiveVal == "yellow") { colorText = matrix.Color(128, 128, 0); colorEffect = pixel.Color(128, 128, 0); }
                if (receiveVal == "cyan") { colorText = matrix.Color(0, 128, 128); colorEffect = pixel.Color(0, 128, 128); }
                if (receiveVal == "white") { colorText = matrix.Color(128, 128, 128); colorEffect = pixel.Color(128, 128, 128); }
                //color = getColor(receiveVal);
            }
        }
        else {
            printText(receiveVal, 100);
        }
        //matrix.setPixelColor(receiveVal.toInt(), matrix.Color(128, 0, 0));
        matrix.show();
    }

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

void triggerShakeEffect() {
    SerialBT.println("1");
    Serial.println("1");
    delay(10);
    SerialBT.println("0");
    Serial.println("0");

}

uint32_t ColorWheel(byte wheelPos) {
    wheelPos = 255 - wheelPos;

    if (wheelPos < 85) {
        return matrix.Color(255 - wheelPos * 3, 0, wheelPos * 3);
    }
    else if (wheelPos < 170) {
        wheelPos -= 85;
        return matrix.Color(0, wheelPos * 3, 255 - wheelPos * 3);
    }
    else {
        wheelPos -= 170;
        return matrix.Color(wheelPos * 3, 255 - wheelPos * 3, 0);
    }
}


void theaterChase(uint32_t col, int wait) {
    for (int a = 0; a < 10; a++) {  
        for (int b = 0; b < 3; b++) { 
            matrix.fillScreen(0);
            for (int c = b; c < matrix.numPixels(); c += 3) {
                matrix.setPixelColor(c, col); 
            }
            matrix.show();
            delay(wait); 
        }
    }
}

void expandingContractingRings(uint32_t col, int wait) {
    matrix.fillScreen(0);
    matrix.show();
    for (int r = 0; r < 4; r++) {
        for (int p = 0; p < rings[r].size(); p++) {
            int x = rings[r][p].first;
            int y = rings[r][p].second;
            matrix.drawPixel(x, y, col);
            matrix.show();
            delay(wait); 
        }
    }

    delay(500);

    for (int r = 3; r >= 0; r--) {
        for (int p = rings[r].size() - 1; p >= 0; p--) {
            int x = rings[r][p].first;
            int y = rings[r][p].second;
            matrix.drawPixel(x, y, 0);  
            matrix.show();
            delay(wait); 
        }
    }
}

void explodingCenter(uint32_t col, int wait) {
    matrix.fillScreen(0); 
    matrix.show();
    int centerX = 3;
    int centerY = 3;

    for (int i = 0; i < 4; i++) {
        for (int x = centerX - i; x <= centerX + i; x++) {
            for (int y = centerY - i; y <= centerY + i; y++) {
                if (x >= 0 && x < 8 && y >= 0 && y < 8) {
                    matrix.drawPixel(x, y, col); 
                }
            }
        }
        matrix.show();
        delay(wait); 
    }
}

void rainbowSweep(int wait) {
    for (int i = 0; i < 256; i++) {
        for (int j = 0; j < 8; j++) {
            for (int k = 0; k < 8; k++) {
                matrix.drawPixel(j, k, ColorWheel((i + (j * 32) + (k * 32)) & 255)); 
            }
        }
        matrix.show();
        delay(wait);  
    }
}



void printText(String txt, int sped) {

    if (effect == "Flashy") {
        theaterChase(colorEffect, 100);
    }
    else if (effect == "Loading") {
        expandingContractingRings(colorText, 100);
    }
    else if (effect == "Explode") {
        explodingCenter(colorText, 100);
    }
    else if (effect == "Rainbow") {
        rainbowSweep(20);
    }



    matrix.show();

    int textPos = MATRIX_WIDTH;
    matrix.fillScreen(0);

    int16_t textWidth = 6 * strlen(txt.c_str());

    matrix.setTextColor(colorText);
    while (textPos > -textWidth) {
        textPos--;
        matrix.fillScreen(0);
        matrix.setCursor(textPos, 0);
        matrix.print(txt);
        matrix.show();
        delay(sped);
    }

    matrix.setTextColor(0);
}

uint32_t getColor(String colorName) {
    Serial.println(colorName.c_str());
    if (colorName == "Red") return matrix.Color(128, 0, 0);
    if (colorName == "Green") return matrix.Color(0, 128, 0);
    if (colorName == "Blue") return matrix.Color(0, 0, 128);
    if (colorName == "Yellow") return matrix.Color(128, 128, 0);
    return matrix.Color(128, 128, 128); // Default white
}