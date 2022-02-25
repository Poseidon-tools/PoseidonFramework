# Inseminator - DI addon to Poseidon framework.

Inseminator is a **dependency injection** framework built specifically to target Unity 3D and **Poseidon framework**.

## Status
#### Addon status: in development
Must-have todo list:
* ~~resolvers performance optimalization (reduce GC collection and CPU time usage -> optimize attributes searching)~~ -> **reflection baking** ✅
* property injection ✅
* method injection (bonus stuff) ✅
* multi-level injection (3x"i" - into-installer injection) ✅

# Compatibility

Tested in Unity 3D on the following platforms:

 - Standalone PC ✅
 - Android (Mono + .Net Standard 2.0) ✅
 - WebGL ✅
 - iOS ⏳
 
## Purpose

In contrast to other Unity DI-frameworks available, we chosen Inseminator to have simple duty. The main and only purpose of Inseminator is to **serve dependencies for given scope**. No other functionalities, no messaging, no pub-sub systems hidden inside.

## Why not existing framework?

The idea of making our own DI-system show up when we've spent many time on writing another and another state machines with states, where all dependencies was passed in constructors. **More complex state equals more dependencies**, and this equals **looooong constructors**, even 15+ lines straight down.
We've tried integrate Extenject framework with our workflow, but the way how Extenject works, is not meeting our pipeline requirements. The main problem that occured also in our pipeline, is described in post linked below - original post is almost 4 years old now, followed by over 450 people and still waiting for any tips or solution. Even if this issue will be resolved in future, Extenject require too much additional effort to make our state machines and states working.
So, we decided to get dependency injection in our hands and make a system, that will satisfy our requirements. It's not perfect and it's not even finished yet, but at this moment is working on our main platforms, and most important - with state machines, even nested couple of times.

Related post: https://answers.unity.com/questions/1455259/how-to-fix-the-exception-zenjectexception-unable-t.html

## Main features

