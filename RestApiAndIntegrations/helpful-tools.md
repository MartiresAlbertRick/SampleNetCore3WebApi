# Helpful dotnet-tools which you should install to assist with your projects

### Original source

https://github.com/natemcmaster/dotnet-tools

### Installation / usage

Tools can be installed globally and used in any project using the `-g` or `--global` switch.

Alternatively, tools can be installed locally, but in order to do this you need to create a local tool manifest:

`dotnet new tool-manifest`

```
D:\SOURCE\ad-caaps-api>dotnet new tool-manifest
The template "Dotnet local tool manifest file" was created successfully.
```

Once the manifest is installed, tools installed without the `-g` or `--global` switch, will be added to it.

Below log shows results of installing all tools globally

```
D:\SOURCE\ad-caaps-api>dotnet tool install Nake --version 3.0.0-beta-02 --global
You can invoke the tool using the following command: nake
Tool 'nake' (version '3.0.0-beta-02') was successfully installed.

D:\SOURCE\ad-caaps-api>dotnet tool install -g Korzh.DbTool
You can invoke the tool using the following command: dbtool
Tool 'korzh.dbtool' (version '1.4.1') was successfully installed.

D:\SOURCE\ad-caaps-api>dotnet tool install -g dotnetCampus.UpdateAllDotNetTools
You can invoke the tool using the following command: dotnet-updatealltools
Tool 'dotnetcampus.updatealldotnettools' (version '1.0.0') was successfully installed.
```

Below log shows results of installing all tools locally

```
D:\SOURCE\ad-caaps-api>dotnet tool install Nake --version 3.0.0-beta-02
You can invoke the tool from this directory using the following commands: 'dotnet tool run nake' or 'dotnet nake'.
Tool 'nake' (version '3.0.0-beta-02') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install dotnet-ef
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-ef' or 'dotnet dotnet-ef'.
Tool 'dotnet-ef' (version '3.1.3') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install Dacpac.Tool
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-dacpac' or 'dotnet dotnet-dacpac'.
Tool 'dacpac.tool' (version '2.0.0.391') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install Cake.Tool
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-cake' or 'dotnet dotnet-cake'.
Tool 'cake.tool' (version '0.37.0') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install Korzh.DbTool
You can invoke the tool from this directory using the following commands: 'dotnet tool run dbtool' or 'dotnet dbtool'.
Tool 'korzh.dbtool' (version '1.4.1') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install dotnet-config2json
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-config2json' or 'dotnet dotnet-config2json'.
Tool 'dotnet-config2json' (version '0.3.0') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install dotnet-dbinfo
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-dbinfo' or 'dotnet dotnet-dbinfo'.
Tool 'dotnet-dbinfo' (version '1.4.0') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install dotnet-depends
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-depends' or 'dotnet dotnet-depends'.
Tool 'dotnet-depends' (version '0.4.0') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install dotnet-property
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-property' or 'dotnet dotnet-property'.
Tool 'dotnet-property' (version '1.0.0.11') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install dotnet-retire
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-retire' or 'dotnet dotnet-retire'.
Tool 'dotnet-retire' (version '4.0.1') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install depguard
You can invoke the tool from this directory using the following commands: 'dotnet tool run depguard' or 'dotnet depguard'.
Tool 'depguard' (version '0.1.0') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install dotnet-try
You can invoke the tool from this directory using the following commands: 'dotnet tool run dotnet-try' or 'dotnet dotnet-try'.
Tool 'dotnet-try' (version '1.0.19553.4') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install localappveyor
You can invoke the tool from this directory using the following commands: 'dotnet tool run LocalAppVeyor' or 'dotnet LocalAppVeyor'.
Tool 'localappveyor' (version '0.5.5') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.

D:\SOURCE\ad-caaps-api>dotnet tool install NuKeeper
You can invoke the tool from this directory using the following commands: 'dotnet tool run nukeeper' or 'dotnet nukeeper'.
Tool 'nukeeper' (version '0.26.1') was successfully installed. Entry is added to the manifest file D:\SOURCE\ad-caaps-api\.config\dotnet-tools.json.
```

### Upgrade installed tools

```
dotnet-updatealltools
```

### More global tools

```
D:\SOURCE\ad-caaps-api>dotnet tool install -g PowerShell
You can invoke the tool using the following command: pwsh
Tool 'powershell' (version '7.0.0') was successfully installed.

D:\SOURCE\ad-caaps-api>dotnet tool install -g dotnet-reportgenerator-globaltool
You can invoke the tool using the following command: reportgenerator
Tool 'dotnet-reportgenerator-globaltool' (version '4.5.6') was successfully installed.

D:\SOURCE\ad-caaps-api>dotnet tool install -g dotnet-ssllabs-check
You can invoke the tool using the following command: ssllabs-check
Tool 'dotnet-ssllabs-check' (version '2.1.1') was successfully installed.

D:\SOURCE\ad-caaps-api>dotnet tool install -g WhiteSpaceWarrior
You can invoke the tool using the following command: WhiteSpaceWarrior
Tool 'whitespacewarrior' (version '1.0.1') was successfully installed.
```
