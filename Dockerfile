FROM microsoft/dotnet:2.1.2-aspnetcore-runtime-alpine3.7
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source} .
RUN mkdir /app/data
VOLUME /app
ENTRYPOINT ["dotnet", "WebApi.dll"]