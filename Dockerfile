FROM mcr.microsoft.com/dotnet/core/sdk:3.1
COPY . /app
WORKDIR /app/src/DataService/
RUN cd /app/src/DataService && dotnet build
CMD cd /app/src/DataService/bin/Debug/netcoreapp3.1/ && dotnet DataService.dll
