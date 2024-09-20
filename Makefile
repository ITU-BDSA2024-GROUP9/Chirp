build:
	dotnet build
clean:
	dotnet clean
restore:
	dotnet restore
watch:
	dotnet watch --project .\src\Chirp.CLI\ $(type)
run:
	dotnet run --project .\src\Chirp.CLI\ $(type)