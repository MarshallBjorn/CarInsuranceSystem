# 🚗 SaveYourCar App

## 📄 Przegłąd
### Krosplatfromowa aplikacja dla zarządzania ubezpieczeniem dla aut.
![зображення](https://github.com/user-attachments/assets/3e04bf60-237f-4bc6-9a90-5c23799681c7)
- **Frontend**: Avalonia UI (.NET)
- **Backend**: ASP.NET Core Web API
- **Database**: PostgreSQL
- **Deployment**: Docker

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

4. Uruchom aplikację
```bash
dotnet run --project src/App
```

### 🔐 Dane do logowania
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

### Logowanie

### Rejestracja

### Edycja danych

### Edycja hasła

### 🛠️ Tworzenie firmy
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki firmy.
3. Kliknij przycisk "Add Firm".
4. Wprowadź dane i zapisz.

### 🛠️ Tworzenie typu ubezpieczenia
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki firmy.
3. Kliknij przycisk "Add Insurance Type".
4. Wprowadź dane i zapisz.

### ✏️ Edycja typu ubezpieczenia
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki firmy.
3. Obok potrzebnego ubezpieczenia kliknij "EDIT".
4. Wypełnij formularz i zapisz.

### ✏️ Edycja firmy.
1. Zaloguj się do aplikacji.
2. Przejdź do zakładki firmy.
3. Obok potrzebnej firmy kliknij "EDIT".
4. Wypełnij formularz i zapisz.





