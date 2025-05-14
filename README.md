# 📌 ResumeAnalyzer

## 🚀 Project Overview
- 🖧 **Microservices Architecture:**
  
**ResumeAnalyzer** is a web application that streamlines resume submission and analysis for job seekers and HR professionals. It demonstrates best practices in authentication, authorization, and secure API design using **ASP.NET Core MVC**.


---

## 🌟 Features

- ✅ **User Registration and Login:** Secure authentication with JWT.
- ✅ **Role-Based Authorization:** Access control (HR and User roles).
- ✅ **Resume Upload:** Securely upload resumes (e.g., PDF, DOCX).
- ✅ **Resume Dashboard:** HR users can view and manage submitted resumes.
- ✅ **Secure API Services:** Separate AuthService and ApplyService with JWT protection.
- ✅ **Session Management:** Secure user sessions with JWT and session storage.

---

## 🚦 Architecture

- 🔐 **AuthService:** Manages user authentication and JWT token generation.  
- 📥 **ApplyService:** Handles resume uploads and user applications.  
- 📊 **ResumesService:** Allows HR to download, filter by keywords, and manage resumes efficiently.  
- 🌐 **ResumeAnalyzerMVC:** User-facing front-end for interacting with the application.

---

## ⚙️ Tech Stack

- **Frontend:** ASP.NET Core MVC, Razor Views, CSS  
- **Backend:** ASP.NET Core Web API, Entity Framework Core, C#  
- **Database:** MySQL  
- **Authentication:** JWT (JSON Web Token)  
- **Authorization:** Role-based (HR and User)  
- **Security:** BCrypt password hashing, JWT token protection  

---

## 🚀 Feature Improvements

- 📧 **Contact Form:** Implement a fully functional contact form for user inquiries and support requests.  
- 🎨 **Modern UI Design:** Upgrade the UI with a clean, responsive design 
- 🛡️ **Admin Panel:** Add an admin dashboard where admins can manage users, delete accounts, and promote users to HR role.  

---

## 🔒 Security Measures

- 🛡️ **JWT Authentication:** Secure login and registration using JSON Web Tokens.
- 🛑 **Role-Based Access Control:** Only HR users can access the resume dashboard.
- 🔑 **Secure Password Storage:** Passwords are hashed using BCrypt.
- 🛡️ **Session Management:** User session is managed securely with JWT and session variables.

