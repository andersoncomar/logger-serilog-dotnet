using System.Text.Json;

namespace Logger.Provider.ValueObjects
{
  public abstract class BaseSettings
  {
    public static string AppSettingName = "BaseSettings";

    public bool ShowHeader { get; private set; } = false;

    public override string ToString()
    {
      return JsonSerializer.Serialize(this, GetType());
    }
  }
}