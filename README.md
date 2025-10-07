# QuestTracker-ConsoleAdventureManager
🎯 Project Goal
Bygga en konsolapplikation som fungerar som ett äventyrsregister. Användaren är en “hjälte” som loggar in i systemet, får quests (uppdrag) att hantera, och får hjälp via notifikationer, AI-assistent och säker inloggning.


📖 Story/Theme
Användare = hjältar.
Quests = uppdrag/äventyr (med deadline och prioritet).
Appen = Quest Guild Terminal, där alla uppdrag och status rapporteras.

⚙️ Core Features

1. User Registration & Login (Hero Profile)
Skapa ny hjälteprofil med:
Username (hjältenamn)
Password (lösenord) – styrkekontroll (minst 6 tecken, 1 siffra, 1 stor bokstav, 1 specialtecken).
Email eller Phone för 2FA.

Vid inloggning:
Ange namn/lösenord.
Systemet skickar kod via SMS/Email (2FA) → måste anges korrekt för att komma in i guilden.


2. Quest Management
Hjälten kan skapa, visa, uppdatera, och avsluta quests.
Quest-attribut:
Title (uppdragsnamn)
Description (beskrivning av uppdraget)
DueDate (när uppdraget måste slutföras)
Priority (Hög, Medium, Låg)
IsCompleted (om uppdraget är klart)

Funktioner:
AddQuest()
ShowAllQuests()
CompleteQuest()
UpdateQuest()


3. Notifications (Guild Alerts)
Om ett quest närmar sig deadline (t.ex. < 24 timmar kvar):

Skicka SMS eller email → “⚔️ Hjälte, ditt uppdrag [Titel] måste vara klart imorgon!”.

Användaren kan också manuellt begära en rapport i menyn för att se vilka uppdrag som är nära deadline.


4. AI Assistance (Guild Advisor – ChatGPT)
AI kan hjälpa till med:
Generera quest descriptions → t.ex. användaren skriver bara titel “Rädda byn” → AI skapar en episk quest-text.
Föreslå prioritet → baserat på deadline och innehåll.
Sammanfatta quests → systemet ger en kort heroisk briefing över alla pågående uppdrag.


5. Reports & Summaries
Visa:
Antal aktiva quests.
Antal klara quests.
Antal quests nära deadline.

Visa sammanfattning i textform (“Du har 3 pågående quests, 1 måste slutföras idag, 2 är under kontroll.”).


6. Menu System (Guild Terminal)
Huvudmeny ska minst ha:
1. Register hero
2. Login hero
3. Exit

När en hjälte är inloggad:
1. Add new quest
2. View all quests
3. Update/Complete quest
4. Request Guild Advisor help (AI)
5. Show guild report
6. Logout


🛠 Technical Requirements
Classes
User (hero profile)
Quest (attributes, methods)
QuestManager (add, remove, complete quests)
Authenticator (register, login, 2FA check)
NotificationService (SMS/email reminders)
GuildAdvisorAI (ChatGPT API integration)
MenuHelper (static helper for clean menus)

Conditional Logic Examples
Password strength validation.
2FA code verification.
Deadline checks for quests.
AI choosing priority based on rules.

Stretch goals
Multiple hero accounts.
Persistent storage (saving hero quests to file/DB).
Achievements system (award badges for X completed quests).