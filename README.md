# README - Backend

## Tecnologie Utilizzate

### Frontend
- **React**
- **Redux Toolkit** *(state management)*
- **TypeScript**
- **React Bootstrap** *(UI components)*

### Backend
- **C# .NET**
- **Entity Framework Core**
- **JWT Authentication**
- **Swagger** *(API Documentation)*

### Database
- **SQL Database**

---

## API Routes

Le sezioni che seguiranno descrivono le rotte attualmente utilizzate nel progetto, all'interno del backend troverete comunque i commenti con "Used" vicino che significano appunto che sono usati nel front o che almeno sono implementati già all'interno di esso.
Il tutto è fatto in local host
https://localhost:7124/
---

## Auth API Routes

### Register Customer

Permette la registrazione di un nuovo utente con ruolo **Customer**.

- **Endpoint:** `POST /register/customer`
- **Accesso:** Pubblico `AllowAnonymous`

#### Request Body
```json
{
  "firstName": "Mario",
  "lastName": "Rossi",
  "email": "mario@email.com",
  "password": "Password123!"
}
```

#### Response (200 OK)
```json
{
  "token": "jwt_token_here",
  "username": "mario@email.com",
  "email": "mario@email.com",
  "firstName": "Mario",
  "lastName": "Rossi",
  "role": "Customer",
  "expiration": "2026-02-13T12:00:00Z"
}
```

---

### Login

Autenticazione utente tramite email e password.

- **Endpoint:** `POST /login`
- **Accesso:** Pubblico `AllowAnonymous`

#### Request Body
```json
{
  "email": "mario@email.com",
  "password": "Password123!"
}
```

#### Response (200 OK)
```json
{
  "token": "jwt_token_here",
  "username": "mario@email.com",
  "email": "mario@email.com",
  "firstName": "Mario",
  "lastName": "Rossi",
  "role": "Customer",
  "expiration": "2026-02-13T12:00:00Z"
}
```

---

### Register Staff

Permette ad un Admin di creare un account staff.

- **Endpoint:** `POST /register/staff`
- **Accesso:** Solo Admin `Authorize(Roles = "Admin")`

#### Request Body
```json
{
  "firstName": "Luigi",
  "lastName": "Verdi",
  "email": "staff@email.com",
  "password": "Password123!",
  "role": "Receptionist"
}
```

#### Ruoli accettati

- Receptionist
- RoomStaff

#### Response (200 OK)
```json
{
  "token": "jwt_token_here",
  "username": "staff@email.com",
  "email": "staff@email.com",
  "firstName": "Luigi",
  "lastName": "Verdi",
  "role": "Receptionist",
  "expiration": "2026-02-13T12:00:00Z"
}
```

---

### Get All Staff

Restituisce la lista di tutti gli utenti staff registrati.

- **Endpoint:** `GET /staff`
- **Accesso:** Solo Admin `Authorize(Roles = "Admin")`

#### Response (200 OK)
```json
[
  {
    "id": "123",
    "username": "staff@email.com",
    "email": "staff@email.com",
    "firstName": "Luigi",
    "lastName": "Verdi",
    "role": "Receptionist",
    "isActive": true
  }
]
```

---

### Deactivate Staff (Soft Delete)

Disattiva un membro dello staff senza eliminarlo dal database.

- **Endpoint:** `PUT /staff/{id}/deactivate`
- **Accesso:** Solo Admin

#### Response

- `204 No Content` → Staff disattivato correttamente
- `404 Not Found` → Utente non trovato

---

### Reactivate Staff

Riattiva un membro dello staff precedentemente disattivato.

- **Endpoint:** `PUT /staff/{id}/reactivate`
- **Accesso:** Solo Admin

#### Response

- `204 No Content` → Staff riattivato correttamente
- `404 Not Found` → Utente non trovato

## Charge API Routes

