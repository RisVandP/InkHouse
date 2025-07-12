# InkHouse Library Management System

[中文版说明](./README.zh.md)

InkHouse is a cross-platform library management system built with C#, Avalonia, and MySQL. It supports both Windows and Linux environments, providing a modern, elegant, and easy-to-use interface.

---

## 🤝 Team Collaboration & Project Structure

### Team Collaboration
- **Version Control**: Everyone uses Git, with a remote repo (GitHub/GitLab) for collaboration.
- **Branch Management**: The main branch (main) holds only stable, runnable code. Create a new feature/bugfix branch for each task, merge to main after completion.
- **Commit Message Convention**: Clearly describe each change, preferably in English (e.g., `feat: add book borrow feature`).
- **Code Review**: Team members should review each other's code before merging to improve quality.
- **Task Assignment**: Use issues or a todo list to assign tasks and sync progress regularly.

### Project Structure
| Path                | Purpose                                         |
|---------------------|-------------------------------------------------|
| `App.axaml`         | App entry XAML, defines global styles/resources |
| `App.axaml.cs`      | App entry C# code, initializes the application  |
| `Assets/`           | Stores images, icons, and other static assets   |
| `Models/`           | Data models, define DB table structures         |
| `Services/`         | Service layer, encapsulates DB/business logic   |
| `ViewModels/`       | ViewModel layer, connects UI and data logic     |
| `Views/`            | View layer, XAML UI files and code-behind       |
| `InkHouse.csproj`   | Project config, dependency management           |
| `Program.cs`        | Main program entry point                        |
| `app.manifest`      | App manifest, configures permissions, etc.      |
| `bin/`, `obj/`      | Build output and intermediate files (auto-gen)  |

### Layered Architecture
- **Models**: Define C# classes mapping to DB tables (e.g., Book.cs, User.cs), for data access and entity mapping.
- **Services**: Encapsulate all DB interaction logic (e.g., UserService), such as CRUD and business rules.
- **ViewModels**: Handle UI logic and data binding, bridge between Views and Services (e.g., LoginViewModel).
- **Views**: XAML files define UI layout, .cs files handle UI events (e.g., button clicks).

### Quick Start & Dev Tips
- **Read the README and structure guide first to understand each layer's responsibility.**
- **For new features, create corresponding files in Models/Services/ViewModels/Views.**
- **Use breakpoints for debugging, and leverage Rider's XAML preview and DB tools.**
- **Communicate frequently; ask questions when in doubt.**

---

## 🛠️ Development Architecture & Workflow

### 🎯 Architecture Advantages
- ✅ **Unified Configuration Management**: All configurations are centralized in `AppConfig.cs`
- ✅ **Service Manager**: Get services through `ServiceManager` without manual creation
- ✅ **Automatic Error Handling**: `ViewModelBase` provides unified error handling
- ✅ **Simplified Development**: No need to manually create database connections and services
- ✅ **Framework Design**: Only provides architecture framework, specific business logic is implemented by team members

### 🏗️ Architecture Overview

#### 1. Configuration Management (`AppConfig.cs`)
```csharp
// Modify database connection information
AppConfig.SetDatabaseConnection("server", 3306, "database", "username", "password");

// Or directly modify the connection string
AppConfig.DatabaseConnectionString = "your connection string";
```

#### 2. Service Manager (`ServiceManager.cs`)
```csharp
// Get various services (no manual creation needed)
var userService = ServiceManager.Instance.UserService;
var bookService = ServiceManager.Instance.BookService;
var borrowService = ServiceManager.Instance.BorrowRecordService;
```

#### 3. Base ViewModel (`ViewModelBase.cs`)
```csharp
public class YourViewModel : ViewModelBase
{
    // Automatically get the following features:
    // - IsLoading: Loading state
    // - ErrorMessage: Error message
    // - SuccessMessage: Success message
    // - ShowError(): Display error
    // - ShowSuccess(): Display success
    // - ExecuteAsync(): Safely execute async operations
}
```

### 🚀 Quick Start Guide

#### Step 1: Configure Database
Modify database connection information in `Services/AppConfig.cs`:
```csharp
public static string DatabaseConnectionString { get; set; } = 
    "server=your_server;port=3306;database=InternShip;user=your_username;password=your_password;";
```

#### Step 2: Create New ViewModel
```csharp
public class MyFeatureViewModel : ViewModelBase
{
    private string _myProperty;
    
    public string MyProperty
    {
        get => _myProperty;
        set => SetProperty(ref _myProperty, value);
    }
    
    public ICommand MyCommand { get; }
    
    public MyFeatureViewModel()
    {
        MyCommand = new AsyncRelayCommand(MyMethodAsync);
    }
    
    private async Task MyMethodAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Get service
            var service = ServiceManager.Instance.UserService;
            
            // Execute business logic
            var result = service.SomeMethod();
            
            // Show result
            ShowSuccess("Operation successful!");
        });
    }
}
```

