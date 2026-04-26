# React + Vite

This template provides a minimal setup to get React working in Vite with HMR and some ESLint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the ESLint configuration

If you are developing a production application, we recommend using TypeScript with type-aware lint rules enabled. Check out the [TS template](https://github.com/vitejs/vite/tree/main/packages/create-vite/template-react-ts) for information on how to integrate TypeScript and [`typescript-eslint`](https://typescript-eslint.io) in your project.

## Backend AWS environment variables (important for this frontend)

This frontend uploads and displays product data from backend API. AWS credentials are required on backend side, not in React client code.

Use these backend variables:

- `AWS__AccessKey`
- `AWS__SecretKey`
- `AWS__Region`
- `AWS__BucketName`

### When to use each setup

1. **PowerShell session variables** - for quick one-time backend run from terminal.
2. **`launchSettings.json`** - for regular local development in IDE.
3. **`.env`** - only if backend code explicitly loads `.env` (ASP.NET Core does not do that automatically).

PowerShell example for backend start:

```powershell
$env:AWS__AccessKey="YOUR_ACCESS_KEY"
$env:AWS__SecretKey="YOUR_SECRET_KEY"
$env:AWS__Region="eu-north-1"
$env:AWS__BucketName="fsdaoct242ruproducts"
dotnet run --project "..\..\AWS Services Test FSDA_Oct_24_2_ru.csproj"
```

Security note: never store real AWS keys in frontend files or commit them to git.