Tutte le rotte sono protette e accessibili solo agli utenti con ruolo:
- `Admin`
- `Receptionist`
- `RoomStaff`

---

### Create Charge

Crea un nuovo addebito associato ad una prenotazione esistente.

- **Endpoint:** `POST /api/charge`
- **Accesso:** Admin, Receptionist, RoomStaff

#### Request Body
```json
{
  "reservationId": "guid_reservation_here",
  "description": "Minibar",
  "type": "Service",
  "unitPrice": 10.00,
  "quantity": 2,
  "vatRate": 0.22,
  "isInvoiced": false
}
```

#### Response (201 Created)
```json
{
  "id": "guid_charge_here",
  "reservationId": "guid_reservation_here",
  "description": "Minibar",
  "type": "Service",
  "unitPrice": 10.00,
  "quantity": 2,
  "amount": 20.00,
  "vatRate": 0.22,
  "isInvoiced": false,
  "createdBy": "System",
  "updatedBy": null
}
```

#### Errori possibili

- `400 Bad Request` → Reservation non trovata
- `500 Internal Server Error` → Errore generico lato server

---

### Update Charge

Aggiorna un addebito esistente.

- **Endpoint:** `PUT /api/charge/{id}`
- **Accesso:** Admin, Receptionist, RoomStaff

#### Request Body
```json
{
  "reservationId": "guid_reservation_here",
  "description": "Room Service",
  "type": "Service",
  "unitPrice": 25.00,
  "quantity": 1,
  "vatRate": 0.22,
  "isInvoiced": false
}
```

#### Response (200 OK)
```json
{
  "id": "guid_charge_here",
  "reservationId": "guid_reservation_here",
  "description": "Room Service",
  "type": "Service",
  "unitPrice": 25.00,
  "quantity": 1,
  "amount": 25.00,
  "vatRate": 0.22,
  "isInvoiced": false,
  "createdBy": "Admin User",
  "updatedBy": "Receptionist User"
}
```

#### Errori possibili

- `404 Not Found` → Charge non trovato
- `500 Internal Server Error` → Errore generico lato server

---

### Delete Charge

Elimina definitivamente un addebito (hard delete).

**Un charge non può essere eliminato se è già stato fatturato** (`isInvoiced = true`).

- **Endpoint:** `DELETE /api/charge/{id}`
- **Accesso:** Admin, Receptionist, RoomStaff

#### Response

- `204 No Content` → Charge eliminato correttamente

#### Errori possibili

- `404 Not Found` → Charge non trovato
- `400 Bad Request` → Impossibile eliminare un charge già fatturato
- `500 Internal Server Error` → Errore generico lato server

## Guest API Routes

Tutte le rotte sono protette e accessibili solo agli utenti con ruolo:
- `Admin`
- `Receptionist`

---

### Create Guest

Crea un nuovo ospite associato ad una prenotazione esistente.

- **Endpoint:** `POST /api/guest`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "reservationId": "guid_reservation_here",
  "firstName": "Mario",
  "lastName": "Rossi",
  "birthDate": "1990-05-10",
  "birthCity": "Roma",
  "citizenship": "Italiana",
  "role": "HeadOfFamily",
  "taxCode": "RSSMRA90E10H501X",
  "address": "Via Roma 10",
  "cityOfResidence": "Roma",
  "province": "RM",
  "postalCode": "00100",
  "documentType": "ID Card",
  "documentNumber": "AB1234567",
  "documentExpiration": "2030-01-01"
}
```

#### Response (200 OK)
```json
{
  "id": "guid_guest_here",
  "reservationId": "guid_reservation_here",
  "firstName": "Mario",
  "lastName": "Rossi",
  "birthDate": "1990-05-10",
  "birthCity": "Roma",
  "citizenship": "Italiana",
  "role": "HeadOfFamily",
  "taxCode": "RSSMRA90E10H501X",
  "address": "Via Roma 10",
  "cityOfResidence": "Roma",
  "province": "RM",
  "postalCode": "00100",
  "documentType": "ID Card",
  "documentNumber": "AB1234567",
  "documentExpiration": "2030-01-01"
}
```

#### Errori possibili

- `400 Bad Request` → Reservation non trovata oppure dati obbligatori mancanti
- `500 Internal Server Error` → Errore generico lato server

---

### Update Guest

Aggiorna i dati di un ospite esistente.

- **Endpoint:** `PUT /api/guest/{id}`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "firstName": "Mario",
  "lastName": "Bianchi",
  "cityOfResidence": "Milano",
  "role": "Single"
}
```

