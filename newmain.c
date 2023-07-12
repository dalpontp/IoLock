/*
 * File:   newmain.c
 * Author: eupan
 *
 * Created on July 6, 2023, 10:25 PM
 */

// CONFIG
#pragma config FOSC = HS       // Oscillator Selection bits (XT oscillator)
#pragma config WDTE = OFF      // Watchdog Timer Enable bit (WDT disabled)
#pragma config PWRTE = ON      // Power-up Timer Enable bit (PWRT disabled)
#pragma config BOREN = ON     // Brown-out Reset Enable bit (BOR disabled)
#pragma config LVP = ON        // Low-Voltage (Single-Supply) In-Circuit Serial Programming Enable bit (RB3/PGM pin has PGM function; low-voltage programming enabled)
#pragma config CPD = OFF       // Data EEPROM Memory Code Protection bit (Data EEPROM code protection off)
#pragma config WRT = OFF       // Flash Program Memory Write Enable bits (Write protection off; all program memory may be written to by EECON control)
#pragma config CP = OFF        // Flash Program Memory Code Protection bit (Code protection off)

#define _XTAL_FREQ 8*1000000


#include <stdio.h>
#include <xc.h>
#include <time.h> // per ottenere il timestamp
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

void Lcd_Port(char);
void Lcd_Cmd(char);
void Lcd_Init();
void Lcd_Clear();
void Lcd_Set_Cursor(char, char);
void Lcd_Write_Char(char);
void Lcd_Write_String(char*);
void Lcd_Shift_Right();
void Lcd_Shift_Left();
char* Lcd_Write_Int(int);
char Numpad_Read();
void UART_TxString(const char*);
void UART_init(long int);
void UART_TxInt(int);

//numero random

unsigned int generate_random_number() {
    unsigned int seed;
    int numrand;
    // Leggi il valore corrente del timer
    seed = TMR0;

    // Utilizza il valore del timer come seme per il generatore di numeri casuali
    srand(seed);

    // Genera un numero casuale di 5 cifre
    numrand = rand() % 89999 + 10000;
    if (numrand < 0) { //ritorno solo numeri positivi
        numrand = -numrand;
    }

    return numrand;
}
//per LA SERIALE
unsigned char dato[19]; //ricevuto in risposta da gateway
unsigned char received, counterFinished, receivedBytes;
char i;
unsigned int Counter;

