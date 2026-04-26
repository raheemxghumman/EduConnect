# RooCode Prompt — EduConnect Blazor Academic Portal

---

## YOUR OPERATING RULES — READ BEFORE ANYTHING ELSE

1. **Work in phases.** Each phase is listed below with a clear checklist. Do not start Phase N+1 until Phase N is fully complete and verified.
2. **After completing each phase, STOP and ask the user:** `"✅ Phase [N] complete. All items verified. Ready to move to Phase [N+1]: [phase name]? Reply YES to continue."`
3. **Do not proceed until the user replies YES.**
4. **Every file you create must be 100% complete.** No `// TODO`, no `// implement later`, no empty method bodies, no stub implementations. If a method exists, it must be fully working.
5. **After writing every `.razor` or `.cs` file, immediately run `dotnet build` and fix ALL errors before doing anything else.** Never move on with a broken build.
6. **Never leave a file half-written.** Write the complete file in one pass, then verify it compiles.
7. **No placeholder UI.** Every page must render real data with real functionality.
8. **At the end of every phase, run `dotnet build` and confirm 0 errors, 0 warnings that affect functionality.**
9. **The final phase is a full end-to-end test** of every feature. Fix every bug found before marking done.
10. **If you are unsure about anything, make a decision and document it.** Do not ask clarifying questions mid-phase. Ask the user only at phase boundaries.

---

## PROJECT SPECIFICATION

**Name:** EduConnect — University Academic Web Portal
**Assignment:** Visual Programming Assignment 2

| Property | Value |
|---|---|
| Framework | Blazor Server (.NET 10) |
| UI Styling | Bootstrap 5 via CDN — absolutely NO third-party Blazor component libraries (no MudBlazor, no Radzen, nothing) |
| Language | C# (.NET 8) |
| Data Storage | In-memory C# collections only — no database, no EF Core, no SQLite |
| State Management | Scoped services with C# delegate events (`Action`, `Action<T>`) |
| IDE Compatibility | Must open and build in Visual Studio 2022 without extra steps |

---

## DESIGN RULES — CUSTOM PROFESSIONAL UI

- **Remove the default Blazor sidebar entirely.** Do not use the default `NavMenu.razor` sidebar layout. Delete it.
- **Remove the default Blazor top navbar.** Build a completely custom top navigation bar using Bootstrap 5 `navbar` classes.
- **Custom layout:** The app uses a full-width top navbar + content area below. No left sidebar at all.
- **Color scheme:** Dark navy (`#1a2744`) navbar and header accents. White content background. Bootstrap cards with subtle shadow (`shadow-sm`). Clean, minimal look.
- **Fonts:** Use Google Fonts — add `Poppins` via CDN in `_Host.cshtml`. Apply `font-family: 'Poppins', sans-serif` globally.
- **No inline `style=""` attributes** for colors or layout. Use Bootstrap utility classes and a single `wwwroot/css/app.css` for custom rules.
- **Responsive:** All pages must use Bootstrap grid (`col-md-*`, `col-sm-*`) and look correct at 768px mobile width.
- **No Blazor default pages:** Delete `Counter.razor`, `FetchData.razor`, `Index.razor`, `Weather.razor`, `Error.razor` content. Replace `Index.razor` with a redirect to `/login`.

---

## ARCHITECTURE RULES — MUST BE FOLLOWED EXACTLY

### Folder Structure
```
EduConnect/
├── Exceptions/
│   ├── StudentHasActiveEnrollmentsException.cs
│   └── CourseFullException.cs
├── Interfaces/
│   ├── IValidatable.cs
│   ├── IRepository.cs
│   ├── IStudentService.cs
│   ├── ICourseService.cs
│   └── IGradeService.cs
├── Models/
│   ├── Person.cs
│   ├── Student.cs
│   ├── Faculty.cs
│   ├── Admin.cs
│   ├── Course.cs
│   ├── GradeRecord.cs
│   ├── Notification.cs
│   ├── AuthState.cs
│   ├── EnrollmentStatus.cs
│   └── NotificationType.cs
├── Services/
│   ├── SeedData.cs
│   ├── AuthStateService.cs
│   ├── NotificationService.cs
│   ├── StudentService.cs
│   ├── CourseService.cs
│   └── GradeService.cs
├── Components/          ← reusable components embedded inside pages
│   ├── AlertBox.razor
│   ├── ConfirmDialog.razor
│   ├── StudentCard.razor
│   ├── CourseCard.razor
│   ├── GradeTable.razor
│   ├── LoadingSpinner.razor
│   └── NotificationBell.razor
├── Pages/
│   ├── Login.razor                         → /login
│   ├── Unauthorized.razor                  → /unauthorized
│   ├── Dashboard.razor                     → /dashboard
│   ├── Courses.razor                       → /courses
│   ├── NotificationsPage.razor             → /notifications
│   ├── Admin/
│   │   ├── StudentList.razor               → /admin/students
│   │   ├── StudentAdd.razor                → /admin/students/add
│   │   ├── StudentEdit.razor               → /admin/students/edit/{Id:guid}
│   │   ├── StudentDetail.razor             → /admin/students/{Id:guid}
│   │   ├── CourseManagement.razor          → /admin/courses
│   │   ├── GradeReport.razor               → /admin/reports/grades
│   │   └── Broadcast.razor                 → /admin/notifications
│   ├── Faculty/
│   │   ├── FacultyCourses.razor            → /faculty/courses
│   │   └── GradeSubmission.razor           → /faculty/grades
│   └── Student/
│       ├── Enroll.razor                    → /student/enroll
│       └── StudentGrades.razor             → /student/grades
├── Shared/
│   ├── MainLayout.razor                    ← custom layout, NO sidebar
│   └── AppNavBar.razor                     ← custom Bootstrap 5 top navbar
└── wwwroot/
    └── css/
        └── app.css                         ← custom CSS rules
```

