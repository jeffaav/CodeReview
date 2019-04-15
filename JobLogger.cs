using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace CodeReview
{
  public class JobLogger
  {
    private readonly IEnumerable<LogLevelEnum> _logLevels;
    private readonly bool _logToFile;
    private readonly bool _logToDatabase;
    private readonly bool _logToConsole;
    private readonly string _connectionString;
    private readonly string _logFileDirectory;

    public JobLogger(JobLoggerOptions jobLoggerOptions)
    {
      // NOTA: el operador is existe apartir de C# 7
      if (jobLoggerOptions is null)
        throw new ArgumentNullException("jobLoggerOptions");

      if (jobLoggerOptions.Destinations is null || jobLoggerOptions.Destinations.Count() is 0)
        throw new Exception("There is no destination configurate to put the message");

      if (jobLoggerOptions.Levels is null || jobLoggerOptions.Levels.Count() is 0)
        throw new Exception("There is no level configurate for the message");

      this._logLevels = jobLoggerOptions.Levels;
      this._logToFile = jobLoggerOptions.Destinations.Contains(LogDestinationEnum.File);
      this._logToDatabase = jobLoggerOptions.Destinations.Contains(LogDestinationEnum.Database);
      this._logToConsole = jobLoggerOptions.Destinations.Contains(LogDestinationEnum.Console);

      var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .Build();
      

      this._connectionString = configuration["ConnectionString"];
      this._logFileDirectory = configuration["LogFileDirectory"];
    }

    public void LogMessage(string message) => LogMessageWithLevel(LogLevelEnum.Message, message);

    public void LogWarning(string message) => LogMessageWithLevel(LogLevelEnum.Warning, message);

    public void LogError(string message) => LogMessageWithLevel(LogLevelEnum.Error, message);

    private void LogMessageWithLevel(LogLevelEnum logLevelEnum, string message)
    {
      if (!_logLevels.Contains(logLevelEnum))
        return;

      if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException("Message must be specified");

      if (_logToFile)
        LogMessageToFile(logLevelEnum, message);

      if (_logToConsole)
        LogMessageToConsole(logLevelEnum, message);

      if (_logToDatabase)
        LogMessageToDatabase(logLevelEnum, message);
    }

    private void LogMessageToFile(LogLevelEnum logLevelEnum, string message)
    {
      var shortDate = DateTime.Now.ToString("yyyy-MM-dd");
      // NOTA: El literal-string solo se puede a apartir de C# 6 o usando un compilador que haga un downgrade del código para versiones inferiores
      var logFilePath = $"{_logFileDirectory}LogFile{shortDate}.txt";
      var fileContent = string.Empty;

      if (!Directory.Exists(_logFileDirectory))
        Directory.CreateDirectory(_logFileDirectory);

      if (File.Exists(logFilePath))
        fileContent = File.ReadAllText(logFilePath);

      fileContent += $"{FormatMessage(logLevelEnum, message)}\n";

      File.WriteAllText(logFilePath, fileContent);
    }

    private void LogMessageToConsole(LogLevelEnum logLevelEnum, string message)
    {
      switch (logLevelEnum)
      {
        case LogLevelEnum.Error:
          Console.ForegroundColor = ConsoleColor.Red;
          break;
        case LogLevelEnum.Warning:
          Console.ForegroundColor = ConsoleColor.Yellow;
          break;
        case LogLevelEnum.Message:
        default:
          Console.ForegroundColor = ConsoleColor.White;
          break;
      }

      Console.WriteLine(FormatMessage(logLevelEnum, message));
    }

    private void LogMessageToDatabase(LogLevelEnum logLevelEnum, string message)
    {
      var logType = 0;

      switch (logLevelEnum)
      {
        case LogLevelEnum.Error:
          logType = 2;
          break;
        case LogLevelEnum.Warning:
          logType = 3;
          break;
        case LogLevelEnum.Message:
        default:
          logType = 1;
          break;
      }

      using (var connection = new SqlConnection(_connectionString))
      // NOTA: El literal-string solo se puede a apartir de C# 6 o usando un compilador que haga un downgrade del código para versiones inferiores
      using (var command = new SqlCommand($"INSERT INTO Log VALUES('{message}', {logType})"))
      {
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
      }
    }

    // NOTA: El literal-string solo se puede a apartir de C# 6 o usando un compilador que haga un downgrade del código para versiones inferiores
    private string FormatMessage(LogLevelEnum logLevelEnum, string message) => $"[{logLevelEnum}]: {DateTime.Now.ToString()} - {message}.";

  }
}

