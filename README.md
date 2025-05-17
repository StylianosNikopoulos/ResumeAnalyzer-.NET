# 📌 ResumeAnalyzer

## 🚀 Project Overview
**ResumeAnalyzer** is a microservices-based web application designed to streamline resume submission and analysis for job seekers and HR professionals. It follows best practices for authentication, authorization, and secure API design using **ASP.NET Core MVC**.

---

## 🌟 Key Features
- ✅ **User Registration and Login:** Secure authentication with JWT.
- ✅ **Role-Based Authorization:** Access control (HR and User roles).
- ✅ **Resume Upload:** Securely upload resumes (PDF, DOCX).
- ✅ **Resume Management:** HR users can view, filter, and download resumes.
- ✅ **Microservices Architecture:** Separate AuthService, ApplyService, EmailService, and ResumesService with JWT protection.
- ✅ **Secure Session Management:** User sessions secured with JWT.

---

## 🚦 System Architecture
- 🔐 **AuthService:** Manages user authentication and JWT token generation.  
- 📥 **ApplyService:** Handles resume uploads and user applications.  
- 📊 **ResumesService:** Manages resume viewing, filtering, and downloading for HR.  
- 📧 **EmailService:** Sends email notifications (e.g., confirmation emails).  
- 🌐 **ResumeAnalyzerMVC:** Front-end interface for user interactions.

---

## ⚙️ Tech Stack
- **Frontend:** ASP.NET Core MVC, Razor Views, CSS  
- **Backend:** ASP.NET Core Web API, Entity Framework Core, C#  
- **Database:** MySQL  
- **Authentication:** JWT (JSON Web Token)  
- **Authorization:** Role-based (HR and User)  
- **Security:** BCrypt password hashing, JWT token protection  

---

## 🚀 Planned Feature Improvements
- 🎨 **UI/UX Enhancements:** Modern, responsive design for a better user experience.  
- 🛡️ **Admin Panel:** Add an admin dashboard for user management and role management.  

---

## 🔒 Security Measures
- 🛡️ **JWT Authentication:** Secure login and registration using JSON Web Tokens.  
- 🛑 **Role-Based Access Control:** Restrict access to resume dashboard (HR only).  
- 🔑 **Secure Password Storage:** Passwords hashed using BCrypt.  
- 🔒 **Session Management:** Secure user sessions with JWT and session storage.  