### DI Registration Rules (Program.cs)
```csharp
builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IGradeService, GradeService>();
```
- **Never use `new ServiceName()` inside any `.razor` file.** Always use `@inject`.
- **Never call `new()` for services in components.** DI only.

### Global Imports (`_Imports.razor`)
Add these namespaces so every page/component can use them without repeating `@using`:
```razor
@using EduConnect.Models
@using EduConnect.Services
@using EduConnect.Interfaces
@using EduConnect.Exceptions
@using EduConnect.Components
@using Microsoft.AspNetCore.Components
@using Microsoft.AspNetCore.Components.Web
```

### Cascading Auth State
In `MainLayout.razor`, wrap everything in:
```razor
<CascadingValue Value="AuthSvc.State">
    @Body
</CascadingValue>
```
Pages and components that need auth context use `[CascadingParameter] public AuthState Auth { get; set; }`.

### AuthGuard Pattern
Every protected page must check auth at the top of `OnInitialized()`:
```csharp
protected override void OnInitialized()
{
    if (!Auth.IsAuthenticated)        { Nav.NavigateTo("/login");        return; }
    if (RequiredRole != Auth.Role)    { Nav.NavigateTo("/unauthorized"); return; }
}
```
Where `Auth` is the `[CascadingParameter]` and `Nav` is injected `NavigationManager`.

---

## SEED DATA — EXACT ACCOUNTS

Use these exact credentials so testing is predictable:

| Role | Name | Email | Password |
|---|---|---|---|
| Admin | Dr. Admin | admin@edu.pk | admin123 |
| Faculty | Dr. Ayesha Khan | ayesha@edu.pk | faculty123 |
| Faculty | Prof. Bilal Ahmed | bilal@edu.pk | faculty123 |
| Student | Wania Rahman | wania@student.edu.pk | student123 |
| Student | Roman Fatima | roman@student.edu.pk | student123 |
| Student | Abdulraheem | raheem@student.edu.pk | student123 |
| Student | Sara Malik | sara@student.edu.pk | student123 |
| Student | Ali Hassan | ali@student.edu.pk | student123 |

Seed 5 courses: CS-101 Programming Fundamentals (cap 30), CS-201 Data Structures (cap 25), CS-301 Database Systems (cap 20), CS-284 Visual Programming (cap 35), CS-401 Software Engineering (cap 10).
Pre-enroll Wania and Abdulraheem in CS-101 and CS-201. Fill CS-401 to exactly 10 students so it is Full for testing.

---

## COMPLETE BUSINESS LOGIC

### Models

**Person (abstract):**
- `Id` (Guid, auto-generated), `FullName`, `Email`, `PasswordHash`
- `abstract string GetRole()`

**Student : Person, IValidatable:**
- `Semester` (1–8), `CGPA` (decimal, 0.0–4.0), `List<Course> Enrollments`, `List<GradeRecord> Grades`
- `GetRole()` → `"Student"`
- `Validate()` → checks FullName not empty, Email contains `@`, Semester 1–8, CGPA 0–4

**Faculty : Person:**
- `List<Course> AssignedCourses`
- `GetRole()` → `"Faculty"`

**Admin : Person:**
- `GetRole()` → `"Admin"`

**Course:**
- `Id`, `Code`, `Title`, `CreditHours` (int), `MaxCapacity` (int), `IsActive` (bool, default true), `AssignedFacultyId` (Guid)
- `List<Student> Enrolled`
- Computed (no setter): `EnrollmentStatus` → Open if <70% full, AlmostFull if 70–99%, Full if 100%

**GradeRecord:**
- `Id`, `StudentId`, `CourseId`, `CourseTitle`, `CreditHours`, `Marks` (int 0–100)
- Computed `LetterGrade`: A≥85, B≥70, C≥55, D≥45, F<45
- Computed `GradePoints`: A=4.0, B=3.0, C=2.0, D=1.0, F=0.0

**Notification:**
- `Id`, `Message`, `NotificationType` (enum: Enrollment/GradePosted/Announcement), `UserId`, `IsRead` (bool), `Timestamp` (DateTime)

**AuthState:**
- `Person? CurrentUser`
- `bool IsAuthenticated` → CurrentUser != null
- `string Role` → CurrentUser?.GetRole() ?? ""
- `bool IsAdmin`, `IsFaculty`, `IsStudent` → computed from Role

### Grade CGPA Formula
```
CGPA = Σ(GradePoints × CreditHours) / Σ(CreditHours)
```
Weighted average across all graded courses. Rounded to 2 decimal places. Returns 0.00 if no grades.

### Enrollment Rules (enforced in CourseService, NOT in components)
- Cannot enroll if course is Full → throw `CourseFullException`
- Cannot enroll in a course already in student's Enrollments → throw `InvalidOperationException("Already enrolled")`
- Drop is only allowed if `course.IsActive == true` → throw `InvalidOperationException("Cannot drop inactive course")` otherwise

### Student Delete Rule (enforced in StudentService, NOT in component)
- If `student.Enrollments.Any()` → throw `StudentHasActiveEnrollmentsException`

---

## BINDING REQUIREMENTS — MUST BE DEMONSTRABLE

| Binding Type | Where Used |
|---|---|
| One-way (`@variable`) | NavBar user name + role badge, Student dashboard CGPA and enrolled courses |
| Two-way (`@bind`) | All form inputs (Add/Edit Student, Add/Edit Course, grade marks inputs, broadcast textarea) |
| `@bind:event="oninput"` | Live search on `/admin/students` — list must filter with every keystroke, no button needed |
| `EventCallback<Course>` | `CourseCard.OnEnroll` — child fires callback, parent calls service |
| `CascadingParameter` | `AuthState` cascaded from `MainLayout`, read in protected pages without re-injecting |
| `IDisposable` + event unsubscribe | `NotificationBell`, `AppNavBar` — subscribe in `OnInitialized`, unsubscribe in `Dispose()` |