#### Response (200 OK)
```json
{
  "id": "guid_guest_here",
  "reservationId": "guid_reservation_here",
  "firstName": "Mario",
  "lastName": "Bianchi",
  "birthCity": "Roma",
  "citizenship": "Italiana",
  "role": "Single",
  "cityOfResidence": "Milano",
  "updatedBy": "Receptionist User",
  "updatedAt": "2026-02-13T12:00:00Z"
}
```

#### Errori possibili

- `404 Not Found` → Guest non trovato
- `400 Bad Request` → Dati non validi o regole ruolo non rispettate
- `500 Internal Server Error` → Errore generico lato server

---

### Delete Guest

Elimina definitivamente un ospite dal sistema (hard delete).

- **Endpoint:** `DELETE /api/guest/{id}`
- **Accesso:** Admin, Receptionist

#### Response

- `204 No Content` → Guest eliminato correttamente

#### Errori possibili

- `404 Not Found` → Guest non trovato
- `500 Internal Server Error` → Errore generico lato server

## Invoice API Routes

Le rotte sono protette e accessibili ai ruoli:
- `Admin`
- `Receptionist`
- `RoomStaff` (solo per visualizzare le fatture)

---

### Create Invoice

Crea una nuova fattura per una prenotazione esistente.

- **Endpoint:** `POST /api/invoice`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "reservationId": "guid_reservation_here",
  "customer": {
    "firstName": "Mario",
    "lastName": "Rossi",
    "taxCode": "RSSMRA90E10H501X",
    "address": "Via Roma 10",
    "city": "Roma",
    "country": "Italia"
  },
  "items": [
    {
      "description": "Room - Deluxe",
      "unitPrice": 100.00,
      "quantity": 2,
      "vatRate": 22
    },
    {
      "description": "Minibar",
      "unitPrice": 10.00,
      "quantity": 1,
      "vatRate": 22
    }
  ],
  "createdBy": "Admin User"
}
```

#### Response (201 Created)
```json
{
  "invoiceId": "guid_invoice_here"
}
```

#### Errori possibili

- `400 Bad Request` → Nessun item selezionato per la fatturazione o altri errori di validazione
- `404 Not Found` → Reservation non trovata

---

### Cancel Invoice

Annulla una fattura esistente.

- **Endpoint:** `POST /api/invoice/{invoiceId}/cancel`
- **Accesso:** Admin, Receptionist

#### Response

- `204 No Content` → Fattura annullata correttamente

#### Errori possibili

- `404 Not Found` → Fattura non trovata
- `400 Bad Request` → Fattura già annullata
- `500 Internal Server Error` → Errore generico lato server

---

### Get Invoice by Id

Restituisce i dettagli di una fattura.

- **Endpoint:** `GET /api/invoice/{invoiceId}`
- **Accesso:** Admin, Receptionist, RoomStaff

#### Response (200 OK)
```json
{
  "id": "guid_invoice_here",
  "reservationId": "guid_reservation_here",
  "invoiceNumber": "2026-0001",
  "status": "Issued",
  "issueDate": "2026-02-13T12:00:00Z",
  "subTotal": 110.00,
  "taxAmount": 24.20,
  "totalAmount": 134.20,
  "balanceDue": 134.20,
  "remainingAmount": 0.00,
  "customer": {
    "firstName": "Mario",
    "lastName": "Rossi",
    "taxCode": "RSSMRA90E10H501X",
    "address": "Via Roma 10",
    "city": "Roma",
    "country": "Italia"
  },
  "items": [
    {
      "id": "guid_item_here",
      "description": "Room - Deluxe",
      "unitPrice": 100.00,
      "quantity": 2,
      "totalPrice": 200.00,
      "vatRate": 22,
      "vatAmount": 36.07
    },
    {
      "id": "guid_item_here",
      "description": "Minibar",
      "unitPrice": 10.00,
      "quantity": 1,
      "totalPrice": 10.00,
      "vatRate": 22,
      "vatAmount": 1.80
    }
  ],
  "payments": [
    {
      "paymentId": "guid_payment_here",
      "amountApplied": 50.00,
      "createdAt": "2026-02-13T12:00:00Z"
    }
  ]
}
```

#### Errori possibili

- `404 Not Found` → Fattura non trovata

---

### Get Invoice PDF

Restituisce la fattura in formato HTML pronto per essere visualizzato o scaricato.

- **Endpoint:** `GET /api/invoice/{invoiceId}/pdf`
- **Accesso:** Admin, Receptionist

#### Response

- `200 OK` → File HTML della fattura
- `404 Not Found` → Fattura non trovata o file non generato

## Payment API Routes

Questa sezione descrive le rotte utilizzate per la gestione dei **pagamenti (Payments)**.

**Base URL:** `/api/payment`

Le rotte sono protette e accessibili ai ruoli:
- `Admin`
- `Receptionist`

---

### Create Payment

Crea un nuovo pagamento associato a una prenotazione esistente.

- **Endpoint:** `POST /api/payment`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "reservationId": "guid_reservation_here",
  "amount": 100.00,
  "type": "Cash",
  "paymentMethodId": "guid_paymentMethod_here",
  "paidAt": "2026-02-13T12:00:00Z",
  "createdBy": "Admin User"
}
```

