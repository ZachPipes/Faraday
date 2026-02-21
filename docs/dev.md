To update migration and database:
- Run in src/Faraday.Infrastructure
  - ```dotnet ef migrations add "title"```
- Update db
  - ```PS C:\Users\zache\.dev\Projects\Faraday\src> dotnet ef database update --project Faraday.Infrastructure --startup-project Faraday.UI```