---

## UI DESIGN SPEC

### Custom Top Navbar (`AppNavBar.razor`)
- Dark navy background (`#1a2744`)
- Left: logo text "🎓 EduConnect" in white bold
- Center: navigation links (role-conditional, shown below)
- Right: logged-in user name + role badge + `NotificationBell` component + Logout button
- If not authenticated: show only Login link on right
- Role-conditional links:
  - **All authenticated:** Dashboard, Courses, Notifications
  - **Admin only:** dropdown "Admin ▾" → Students, Courses (admin), Grade Report, Broadcast
  - **Faculty only:** My Courses, Submit Grades
  - **Student only:** Enroll, My Grades
- On mobile: collapses to hamburger (Bootstrap navbar-toggler)
- Subscribe to `AuthStateService.OnAuthChanged` in `OnInitialized()`, call `StateHasChanged()` on change, unsubscribe in `Dispose()`

### Login Page
- Centered card, width 420px, vertically centered on full viewport height
- Card header: dark navy, white text "🎓 EduConnect"
- Fields: Email, Password (type=password)
- Red error alert if credentials invalid (use `AlertBox` component)
- Below card: small grey demo credentials text listing all test accounts
- No navbar shown on login page — show it only when authenticated

### Dashboard
- **Admin dashboard:** 4 quick-action cards in a row (Manage Students, Manage Courses, Grade Report, Broadcast) + a stats strip showing total students, total courses, total enrollments
- **Faculty dashboard:** list of assigned courses with enrolled count, quick link to Submit Grades
- **Student dashboard:** CGPA card (large number, green), Semester badge, enrolled courses count, then a list of enrolled courses with course code and title — all one-way binding from AuthState

### `/admin/students` (Student List)
- Search input at top with `@bind:event="oninput"` — table filters live
- Bootstrap table with columns: Name, Email, Semester, CGPA, Enrollments count, Actions
- Actions: View (blue), Edit (yellow), Delete (red)
- Delete triggers `ConfirmDialog` modal first

### `/admin/students/add` and `/admin/students/edit/{Id}`
- Full Bootstrap form card
- All inputs use `@bind`
- Semester is a `<select>` dropdown (options 1–8)
- CGPA is `<input type="number" step="0.01" min="0" max="4">`
- On submit: run `IValidatable.Validate()` → show inline `is-invalid` class + `invalid-feedback` div below each failing field
- Success: show green AlertBox, reset form (Add) or show success message (Edit)

### `/admin/students/{Id}` (Detail)
- Full profile card: name, email, semester, CGPA
- Enrolled courses list sorted alphabetically by title using LINQ `.OrderBy(c => c.Title)`
- Grade history table using `GradeTable` component
- Average marks computed with LINQ `.Average(g => g.Marks)` displayed at top of grades section

### Course Catalog (`/courses`)
- Bootstrap card grid, 3 columns on desktop, 1 on mobile
- Each card uses `CourseCard` component

### `CourseCard` Component
- Shows: code, title, credit hours, EnrollmentStatus badge, enrolled/max count
- Bootstrap progress bar: width = `(enrolled / maxCapacity) * 100`%
- Color: green if Open, yellow if AlmostFull, red if Full
- `OnEnroll EventCallback<Course>` parameter — renders Enroll button only if callback is set

### `/student/enroll`
- Top section: available courses as `CourseCard` grid with Enroll button
- Bottom section: enrolled courses table with Drop button (disabled if course not active)
- Enroll calls the `EventCallback<Course>` → parent handler calls `ICourseService.EnrollStudent()`
- Catch `CourseFullException` → show `AlertBox` error
- Catch `InvalidOperationException` → show `AlertBox` warning

### `/faculty/grades` (Grade Submission)
- Dropdown to select assigned course (two-way `@bind`)
- When course selected (use `@bind:after` event) → load enrolled students into table
- Inline editable table: student name + marks input (`@bind` on each input, type=number 0–100) + live letter grade column (computed from current marks value)
- Row color: green row if A/B, yellow if C/D, red if F
- Validate: if marks < 0 or > 100, show `is-invalid` and disable Submit button
- Submit → calls `IGradeService.SubmitGrade()` for each row → shows success `AlertBox`

### `/student/grades`
- Top summary card: CGPA (large, green), total credits, courses graded
- Table using `GradeTable` component with conditional row colors