void main(void) {
    //inizializzo la seriale
    INTCON |= 0xA0; // GIE = 1; T0IE = 1;
    OPTION_REG |= 0x85; // PS2 = 1; PS1 = 0; PS0 = 0;
    UART_init(115200);
    //inizializzo LCD
    Lcd_Init();
    Lcd_Set_Cursor(0, 3);
    Lcd_Write_String("Premi *");

    unsigned char operatore [2];
    char selettore, risultato;
    char read = 0xff;

    //NUMRAND
    // Inizializza il timer
    TMR0 = 0;
    OPTION_REGbits.T0CS = 0; // Usa il clock interno come sorgente per il timer
    OPTION_REGbits.PSA = 0; // Assegna il prescaler al timer
    OPTION_REGbits.PS = 0b111; // Imposta il prescaler a 256

    // Genera un numero casuale di 5 cifre
    unsigned int numrand;
    char sendpacket[19]; //pacchetto da inviare 
    char examplepacket[] = "100/200/0/12345"; //id pic/ id gateway/ 0 pic invia; 1 gateway invia/ payload
    char receivedpacket[] = "";
    char *ptr = examplepacket; //sendpacket;
    char idpic[] = "99";
    char idgateway[] = "200";
    char sep = '/';
    char inviofrompic = '0';
    char endpacket = '$';
    char payload[] = ""; //se dimentico le virgolette per inizializzare si schianta tutto
    bool richiesta = false; //check perché quando é stato richiesto aspetti solo la risposta, nessun nuovo invio di codice
    char pwreceived[5];

    char *token;


    while (1) {

        //SERIALE
        //        if (received) { //se ricevo 
        //            Lcd_Set_Cursor(1, receivedBytes++);
        //            if (receivedBytes > 16) {
        //                Lcd_Set_Cursor(1, 0);
        //                Lcd_Write_String("                ");
        //                Lcd_Set_Cursor(1, 0);
        //                receivedBytes = 1;
        //            }
        //            Lcd_Write_String("CAZZO BILLY");
        //            //Lcd_Write_Char(dato[0]);
        //            i = 0;
        //            received = 0;
        //        } 
        //        else { //non ricevo ma invio
        //            read = Numpad_Read();
        //            if (read == '*') {
        //                Lcd_Clear();
        //                numrand = generate_random_number();
        //                Lcd_Write_Int(numrand); //stampo il numero casuale
        //                UART_TxString(*ptr);
        //            }
        //            if (received) {
        //                Lcd_Clear();    
        //                //Lcd_Write_String("PORTA APERTA");
        //                Lcd_Write_Char('3');
        //                received=0;
        //                
        //        }
        // else { //non ricevo ma invio
        read = Numpad_Read();
        if (read == '*' && richiesta == false) {
            Lcd_Clear();
            numrand = generate_random_number();
            Lcd_Set_Cursor(0, 3);
            Lcd_Write_Int(numrand); //stampo il numero casuale
            //NON FUNZIA BENE
            //                strcat(sendpacket,idpic); //char examplepacket[] = "100/200/0/12345"; 
            //                strcat(sendpacket,sep); //sto costruendo il pacchetto da inviare
            //                strcat(sendpacket,idgateway);
            //                strcat(sendpacket,sep);
            //                strcat(sendpacket,'0');
            //                strcat(sendpacket,sep);
            //                strcat(sendpacket,numrand); //payload
            sprintf(payload, "%d", numrand); //salvo numrand in stringa
            sprintf(sendpacket, "%s%c%s%c%c%c%s%c", idpic, sep, idgateway, sep, inviofrompic, sep, payload, endpacket); //creo pacchetto da mandare
            UART_TxString(sendpacket); //funzia bene al primo click dell'asterisco
            richiesta = true; //ho richiesto l'accesso, mi metto in modalitá ricezione

        } else if (richiesta == true && received) { //se sto aspettando l'apertura porta
            Lcd_Clear();
            Lcd_Set_Cursor(0, 3);
            Lcd_Write_String("INSERISCI CODICE OTP");
            read = Numpad_Read();
            token = strtok(dato, '/'); //suddivido il dato ricevuto per elementi
            while (token != NULL) {
                if (i == 3) { //indice 3 perché campo payliad (pw))
                    pwreceived=token;
                }
                token = strtok(NULL, '/');
                i++;
            }
            if (strcmp(pwreceived, read) == 0) { //se codice messo da cloud corrisponde al codice inserito nel pic faccio accedere
                Lcd_Write_String("BENVENUTO");
            }
            else("ACCESSO NEGATO");

            
            received = 0;
        }
        //          else
        //        {
        //            Lcd_Clear();
        //            Lcd_Write_String("waiting for the permission");
        //            
        //        }
        //        if (counterFinished) {
        //            Lcd_Set_Cursor(0, 0);
        //            Lcd_Write_Int(Counter);
        //            UART_TxInt(Counter);
        //            counterFinished = 0;
        //        }

        //        if (read >= 0 && read <= 9) {
        //            if (!selettore) Lcd_Clear();
        //            if (selettore) Lcd_Set_Cursor(0, 2);
        //            operatore[selettore] = read;
        //            Lcd_Write_Int(operatore[selettore]);
        //        } else if (read == '*') {
        //            if (selettore < 1) {
        //                selettore++;
        //                Lcd_Write_Char('+');
        //            }
        //        } else if (read == '#') {
        //            if (selettore < 1) continue;
        //            risultato = operatore[0] + operatore[1];
        //            selettore = 0;
        //            Lcd_Write_Char('=');
        //            Lcd_Write_Int(risultato);
        //        }

    }
}

//char* convert_int_to_string(int number) {
//    char* str = malloc(12);  // Allocazione dinamica della memoria per la stringa
//    sprintf(str, "%d", number);
//    return str;
//}


//SERIALE

void UART_init(long int baudrate) {

    TXSTA |= 0x24;
    RCSTA = 0x90;
    SPBRG = (char) (_XTAL_FREQ / (long) (64UL * baudrate)) - 1;
    INTCON |= 0x80;
    INTCON |= 0x40;
    PIE1 |= 0x20;
}

void UART_TxChar(char ch) {
    TRISC &= ~0x40;
    TRISC |= 0x80;
    while (!(PIR1 & 0x10));
    PIR1 &= ~0x10;
    TXREG = ch; //copio il char nel registro TXREG che me lo trasmetterá

}

void UART_TxString(const char* str) {
    unsigned char i = 0;
    while (str[i] != 0) {
        UART_TxChar(str[i]);
        i++;
    }
}

void UART_TxInt(int val) {
    int n = val;
    char buffer[50];
    int i = 0;
    char isNeg = n < 0;

    unsigned int n1 = isNeg ? -n : n;

    while (n1 != 0) {
        buffer[i++] = n1 % 10 + '0';
        n1 = n1 / 10;
    }

    if (isNeg)
        buffer[i++] = '-';

    buffer[i] = '\0';

    for (int t = 0; t < i / 2; t++) {
        buffer[t] ^= buffer[i - t - 1];
        buffer[i - t - 1] ^= buffer[t];
        buffer[t] ^= buffer[i - t - 1];
    }

    if (n == 0) {
        buffer[0] = '0';
        buffer[1] = '\0';
    }
    UART_TxString(buffer);

}

void __interrupt()ISR() {
    if (RCIF) { //se va ad 1 ricevuto qualcosa
        dato[i++] = RCREG; //salvo il dato ricevuto 
        dato[i] = '\0';
        received = 1; //ricevuto qualcosa


        RCIF = 0; //resetto il registro
    }
    if (T0IF) { //? maybe timer
        static unsigned int interruptCounter;
        interruptCounter++;
        T0IF = 0;
        if (interruptCounter > 625) {
            Counter++;
            interruptCounter = 0;
            counterFinished = 1;

        }
        TMR0 = 131;
    }
}
#define RS PORTEbits.RE2
#define EN PORTEbits.RE1
#define D4 PORTDbits.RD4
#define D5 PORTDbits.RD5
#define D6 PORTDbits.RD6
#define D7 PORTDbits.RD7

