# dotnet-cloudfoundry-broker
Implementation of a scaffold service broker written in dotnet. Based on the work of https://github.com/orangeglasses/service-broker-dotnet

The service broker expects a configuration setting named `Authentication:Password` to exist in the environment. This setting will be used as the basic authentication password by which the platform authenticates against the service broker. The corresponding username is `mybroker`.