#### Response (200 OK)
```json
{
  "id": "guid_payment_here",
  "reservationId": "guid_reservation_here",
  "amount": 100.00,
  "type": "Cash",
  "status": "Completed",
  "paymentMethodId": "guid_paymentMethod_here",
  "paymentMethodCode": "CASH",
  "paymentMethodDescription": "Cash Payment",
  "paidAt": "2026-02-13T12:00:00Z",
  "createdAt": "2026-02-13T12:00:00Z",
  "updatedAt": null,
  "deletedAt": null,
  "createdBy": "Admin User",
  "updatedBy": null,
  "deletedBy": null
}
```

#### Errori possibili

- `400 Bad Request` → Reservation non trovata, metodo di pagamento non attivo, importo <= 0

---

### Update Payment

Aggiorna un pagamento esistente.

- **Endpoint:** `PUT /api/payment/{id}`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "amount": 120.00,
  "type": "Card",
  "status": "Completed",
  "paymentMethodId": "guid_paymentMethod_here",
  "paidAt": "2026-02-13T13:00:00Z",
  "updatedBy": "Admin User"
}
```

#### Response (200 OK)
```json
{
  "id": "guid_payment_here",
  "reservationId": "guid_reservation_here",
  "amount": 120.00,
  "type": "Card",
  "status": "Completed",
  "paymentMethodId": "guid_paymentMethod_here",
  "paymentMethodCode": "CARD",
  "paymentMethodDescription": "Credit Card",
  "paidAt": "2026-02-13T13:00:00Z",
  "createdAt": "2026-02-13T12:00:00Z",
  "updatedAt": "2026-02-13T13:00:00Z",
  "deletedAt": null,
  "createdBy": "Admin User",
  "updatedBy": "Admin User",
  "deletedBy": null
}
```

#### Errori possibili

- `400 Bad Request` → Importo <= 0, metodo di pagamento non attivo
- `404 Not Found` → Pagamento non trovato

---

### Delete Payment

Elimina (soft delete) un pagamento esistente.

- **Endpoint:** `DELETE /api/payment/{id}`
- **Accesso:** Admin

#### Response

- `204 No Content` → Pagamento eliminato correttamente

#### Errori possibili

- `404 Not Found` → Pagamento non trovato

## Payment & Payment Method API

### Base URLs

- **Payment:** `/api/payment`
- **Payment Method:** `/api/paymentmethod`

Le rotte sono protette da ruoli e richiedono l'autenticazione:
- `Admin`
- `Receptionist`
- `RoomStaff` (solo per alcune rotte di lettura)

---

## Payment API Routes

### Create Payment

Crea un nuovo pagamento per una prenotazione esistente.

- **Endpoint:** `POST /api/payment`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "reservationId": "guid_reservation_here",
  "amount": 100.00,
  "type": "Cash",
  "paymentMethodId": "guid_paymentMethod_here",
  "paidAt": "2026-02-13T12:00:00Z",
  "createdBy": "Admin User"
}
```