#### Step 3: Create View
```csharp
public partial class MyFeatureView : UserControl
{
    public MyFeatureView()
    {
        InitializeComponent();
        DataContext = new MyFeatureViewModel(); // No need to pass services
    }
}
```

### 📚 Service Usage Examples

#### User Service (UserService)
```csharp
var userService = ServiceManager.Instance.UserService;

// User login (already implemented)
var user = userService.Login("username", "password");

// TODO: Team members can add other user-related methods here
// For example: GetAllUsers(), AddUser(), UpdateUser(), DeleteUser(), etc.
```

#### Book Service (BookService)
```csharp
var bookService = ServiceManager.Instance.BookService;

// TODO: Team members can add book-related methods here
// For example: GetAllBooks(), AddBook(), UpdateBook(), DeleteBook(), etc.
```

#### Borrow Record Service (BorrowRecordService)
```csharp
var borrowService = ServiceManager.Instance.BorrowRecordService;

// TODO: Team members can add borrow record-related methods here
// For example: GetAllBorrowRecords(), BorrowBook(), ReturnBook(), etc.
```

### 🎨 UI Development Tips

#### 1. Data Binding
```xml
<!-- Bind to ViewModel properties -->
<TextBox Text="{Binding UserName}" />
<Button Command="{Binding LoginCommand}" Content="Login" />

<!-- Bind to lists -->
<ListBox ItemsSource="{Binding Books}">
    <ListBox.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Title}"/>
        </DataTemplate>
    </ListBox.ItemTemplate>
</ListBox>
```

#### 2. Display Loading State
```xml
<StackPanel>
    <!-- Loading indicator -->
    <ProgressBar IsIndeterminate="{Binding IsLoading}" 
                 IsVisible="{Binding IsLoading}" />
    
    <!-- Error message -->
    <TextBlock Text="{Binding ErrorMessage}" 
               Foreground="Red" 
               IsVisible="{Binding ErrorMessage, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" />
    
    <!-- Success message -->
    <TextBlock Text="{Binding SuccessMessage}" 
               Foreground="Green" 
               IsVisible="{Binding SuccessMessage, Converter={x:Static StringConverters.IsNotNullOrEmpty}}" />
</StackPanel>
```

#### 3. Command Binding
```xml
<Button Command="{Binding AddBookCommand}" Content="Add Book" />
<Button Command="{Binding UpdateBookCommand}" Content="Update Book" />
<Button Command="{Binding DeleteBookCommand}" Content="Delete Book" />
```

### 🔧 Debugging Tips

#### 1. View Error Information
All service methods have exception handling, error messages will be output to console:
```csharp
Console.WriteLine($"Operation failed: {ex.Message}");
```

#### 2. Use ViewModel's Debug Features
```csharp
// Display debug information in ViewModel
ShowError("Debug information");
ShowSuccess("Operation successful");
```

#### 3. Check Database Connection
```csharp
// Set debug mode in AppConfig.cs
AppConfig.IsDebugMode = true;
```

### 📝 Development Standards

#### 1. Naming Conventions
- **Class names**: PascalCase (e.g., `BookService`, `LoginViewModel`)
- **Method names**: PascalCase (e.g., `GetAllBooks()`, `ValidateUser()`)
- **Property names**: PascalCase (e.g., `BookTitle`, `IsAvailable`)
- **Private fields**: camelCase with underscore (e.g., `_bookService`, `_searchTerm`)

#### 2. File Organization
```
Models/
├── Book.cs              # Book entity
├── User.cs              # User entity
└── BorrowRecord.cs      # Borrow record entity

Services/
├── AppConfig.cs         # Configuration management
├── ServiceManager.cs    # Service manager
├── UserService.cs       # User service
├── BookService.cs       # Book service
└── BorrowRecordService.cs # Borrow record service

ViewModels/
├── ViewModelBase.cs     # Base ViewModel
├── LoginViewModel.cs    # Login ViewModel
└── YourFeatureViewModel.cs # Your feature ViewModel

Views/
├── LoginView.axaml      # Login interface
└── YourFeatureView.axaml # Your feature interface
```

#### 3. Code Comments
All public methods and properties should have comments:
```csharp
/// <summary>
/// Get all books
/// </summary>
/// <returns>List of books</returns>
public List<Book> GetAllBooks()
{
    // Implementation code
}
```

### 🚨 Common Issues

#### Q1: Database connection failed?
A1: Check the connection string in `AppConfig.cs` is correct, ensure the database server is accessible.

#### Q2: Why is my ViewModel not responding?
A2: Make sure the ViewModel inherits from `ViewModelBase` and properties use the `SetProperty` method.

#### Q3: How to add new services?
A3: Create new service classes following the same pattern, then add corresponding properties in `ServiceManager`.

#### Q4: How to handle complex business logic?
A4: Implement complex business logic in the service layer, ViewModel only handles UI interaction and data binding.

### 🎉 Summary

The new architecture makes development simpler and more unified:

1. **Centralized Configuration Management**: All configurations in one place
2. **Unified Service Access**: Get all services through ServiceManager
3. **Automatic Error Handling**: ViewModelBase provides unified error handling
4. **Cleaner Code**: No need to repeat database connection and service creation
5. **Complete Comments**: All code has detailed comments
6. **Framework Design**: Only provides architecture framework, specific business logic implemented by team members

Now you can focus on implementing business logic instead of repeating database connection code!

### 📝 Message to Team Members

This architecture has already set up the basic framework for you, including:
- ✅ Database connection management
- ✅ Service manager
- ✅ Unified error handling
- ✅ Basic login functionality

You only need to:
1. Add specific business logic methods in service classes
2. Call these methods in ViewModels
3. Display results in Views

All database connections, error handling, and configuration management have been handled for you!



### 5. 🔄 Git Workflow

#### Before Starting Development
```bash
# Always pull latest changes
git pull origin main

# Create feature branch
git checkout -b feature/your-feature-name
```

#### During Development
```bash
# Check status
git status

# Add changes
git add .

# Commit with descriptive message
git commit -m "feat: add book search functionality

- Add Book entity with EF Core mapping
- Create BookService for database operations
- Implement BookSearchViewModel with MVVM pattern
- Add BookSearchView with search interface"
```

#### Before Merging
```bash
# Push your branch
git push origin feature/your-feature-name

# Create Pull Request (PR) on GitHub/GitLab
# Request code review from team members
# Address review comments if any
# Merge to main after approval
```

### 6. 🐛 Common Issues & Solutions

#### Entity Framework Issues
- **"Table doesn't exist"**: Run database migrations
- **"Connection failed"**: Check connection string in `AppConfig.cs`
- **"Entity not found"**: Ensure DbSet is added to DbContext

#### Avalonia UI Issues
- **"Binding errors"**: Check property names match between View and ViewModel
- **"XAML preview not working"**: Restart Rider or clean/rebuild project
- **"Controls not showing"**: Verify XAML syntax and namespace declarations

#### Git Issues
- **"Merge conflicts"**: Resolve conflicts manually, then commit
- **"Branch behind main"**: Rebase your branch: `git rebase main`

---

## 🚀 Quick Start with Rider IDE

### 1. 🖥️ Open the Project in Rider
- Launch JetBrains Rider.
- Click `Open` and select the project root folder (the one containing `InkHouse.sln`).

### 2. 📦 Restore Dependencies
- Rider will automatically detect the solution and prompt to restore NuGet packages.
- If not, right-click the solution in the Solution Explorer and select `Restore NuGet Packages`.

### 3. ⚙️ Configure Database Connection
- In the Solution Explorer, navigate to `InkHouse/Services/AppConfig.cs`.
- Find the line:
  ```csharp
  public static string DatabaseConnectionString { get; set; } = 
      "server=YOUR_SERVER;port=3306;database=InternShip;user=YOUR_USER;password=YOUR_PASSWORD;";
  ```
- Replace `YOUR_SERVER`, `YOUR_USER`, `YOUR_PASSWORD` with your actual MySQL information.

### 4. 🗄️ Initialize the Database
- Use your preferred MySQL client (such as MySQL Workbench, DBeaver, or Rider's built-in Database tool) to create a database named `InternShip`.
- Example SQL:
  ```sql
  CREATE DATABASE InternShip;
  ```

### 5. 🛠️ Build and Run the Project
- In Rider, select `InkHouse` as the startup project.
- Click the green `Run` button or press `Shift+F10` to build and run.
- The application window will appear.

### 6. 🖼️ Visual UI Design
- You can edit `.axaml` files with Rider's built-in XAML previewer for Avalonia.
- Open any `.axaml` file (such as `LoginView.axaml`), and use the split view to see both code and visual preview.
- Drag and drop controls, or edit XAML directly for custom layouts.

### 7. 🐞 Debugging
- Set breakpoints in your C# code.
- Use the `Debug` button or press `Shift+F9` to start debugging.
- Inspect variables, step through code, and view call stacks as needed.

---

## 🧩 Features
- 👤 User login (Admin & Normal User)
- 📚 Book management (CRUD)
- 🔄 Borrow and return records
- 🛡️ Role-based access control

## 🛠️ Tech Stack
- Avalonia UI (cross-platform desktop)
- Entity Framework Core (ORM)
- MySQL (cloud database)

## 👥 Contributors
- Member A
- Member B
- Member C

## ❓ FAQ & Tips
- If you see errors about missing SDK, make sure you installed .NET 8.0+ and restarted Rider
- If you cannot connect to MySQL, check your firewall, IP, port, username, and password
- For Linux, you may need to install additional libraries for Avalonia UI (see [Avalonia Docs](https://docs.avaloniaui.net/))
- For more Rider tips, see [Rider Documentation](https://www.jetbrains.com/help/rider/)

## 📄 License
This project is for educational and internship purposes only. Feel free to use and modify.