### `/admin/reports/grades`
- All students sorted by CGPA descending
- Rank column (#1, #2, ...)
- Summary badges below table: count in each grade category

### `NotificationBell` Component
- Bell icon (🔔) with Bootstrap badge showing unread count (red pill)
- Clicking opens Bootstrap dropdown with latest 5 notifications
- Each notification shows: type badge, message, timestamp
- Link at bottom: "View all notifications"
- Subscribes to `NotificationService.OnNewNotification` in `OnInitialized()`
- Unsubscribes in `IDisposable.Dispose()`
- Calls `InvokeAsync(StateHasChanged)` on new notification

### `/notifications`
- Full list for logged-in user
- Each row: type badge, bold if unread, message, timestamp, "Mark Read" button
- "Mark All Read" button at top right
- Reactive: subscribes to `OnNewNotification`

### `/admin/notifications` (Broadcast)
- Textarea with `@bind` for message
- Three checkboxes: Students, Faculty, Admins (at least one must be checked to enable Send)
- Send fires `NotificationService.AddNotification()` for every user matching selected roles
- Success `AlertBox` after send, form resets

---

## THE PHASES

Work through these phases one at a time. Do not proceed to the next phase until you confirm the current phase is working.

---

### PHASE 1 — Project Scaffold & Configuration

**Tasks:**
1. Run `dotnet new blazorserver -n EduConnect -f net8.0` (or verify it already exists)
2. Delete default pages: `Counter.razor`, `FetchData.razor`, `SurveyPrompt.razor`, `Index.razor` content
3. Create all folders: `Exceptions/`, `Interfaces/`, `Models/`, `Services/`, `Components/`, `Pages/Admin/`, `Pages/Faculty/`, `Pages/Student/`
4. Edit `_Host.cshtml` (or `App.razor` depending on .NET 8 template):
   - Add Bootstrap 5 CDN: `https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css`
   - Add Bootstrap 5 JS CDN: `https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js`
   - Add Google Fonts Poppins: `https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap`
   - Remove default Blazor CSS link (`bootstrap.min.css` from wwwroot if present)
5. Create `wwwroot/css/app.css` with:
   ```css
   *, body { font-family: 'Poppins', sans-serif; }
   .navbar-brand { font-weight: 700; letter-spacing: 0.5px; }
   .card { border-radius: 12px; }
   .btn { border-radius: 8px; }
   .form-control, .form-select { border-radius: 8px; }
   .table th { font-weight: 600; font-size: 0.85rem; text-transform: uppercase; letter-spacing: 0.5px; }
   .sidebar-none { display: none; }
   .page-wrapper { padding-top: 70px; }
   ```
6. Update `_Imports.razor` with all required `@using` statements (listed in Architecture section above)
7. Replace `Index.razor` with a simple redirect:
   ```razor
   @page "/"
   @inject NavigationManager Nav
   @code { protected override void OnInitialized() => Nav.NavigateTo("/login"); }
   ```
8. Run `dotnet build` → must be 0 errors

**Verification checklist before asking user to continue:**
- [ ] `dotnet build` → 0 errors
- [ ] All folders exist
- [ ] `_Imports.razor` has all namespaces
- [ ] Bootstrap 5 CDN is in the HTML host file
- [ ] Default pages removed

---

### PHASE 2 — Models, Exceptions, Interfaces

**Tasks (write files in this order, build after each):**

1. `Exceptions/StudentHasActiveEnrollmentsException.cs`
2. `Exceptions/CourseFullException.cs`
3. `Interfaces/IValidatable.cs`
4. `Interfaces/IRepository.cs` — generic `GetAll()`, `GetById(Guid)`, `Add(T)`, `Update(T)`, `Delete(Guid)`
5. `Interfaces/IStudentService.cs` — extends `IRepository<Student>`, adds `List<Student> Search(string term)` — NO grade methods (ISP)
6. `Interfaces/ICourseService.cs` — extends `IRepository<Course>`, adds `EnrollStudent(Guid courseId, Guid studentId)`, `DropCourse(Guid, Guid)`, `GetForFaculty(Guid)`, `GetAvailable()`
7. `Interfaces/IGradeService.cs` — `SubmitGrade(GradeRecord)`, `GetGradesForCourse(Guid)`, `GetGradesForStudent(Guid)`, `ComputeCGPA(Guid)` — completely separate from IStudentService (ISP)
8. `Models/EnrollmentStatus.cs` — enum: Open, AlmostFull, Full
9. `Models/NotificationType.cs` — enum: Enrollment, GradePosted, Announcement
10. `Models/Person.cs` — abstract, with Id/FullName/Email/PasswordHash, abstract GetRole()
11. `Models/Student.cs` — inherits Person, implements IValidatable
12. `Models/Faculty.cs`
13. `Models/Admin.cs`
14. `Models/Course.cs` — with computed EnrollmentStatus property (no setter)
15. `Models/GradeRecord.cs` — with computed LetterGrade and GradePoints (no setters)
16. `Models/Notification.cs`
17. `Models/AuthState.cs`
18. Run `dotnet build` → 0 errors

**Verification checklist:**
- [ ] `dotnet build` → 0 errors
- [ ] All 17 files exist and are complete
- [ ] `GradeRecord.LetterGrade` and `GradeRecord.GradePoints` are computed properties with no setter
- [ ] `Course.EnrollmentStatus` is a computed property with no setter
- [ ] `IStudentService` has zero grade-related methods
- [ ] `IGradeService` is a completely separate interface

---

### PHASE 3 — Services & Seed Data

**Tasks (write in this order, build after each file):**

1. **`Services/SeedData.cs`** — static class with static constructor. Initializes:
   - `public static List<Person> Users` — all 8 accounts (1 Admin, 2 Faculty, 5 Students)
   - `public static List<Student> Students` — same 5 student objects (shared reference with Users)
   - `public static List<Course> Courses` — 5 courses
   - Pre-enroll Wania and Abdulraheem in CS-101 and CS-201 (add to both `course.Enrolled` and `student.Enrollments`)
   - Fill CS-401 to 10/10 capacity with anonymous students
   - Assign courses to faculty (add to `faculty.AssignedCourses` and set `course.AssignedFacultyId`)
   - **CRITICAL:** The Students list and Users list must share the SAME object references, not copies. When a student's `Enrollments` or `Grades` list is modified at runtime, it must be visible everywhere.

2. **`Services/AuthStateService.cs`**
   - Scoped service
   - `public AuthState State { get; private set; } = new()`
   - `public event Action? OnAuthChanged`
   - `Login(string email, string password)` → find in `SeedData.Users`, set `State.CurrentUser`, invoke `OnAuthChanged`, return bool
   - `Logout()` → reset State, invoke `OnAuthChanged`

3. **`Services/NotificationService.cs`**
   - `public event Action<Notification>? OnNewNotification`
   - Private `List<Notification> _all = new()`
   - `AddNotification(Notification n)` → add to list, invoke event
   - `GetForUser(Guid userId)` → filter + order by Timestamp descending
   - `GetUnreadCount(Guid userId)` → count where !IsRead
   - `MarkAsRead(Guid notificationId)` → set IsRead = true
   - `MarkAllAsRead(Guid userId)` → set all matching IsRead = true

4. **`Services/StudentService.cs`** implements `IStudentService`
   - Uses `SeedData.Students` as the backing list (not a copy — the actual reference)
   - `Search(string term)` → case-insensitive contains on FullName or Email; empty term returns all
   - `Delete(Guid id)` → check `student.Enrollments.Any()` first, throw `StudentHasActiveEnrollmentsException` if true

5. **`Services/CourseService.cs`** implements `ICourseService`
   - Constructor receives `NotificationService` via DI (not newed)
   - Uses `SeedData.Courses` and `SeedData.Students` as backing lists
   - `EnrollStudent`: throw `CourseFullException` if Full; throw `InvalidOperationException` if already enrolled; add to both `course.Enrolled` and `student.Enrollments`; fire enrollment notification
   - `DropCourse`: throw `InvalidOperationException` if `!course.IsActive`; remove from both lists
   - `GetAvailable()` → courses where EnrollmentStatus != Full and IsActive == true
   - `GetForFaculty(Guid)` → filter by AssignedFacultyId

6. **`Services/GradeService.cs`** implements `IGradeService`
   - Constructor receives `NotificationService` and `ICourseService` via DI
   - Private `List<GradeRecord> _grades = new()`
   - `SubmitGrade(GradeRecord r)` → remove existing record for same student+course, add new one, recompute student CGPA, fire GradePosted notification for the student
   - `ComputeCGPA(Guid studentId)` → `Σ(GradePoints × CreditHours) / Σ(CreditHours)`, round to 2 decimal places, return 0 if no grades
   - After computing CGPA, update `student.CGPA` on the Student object in `SeedData.Students`

7. **`Program.cs`** — add all DI registrations (listed in Architecture section)

8. Run `dotnet build` → 0 errors

**Verification checklist:**
- [ ] `dotnet build` → 0 errors
- [ ] `SeedData` static constructor runs without throwing
- [ ] `AuthStateService.Login()` correctly finds users from `SeedData.Users`
- [ ] `StudentService` uses the actual `SeedData.Students` list reference (not a copy)
- [ ] `CourseService` constructor takes `NotificationService` as parameter (DI), not newed
- [ ] `GradeService.ComputeCGPA` uses weighted formula

---

### PHASE 4 — Shared Layout & Navigation

**Tasks:**

1. **Delete** `Shared/NavMenu.razor` (the default sidebar)
2. **Delete** the sidebar-related layout from `Shared/MainLayout.razor`

3. **Create `Shared/AppNavBar.razor`** — custom top navbar:
   - Uses `@inject AuthStateService AuthSvc` and `@implements IDisposable`
   - Dark navy background (`style` on nav element only for custom hex color, or add `.edu-navbar { background-color: #1a2744; }` in app.css)
   - Left: `🎓 EduConnect` brand link to `/dashboard`
   - Center: nav links, all conditional on `AuthSvc.State.IsAuthenticated` and role
   - Right: when authenticated → one-way binding displaying `@AuthSvc.State.CurrentUser!.FullName` + role badge + `<NotificationBell />` + Logout button
   - When NOT authenticated → Login link only
   - Subscribes to `AuthSvc.OnAuthChanged` in `OnInitialized()`, calls `InvokeAsync(StateHasChanged)`, unsubscribes in `Dispose()`
   - Logout button calls `AuthSvc.Logout()` then navigates to `/login`

4. **Rewrite `Shared/MainLayout.razor`** completely:
   ```razor
   @inherits LayoutComponentBase
   @inject AuthStateService AuthSvc

   <CascadingValue Value="AuthSvc.State">
       <AppNavBar />
       <div class="container-fluid page-wrapper">
           @Body
       </div>
   </CascadingValue>
   ```
   No sidebar. No `.sidebar`. No `.main` div with left margin. Clean full-width layout.

5. Run `dotnet build` → 0 errors

**Verification checklist:**
- [ ] `dotnet build` → 0 errors
- [ ] No sidebar-related HTML or CSS remains in layout files
- [ ] `CascadingValue<AuthState>` wraps `@Body`
- [ ] NavBar subscribes and unsubscribes from `OnAuthChanged` correctly
- [ ] NavBar shows role-specific links (code review, no runtime test yet)

---

### PHASE 5 — The 7 Reusable Components

Write each component fully. Build after each one.

**1. `Components/AlertBox.razor`**
- Parameters: `string Message`, `AlertType Type` (enum: Success/Warning/Error/Info, defined inside this component)
- Renders Bootstrap alert div with correct `alert-*` class
- Has X dismiss button that sets `Message = ""`
- Renders nothing if `Message` is empty or null

**2. `Components/ConfirmDialog.razor`**
- Parameters: `string Title`, `string Message`, `bool IsVisible`, `EventCallback OnConfirmed`, `EventCallback OnCancelled`
- Renders Bootstrap modal overlay when `IsVisible == true`
- Confirm button (red) → invokes `OnConfirmed`
- Cancel button (grey) → invokes `OnCancelled`
- Overlay background: semi-transparent dark div

**3. `Components/StudentCard.razor`**
- Parameter: `Student Student` (EditorRequired)
- Card showing FullName, Semester, CGPA with colored badge
- Badge: bg-success if CGPA ≥ 3.0, bg-warning if ≥ 2.0, bg-danger if < 2.0

**4. `Components/CourseCard.razor`**
- Parameters: `Course Course` (EditorRequired), `EventCallback<Course> OnEnroll`
- Bootstrap card with: code+title, credit hours, enrolled/max, Bootstrap progress bar, EnrollmentStatus badge
- If `OnEnroll.HasDelegate` is true → show Enroll button; disabled if Full
- Enroll button click → `await OnEnroll.InvokeAsync(Course)`

**5. `Components/GradeTable.razor`**
- Parameter: `List<GradeRecord> Grades` (EditorRequired)
- Bootstrap table with columns: Course, Marks, Letter Grade, Status (Pass/Fail)
- Row class: `table-success` for A/B, `table-warning` for C/D, `table-danger` for F

**6. `Components/LoadingSpinner.razor`**
- Parameter: `bool IsLoading`
- Shows Bootstrap spinner centered in a div when `IsLoading == true`
- Shows nothing when false

**7. `Components/NotificationBell.razor`**
- `@implements IDisposable`
- `@inject NotificationService NotifSvc`
- `@inject AuthStateService AuthSvc`
- `[CascadingParameter] public AuthState Auth { get; set; }` — for user ID
- Bell button with unread count badge
- Dropdown with latest 5 notifications
- `OnInitialized()` → subscribe to `NotifSvc.OnNewNotification += OnNotif`
- Handler: refresh counts + `InvokeAsync(StateHasChanged)`
- `Dispose()` → unsubscribe

Run `dotnet build` → 0 errors after all 7 components.

**Verification checklist:**
- [ ] `dotnet build` → 0 errors
- [ ] All 7 component files are complete (no stub methods)
- [ ] `CourseCard.OnEnroll` is `EventCallback<Course>`, not `EventCallback`
- [ ] `NotificationBell` implements `IDisposable` and unsubscribes
- [ ] `AlertBox` renders nothing when Message is empty
- [ ] `ConfirmDialog` only renders when `IsVisible == true`

---

### PHASE 6 — Authentication Pages

**1. `Pages/Login.razor`** (`@page "/login"`)
- Does NOT use `AuthGuard` — it IS the auth page
- Full-viewport centered layout (flexbox): `d-flex justify-content-center align-items-center` on a div with `style="min-height:100vh"`
- Card: width 420px, shadow, rounded
- Card header: `background-color: #1a2744`, white text, "🎓 EduConnect"
- Two fields: Email (`@bind="_email"`), Password (`@bind="_password"` type=password)
- Error shown via `<AlertBox>` component
- Login button: on click calls `AuthSvc.Login(_email, _password)` → if true navigate to `/dashboard`, if false set error message
- Hint box below card listing demo accounts
- **No navbar rendered on this page** — add `@layout EmptyLayout` and create `Shared/EmptyLayout.razor` (just renders `@Body` with no navbar)

**2. `Shared/EmptyLayout.razor`** — layout with no navbar, for login page only:
```razor
@inherits LayoutComponentBase
<div>@Body</div>
```

**3. `Pages/Unauthorized.razor`** (`@page "/unauthorized"`)
- Simple 403 page, centered, Bootstrap jumbotron style
- Large red 403, "Access Denied" heading, description, button back to dashboard

Run `dotnet build` → 0 errors.

**Test this phase manually:**
- Run the app: `dotnet run`
- Navigate to `/` → should redirect to `/login`
- Login page should render with no navbar
- Enter wrong credentials → error alert appears
- Enter `admin@edu.pk` / `admin123` → navigates to `/dashboard` (page will be blank for now, that's OK)
- Navbar should now appear with "Dr. Admin" and Admin role badge
- Logout button → navigates back to `/login`

**Verification checklist:**
- [ ] `dotnet build` → 0 errors
- [ ] Login page has no navbar (EmptyLayout)
- [ ] Wrong credentials shows error in AlertBox
- [ ] Correct credentials navigate to `/dashboard`
- [ ] Navbar shows user name after login
- [ ] Logout clears state and redirects to `/login`

---

### PHASE 7 — Dashboard & Courses Catalog

**1. `Pages/Dashboard.razor`** (`@page "/dashboard"`)
- `[CascadingParameter] public AuthState Auth { get; set; }`
- `OnInitialized()` → if not authenticated, navigate to `/login`
- **Admin view:** Stats row (total students, courses, enrollments), then 4 action cards linking to admin pages
- **Faculty view:** Greeting, list of assigned courses with enrolled count, quick links
- **Student view:** Summary cards (CGPA one-way binding, Semester, enrolled count), then list of enrolled courses — all using `@Auth` cascading parameter, not re-injecting services

**2. `Pages/Courses.razor`** (`@page "/courses"`)
- `[CascadingParameter] public AuthState Auth { get; set; }`
- Requires authenticated user → redirect to `/login` if not
- Displays all courses as Bootstrap card grid using `CourseCard` component (no OnEnroll callback here — read-only view)
- 3 columns desktop, 1 column mobile

Run `dotnet build` → 0 errors.

**Verification checklist:**
- [ ] Admin dashboard shows stats and 4 action cards (not blank)
- [ ] Faculty dashboard shows assigned courses
- [ ] Student dashboard shows CGPA, semester, enrolled courses (all one-way bound from Auth)
- [ ] `/courses` shows course cards for all roles

---

### PHASE 8 — Admin: Student Management (Full CRUD)

**This is the most critical phase — every sub-feature must work.**

**1. `Pages/Admin/StudentList.razor`** (`@page "/admin/students"`)
- Auth guard: Admin only
- Live search: `<input @bind="_search" @bind:event="oninput" />` — computed property `FilteredStudents` returns `StudentSvc.Search(_search)`
- Bootstrap table with View/Edit/Delete buttons per row
- Delete flow: set `_toDelete`, set `_showConfirm = true` → `ConfirmDialog` shows → on confirm call `StudentSvc.Delete()` → catch `StudentHasActiveEnrollmentsException` → show `AlertBox` error

**2. `Pages/Admin/StudentAdd.razor`** (`@page "/admin/students/add"`)
- Auth guard: Admin only
- Form bound to a `Student _student = new()` instance using `@bind` on every field
- Semester: `<select @bind="_student.Semester">` with options 1–8
- CGPA: `<input type="number" step="0.01" min="0" max="4" @bind="_student.CGPA">`
- Password field: `<input @bind="_student.PasswordHash">`
- On Save: run `_errors = _student.Validate()` → if any errors, show `is-invalid` + `invalid-feedback` per field → if no errors, call `StudentSvc.Add(_student)`, show success AlertBox, reset `_student = new()`
- Cancel → navigate to `/admin/students`
- **CRITICAL: The Save button must use `@onclick="Save"` where `Save` is a method in `@code`. Do NOT use an HTML form submit — Blazor event handlers only.**

**3. `Pages/Admin/StudentEdit.razor`** (`@page "/admin/students/edit/{Id:guid}"`)
- Auth guard: Admin only
- `[Parameter] public Guid Id { get; set; }`
- In `OnInitialized()` → load student via `StudentSvc.GetById(Id)` and assign to `_student`
- All fields pre-populated (one-way on load, two-way for editing)
- On Update: validate → if ok call `StudentSvc.Update(_student)`, show success

**4. `Pages/Admin/StudentDetail.razor`** (`@page "/admin/students/{Id:guid}"`)
- Auth guard: Admin only
- Show full profile: name, email, semester, CGPA
- Enrolled courses: LINQ sorted alphabetically `.OrderBy(c => c.Title)` as list group
- Grade history: `<GradeTable Grades="_student.Grades" />`
- Average marks if any grades: `_student.Grades.Average(g => g.Marks).ToString("F1")`

Run `dotnet build` → 0 errors.

**Test this phase manually before marking complete:**
- Navigate to `/admin/students` → table shows seeded students
- Type in search box → table filters with every keystroke
- Click Add Student → form opens
- Submit empty form → validation errors appear below each field
- Fill valid data → click Save → student appears in list
- Click Edit on a student → form pre-filled with their data → update → changes saved
- Click View → detail page shows profile, enrollments, grades
- Click Delete on a student with no enrollments → confirm modal → student removed
- Click Delete on Wania (has enrollments) → confirm → error alert "has active enrollments"

**Verification checklist:**
- [ ] Live search filters without button click
- [ ] Add form shows per-field validation errors on empty submit
- [ ] Add form saves and resets after successful submission
- [ ] Edit form pre-populates correctly
- [ ] Delete with active enrollments shows error AlertBox, does NOT delete
- [ ] Detail page shows sorted courses and grade history

---

### PHASE 9 — Admin: Course Management

**`Pages/Admin/CourseManagement.razor`** (`@page "/admin/courses"`)
- Auth guard: Admin only
- Add/Edit form above the table (toggles between Add and Edit mode)
- Form fields: Code, Title, CreditHours (number), MaxCapacity (number), IsActive (checkbox)
- Bootstrap table listing all courses with columns: Code, Title, Credits, Enrolled/Max, Status badge, Actions
- Edit: populates form with course data, changes button to "Update"
- Delete: ConfirmDialog → `CourseSvc.Delete()`
- Status badge colors: green Open, yellow AlmostFull, red Full

Run `dotnet build` → 0 errors.

**Verification checklist:**
- [ ] Add course form works
- [ ] Edit pre-fills form
- [ ] Delete with confirm works
- [ ] Status badges show correct colors

---

### PHASE 10 — Faculty: Courses & Grade Submission

**1. `Pages/Faculty/FacultyCourses.razor`** (`@page "/faculty/courses"`)
- Auth guard: Faculty only
- Shows `CourseSvc.GetForFaculty(Auth.CurrentUser!.Id)` as CourseCard grid

**2. `Pages/Faculty/GradeSubmission.razor`** (`@page "/faculty/grades"`)
- Auth guard: Faculty only
- Dropdown: select from faculty's assigned courses, two-way `@bind="_selectedCourseId"`
- Use `@bind:after="OnCourseSelected"` to load enrolled students when dropdown changes
- `OnCourseSelected()`: find course, build `List<GradeRow>` from enrolled students, check for existing grades
- Editable table: student name | marks input (`@bind="row.Marks" @bind:after="() => ValidateRow(row)"`) | live letter grade | validation icon
- Row validation: marks must be 0–100, show `is-invalid` if invalid, disable Submit if any invalid
- Submit button: for each row, call `GradeSvc.SubmitGrade(new GradeRecord { ... })`, show success AlertBox

```csharp
private class GradeRow {
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public int Marks { get; set; }
    public string Error { get; set; } = "";
    public string Grade => Marks switch { >=85=>"A", >=70=>"B", >=55=>"C", >=45=>"D", _=>"F" };
}
```

Run `dotnet build` → 0 errors.

**Verification checklist:**
- [ ] Faculty course list shows only their courses
- [ ] Selecting course in dropdown loads students
- [ ] Marks input updates letter grade column live
- [ ] Invalid marks (e.g. 150) shows error, disables submit
- [ ] Submit saves grades and fires GradePosted notification

---

### PHASE 11 — Student: Enrollment & Grades

**1. `Pages/Student/Enroll.razor`** (`@page "/student/enroll"`)
- Auth guard: Student only
- Available courses: `CourseSvc.GetAvailable()` as CourseCard grid with `OnEnroll` callback
- Enrolled courses: table with Drop button (disabled if course inactive)
- `DoEnroll(Course c)`: calls `CourseSvc.EnrollStudent(c.Id, Auth.CurrentUser!.Id)`, catches `CourseFullException` and `InvalidOperationException`, shows `AlertBox`
- `DoDrop(Course c)`: calls `CourseSvc.DropCourse(c.Id, Auth.CurrentUser!.Id)`, catches `InvalidOperationException`

**2. `Pages/Student/StudentGrades.razor`** (`@page "/student/grades"`)
- Auth guard: Student only
- CGPA summary card at top
- `GradeTable` component with student's grades

Run `dotnet build` → 0 errors.

**Verification checklist:**
- [ ] Available courses show with Enroll button
- [ ] Enrolling a full course shows AlertBox error
- [ ] Enrolling same course twice shows error
- [ ] Drop works for active course, blocked for inactive
- [ ] Student grades page shows CGPA and grade table

---

### PHASE 12 — Notification System & Admin Reports

**1. `Pages/NotificationsPage.razor`** (`@page "/notifications"`)
- Auth guard: authenticated
- `@implements IDisposable`
- Subscribe to `NotifSvc.OnNewNotification` in `OnInitialized()`, refresh list, unsubscribe in `Dispose()`
- List all notifications for current user: type badge, bold if unread, message, timestamp
- "Mark Read" per row, "Mark All Read" button

**2. `Pages/Admin/Broadcast.razor`** (`@page "/admin/notifications"`)
- Auth guard: Admin only
- Textarea with `@bind="_message"`
- Checkboxes for Students/Faculty/Admins
- Send button (disabled if no message or no role selected)
- On send: loop `SeedData.Users`, fire notification for each matching role

**3. `Pages/Admin/GradeReport.razor`** (`@page "/admin/reports/grades"`)
- Auth guard: Admin only
- Table: all students sorted by CGPA descending, with rank column
- Category summary cards below

Run `dotnet build` → 0 errors.

**Verification checklist:**
- [ ] Enrolling a student triggers Enrollment notification visible in bell
- [ ] Submitting grades triggers GradePosted notification
- [ ] Broadcast sends to correct roles
- [ ] Notification bell badge updates without page reload (reactive StateHasChanged)
- [ ] Grade report shows students sorted by CGPA

---

### PHASE 13 — Final Integration & Bug Fix Pass

This phase is a complete end-to-end test of every route and feature. Fix every bug found.

**Test script — run through every item:**

**As Admin (admin@edu.pk / admin123):**
- [ ] Login → dashboard shows stats and 4 action cards
- [ ] Navbar shows "Dr. Admin" with Admin badge
- [ ] `/admin/students` → student table loads, search filters live
- [ ] Add student with empty form → per-field errors shown
- [ ] Add student with valid data → appears in table
- [ ] Edit student → form pre-filled, update saves
- [ ] View student detail → enrollments sorted alphabetically, grades visible
- [ ] Delete student with enrollments → error, student NOT deleted
- [ ] Delete student without enrollments → confirm → removed
- [ ] `/admin/courses` → course list loads, add/edit/delete work
- [ ] `/admin/reports/grades` → students sorted by CGPA
- [ ] `/admin/notifications` → broadcast to Students → check student account receives it
- [ ] Logout → redirected to login

**As Faculty (ayesha@edu.pk / faculty123):**
- [ ] Login → faculty dashboard shows assigned courses
- [ ] `/faculty/courses` → shows CS-101, CS-201, CS-401
- [ ] `/faculty/grades` → select CS-101 → enrolled students load in table
- [ ] Enter marks, submit → success alert
- [ ] Student notification bell should show GradePosted after grade submission

**As Student (wania@student.edu.pk / student123):**
- [ ] Login → student dashboard shows CGPA, semester, enrolled courses
- [ ] `/student/grades` → shows grades if submitted by faculty, CGPA summary
- [ ] `/student/enroll` → available courses shown, CS-401 disabled (Full)
- [ ] Enroll in CS-284 → success
- [ ] Try enroll CS-284 again → "already enrolled" error
- [ ] Notification bell shows enrollment notification
- [ ] Drop CS-284 → removed from enrolled list
- [ ] `/notifications` → full list of notifications, mark as read works

**As Student navigating to admin page:**
- [ ] Navigate to `/admin/students` → redirected to `/unauthorized`

**Final build:**
- [ ] `dotnet build` → 0 errors, 0 warnings about missing implementations
- [ ] All 16 routes accessible (no 404 or blank pages)
- [ ] No page crashes on any user flow
- [ ] Notification bell updates reactively on enrollment and grade events

After completing all checklist items → report summary of what was tested and confirmed working.

---

## SOLID COMPLIANCE — ADD COMMENTS

After Phase 13, add a brief XML comment to the top of each service file identifying which SOLID principle it demonstrates:

```csharp
/// <summary>
/// SRP: This service has exactly one reason to change — student data management.
/// ISP: Implements IStudentService which contains no grade methods (grade concerns belong to IGradeService).
/// DIP: Injected as IStudentService interface — consumers depend on abstraction, not concrete type.
/// </summary>
public class StudentService : IStudentService { ... }
```

Add similar comments to: `GradeService`, `CourseService`, `NotificationService`, `AuthStateService`, `Person` hierarchy, `IRepository<T>`.

---

## END OF PROMPT

Start with Phase 1 now. After each phase is fully verified against its checklist, stop and ask:
`"✅ Phase [N] — [Name] complete. All checklist items verified. Shall I proceed to Phase [N+1]: [Name]? Reply YES to continue."`