#### Response (200 OK)
```json
{
  "id": "guid_payment_here",
  "reservationId": "guid_reservation_here",
  "amount": 100.00,
  "type": "Cash",
  "status": "Completed",
  "paymentMethodId": "guid_paymentMethod_here",
  "paymentMethodCode": "CASH",
  "paymentMethodDescription": "Cash Payment",
  "paidAt": "2026-02-13T12:00:00Z",
  "createdAt": "2026-02-13T12:00:00Z",
  "updatedAt": null,
  "deletedAt": null,
  "createdBy": "Admin User",
  "updatedBy": null,
  "deletedBy": null
}
```

#### Errori possibili

- `400 Bad Request` → Reservation non trovata, metodo di pagamento non attivo o importo <= 0

---

### Update Payment

Aggiorna un pagamento esistente.

- **Endpoint:** `PUT /api/payment/{id}`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "amount": 120.00,
  "type": "Card",
  "status": "Completed",
  "paymentMethodId": "guid_paymentMethod_here",
  "paidAt": "2026-02-13T13:00:00Z",
  "updatedBy": "Admin User"
}
```

#### Response (200 OK)
```json
{
  "id": "guid_payment_here",
  "reservationId": "guid_reservation_here",
  "amount": 120.00,
  "type": "Card",
  "status": "Completed",
  "paymentMethodId": "guid_paymentMethod_here",
  "paymentMethodCode": "CARD",
  "paymentMethodDescription": "Credit Card",
  "paidAt": "2026-02-13T13:00:00Z",
  "createdAt": "2026-02-13T12:00:00Z",
  "updatedAt": "2026-02-13T13:00:00Z",
  "deletedAt": null,
  "createdBy": "Admin User",
  "updatedBy": "Admin User",
  "deletedBy": null
}
```

#### Errori possibili

- `400 Bad Request` → Importo <= 0, metodo di pagamento non attivo
- `404 Not Found` → Pagamento non trovato

---

### Delete Payment

Elimina un pagamento (soft delete).

- **Endpoint:** `DELETE /api/payment/{id}`
- **Accesso:** Admin

#### Response

- `204 No Content` → Pagamento eliminato correttamente

#### Errori possibili

- `404 Not Found` → Pagamento non trovato

---

## Payment Method API Routes

### Get All Payment Methods

Restituisce la lista di tutti i metodi di pagamento.

- **Endpoint:** `GET /api/paymentmethod?includeInactive=false`
- **Accesso:** Admin, Receptionist, RoomStaff

#### Query Parameters

- `includeInactive` (opzionale, default `false`) → se `true` restituisce anche i metodi disattivati

#### Response (200 OK)
```json
[
  {
    "id": "guid_method_here",
    "code": "CASH",
    "description": "Cash Payment",
    "isActive": true
  },
  {
    "id": "guid_method_here",
    "code": "CARD",
    "description": "Credit Card",
    "isActive": true
  }
]
```

---

### Create Payment Method

Crea un nuovo metodo di pagamento.

- **Endpoint:** `POST /api/paymentmethod`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "code": "CARD",
  "description": "Credit Card"
}
```

