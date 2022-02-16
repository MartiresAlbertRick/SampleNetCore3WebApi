# Build

## Run (from source)

```
dotnet run -c release -r win-x64
```

## Build

```
dotnet build -c release -r win-x64
```

## Publish

```
dotnet publish -c release -r win-x64
```

## Build single-file executable - for deployment

### Install dotnet-warp tool

```
dotnet tool install --global dotnet-warp --version 1.1.0
```

If already installed, make sure you update to 1.1.0 (or latest)

```
dotnet tool update --global dotnet-warp --version 1.1.0
```

## Warp!

Now build, publish and warp/wrap the contents into a single executable:

```
dotnet-warp
```

If you get any build errors, run the command with --verbose

```
dotnet-warp --verbose
```

You should get an .exe file built in the root project folder.
