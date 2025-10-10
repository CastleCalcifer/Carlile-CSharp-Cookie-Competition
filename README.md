# Carlile-Cookie-Competition# Carlile-Cookie-Competition

## Description

Carlile-Cookie-Competition is a web application for managing and voting in a family cookie competition. Participants can submit cookies, vote for their favorites, and award special recognitions such as "Most Creative" and "Best Presentation." The site tracks votes, displays results, and supports award submissions. It is built with ASP.NET Core (.NET 8), uses Razor Pages, and stores data in a SQLite database.

## How to Run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://www.sqlite.org/download.html) (optional, database file is created automatically)
- [Visual Studio Community 2022](https://visualstudio.microsoft.com/vs/community/) or [Visual Studio Code](https://code.visualstudio.com/)

---

### Running in Visual Studio Community 2022

1. **Open the Solution:**
   - Launch Visual Studio.
   - Open the solution file (`.sln`) for the project.

2. **Restore NuGet Packages:**
   - Visual Studio will automatically restore NuGet packages on project load.

3. **Build and Run:**
   - Press `F5` to build and run the project with debugging, or `Ctrl+F5` to run without debugging.
   - The application will start and open in your default browser (typically at `https://localhost:5001` or similar).

---

### Running in Visual Studio Code

1. **Open the Project Folder:**
   - Launch VSCode.
   - Open the folder containing the project (`Carlile-Cookie-Competition`).

2. **Restore Dependencies:**
   - Open a terminal in VSCode (`Ctrl+``).
   - Run:
3. **Build and Run:**
   - In the terminal, run:
- The application will start and display the listening URL (e.g., `https://localhost:5001`).


### Accessing the Website

- Open your browser and navigate to the URL shown in the terminal or Visual Studio output (e.g., `https://localhost:5001`).
- Use the navigation bar to access voting, results, and awards pages.

---

## Notes

- The database file (`votes.db`) will be created automatically in the project directory.
- When logging in as a baker, to log out you need to clear your cookies. This website focuses on UX since it is made for a specific purpose, so a log out system could complicate things for non techsaavy users.
