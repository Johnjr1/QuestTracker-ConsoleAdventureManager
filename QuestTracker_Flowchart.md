# QuestTracker Console Adventure Manager - System Flowchart

## Main Application Flow

```mermaid
flowchart TD
    A[Program Start] --> B[Menu.Start]
    B --> C{Main Menu}
    C -->|1| D[User.Register]
    C -->|2| E[User.Login]
    C -->|3| Z[Exit Application]
    
    D --> D1[Enter Username]
    D1 --> D2[Enter Phone Number]
    D2 --> D3[SMSVerification.SendVerificationCode]
    D3 --> D4{Verification Success?}
    D4 -->|No| D5[Registration Failed]
    D4 -->|Yes| D6[Enter Password]
    D6 --> D7[Password Strength Check]
    D7 --> D8{Password Valid?}
    D8 -->|No| D6
    D8 -->|Yes| D9[Create User Account]
    D9 --> D10[Hash Password]
    D10 --> D11[Add to Users List]
    D11 --> B
    
    E --> E1[Enter Username & Password]
    E1 --> E2[Validate Credentials]
    E2 --> E3{Credentials Valid?}
    E3 -->|No| E4[Login Failed]
    E3 -->|Yes| E5[SMSVerification.SendVerificationCode]
    E5 --> E6{2FA Success?}
    E6 -->|No| E7[Login Failed]
    E6 -->|Yes| E8[Set LoggedInUser]
    E8 --> F[HeroMenu]
    
    F --> G{Hero Menu}
    G -->|1| H[QuestManager.AddQuest]
    G -->|2| I[QuestManager.ShowQuests]
    G -->|3| J[QuestManager.CompleteQuest]
    G -->|4| K[QuestManager.UpdateQuest]
    G -->|5| L[GuildAdvisorAI.GenerateQuestFromTitle]
    G -->|6| M[QuestManager.ShowFullQuestReport]
    G -->|7| N[User.Logout]
    N --> B
    
    H --> H1{AI Help?}
    H1 -->|Yes| H2[GuildAdvisorAI.GenerateQuestFromTitle]
    H1 -->|No| H3[Manual Quest Creation]
    H2 --> H4{AI Success?}
    H4 -->|No| H3
    H4 -->|Yes| H5[Add AI Quest to User]
    H3 --> H6[Enter Quest Details]
    H6 --> H7[Create Quest Object]
    H7 --> H8[Add to User.Quests]
    H8 --> F
    
    I --> I1[Display All User Quests]
    I1 --> F
    
    J --> J1[Enter Quest Title]
    J1 --> J2[Find Quest by Title]
    J2 --> J3{Quest Found?}
    J3 -->|No| J4[Quest Not Found]
    J3 -->|Yes| J5[Mark as Completed]
    J5 --> F
    
    K --> K1[Enter Quest Title]
    K1 --> K2[Find Quest by Title]
    K2 --> K3{Quest Found?}
    K3 -->|No| K4[Quest Not Found]
    K3 -->|Yes| K5[Update Quest Fields]
    K5 --> F
    
    L --> L1[Enter Quest Title]
    L1 --> L2[Call OpenAI API]
    L2 --> L3[Parse AI Response]
    L3 --> L4[Create Quest Object]
    L4 --> L5[Add to User.Quests]
    L5 --> F
    
    M --> M1[Calculate Quest Statistics]
    M1 --> M2[Display Report]
    M2 --> M3[Check Upcoming Deadlines]
    M3 --> M4[Send Notifications if Needed]
    M4 --> F
```

## System Architecture

```mermaid
flowchart TD
    subgraph "Core Application"
        A[Program.cs] --> B[Menu.cs]
        B --> C[User.cs]
        B --> D[QuestManager.cs]
    end
    
    subgraph "Models"
        E[Quest.cs]
        F[User.cs]
    end
    
    subgraph "Services"
        G[QuestManager.cs]
        H[GuildAdvisorAI.cs]
        I[SMSVerification.cs]
        J[NotificationService.cs]
    end
    
    subgraph "External APIs"
        K[OpenAI API]
        L[Twilio SMS API]
    end
    
    subgraph "Data Storage"
        M[In-Memory Lists]
        N[User.Users List]
        O[User.Quests List]
    end
    
    A --> B
    B --> C
    B --> G
    C --> F
    G --> E
    H --> K
    I --> L
    J --> L
    C --> N
    F --> O
```