#### Response (200 OK)
```json
{
  "id": "guid_method_here",
  "code": "CARD",
  "description": "Credit Card",
  "isActive": true
}
```

#### Errori possibili

- `400 Bad Request` → Codice già esistente

---

### Update Payment Method

Aggiorna un metodo di pagamento esistente.

- **Endpoint:** `PUT /api/paymentmethod/{id}`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "description": "Updated Description",
  "isActive": true
}
```

#### Response (200 OK)
```json
{
  "id": "guid_method_here",
  "code": "CARD",
  "description": "Updated Description",
  "isActive": true
}
```

#### Errori possibili

- `404 Not Found` → Metodo non trovato

---

### Deactivate Payment Method

Disattiva un metodo di pagamento senza eliminarlo.

- **Endpoint:** `DELETE /api/paymentmethod/{id}`
- **Accesso:** Admin

#### Response

- `204 No Content` → Metodo disattivato correttamente

#### Errori possibili

- `404 Not Found` → Metodo non trovato

## Room API

### Base URL

`/api/room`

Le rotte sono protette da ruoli e autenticazione, salvo diversamente indicato.

---

## Room API Routes

### Create Room

Crea una nuova stanza.

- **Endpoint:** `POST /api/room`
- **Accesso:** Admin

#### Request Body
```json
{
  "roomNumber": "101",
  "roomName": "Deluxe Suite",
  "description": "Suite con vista mare",
  "beds": 2,
  "bedsTypes": "King, Queen",
  "priceForNight": 150.00
}
```

#### Response (201 Created)
```json
{
  "id": "guid_room_here",
  "roomNumber": "101",
  "roomName": "Deluxe Suite",
  "description": "Suite con vista mare",
  "beds": 2,
  "bedsTypes": "King, Queen",
  "priceForNight": 150.00
}
```

---

### Update Room

Aggiorna i dati di una stanza esistente.

- **Endpoint:** `PUT /api/room/{id}`
- **Accesso:** Admin

#### Request Body
```json
{
  "roomNumber": "102",
  "roomName": "Executive Suite",
  "description": "Suite ristrutturata",
  "beds": 3,
  "bedsTypes": "King, Queen, Twin",
  "priceForNight": 200.00
}
```

#### Response (200 OK)
```json
{
  "id": "guid_room_here",
  "roomNumber": "102",
  "roomName": "Executive Suite",
  "description": "Suite ristrutturata",
  "beds": 3,
  "bedsTypes": "King, Queen, Twin",
  "priceForNight": 200.00
}
```

#### Errori possibili

- `404 Not Found` → Stanza non trovata
- `500 Internal Server Error` → Errore interno durante l'aggiornamento

---

### Delete Room

Elimina una stanza (soft delete inclusi foto).

- **Endpoint:** `DELETE /api/room/{id}`
- **Accesso:** Admin

#### Response

- `204 No Content` → Stanza eliminata

#### Errori possibili

- `404 Not Found` → Stanza non trovata

---

### Get Room by Id

Recupera i dettagli di una stanza.

- **Endpoint:** `GET /api/room/{id}`
- **Accesso:** Pubblico

#### Response (200 OK)
```json
{
  "id": "guid_room_here",
  "roomNumber": "101",
  "roomName": "Deluxe Suite",
  "description": "Suite con vista mare",
  "beds": 2,
  "bedsTypes": "King, Queen",
  "priceForNight": 150.00,
  "photos": [
    {
      "id": "guid_photo_here",
      "url": "/uploads/rooms/photo1.jpg",
      "isCover": true
    }
  ]
}
```

#### Errori possibili

- `404 Not Found` → Stanza non trovata

---

### Get All Rooms

Recupera tutte le stanze disponibili.

- **Endpoint:** `GET /api/room`
- **Accesso:** Pubblico

#### Response (200 OK)
```json
[
  {
    "id": "guid_room_here",
    "roomNumber": "101",
    "roomName": "Deluxe Suite",
    "description": "Suite con vista mare",
    "beds": 2,
    "bedsTypes": "King, Queen",
    "priceForNight": 150.00,
    "coverPhotoUrl": "/uploads/rooms/photo1.jpg"
  }
]
```

---

### Get Room Calendar

Restituisce calendario prenotazioni per tutte le stanze in un intervallo.

- **Endpoint:** `GET /api/room/calendar?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD`
- **Accesso:** Admin, Receptionist, RoomStaff

#### Response (200 OK)
```json
[
  {
    "roomId": "guid_room_here",
    "roomNumber": "101",
    "roomName": "Deluxe Suite",
    "roomPrice": 150.00,
    "reservations": [
      {
        "reservationId": "guid_reservation_here",
        "checkIn": "2026-02-15",
        "checkOut": "2026-02-20",
        "guestName": "Mario Rossi",
        "status": "Confirmed",
        "startsBeforeRange": false,
        "endsAfterRange": false
      }
    ]
  }
]
```

---

### Get Available Rooms

Restituisce le stanze libere in un intervallo.

- **Endpoint:** `GET /api/room/available?checkIn=YYYY-MM-DD&checkOut=YYYY-MM-DD`
- **Accesso:** Pubblico

#### Response (200 OK)
```json
[
  {
    "id": "guid_room_here",
    "roomNumber": "102",
    "roomName": "Executive Suite",
    "description": "Suite ristrutturata",
    "beds": 3,
    "bedsTypes": "King, Queen, Twin",
    "priceForNight": 200.00,
    "coverPhotoUrl": "/uploads/rooms/photo2.jpg"
  }
]
```

#### Errori possibili

- `400 Bad Request` → Check-out precedente al check-in

---

## Room Photo API Routes

### Add Photo to Room

Aggiunge una foto a una stanza.

- **Endpoint:** `POST /api/room/{id}/photos`
- **Accesso:** Admin

#### FormData

- `file` → file immagine (.jpg, .jpeg, .png, .webp)
- `isCover` → booleano (true se foto copertina)

#### Response

- `200 OK` → Foto aggiunta
- `400/500` → Tipo file non valido o stanza non trovata

---

### Delete Photo

Elimina una foto di una stanza.

- **Endpoint:** `DELETE /api/room/photos/{photoId}`
- **Accesso:** Admin

#### Response

- `204 No Content` → Foto eliminata

#### Errori possibili

- `404 Not Found` → Foto non trovata

---

### Set Cover Photo

Imposta una foto come copertina della stanza.

- **Endpoint:** `PATCH /api/room/photos/{photoId}/cover`
- **Accesso:** Admin

#### Response

- `204 No Content` → Copertina impostata

#### Errori possibili

- `404 Not Found` → Foto non trovata

## Reservation API

### Base URL

`/api/reservation`

Le rotte sono protette da ruoli e autenticazione, salvo diversamente indicato.

---

## Reservation API Routes

### Create Reservation

Crea una nuova prenotazione.

- **Endpoint:** `POST /api/reservation`
- **Accesso:** Pubblico (Anonymous)
  - Se loggato come Admin o Receptionist, può associare la prenotazione a un cliente.

#### Request Body
```json
{
  "firstName": "Mario",
  "lastName": "Rossi",
  "phone": "3331234567",
  "email": "mario.rossi@email.com",
  "roomId": "guid_room_here",
  "checkIn": "2026-02-20",
  "checkOut": "2026-02-25",
  "note": "Richiesta vista mare"
}
```

#### Response (201 Created)
```json
{
  "id": "guid_reservation_here",
  "customerName": "Mario Rossi",
  "phone": "3331234567",
  "email": "mario.rossi@email.com",
  "roomId": "guid_room_here",
  "roomNumber": "101",
  "checkIn": "2026-02-20",
  "checkOut": "2026-02-25",
  "status": "Pending",
  "note": "Richiesta vista mare"
}
```

#### Errori possibili

- `400 Bad Request` → Date non valide, stanza occupata o stanza non esistente

---

### Update Reservation

Aggiorna una prenotazione esistente.

- **Endpoint:** `PUT /api/reservation/{id}`
- **Accesso:** Admin, Receptionist

#### Request Body
```json
{
  "firstName": "Mario",
  "lastName": "Rossi",
  "phone": "3331234567",
  "email": "mario.rossi@email.com",
  "roomId": "guid_room_here",
  "checkIn": "2026-02-21",
  "checkOut": "2026-02-26",
  "status": "Confirmed",
  "note": "Richiesta cambio camera"
}
```

#### Response (200 OK)
```json
{
  "id": "guid_reservation_here",
  "customerName": "Mario Rossi",
  "phone": "3331234567",
  "email": "mario.rossi@email.com",
  "roomId": "guid_room_here",
  "roomNumber": "101",
  "checkIn": "2026-02-21",
  "checkOut": "2026-02-26",
  "status": "Confirmed",
  "note": "Richiesta cambio camera"
}
```

#### Errori possibili

- `404 Not Found` → Prenotazione non trovata
- `400 Bad Request` → Date non valide o stanza occupata

---

### Get Reservation by Id

Recupera i dettagli di una prenotazione.

- **Endpoint:** `GET /api/reservation/{id}`
- **Accesso:** Admin, Receptionist, RoomStaff

#### Response (200 OK)
```json
{
  "id": "guid_reservation_here",
  "customerName": "Mario Rossi",
  "phone": "3331234567",
  "email": "mario.rossi@email.com",
  "roomId": "guid_room_here",
  "roomNumber": "101",
  "checkIn": "2026-02-21",
  "checkOut": "2026-02-26",
  "status": "Confirmed",
  "note": "Richiesta cambio camera",
  "paymentStatus": "NotPaid",
  "remainingAmount": 750.00,
  "isRoomInvoiced": false,
  "guests": [],
  "payments": [],
  "charges": [],
  "invoices": []
}
```

#### Errori possibili

- `404 Not Found` → Prenotazione non trovata

---

### Cancel Reservation (Soft Delete)

Annulla una prenotazione.

- **Endpoint:** `DELETE /api/reservation/{id}`
- **Accesso:** Admin, Receptionist

#### Response

- `204 No Content` → Prenotazione cancellata

#### Errori possibili

- `404 Not Found` → Prenotazione non trovata
- `400 Bad Request` → Prenotazione contiene addebiti fatturati o fatture emesse

---

### Search Reservations

Ricerca prenotazioni in base a filtri.

- **Endpoint:** `POST /api/reservation/search`
- **Accesso:** Admin, Receptionist, RoomStaff

#### Request Body (esempio filtri)
```json
{
  "customerName": "Mario Rossi",
  "email": "mario.rossi@email.com",
  "phone": "3331234567",
  "roomId": "guid_room_here",
  "status": "Confirmed",
  "fromDate": "2026-02-01",
  "toDate": "2026-02-28"
}
```

#### Response (200 OK)
```json
[
  {
    "id": "guid_reservation_here",
    "customerName": "Mario Rossi",
    "phone": "3331234567",
    "email": "mario.rossi@email.com",
    "roomId": "guid_room_here",
    "roomNumber": "101",
    "checkIn": "2026-02-21",
    "checkOut": "2026-02-26",
    "status": "Confirmed"
  }
]
```
