# DialogueScript
Simple visual novel-ish game script for UniGDC games.

## Language Specification
There are four components in this script language.

### Say Statements
```DialogueScript
// Usage:
<character>: "<content>"
```
This is the most basic part of the language. To make a dialogue, use this `Say Statement`.
If you use this statement, the `<character>` will say `<content>` in your host program.
Note that `<character>` object must be defined and registered by host program in order to produce dialogues.

For example:
```DialogueScript
hibiki: "Hello, world."

// Given that character object hibiki is declared before,
// Character Hibiki will say "Hello, world.".
```

### Set Statements
```DialogueScript
// Usage:
set <property> <value>
```
To set the value of property, use `Set Statement`. This does basically the same thing as `<property> = <value>;` in conventional C-like languages.
Note that `<property>` must be declared and registered before the execution in order to change the value properly.
`<property>` can be a single variable, or a property of certain class, if you mix this statement with `Of Clause`.


For example:
```DialogueScript
set age 5

// This statement will set the variable age 5.
// For now, you cannot declare a variable within the script.
```

### Of Clauses
```DialogueScript
// Usage:
<property> of <object>
```
This clause as whole is a single property. It means that this clause can be an argument of `Set Statements`, as described above.
This clause returns the property of an object. Basically the same as `<object>.<property>` in conventional C-like languages.


For example:
```DialogueScript
set pose of hibiki "doyagao"

// Given that character object hibiki is declared and hibiki has pose property,
// It will change the pose of Hibiki.
```

### Pause Statements
```DialogueScript
// Usage:
pause [<milliseconds>]
```
This will pause the script for a given amount of time.
If argument `<milisecond>` is not present, then the game will wait for user interaction even if the skip mode is enabled.

For example:
```DialogueScript
pause 1000

// This script will pause the progress for 1000 milliseconds, which is basically 1 second.
```