## Authentication Flow

```mermaid
sequenceDiagram
    participant U as User
    participant M as Menu
    participant UU as User Class
    participant S as SMSVerification
    participant T as Twilio API
    
    U->>M: Choose Registration
    M->>UU: Register()
    UU->>U: Enter Username
    U->>UU: Username
    UU->>U: Enter Phone Number
    U->>UU: Phone Number
    UU->>S: SendVerificationCode()
    S->>T: Send SMS
    T-->>S: SMS Sent
    S->>U: Enter Verification Code
    U->>S: Verification Code
    S->>S: Validate Code
    S-->>UU: Verification Result
    UU->>U: Enter Password
    U->>UU: Password
    UU->>UU: Hash Password
    UU->>UU: Create User Account
    UU-->>M: Registration Complete
```

## Quest Management Flow

```mermaid
flowchart TD
    A[Quest Management] --> B{Add Quest}
    A --> C[View Quests]
    A --> D[Complete Quest]
    A --> E[Update Quest]
    A --> F[AI Quest Generation]
    
    B --> B1{Use AI?}
    B1 -->|Yes| B2[GuildAdvisorAI]
    B1 -->|No| B3[Manual Entry]
    B2 --> B4[OpenAI API Call]
    B4 --> B5[Parse JSON Response]
    B5 --> B6[Create Quest Object]
    B3 --> B7[User Input Fields]
    B7 --> B6
    B6 --> B8[Add to User.Quests]
    
    C --> C1[Display All Quests]
    C1 --> C2[Show Status & Details]
    
    D --> D1[Find Quest by Title]
    D1 --> D2{Quest Found?}
    D2 -->|Yes| D3[Mark as Completed]
    D2 -->|No| D4[Show Error]
    
    E --> E1[Find Quest by Title]
    E1 --> E2{Quest Found?}
    E2 -->|Yes| E3[Update Fields]
    E2 -->|No| E4[Show Error]
    
    F --> F1[Enter Quest Title]
    F1 --> F2[AI Generation]
    F2 --> F3[Add to User.Quests]
```

## Data Models

```mermaid
classDiagram
    class Quest {
        +string Title
        +string Description
        +DateTime DueDate
        +string Priority
        +bool IsCompleted
    }
    
    class User {
        +string Username
        +string PasswordHash
        +string PhoneNumber
        +List~Quest~ Quests
        +static List~User~ Users
        +static User LoggedInUser
        +Register()
        +Login()
        +Logout()
        +Hash()
    }
    
    class QuestManager {
        +AddQuest()
        +ShowQuests()
        +CompleteQuest()
        +UpdateQuest()
        +CheckForUpcomingDeadlines()
        +ShowFullQuestReport()
    }
    
    class GuildAdvisorAI {
        +GenerateQuestFromTitle()
    }
    
    class SMSVerification {
        +SendVerificationCode()
    }
    
    class Notifications {
        +SendQuestDeadlineAlert()
    }
    
    User ||--o{ Quest : contains
    QuestManager --> Quest : manages
    QuestManager --> User : works with
    GuildAdvisorAI --> Quest : creates
    SMSVerification --> User : verifies
    Notifications --> User : notifies
```

## External Integrations

```mermaid
flowchart LR
    subgraph "QuestTracker App"
        A[GuildAdvisorAI]
        B[SMSVerification]
        C[Notifications]
    end
    
    subgraph "External Services"
        D[OpenAI API]
        E[Twilio SMS]
    end
    
    A -->|HTTP Request| D
    D -->|JSON Response| A
    B -->|SMS Send| E
    C -->|SMS Send| E
    E -->|SMS Delivery| B
    E -->|SMS Delivery| C
```

## Key Features Summary

1. **User Management**: Registration with phone verification and password strength validation
2. **Quest Management**: CRUD operations for quests with manual and AI-assisted creation
3. **AI Integration**: OpenAI API for generating quest descriptions and details
4. **SMS Notifications**: Twilio integration for verification and deadline alerts
5. **Security**: Password hashing and two-factor authentication
6. **Reporting**: Comprehensive quest statistics and deadline monitoring

## Technology Stack

- **Language**: C# (.NET 9.0)
- **UI Framework**: Spectre.Console for rich console interface
- **AI Integration**: OpenAI GPT-4o-mini API
- **SMS Service**: Twilio API
- **Security**: SHA256 password hashing
- **Configuration**: dotenv.net for environment variables

