# DotNetCore.EnsureRunning

Fastest and easiest way to setup a function that runs securly given number of times (N) among pool of (M) services

## Installation

Using nuget:
**Install-Package PeterKottas.DotNetCore.EnsureRunning**

## Explanation

Once again, I feel a need to first explain what are we trying to do and why. Imagine a situation where you need to poll a feed. You want to do it just once at a given time but at the same time, you want the execution to be fault tolerant. 

Initially, this calls for a scheduler and there's a good amount of these out there. However, sometimes, the performance can be important to you. Let's say you have a polling action and you want to trigger it again right after it finished. Schedullers usually works with cron expresions so schedulled jobs run only every minute at most and when you try to hack your way around it, it just doesn't work. Believe me :) 

Thats where *EnsureRunning* comes in. All you need is a storage (SQL is supported at the moment although I am looking at redis already). In a few lines of code, it will allow you to achieve this usecase without any worries. 

## Usage

1. Create .NETCore console app with a project.json simmilar to this:
	
	```cs
	{
		"version": "1.0.0-*",
		"buildOptions": {
			"emitEntryPoint": true
		},
		"frameworks": {
			"netcoreapp1.1": {
				"dependencies": {
					"Microsoft.NETCore.App": {
						"version": "1.1.0"//Optionally add "type": "platform" if you don't want self contained app
					}
				},
				"imports": "dnxcore50"
			}
		},
		"runtimes": { //Optionally add runtimes that you want to support
			"win81-x64": {}
		}
	}
	```
2. Configure library to use sql storage:
	
	```cs
	EnsureRunning.Configure(configruator =>
  {
    configruator.UseSqlServerStorage("Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;");
  });
	```
2. Use the api to run a function given amount of times securly and in very performant way across any number of machines or services
	
	```cs
	EnsureRunning.Action("hello-world-action", (state) =>
  {
    Console.WriteLine("Hello world action\nCounter:{0}", state.Counter);
    return new AfterActionConfig()
    {
      AfterActionBehaviour = AfterActionBehaviourEnum.RunAgain
    };
	}).WithBetweenActionDelay(1000).Run();
	```
3. Breakdown:
	
	```cs
	EnsureRunning.Action //Requires 2 arguments. First is an unique id of the action, second is the action itself.
	Func<ActionState, AfterActionConfig> action // This is how the action looks like. You get state with some useful info and you return AfterActionConfig that specifies what happens after action
	EnsureRunning.Action //Opens a chain api allowing you to provide further configuration, .WithBetweenActionDelay(1000) is I think quite self explanatory.
	//You start the action by using .Run() method
	```
4. Whole EnsuredAction api:
	
	```cs
	/// <summary>
	/// Action that is ensured to run given amount of times across multiple services
	/// </summary>
	public interface IEnsuredAction
	{
			/// <summary>
			/// Runs the action
			/// </summary>
			void Run();

			/// <summary>
			/// Configures the action to run once
			/// </summary>
			/// <returns></returns>
			IEnsuredAction Once();

			/// <summary>
			/// Configures the action to run given amount of times
			/// </summary>
			/// <param name="n"></param>
			/// <returns></returns>
			IEnsuredAction Times(int n);

			/// <summary>
			/// Add method that triggers when there is an exception in the action
			/// </summary>
			/// <param name="onException"></param>
			/// <returns></returns>
			IEnsuredAction WithOnException(Func<Exception, ActionState, AfterActionConfig> onException);

			/// <summary>
			/// Specifies behaviour of the library when there's an exception in onException
			/// </summary>
			/// <param name="onExceptionExceptionBehaviour"></param>
			/// <returns></returns>
			IEnsuredAction WithExceptionInOnExceptionBehaviour(AfterActionConfig onExceptionExceptionBehaviour);

			/// <summary>
			/// Specifies the delay between actions
			/// </summary>
			/// <param name="betweenActionIntervalMs"></param>
			/// <returns></returns>
			IEnsuredAction WithBetweenActionDelay(int betweenActionIntervalMs);

			/// <summary>
			/// This interval determines how often the runner checks the storage when waiting to start action execution
			/// </summary>
			/// <param name="storageCheckIntervalMs"></param>
			/// <returns></returns>
			IEnsuredAction WithStorageCheckInterval(int storageCheckIntervalMs);

			/// <summary>
			/// Hearbeat interval
			/// </summary>
			/// <param name="heartBeatIntervalMs"></param>
			/// <returns></returns>
			IEnsuredAction WithHeartBeatInterval(int heartBeatIntervalMs);

			/// <summary>
			/// Time after which heatbeat is considered to be dead and is automatically replaced by different runner
			/// </summary>
			/// <param name="heartBeatTimeoutMs"></param>
			/// <returns></returns>
			IEnsuredAction WithHeartBeatTimeout(int heartBeatTimeoutMs);

			/// <summary>
			/// Outputs debug info
			/// </summary>
			/// <returns></returns>
			IEnsuredAction WithDebugInfo();
	}
	```

## Created and sponsored by

- [GuestBell](https://guestbell.com/) - Customer centric online POS for Hotels and short terms stays.

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## License

MIT
