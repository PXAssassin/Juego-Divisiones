const int ledRojo = 2;
const int ledVerde = 3;
const int buttonPin = 4;

const int switch_1 = 5;
const int switch_2 = 6;
const int switch_3 = 7;
const int switch_4 = 8;
const int switch_5 = 9;
const int switch_6 = 10;
const int switch_7 = 11;
const int switch_8 = 12;

int buttonState = 0;


void setup()
{
  Serial.begin(9600);
  
  pinMode(switch_1, INPUT);
  pinMode(switch_2, INPUT);
  pinMode(switch_3, INPUT);
  pinMode(switch_4, INPUT);
  pinMode(switch_5, INPUT);
  pinMode(switch_6, INPUT);
  pinMode(switch_7, INPUT);
  pinMode(switch_8, INPUT);

  pinMode(buttonPin, INPUT);
  
  pinMode(ledRojo, OUTPUT);
  pinMode(ledVerde, OUTPUT);
}

void loop()
{
  buttonState = digitalRead(buttonPin);

  if (buttonState == HIGH){

    //Serial.println(1);
    Serial.println(switches_activos());
    digitalWrite(ledVerde, HIGH);
    delay(1000);
    digitalWrite(ledVerde, LOW);
  }
  //Serial.println(analogRead(A0));
  //delay(2);
}

int switches_activos() {
  int activos = 0;
  int switches[] = {switch_1, switch_2, switch_3, switch_4, switch_5, switch_6, switch_7, switch_8};

  // Recorremos los switches y sumamos los activos
  for (int i = 0; i < 8; i++) {
    if (digitalRead(switches[i]) == HIGH) {  // Si el switch está activo (HIGH)
      activos++;
    }
  }
  return activos;
}








