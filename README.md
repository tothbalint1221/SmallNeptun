# SmallNeptun

ASP.NET Core Web API beadando oktatas-szervezo rendszerhez.

## Futtatas

1. Connection string: `SmallNeptun/appsettings.json`
2. Adatbazis neve alapbol: `CourseManagerDB`
3. Inditas utan Swagger: `/swagger`
4. Ures adatbazis eseten a `DataSeeder` automatikusan feltolti a tesztadatokat.

## Fo funkciok

- Felhasznalok kezelese, inaktivalas/reaktivalas
- Tantargyak kezelese, inaktivalas/reaktivalas
- Kurzusok hirdetese, modositasa, torlese
- Hallgatok kurzusfelvetele, lejelentkezese, atjelentkezese
- Orarendi idopontok kezelese
- Ertesitesek listazasa es hatterfolyamatbol generalasa

## Teszteles

Swagger UI-ban erdemes vegigprobalni:

- `POST /api/users/register`
- `GET /api/subjects`
- `POST /api/courses`
- `POST /api/subjects/{subjectId}/register`
- `POST /api/courses/{courseId}/schedule`
- `GET /api/notifications`
