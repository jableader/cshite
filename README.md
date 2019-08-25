# Nothing more than a uni assignment
## General design guidelines
### Architecture
I have seperated out three main pieces of functionality

#### Display & Validation
Handled by the classes within the UI folder. These classes should be loosly coupled to the banking application and should be reusable for any UI-heavy console app.

#### Banking operations
Handled by the classes within the Model folder. These classes need not know they're a console application and should be usable from a Winforms or Webserver app.

#### Implementation specific
Handled by the Program class. This uses the APIs provided to actually implement the desired functionality. This is happy to know it's banking console app.

### State
State is a necessary evil. Each 'unit' of state a class exposes exponentially increases the complexity of the class (since you have increased the number of potential data configurations).
  I opt for immutability where possible and concede where it's more practical or efficient to utilize mutability.
  + This is why const/readonly is prevalent throughout this program (if something doesn't need to change, don't let it!). 
  + Methods that don't need state are generally made static, this makes it obvious to know which methods DO mutate state. Sometimes I pass field values as args into a method where I could make it non-static and access the field directly, personally I prefer this approach since it makes it most explicit what data an operation requires.
  
### Comments
Good code should be easily reabable and self documenting. Many short, well-named methods are usually better than large multi-operation methods with many comments.
  + All public methods should include a comment on their intended operation, code within that method however should be readable without heavy commenting.
  + It is reasonable to assume the reader of the code is familiar with the language's syntax and standard libraries, so they should not require additional comments.
  + Enums are a rockstar, any multi-state choice should probably be represented with an enum (even if a bool would work, an enum provides better semantic meaning)

### Struct vs Class
I have used a class where I do not wish state to be mutable externally once passed into a given function. Otherwise I have used a class.
  + I understand in some cases by wrapping them in arrays I lose this benefit, but deemed the cost of a mem-copy overkill.

### Access modifiers
I've taken a pretty straightforward "provide mimimum access required for this class's API to be useful"
  + Public: These are the 'proper' way to access api methods. Public methods are callable anywhere, so I tried to minimise oppertunity for devs to shoot themselves in the foot by providing xml comments as documentation on how to use APIs.
  + Internal: For module-specific functionality. Lets just pretend 'internal' is per namespace because it feels overkill to actually move each folder into its own seperate project.
  + Protected: Certain helpers and subfunctions need only be accessed by subclasses, protected is perfect for this
  + Private: Default access level (you won't find any private keywords here). If it has no reason to be accessed by other classes I leave it as default