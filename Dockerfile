FROM microsoft/dotnet:2.2.0-aspnetcore-runtime-bionic
ARG source
WORKDIR /app
EXPOSE 5000
COPY ${source} .
RUN mkdir /app/data
VOLUME /app
ENTRYPOINT ["dotnet", "WebApi.dll"]