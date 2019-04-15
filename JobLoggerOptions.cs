using System.Collections.Generic;
using System.Linq;

namespace CodeReview
{
  public class JobLoggerOptions
  {
    public IEnumerable<LogDestinationEnum> Destinations { get; set; } = Enumerable.Empty<LogDestinationEnum>();

    public IEnumerable<LogLevelEnum> Levels { get; set; } = Enumerable.Empty<LogLevelEnum>();

    // NOTA: La auto-asignación solo se puede a apartir de C# 6 o usando un compilador que haga un downgrade del código para versiones inferiores
  }
}