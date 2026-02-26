# Task_20260225

직원 연락처 데이터를 업로드(JSON/CSV)하고 조회하는 ASP.NET Core Web API 프로젝트입니다.

## 구성

- `Task_20260225`: Web API 본 프로젝트
- `TaskTest`: NUnit 테스트 프로젝트
- `Task_20260225.sln`: 솔루션 파일

## 요구 사항

- .NET SDK 10.0

## 실행 방법

```bash
dotnet restore
dotnet run --project Task_20260225
```

실행 후 기본 주소 예시:

- `https://localhost:5001`
- `http://localhost:5000`

정적 페이지(`wwwroot/index.html`)가 기본 페이지로 제공됩니다.

## 테스트 실행

```bash
dotnet test
```

## API

기본 경로는 `/api` 입니다.

### 1) 직원 목록 조회 (페이지네이션)

- `GET /api/Employee?page={page}&pageSize={pageSize}`

예시:

```bash
curl "http://localhost:5000/api/Employee?page=1&pageSize=20"
```

### 2) 이름으로 직원 조회

- `GET /api/Employee/{name}`

예시:

```bash
curl "http://localhost:5000/api/Employee/김철수"
```

### 3) 직원 데이터 업로드 (파일)

- `POST /api/Employee`
- Form-data: `file` (.json 또는 .csv)

예시:

```bash
curl -X POST "http://localhost:5000/api/Employee" \
  -F "file=@Task_20260225/TestData/contact.json"
```

### 4) 직원 데이터 업로드 (텍스트)

- `POST /api/Employee`
- Form-data: `text` (JSON 문자열 또는 CSV 문자열)

예시:

```bash
curl -X POST "http://localhost:5000/api/Employee" \
  -F 'text=[{"name":"홍길동","email":"hong@test.com","phone":"010-1111-2222","date":"2026.02.25"}]'
```

## 샘플 데이터

- `Task_20260225/TestData/contact.json`
- `Task_20260225/TestData/contact.csv`

