# 🚗 SaveYourCar App

## 📄 Przegląd
### Krosplatfromowa aplikacja dla zarządzania ubezpieczeniem dla aut.
![зображення](https://github.com/user-attachments/assets/3e04bf60-237f-4bc6-9a90-5c23799681c7)
- **Frontend**: Avalonia UI (.NET)
- **Backend**: ASP.NET Core Web API
- **Database**: PostgreSQL
- **Deployment**: Docker

### ERD
![зображення](https://github.com/user-attachments/assets/171458d3-1b55-4538-b1f0-0cf19dc6c89a)

---
## 🛠️ Uruchomienie

- [.NET SDK](https://dotnet.microsoft.com/download) (wersja 8.0)
- [Docker](https://www.docker.com/products/docker-desktop)

### 🚀 Instrukcję uruchomienia

1. Zklonuj repozytorium:
```bash
git clone git@github.com:MarshallBjorn/SaveYourCar.git
cd SaveYourCar
```

2. Stwórz .env
```bash
cp .env.example .env
```

2. Uruchom API i bazę danych:
```bash
docker-compose up --build
```

3. Dodaj migrację
```bash
cd src/Infrastructure
dotnet ef database update
```

4. Zrestartuj kontenery Docker
```bash
docker-compose down
docker-compose up --build
```

5. Uruchom aplikację
```bash
dotnet run --project src/App
```

### 🔐 Dane do logowania
```bash
login: oleksij.nawrockij@gmail.com
haslo: supperp@sswrd123
```
---
## 🧱 Struktura projektu
```
project-root/
├── src/
│   ├── App/              # Aplikacja desktopowa (Avalonia)
│   ├── Core/             # Wspólne modele, DTO i walidatory
│   └── Infrastructure/   # Logika dostępu do danych, repozytoria itp.
├── .env                  # Zmienne środowiskowe (np. connection string)
├── Dockerfile            # Plik Docker dla API
├── docker-compose.yml    # Kompozycja Docker (API + DB)
├── README.md             # Ten plik dokumentacji
```
---
## 📋 Typowe zadania i użycie

### 🔐 Logowanie
1. Przejdź do zakładki "Auth".
2. Wprowadź dane do logowania.
3. Wciśnij przycisk "LOGIN".
![зображення](https://github.com/user-attachments/assets/c69411f9-93bb-41e4-ab76-507ded56e98a)

### 🛠️ Rejestracja
1. Przejdź do zakładki "Auth".
2. Wciśnij tekst pod przyciskiem "LOGIN"
3. Wprowadź dane do rejestracji.
4. Wciśnij przycisk "REGISTER"
![зображення](https://github.com/user-attachments/assets/852cb5b4-e680-44a4-bab6-a8bd52c41a2e)

### ✏️ Edycja danych użytkownika
1. Zaloguj się do aplikacji.
2. W zakładce "Auth" wciśnij przycisk "Edit".
3. W polach z danymi zaktualizuj swoje danne.
4. Wciśnij przycisk "Save"
![зображення](https://github.com/user-attachments/assets/bf0525a1-a1f4-4d9a-9a29-de930d37bebc)

### ✏️ Edycja hasła
1. Zaloguj się do aplikacji.
2. W zakładce "Auth" wciśnij przycisk "Change Password".
3. Wpisz stare hasło, nowe oraz powtórz nowe.
4. Wpiśnij przycisk "CONFIRM"
![зображення](https://github.com/user-attachments/assets/f7c614de-5e78-4164-a5fa-3f428c3f0af3)

### 🛠️ Tworzenie firmy
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki "Firm".
3. Kliknij przycisk "Add Firm".
4. Wprowadź dane i zapisz.
![зображення](https://github.com/user-attachments/assets/5b52df13-f900-4f0a-9954-30c526419288)

### 🛠️ Tworzenie typu ubezpieczenia
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki firmy.
3. Kliknij przycisk "Add Insurance Type".
4. Wprowadź dane i zapisz.
![зображення](https://github.com/user-attachments/assets/7a1a576d-5231-4b72-80ec-0156467e1b0c)

### ✏️ Edycja typu ubezpieczenia
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki "Firm".
3. Obok potrzebnego ubezpieczenia kliknij "EDIT".
4. Wypełnij formularz i zapisz.
![зображення](https://github.com/user-attachments/assets/ab495ec2-ad8e-4621-95a6-9065fd5d96b4)

### ✏️ Edycja firmy.
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki "Firm".
3. Obok potrzebnej firmy kliknij "EDIT".
4. Wypełnij formularz i zapisz.
![зображення](https://github.com/user-attachments/assets/5efdb915-e004-40c1-a2bf-500cd3b8cd01)

### 🛠️ Tworzenie auta
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki "Car".
3. Kliknij przycisk "Add Car".
4. Wprowadź dane i zapisz.
![зображення](https://github.com/user-attachments/assets/8854d210-2ed0-493b-98a1-df63724865c7)

### ✏️ Edycja auta
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki "Car".
3. Obok potrzebnego auta kliknij "EDIT".
4. Wypełnij formularz i zapisz.
![зображення](https://github.com/user-attachments/assets/95d8c814-b820-45ed-a575-bfd4222b8a35)
---
## Skrajne przypadki

### Zakładki "Car" oraz "Firm" dla użytkownika niezalogowanego
![зображення](https://github.com/user-attachments/assets/79b03dea-6efb-41b5-a72d-6264ff28fb7c)
![зображення](https://github.com/user-attachments/assets/e677f3fe-fbca-4442-9e6b-5b71426fe57a)

### Niepoprawne dane dla widoku rejestracji.
![зображення](https://github.com/user-attachments/assets/18d82b1f-757f-4c2f-bb4f-72a716deea27)

### Widoki dla niepoprawnie wprowadzoncych danych w zakładce "Firm"
![зображення](https://github.com/user-attachments/assets/9d8069ec-e277-4bd8-acf4-5e6154a21ec9)
![зображення](https://github.com/user-attachments/assets/254d9ffe-b281-413e-9b79-0847cefc585e)
![зображення](https://github.com/user-attachments/assets/717feb27-46a4-4357-aeba-82edaf69fa71)
![зображення](https://github.com/user-attachments/assets/5ff35e5e-ac1a-49a8-b0c1-d47d1ebce82c)

### Widoki dla nieporawnie wprowadzonych danych w zakładce "Car"
![зображення](https://github.com/user-attachments/assets/41f3ddc3-07ea-45e6-815e-2743cf5fa724)
![зображення](https://github.com/user-attachments/assets/fba4f6eb-d24f-41b3-89dc-5fe03359b0a1)






