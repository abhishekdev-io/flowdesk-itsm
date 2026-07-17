# The Web layer of the backend.

### appsettings.json ---- 
"Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
}

This block configures the built-in Microsoft.Extensions.Logging providers. Once builder.Host.UseSerilog(...) is registered, Serilog takes over as the logging provider entirely — this Logging section has no provider left reading it, so it's inert. You can delete it safely.