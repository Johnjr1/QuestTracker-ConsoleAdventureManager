# QuestTracker UML Class Diagram

## System Architecture Overview

This UML diagram represents the QuestTracker Console Adventure Manager system, showing the relationships between classes, services, and data models.

```mermaid
classDiagram
    %% Main Entry Point
    class Program {
        +Main() void
    }
    
    %% Core Models
    class User {
        +string Username
        +string PasswordHash
        +string PhoneNumber
        +List~Quest~ Quests
        +static List~User~ Users
        +static User LoggedInUser
        +Register() void
        +Login() bool
        +Logout() void
        -Hash(string) string
    }
    
    class Quest {
        +string Title
        +string Description
        +DateTime DueDate
        +string Priority
        +bool IsCompleted
    }
    
    %% UI Layer
    class Menu {
        +Start() Task
        -HeroMenu() Task
    }
    
    %% Service Layer
    class QuestManager {
        +AddQuest(User) Task
        +ShowQuests(User) void
        +CompleteQuest(User) void
        +UpdateQuest(User) void
        +CheckForUpcomingDeadlines(User) void
        +ShowFullQuestReport(User) void
    }
    
    class GuildAdvisorAI {
        -HttpClient client
        +GenerateQuestFromTitle(string) Task~Quest~
        -QuestAIResponse
    }
    
    class Notifications {
        +SendQuestDeadlineAlert(string, string, string, DateTime) void
    }
    
    class SMSVerification {
        +SendVerificationCode(string) bool
    }
    
    %% External Dependencies
    class TwilioAPI {
        +SendSMS() void
    }
    
    class OpenAIAPI {
        +GenerateQuest() Quest
    }
    
    %% Relationships
    Program --> Menu : starts
    Menu --> User : manages
    Menu --> QuestManager : uses
    Menu --> GuildAdvisorAI : uses
    
    User "1" --> "*" Quest : owns
    User --> SMSVerification : uses for 2FA
    
    QuestManager --> User : operates on
    QuestManager --> Quest : manages
    QuestManager --> Notifications : triggers
    
    GuildAdvisorAI --> OpenAIAPI : calls
    GuildAdvisorAI --> Quest : creates
    
    Notifications --> TwilioAPI : uses
    SMSVerification --> TwilioAPI : uses
    
    %% Static relationships
    User ..> Quest : contains
    QuestManager ..> User : works with
    QuestManager ..> Quest : manipulates
```

## Key Relationships Explained

### 1. **Core Data Model**
- `User` has a one-to-many relationship with `Quest`
- Each user can have multiple quests
- User authentication includes phone verification

### 2. **Service Layer Architecture**
- `QuestManager` handles all quest-related operations
- `GuildAdvisorAI` provides AI-powered quest generation
- `Notifications` and `SMSVerification` handle communication

### 3. **External Integrations**
- **Twilio API**: SMS verification and deadline notifications
- **OpenAI API**: AI-powered quest generation

### 4. **User Flow**
1. User registration with phone verification
2. Login with 2FA
3. Quest management through Menu system
4. AI assistance for quest creation
5. Automated deadline notifications

## Design Patterns Used

### 1. **Static Service Pattern**
- `QuestManager`, `GuildAdvisorAI`, `Notifications`, and `SMSVerification` are static classes
- Provides utility functions without instantiation

### 2. **Repository Pattern (Implicit)**
- `User.Users` static list acts as an in-memory repository
- Quest data is stored within User objects

### 3. **Service Layer Pattern**
- Clear separation between UI (Menu), business logic (QuestManager), and external services
- Each service has a specific responsibility

### 4. **Dependency Injection (Environment-based)**
- External API keys loaded via environment variables
- Services configured through .env file

## Security Features

1. **Password Hashing**: SHA256 encryption for passwords
2. **Two-Factor Authentication**: SMS verification for login
3. **Input Validation**: Password strength requirements
4. **Secure API Communication**: Bearer token authentication

## External Dependencies

- **Spectre.Console**: Rich console UI
- **Twilio**: SMS services
- **OpenAI**: AI quest generation
- **dotenv.net**: Environment variable management
- **System.Text.Json**: JSON serialization
