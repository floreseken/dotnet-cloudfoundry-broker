# dotnet-cloudfoundry-broker
Implementation of a scaffold service broker written in dotnet. Based on the work of https://github.com/orangeglasses/service-broker-dotnet

The service broker expects a configuration setting named `Authentication:Password` to exist in the environment. This setting will be used as the basic authentication password by which the platform authenticates against the service broker. The corresponding username is `mybrokeruser`.

# how to use
The broker it self must be running as an app on CF. To do this:

- `cf push` in the app folder
- set the env var **Authentication:Password** to your password
- restart the app

Now we must register the broker. This is done with:

- `cf csb mybroker mybrokeruser <password from Authentication:Password here> <full url of pushed broker app here>`
