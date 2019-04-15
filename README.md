# Feedback

### Con respecto al constructor

- Es innecesario porque solo se tiene un método y es estático. Además, cada vez que se quiera crear una nueva instancia, los valores privados se van a modificar para todas las instancias debido a que son estáticos.

- Recibe muchos parámetros. Es preferible para este caso crear una clase para agrupar los parámetros (por ejemplo "JobLoggerOptions") y ahi se agreguen como propiedades. De esta manera si se agregan mas parámetros a la clase de opciones no se verían afectadas otras partes del código del proyecto donde se esté utilizando esta clase **JobLogger**. Ejm:
```csharp

using System.Collections.Generic;
using System.Linq;

public class JobLoggerOptions
{
  public IEnumerable<LogDestinationEnum> Destinations { get; set; } = Enumerable.Empty<LogDestinationEnum>(); 

  public IEnumerable<LogLevelEnum> Levels { get; set; } = Enumerable.Empty<LogLevelEnum>(); 

  // NOTA: La auto-asignación solo se puede a apartir de C# 6 o usando un compilador que haga un downgrade del código para versiones inferiores
}

public enum LogDestinationEnum
{
  File,
  Console,
  Database
}

public enum LogLevelEnum
{
  Message,
  Warning,
  Error
}
```

### Con respecto al método LogMessage

- Tiene un error en la definición de sus parámetros. Tiene dos parámetros que se llaman igual y debido a esto el código no va a compilar.

- las primera lineas del código del método está en el lugar incorrecto y se debe refactorizar. Se debe poner despues del if de validación del parámetro **message** y asignar el resultado de la función **Trim**. La validación de los destinos a donde se quiere loggear el mensaje esta duplicada. Adicionalmente se puede mejorar la validación:
```csharp
if (string.IsNullOrWhiteSpace(message))
  throw new ArgumentException("Message must be specified");

if (!_logToConsole && !_logToFile && !LogToDatabase)
  throw new Exception("There is no destination configurate to put the message");

if (!message && !warning && !error)
  throw new Exception("There is no level configurate for the message")

message = message.Trim();
```

- Después siempre se crea la conexión a la base de datos sin importar el destino que se escoga para poner el log. Debería validar que se escoga como destino la opción **LogToDatabase**.

- La variable **"t"** no expresa con el nombre cual va a ser su función dentro del código del método. Se puede inferir que es el tipo de log si se lee el query de inserección líneas abajo.

- El query de insercción debe estar dentro de la validación si se seleccione la opción **LogToDatabase**. 

- la variable **"l"** no expresa con el nombre cual va a ser su función dentro del código del método. Se puede inferir que se almacena el contenido del archivo "txt" de log.

- Es preferible refactorizar la lectura del valor con nombre dinámico como lo es **System.Configuration.ConfigurationManager.AppSettings["Log FileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt"** a una variable para que sea mas legible:
```csharp
var logFilePath = System.Configuration.ConfigurationManager.AppSettings["Log FileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt";

// ó 

var logFilePath = string.Format("{0}LogFile{1}.txt", System.Configuration.ConfigurationManager.AppSettings["Log FileDirectory"], DateTime.Now.ToShortDateString());

// ó

var logFilePath = $"{System.Configuration.ConfigurationManager.AppSettings["Log FileDirectory"]}LogFile{DateTime.Now.ToShortDateString()}.txt";

// NOTA: El literal-string solo se puede a apartir de C# 6 o usando un compilador que haga un downgrade del código para versiones inferiores
```

- los **if** donde se valida el tipo de nivel del log y se agrega el nuevo message es innecesario. Se debe refactorizar esa sección:
```csharp
```

- La escritura del archivo en disco debe estar dentro de la validación de la opción **_logToFile**.

- La ultima sección del código debe estar dentro de la validación de la opción **_logToConsole**.

### Tips en general

- Es preferible que se refencia el namespace de la clase en la parte superior del archivo para que el código sea mas legible.

- Se debe mantener un estándar para definir las variables privadas dentro de una clase. Por lo general se escriben con un **"_"** antes de cada nombre de variable. Pero puede variar por cada proyecto y depende del equipo de desarrollo.

- las variables en general se deben escribir con las palabras necesarias que expresen su función dentro del código.

- borrar variables que no se utilicen. Como por ejemplo la variable **_initialized**.

- Para esta clase es preferible exponer metodos que hagan especificamente lo que buscas realizar. Por ejemplo, La clase debe exponer n métodos públicos para imprimir el log de acuerdo a cada nivel que se maneja en su configuración. Y abstraer la lógica de escribir el log en un método privado que reciba como parámetro solo el mensaje y el nivel.