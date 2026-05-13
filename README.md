# SurveyApp Backend

## 1 Opis
Backend aplikacji SurveyApp napisany w ASP.NET Core Web API.
Aplikacja umożliwiająca logowanie i rejestrację użytkowników. Po zalogowaniu użytkownicy mają do wyboru: tworzenie, przeglądanie i wypełnianie ankiet oraz analizę wyników przedstawionych za pomocą wykresów.



## 2 Funkcjonalności

- Rejestracja i logowanie użytkowników
- Autoryzacja JWT
- Tworzenie ankiet
- Obsługa pytań tekstowych
- Obsługa pytań jednokrotnego wyboru
- Obsługa pytań wielokrotnego wyboru
- Zapisywanie odpowiedzi użytkowników
- Analiza wyników ankiet
- Statystyki i agregacja odpowiedzi



## 3 Tech Stack

- C#
- ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Identity
- JWT Authentication
- SQL Server
- LINQ


## 4 Klonowanie repozytorium

```bash
git clone https://github.com/xMufc/SurveyAppBackend
cd SurveyAppBackend
```

## 5 Konfiguracja bazy banych

Należy zmienić ```"ConnectionStrings"``` w pliku ```appsettings.json```


## 6 Uruchomienie migracji

```bash
dotnet ef database update
```


## 7 Uruchomienie projektu

```bash
dotnet run
```
Swagger będzie dostępny pod: https://localhost:7284/swagger


## 8 SurveyApp API

### Endpointy użytkownika

#### Rejestracja użytkownika

Tworzy nowe konto użytkownika.

```http
POST /User/register
```

##### Body

```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

---

#### Logowanie użytkownika

Loguje użytkownika i zwraca token JWT.

```http
POST /User/login
```

##### Body

```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

##### Response

```json
{
  "token": "JWT_TOKEN"
}
```

---

### Endpointy ankiet

#### Tworzenie ankiety

Tworzy nową ankietę wraz z pytaniami i opcjami odpowiedzi.

```http
POST /api/Survey/create_survey
```

Wymaga autoryzacji JWT.

---

#### Pobranie ankiety po ID

Zwraca pełne dane ankiety wraz z pytaniami i opcjami.

```http
GET /api/Survey/{id}
```

Wymaga autoryzacji JWT.

---

#### Usuwanie ankiety

Usuwa ankietę należącą do zalogowanego użytkownika.

```http
DELETE /api/Survey/{id}
```

Wymaga autoryzacji JWT.

---

#### Pobranie wszystkich ankiet użytkownika

Zwraca wszystkie ankiety utworzone przez zalogowanego użytkownika.

```http
GET /api/Survey/all_surveys_by_user_id
```

Wymaga autoryzacji JWT.

---

#### Pobranie wszystkich ankiet

Zwraca listę wszystkich dostępnych ankiet.

```http
GET /api/Survey/all_surveys
```

Wymaga autoryzacji JWT.

---

### Endpointy odpowiedzi

#### Wysłanie odpowiedzi do ankiety

Zapisuje odpowiedzi użytkownika dla wybranej ankiety.

Obsługiwane typy odpowiedzi:
- tekstowe
- pojedynczy wybór
- wielokrotny wybór

```http
POST /api/Survey/post_response_to_survey
```

Wymaga autoryzacji JWT.

---

### Endpointy wyników i statystyk

#### Pobranie wyników ankiety

Zwraca zagregowane wyniki ankiety.

Endpoint zwraca:
- liczbę respondentów
- liczbę głosów dla każdej opcji
- odpowiedzi tekstowe użytkowników

```http
GET /api/Survey/results/{surveyId}
```

Wymaga autoryzacji JWT.  
Dostęp posiada wyłącznie właściciel ankiety.

---

### Autoryzacja

Endpointy chronione wymagają przesłania tokenu JWT w nagłówku żądania.

Przykład:

```http
Authorization: Bearer YOUR_JWT_TOKEN
```

### Frontend

Frontend aplikacji:

https://github.com/xMufc/SurveyAppFrontend
