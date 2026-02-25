namespace Presentation.Exceptions;

public class SettingsMissingException(string missingSettingName)
: InvalidOperationException($"Settings were missing in appconfig.json.  Missing section was {missingSettingName}")
{
}
