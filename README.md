# Cliente Ahorcado
## Proyecto en WPF (Windows Presentation Fundation) del juego del Ahorcado (Hangman) con integración del chat
---

### Requisitos previos
- Windows 10/11 con **.NET Framework** (la misma versión que usan los proyectos de servidor) instalado.
- **Visual Studio** (con la carga de trabajo de desarrollo de escritorio de .NET / WPF).
- Estar en la **misma red local** que las máquinas donde corren `ServidorAhorcado` y `ServidorChatAhorcado`, con los puertos correspondientes accesibles (ver el README de cada servidor).
- Conocer la **IP y el puerto** de ambos servidores antes de configurar el cliente.

### Clonar e instalar el proyecto
 
```bash
git clone https://github.com/MaxSJimenez04/ahorcado-cliente.git ClienteAhorcado
cd ClienteAhorcado
```
 
### Abrir el proyecto
 
No hace falta un `.sln`: abrir el `.csproj` directamente.
 
- Visual Studio → `File` → `Open` → `Project/Solution` → seleccionar `ClienteAhorcado.csproj`.4

### Restaurar los paquetes NuGet
 
Agregar (una sola vez, ya versionado en el repo) un `nuget.config` dentro de `ClienteAhorcado/`:
 
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="repositoryPath" value="..\packages" />
  </config>
</configuration>
```
 
Esto crea la carpeta `packages` un nivel arriba de `ClienteAhorcado/`, sin depender de ningún otro proyecto.
 
- **Desde Visual Studio:** la restauración automática descarga los paquetes al abrir el `.csproj`.
- **Desde línea de comandos** (requiere `nuget.exe` en el `PATH`):
```bash
  nuget restore ClienteAhorcado.csproj
```

### Compilar
 
`Build → Build Solution` (o `Ctrl+Shift+B`).


## Configurar la conexión a los servidores
 
El cliente sabe a qué IP y puerto conectarse gracias a los **endpoints** definidos en `App.config`. Por defecto, si fueron generados con `localhost`, **solo funcionarán en la misma máquina donde corren los servidores** — hay que reemplazar esas direcciones por la IP real del servidor en la red.

### Ejemplo
**Dentro de `App.config`**
``` xml
<bindings>
        <wsDualHttpBinding>
				<binding name="WSDualHttpBinding_ISesionService" clientBaseAddress="http://192.168.1.71:54670/Sesion">
					<security mode="None" />
				</binding>
				<binding name="WSDualHttpBinding_IEstadisticasService" clientBaseAddress="http://192.168.1.71:54670/Estadisticas">
					<security mode="None" />
				</binding>
				<binding name="WSDualHttpBinding_IPalabraService" clientBaseAddress="http://192.168.1.71:54670/Palabra">
					<security mode="None" />
				</binding>
				<binding name="WSDualHttpBinding_IPartidaService" clientBaseAddress="http://192.168.1.71:54670/Partida">
					<security mode="None" />
				</binding>
				<binding name="WSDualHttpBinding_IUsuarioService" clientBaseAddress="http://192.168.1.71:54670/Usuario">
					<security mode="None" />
				</binding>
        </wsDualHttpBinding>
    </bindings>
```

### Para el chat
Para conectar el chat, esté en un dispositivo diferente al servidor o dentro del mismo, sólo se debe cambiar la **direccion IP y el puerto** (generalmente escucha en el puerto *9000*)

**Dentro de `vistas/wPartidaJugador.xaml.cs` línea 289 - 302**

``` C#
    private async void ConectarChat()
        {
            _chat = new utils.ChatCliente();
            _chat.MensajeRecibido += OnMensajeChatRecibido;

            try
            {
                await _chat.ConectarAsync("192.168.1.68", 9000, _partidaActual.idPartida, utils.Sesion.Instancia.Usuario);
            }
            catch (Exception)
            {
            }
        }

```

### Abrir el puerto en firewall de Windows

```powershell
# Ejemplo: abrir el puerto 8080 para HTTP
New-NetFirewallRule -DisplayName "WCF Service HTTP" -Direction Inbound -Protocol TCP -LocalPort 8080 -Action Allow
 
# Ejemplo: abrir el puerto 808 para net.tcp
New-NetFirewallRule -DisplayName "WCF Service NetTcp" -Direction Inbound -Protocol TCP -LocalPort 808 -Action Allow
```

Se debe ejecutar con los puertos 54669 / 54670 para *Servidor* y puerto 9000 para *Chat*

## Ejecutar la aplicación
 
1. Confirmar que ambos servidores estén corriendo y accesibles (ver sección anterior y el README de cada servidor).
2. En Visual Studio, marcar `ClienteAhorcado` como **Startup Project** (si hay más de un proyecto abierto).
3. `F5` o `Ctrl+F5` para ejecutar.

