FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
COPY . /mysrc
WORKDIR /mysrc
RUN dotnet publish -c Release -r linux-x64 -o /app --self-contained

FROM mcr.microsoft.com/dotnet/runtime:6.0.8-bullseye-slim-amd64 as runtime
ENV USE_DOCKER_ENV=true
COPY --from=build /app /app
WORKDIR /app
RUN chmod u+x Ao.Cache.CastleProxy.Sample
ENTRYPOINT [ "./Ao.Cache.CastleProxy.Sample" ]