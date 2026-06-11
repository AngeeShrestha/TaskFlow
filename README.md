# TaskFlow — Task Manager Web App

A full-stack task management web application built with ASP.NET Core 9 MVC and SQL Server.

## 🔗 Tech Stack
- ASP.NET Core 9 MVC (C#)
- Entity Framework Core 9
- SQL Server
- ASP.NET Identity (Authentication)
- Bootstrap + Custom CSS (Dark UI)

## ✨ Features
- User registration and login
- Add, edit, delete and complete tasks
- Priority levels (High, Medium, Low)
- Categories (Work, Personal, Shopping, Health)
- Due dates with overdue detection
- Search and filter tasks
- Drag-to-reorder tasks
- Dashboard with live stats and progress bars

## 📸 Screenshots
<img width="1918" height="1078" alt="image" src="https://github.com/user-attachments/assets/a33e6e86-60f6-461c-aab0-3f5f581f0d42" />
<img width="1918" height="1078" alt="image" src="https://github.com/user-attachments/assets/43b0a566-c247-490a-a90b-0769cad4ab9c" />
<img width="1918" height="1078" alt="image" src="https://github.com/user-attachments/assets/b0bc7f83-2cf7-4ae6-a9c3-77edbba3de86" />

## 🚀 How to Run
1. Clone the repo
2. Update the connection string in `appsettings.json` with your SQL Server name
3. Run migrations:
    Add-Migration InitialCreate
    Update-Database
4. Click Commit changes