So, what our DI-system is able to?
* **field injection and property injection**  at scene init (based on execution order) on any gameobject on scene, in any of your components -> see **Documentation/Injection**
* **injection in nested objects**, even if the object itself is not injected (even could be uninitialized and our system will try to force initialize it, if you'll allow for this in attribute) -> see **Documentation/Surrogates**
* **custom resolving modules support** - by default Inseminator comes with reflection-based injection module, which will handle injecting and surrogates for you. But you can implement your own module for more complex tasks - like we did by implementing custom module for resolving state machines, states inside and states dependencies(including nested state machines etc). See **Documentation/ResolvingModules**
* resolving for **3 main scopes - Scene, GameObject and ScriptableObject**. You can install your dependencies for scene objects (it's a main container for whole scene), you can use GameObjectDependencyResolver to separate your game object from scene dependencies and make it self-sufficient, using other set of installers/objects. Finally, you can use ScriptableObjectDependencyResolver to inject things in scriptable objects. To be honest, we don't think it will be useful, but it's ready to go.

# Documentation

How to use Inseminator system?
Table of content:
* Simple Injection
	* Dependency resolvers
	* Installers & Binding
	* Resolving Modules
* Reflection baking
* Surrogates
* Method Injection via Inseminator
* Injecting into instantiated object
	* Factories
* Advanced stuff
	* Self-sufficient instantiated objects
	* Resolving Poseidon's State Machines


## Simple Injection
### Default way

By default, we can inject value into object in a few ways. We can do it by constructor (if we're working on non-mono classes), in MonoBehaviours we can do it with some sort of injection method and pass dependencies as method params, or make a ugly and low-performance calls to one of Unity's Find methods, like FindObjectOfType\<T>.
```C#
// constructor injection example
public MyClass(MyObject myObject)
{
    this.myObject = myObject;
}

// method injection example in Mono class
public void Initialize(MyObject myObject)  
{  
  this.myObject = myObject;  
}

// using Unity's default, low-performance methods in Mono class
private void Start()
{
    myObject = FindObjectOfType<MyObject>();
}
```
### Using Inseminator
Injecting values into MonoBehaviours is pretty simple, and require you to use special attribute, that comes with Inseminator.
```C#
public class MonoInjectable : MonoBehaviour  
{  
	#region Properties
	// property injection
	[field: SerializeField, InseminatorAttributes.Inseminate, PreviewField]
        public MessageData SceneScopeMessageData { get; private set; }
	#endregion
	#region Private Variables  
	[InseminatorAttributes.Inseminate] private ViewManager viewManager;  
	[InseminatorAttributes.Inseminate] private InseminatorMonoFactory monoFactory;  
	#endregion
	#region Unity Methods
	private void Start()  
	 {
		 Debug.Log($"Hey, {name} here! I'm using properly injected ViewManager: {viewManager.name}");  
	 }
	 #endregion
 }
```
As a result you'll see message logged in console, and suprisingly no NullReferenceException error, although at first glance it seems that viewManager can't have value, because it's nor exposed in inspector or initialized via any method.

Using `[Inseminate]` attribute is only a top of an iceberg, but it's required to tell Inseminator system that you have field in your component, waiting for value to be injected. Next step is dependency resolvers setup.

### Dependency resolvers
Basically dependency resolver is a container, which will deal with installing and assigning dependencies. **Each dependency resolver is considered as separate scope** (working on scope parenting at this moment). For now, you cannot establish communication between subscope and main scope. It means that each of your dependency resolvers should be self-sufficient and should be able to serve dependencies in it's scope on it's own. This will become more understandable after default resolvers introduction part.
#### Default resolvers
Inseminator comes with 3 default dependency resolvers included:

 - Scene dependency resolver
 - Game object dependency resolver
 - Scriptable object dependency resolver
 
The most important resolver is the **scene dependency resolver**. In case of scene-wide dependencies, you'll definitely be using this one most of the time.

**Game object resolver** lets you cut off from main scene scope and create subscope for game object and it's children. For example, if you'll be using some dynamic loaded AR prefab with complex internal structure (like state machine, separate UI, minigames etc), you'll probably want to use other set of dependencies for this internal structure.
For now, game object resolver isn't able to communicate with scene scope or any parent scope in order to borrow some of their dependencies, but this is a feature that Inseminator definitely lacks and will be added ASAP.

**Scriptable object dependency resolver** works just like game object dependency resolver, with one difference - it's operating on scriptable objects found in project. You can inject values inside the scriptable objects and these values will be legit in build.

#### How it works
Dependency resolver is obligated to deal with main DI process - it should gather all objects interested in injecting, install dependency bindings and finally inject values.
To keep things simple, dependency resolver is not doing any complex operations on it's own. It should be rather considered as DI control center, which is using modules to accomplish DI-process.

The DI process is based on two operations: dependency installation & binding and dependency injection.
Dependencies are installed and registered in DI lookup dictionary via Installers, and dependency resolving modules are looking for fields/properties/custom things to accomplish injecting operation.

Both installers and dependency resolving modules can be easily added/removed or swapped using inspector of dependency resolver.

### Installers & Binding
First step to create new dependency, that will be injected to other places, is to create installer and binding.

Installer has only one task - tell the dependency resolver, that there are certain bindings for certain types. It's not strictly set that you should create separate installer for every new binding - you also can keep everything in one installer, but we're recommending splitting installers into logical chunks.

**Creating an installer**

To create new installer, simply make a new class and derive from `InseminatorInstaller`. Next, implement method 
```C#
InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
``` 
which is required in every new installer.

Let's assume you want to install 3 instances of text loggers, and each is derived from ITextLogger interface.
Nothing more simpler, just fill body of overriden method with .Bind<> calls to the dependency resolver:
```C#
public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)  
{  
	//simple logger, nothing fancy
	 inseminatorDependencyResolver.Bind<ITextLogger>(new TestLogger(), "TestLogger");  
 	// logger which will log text colored to green
	 inseminatorDependencyResolver.Bind<ITextLogger>(new GreenTextLogger(), "GreenTextLogger");  
  	// logger which will log text colored to red, with font size of 60
	 inseminatorDependencyResolver.Bind<ITextLogger>(new CustomLogger(Color.red, 60), "CustomLoggerRed60");
}
```

What exactly to pass in the call?
First, specify the target binding **type in <>** braces. In this case, we want to bind new instances to the ITextLogger type.
Then, **pass the instance of your type** as first method param.
**Optionally**, pass the **instance identifier as second method** param.

**Instance identifier is extremely important** in cases, when you have **more than one bindings for certain type** - like we do in this example. So, with more than one binding per type, you should use instance identifiers. It's up to you what will be the id - it could be string, enum, type or anything else. Just remember, that later on you have to access this identifier in order to tell the DI system, that in this particular class you want to inject this particular instance of an object.

When you're done with your bindings, **last thing to do is to create a installer object in your hierarchy** (good practice is to keep installers under some "Installers" object in your dependency resolver object's hierarchy) and **add it to the installers list in dependency resolver**. 
When this is ready, you can specify what do you need to be injected in your class.

### Insemination
Let's say your class is pretty simple, and all you want to do is to print some red text.
```C#
public class TextPrinter : MonoBehaviour
{  
	[InseminatorAttributes.Inseminate(InstanceId = "CustomLoggerRed60")]
	private ITextLogger testLogger;

	private void OnEnter()
	{
		testLogger.LogMessage("some red text");
	}
 }
```
One and only thing you should worry about is to specify proper instance id in `[Inseminate]` attribute (if there is more than one binding for your target type). In installer example above, we just binded 3 different instances to ITextLogger type, so in order to print red text (as required), you need to specify proper instance id.

### Dependency resolving modules
Dependency resolver is not doing any magic itself. It's using resolving modules to perform injecting operations. Inseminator comes up with 4 basic modules, that will accomplish any "common" injection scenario, including nested injections and injections into Poseidon's state machines. 
Also, **Inseminator is now armed** with alternate resolving conception - **reflection baking & prebaked resolving**.

**Basics**

In order to using Inseminator properly, all you have to know is that dependency resolving modules are running in DI-resolving process, once per every resolved object. This is the dirty work that should be made, and it's made by resolving modules. 

Standard resolving module is basic module, that is looking for fields tagged with one of Inseminator attributes, and it's asking dependency resolver for binding requested by this particular field. If dependency is found, module is setting value of the field and the process is going from scratch for next field. It's the standard resolving module, but it's not the perfect one. 
Dependency resolving includes a lot of reflection calls and some of them are really heavy(especially on mobile devices), so if you're targeting low-end devices with a lot of dynamic object instancing via factories etc, please read about reflection baking below.

State machine resolving module is the next module included with Inseminator.  The principle stays unchanged - module is running over and over again for each of resolved objects. It's basically looking for StateMachine<T> fields in objects, and then for it's states. When states are found, it's calling dependency resolver to perform whole DI-resolving process for these states, so all the resolving modules will run on states. This mechanism let us resolve dependencies even in really, really deep places of your structure, like in states declared in state machine, which is declared in another state of another state machine, which is declared in one of scene objects.

**Prebaked module**
	
The last default modules are the prebaked modules - one for resolving baked injection fields, second one for resolving baked state machines.
These modules are used in dependency-baking approach, which is described below. Although both are still reflection based, they're faster and more memory-friendly than standard modules, because the amount of expensive reflection calls is reduced. 
PrebakedInjectionModule is an baked alternate for StandardInjectingModule, and the BakedStateMachineResolvingModule is an alternate for StateMachineResolvingModule.

	
### Reflection baking
As mentioned above, standard dependency resolving approach is pretty expensive thing if we consider it running dynamically in 60fps application. It's definitely worth using with many static scene dependencies and a few dynamic dependencies, or when the case is rather multi-screen bussiness-like app than super smooth mobile game. Often reflection calls can really slow down any app, so this is why dynamic dependency resolving could cause frame drops on low-end devices.

But there is alternate approach for dependency resolving, and it's called reflection baking. The main idea of this approach is to avoid expensive reflection calls required to get informations about field's attributes. This approach can speed up even dynamic resolving without so much harrasment to the framerate - but you should be warned, that it's not perfect. It still need to spend some CPU time on injecting and allocate some memory to get it's bussiness done.

	
#### How to use
To get this approach working, all you need to do is:
* **use prebaked resolving module** in all your dependency resolvers, instead of standard resolving modules 
* **regularly bake project** using menu option "Tools/Inseminator/Bake"
* optionally you can enable useful checkbox in InseminatorSetting scriptable object, called "**Auto Bake On Build**" -> Inseminator will perform baking process before every build
* next optional thing you can **specify is the Prefabs paths**: you can specify where to search for prefabs that require baking. **For larger projects is heavily recommended** to keep your bake-required prefabs in separate place and specify paths in settings, otherwise baking process could take much more time, cause it will search whole project for bakeable prefabs.

**Injection stays the same** - you tag field with attribute (Inseminate or Surrogate) and prebaked resolver will handle rest of work for you.

#### How it works
The main issue in standard resolving process is the attributes resolving from fields. `GetCustomAttributes<>()` **call is really expensive** - it is allocating memory and it's using noticeable CPU time per single request. **Dependency resolving requires hundreds (and more) calls**, so the outcome is that framerate may drop badly when you'll try to dynamically instantiate new object(requiring some dependencies or having separate game object scope) via factory at runtime. 

For simple resolving at the scene lifecycle beginning it's totally ok and **could be for example masked with some loading screen**, but frame drop caused by dynamic instantiation is almost impossible to hide.

**What the baked approach does**, is to avoid these calls to GetCustomAttributes and, for example, resolving state machines at runtime (which requires also some heaviest reflection calls). **All the hard and resources-consuming work is done in baking process**, when baking modules are scanning objects scene by scene, prefab by prefab looking for bakeable fields, surrogates and state machines. When one of mentioned is found, **it's stored in dictionary, inside special data object**. Dictionary is matching the type of object with injectable/surrogate fields or state machine fields names.
The baking data object is then saved as .json file inside Resources folder.

At runtime, **baked dependency resolvers are not longer looking for any attributes or state machine fields**, but they **know exactly what to seek for** in particular object types - using dictionary with type/list of fields names matching. Thanks to this, resolving process is **much faster** (4-9x) and it's consuming about **3-10x less memory** (depending on resolving complexity). 

### Surrogates

Making a surrogate field is a way to tell DI-system, that there is a object that does not exactly wait for value injection, but contains nested dependencies inside, that need to be also resolved. Of course, **you can create surrogate containing surrogates**, that's completely fine, so far as you you're injecting some value at the end. **Keeping empty surrogate fields** inside classes cause empty-searching for dependencies to resolve and **is slowing down resolving process**.

#### How to use
Let's assume that beside injecting directly values into the class fields, you want to have another object, that requires dependency resolving in its structure. All you have to do is marking this field with `[InseminatorAttributes.Surrogate]` attribute, optionally passing `ForceInitialization` param value.
```C#
[InseminatorAttributes.Surrogate]  
private TestNestedModuleInjection nestedModuleInjection = new TestNestedModuleInjection();
```
With having this done, you can add injectable or surrogate field that need to be resolved.
```C#
public class TestNestedModuleInjection
{  
 #region Private Variables  
 [InseminatorAttributes.Inseminate] private ViewManager usedViewManager;  
 [InseminatorAttributes.Surrogate] private NestedInNested nested = new NestedInNested();  
 [InseminatorAttributes.Surrogate(ForceInitialization = true)] private NestedInNested nestedUninitialized;
 #endregion
 // ... rest of the class
 }
```
Let's take a closer look at these fields. `TestNestedModuleInjection` object is marked as surrogate field in another place in code, and it's containing one injectable field and two surrogates.
So, the standard Inseminate field and standard surrogate field should not be a mystery now, but there is third field, with additional parameter for surrogate attribute.
It's `ForceInitialization` property and it's used to tell DI-system, that **before resolving values inside this surrogate, it should be force initialized if it's not initialized yet**. So basically, you can have uninitialized object as surrogate, and it'll be initialized for you(if possible - remember, it **won't work for Unity's Mono objects**).
Getting back to the class:
```C#
public class TestNestedModuleInjection
{  
 #region Private Variables  
 [InseminatorAttributes.Inseminate] private ViewManager usedViewManager;  
 [InseminatorAttributes.Surrogate] private NestedInNested nested = new NestedInNested();  
 [InseminatorAttributes.Surrogate(ForceInitialization = true)] private NestedInNested nestedUninitialized;  
 #endregion  
 #region Public API  
    public void Alert()  
	 {
		Debug.Log($"Used ViewManager is: {usedViewManager.name}", usedViewManager);
		nested.Alert();
		// variable was not initialized manually using ctor
		nestedUninitialized.Alert();  
	 }
#endregion  
}
```
The field `[InseminatorAttributes.Surrogate(ForceInitialization = true)] private NestedInNested nestedUninitialized;` is not initialized anywhere, and expected result of calling `nestedUninitialized.Alert()` method could be **NullReferenceException**. But, thanks to `ForceInitialize` param, the field **will be initialized during dependency resolving process** and it's structure also will be resolved.

	
### Method Injection via Inseminator

It's possible to inject value into the field and property, but what about using injection method?
Let's assume, that after injection you want to use the values immediately, just after injection process has given you the dependencies. Using standard field/property injection is not the way, but you can use dedicated feature called method injection.
First, example:
```C#
// define the attribute of method
[InseminatorAttributes.InseminateMethod(ParamIds = new object[]  
 {"SampleMessage", "SecondaryMessage"})]  
 // specify method declaration
private void TestMethodInjection(MessageData messageData, MessageData secondaryMessage)  
{  
  // cache injected values and use them
  this.secondaryMessage = secondaryMessage;  
  this.messageData = messageData;
  Debug.Log("Method injection messages:");  
  Debug.Log($"Sample: {messageData.Message}");  
  Debug.Log($"Secondary: {secondaryMessage.Message}");  
}
```
To properly make use of method injection, you have to notice few things:
* method cannot be static
* you have to define method parameters, method without params won't be properly invoked
* you have to attach special attribute
* if you want your injecting params to be resolved from specifed instance, you have to fill ParamIds array in attribute

The attribute to use with method injection is `InseminateMethod` attribute.

#### Tricky case
What if you have three params, but you only want 1st and 3rd param to have id? It's quite simple - you have to specify ParamIds array like that:
```C#
[InseminatorAttributes.InseminateMethod(ParamIds = new object[]  
 {"SampleMessage", string.Empty, "SecondaryMessage"})]
```
So the first param will be waiting for instance with "SampleMessage" id, second param will wait for the main instance of an object(without id specified or first of all instances with id, if non-id instance is not defined), and third param will be waiting for instance with "SecondaryMessage" id.

### Injecting into instantiated object
As described previously, the main part of DI-process is performed at the scene startup. Scene objects are resolved and flow is going on. But the tricky part is when you want to instantiate new object on scene, and it's containing custom components requiring dependency resolving. 
For the record - the standard Unity way of instantiating objects is following:
```C#
var newObject = Instantiate(templateObject, parent);
```
Let templateObject has one main component of DynamicItemExample type:
```C#
public class DynamicItemExample : MonoBehaviour  
{  
#region Inspector  
	[SerializeField] private TMP_Text textRenderer;  
#endregion  
#region Private Variables  
    [InseminatorAttributes.Inseminate] private ITextLogger textLogger;  
#endregion
#region Unity Methods  
    private void OnEnable()
    {
    textLogger.LogMessage($"{name}", textRenderer);
    }
#endregion  
#region Public API  
    public void OverrideText(string text)
    {
	    textLogger.LogMessage(text, textRenderer);
	}
#endregion  
}
```
If `templateObject` will be instantiated using default approach, **NullReferenceException** will occur anytime calling textLogger.LogMessage method.
It's all because scene objects are already resolved, and the DI-process is not running anymore, so the dependency in this instantiated object cannot be resolved.
#### Factories
To get instantiated object working properly(with its dependencies) with DI-system, you need to replace Instantiate calls with **MonoFactory**.

`InseminatorMonoFactory` is prepared exactly for dealing with dynamic object instantiation and resolving its dependencies in a fly.

#### How to use
In order to use `InseminatorMonoFactory` for instantiating objects, you need to **install it first**.
Just like installation of other dependencies, **all you have to do is to add FactoryInstaller** object somewhere in your resolver hierarchy, and then drag it to the installers list in resolver's inspector.

Next step is **injecting factory in place you want to use it** for instantiating objects at runtime.
You can do it simply, just as any other field:
`[InseminatorAttributes.Inseminate] private InseminatorMonoFactory factory;`

And finally, for instantiating your object with resolved dependencies:

`var item = factory.Create(outroView.TemplateItem, outroView.ItemsContainer);`

The `Create<T>(template, parent)` method is a generic method, so you can pass to it anything what is derived from **Component** type (basically any of your MonoBehaviours), and it'll create this object for you. 

After calling `Create` method (If there is no errors etc), your object is ready to go and you can reposition it etc.

#### Custom factories
You can create your own factory that will do more complex work for you. It's quite simple, because it works just as standard installed dependency, except factory have to have dependency resolver injected to be able to call resolving proces for freshly instantiated object. If you'll need to implement factory on your own, look at `InseminatorMonoFactory` class for reference.


### Advanced
#### Self sufficient objects
The rare, but still possible case is that you want to dynamic instantiate an object, which won't use scene scope in order to get references itself(or for any of it's child objects). It's called self-sufficient object and can be done in quite simply way.

Your prefab should have GameObjectDependencyResolver component, with declared installers and modules and voila - self-sufficient object is set up. You can declare your own dependencies inside this scope and main-scene scope won't touch this prefab after instantiating. But remember, that self-sufficient objects also have to be created via factory.

#### State Machine Resolving
How to properly setup state machine to work with Inseminator? For injecting purposes, it's extremely important to have state machine initialized in field declaration, just like that:
```C#
private StateMachine<ApplicationState> appStateManager = new StateMachine<ApplicationState>(  
  new State<ApplicationState>[]  
 {
	new AppIntroState(),  
	new ApplicationExampleState(),  
	new ApplicationOutroState()  
 });
```
It's important because state machine resolving module is expecting to retrieve states inside state machine, so the state machine initialization with states declaration should be done immediately along with field declaration. Without declared states, resolving module cannot resolve anything, simply because state machine States[] array is empty.

State machine resolving itself works just like standard resolving approach -> all you need to do is to add preferred state machine resolving module to your Dependency Resolver. It could be StateMachineResolvingModule or BakedStateMachineResolvingModule.

##### Dynamic state machines ❌
Just for keep things clear - **it's not possible to dynamically create state machine and get dependencies injected into states**, like that:
```C#
private void OnEnable()  
{  
	appStateManager = new StateMachine<ApplicationState>(new State<ApplicationState>[]  
	{
		new AppIntroState()  
	});
	 appStateManager.Run();  
}
```
**This functionality is not supported.**
	

### Multi-level injection
This chapter will show you how to accomplish pretty tricky and a bit hard task, which occasionally might be necessary in your project: multi-level injection.

#### The problem
Let's assume, that you have prefab("Enemy" for example) created dynamically on the scene via factory, and this prefab itself is also separate GameObject context, with additional installer, maybe different resolving modules. But this prefab is not self-sufficient like in described in one of previous chapters, and requires some dependencies from parent-level dependency resolvers. 

#### Solution
The proper way of multi-level injection is to use into-installer injection approach.
So, if your goal is to inject a dependency defined in parent-level scope(for example in highest scope - scene scope) into object inside separated, bottom-level scope (for example it'll be some component inside our "Enemy" prefab game object scope), all you have to do is re-bind dependency coming from parent-level dependency resolver.
With this approach you can simply re-bind dependency defined already in higher level scope and use it in your lower level scope.
In short: if scene ViewManager system is installed in SceneResolver and you want it to be injected inside component attached to an child-object in "Enemy" prefab GameObject scope hierarchy, you should apply re-binding techniquie in one of yours "Enemy" prefab GameObject scope installers, as following:
```C#
//this should come from parent-resolver  
var parentVM = ResolveInParent<ViewManager>(inseminatorDependencyResolver.Parent);  
inseminatorDependencyResolver.Bind(parentVM);
```
To sum up:
you can explicitly resolve dependency of wanted type from parent-level resolvers by calling ResolveInParent\<T> method, available in every Installer deriving from InseminatorInstaller base class.
Then, you can bind this freshly resolved dependency inside this installer, so it'll become available for injecting inside this separated scope.
