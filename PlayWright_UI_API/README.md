# Packages
- Microsoft.Playwright
- Microsoft.Playwright.XUnit

# Toolings
- dotnet tool install --global Microsoft.Playwright.CLI
- playwright install
Or, use local chrome, from .runsettings
- <RunConfiguration>
    <EnvironmentVariables>
      <PLAYWRIGHT_CHROME_CHANNEL>chrome</PLAYWRIGHT_CHROME_CHANNEL>
    </EnvironmentVariables>
  </RunConfiguration>

- dotnet test --settings playwright.runsettings 
Or, configure in .csproj
- <RunSettingsFilePath>$(MSBuildProjectDirectory)\playwright.runsettings</RunSettingsFilePath>