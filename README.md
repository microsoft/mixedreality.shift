# Introduction
*Shift* is a cross-platform application that is capable of running arbitrary pipelines on endpoints. Shift does not require complicated server-agent architectures and can be deployed quickly to endpoints without significant infrastructure requirements. It is capable of reporting results asynchronously via cloud-based telemetry.

## Running  *Shift*
Shift runs a user-defined *manifest* on a host. A manifest file is a list of components and tasks that makes up a product. Think of components as lego pieces, and the tasks as an instruction set. Shift downloads the components from the location specified in the manifest file, and performs a set of instructions specified sequentially. Essentially, Shift Core analyzes and interprets the manifest file.

`shift.exe run [path to the manifest file or standalone archive]`

## Packing maniests
Manifests can also be packed up so they can be executed offline without network connectivity. This is useful for air gapped, high security or low connectivity situations. The pack command produces an archive that contains all remote resources.

`shift.exe pack --manifest-path [path to the manifest file] --output-path [path to the output archive]`

## Additional commands and options
Additional commands can be discovered using the Shift application.

`shift.exe -h`

## Creating a manifest.json

Manifest contains a list of components, a list of bundles, component locations, and component tasks. Below is an example of a manifest file. 

```
{
  "Version": "{{VERSION}}",
  "Components": [
    {
      "id": "nullTaskComponent",
      "description": "Component with no task",
      "owner": "chaelee",
      "deviceDemands": [ "windows", "pc" ],
      "location": {
        "organization": "microsoft",
        "project": "180A7408-2DEB-44D9-AE8D-992FA4C3B9A6",
        "feed": "Product.Components.Release",
        "name": "nullTaskComponent",
        "version": "0.1.0"
      },
      "task": null
    },
  ],
  "Bundles": [
    {
      "bundles": [],
      "components": [
        "nullTaskComponent"
      ],
      "description": "The default bundle",
      "id": "default"
    }
  ]
}
```

| Section    | Attributes    | Description                                                                                                                                                                                                                           | Example                                                                                                                                                                                                                                                                                                | Type             |
|------------|---------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|------------------|
| Version    |               | Version of the manifest file. This should not change.                                                                                                                                                                                 | "{{VERSION}}"                                                                                                                                                                                                                                                                                          |                  |
| Components | id            | Unique identifier for the component                                                                                                                                                                                                   | "noTaskComponent"                                                                                                                                                                                                                                                                                      | string           |
|            | description   | Description of the component                                                                                                                                                                                                          | "Component with no task"                                                                                                                                                                                                                                                                               | string           |
|            | owner         | Owner of the component                                                                                                                                                                                                                | "chaelee", "chaelee@microsoft"                                                                                                                                                                                                                                                                         | string           |
|            | deviceDemands | Specification of where the component belongs.                                                                                                                                                                                         | ["windows", "hololens2", "xbox", "pc", "ios", "android"]                                                                                                                                                                                                                                               | array of strings |
|            | location      | ADO feed location of where the artifact sits. Currently we do not support other ADO location such as pipeline artifact, but we intend to implement it if there are enough requests. You can also specify network directory location.  | {"organization": "microsoft", "project": "180A7408-2DEB-44D9-AE8D-992FA4C3B9A6", "feed": "Product.Components.Release", "name": "pluginTestComponent", "version": "0.1.0"}, {"path": "\\\\mrfs\\private\\openxr\\ARDK-WinInProcVulkan\\CannonDemo" } | object           |
|            | task          | Task to perform with the artifact. See the task section for more details.                                                                                                                                                             |                                                                                                                                                                                                                                                                                                        | object           |
| Bundles    | bundles       | A bundle can reference another bundle.                                                                                                                                                                                                | ["default", "example_apps"]                                                                                                                                                                                                                                                                            | array of strings |
|            | components    | List of components                                                                                                                                                                                                                    | ["noTaskComponent"]                                                                                                                                                                                                                                                                                    | array of strings |
|            | description   | Description of the bundle                                                                                                                                                                                                             | "The default bundle"                                                                                                                                                                                                                                                                                   | string           |
|            | id            | Unique identifier for the bundle                                                                                                                                                                                                      | "default"                                                                                                                                                                                                                                                                                              | string           |


## Specifying tasks
### Powershell task

You can specify a PowerShell task as below:
```
{
    "id": "pwshTaskComponent",
    "description": "Test component with powersell task type",
    "owner": "chaelee",
    "deviceDemands": [ "windows", "pc" ],
    "location": {
        "organization": "microsoft",
        "project": "180A7408-2DEB-44D9-AE8D-992FA4C3B9A6",
        "feed": "Product.Components.Release",
        "name": "pwshTaskComponent",
        "version": "0.1.0"
    },
    "task": {
        "type": "pwsh",
        "script": "echo \"Hello, Shift\""
    }
}
```
### Custom task

Shift also allows you to specify custom tasks. This is done through writing a custom plugin for your product. Implement the abstract class `PluginDefinition`, and add it to the list of plugin definitions in the `Program.cs` file within the cli project. Below is an example component with a custom plugin task. Reach out to chaelee@microsoft.com for questions.
```
{
    "id": "pluginTaskComponent",
    "description": "Test component with plugin task type",
    "owner": "chaelee",
    "deviceDemands": [ "headset" ],
    "location": {
        "organization": "microsoft",
        "project": "180A7408-2DEB-44D9-AE8D-992FA4C3B9A6",
        "feed": "Product.Components.Release",
        "name": "pluginTaskComponent",
        "version": "0.1.0"
    },
    "task": {
        "type": "customPluginName",
        "action": "HeadsetInstall"
    }
}
```

## Configuring component locations
### Specifying a network directory location

You can also specify the network directory location to copy the file. This is not advised since it may be unreliable and we are unsure how long we will support this.

```
{
    "id": "directoryTestComponent",
    "location": {
        "path": "\\\\mrfs\\private\\openxr\\ARDK-WinInProcVulkan\\CannonDemo"
    }
}
```
### Specifying a HTTP location

You can also specify a file located on a webs erver.

```
{
    "id": "httpTestComponent",
    "location": {
        "uri": "https://microsoft.com/favicon.ico"
    }
}
```
### Specifying an Azure DevOps Feed location
```
{
    "id": "adoFeedComponent",
    "location": {
        "organization": "microsoft",
        "project": "180A7408-2DEB-44D9-AE8D-992FA4C3B9A6",
        "feed": "Product.Components.Release",
        "name": "nullTaskComponent",
        "version": "0.1.0"
    }
}
```
## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

