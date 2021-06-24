# Aplikacja internetowa do tworzenia i wypełniania ankiet.

## Wymagania
Projekt i implemetacja systemu webowego umożliwiającego głosowanie (lub  ankietowanie) w sposób umożliwiający zachowanie anonimowości użytkowników. System zrealizowany w formie aplikacji webowej powinien uwzględniać możliwość oddania głosu lub wyrażenia opinii w taki sposób, aby realizować następujące funkcje:

• informacje przechowywane w bazie danych,

• reprezentacja nie umożliwia powiązania użytkownika z konkretnymi danymi,

• reprezentacja umożliwia sprawdzenie czy dana osoba przekazała dane,

• reprezentacja umożliwia sprawdzenie przez użytkownika czy jego dane są zapisane w bazie.

Implementacja powinna uzględniać responsywny interfejs. Do zapewnienia anonimowości należy wykorzystać techniki kryptograficzne (funkcje skrótu) oraz metody generowania tokenów. Rekomedowane jest wykorzystanie ogólnodostępnych bibliotek programistycznych.

### Funkcjonalności dostępne dla wszystkich:

• rejestracja

• logowanie

### Funkcjonalności dostępne jedynie dla zalogowanych użytkowników:

• tworzenie nowych ankiet

• wypełnianie ankiet

• prezentacja statystyk ankiety i opcjonalny eksport do pliku JSON.

• sprawdzanie integralności ankiety

• przeglądanie ankiet do wypełnienia i - osobno - utworzonych 

## Instalacja

Aplikacja zbudowana z pomocą Visual Studio 2019 i SQL Server Management Studio 2012. Aplikację powinno się dać zbudować też alternatywnymi narzędziami jak Visual Studio Code, narzędzia firmy JetBrains i serwery kompatybilne z SQL Server mogące zaimportować plik .bak.
Plik survey_backup zaimportować do wybranego serwera a następnie zmienić właściwość "DefaultConnection" w pliku appsettings.json na wartość wskazującą na Twój lokalny serwer

## Dokumentacja

### Autor

Damian Wyka
