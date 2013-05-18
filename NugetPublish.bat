@echo Output Directory: %1
@cd %1
@nuget Pack DataAccess.nuspec -BasePath %1 -OutputDirectory %1