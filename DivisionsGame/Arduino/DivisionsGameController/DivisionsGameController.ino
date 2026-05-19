const int ledRojo = 23;
const int ledVerde = 18;
const int buttonPin = 19;

const int switch_1 = 13;
const int switch_2 = 12;
const int switch_3 = 14;
const int switch_4 = 27;
const int switch_5 = 26;
const int switch_6 = 25;
const int switch_7 = 32;
const int switch_8 = 33;

const unsigned long intervaloEnvioMs = 50;
const unsigned long tiempoLedMs = 700;
const unsigned long debounceBotonMs = 35;

unsigned long ultimoEnvio = 0;
unsigned long apagarLedEn = 0;

int lecturaBotonAnterior = HIGH;
int estadoBotonEstable = HIGH;
unsigned long ultimoCambioBoton = 0;

void setup()
{
  Serial.begin(9600);

  pinMode(switch_1, INPUT_PULLUP);
  pinMode(switch_2, INPUT_PULLUP);
  pinMode(switch_3, INPUT_PULLUP);
  pinMode(switch_4, INPUT_PULLUP);
  pinMode(switch_5, INPUT_PULLUP);
  pinMode(switch_6, INPUT_PULLUP);
  pinMode(switch_7, INPUT_PULLUP);
  pinMode(switch_8, INPUT_PULLUP);

  pinMode(buttonPin, INPUT_PULLUP);

  pinMode(ledRojo, OUTPUT);
  pinMode(ledVerde, OUTPUT);
}

void loop()
{
  enviarEstadoAUnity();
  leerRespuestaDeUnity();
  actualizarLeds();
}

void enviarEstadoAUnity()
{
  unsigned long ahora = millis();

  if (ahora - ultimoEnvio < intervaloEnvioMs)
  {
    return;
  }

  ultimoEnvio = ahora;

  Serial.print("S=");
  Serial.print(switchesActivos());
  Serial.print(";B=");
  Serial.println(botonPresionado() ? 1 : 0);
}

void leerRespuestaDeUnity()
{
  while (Serial.available() > 0)
  {
    char respuesta = Serial.read();

    if (respuesta == '1')
    {
      encenderLedResultado(ledVerde);
    }
    else if (respuesta == '0')
    {
      encenderLedResultado(ledRojo);
    }
  }
}

void encenderLedResultado(int led)
{
  digitalWrite(ledRojo, LOW);
  digitalWrite(ledVerde, LOW);
  digitalWrite(led, HIGH);
  apagarLedEn = millis() + tiempoLedMs;
}

void actualizarLeds()
{
  if (apagarLedEn > 0 && millis() >= apagarLedEn)
  {
    digitalWrite(ledRojo, LOW);
    digitalWrite(ledVerde, LOW);
    apagarLedEn = 0;
  }
}

int switchesActivos()
{
  int activos = 0;
  int switches[] = { switch_1, switch_2, switch_3, switch_4, switch_5, switch_6, switch_7, switch_8 };

  for (int i = 0; i < 8; i++)
  {
    if (pinActivo(switches[i]))
    {
      activos++;
    }
  }

  return activos;
}

bool pinActivo(int pin)
{
  return digitalRead(pin) == LOW;
}

bool botonPresionado()
{
  int lecturaActual = digitalRead(buttonPin);

  if (lecturaActual != lecturaBotonAnterior)
  {
    ultimoCambioBoton = millis();
    lecturaBotonAnterior = lecturaActual;
  }

  if (millis() - ultimoCambioBoton >= debounceBotonMs)
  {
    estadoBotonEstable = lecturaActual;
  }

  return estadoBotonEstable == LOW;
}
