# Translator API

ASP.NET Core API that stores translation history in Aiven PostgreSQL.

## Local setup

Rotate the Aiven password if it was pasted into chat or source control, then set the connection string as an environment variable:

```powershell
$env:AIVEN_POSTGRES="Host=pg-302f58a7-besher-d002.h.aivencloud.com;Port=21304;Database=defaultdb;Username=avnadmin;Password=<NEW_ROTATED_PASSWORD>;SSL Mode=Require;"
dotnet run --project translator.Api
```

For stricter certificate verification, download `ca.pem` from Aiven and use:

```powershell
$env:AIVEN_POSTGRES="Host=pg-302f58a7-besher-d002.h.aivencloud.com;Port=21304;Database=defaultdb;Username=avnadmin;Password=<NEW_ROTATED_PASSWORD>;SSL Mode=VerifyFull;Root Certificate=C:\path\to\ca.pem;"
```

## Endpoints

- `GET /health`
- `GET /api/translations`
- `POST /api/translations`

Example POST body:

```json
{
  "sourceLanguageCode": "en",
  "targetLanguageCode": "de",
  "sourceText": "Hello",
  "translatedText": "Hallo"
}
```

## Deploy to Render

This repository includes a root `Dockerfile` and `render.yaml` for Render.

1. Push the whole `C:\Users\beshe\Desktop\ubersetzt` folder to GitHub.
2. In Render, create a new **Blueprint** from the GitHub repository, or create a new **Web Service** using Docker.
3. Set the environment variable `AIVEN_POSTGRES` in Render:

```text
Host=pg-302f58a7-besher-d002.h.aivencloud.com;Port=21304;Database=defaultdb;Username=avnadmin;Password=<NEW_ROTATED_PASSWORD>;SSL Mode=Require;
```

4. Deploy the service.
5. Open:

```text
https://your-render-service.onrender.com/health
```

If it returns `{"status":"ok",...}`, the API is online.

For the desktop app, set `TRANSLATOR_API_URL` to your Render URL:

```powershell
$env:TRANSLATOR_API_URL="https://your-render-service.onrender.com/"
dotnet run --project translator
```

Keep the Aiven password out of source control. Add it only in Render's environment variable settings.
