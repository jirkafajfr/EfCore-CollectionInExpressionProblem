FROM mcr.microsoft.com/dotnet/core/sdk:3.0

COPY CollectionInExpressionTest.sln .
COPY EfCore2x/EfCore2x.csproj EfCore2x/EfCore2x.csproj
COPY EfCore3x/EfCore3x.csproj EfCore3x/EfCore3x.csproj
RUN dotnet restore --no-cache

COPY . .
RUN dotnet test