//LCD Header per indirizzamento a 4 bit 

void Lcd_Port(char a) {

    if (a & 1) D4 = 1;
    else D4 = 0;

    if (a & 2) D5 = 1;
    else D5 = 0;

    if (a & 4) D6 = 1;
    else D6 = 0;

    if (a & 8) D7 = 1;
    else D7 = 0;
}

void Lcd_Cmd(char a) {
    TRISD &= ~0xff;
    TRISE &= ~0x06;


    RS = 0; // Invio comando
    Lcd_Port(a);
    EN = 1;
    __delay_ms(4);
    EN = 0;
}

void Lcd_Init() {
    TRISD &= ~0xff;
    TRISE &= ~0x06;
    Lcd_Port(0x00);
    __delay_ms(20);
    Lcd_Cmd(0x03);
    __delay_ms(5);
    Lcd_Cmd(0x03);
    __delay_ms(10);
    Lcd_Cmd(0x03);

    Lcd_Cmd(0x02); //LCD pilotato con 4 linee

    Lcd_Cmd(0x02); //comando
    Lcd_Cmd(0x08); //a due righe

    Lcd_Cmd(0x00); //accendo display
    Lcd_Cmd(0x0C); //e aspegni cursore  

    Lcd_Cmd(0x00);
    Lcd_Cmd(0x06); //incremento il cursore
}

void Lcd_Clear() // Cancella LCD
{
    Lcd_Cmd(0);
    Lcd_Cmd(1);
}

void Lcd_Set_Cursor(char riga, char colonna) {
    char temp, z, y;
    if (riga == 0) {
        temp = 0x80 + colonna;
        z = temp >> 4; // z = 4 bit piu' significativi
        y = temp & 0x0F; // y = 4 bit meno significativi
        Lcd_Cmd(z);
        Lcd_Cmd(y);
    } else if (riga >= 1) {
        temp = 0xC0 + colonna;
        z = temp >> 4;
        y = temp & 0x0F;
        Lcd_Cmd(z);
        Lcd_Cmd(y);
    }
}

void Lcd_Write_Char(char a) {
    char temp, y;
    temp = a & 0x0F;
    y = a & 0xF0;

    RS = 1; // Invio a

    Lcd_Port(y >> 4); //shift pin
    EN = 1;
    __delay_us(4);
    EN = 0;
    Lcd_Port(temp);
    EN = 1;
    __delay_us(4);
    EN = 0;
}

void Lcd_Write_String(char *a) {
    int i;
    for (i = 0; a[i] != '\0'; i++)//
        Lcd_Write_Char(a[i]);
}

void Lcd_Shift_Right() {
    Lcd_Cmd(0x01);
    Lcd_Cmd(0x0C);
}

void Lcd_Shift_Left() {
    Lcd_Cmd(0x01);
    Lcd_Cmd(0x08);
}

char* Lcd_Write_Int(int val) {

    int n = val;
    char buffer[50];
    int i = 0;
    char isNeg = n < 0;

    unsigned int n1 = isNeg ? -n : n;

    while (n1 != 0) {
        buffer[i++] = n1 % 10 + '0';
        n1 = n1 / 10;
    }

    if (isNeg)
        buffer[i++] = '-';

    buffer[i] = '\0';

    for (int t = 0; t < i / 2; t++) {
        buffer[t] ^= buffer[i - t - 1];
        buffer[i - t - 1] ^= buffer[t];
        buffer[t] ^= buffer[i - t - 1];
    }

    if (n == 0) {
        buffer[0] = '0';
        buffer[1] = '\0';
    }

    Lcd_Write_String(buffer);

}

//numpad
#define COLPORT PORTB
#define ROWPORT PORTD


#define X_1    RD3
#define X_2    RD2
#define X_3    RD1
#define X_4    RD0
#define Y_1    RB0
#define Y_2    RB1
#define Y_3    RB2

const unsigned char keypad [] = {'*', 7, 4, 1, 0, 8, 5, 2, '#', 9, 6, 3};

char Numpad_Read() {

    TRISD |= 0x0f;
    TRISB &= ~0x07;
    char colScan, rowScan, currentKeyVal, currentKey;
    static char oldKeyVal;
    for (colScan = 0; colScan < 3; colScan++) {
        COLPORT |= 0x07;
        COLPORT &= ~(1 << colScan);
        __delay_ms(15);
        for (rowScan = 0; rowScan < 4; rowScan++) {
            currentKeyVal = (ROWPORT & (1 << rowScan));

            if (!currentKeyVal && oldKeyVal) {
                currentKey = keypad[rowScan + (4 * colScan)];
                oldKeyVal = 0;
                while (!currentKeyVal) {
                    currentKeyVal = (ROWPORT & (1 << rowScan));
                    __delay_ms(20);
                }
                return currentKey;
            }

            oldKeyVal = 1;
        }
    }
    return 0xff;

}