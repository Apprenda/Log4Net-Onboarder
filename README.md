# Description #

The Apprenda log4net Onboarder is a generic solution that will allow any .NET guest application that uses log4net to maintain its usage of the log4net framework, while leveraging Apprenda’s centralized logging services to log messages to the Developer Portal.  

The Onboarder bundles a log4net Appender implementation for the Apprenda Platform 6.0.x Logging API, and leverages the Platform’s Custom Bootstrap Policy functionality to detect usage of log4net in .NET UI and WCF service components. If usage is detected, the Apprenda log4net Appender will be copied into the workload at deploy time and any existing log4net configuration will be augmented to use the Appender. This will set the default logging level to DEBUG to allow the cloud platform log overrides to govern message traffic flowing from the Appender to the Logging API.  

It should be noted that the Appender will be bootstrapped only for .NET UI and WCF service workloads, and will not apply to Windows Services workloads. 

# Requirements #
Visual Studio 2013 and the Apprenda Platform SDK available from http://docs.apprenda.com/downloads.

# Installation #

## Building ##
The solution may be built from the command line using 

    msbuild /m /p:Configuration=Release /p:Platform="Any CPU"

Or by building the solution in Visual Studio. In either case, the Bootstrap Policy archive will be created at the root of the repository, **Apprenda.LOG4Net.BSP.zip**.


## Create the log4net Appender Bootstrap Policy ##
The following Bootstrap Policy must be created in the Configuration>Application Bootstrap Policies section of the SOC.   

### Working Directory Bootstrap Policy ###

|General Settings	|Value |
|-------------------|------|
|Name	|log4net Appender Bootstrap Policy
|Description	|Bootstraps the log4net Appender into .Net UI and WCF service workloads and configures the workloads to leverage the Appender.
|Active Policy	|Yes|
|Applies to	|.NET Application Components|
|Application Stage	|Sandbox and Published|
|*Bootstrap Libraries	*|Run a custom workflow for deployments using the **Apprenda.Log4Net.BSP.zip** archive.
|Conditions	|Always bootstrap to all deployed components


Once the Bootstrap Policy is created, any newly deployed .NET UI and WCF service workloads will leverage the Appender to write logs that will appear in the Developer Portal.  Components deployed prior to the completion of this set up should be undeployed and then redeployed in order for the Appender to take effect.
